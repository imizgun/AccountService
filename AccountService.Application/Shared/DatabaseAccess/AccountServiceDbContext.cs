using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Shared.DatabaseAccess;

public partial class AccountServiceDbContext : DbContext
{
    public AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : base(options) { }

    public AccountServiceDbContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        OnModelCreatingAccounts(modelBuilder);
        OnModelCreatingTransactions(modelBuilder);
        OnModelCreatingEvents(modelBuilder);
    }

    partial void OnModelCreatingAccounts(ModelBuilder modelBuilder);
    partial void OnModelCreatingTransactions(ModelBuilder modelBuilder);
    partial void OnModelCreatingEvents(ModelBuilder modelBuilder);
}