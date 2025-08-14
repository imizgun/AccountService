using AccountService.Application.Features.Accounts.Domain;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.GetAccount;

public class GetAccountByOwnerQueryHandler(IAccountRepository accountRepository, IMapper mapper)
    : IRequestHandler<GetAccountsByOwnerQuery, List<AccountDto>>
{
    public async Task<List<AccountDto>> Handle(GetAccountsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var res = await accountRepository.GetAllOwnerAccounts(request.OwnerId, cancellationToken);

        return mapper.Map<List<AccountDto>>(res);
    }
}