namespace AccountService.Application.Features.Queues.Domain;

public record AccountOpened(
	Guid EventId,
	DateTime OccurredAt,
	Guid AccountId,
	Guid OwnerId,
	string Currency,
	string AccountType
	);