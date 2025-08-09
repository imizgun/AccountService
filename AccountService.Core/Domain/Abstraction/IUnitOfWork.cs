﻿namespace AccountService.Core.Domain.Abstraction;

public interface IUnitOfWork
{
	Task BeginTransactionAsync(CancellationToken ct = default);
	Task CommitAsync(CancellationToken ct = default);
	Task RollbackAsync(CancellationToken ct = default);
	Task<int> SaveChangesAsync(CancellationToken ct = default);
}