namespace AccountService.Responses;


/// <summary>
/// Общий тип для REST ответов
/// </summary>
/// <typeparam name="T">Тип данных, возвращаемый эндпоинтом</typeparam>
public class MbResult<T>
{
    /// <summary>
    /// Флаг успешности операции
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Тело ответа
    /// </summary>
    public T? Result { get; init; }

    /// <summary>
    /// Сообщение об исходе операции
    /// </summary>
    public string? Message { get; init; }

    public MbError? MbError { get; init; }

    /// <summary>
    /// Успешная операция
    /// </summary>
    /// <param name="result">Результат операции</param>
    /// <param name="message">Сообщение об операции</param>
    /// <returns>Instance MbResult</returns>
    public static MbResult<T> Ok(T result, string? message = null) => new()
    {
        IsSuccess = true,
        Result = result,
        Message = message
    };

    /// <summary>
    /// Неудачная операция
    /// </summary>
    /// <param name="message">Сообщение об ошибке</param>
    /// <returns>Instance MbResult</returns>
    public static MbResult<T> Fail(string message) => new()
    {
        IsSuccess = false,
        Result = default,
        Message = message,
        MbError = new MbError
        {
            Message = message
        }
    };

    public static MbResult<T> ValidationFail(List<string> validationErrors) => new()
    {
        IsSuccess = false,
        Result = default,
        Message = "Validation failed",
        MbError = new MbError
        {
            Message = "One or more validation errors occurred.",
            ValidationErrors = validationErrors
        }
    };
}
