using AccountService.Application.Shared.Events;
using Swashbuckle.AspNetCore.Annotations;

namespace AccountService.Application.Features.Accounts.Events;

/// <summary>
/// Событие разблокировки клиента
/// </summary>
/// <param name="eventId"></param>
/// <param name="occurredAt"></param>
/// <param name="meta"></param>
/// <param name="clientId"></param>
public class ClientUnblocked(
    Guid eventId,
    DateTime occurredAt,
    Meta meta,
    Guid clientId
) : DefaultEvent(eventId, meta, occurredAt)
{
    /// <summary>
    /// ID клиента, который был разблокирован
    /// </summary>
    public Guid ClientId { get; set; } = clientId;
}