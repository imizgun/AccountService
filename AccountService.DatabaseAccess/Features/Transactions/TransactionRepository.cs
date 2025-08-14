using AccountService.Core.Features.Transactions;
using AccountService.DatabaseAccess.Repositories;
using Microsoft.EntityFrameworkCore;
// ReSharper disable StringLiteralTypo

namespace AccountService.DatabaseAccess.Features.Transactions;

// ReSharper disable once IdentifierTypo
public class TransactionRepository(AccountServiceDbContext context) : BaseRepository<Transaction>(context), ITransactionRepository
{
    public async Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var res = await DbSet.Where(t => t.AccountId == accountId)
            .AsNoTracking()
            .OrderByDescending(t => t.TransactionDate)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return res;
    }
}