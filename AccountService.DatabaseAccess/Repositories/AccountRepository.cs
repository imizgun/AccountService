using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;

public class AccountRepository(AccountServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await DbSet.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return account;
    }

    public async Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken)
    {
        return await DbSet.Where(a => a.OwnerId == ownerId).ToListAsync(cancellationToken);
    }

    public async Task<bool> CloseAccountAsync(Account account, CancellationToken cancellationToken)
    {
        var obj = await DbSet.FirstOrDefaultAsync(x => x.Id == account.Id, cancellationToken);

        if (obj == null) return false;

        obj.ClosingDate = account.ClosingDate;

        var res = await DbSet.ExecuteUpdateAsync(s => 
            s.SetProperty(a => a.ClosingDate, account.ClosingDate), cancellationToken);

        return res > 0;
    }

    public async Task<bool> UpdateAccount(Account account, CancellationToken cancellationToken)
    {
        var obj = DbSet.FirstOrDefault(x => x.Id == account.Id);

        if (obj == null) return await Task.FromResult(false);

        obj.InterestRate = account.InterestRate;
        obj.Balance = account.Balance;

        var res = await DbSet.ExecuteUpdateAsync(s => 
            s.SetProperty(a => a.InterestRate, account.InterestRate), cancellationToken);

        return res > 0;
    }
}