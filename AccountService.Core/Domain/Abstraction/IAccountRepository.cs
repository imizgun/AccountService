using AccountService.Core.Domain.Entities;

namespace AccountService.Core.Domain.Abstraction;
// ReSharper disable once IdentifierTypo
public interface IAccountRepository : IBaseRepository<Account>
{
    Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken);
    Task<bool> ValidateAccountBalanceAsync(Guid accountId, decimal expectedAmount, CancellationToken cancellationToken);
}