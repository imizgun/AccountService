using AccountService.Application.Features.Boxes.Domain;
using AccountService.Application.Shared.DatabaseAccess;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Features.Boxes.DatabaseAccess;

public class InboxConsumedRepository(AccountServiceDbContext dbContext) : IInboxConsumedRepository
{
    public async Task AddAsync(InboxConsumed consumed, CancellationToken cancellationToken = default)
    {
        await dbContext.InboxConsumed.AddAsync(consumed, cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await dbContext.InboxConsumed.AnyAsync(x => x.MessageId == messageId, cancellationToken);
    }
}