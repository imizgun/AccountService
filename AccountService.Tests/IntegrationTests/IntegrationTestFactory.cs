using AccountService.DatabaseAccess;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace AccountService.Tests.IntegrationTests;

public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _dbContainer;
    private string? _connectionString;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = AuthTestHandler.MyScheme;
                options.DefaultChallengeScheme = AuthTestHandler.MyScheme;
                options.DefaultScheme = AuthTestHandler.MyScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, AuthTestHandler>(AuthTestHandler.MyScheme, _ => { });

            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AccountServiceDbContext>));
            if (descriptor is not null)
                services.Remove(descriptor);

            services.AddDbContext<AccountServiceDbContext>(opt => {
                opt.UseNpgsql(_connectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("accountService")
            .WithUsername("postgres")
            .WithPassword("123")
            .Build();

        await _dbContainer.StartAsync();

        _connectionString = _dbContainer.GetConnectionString();

        using var scope = Services.CreateScope();
        var sp = scope.ServiceProvider;

        var dbOptions = new DbContextOptionsBuilder<AccountServiceDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        var db = ActivatorUtilities.CreateInstance<AccountServiceDbContext>(sp, dbOptions);
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_dbContainer is not null)
            await _dbContainer.StopAsync();
    }
}
