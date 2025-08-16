using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Transactions.Domain;
using AccountService.Application.Features.Transactions.Events;
using AccountService.Application.Shared.DatabaseAccess.Abstractions;
using AccountService.Application.Shared.Events;
using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.MakeTransactions;

public class MakeTransactionsCommandHandler(
    ITransactionRepository transactionRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IOutboxMessageRepository outboxMessageRepository) : IRequestHandler<MakeTransactionCommand, Guid>
{
    public async Task<Guid> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.AccountId == request.CounterpartyAccountId)
            throw new InvalidOperationException("You can't make transactions to the same account");

        if (!Enum.TryParse<ETransactionType>(request.TransactionType, out var transactionType))
            throw new InvalidOperationException("Transaction type is not valid");

        // Transaction start

        await unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken)
                          ?? throw new InvalidOperationException("Account not found");
            
            if (account.IsFrozen) throw new InvalidOperationException("Account is frozen, you can't make transactions from it.");

            var counterpartyAccount = request.CounterpartyAccountId is not null
                ? await accountRepository.GetByIdForUpdateAsync(request.CounterpartyAccountId.Value,
                    cancellationToken)
                : null;

            if (transactionType == ETransactionType.Credit && counterpartyAccount is not null)
                throw new InvalidOperationException("You can't do credit from other account");

            var isSecondTransactionSuccess = true;

            // Создание транзакций, изменение баланса и сохранение в БД
            var transaction = Transaction.Create(
                request.AccountId,
                request.CounterpartyAccountId,
                request.Amount,
                request.Currency,
                transactionType,
                request.Description
            );
            DefaultEvent firstTransactionEvent;

            if (transactionType == ETransactionType.Debit) 
            {
                account.Withdraw(request.Amount);
                firstTransactionEvent = new MoneyDebited(
                        Guid.NewGuid(),
                        DateTime.UtcNow,
                        new Meta(request.CorrelationId),
                        request.AccountId,
                        request.Amount,
                        request.Currency,
                        transaction.Id,
                        request.Description
                    );
            }
            else 
            {
                account.Deposit(request.Amount);
                firstTransactionEvent = new MoneyCredited(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    new Meta(request.CorrelationId),
                    request.AccountId,
                    request.Amount,
                    request.Currency,
                    request.CorrelationId
                    );
            }

            var transactionId = await transactionRepository.CreateAsync(transaction, cancellationToken);
            await outboxMessageRepository.AddAsync(new OutboxMessage(firstTransactionEvent), cancellationToken);

            if (counterpartyAccount is not null)
            {
                var counterpartyTransaction = transaction.GetReverseTransaction();
                counterpartyAccount.Deposit(request.Amount);
                
                var counterpartyEvent = new MoneyDebited(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    new Meta(request.CorrelationId),
                    request.CounterpartyAccountId!.Value,
                    request.Amount,
                    request.Currency,
                    transaction.Id,
                    request.Description
                );
                
                await transactionRepository.CreateAsync(counterpartyTransaction, cancellationToken);
                await outboxMessageRepository.AddAsync(new OutboxMessage(counterpartyEvent), cancellationToken);

            }
            
            var transferEvent = new TransferCompleted(
                    Guid.NewGuid(),
                    DateTime.UtcNow,
                    new Meta(request.CorrelationId),
                    request.AccountId,
                    request.CounterpartyAccountId ?? null,
                    request.Amount,
                    request.Currency,
                    transaction.Id
                );

            await outboxMessageRepository.AddAsync(new OutboxMessage(transferEvent), cancellationToken);

            // Сохранение изменений в БД
            await unitOfWork.SaveChangesAsync(cancellationToken);

            // Проверка баланса аккаунтов после транзакции
            var isFirstTransactionSuccess =
                await accountRepository.ValidateAccountBalanceAsync(account.Id, account.Balance, cancellationToken);
            if (counterpartyAccount is not null)
                isSecondTransactionSuccess =
                    await accountRepository.ValidateAccountBalanceAsync(counterpartyAccount.Id,
                        counterpartyAccount.Balance, cancellationToken);

            if (!isFirstTransactionSuccess || !isSecondTransactionSuccess)
                throw new InvalidOperationException("Transaction failed due to account balance mismatch");

            // Если все прошло успешно, фиксируем транзакцию
            await unitOfWork.CommitAsync(cancellationToken);

            return transactionId;

            // Transaction end
        }
        catch
        {
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}