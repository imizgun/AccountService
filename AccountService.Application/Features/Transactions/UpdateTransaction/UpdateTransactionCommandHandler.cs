using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler(ITransactionRepository transactionRepository) : IRequestHandler<UpdateTransactionCommand, bool>
{
    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        return await transactionRepository.UpdateTransactionAsync(request.TransactionId, request.Description, cancellationToken, request.Xmin);
    }
}