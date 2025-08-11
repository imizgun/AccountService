using AccountService.Core.Features.Accounts;
using AccountService.Core.Features.Transactions;
using AccountService.DatabaseAccess.Features.Accounts;
using AccountService.DatabaseAccess.Features.Transactions;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess;

public class AccountServiceDbContext : DbContext 
{
	public AccountServiceDbContext(DbContextOptions<AccountServiceDbContext> options) : base(options) { }
	
	public AccountServiceDbContext() { }

	protected override void OnModelCreating(ModelBuilder modelBuilder) 
	{
		base.OnModelCreating(modelBuilder);
		modelBuilder.ApplyConfiguration(new AccountConfiguration());
		modelBuilder.ApplyConfiguration(new TransactionConfiguration());
	}
	
	public DbSet<Account> Accounts { get; set; }
    // ReSharper disable once UnusedMember.Global Поле используется как модель в БД
    public DbSet<Transaction> Transactions { get; set; }
}