namespace AccountService.Application.Features.Transactions.Operations.UpdateTransaction;

/// <summary>
/// Запрос на изменение описания транзакции
/// </summary>
/// <param name="Description">Новое описание</param>
// ReSharper disable once IdentifierTypo
public record UpdateTransactionRequest(string Description);