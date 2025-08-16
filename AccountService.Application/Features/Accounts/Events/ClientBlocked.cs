using AccountService.Application.Shared.Events;

namespace AccountService.Application.Features.Accounts.Events;

public class ClientBlocked(
	Guid eventId,
	DateTime occurredAt,
	Meta meta,
	Guid clientId
) : DefaultEvent {
	public Guid ClientId { get; set; } = clientId;
}