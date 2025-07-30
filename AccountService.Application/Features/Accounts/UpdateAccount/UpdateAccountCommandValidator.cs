using FluentValidation;

namespace AccountService.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();

        RuleFor(x => x.InterestRate)
            .Must(x => x >= 0);
    }
}