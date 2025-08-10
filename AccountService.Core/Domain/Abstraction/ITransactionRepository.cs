using AccountService.Core.Domain.Entities;

namespace AccountService.Core.Domain.Abstraction;
// ReSharper disable once IdentifierTypo
public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}