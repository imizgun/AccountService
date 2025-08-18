using AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

namespace AccountService.Application.Features.Interest.Events;

/// <summary>
/// Событие начисления процентов на счет
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="accountId"></param>
/// <param name="periodFrom"></param>
/// <param name="periodTo"></param>
/// <param name="amount"></param>
public class InterestAccrued(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid accountId,
    DateTime periodFrom,
    DateTime periodTo,
    decimal amount
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID счета, на который начислены проценты
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
    
    /// <summary>
    /// Левая дата периода, за который начислены проценты
    /// </summary>
    public DateTime PeriodFrom { get; set; } = periodFrom;
    
    /// <summary>
    /// Правая дата периода, за который начислены проценты
    /// </summary>
    public DateTime PeriodTo { get; set; } = periodTo;
    
    /// <summary>
    /// Сумма начислений
    /// </summary>
    public decimal Amount { get; set; } = amount;
}