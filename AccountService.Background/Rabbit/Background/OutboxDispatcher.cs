using System.Text.Json;
using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.Events;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AccountService.Background.Rabbit.Background;

public class OutboxDispatcher(
	IServiceScopeFactory scopeFactory,
	IBus publishEndpoint
	) : BackgroundService 
{
	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		while (!stoppingToken.IsCancellationRequested)
		{
			using var scope = scopeFactory.CreateScope();
			var outbox = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
			var batch = await outbox.TakePendingAsync(100, stoppingToken);

			foreach (var msg in batch)
			{
				try {
					var meta = JsonSerializer.Deserialize<DefaultEvent>(msg.Payload);
                    
					await publishEndpoint.Publish(msg, ctx =>
					{
						if (meta!.Meta.CorrelationId != Guid.Empty)
							ctx.Headers.Set("X-Correlation-Id", meta.Meta.CorrelationId.ToString());
						if (meta.Meta.CausationId != Guid.Empty)
							ctx.Headers.Set("X-Causation-Id", meta.Meta.CausationId.ToString());
						if (!string.IsNullOrWhiteSpace(meta.Meta.Version))
							ctx.Headers.Set("meta.version", meta.Meta.Version);
						
					}, stoppingToken);

					await outbox.MarkAsPublishedAsync(msg.Id, stoppingToken);
				}
				catch (Exception ex)
				{
				}
			}

			await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
		}
	}
}