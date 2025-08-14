using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Shared.Domain.Abstraction;

namespace AccountService.Application.Features.Accounts.Domain;
// ReSharper disable once IdentifierTypo
public class Account : IIdentifiable
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public AccountType AccountType { get; set; }
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength Строка валюты проверяется, поэтому не может быть неограниченной длины
    public string Currency { get; set; } = null!;
    public decimal Balance { get; private set; }
    private decimal? _interestRate;
    public decimal? InterestRate
    {
        get => _interestRate;
        set
        {
            if (AccountType == AccountType.Checking && value.HasValue)
                throw new ArgumentException("Checking accounts cannot have an interest rate.");
            _interestRate = value;
        }
    }
    // ReSharper disable once UnusedAutoPropertyAccessor.Local : приватный сеттер нужен для гарантии постоянства этого поля
    public uint Xmin { get; private set; }
    public DateTime OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public bool IsFrozen { get; set; } = false;

    public List<Transaction> Transactions { get; set; } = [];

    public Account() { }

    private Account(Guid id, Guid ownerId, AccountType accountType, string currency, decimal? interestRate)
    {
        Id = id;
        OwnerId = ownerId;
        AccountType = accountType;
        Currency = currency;
        InterestRate = interestRate;
        OpeningDate = DateTime.UtcNow;
        Balance = 0m;
    }

    public static Account Create(Guid ownerId, AccountType accountType, string currency, decimal? interestRate)
    {
        if (accountType == AccountType.Checking && interestRate != null)
            throw new ArgumentException("Checking accounts cannot have an interest rate.");

        return new Account(Guid.NewGuid(), ownerId, accountType, currency, interestRate);
    }

    public void Close()
    {
        if (ClosingDate is not null)
            throw new InvalidOperationException("Account is already closed.");

        ClosingDate = DateTime.UtcNow;
    }

    private bool CanWithdrawal(decimal amount)
    {
        if (amount > Balance) throw new InvalidOperationException("Insufficient funds.");

        return amount > 0;
    }

    public void Withdraw(decimal amount)
    {
        if (CanWithdrawal(amount))
            Balance -= amount;
    }

    public void Deposit(decimal amount)
    {
        Balance += amount;
    }
}