using AccountService.Core.Domain.Abstraction;
using AccountService.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccountService.DatabaseAccess.Repositories;

public class BaseRepository<T>(AccountServiceDbContext context) : IBaseRepository<T> where T : class, IIdentifiable
{
    protected AccountServiceDbContext Context = context;
    protected DbSet<T> DbSet => Context.Set<T>();

    public async Task<Guid> CreateAsync(T obj, CancellationToken cancellationToken) {
        await DbSet.AddAsync(obj, cancellationToken);
        var res = await Context.SaveChangesAsync(cancellationToken);
        return res > 0 ? obj.Id : Guid.Empty;
    }
}