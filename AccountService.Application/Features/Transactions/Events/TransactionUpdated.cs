using AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Transactions.Events;

/// <summary>
/// Событие обновления транзакции
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="transactionId"></param>
/// <param name="newDescription"></param>
public class TransactionUpdated(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid transactionId,
    string newDescription
) : DefaultEvent(eventId, meta, occurredAt)
{
    
    /// <summary>
    /// ID связанной транзакции
    /// </summary>
    public Guid TransactionId { get; set; } = transactionId;
    
    /// <summary>
    /// Новое описание транзакции
    /// </summary>
    public string NewDescription { get; set; } = newDescription;
}