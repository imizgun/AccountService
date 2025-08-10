namespace AccountService.Core.Domain.Abstraction;

public interface IAccrueInterestRateSelector 
{
	Task<List<Guid>> SelectAccountsForAccrualAsync(CancellationToken cancellationToken);
}