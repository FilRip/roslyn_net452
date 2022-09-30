using System;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public class CachingIdentityFactory<TKey, TValue> : CachingBase<CachingIdentityFactory<TKey, TValue>.Entry> where TKey : class
    {
        public struct Entry
        {
            internal TKey key;

            internal TValue value;
        }

        private readonly Func<TKey, TValue> _valueFactory;

        private readonly ObjectPool<CachingIdentityFactory<TKey, TValue>>? _pool;

        public CachingIdentityFactory(int size, Func<TKey, TValue> valueFactory)
            : base(size)
        {
            _valueFactory = valueFactory;
        }

        public CachingIdentityFactory(int size, Func<TKey, TValue> valueFactory, ObjectPool<CachingIdentityFactory<TKey, TValue>> pool)
            : this(size, valueFactory)
        {
            _pool = pool;
        }

        public void Add(TKey key, TValue value)
        {
            int num = RuntimeHelpers.GetHashCode(key) & mask;
            entries[num].key = key;
            entries[num].value = value;
        }

        public bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            int num = RuntimeHelpers.GetHashCode(key) & mask;
            Entry[] array = entries;
            if (array[num].key == key)
            {
                value = array[num].value;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public TValue GetOrMakeValue(TKey key)
        {
            int num = RuntimeHelpers.GetHashCode(key) & mask;
            Entry[] array = entries;
            if (array[num].key == key)
            {
                return array[num].value;
            }
            TValue val = _valueFactory(key);
            array[num].key = key;
            array[num].value = val;
            return val;
        }

        public static ObjectPool<CachingIdentityFactory<TKey, TValue>> CreatePool(int size, Func<TKey, TValue> valueFactory)
        {
            Func<TKey, TValue> valueFactory2 = valueFactory;
            return new ObjectPool<CachingIdentityFactory<TKey, TValue>>((ObjectPool<CachingIdentityFactory<TKey, TValue>> pool) => new CachingIdentityFactory<TKey, TValue>(size, valueFactory2, pool), Environment.ProcessorCount * 2);
        }

        public void Free()
        {
            _pool?.Free(this);
        }
    }
}
