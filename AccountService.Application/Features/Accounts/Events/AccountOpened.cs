namespace AccountService.Application.Features.Accounts.Events;

public record AccountOpened(
	Guid EventId,
	DateTime OccurredAt,
	Guid AccountId,
	Guid OwnerId,
	string Currency,
	string AccountType
	);