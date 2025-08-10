using System.Data;
using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;
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