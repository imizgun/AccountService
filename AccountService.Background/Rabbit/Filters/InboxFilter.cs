using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using Microsoft.Extensions.Logging;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Background.Rabbit.Filters;

public class InboxFilter<T>(
    IInboxConsumedRepository inboxConsumedRepository,
    IUnitOfWork unitOfWork,
    ILogger<InboxFilter<T>> logger) : IFilter<ConsumeContext<T>> where T : DefaultEvent
{
    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        logger.LogInformation("[InboxFilter] Processing message");
        var messageId = context.Message.EventId != Guid.Empty
            ? context.Message.EventId
            : context.MessageId ?? Guid.Empty;

        await unitOfWork.BeginTransactionAsync(context.CancellationToken);

        try
        {
            var handled = await inboxConsumedRepository.ExistsAsync(messageId, context.CancellationToken);

            if (handled)
            {
                logger.LogWarning("Message with ID {MessageId} has already been processed by handler {HandlerName}. Skipping processing.",
                    messageId, typeof(T).Name);

                await unitOfWork.CommitAsync(context.CancellationToken);
                return;
            }

            logger.LogInformation("Message with ID {MessageId} wasn't processed yet. Processing with handler {HandlerName}.",
                messageId, typeof(T).Name);

            await next.Send(context);

            await inboxConsumedRepository.AddAsync(new InboxConsumed
            {
                MessageId = messageId,
                Handler = typeof(T).Name,
                ProcessedAt = DateTime.UtcNow
            });

            await unitOfWork.SaveChangesAsync(context.CancellationToken);
            await unitOfWork.CommitAsync(context.CancellationToken);

            logger.LogInformation("[InboxFilter] Message {MessageId} processed successfully", messageId);
        }
        catch (DbUpdateException e) when (IsUniqueViolation(e))
        {
            // Дубликат - это нормально, откатываем и считаем успешным
            logger.LogInformation("Duplicate message {MessageId} detected, treating as already processed", messageId);
            await unitOfWork.RollbackAsync(context.CancellationToken);
        }
        catch (Exception ex)
        {
            // Любая другая ошибка - откатываем и пробрасываем
            logger.LogError(ex, "Error processing message {MessageId}", messageId);
            await unitOfWork.RollbackAsync(context.CancellationToken);
            throw;
        }
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope("inbox");

    private static bool IsUniqueViolation(DbUpdateException e)
        => e.InnerException?.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) == true;
}