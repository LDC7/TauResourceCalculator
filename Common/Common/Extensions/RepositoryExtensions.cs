using System;
using System.Linq;
using System.Threading.Tasks;
using TauResourceCalculator.Common.Abstractions;

namespace TauResourceCalculator.Common.Extensions;

public static class RepositoryExtensions
{
  public static TEntity? Get<TEntity>(IRepository<TEntity> repository, Guid id)
    where TEntity : class, IIdentifiable
  {
    return repository.Get().FirstOrDefault(e => e.Id == id);
  }

  public static ValueTask Add<TEntity>(IRepository<TEntity> repository, params TEntity[] entities)
    where TEntity : class, IIdentifiable
  {
    return repository.Add(entities);
  }

  public static ValueTask Add<TEntity>(IRepository<TEntity> repository, TEntity entity)
    where TEntity : class, IIdentifiable
  {
    return repository.Add(new[] { entity });
  }

  public static ValueTask Remove<TEntity>(IRepository<TEntity> repository, params TEntity[] entities)
    where TEntity : class, IIdentifiable
  {
    return repository.Remove(entities);
  }

  public static ValueTask Remove<TEntity>(IRepository<TEntity> repository, TEntity entity)
    where TEntity : class, IIdentifiable
  {
    return repository.Remove(new[] { entity });
  }
}
