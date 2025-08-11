using Hangfire.Dashboard;

namespace AccountService.Configs;

public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context)
	{
		return true;
	}
}