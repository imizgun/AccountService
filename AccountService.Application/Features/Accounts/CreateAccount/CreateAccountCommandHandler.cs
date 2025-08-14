using AccountService.Core.Features.Accounts;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateAccountCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<AccountType>(request.AccountType, true, out var type))
            throw new ArgumentException("Invalid account type");

        var newAccount = Account.Create(
            request.OwnerId,
            type,
            request.Currency,
            request.InterestRate
            );

        var res = await accountRepository.CreateAsync(newAccount, cancellationToken);
        var save = await unitOfWork.SaveChangesAsync(cancellationToken);
        return save == 1 ? res : throw new InvalidOperationException("Failed to create account");
    }
}