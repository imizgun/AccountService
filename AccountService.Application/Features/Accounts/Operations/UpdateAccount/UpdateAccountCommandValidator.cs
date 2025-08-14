using FluentValidation;

namespace AccountService.Application.Features.Accounts.Operations.UpdateAccount;

// ReSharper disable once UnusedMember.Global Валидатор неявно используется в Пайплайне
public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommand>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID must not be empty.");
    }
}