using AccountService.Application.Shared.Events;
using System.Text.Json;
// ReSharper disable EntityFramework.ModelValidation.UnlimitedStringLength Длина строки ограничена типом обработчика

namespace AccountService.Application.Features.Boxes.Domain;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTime OccurredAt { get; set; }
    public DateTime? PublishedAt { get; set; }

    public OutboxMessage() { }

    public OutboxMessage(DefaultEvent @event)
    {
        Id = Guid.NewGuid();
        Type = @event.GetType().AssemblyQualifiedName!;
        Payload = JsonSerializer.Serialize<object>(@event);
        OccurredAt = DateTime.UtcNow;
        PublishedAt = null;
    }
}