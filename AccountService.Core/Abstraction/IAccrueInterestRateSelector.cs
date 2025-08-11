namespace AccountService.Core.Abstraction;

public interface IAccrueInterestRateSelector 
{
	Task<List<Guid>> SelectAccountsForAccrualAsync(CancellationToken cancellationToken);
}