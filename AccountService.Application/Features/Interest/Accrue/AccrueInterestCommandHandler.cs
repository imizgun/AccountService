using AccountService.Core.Abstraction;
using MediatR;

namespace AccountService.Application.Features.Interest.Accrue;

public class AccrueInterestCommandHandler(IAccrueInterestRateExecutor executor) : IRequestHandler<AccrueInterestCommand> 
{
	public async Task Handle(AccrueInterestCommand request, CancellationToken cancellationToken) =>
		await executor.AccrueInterestRateAsync(request.AccountId, cancellationToken);
}