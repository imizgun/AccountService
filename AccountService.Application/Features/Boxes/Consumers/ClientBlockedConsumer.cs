using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Boxes.Consumers;

public class ClientBlockedConsumer(
	IAccountRepository accountRepository,
	ILogger<ClientBlockedConsumer> logger) : IConsumer<ClientBlocked>
{
	public async Task Consume(ConsumeContext<ClientBlocked> context) 
	{
		logger.LogInformation("[ClientBlockedConsumer] Processing ClientBlocked event for ClientId: {ClientId}", context.Message.ClientId);
		await accountRepository.ToggleFrozenAccountAsync(context.Message.ClientId, true, context.CancellationToken);
	}
}