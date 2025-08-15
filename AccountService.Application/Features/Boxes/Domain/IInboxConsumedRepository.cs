namespace AccountService.Application.Features.Boxes.Domain;

public interface IInboxConsumedRepository
{
	Task<bool> AddAsync(InboxConsumed consumed, CancellationToken cancellationToken = default);
	Task<bool> ExistsAsync(Guid messageId, string handler, CancellationToken cancellationToken = default);
}