using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Features.Boxes.DatabaseAccess;

public class OutboxMessageRepository(AccountServiceDbContext dbContext) : IOutboxMessageRepository
{
	public async Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default) 
	{
		await dbContext.OutboxMessages.AddAsync(message, cancellationToken);
	}

	public async Task<List<OutboxMessage>> TakePendingAsync(uint amount, CancellationToken cancellationToken = default) 
	{
		return await dbContext.OutboxMessages
			.Where(x => x.PublishedAt == null)
			.OrderBy(x => x.OccurredAt)
			.Take((int)amount)
			.ToListAsync(cancellationToken);
	}

	public async Task MarkAsPublishedAsync(Guid id, CancellationToken cancellationToken = default) 
	{
		await dbContext.OutboxMessages
			.Where(x => x.Id == id)
			.ExecuteUpdateAsync(x => x.SetProperty(m => m.PublishedAt, _ => DateTime.UtcNow), cancellationToken);
	}
}