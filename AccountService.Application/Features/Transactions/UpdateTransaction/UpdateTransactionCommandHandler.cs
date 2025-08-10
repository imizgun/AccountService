using AccountService.Core.Domain.Abstraction;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler(ITransactionRepository transactionRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionCommand, bool>
{
    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdForUpdateAsync(request.TransactionId, cancellationToken);
        
        if (transaction is null)
        {
            return false;
        }
        
        transaction.Description = request.Description;

        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1;
    }
}