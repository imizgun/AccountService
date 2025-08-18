namespace AccountService.Application.Shared.Events;

/// <summary>
/// Базовый класс для событий приложения
/// </summary>
/// <param name="eventId">ID события</param>
/// <param name="meta">Метаданные</param>
/// <param name="occuredAt">Время фиксации события</param>
public class DefaultEvent(Guid eventId, Meta meta, DateTime occuredAt) : IEventIdentifiable
{
    /// <summary>
    /// ID события
    /// </summary>
    public Guid EventId { get; set; } = eventId;
    
    /// <summary>
    /// Метаданные события
    /// </summary>
    public Meta Meta { get; set; } = meta;
    
    /// <summary>
    /// Дата и время, когда событие произошло
    /// </summary>
    public DateTime OccurredAt { get; set; } = occuredAt;

    public DefaultEvent() : this(Guid.Empty, null!, DateTime.MinValue) { }
}