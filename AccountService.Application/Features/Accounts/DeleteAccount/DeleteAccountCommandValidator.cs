using FluentValidation;

namespace AccountService.Application.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandValidator : AbstractValidator<DeleteAccountCommand>
{
    public DeleteAccountCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty();
    }
}