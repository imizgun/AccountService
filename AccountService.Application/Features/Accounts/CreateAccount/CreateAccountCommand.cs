using MediatR;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public record CreateAccountCommand(
	Guid OwnerId, 
	string Currency, 
	string AccountType, 
	decimal? InterestRate) : IRequest<Guid>;