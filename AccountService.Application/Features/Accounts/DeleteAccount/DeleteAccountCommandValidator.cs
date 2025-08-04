using FluentValidation;

namespace AccountService.Application.Features.Accounts.DeleteAccount;

// ReSharper disable once UnusedMember.Global Валидатор неявно используется в Пайплайне
public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .WithMessage("Account ID must not be empty.");
    }
}