using AccountService.Application.Shared.DatabaseAccess;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AccountService.Application.Features.Interest.DatabaseAccess;

public class AccrueInterestRateExecutor(
    IUnitOfWork unitOfWork,
    AccountServiceDbContext dbContext) : IAccrueInterestRateExecutor
{
    public async Task AccrueInterestRateAsync(Guid accountId, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            await dbContext.Database.ExecuteSqlRawAsync(
                "CALL accrue_interest(@account_id)",
                new NpgsqlParameter("account_id", accountId));

            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}