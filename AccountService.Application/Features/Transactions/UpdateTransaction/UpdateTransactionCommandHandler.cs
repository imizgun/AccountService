using AccountService.Core.Features.Transactions;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionCommand, bool>
{
    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);
        
        if (transaction is null) throw new KeyNotFoundException($"Transaction with ID {request.TransactionId} not found");
        
        transaction.Description = request.Description;

        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1 ? true : throw new InvalidOperationException($"Error while updating transaction with ID {request.TransactionId}");
    }
}