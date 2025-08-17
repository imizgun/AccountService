using AccountService.Application.Features.Boxes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Application.Features.Boxes.DatabaseAccess;

public class InboxDeadLetterConfiguration : IEntityTypeConfiguration<InboxDeadLetter> 
{
	public void Configure(EntityTypeBuilder<InboxDeadLetter> builder) 
	{
		builder.HasKey(x => x.MessageId);
		builder.Property(x => x.Error).IsRequired();
		builder.Property(x => x.ReceivedAt).IsRequired();
		builder.Property(x => x.Handler).IsRequired();
		builder.Property(x => x.Payload).IsRequired();
	}
}