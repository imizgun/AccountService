namespace AccountService.Application.Features.Interest.Events;

public record InterestAccrued(
	Guid EventId,
	DateTime OccurredAt,
	Guid AccountId,
	DateTime PeriodFrom,
	DateTime PeriodTo,
	decimal Amount
	);