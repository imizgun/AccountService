using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;

public class TransactionRepository(AccountServiceDbContext context) : BaseRepository<Transaction>(context), ITransactionRepository
{
    public async Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var res = await DbSet.Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return res;
    }

    public async Task<bool> DeleteTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        var transaction = await DbSet.FirstOrDefaultAsync(x => x.Id == transactionId, cancellationToken);

        if (transaction == null) return false;

        var res = await DbSet.ExecuteUpdateAsync(s => 
            s.SetProperty(x => x.IsDeleted, true), 
            cancellationToken);

        return res > 0;
    }

    public async Task<bool> UpdateTransactionAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        var transact = await DbSet.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (transact == null) return false;

        transact.Description = description;
        
        var res = await DbSet.ExecuteUpdateAsync(s => 
            s.SetProperty(x => x.Description, description), 
            cancellationToken);

        return res > 0;
    }
}