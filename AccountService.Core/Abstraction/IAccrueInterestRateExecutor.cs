namespace AccountService.Core.Abstraction;

public interface IAccrueInterestRateExecutor 
{
	Task AccrueInterestRateAsync(Guid accountId, CancellationToken cancellationToken);
}