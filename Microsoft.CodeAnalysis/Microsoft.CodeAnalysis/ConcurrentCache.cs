using System.Collections.Generic;

namespace Microsoft.CodeAnalysis
{
    public class ConcurrentCache<TKey, TValue> : CachingBase<ConcurrentCache<TKey, TValue>.Entry> where TKey : notnull
    {
        public class Entry
        {
            internal readonly int hash;

            internal readonly TKey key;

            internal readonly TValue value;

            internal Entry(int hash, TKey key, TValue value)
            {
                this.hash = hash;
                this.key = key;
                this.value = value;
            }
        }

        private readonly IEqualityComparer<TKey> _keyComparer;

        public ConcurrentCache(int size, IEqualityComparer<TKey> keyComparer)
            : base(size)
        {
            _keyComparer = keyComparer;
        }

        public ConcurrentCache(int size)
            : this(size, EqualityComparer<TKey>.Default)
        {
        }

        public bool TryAdd(TKey key, TValue value)
        {
            int hashCode = _keyComparer.GetHashCode(key);
            int num = hashCode & mask;
            Entry entry = entries[num];
            if (entry != null && entry.hash == hashCode && _keyComparer.Equals(entry.key, key))
            {
                return false;
            }
            entries[num] = new Entry(hashCode, key, value);
            return true;
        }

        public bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            int hashCode = _keyComparer.GetHashCode(key);
            int num = hashCode & mask;
            Entry entry = entries[num];
            if (entry != null && entry.hash == hashCode && _keyComparer.Equals(entry.key, key))
            {
                value = entry.value;
                return true;
            }
            value = default(TValue);
            return false;
        }
    }
}
