using AccountService.Application.Shared.Events;

// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Accounts.Events;

/// <summary>
/// Событие открытия счета
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="accountId"></param>
/// <param name="ownerId"></param>
/// <param name="currency"></param>
/// <param name="accountType"></param>
public class AccountOpened(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid accountId,
    Guid ownerId,
    string currency,
    string accountType
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID Счета
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
    
    /// <summary>
    /// ID владельца счета
    /// </summary>
    public Guid OwnerId { get; set; } = ownerId;

    /// <summary>
    /// Валюта счета
    /// </summary>
    public string Currency { get; set; } = currency;
    
    /// <summary>
    /// Тип счета
    /// </summary>
    public string AccountType { get; set; } = accountType;
}