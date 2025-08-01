using AccountService.Application.Services.Abstractions;
using AccountService.Core.Domain.Enums;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountCommandValidator(ICurrencyService currencyService, IClientService clientService)
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

        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("OwnerId cannot be empty")
            .Must(clientService.IsClientExists)
            .WithMessage("Owner does not exist");
    }

    private static bool BeValidAccountType(string accountType)
    {
        return Enum.TryParse<AccountType>(accountType, true, out _);
    }
}