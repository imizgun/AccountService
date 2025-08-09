using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandHandler(ITransactionRepository transactionRepository) : IRequestHandler<DeleteTransactionCommand, bool>
{
    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        return await transactionRepository
            .DeleteTransactionByIdAsync(request.TransactionId, cancellationToken, request.Xmin);
    }
}