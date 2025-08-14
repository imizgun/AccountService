using MediatR;

namespace AccountService.Application.Features.Interest.Operations.Accrue;

public record AccrueInterestCommand(Guid AccountId) : IRequest;