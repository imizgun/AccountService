namespace AccountService.Application.Features.Boxes.Domain;

public interface IOutboxMessageRepository
{
    Task AddAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    Task<List<OutboxMessage>> TakePendingAsync(uint amount, CancellationToken cancellationToken = default);
    Task MarkAsPublishedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> GetUnprocessedMessagesAsync(CancellationToken cancellationToken = default);
}