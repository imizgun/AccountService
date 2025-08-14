using AccountService.Core.Abstraction;

namespace AccountService.Core.Features.Accounts;
// ReSharper disable once IdentifierTypo
public interface IAccountRepository : IBaseRepository<Account>
{
    Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken);
    Task<bool> ValidateAccountBalanceAsync(Guid accountId, decimal expectedAmount, CancellationToken cancellationToken);
}