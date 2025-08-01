using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using AccountService.Core.Domain.Enums;
using MediatR;

namespace AccountService.Application.Features.Transactions.MakeTransactions;

public class MakeTransactionsCommandHandler : IRequestHandler<MakeTransactionCommand, Guid>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public MakeTransactionsCommandHandler(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    public async Task<Guid> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.AccountId == request.CounterpartyAccountId)
            throw new InvalidOperationException("You can't make transactions to the same account");

        if (!Enum.TryParse<TransactionType>(request.TransactionType, out var transactionType))
            throw new InvalidOperationException("Transaction type is not valid");

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken)
                      ?? throw new InvalidOperationException("Account not found");

        var counterpartyAccount = request.CounterpartyAccountId is not null
            ? await _accountRepository.GetByIdAsync(request.CounterpartyAccountId.Value, cancellationToken)
            : null;

        if (transactionType == TransactionType.Credit && counterpartyAccount is not null)
            throw new InvalidOperationException("You can't do credit from other account");

        // Transaction
        var transaction = Transaction.Create(
            request.AccountId,
            request.CounterpartyAccountId,
            request.Amount,
            request.Currency,
            transactionType,
            request.Description
        );

        if (transactionType == TransactionType.Debit)
            account.Withdraw(request.Amount);
        else
            account.Deposit(request.Amount);

        var transactionId = await _transactionRepository.CreateAsync(transaction, cancellationToken);
        await _accountRepository.UpdateAccount(account, cancellationToken);

        if (counterpartyAccount is null) return transactionId;

        var counterpartyTransaction = transaction.GetReverseTransaction();
        counterpartyAccount.Deposit(request.Amount);
        await _transactionRepository.CreateAsync(counterpartyTransaction, cancellationToken);
        await _accountRepository.UpdateAccount(counterpartyAccount, cancellationToken);

        return transactionId;
        // Transaction end
    }
}