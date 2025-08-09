using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.DatabaseAccess.Configurations;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new {x.AccountId, x.TransactionDate})
            .HasDatabaseName("IX_Transactions_AccountId_Date");
        
        builder.HasIndex(x => x.TransactionDate)
            .HasDatabaseName("IX_Transactions_Date_gist")
            .HasMethod("gist");
        
        builder.Property(x => x.Xmin)
            // ReSharper disable once StringLiteralTypo
            .HasColumnName("xmin")
            .IsRowVersion()
            .ValueGeneratedOnAddOrUpdate();
        
        builder.Property(x => x.AccountId).IsRequired();
        builder.Property(x => x.Currency).IsRequired();
        builder.Property(x => x.Description).IsRequired().HasMaxLength(Transaction.MaxDescriptionLength);
        builder.Property(x => x.TransactionDate).IsRequired();
        builder.Property(x => x.Amount).IsRequired();
        builder.Property(x => x.TransactionType).IsRequired();
    }
}