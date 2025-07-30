using AccountService.Core.Domain.Entities;

namespace AccountService.Core.Domain.Abstraction;

public interface ITransactionRepository : IBaseRepository<Transaction>
{
}