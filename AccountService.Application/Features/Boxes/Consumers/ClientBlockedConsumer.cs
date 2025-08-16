using AccountService.Application.Features.Accounts.Domain;
using AccountService.Application.Features.Accounts.Events;
using MassTransit;

namespace AccountService.Application.Features.Boxes.Consumers;

public class ClientBlockedConsumer(IAccountRepository accountRepository) : IConsumer<ClientBlocked>
{
	public async Task Consume(ConsumeContext<ClientBlocked> context) 
	{
		await accountRepository.ToggleFrozenAccountAsync(context.Message.ClientId, true, context.CancellationToken);
	}
}