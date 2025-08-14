namespace AccountService.Application.Features.Queues.Domain;

public record TransferCompleted(
	Guid EventId,
	DateTime OccurredAt,
	Guid SourceAccountId,
	Guid DestinationAccountId,
	decimal Amount,
	string Currency,
	Guid TransferId
	);