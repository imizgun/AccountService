using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;

namespace AccountService.DatabaseAccess.Repositories;

public class AccountRepository : BaseRepository<Account>, IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = DbSet.FirstOrDefault(a => a.Id == id);

        return await Task.FromResult(account);
    }

    public async Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken)
    {
        return await Task.FromResult(DbSet.Where(a => a.OwnerId == ownerId).ToList());
    }

    public async Task<bool> CloseAccountAsync(Account account, CancellationToken cancellationToken)
    {
        var res = DbSet.FirstOrDefault(x => x.Id == account.Id);

        if (res == null) return await Task.FromResult(false);

        res.ClosingDate = account.ClosingDate;

        // .ExecuteUpdateAsync(s => 
        //     s.SetProperty(a => a.ClosingDate, account.ClosingDate), cancellationToken);

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateInterestRate(Account account, CancellationToken cancellationToken)
    {
        var res = DbSet.FirstOrDefault(x => x.Id == account.Id);

        if (res == null) return await Task.FromResult(false);

        res.InterestRate = account.InterestRate;

        // .ExecuteUpdateAsync(s => 
        //     s.SetProperty(a => a.InterestRate, account.InterestRate), cancellationToken);

        return await Task.FromResult(true);
    }
}