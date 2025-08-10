using MediatR;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;
// ReSharper disable once IdentifierTypo
public record UpdateTransactionCommand(Guid TransactionId, string Description) : IRequest<bool>;