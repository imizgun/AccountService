using AccountService.Application.Features.DTOs;
using AccountService.Core.Domain.Abstraction;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Transactions.GetTransactions;

public class GetTransactionsCommandHandler(ITransactionRepository transactionRepository, IMapper mapper) : IRequestHandler<GetTransactionsCommand, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetTransactionsCommand request, CancellationToken cancellationToken)
    {
        var res = await transactionRepository
            .GetAllFromAccountAsync(
                request.AccountId,
                request.SkipPage,
                request.Take, cancellationToken);

        return mapper.Map<List<TransactionDto>>(res);
    }
}