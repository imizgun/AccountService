using AccountService.Application.Features.Boxes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Application.Features.Boxes.DatabaseAccess;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage> 
{
	public void Configure(EntityTypeBuilder<OutboxMessage> builder) 
	{
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Type).IsRequired();
		builder.Property(x => x.Payload).IsRequired();
		builder.Property(x => x.OccurredAt).IsRequired();
		builder.Property(x => x.PublishedAt).IsRequired(false);
		
	}
}