namespace AccountService.Requests;


/// <summary>
/// Запрос на создание транзакции
/// </summary>
/// <param name="CounterpartyAccountId">ID контрагента (опционально)</param>
/// <param name="TransactionType">Тип транзакции (Debit, Credit)</param>
/// <param name="Currency">Валюта</param>
/// <param name="Amount">Сумма перевода</param>
/// <param name="Description">Описание транзакции</param>
public record CreateTransactionRequest(
    Guid? CounterpartyAccountId,
    string TransactionType,
    string Currency,
    decimal Amount,
    string Description);