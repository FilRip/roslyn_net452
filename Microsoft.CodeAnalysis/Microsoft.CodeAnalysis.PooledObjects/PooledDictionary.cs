using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.PooledObjects
{
    public sealed class PooledDictionary<K, V> : Dictionary<K, V> where K : notnull
    {
        private readonly ObjectPool<PooledDictionary<K, V>> _pool;

        private static readonly ObjectPool<PooledDictionary<K, V>> s_poolInstance = CreatePool(EqualityComparer<K>.Default);

        private PooledDictionary(ObjectPool<PooledDictionary<K, V>> pool, IEqualityComparer<K> keyComparer)
            : base(keyComparer)
        {
            _pool = pool;
        }

        public ImmutableDictionary<K, V> ToImmutableDictionaryAndFree()
        {
            ImmutableDictionary<K, V> result = this.ToImmutableDictionary(base.Comparer);
            Free();
            return result;
        }

        public ImmutableDictionary<K, V> ToImmutableDictionary()
        {
            return this.ToImmutableDictionary(base.Comparer);
        }

        public void Free()
        {
            Clear();
            _pool?.Free(this);
        }

        public static ObjectPool<PooledDictionary<K, V>> CreatePool(IEqualityComparer<K> keyComparer)
        {
            IEqualityComparer<K> keyComparer2 = keyComparer;
            ObjectPool<PooledDictionary<K, V>> pool = null;
            pool = new ObjectPool<PooledDictionary<K, V>>(() => new PooledDictionary<K, V>(pool, keyComparer2), 128);
            return pool;
        }

        public static PooledDictionary<K, V> GetInstance()
        {
            return s_poolInstance.Allocate();
        }
    }
}
