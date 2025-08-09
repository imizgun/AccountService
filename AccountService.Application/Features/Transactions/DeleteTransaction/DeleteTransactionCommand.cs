using MediatR;

namespace AccountService.Application.Features.Transactions.DeleteTransaction;
// ReSharper disable once IdentifierTypo
public record DeleteTransactionCommand(Guid TransactionId, uint Xmin) : IRequest<bool>;