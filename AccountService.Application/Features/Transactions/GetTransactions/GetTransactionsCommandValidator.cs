using AccountService.Core.Features.Accounts;
using FluentValidation;

namespace AccountService.Application.Features.Transactions.GetTransactions;

// ReSharper disable once UnusedMember.Global Валидатор неявно используется в Пайплайне
public class GetTransactionsCommandValidator : AbstractValidator<GetTransactionsCommand>
{
    public GetTransactionsCommandValidator(IAccountRepository accountRepository)
    {
        RuleFor(t => t.AccountId)
            .NotEmpty()
            .WithMessage("Account ID cannot be empty.")
            .MustAsync(async (x, token) => await accountRepository.ExistsAsync(x, token))
            .WithMessage("Account with this ID does not exist.");

        RuleFor(t => t.Take)
            .GreaterThan(0)
            .WithMessage("Take must be greater than 0.");

        RuleFor(t => t.SkipPage)
            .Must(x => x >= 0)
            .WithMessage("Skip page must be greater or equal 0.");
    }
}