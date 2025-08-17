using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace AccountService.Application.Features.Boxes.Consumers;

public class ClientUnblockedConsumer(
	IAccountRepository accountRepository,
	ILogger<ClientUnblockedConsumer> logger) : IConsumer<ClientUnblocked> 
{
	public async Task Consume(ConsumeContext<ClientUnblocked> context) 
	{
		logger.LogInformation("[ClientUnblockedConsumer] Processing ClientUnblocked event for ClientId: {ClientId}", context.Message.ClientId);
		
		await accountRepository.ToggleFrozenAccountAsync(context.Message.ClientId, false, context.CancellationToken);
	}
}