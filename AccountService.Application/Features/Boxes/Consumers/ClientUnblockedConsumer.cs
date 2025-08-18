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
        logger.LogInformation("[ClientBlockedConsumer] Processing ClientUnblocked event: {@event}", context.Message);

        await accountRepository.ToggleFrozenAccountAsync(context.Message.ClientId, false, context.CancellationToken);
    }
}