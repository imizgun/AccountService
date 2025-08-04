using FluentValidation;

namespace AccountService.Application.Features.Accounts.GetAccount;

// ReSharper disable once UnusedMember.Global Валидатор неявно используется в Пайплайне, сейчас он пустой, но позже будет использоваться для проверки прав
public class GetAccountByOwnerQueryValidator : AbstractValidator<GetAccountsByOwnerQuery>
{
    //public GetAccountByOwnerQueryValidator()
    //{
        // Поскольку юзеры проверяются теперь из JWT, это излишне
        // RuleFor(x => x.OwnerId)
        //     .NotEmpty()
        //     .WithMessage("Owner ID must not be empty.")
        //     .Must(x => clientService.IsClientExists(x))
        //     .WithMessage("Owner with the specified ID does not exist.");
    //}
}