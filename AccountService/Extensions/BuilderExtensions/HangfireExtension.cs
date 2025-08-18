using AccountService.Background.DailyAccrueInterestRate;
using Hangfire;
using Hangfire.PostgreSql;

namespace AccountService.Extensions.BuilderExtensions;

public static class HangfireExtensions
{
    public static void AddCustomHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfireWithPostgres(configuration);
        services.AddScoped<AccrueInterestRateJob>();
    }

    private static void AddHangfireWithPostgres(this IServiceCollection services, IConfiguration cfg)
    {
        var conn = cfg.GetConnectionString("AccountServiceDbContext");
        services.AddHangfire(options =>
        {
            options
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseColouredConsoleLogProvider()
                .UsePostgreSqlStorage(config => config.UseNpgsqlConnection(conn), new PostgreSqlStorageOptions
                {
                    PrepareSchemaIfNecessary = true,
                    SchemaName = "hangfire"
                });
        });

        services.AddHangfireServer();
    }
}