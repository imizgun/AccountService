using AccountService.Application.Features.Interest.Operations.Accrue;
using AccountService.Application.Shared.Domain.Abstraction;
using MediatR;

namespace AccountService.Background.DailyAccrueInterestRate;

public class AccrueInterestRateJob(
    IAccrueInterestRateSelector selector,
    IMediator mediator)
{
    public async Task RunJobAsync(CancellationToken cancellationToken)
    {
        var accountIds = await selector.SelectAccountsForAccrualAsync(cancellationToken);

        foreach (var accountId in accountIds)
        {
            await mediator.Send(new AccrueInterestCommand(accountId), cancellationToken);
        }
    }
}