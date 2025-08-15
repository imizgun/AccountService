using AccountService.Application.Features.Interest.Operations.Accrue;

namespace AccountService.Application.Shared.Domain.Abstraction;

public interface IAccrueInterestRateExecutor
{
    Task AccrueInterestRateAsync(AccrueInterestCommand command, CancellationToken cancellationToken);
}