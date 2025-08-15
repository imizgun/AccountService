using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class AccountUpdated(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid accountId,
	decimal newInterestRate
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid AccountId { get; set; } = accountId;
	public decimal NewInterestRate { get; set; } = newInterestRate;
}