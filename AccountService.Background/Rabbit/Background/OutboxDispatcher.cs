using System.Text.Json;
using AccountService.Application.Features.Accounts.Events;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Features.Interest.Events;
using AccountService.Application.Features.Transactions.Events;
using AccountService.Application.Shared.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AccountService.Background.Rabbit.Background;

public class OutboxDispatcher(
	IServiceScopeFactory scopeFactory,
	ILogger<OutboxDispatcher> logger
	) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = scopeFactory.CreateScope();
			var outbox = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
			var bus = scope.ServiceProvider.GetRequiredService<IBus>();
			var batch = await outbox.TakePendingAsync(100, stoppingToken);
			
			foreach (var msg in batch)
			{
				try {
					var eventType = Type.GetType(msg.Type);
					var @event = (DefaultEvent?)JsonSerializer.Deserialize(msg.Payload, eventType!);
					if (eventType == null || @event == null) continue;
                    
					await bus.Publish(@event, ctx =>
					{
						if (@event.EventId != Guid.Empty)
							ctx.MessageId = @event.EventId;
						if (@event.Meta.CorrelationId != Guid.Empty)
							ctx.Headers.Set("X-Correlation-Id", @event.Meta.CorrelationId.ToString());
						if (@event.Meta.CausationId != Guid.Empty)
							ctx.Headers.Set("X-Causation-Id", @event.Meta.CausationId.ToString());
						if (!string.IsNullOrWhiteSpace(@event.Meta.Version))
							ctx.Headers.Set("meta.version", @event.Meta.Version);
						
						ctx.SetRoutingKey(RoutingKeyFor(eventType));
						
					}, stoppingToken);
					

					await outbox.MarkAsPublishedAsync(msg.Id, stoppingToken);
					logger.LogInformation("Publishing event {EventType} with ID {EventId} and Correlation ID {CorrelationId}",
						eventType.Name, @event.EventId, @event.Meta.CorrelationId);
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Failed to publish event: {Message}", ex.Message);
					throw;
				}
			}

			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}
	}
	
	private static string RoutingKeyFor(Type t) => t.Name switch
	{
		nameof(AccountOpened)      => "account.opened",
		nameof(AccountClosed)      => "account.closed",
		nameof(AccountUpdated)     => "account.updated",
		nameof(MoneyCredited)      => "money.credited",
		nameof(MoneyDebited)       => "money.debited",
		nameof(TransferCompleted)  => "money.completed",
		nameof(TransactionDeleted) => "money.deleted",
		nameof(TransactionUpdated) => "money.updated",
		nameof(InterestAccrued)    => "account.accrued",
		_ => string.Empty
	};
}