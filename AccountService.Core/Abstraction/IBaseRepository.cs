namespace AccountService.Core.Abstraction;

public interface IBaseRepository<T> where T : class, IIdentifiable
{
    Task<Guid> CreateAsync(T obj, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<T?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken);
}