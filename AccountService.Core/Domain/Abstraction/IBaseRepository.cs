namespace AccountService.Core.Domain.Abstraction;

public interface IBaseRepository<in T> where T : class, IIdentifiable
{
    Task<Guid> CreateAsync(T obj, CancellationToken cancellationToken);
}