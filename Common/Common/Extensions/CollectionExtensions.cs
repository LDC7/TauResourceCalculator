using System.Collections.Generic;

namespace TauResourceCalculator.Common.Extensions;

public static class CollectionExtensions
{
  public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> values)
  {
    if (collection is List<T> list)
    {
      list.AddRange(values);
      return;
    }

    foreach (var value in values)
      collection.Add(value);
  }
}
