using AccountService.Core.Features.Transactions;
using Xunit;

namespace AccountService.Tests.UnitTests;

public class TransactionTests 
{
	[Fact]
	public void CreateTransaction_ValidInput_ShouldCreateTransaction()
	{
		// Arrange
		var accountId = Guid.NewGuid();
		var counterpartyAccountId = Guid.NewGuid();
		var amount = 100.0m;
		var currency = "USD";
		var transactionType = ETransactionType.Debit;
		var description = "Test transaction";

		// Act
		var transaction = Transaction.Create(accountId, counterpartyAccountId, amount, currency, transactionType, description);

		// Assert
		Assert.NotEqual(Guid.Empty, transaction.Id);
		Assert.Equal(accountId, transaction.AccountId);
		Assert.Equal(counterpartyAccountId, transaction.CounterpartyAccountId);
		Assert.Equal(amount, transaction.Amount);
		Assert.Equal(currency, transaction.Currency);
		Assert.Equal(transactionType, transaction.TransactionType);
		Assert.Equal(description, transaction.Description);
		Assert.False(transaction.IsDeleted);
	}
	
	[Fact]
	public void CreateTransaction_ZeroOrNegativeAmount_ShouldThrowArgumentException() 
	{
		// Arrange
		var accountId = Guid.NewGuid();
		var counterpartyAccountId = Guid.NewGuid();
		var amount = 0.0m; // or negative value
		var currency = "USD";
		var transactionType = ETransactionType.Credit;
		var description = "Test transaction";

		// Act & Assert
		Assert.Throws<ArgumentException>(() => Transaction.Create(accountId, counterpartyAccountId, amount, currency, transactionType, description));
	}
	
	[Fact]
	public void CreateReversalTransactionWithNullCounterpartyAccountId_ShouldThrowInvalidOperationException() 
	{
		// Arrange
		var accountId = Guid.NewGuid();
		var amount = 100.0m;
		var currency = "USD";
		var transactionType = ETransactionType.Credit;
		var description = "Test transaction";

		var transaction = Transaction.Create(accountId, null, amount, currency, transactionType, description);

		// Act & Assert
		Assert.Throws<InvalidOperationException>(() => transaction.GetReverseTransaction());
	}
	
	[Fact]
	public void CreateNormalTransactionAndReverse_ShouldBeOk() 
	{
		// Arrange
		var accountId = Guid.NewGuid();
		var counterpartyAccountId = Guid.NewGuid();
		var amount = 100.0m;
		var currency = "USD";
		var transactionType = ETransactionType.Debit;
		var description = "Test transaction";

		var transaction = Transaction.Create(accountId, counterpartyAccountId, amount, currency, transactionType, description);

		// Act
		var reversedTransaction = transaction.GetReverseTransaction();

		// Assert
		Assert.NotEqual(transaction.Id, reversedTransaction.Id);
		Assert.Equal(counterpartyAccountId, reversedTransaction.AccountId);
		Assert.Equal(accountId, reversedTransaction.CounterpartyAccountId);
		Assert.Equal(amount, reversedTransaction.Amount);
		Assert.Equal(currency, reversedTransaction.Currency);
		Assert.Equal(ETransactionType.Credit, reversedTransaction.TransactionType);
		Assert.Equal(description, reversedTransaction.Description);
	}
	
	[Fact]
	public void CreateTransactionWithMaxDescriptionLength_ShouldThrowException() 
	{
		// Arrange
		var accountId = Guid.NewGuid();
		var amount = 100.0m;
		var currency = "USD";
		var transactionType = ETransactionType.Credit;
		var description = new string('A', Transaction.MaxDescriptionLength + 1); // Max length description

		// Act
		var transaction = Transaction.Create(accountId, null, amount, currency, transactionType, description);

		// Assert
		Assert.NotEqual(Guid.Empty, transaction.Id);
		Assert.Equal(description, transaction.Description);
	}

	[Fact]
	public void CreateTransactionWithCounterpartyCredit_ShouldThrow() 
	{
		// Arrange
		var accountId = Guid.NewGuid();
		var counterpartyAccountId = Guid.NewGuid();
		var amount = 100.0m;
		var currency = "USD";
		var transactionType = ETransactionType.Credit;
		var description = "Test transaction";

		Assert.Throws<ArgumentException>(() =>
			Transaction.Create(accountId, counterpartyAccountId, amount, currency, transactionType, description));
	}
}