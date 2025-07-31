using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, bool>
{
    private readonly ITransactionRepository _transactionRepository;

    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<bool> Handle(DeleteTransactionCommand request, CancellationToken cancellationToken)
    {
        return await _transactionRepository
            .DeleteTransactionByIdAsync(request.TransactionId, cancellationToken);
    }

}