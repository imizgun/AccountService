using System.Data;
using AccountService.DatabaseAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountService.DatabaseAccess.Repositories;

public class UnitOfWorkRepository(AccountServiceDbContext context) : IUnitOfWork, IDisposable, IAsyncDisposable
{
	private IDbContextTransaction? _transaction;
	
	public async Task BeginTransactionAsync(CancellationToken ct = default)
	{
		_transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
	}
	
	public Task CommitAsync(CancellationToken ct = default) => _transaction != null ? _transaction.CommitAsync(ct) : Task.CompletedTask;
	
	public  Task RollbackAsync(CancellationToken ct = default) => _transaction != null ? _transaction.RollbackAsync(ct) : Task.CompletedTask;
		
	public async Task<int> SaveChangesAsync(CancellationToken ct = default) => await context.SaveChangesAsync(ct);

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