
using AccountService.Application.Shared.Domain.Abstraction;

namespace AccountService.Application.Features.Accounts.Domain;
// ReSharper disable once IdentifierTypo
public interface IAccountRepository : IBaseRepository<Account>
{
    Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken);
    Task<bool> ValidateAccountBalanceAsync(Guid accountId, decimal expectedAmount, CancellationToken cancellationToken);
}