using AccountService.Background.DailyAccrueInterestRate;
using Hangfire;
using Hangfire.PostgreSql;

namespace AccountService.Configs;

public static class HangfireConfig
{
	public static IServiceCollection AddHangfireWithPostgres(this IServiceCollection services, IConfiguration cfg)
	{
		var conn = cfg.GetConnectionString("AccountServiceDbContext");
		services.AddHangfire(options =>
		{
			options
				.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
				.UseSimpleAssemblyNameTypeSerializer()
				.UseRecommendedSerializerSettings()
				.UseColouredConsoleLogProvider()
				.UsePostgreSqlStorage(config => config.UseNpgsqlConnection(conn) , new PostgreSqlStorageOptions
				{
					PrepareSchemaIfNecessary = true,
					SchemaName = "hangfire"
				});
		});

		services.AddHangfireServer();

		return services;
	}

	public static void AddDailyInterestRecurringJob(this IApplicationBuilder app, TimeZoneInfo tz)
	{
		const string jobId = "daily-accrue-interest";
		RecurringJob.AddOrUpdate<AccrueInterestRateJob>(
			jobId,
			job => job.RunJobAsync(CancellationToken.None),
			cronExpression: "0 2 * * *",
			new RecurringJobOptions {
				TimeZone = tz
			});
	}
}