using AccountService.Background.DailyAccrueInterestRate;
using AccountService.Filters;
using Hangfire;

namespace AccountService.Extensions.AppExtensions;

public static class HangfireAppExtensions
{
    public static void UseCustomHangfire(this WebApplication app)
    {
        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            Authorization = [new AllowAllDashboardAuthorizationFilter()]
        });

        app.AddDailyInterestRecurringJob(TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
    }


    // ReSharper disable once UnusedParameter.Local Нужно для метода-расширения
    private static void AddDailyInterestRecurringJob(this IApplicationBuilder app, TimeZoneInfo tz)
    {
        const string jobId = "daily-accrue-interest";
        RecurringJob.AddOrUpdate<AccrueInterestRateJob>(
            jobId,
            job => job.RunJobAsync(CancellationToken.None),
            cronExpression: "0 2 * * *",
            new RecurringJobOptions
            {
                TimeZone = tz
            });
    }
}