using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Accounts.Operations.DeleteAccount;

public class DeleteAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken);

        if (account == null) throw new KeyNotFoundException("Account not found");

        if (account.ClosingDate != null) throw new InvalidOperationException("Account is already closed");

        account.Close();

        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1
            ? true
            : throw new InvalidOperationException("Error while closing account");
    }
}