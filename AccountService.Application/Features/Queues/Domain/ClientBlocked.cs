namespace AccountService.Application.Features.Queues.Domain;

public record ClientBlocked(
	Guid EventId,
	DateTime OccurredAt,
	Guid ClientId
	);