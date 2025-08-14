﻿namespace AccountService.Application.Features.Transactions.Events;

public record MoneyCredited(
	Guid EventId,
	DateTime OccurredAt,
	Guid AccountId,
	decimal Amount,
	string Currency,
	Guid OperationId
	);