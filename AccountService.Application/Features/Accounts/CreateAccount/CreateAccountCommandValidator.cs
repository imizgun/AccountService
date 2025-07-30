using AccountService.Application.Services.Abstractions;
using AccountService.Core.Domain.Enums;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand> {
	private ICurrencyService _currencyService;
	
	public CreateAccountCommandValidator(ICurrencyService currencyService) {
		_currencyService = currencyService;
		
		RuleFor(x => x.OwnerId)
			.NotEmpty();

		RuleFor(x => x.AccountType)
			.NotEmpty()
			.Must(BeValidAccountType);

		RuleFor(x => x.Currency)
			.NotEmpty()
			.Must(_currencyService.IsValidCurrency);
	}
	
	private bool BeValidAccountType(string accountType) {
		return Enum.TryParse<AccountType>(accountType, true, out var account);
	}
}