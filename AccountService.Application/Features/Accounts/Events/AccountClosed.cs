using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

/// <summary>
/// Событие закрытия счета
/// </summary>
public class AccountClosed(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid accountId
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID Счета
    /// </summary>
    public Guid AccountId { get; set; } = accountId;
}