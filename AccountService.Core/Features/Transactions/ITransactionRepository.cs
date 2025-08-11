using AccountService.Core.Abstraction;

namespace AccountService.Core.Features.Transactions;
// ReSharper disable once IdentifierTypo
public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}