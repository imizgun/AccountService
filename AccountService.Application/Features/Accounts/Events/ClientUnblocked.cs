namespace AccountService.Application.Features.Accounts.Events;

public record ClientUnblocked(
	Guid EventId,
	DateTime OccurredAt,
	Guid ClientId
);