using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.MakeTransactions;

/// <summary>
/// Команда на создание транзакции
/// </summary>
/// <param name="AccountId">ID аккаунта</param>
/// <param name="CounterpartyAccountId">ID контрагента (опционально)</param>
/// <param name="TransactionType">Тип транзакции (Debit, Credit)</param>
/// <param name="Currency">Валюта</param>
/// <param name="Amount">Сумма перевода</param>
/// <param name="Description">Описание транзакции</param>
public record MakeTransactionCommand(
    Guid AccountId,
    Guid? CounterpartyAccountId,
    string TransactionType,
    string Currency,
    decimal Amount,
    string Description,
    Guid CorrelationId) : IRequest<Guid>;