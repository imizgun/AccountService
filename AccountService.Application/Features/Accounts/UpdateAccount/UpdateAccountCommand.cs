using MediatR;

namespace AccountService.Application.Features.Accounts.UpdateAccount;

public record UpdateAccountCommand(Guid AccountId, decimal? InterestRate) : IRequest<bool>;