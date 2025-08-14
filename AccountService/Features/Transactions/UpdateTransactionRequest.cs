namespace AccountService.Features.Transactions;

/// <summary>
/// Запрос на изменение описания транзакции
/// </summary>
/// <param name="Description">Новое описание</param>
// ReSharper disable once IdentifierTypo
public record UpdateTransactionRequest(string Description);