namespace AccountService.Core.Domain.Abstraction;

public interface IAccrueInterestRateExecutor 
{
	Task AccrueInterestRateAsync(Guid accountId, CancellationToken cancellationToken);
}