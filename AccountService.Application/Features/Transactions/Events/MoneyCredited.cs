using AccountService.Application.Shared.Events;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Transactions.Events;

/// <summary>
/// Событие зачисления денег на счет
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="accountId"></param>
/// <param name="amount"></param>
/// <param name="currency"></param>
/// <param name="operationId"></param>
public class MoneyCredited(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid accountId,
    decimal amount,
    string currency,
    Guid operationId
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID счета, на который зачислены деньги
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
    
    /// <summary>
    /// Сумма зачисления
    /// </summary>
    public decimal Amount { get; set; } = amount;
    
    /// <summary>
    /// Валюта, в которой зачислены деньги
    /// </summary>
    public string Currency { get; set; } = currency;
    
    /// <summary>
    /// ID операции, связанной с зачислением денег
    /// </summary>
    public Guid OperationId { get; set; } = operationId;
}