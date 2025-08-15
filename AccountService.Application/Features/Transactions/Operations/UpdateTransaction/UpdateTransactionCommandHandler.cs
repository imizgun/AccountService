using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Features.Transactions.Events;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.UpdateTransaction;


public class UpdateTransactionCommandHandler(
    ITransactionRepository transactionRepository, 
    IUnitOfWork unitOfWork,
    IOutboxMessageRepository outboxMessageRepository) : IRequestHandler<UpdateTransactionCommand, bool>
{
    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken) 
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try {
            var transaction =
                await transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);

            if (transaction is null)
                throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found");

            transaction.Description = request.Description;
            var transactionUpdateEvent = new TransactionUpdated(
                Guid.NewGuid(),
                DateTime.UtcNow,
                new Meta(request.CorrelationId),
                request.TransactionId,
                transaction.Description);

            await outboxMessageRepository.AddAsync(new OutboxMessage(transactionUpdateEvent), cancellationToken);

            return await unitOfWork.SaveChangesAsync(cancellationToken) == 2
                ? true
                : throw new InvalidOperationException(
                    $"Error while updating transaction with ID {request.TransactionId}");
        }
        catch {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}