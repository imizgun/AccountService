using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Transactions.Events;

public class TransferCompleted(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid sourceAccountId,
	Guid? destinationAccountId,
	decimal amount,
	string currency,
	Guid transferId
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid SourceAccountId { get; set; } = sourceAccountId;
	public Guid? DestinationAccountId { get; set; } = destinationAccountId;
	public decimal Amount { get; set; } = amount;
	public string Currency { get; set; } = currency;
	public Guid TransferId { get; set; } = transferId;
}