using AccountService.Application.DTOs;
using AccountService.Core.Domain.Abstraction;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Accounts.GetAccount;

public class GetAccountByOwnerQueryHandler(IAccountRepository accountRepository, IMapper mapper) : IRequestHandler<GetAccountsByOwnerQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var res = await accountRepository.GetAllOwnerAccounts(request.OwnerId, cancellationToken);

        return mapper.Map<List<AccountDto>>(res);
    }
}