using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TauResourceCalculator.Common.Abstractions;

public interface IRepository<TEntity>
  where TEntity : class, IIdentifiable
{
  IQueryable<TEntity> Get();

  ValueTask Add(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

  ValueTask Remove(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

  ValueTask SaveChanges(CancellationToken cancellationToken = default);
}
