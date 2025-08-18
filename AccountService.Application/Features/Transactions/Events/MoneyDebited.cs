using AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Transactions.Events;


/// <summary>
/// Событие списания денег со счета
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="accountId"></param>
/// <param name="amount"></param>
/// <param name="currency"></param>
/// <param name="operationId"></param>
/// <param name="reason"></param>
public class MoneyDebited(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid accountId,
    decimal amount,
    string currency,
    Guid operationId,
    string reason
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID счета, с которого списаны деньги
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
    
    /// <summary>
    /// Сумма списания
    /// </summary>
    public decimal Amount { get; set; } = amount;
    
    /// <summary>
    /// Валюта, в которой списаны деньги
    /// </summary>
    public string Currency { get; set; } = currency;
    
    /// <summary>
    /// ID операции, связанной со списанием денег
    /// </summary>
    public Guid OperationId { get; set; } = operationId;
    
    /// <summary>
    /// Причина списания денег
    /// </summary>
    public string Reason { get; set; } = reason;
}