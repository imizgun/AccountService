using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Features.Transactions.Events;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.DeleteTransaction;

public class DeleteTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    IOutboxMessageRepository outboxMessageRepository) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var transaction =
                await transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);

            if (transaction == null)
                throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found");

            if (transaction.IsDeleted) return true;

            transaction.IsDeleted = true;
            var deleteTransactionEvent = new TransactionDeleted(
                Guid.NewGuid(),
                DateTime.UtcNow,
                new Meta(request.CorrelationId),
                transaction.Id
            );

            await outboxMessageRepository.AddAsync(new OutboxMessage(deleteTransactionEvent), cancellationToken);

            return await unitOfWork.SaveChangesAsync(cancellationToken) == 2
                ? true
                : throw new InvalidOperationException("Failed to delete transaction");
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}