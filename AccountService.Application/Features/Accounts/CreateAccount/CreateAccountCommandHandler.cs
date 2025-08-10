using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using AccountService.Core.Domain.Enums;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateAccountCommand, Guid>
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

        var res = await accountRepository.CreateAsync(newAccount, cancellationToken);
        var save = await unitOfWork.SaveChangesAsync(cancellationToken);
        return save > 0 ? res : Guid.Empty;
    }
}