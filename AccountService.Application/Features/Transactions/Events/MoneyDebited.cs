using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Transactions.Events;

public class MoneyDebited(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid accountId,
	decimal amount,
	string currency,
	Guid operationId,
	string reason
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid AccountId { get; set; } = accountId;
	public decimal Amount { get; set; } = amount;
	public string Currency { get; set; } = currency;
	public Guid OperationId { get; set; } = operationId;
	public string Reason { get; set; } = reason;
}