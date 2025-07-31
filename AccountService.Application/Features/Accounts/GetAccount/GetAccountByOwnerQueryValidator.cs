using AccountService.Application.Services.Abstractions;
using FluentValidation;

namespace AccountService.Application.Features.Accounts.GetAccount;

public class GetAccountByOwnerQueryValidator : AbstractValidator<GetAccountsByOwnerQuery>
{
    private IClientService _clientService;

    public GetAccountByOwnerQueryValidator(IClientService clientService)
    {
        _clientService = clientService;

        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Owner ID must not be empty.")
            .Must(x => _clientService.IsClientExists(x))
            .WithMessage("Owner with the specified ID does not exist.");
    }
}