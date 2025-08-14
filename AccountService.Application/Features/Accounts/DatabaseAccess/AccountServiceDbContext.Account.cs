using AccountService.Application.Features.Accounts.DatabaseAccess;
using AccountService.Application.Features.Accounts.Domain;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс
namespace AccountService.Application.Shared.DatabaseAccess;

public partial class AccountServiceDbContext
{
    public DbSet<Account> Accounts { get; set; } = null!;

    partial void OnModelCreatingAccounts(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
    }
}