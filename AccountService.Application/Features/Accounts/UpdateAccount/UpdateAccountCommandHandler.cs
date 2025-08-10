using AccountService.Core.Domain.Abstraction;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateAccountCommand, bool>
{
    public async Task<bool> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken);

        if (account == null) return false;

        account.InterestRate = request.InterestRate;

        return await unitOfWork.SaveChangesAsync(cancellationToken) == 1;
    }
}