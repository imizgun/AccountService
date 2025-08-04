using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;

namespace AccountService.DatabaseAccess.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
    public async Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var res = DbSet.Where(t => t.AccountId == accountId)
            .OrderByDescending(t => t.TransactionDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return await Task.FromResult(res);
    }

    public async Task<bool> DeleteTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken)
    {
        var transaction = DbSet.FirstOrDefault(x => x.Id == transactionId);

        if (transaction == null) return await Task.FromResult(false);

        transaction.IsDeleted = true;

        return await Task.FromResult(true);
    }

    public async Task<bool> UpdateTransactionAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        var transact = DbSet.FirstOrDefault(x => x.Id == id);

        if (transact == null) return await Task.FromResult(false);

        transact.Description = description;

        return await Task.FromResult(true);
    }
}