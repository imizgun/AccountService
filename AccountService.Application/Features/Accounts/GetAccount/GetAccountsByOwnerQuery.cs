﻿using AccountService.Application.Features.DTOs;
using MediatR;

namespace AccountService.Application.Features.Accounts.GetAccount;

public record GetAccountsByOwnerQuery(Guid OwnerId) : IRequest<List<AccountDto>>;