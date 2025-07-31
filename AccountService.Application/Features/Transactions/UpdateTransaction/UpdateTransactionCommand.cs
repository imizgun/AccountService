using MediatR;

namespace AccountService.Application.Features.Transactions.UpdateTransaction;

public record UpdateTransactionCommand(Guid TransactionId, string Description) : IRequest<bool>;