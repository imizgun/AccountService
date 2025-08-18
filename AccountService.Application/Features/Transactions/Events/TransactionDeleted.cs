using AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Transactions.Events;

/// <summary>
/// Событие удаления транзакции
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="transactionId"></param>
public class TransactionDeleted(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid transactionId
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID связанной транзакции
    /// </summary>
    public Guid TransactionId { get; set; } = transactionId;
}