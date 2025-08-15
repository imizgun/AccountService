using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class ClientUnblocked(
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