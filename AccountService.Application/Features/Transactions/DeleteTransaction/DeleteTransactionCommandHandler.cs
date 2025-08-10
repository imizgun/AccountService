using AccountService.Core.Domain.Abstraction;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);
        
        if (transaction == null)
            return false;
        
        if (transaction.IsDeleted) return true;
        
        transaction.IsDeleted = true;
        
        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1;
    }
}