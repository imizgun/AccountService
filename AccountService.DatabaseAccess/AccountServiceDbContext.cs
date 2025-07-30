using AccountService.Core.Domain.Entities;
using AccountService.DatabaseAccess.Configurations;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess;

public class AccountServiceDbContext : DbContext
{
    public AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
}