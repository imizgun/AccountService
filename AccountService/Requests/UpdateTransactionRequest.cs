namespace AccountService.Requests;

/// <summary>
/// Запрос на изменение описания транзакции
/// </summary>
/// <param name="Description">Новое описание</param>
public record UpdateTransactionRequest(string Description);