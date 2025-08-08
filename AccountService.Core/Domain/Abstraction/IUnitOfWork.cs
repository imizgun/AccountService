namespace AccountService.Core.Domain.Abstraction;

public interface IUnitOfWork
{
	IAccountRepository AccountRepository { get; }
	ITransactionRepository TransactionsRepository { get; }
	Task BeginTransactionAsync(CancellationToken ct = default);
	Task CommitAsync(CancellationToken ct = default);
	Task RollbackAsync(CancellationToken ct = default);
	Task<int> SaveChangesAsync(CancellationToken ct = default);
}