namespace AccountService.Requests;

/// <summary>
/// Запрос на изменение описания транзакции
/// </summary>
/// <param name="Description">Новое описание</param>
/// <param name="Xmin">Версия записи в базе данных (для оптимистичной блокировки)</param>
// ReSharper disable once IdentifierTypo
public record UpdateTransactionRequest(string Description, uint Xmin);