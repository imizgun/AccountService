namespace AccountService.Application.Features.Queues.Domain;

public record MoneyCredited(
	Guid EventId,
	DateTime OccurredAt,
	Guid AccountId,
	decimal Amount,
	string Currency,
	Guid OperationId
	);