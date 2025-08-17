using System.Diagnostics;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.RabbitMq;

namespace AccountService.Tests.IntegrationTests;

public class RabbitIntegrationTestFactory : IntegrationTestWebFactory
{
    private RabbitMqContainer? _rabbit;
    private int _rabbitPort;
    private readonly string _rabbitUser = "admin";
    private readonly string _rabbitPass = "admin";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => {
            services.Configure<MassTransitHostOptions>(options => {
                options.WaitUntilStarted = true;
                options.StartTimeout = TimeSpan.FromSeconds(20);
                options.StopTimeout = TimeSpan.FromSeconds(20);
            });
        });
        
        base.ConfigureWebHost(builder);
    }

    public override async Task InitializeAsync()
    {
        _rabbit = new RabbitMqBuilder()
            .WithImage("rabbitmq:4-management")
            .WithUsername(_rabbitUser)
            .WithPassword(_rabbitPass)
            .WithPortBinding(0, 5672)
            .Build();

        await _rabbit.StartAsync();
        _rabbitPort = _rabbit.GetMappedPublicPort(5672);
        
        Environment.SetEnvironmentVariable("RabbitMQ__Host", "localhost");
        Environment.SetEnvironmentVariable("RabbitMQ__Port", _rabbitPort.ToString());
        Environment.SetEnvironmentVariable("RabbitMQ__Username", _rabbitUser);
        Environment.SetEnvironmentVariable("RabbitMQ__Password", _rabbitPass);
        Environment.SetEnvironmentVariable("RabbitMQ__VHost", "/");
        
        await base.InitializeAsync();
    }

    public override async Task DisposeAsync()
    {
        await base.DisposeAsync();
        if (_rabbit is not null) await _rabbit.StopAsync();
    }
    
    public async Task StopBrokerAsync() => await _rabbit!.ExecAsync(["rabbitmqctl", "stop_app"]);
    public async Task StartBrokerAsync() => await _rabbit!.ExecAsync(["rabbitmqctl", "start_app"]);

    public static async Task WaitUntilAsync(
        Func<Task<bool>> predicate,
        TimeSpan timeout,
        TimeSpan? pollInterval = null,
        CancellationToken cancellationToken = default) {
        var interval = pollInterval ?? TimeSpan.FromMilliseconds(50);
        var sw = Stopwatch.StartNew();

        while (sw.Elapsed < timeout) {
            cancellationToken.ThrowIfCancellationRequested();

            bool ok;
            try {
                ok = await predicate().ConfigureAwait(false);
            }
            catch (OperationCanceledException) {
                throw;
            }
            catch {
                ok = false;
            }

            if (ok) return;

            await Task.Delay(interval, cancellationToken).ConfigureAwait(false);
        }

        throw new TimeoutException($"Condition not met within {timeout.TotalMilliseconds:N0} ms.");
    }
}