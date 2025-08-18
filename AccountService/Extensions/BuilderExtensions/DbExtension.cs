using AccountService.Application.Shared.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Extensions.BuilderExtensions;

public static class DbExtensions
{
    public static void AddCustomDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountServiceDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString(nameof(AccountServiceDbContext))));
    }
}