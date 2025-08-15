namespace AccountService.Application.Shared.Events;

public class DefaultEvent(Guid eventId, Meta meta, DateTime occuredAt) : IEventIdentifiable {
	public Guid EventId { get; set; } = eventId;
	public Meta Meta { get; set; } = meta;
	public DateTime OccurredAt { get; set; } = occuredAt;
	
	public DefaultEvent() : this(Guid.Empty, null!, DateTime.MinValue) { }
}