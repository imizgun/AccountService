using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using AccountService.Core.Domain.Enums;
using MediatR;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AccountType>(request.AccountType, true, out var type))
            throw new Exception("Invalid account type");

        var newAccount = Account.Create(
            request.OwnerId,
            type,
            request.Currency,
            request.InterestRate
            );

        return await accountRepository.CreateAsync(newAccount, cancellationToken);
    }
}