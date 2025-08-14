using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.DeleteTransaction;
public record DeleteTransactionCommand(Guid TransactionId) : IRequest<bool>;