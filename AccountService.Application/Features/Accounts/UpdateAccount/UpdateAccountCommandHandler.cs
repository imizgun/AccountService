using AccountService.Core.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandler : IRequestHandler<UpdateAccountCommand, bool>
{
    private IAccountRepository _accountRepository;

    public UpdateAccountCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);

        if (account == null) return false;

        account.InterestRate = request.InterestRate;

        return await _accountRepository.UpdateInterestRate(account, cancellationToken);
    }
}