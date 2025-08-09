using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandler(IAccountRepository accountRepository) : IRequestHandler<UpdateAccountCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account == null) return false;

        account.InterestRate = request.InterestRate;

        return await accountRepository.UpdateAccount(account, cancellationToken);
    }
}