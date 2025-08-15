using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class AccountClosed(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid accountId
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid AccountId { get; set; } = accountId;
}