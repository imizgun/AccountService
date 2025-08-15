using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Transactions.Events;

public class TransactionUpdated(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid transactionId,
	string newDescription
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid TransactionId { get; set; } = transactionId;
	public string NewDescription { get; set; } = newDescription;
}