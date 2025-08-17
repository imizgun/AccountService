using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class ClientUnblocked(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid clientId
) : DefaultEvent(eventId, meta, occurredAt) {
	public Guid ClientId { get; set; } = clientId;
}