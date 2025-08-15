using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Transactions.Events;

public class MoneyCredited(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid accountId,
	decimal amount,
	string currency,
	Guid operationId
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid AccountId { get; set; } = accountId;
	public decimal Amount { get; set; } = amount;
	public string Currency { get; set; } = currency;
	public Guid OperationId { get; set; } = operationId;
}