using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Enums;

namespace AccountService.Core.Domain.Entities;

public class Transaction : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid? CounterpartyAccountId { get; set; }
    public Account? CounterpartyAccount { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public TransactionType TransactionType { get; set; }
    public string Description { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public bool IsDeleted { get; set; }

    public Transaction() { }

    private Transaction(Guid id, Guid accountId, Guid? counterpartyAccountId, decimal amount,
        string currency, TransactionType transactionType, string description)
    {
        Id = id;
        AccountId = accountId;
        CounterpartyAccountId = counterpartyAccountId;
        Amount = amount;
        Currency = currency;
        TransactionType = transactionType;
        Description = description;
        TransactionDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static Transaction Create(Guid accountId, Guid? counterpartyAccountId, decimal amount,
        string currency, TransactionType transactionType, string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be greater than zero.");

        return new Transaction(Guid.NewGuid(), accountId, counterpartyAccountId, amount, currency,
            transactionType, description);
    }

    public Transaction GetReverseTransaction()
    {
        if (CounterpartyAccountId == null) throw new InvalidOperationException("Cannot reverse a transaction without a counterparty account.");

        var notNullCounterpartyAccountId = CounterpartyAccountId ?? Guid.Empty;

        return Create(
                notNullCounterpartyAccountId,
                AccountId,
                Amount,
                Currency,
                TransactionType == TransactionType.Credit ? TransactionType.Debit : TransactionType.Credit,
                Description
            );
    }
}