namespace AccountService.Application.Features.Queues.Domain;

public record ClientUnblocked(
	Guid EventId,
	DateTime OccurredAt,
	Guid ClientId
);