using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.DatabaseAccess.Configurations;

// Конфигурация на будущее взаимодействие с БД
public class AccountConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountType).IsRequired();
        builder.Property(x => x.Balance).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.OpeningDate).IsRequired();
        builder.Property(x => x.OwnerId).IsRequired();

        builder.HasMany(x => x.Transactions)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}