using MediatR;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;
public record DeleteTransactionCommand(Guid TransactionId) : IRequest<bool>;