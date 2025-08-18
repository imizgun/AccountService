namespace AccountService.Application.Shared.Events;
// ReSharper disable UnusedMember.Global Неиспользуемые свойства нужны для события

/// <summary>
/// Метаданные события, которые публикуется в шину сообщений
/// </summary>
/// <param name="correlationId">ID корреляции</param>
public class Meta(Guid correlationId)
{
    /// <summary>
    /// Версия используемого события
    /// </summary>
    public string Version { get; set; } = "v1";
    
    /// <summary>
    /// Источник события, например, название сервиса
    /// </summary>
    public string Source { get; set; } = "AccountService";
    
    /// <summary>
    /// Корреляционный ID события, используется для отслеживания цепочки событий
    /// </summary>
    public Guid CorrelationId { get; set; } = correlationId;
    
    /// <summary>
    /// ID причинного события, если это событие вызвано другим методом или событием
    /// </summary>
    public Guid CausationId { get; set; } = Guid.NewGuid();
}