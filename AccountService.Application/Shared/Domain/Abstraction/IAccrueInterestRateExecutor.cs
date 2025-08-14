namespace AccountService.Application.Shared.Domain.Abstraction;

public interface IAccrueInterestRateExecutor
{
    Task AccrueInterestRateAsync(Guid accountId, CancellationToken cancellationToken);
}