using AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Accounts.Events;

/// <summary>
/// Событие обновления счета
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="accountId"></param>
/// <param name="newInterestRate"></param>
public class AccountUpdated(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid accountId,
    decimal newInterestRate
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID счета
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
    
    /// <summary>
    /// Новая процентная ставка
    /// </summary>
    public decimal NewInterestRate { get; set; } = newInterestRate;
}