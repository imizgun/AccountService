using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Interest.Events;
using AccountService.Application.Features.Interest.Operations.Accrue;
using AccountService.Application.Shared.DatabaseAccess;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Domain.Abstraction;
using AccountService.Application.Shared.Events;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AccountService.Application.Features.Interest.DatabaseAccess;

public class AccrueInterestRateExecutor(
    IUnitOfWork unitOfWork,
    AccountServiceDbContext dbContext,
    IOutboxMessageRepository outboxMessageRepository) : IAccrueInterestRateExecutor
{
    public async Task AccrueInterestRateAsync(AccrueInterestCommand command, CancellationToken cancellationToken)
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
        try {
            var balance = command.Account.Balance;
            var interestRate = command.Account.InterestRate ?? 1;
            
            await dbContext.Database.ExecuteSqlRawAsync(
                "CALL accrue_interest(@account_id)",
                new NpgsqlParameter("account_id", command.Account.Id));
            
            var accrueEvent = new InterestAccrued(
                Guid.NewGuid(), 
                DateTime.UtcNow,
                new Meta(command.CorrelationId),
                command.Account.Id,
                DateTime.UtcNow - TimeSpan.FromDays(1),
                DateTime.UtcNow,
                balance * interestRate / 100
                );

            await outboxMessageRepository.AddAsync(new OutboxMessage(accrueEvent), cancellationToken);
            
            await dbContext.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}