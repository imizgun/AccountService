using AccountService.Application.Shared.Domain.Abstraction;

namespace AccountService.Application.Features.Transactions.Domain;
// ReSharper disable once IdentifierTypo
public interface ITransactionRepository : IBaseRepository<Transaction>
{
    Task<List<Transaction>> GetAllFromAccountAsync(Guid accountId, int pageNumber, int pageSize, CancellationToken cancellationToken);
}