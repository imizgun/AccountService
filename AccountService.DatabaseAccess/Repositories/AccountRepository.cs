using System.Data;
using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;

public class AccountRepository(AccountServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await DbSet.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return account;
    }

    public async Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken)
    {
        return await DbSet.AsNoTracking().Where(a => a.OwnerId == ownerId).ToListAsync(cancellationToken);
    }

    public async Task<bool> CloseAccountAsync(Account account, CancellationToken cancellationToken, uint xmin)
    {
        var exists = await DbSet.AsNoTracking().AnyAsync(x => x.Id == account.Id, cancellationToken);
        
        if (!exists) return false;
        
        var res = await DbSet
            .Where(ac => ac.Id == account.Id && EF.Property<uint>(ac, "xmin") == xmin)
            .ExecuteUpdateAsync(s => 
            s.SetProperty(a => a.ClosingDate, account.ClosingDate), cancellationToken);
        
        if (res == 0) throw new DBConcurrencyException("Update failed due to concurrency conflict. The account may have been modified by another transaction.");

        return res > 0;
    }

    public async Task<bool> UpdateAccount(Account account, CancellationToken cancellationToken)
    {
        var res = await DbSet
            .Where(ac => ac.Id == account.Id)
            .ExecuteUpdateAsync(s => 
            s.SetProperty(a => a.InterestRate, account.InterestRate).SetProperty(x => x.Balance, account.Balance), 
            cancellationToken);

        return res > 0;
    }

    public async Task<bool> ValidateAccountBalanceAsync(Guid accountId, decimal expectedAmount, CancellationToken cancellationToken) 
    {
        var balance = await DbSet.AsNoTracking()
            .Where(x => x.Id == accountId)
            .Select(x => (decimal?)x.Balance)
            .SingleOrDefaultAsync(cancellationToken);

        return balance == expectedAmount;
    }
}