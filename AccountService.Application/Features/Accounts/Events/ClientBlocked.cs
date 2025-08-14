namespace AccountService.Application.Features.Accounts.Events;

public record ClientBlocked(
	Guid EventId,
	DateTime OccurredAt,
	Guid ClientId
	);