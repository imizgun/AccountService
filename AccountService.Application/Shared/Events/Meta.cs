namespace AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

/// <summary>
/// Метаданные события, которые публикуется в шину сообщений
/// </summary>
/// <param name="correlationId">ID корреляции</param>
public class Meta(Guid correlationId)
{
    public string Version { get; set; } = "v1";
    public string Source { get; set; } = "AccountService";
    public Guid CorrelationId { get; set; } = correlationId;
    public Guid CausationId { get; set; } = Guid.NewGuid();
}