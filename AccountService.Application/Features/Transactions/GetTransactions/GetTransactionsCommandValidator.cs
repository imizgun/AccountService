using FluentValidation;

namespace AccountService.Application.Features.Transactions.GetTransactions;

public class GetTransactionsCommandValidator : AbstractValidator<GetTransactionsCommand>
{
    public GetTransactionsCommandValidator()
    {
        RuleFor(t => t.AccountId)
            .NotEmpty()
            .WithMessage("Account ID cannot be empty.");

        RuleFor(t => t.Take)
            .GreaterThan(0)
            .WithMessage("Take must be greater than 0.");

        RuleFor(t => t.SkipPage)
            .Must(x => x >= 0)
            .WithMessage("Skip page must be greater or equal 0.");
    }
}