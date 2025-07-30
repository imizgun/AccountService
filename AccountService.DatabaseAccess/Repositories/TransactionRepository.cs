using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;

namespace AccountService.DatabaseAccess.Repositories;

public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
{
}