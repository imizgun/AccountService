using AccountService.Application.Features.Boxes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Application.Features.Boxes.DatabaseAccess;

public class InboxConsumedConfiguration : IEntityTypeConfiguration<InboxConsumed>
{
    public void Configure(EntityTypeBuilder<InboxConsumed> builder)
    {
        builder.Property(x => x.Handler).IsRequired();
        builder.Property(x => x.ProcessedAt).IsRequired();
        builder.HasKey(x => x.MessageId);
    }
}