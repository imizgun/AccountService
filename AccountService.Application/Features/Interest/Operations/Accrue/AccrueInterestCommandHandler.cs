using AccountService.Application.Shared.Domain.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Interest.Operations.Accrue;

public class AccrueInterestCommandHandler(IAccrueInterestRateExecutor executor) : IRequestHandler<AccrueInterestCommand>
{
    public async Task Handle(AccrueInterestCommand request, CancellationToken cancellationToken) =>
        await executor.AccrueInterestRateAsync(request, cancellationToken);
}