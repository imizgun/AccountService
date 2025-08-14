namespace AccountService.Application.Shared.Domain.Abstraction;

public interface IAccrueInterestRateSelector
{
    Task<List<Guid>> SelectAccountsForAccrualAsync(CancellationToken cancellationToken);
}