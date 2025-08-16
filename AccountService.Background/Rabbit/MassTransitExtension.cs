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
        
        services.AddScoped(typeof(InboxFilter<>));

        services.AddMassTransit(busCfg =>
        {
            busCfg.AddConsumer<ClientBlockedConsumer>();
            busCfg.AddConsumer<ClientUnblockedConsumer>();
            
            busCfg.UsingRabbitMq((context, rabbit) =>
            {
                rabbit.Host(host, vhost, h =>
                {
                    h.Username(user);
                    h.Password(pass);
                });
                

                rabbit.Message<AccountOpened>(x => x.SetEntityName("account.events"));
                rabbit.Message<AccountUpdated>(x => x.SetEntityName("account.events"));
                rabbit.Message<AccountClosed>(x => x.SetEntityName("account.events"));
                rabbit.Message<MoneyCredited>(x => x.SetEntityName("account.events"));
                rabbit.Message<MoneyDebited>(x => x.SetEntityName("account.events"));
                rabbit.Message<TransferCompleted>(x => x.SetEntityName("account.events"));
                rabbit.Message<TransactionDeleted>(x => x.SetEntityName("account.events"));
                rabbit.Message<TransactionUpdated>(x => x.SetEntityName("account.events"));
                rabbit.Message<InterestAccrued>(x => x.SetEntityName("account.events"));

                rabbit.Publish<AccountOpened>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<AccountUpdated>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<AccountClosed>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<MoneyCredited>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<MoneyDebited>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<TransferCompleted>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<TransactionDeleted>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<TransactionUpdated>(x => x.ExchangeType = ExchangeType.Topic);
                rabbit.Publish<InterestAccrued>(x => x.ExchangeType = ExchangeType.Topic);
                
                rabbit.Publish<DefaultEvent>(x => x.Exclude = true);
                rabbit.Publish<IEventIdentifiable>(x => x.Exclude = true);
                

                rabbit.ReceiveEndpoint("account.crm", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Bind("account.events", s =>
                    {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "account.*";
                    });
                    
                    e.AutoStart = false;
                });

                rabbit.ReceiveEndpoint("account.notifications", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Bind("account.events", s => {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "money.*";
                    });
                    
                    e.AutoStart = false;
                });

                rabbit.ReceiveEndpoint("account.antifraud", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.PrefetchCount = 1;
                    e.Bind("account.events", s =>
                    {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "client.#";
                    });
                    
                    e.UseConsumeFilter(typeof(InboxFilter<>), context);

                    e.ConfigureConsumer<ClientBlockedConsumer>(context);
                    e.ConfigureConsumer<ClientUnblockedConsumer>(context);
                });

                rabbit.ReceiveEndpoint("account.audit", e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Bind("account.events", s =>
                    {
                        s.ExchangeType = ExchangeType.Topic;
                        s.RoutingKey = "#";
                    });

                    e.AutoStart = false;
                });
            });
        });

        return services;
    }
}
