using System.Runtime.InteropServices;
using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;

public class AccountRepository(AccountServiceDbContext context) : BaseRepository<Account>(context), IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await Context.Accounts
            .AsNoTracking()
            .Include(a => a.Transactions)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return account;
    }

    public async Task<List<Account>> GetAllOwnerAccounts(Guid ownerId, CancellationToken cancellationToken)
    {
        return await Context.Accounts
            .Where(a => a.OwnerId == ownerId)
            .ToListAsync(cancellationToken);
    }
}