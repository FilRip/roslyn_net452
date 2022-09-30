using System.Collections.Generic;

namespace Microsoft.CodeAnalysis
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
        {
            if (dictionary.TryGetValue(key, out var value2))
            {
                return value2;
            }
            dictionary.Add(key, value);
            return value;
        }
    }
}
