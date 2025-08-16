namespace AccountService.Application.Features.Boxes.Domain;

public interface IInboxConsumedRepository
{
	Task AddAsync(InboxConsumed consumed, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(Guid messageId, CancellationToken cancellationToken = default);
}