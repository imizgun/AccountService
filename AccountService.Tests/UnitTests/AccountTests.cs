using AccountService.Application.Features.Accounts.Domain;
using Xunit;

namespace AccountService.Tests.UnitTests;


public class AccountTests
{
    [Fact]
    public void AccountTypeCreation_Test()
    {
        // Arrange
        var ownerId = Guid.NewGuid();

        // Act
        var account = Account.Create(ownerId, AccountType.Checking, "USD", null);
        var accountWithInterestDeposit = Account.Create(ownerId, AccountType.Deposit, "USD", 5.0m);
        var accountWithInterestCredit = Account.Create(ownerId, AccountType.Credit, "USD", 5.0m);

        Account CreateAccountWithInterestChecking() => Account.Create(ownerId, AccountType.Checking, "USD", 5.0m);

        // Assert
        Assert.NotEqual(Guid.Empty, account.Id);
        Assert.NotEqual(Guid.Empty, accountWithInterestDeposit.Id);
        Assert.NotEqual(Guid.Empty, accountWithInterestCredit.Id);
        Assert.Throws<ArgumentException>(CreateAccountWithInterestChecking);
    }

    [Fact]
    public void WithdrawalFromEmptyAccount_Test()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var account = Account.Create(ownerId, AccountType.Checking, "USD", null);

        // Assert
        Assert.Throws<InvalidOperationException>(() => account.Withdraw(100));
    }

    [Fact]
    public void DepositAndWithdrawal_Test()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var account = Account.Create(ownerId, AccountType.Checking, "USD", null);

        // Act
        account.Deposit(100);

        // Assert
        Assert.Equal(100, account.Balance);

        account.Withdraw(50);
        Assert.Equal(50, account.Balance);

        account.Withdraw(50);
        Assert.Equal(0, account.Balance);

        Assert.Throws<InvalidOperationException>(() => account.Withdraw(10));
    }
}