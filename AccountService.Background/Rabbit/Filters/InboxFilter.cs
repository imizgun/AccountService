using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;

namespace AccountService.Background.Rabbit.Filters;

using MassTransit;
using Microsoft.EntityFrameworkCore;

public class InboxFilter<T>(
    IInboxConsumedRepository inboxConsumedRepository,
    IUnitOfWork unitOfWork) : IFilter<ConsumeContext<T>> where T : class
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var messageId = context.MessageId ?? TryGetEventId(context.Message) ?? Guid.Empty;

        if (messageId == Guid.Empty)
        {
            await next.Send(context);
            return;
        }

        await unitOfWork.BeginTransactionAsync(context.CancellationToken);

        var handled = await inboxConsumedRepository.ExistsAsync(messageId, context.CancellationToken);

        if (handled)
        {
            await unitOfWork.RollbackAsync(context.CancellationToken);
            return;
        }

        await next.Send(context);

        await inboxConsumedRepository.AddAsync(new InboxConsumed
        {
            MessageId = messageId,
            Handler = typeof(T).Name,
            ProcessedAt = DateTime.UtcNow
        });

        try
        {
            await unitOfWork.SaveChangesAsync(context.CancellationToken);
            await unitOfWork.CommitAsync(context.CancellationToken);
        }
        catch (DbUpdateException e) when (IsUniqueViolation(e))
        {
            await unitOfWork.RollbackAsync(context.CancellationToken);
        }
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope("inbox");

    private static Guid? TryGetEventId(object message)
    {
        var prop = message.GetType().GetProperty("EventId");
        if (prop?.GetValue(message) is Guid g && g != Guid.Empty) return g;
        return null;
    }

    private static bool IsUniqueViolation(DbUpdateException e)
        => e.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true;
}
