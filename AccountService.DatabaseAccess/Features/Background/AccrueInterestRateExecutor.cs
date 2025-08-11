using AccountService.Core.Abstraction;
using AccountService.DatabaseAccess.Abstractions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AccountService.DatabaseAccess.Features.Background;

public class AccrueInterestRateExecutor(
	IUnitOfWork unitOfWork, 
	AccountServiceDbContext dbContext) : IAccrueInterestRateExecutor
{
	public async Task AccrueInterestRateAsync(Guid accountId, CancellationToken cancellationToken) 
	{
		await unitOfWork.BeginTransactionAsync(cancellationToken);
		try {
			await dbContext.Database.ExecuteSqlRawAsync(
				"CALL accrue_interest(@account_id)",
				new NpgsqlParameter("account_id", accountId));
			
			await unitOfWork.CommitAsync(cancellationToken);
		}
		catch {
			await unitOfWork.RollbackAsync(cancellationToken);
			throw;
		}
	}
}