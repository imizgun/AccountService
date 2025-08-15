using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class ClientBlocked(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid clientId
) : IEventIdentifiable {
	public Guid EventId { get; set; } = eventId;
	public DateTime OccurredAt { get; set; } = occurredAt;
	public Meta Meta { get; set; } = meta;
	public Guid ClientId { get; set; } = clientId;
}