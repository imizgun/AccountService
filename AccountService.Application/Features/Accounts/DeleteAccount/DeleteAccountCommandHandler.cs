using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand, bool>
{
    private readonly IAccountRepository _accountRepository;

    public DeleteAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account == null) return false;

        account.Close();

        return await _accountRepository.CloseAccountAsync(account, cancellationToken);
    }
}