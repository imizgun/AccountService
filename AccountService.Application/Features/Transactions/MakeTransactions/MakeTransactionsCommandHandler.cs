using AccountService.Core.Features.Accounts;
using AccountService.Core.Features.Transactions;
using AccountService.DatabaseAccess.Abstractions;
using MediatR;

namespace AccountService.Application.Features.Transactions.MakeTransactions;

public class MakeTransactionsCommandHandler(
    ITransactionRepository transactionRepository, 
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<MakeTransactionCommand, Guid>
{
    public async Task<Guid> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
    {
        if (request.AccountId == request.CounterpartyAccountId)
            throw new InvalidOperationException("You can't make transactions to the same account");

        if (!Enum.TryParse<ETransactionType>(request.TransactionType, out var transactionType))
            throw new InvalidOperationException("Transaction type is not valid");

        // Transaction start

        await unitOfWork.BeginTransactionAsync(cancellationToken);

            try {
                var account = await accountRepository.GetByIdForUpdateAsync(request.AccountId, cancellationToken)
                              ?? throw new InvalidOperationException("Account not found");

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

                if (transactionType == ETransactionType.Debit)
                    account.Withdraw(request.Amount);
                else
                    account.Deposit(request.Amount);

                var transactionId = await transactionRepository.CreateAsync(transaction, cancellationToken);
                // await accountRepository.UpdateAccount(account, cancellationToken);

                if (counterpartyAccount is not null) {
                    var counterpartyTransaction = transaction.GetReverseTransaction();
                    counterpartyAccount.Deposit(request.Amount);
                    await transactionRepository.CreateAsync(counterpartyTransaction, cancellationToken);
                }

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