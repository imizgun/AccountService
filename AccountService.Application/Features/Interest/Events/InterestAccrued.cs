using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Interest.Events;

public class InterestAccrued(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid accountId,
	DateTime periodFrom,
	DateTime periodTo,
	decimal amount
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid AccountId { get; set; } = accountId;
	public DateTime PeriodFrom { get; set; } = periodFrom;
	public DateTime PeriodTo { get; set; } = periodTo;
	public decimal Amount { get; set; } = amount;
}