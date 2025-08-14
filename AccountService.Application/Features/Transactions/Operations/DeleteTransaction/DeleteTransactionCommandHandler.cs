using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.DeleteTransaction;

public class DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);

        if (transaction == null)
            throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found");

        if (transaction.IsDeleted) return true;

        transaction.IsDeleted = true;

        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1 ? true : throw new InvalidOperationException("Failed to delete transaction");
    }
}