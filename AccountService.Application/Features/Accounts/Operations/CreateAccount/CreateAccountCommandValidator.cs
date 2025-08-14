using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Shared.Services.Abstractions;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.Operations.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator(ICurrencyService currencyService)
    {
        RuleFor(x => x.AccountType)
            .NotEmpty()
            .WithMessage("AccountType cannot be empty")
            .Must(BeValidAccountType)
            .WithMessage("AccountType must be a valid enum value");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .WithMessage("Currency cannot be empty")
            .Must(currencyService.IsValidCurrency)
            .WithMessage("Currency must be a valid currency");
    }

    private static bool BeValidAccountType(string accountType)
    {
        return Enum.TryParse<AccountType>(accountType, true, out _);
    }
}