using AccountService.Application.Shared.DatabaseAccess;
using RabbitMQ.Client;

namespace AccountService.Extensions.BuilderExtensions;

public static class HealthCheckExtensions
{
    public static void AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = configuration.GetSection("RabbitMQ");

        var rabbitHost = rabbitConfig["Host"];
        var rabbitPort = rabbitConfig["Port"];
        var rabbitUser = rabbitConfig["Username"];
        var rabbitPass = rabbitConfig["Password"];

        var rabbitConnectionString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}:{rabbitPort}/";

        services.AddSingleton<IConnection>(_ =>
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(rabbitConnectionString)
            };
            return factory.CreateConnectionAsync().GetAwaiter().GetResult();
        }).AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString(nameof(AccountServiceDbContext))!)
            .AddRabbitMQ();
    }
}