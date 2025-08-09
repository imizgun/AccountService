using System.Data;
using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
// ReSharper disable StringLiteralTypo

namespace AccountService.DatabaseAccess.Repositories;

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

    public async Task<bool> DeleteTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken, uint xmin)
    {
        var exists = await DbSet.AsNoTracking().AnyAsync(x => x.Id == transactionId, cancellationToken);
        
        if (!exists) return false;
        
        var res = await DbSet
            .Where(x => x.Id == transactionId && EF.Property<uint>(x, "xmin") == xmin)
            .ExecuteUpdateAsync(s => 
            s.SetProperty(x => x.IsDeleted, true), 
            cancellationToken);

        if (res == 0) throw new DBConcurrencyException("Update failed due to concurrency conflict. The transaction may have been modified by another process.");

        return res > 0;
    }

    public async Task<bool> UpdateTransactionAsync(Guid id, string description, CancellationToken cancellationToken, uint xmin)
    {
        var exists = await DbSet.AsNoTracking().AnyAsync(x => x.Id == id, cancellationToken);
        
        if (!exists) return false;
        
        var res = await DbSet
            // ReSharper disable once StringLiteralTypo
            .Where(x => x.Id == id && EF.Property<uint>(x, "xmin") == xmin)
            .ExecuteUpdateAsync(s => 
            s.SetProperty(x => x.Description, description), 
            cancellationToken);

        if (res == 0) throw new DBConcurrencyException("Update failed due to concurrency conflict. The transaction may have been modified by another process.");

        return res > 0;
    }
}