using AccountService.Application.Features.DTOs;
using AccountService.Core.Domain.Abstraction;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Transactions.GetTransactions;

public class GetTransactionsCommandHandler : IRequestHandler<GetTransactionsCommand, List<TransactionDto>>
{
    private ITransactionRepository _transactionRepository;
    private IMapper _mapper;

    public GetTransactionsCommandHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<List<TransactionDto>> Handle(GetTransactionsCommand request, CancellationToken cancellationToken)
    {
        var res = await _transactionRepository
            .GetAllFromAccountAsync(
                request.AccountId,
                request.SkipPage,
                request.Take, cancellationToken);

        return _mapper.Map<List<TransactionDto>>(res);
    }
}