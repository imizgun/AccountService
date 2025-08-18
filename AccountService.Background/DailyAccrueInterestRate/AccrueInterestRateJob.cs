using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Interest.Operations.Accrue;
using AccountService.Application.Shared.Domain.Abstraction;
using MediatR;

namespace AccountService.Background.DailyAccrueInterestRate;

public class AccrueInterestRateJob(
    IAccrueInterestRateSelector<Account> selector,
    IMediator mediator)
{
    public async Task RunJobAsync(CancellationToken cancellationToken)
    {
        var accountIds = await selector.SelectAccountsForAccrualAsync(cancellationToken);

        foreach (var account in accountIds)
        {
            await mediator.Send(new AccrueInterestCommand(account, Guid.NewGuid()), cancellationToken);
        }
    }
}