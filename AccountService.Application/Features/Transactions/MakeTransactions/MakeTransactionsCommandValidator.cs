using AccountService.Application.Services.Abstractions;
using AccountService.Core.Domain.Abstraction;
using FluentValidation;

namespace AccountService.Application.Features.Transactions.MakeTransactions;

public class MakeTransactionsCommandValidator : AbstractValidator<MakeTransactionCommand>
{
    public MakeTransactionsCommandValidator(ICurrencyService currencyService, IAccountRepository accountRepository)
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MustAsync(async (x, token) => await accountRepository.ExistsAsync(x, token))
            .WithMessage("Account not found");

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.Currency)
            .NotEmpty()
            .Must(currencyService.IsValidCurrency)
            .WithMessage("Currency must be a 3-letter ISO code.");

        RuleFor(x => x.CounterpartyAccountId)
            .MustAsync(async (id, token) =>
            {
                if (id is not null)
                    return await accountRepository.ExistsAsync(id ?? Guid.Empty, token);
                return true;
            })
            .WithMessage("Counterparty account does not exist")
            .MustAsync(async (x, token) =>
            {
                var account = await accountRepository.GetByIdAsync(x ?? Guid.Empty, token);
                return account?.ClosingDate is null;
            })
            .WithMessage("You cannot make transactions to closed account");

        RuleFor(x => x.AccountId)
            .MustAsync(async (x, token) =>
            {
                var account = await accountRepository.GetByIdAsync(x, token);
                return account?.ClosingDate is null;
            })
            .WithMessage("You cannot make transactions from closed account");

    }
}