namespace AccountService.Application.Features.Transactions.Events;

public record TransferCompleted(
	Guid EventId,
	DateTime OccurredAt,
	Guid SourceAccountId,
	Guid DestinationAccountId,
	decimal Amount,
	string Currency,
	Guid TransferId
	);