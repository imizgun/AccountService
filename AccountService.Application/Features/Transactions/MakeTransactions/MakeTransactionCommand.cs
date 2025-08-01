using MediatR;

namespace AccountService.Application.Features.Transactions.MakeTransactions;

public record MakeTransactionCommand(
    Guid AccountId,
    Guid? CounterpartyAccountId,
    string TransactionType,
    string Currency,
    decimal Amount,
    string Description) : IRequest<Guid>;