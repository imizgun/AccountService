using AccountService.Core.Domain.Abstraction;

namespace AccountService.DatabaseAccess.Repositories;

public class BaseRepository<T> : IBaseRepository<T> where T : class, IIdentifiable
{
    // protected AccountServiceDbContext Context = context;
    // protected DbSet<T> DbSet => Context.Set<T>();
    protected readonly List<T> DbSet = [];

    public async Task<Guid> CreateAsync(T obj, CancellationToken cancellationToken)
    {
        var lengthBefore = DbSet.Count;
        DbSet.Add(obj);
        return DbSet.Count > lengthBefore ? await Task.FromResult(obj.Id) : await Task.FromResult(Guid.Empty);
    }
}