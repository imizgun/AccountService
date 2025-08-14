using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.UpdateAccount;

public class UpdateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateAccountCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken);

        if (account == null) throw new InvalidOperationException("Account is already closed");

        account.InterestRate = request.InterestRate;

        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1
            ? true
            : throw new InvalidOperationException("Error while updating account");
    }
}