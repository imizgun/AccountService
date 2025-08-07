using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<DeleteAccountCommand, bool>
{
    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account == null) return false;

        account.Close();

        return await accountRepository.CloseAccountAsync(account, cancellationToken);
    }
}