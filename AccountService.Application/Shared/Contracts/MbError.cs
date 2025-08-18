namespace AccountService.Application.Shared.Contracts;

/// <summary>
/// Общий класс для ошибок
/// </summary>
public class MbError
{
    public string? Message { get; init; }

    /// <summary>
    /// Ошибки валидации
    /// </summary>
    public List<string>? ValidationErrors { get; init; }
}
