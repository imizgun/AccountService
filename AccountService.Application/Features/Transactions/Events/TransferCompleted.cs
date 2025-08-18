using AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Transactions.Events;

/// <summary>
/// Событие завершения перевода
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="sourceAccountId"></param>
/// <param name="destinationAccountId"></param>
/// <param name="amount"></param>
/// <param name="currency"></param>
/// <param name="transferId"></param>
public class TransferCompleted(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid sourceAccountId,
    Guid? destinationAccountId,
    decimal amount,
    string currency,
    Guid transferId
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// Аккаунт списания
    /// </summary>
    public Guid SourceAccountId { get; set; } = sourceAccountId;
    
    /// <summary>
    /// Аккаунт зачисления
    /// </summary>
    public Guid? DestinationAccountId { get; set; } = destinationAccountId;
    
    /// <summary>
    /// Сумма перевода
    /// </summary>
    public decimal Amount { get; set; } = amount;
    
    /// <summary>
    /// Валюта перевода
    /// </summary>
    public string Currency { get; set; } = currency;
    
    /// <summary>
    /// ID перевода
    /// </summary>
    public Guid TransferId { get; set; } = transferId;
}