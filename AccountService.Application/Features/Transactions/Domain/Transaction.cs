using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Shared.Domain.Abstraction;

namespace AccountService.Application.Features.Transactions.Domain;
// ReSharper disable once IdentifierTypo
public class Transaction : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid AccountId { get; set; }
    public Account Account { get; set; } = null!;
    public Guid? CounterpartyAccountId { get; set; }
    // ReSharper disable once UnusedMember.Global Поле необходимо для сущности и используется в её DTO
    public Account? CounterpartyAccount { get; set; }
    public decimal Amount { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength Строка валюты проверяется, поэтому не может быть неограниченной длины
    public string Currency { get; set; } = null!;
    public ETransactionType TransactionType { get; set; }
    public string Description { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public bool IsDeleted { get; set; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Local : приватный сеттер нужен для гарантии постоянства этого поля
    public uint Xmin { get; private set; }
    public const int MaxDescriptionLength = 500;

    public Transaction() { }

    private Transaction(Guid id, Guid accountId, Guid? counterpartyAccountId, decimal amount,
        string currency, ETransactionType eTransactionType, string description)
    {
        Id = id;
        AccountId = accountId;
        CounterpartyAccountId = counterpartyAccountId;
        Amount = amount;
        Currency = currency;
        TransactionType = eTransactionType;
        Description = description;
        TransactionDate = DateTime.UtcNow;
        IsDeleted = false;
    }

    public static Transaction Create(Guid accountId, Guid? counterpartyAccountId, decimal amount,
        string currency, ETransactionType eTransactionType, string description)
    {
        if (amount <= 0)
            throw new ArgumentException("Transaction amount must be greater than zero.");

        if (eTransactionType == ETransactionType.Credit && counterpartyAccountId != null)
            throw new ArgumentException("Credit transactions should not have a counterparty account.");

        return new Transaction(Guid.NewGuid(), accountId, counterpartyAccountId, amount, currency,
            eTransactionType, description);
    }

    public Transaction GetReverseTransaction()
    {
        if (CounterpartyAccountId == null) throw new InvalidOperationException("Cannot reverse a transaction without a counterparty account.");

        var notNullCounterpartyAccountId = CounterpartyAccountId ?? Guid.Empty;

        return new Transaction(
                Guid.NewGuid(),
                notNullCounterpartyAccountId,
                AccountId,
                Amount,
                Currency,
                TransactionType == ETransactionType.Credit ? ETransactionType.Debit : ETransactionType.Credit,
                Description
            );
    }
}