using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.GetTransactions;

public record GetTransactionsCommand(Guid AccountId, int Take, int SkipPage) : IRequest<List<TransactionDto>>;