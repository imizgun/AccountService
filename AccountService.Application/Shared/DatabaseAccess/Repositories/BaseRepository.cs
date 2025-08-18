using AccountService.Application.Shared.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Application.Shared.DatabaseAccess.Repositories;

public class BaseRepository<T>(AccountServiceDbContext context) : IBaseRepository<T> where T : class, IIdentifiable
{
    protected DbSet<T> DbSet => context.Set<T>();

    public async Task<Guid> CreateAsync(T obj, CancellationToken cancellationToken)
    {
        await DbSet.AddAsync(obj, cancellationToken);
        return obj.Id;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await DbSet.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await DbSet.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task<T?> GetByIdForUpdateAsync(Guid id, CancellationToken cancellationToken) =>
        await DbSet
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
}