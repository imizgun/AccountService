using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.DatabaseAccess.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountId).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.Description).IsRequired();
        builder.Property(x => x.TransactionDate).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.TransactionType).IsRequired();
    }
}