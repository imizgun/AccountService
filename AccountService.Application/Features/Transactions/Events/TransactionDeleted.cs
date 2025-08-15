using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Transactions.Events;

public class TransactionDeleted(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid transactionId
) : DefaultEvent(eventId, meta, occurredAt)
{
	public Guid TransactionId { get; set; } = transactionId;
}