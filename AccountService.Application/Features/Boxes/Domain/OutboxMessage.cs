using AccountService.Application.Shared.Events;
using System.Text.Json;

namespace AccountService.Application.Features.Boxes.Domain;

public class OutboxMessage
{
	public Guid Id { get; set; }
	public string Type { get; set; } = null!;   
	public string Payload { get; set; } = null!;
	public DateTime OccurredAt { get; set; }    
	public DateTime? PublishedAt { get; set; }

	public OutboxMessage() { }

	public OutboxMessage(object @event) 
	{
		Id = Guid.NewGuid();
		Type = @event.GetType().AssemblyQualifiedName!;
		Payload = JsonSerializer.Serialize(@event);
		OccurredAt = DateTime.UtcNow;
		PublishedAt = null;
	}
}