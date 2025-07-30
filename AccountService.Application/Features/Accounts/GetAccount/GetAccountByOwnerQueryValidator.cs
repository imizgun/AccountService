using AccountService.Application.Services.Abstractions;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.GetAccount;

public class GetAccountByOwnerQueryValidator : AbstractValidator<GetAccountsByOwnerQuery> {
	private IClientService _clientService;
	
	public GetAccountByOwnerQueryValidator(IClientService clientService) {
		_clientService = clientService;

		RuleFor(x => x.OwnerId)
			.NotEmpty()
			.Must(x => _clientService.IsClientExists(x));
	}
}