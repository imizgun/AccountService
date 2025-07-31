using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, bool>
{
    private readonly ITransactionRepository _transactionRepository;

    public UpdateTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<bool> Handle(UpdateTransactionCommand request, CancellationToken cancellationToken)
    {
        return await _transactionRepository.UpdateTransactionAsync(request.TransactionId, request.Description, cancellationToken);
    }
}