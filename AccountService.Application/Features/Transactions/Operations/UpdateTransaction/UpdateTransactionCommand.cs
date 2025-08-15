using MediatR;

namespace AccountService.Application.Features.Transactions.Operations.UpdateTransaction;
// ReSharper disable once IdentifierTypo
public record UpdateTransactionCommand(Guid TransactionId, string Description, Guid CorrelationId) : IRequest<bool>;