namespace AccountService.Application.Features.Transactions.Domain;

public enum ETransactionType
{
    /// <summary>
    /// Зачисление
    /// </summary>
    Credit,

    /// <summary>
    /// Снятие
    /// </summary>
    Debit
}