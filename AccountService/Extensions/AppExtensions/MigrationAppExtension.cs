using AccountService.Application.Shared.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Extensions.AppExtensions;

public static class MigrationAppExtension
{
    /// <summary>
    /// Применяет EF Core миграции при старте приложения
    /// </summary>
    public static void ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountServiceDbContext>();
        db.Database.Migrate();
    }
}