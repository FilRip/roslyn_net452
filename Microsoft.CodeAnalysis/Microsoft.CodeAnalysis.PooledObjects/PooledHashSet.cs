using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.PooledObjects
{
    public sealed class PooledHashSet<T> : HashSet<T>
    {
        private readonly ObjectPool<PooledHashSet<T>> _pool;

        private static readonly ObjectPool<PooledHashSet<T>> s_poolInstance = CreatePool(EqualityComparer<T>.Default);

        private PooledHashSet(ObjectPool<PooledHashSet<T>> pool, IEqualityComparer<T> equalityComparer)
            : base(equalityComparer)
        {
            _pool = pool;
        }

        public void Free()
        {
            Clear();
            _pool?.Free(this);
        }

        public static ObjectPool<PooledHashSet<T>> CreatePool(IEqualityComparer<T> equalityComparer)
        {
            IEqualityComparer<T> equalityComparer2 = equalityComparer;
            ObjectPool<PooledHashSet<T>> pool = null;
            pool = new ObjectPool<PooledHashSet<T>>(() => new PooledHashSet<T>(pool, equalityComparer2), 128);
            return pool;
        }

        public static PooledHashSet<T> GetInstance()
        {
            return s_poolInstance.Allocate();
        }
    }
}
