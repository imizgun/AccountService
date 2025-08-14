namespace AccountService.Application.Features.Queues.Domain;

public record InterestAccrued(
	Guid EventId,
	DateTime OccurredAt,
	Guid AccountId,
	DateTime PeriodFrom,
	DateTime PeriodTo,
	decimal Amount
	);