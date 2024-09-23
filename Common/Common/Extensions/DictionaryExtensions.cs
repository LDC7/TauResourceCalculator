using System.Collections.Generic;

namespace TauResourceCalculator.Common.Extensions;

public static class DictionaryExtensions
{
  public static void AddToCollection<TKey, TCollection, TValue>(this IDictionary<TKey, TCollection> dictionary, TKey key, TValue value)
    where TCollection : class, ICollection<TValue>, new()
  {
    if (!dictionary.TryGetValue(key, out var collection))
    {
      collection = new TCollection();
      dictionary.Add(key, collection);
    }

    collection.Add(value);
  }
}
