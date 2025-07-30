using AccountService.Application.Services.Abstractions;
using AccountService.Core.Domain.Enums;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommand>
{
    private ICurrencyService _currencyService;
    private IClientService _clientService;

    public CreateAccountCommandValidator(ICurrencyService currencyService, IClientService clientService)
    {
        _currencyService = currencyService;
        _clientService = clientService;

        RuleFor(x => x.OwnerId)
            .NotEmpty();

        RuleFor(x => x.AccountType)
            .NotEmpty()
            .Must(BeValidAccountType);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(_currencyService.IsValidCurrency);

        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .Must(x => _clientService.IsClientExists(x));
    }

    private bool BeValidAccountType(string accountType)
    {
        return Enum.TryParse<AccountType>(accountType, true, out var account);
    }
}