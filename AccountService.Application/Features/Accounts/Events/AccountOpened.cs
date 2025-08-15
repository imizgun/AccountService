using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class AccountOpened(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid accountId,
	Guid ownerId,
	string currency,
	string accountType
) : DefaultEvent(eventId, meta, occurredAt) 
{
	public Guid AccountId { get; set; } = accountId;
	public Guid OwnerId { get; set; } = ownerId;
	public string Currency { get; set; } = currency;
	public string AccountType { get; set; } = accountType;
}