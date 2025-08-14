using AccountService.Application.Features.Transactions.DatabaseAccess;
using AccountService.Application.Features.Transactions.Domain;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace Это частичный класс, у него должнен быть тот же неймспейс
namespace AccountService.Application.Shared.DatabaseAccess;

public partial class AccountServiceDbContext
{
    // ReSharper disable once UnusedMember.Global Поле используется как модель в БД

    public DbSet<Transaction> Transactions { get; set; } = null!;

    partial void OnModelCreatingTransactions(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
    }
}