
using AccountService.Application.Features.Boxes.DatabaseAccess;
using AccountService.Application.Features.Boxes.Domain;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace Это частичный класс и у него должен быть тот же неймспейс
namespace AccountService.Application.Shared.DatabaseAccess;

public partial class AccountServiceDbContext
{
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<InboxConsumed> InboxConsumed { get; set; } = null!;
    public DbSet<InboxDeadLetter> InboxDeadLetters { get; set; } = null!;
    partial void OnModelCreatingEvents(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxConsumedConfiguration());
        modelBuilder.ApplyConfiguration(new InboxDeadLetterConfiguration());
    }
}