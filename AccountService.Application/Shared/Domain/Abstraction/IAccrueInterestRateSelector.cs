namespace AccountService.Application.Shared.Domain.Abstraction;

public interface IAccrueInterestRateSelector<T>
{
    Task<List<T>> SelectAccountsForAccrualAsync(CancellationToken cancellationToken);
}