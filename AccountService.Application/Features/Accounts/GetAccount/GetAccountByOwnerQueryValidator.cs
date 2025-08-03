using AccountService.Application.Services.Abstractions;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.GetAccount;

public class GetAccountByOwnerQueryValidator : AbstractValidator<GetAccountsByOwnerQuery>
{
    public GetAccountByOwnerQueryValidator(IClientService clientService)
    {
        // Поскольку юзеры проверяются теперь из JWT, это излишне
        // RuleFor(x => x.OwnerId)
        //     .NotEmpty()
        //     .WithMessage("Owner ID must not be empty.")
        //     .Must(x => clientService.IsClientExists(x))
        //     .WithMessage("Owner with the specified ID does not exist.");
    }
}