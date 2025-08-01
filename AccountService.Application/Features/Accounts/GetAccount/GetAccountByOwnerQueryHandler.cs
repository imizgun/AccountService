﻿using AccountService.Application.Features.DTOs;
using AccountService.Core.Domain.Abstraction;
using AutoMapper;
using MediatR;

namespace AccountService.Application.Features.Accounts.GetAccount;

public class GetAccountByOwnerQueryHandler : IRequestHandler<GetAccountsByOwnerQuery, List<AccountDto>>
{
    private IAccountRepository _accountRepository;
    private IMapper _mapper;

    public GetAccountByOwnerQueryHandler(IAccountRepository accountRepository, IMapper mapper)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<List<AccountDto>> Handle(GetAccountsByOwnerQuery request, CancellationToken cancellationToken)
    {
        var res = await _accountRepository.GetAllOwnerAccounts(request.OwnerId, cancellationToken);

        return _mapper.Map<List<AccountDto>>(res);
    }
}