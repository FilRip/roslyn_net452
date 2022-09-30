using System;
using System.Collections.Concurrent;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Roslyn.Utilities
{
    public static class ConcurrentDictionaryExtensions
    {
        public static void Add<K, V>(this ConcurrentDictionary<K, V> dict, K key, V value) where K : notnull
        {
            if (!dict.TryAdd(key, value))
            {
                throw new ArgumentException("adding a duplicate", "key");
            }
        }

        public static TValue GetOrAdd<TKey, TArg, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument) where TKey : notnull
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            using (PooledDelegates.GetPooledFunction(valueFactory, factoryArgument, out Func<TKey, TValue> boundFunction))
            {
                return dictionary.GetOrAdd(key, boundFunction);
            }
        }
    }
}
