using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Infrastructure.Data;

public sealed class EntityFrameworkRepository<TEntity> : IRepository<TEntity>
  where TEntity : class, IIdentifiable
{
  private readonly ApplicationDbContext dbContext;
  private readonly DbSet<TEntity> dbSet;

  public EntityFrameworkRepository(ApplicationDbContext dbContext)
  {
    this.dbContext = dbContext;
    this.dbSet = this.dbContext.Set<TEntity>();
  }

  public IQueryable<TEntity> Get()
  {
    return this.dbSet;
  }

  public async ValueTask Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    await this.dbSet.AddRangeAsync(entities, cancellationToken);
  }

  public ValueTask Remove(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    this.dbSet.RemoveRange(entities);
    return new ValueTask();
  }

  public async ValueTask SaveChanges(CancellationToken cancellationToken = default)
  {
    await this.dbContext.SaveChangesAsync(cancellationToken);
  }
}
