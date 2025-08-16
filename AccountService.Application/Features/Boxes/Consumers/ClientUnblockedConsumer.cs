using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using MassTransit;

namespace AccountService.Application.Features.Boxes.Consumers;

public class ClientUnblockedConsumer(IAccountRepository accountRepository) : IConsumer<ClientUnblocked> 
{
	public async Task Consume(ConsumeContext<ClientUnblocked> context) 
	{
		await accountRepository.ToggleFrozenAccountAsync(context.Message.ClientId, false, context.CancellationToken);
	}
}