using AccountService.Application.Features.Accounts.Events;
using AccountService.Application.Features.Boxes.Consumers;
using AccountService.Application.Features.Interest.Events;
using AccountService.Application.Features.Transactions.Events;
using AccountService.Application.Shared.Events;
using AccountService.Background.Rabbit.Filters;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AccountService.Background.Rabbit;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration cfg)
    {
        var host = cfg["RabbitMQ:Host"] ?? "rabbitmq";
        var user = cfg["RabbitMQ:Username"] ?? "guest";
        var pass = cfg["RabbitMQ:Password"] ?? "guest";
        var vhost = cfg["RabbitMQ:VHost"] ?? "/";
        var port = int.TryParse(cfg["RabbitMQ:Port"], out var p) ? p : 5672;
        var vhostPath = vhost == "/" ? "/" : vhost.StartsWith('/') ? vhost : "/" + vhost;

        var factory = new ConnectionFactory { Uri = new Uri($"amqp://{user}:{pass}@{host}:{port}/") };
        using var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
        using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

        channel.ExchangeDeclareAsync("account.events", ExchangeType.Topic, durable: true, autoDelete: false).GetAwaiter().GetResult();

        channel.QueueDeclareAsync("account.crm", durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
        channel.QueueBindAsync("account.crm", "account.events", "account.*").GetAwaiter().GetResult();

        channel.QueueDeclareAsync("account.audit", durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
        channel.QueueBindAsync("account.audit", "account.events", "#").GetAwaiter().GetResult();

        channel.QueueDeclareAsync("account.notifications", durable: true, exclusive: false, autoDelete: false).GetAwaiter().GetResult();
        channel.QueueBindAsync("account.audit", "account.events", "money.#").GetAwaiter().GetResult();

        services.AddScoped(typeof(InboxFilter<>));

        services.AddMassTransit(busCfg =>
        {
            busCfg.AddConsumer<ClientBlockedConsumer>();
            busCfg.AddConsumer<ClientUnblockedConsumer>();
            busCfg.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(new Uri($"rabbitmq://{host}:{port}{vhostPath}"), h =>
                {
                    h.Username(user);
                    h.Password(pass);
                });

                rabbit.Message<DefaultEvent>(x => x.SetEntityName("account.events"));
                rabbit.Message<IEventIdentifiable>(x => x.SetEntityName("account.events"));
                rabbit.Message<AccountOpened>(x => x.SetEntityName("account.events"));
                rabbit.Message<AccountUpdated>(x => x.SetEntityName("account.events"));
                rabbit.Message<AccountClosed>(x => x.SetEntityName("account.events"));
                rabbit.Message<MoneyCredited>(x => x.SetEntityName("account.events"));
                rabbit.Message<MoneyDebited>(x => x.SetEntityName("account.events"));
                rabbit.Message<TransferCompleted>(x => x.SetEntityName("account.events"));
                rabbit.Message<TransactionDeleted>(x => x.SetEntityName("account.events"));
                rabbit.Message<TransactionUpdated>(x => x.SetEntityName("account.events"));
                rabbit.Message<InterestAccrued>(x => x.SetEntityName("account.events"));
                rabbit.Message<ClientBlocked>(x => x.SetEntityName("account.events"));
                rabbit.Message<ClientUnblocked>(x => x.SetEntityName("account.events"));

                rabbit.Publish<DefaultEvent>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<IEventIdentifiable>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<AccountOpened>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<AccountUpdated>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<AccountClosed>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<MoneyCredited>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<MoneyDebited>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<TransferCompleted>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<TransactionDeleted>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<TransactionUpdated>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<InterestAccrued>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<ClientBlocked>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<ClientUnblocked>(x => x.ExchangeType = ExchangeType.Topic);

                rabbit.ReceiveEndpoint("account.antifraud", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.PrefetchCount = 1;
                    e.Bind("account.events", s =>
                    {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "client.#";
                    });

                    e.Bind<ClientBlocked>(s =>
                    {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "client.blocked";
                    });
                    e.Bind<ClientUnblocked>(s =>
                    {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "client.unblocked";
                    });

                    e.UseConsumeFilter(typeof(EnvelopeFilter<>), context);
                    e.UseConsumeFilter(typeof(InboxFilter<>), context);

                    e.ConfigureConsumer<ClientBlockedConsumer>(context);
                    e.ConfigureConsumer<ClientUnblockedConsumer>(context);
                });
            });
        });

        return services;
    }
}
