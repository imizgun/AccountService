using AccountService.Application.Features.Accounts.Domain;
using MediatR;

namespace AccountService.Application.Features.Interest.Operations.Accrue;

public record AccrueInterestCommand(Account Account, Guid CorrelationId) : IRequest;