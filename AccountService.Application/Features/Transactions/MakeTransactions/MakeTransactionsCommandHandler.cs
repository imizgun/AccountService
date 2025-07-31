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
        if (request.CounterpartyAccountId is not null)
        {
            var exists =
                await _accountRepository.ExistsAsync(request.CounterpartyAccountId ?? Guid.Empty, cancellationToken);
            if (!exists) throw new InvalidOperationException($"Account with id {request.CounterpartyAccountId} does not exist");
        }

        var account = await _accountRepository.GetByIdAsync(request.AccountId, cancellationToken);
        var counterpartyAccount = await _accountRepository.GetByIdAsync(request.CounterpartyAccountId ?? Guid.Empty, cancellationToken);

        if (account == null) return Guid.Empty;

        // Transaction
        account.Withdraw(request.Amount);

        var transaction = Transaction.Create(
            request.AccountId,
            request.CounterpartyAccountId,
            request.Amount,
            request.Currency,
            TransactionType.Debit,
            request.Description
        );

        var transactionId = await _transactionRepository.CreateAsync(transaction, cancellationToken);

        try
        {
            var counterpartyTransaction = transaction.GetReverseTransaction();
            counterpartyAccount?.Deposit(request.Amount);
            await _transactionRepository.CreateAsync(counterpartyTransaction, cancellationToken);
        }
        catch (InvalidOperationException) { }

        return transactionId;
        // Transaction end
    }
}