using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Shared.DatabaseAccess;
using AccountService.Application.Shared.DatabaseAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Features.Accounts.DatabaseAccess;
// ReSharper disable once IdentifierTypo

public class AccountRepository(AccountServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken) =>
        await DbSet.AsNoTracking()
            .Where(a => a.OwnerId == ownerId)
            .ToListAsync(cancellationToken);

    public async Task<bool> ValidateAccountBalanceAsync(Guid accountId, decimal expectedAmount, CancellationToken cancellationToken)
    {
        var balance = await DbSet.AsNoTracking()
            .Where(x => x.Id == accountId)
            .Select(x => (decimal?)x.Balance)
            .SingleOrDefaultAsync(cancellationToken);

        return balance == expectedAmount;
    }
}