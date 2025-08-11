using MediatR;

namespace AccountService.Application.Features.Interest.Accrue;

public record AccrueInterestCommand(Guid AccountId) : IRequest;