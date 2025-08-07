using AccountService.Core.Domain.Entities;

namespace AccountService.Core.Domain.Abstraction;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<bool> DeleteTransactionByIdAsync(Guid transactionId, CancellationToken cancellationToken);
    Task<bool> UpdateTransactionAsync(Guid id, string description, CancellationToken cancellationToken);
}