using AccountService.Core.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountService.DatabaseAccess.Repositories;

public class UnitOfWorkRepository(
	AccountServiceDbContext context, 
	IAccountRepository accountRepository, 
	ITransactionRepository transactionRepository) : IUnitOfWork, IDisposable, IAsyncDisposable
{
	public IAccountRepository AccountRepository => accountRepository;
	public ITransactionRepository TransactionsRepository => transactionRepository;
	private IDbContextTransaction? _transaction;
	
	public async Task BeginTransactionAsync(CancellationToken ct = default)
	{
		_transaction = await context.Database.BeginTransactionAsync(ct);
		await context.Database.ExecuteSqlRawAsync("SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;", ct);
	}
	
	public Task CommitAsync(CancellationToken ct = default) => _transaction != null ? _transaction.CommitAsync(ct) : Task.CompletedTask;
	
	public  Task RollbackAsync(CancellationToken ct = default) => _transaction != null ? _transaction.RollbackAsync(ct) : Task.CompletedTask;
		
	public async Task SaveChangesAsync(CancellationToken ct = default) => await context.SaveChangesAsync(ct);

	public void Dispose() 
	{
		_transaction?.Dispose();
		context.Dispose();
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync() {
		if (_transaction != null)
		{
			await _transaction.DisposeAsync();
			_transaction = null;
		}
		await context.DisposeAsync();
		GC.SuppressFinalize(this);
	}
}