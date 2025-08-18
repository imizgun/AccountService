using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

/// <summary>
///  Событие блокировки клиента
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="clientId"></param>
public class ClientBlocked(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid clientId
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID клиента, который был заблокирован
    /// </summary>
    public Guid ClientId { get; set; } = clientId;
}