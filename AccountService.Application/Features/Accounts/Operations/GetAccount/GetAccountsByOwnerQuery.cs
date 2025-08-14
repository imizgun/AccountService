using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.GetAccount;

public record GetAccountsByOwnerQuery(Guid OwnerId) : IRequest<List<AccountDto>>;