using AccountService.Core.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;

public class BaseRepository<T>(AccountServiceDbContext context) : IBaseRepository<T> where T : class, IIdentifiable
{
    protected DbSet<T> DbSet => context.Set<T>();

    public async Task<Guid> CreateAsync(T obj, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(obj, cancellationToken);
        var res = await context.SaveChangesAsync(cancellationToken);
        return res > 0 ? obj.Id : Guid.Empty;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(x => x.Id == id, cancellationToken);
    }
}