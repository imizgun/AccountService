using AccountService.Application.Features.DTOs;
using MediatR;

namespace AccountService.Application.Features.Transactions.GetTransactions;

public record GetTransactionsCommand(Guid AccountId, int Take, int SkipPage) : IRequest<List<TransactionDto>>;