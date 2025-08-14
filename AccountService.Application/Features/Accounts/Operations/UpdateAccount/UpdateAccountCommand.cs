using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.UpdateAccount;

public record UpdateAccountCommand(Guid AccountId, decimal? InterestRate) : IRequest<bool>;