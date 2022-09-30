using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis.Collections
{
    public sealed class OrderPreservingMultiDictionary<K, V> : IEnumerable<KeyValuePair<K, OrderPreservingMultiDictionary<K, V>.ValueSet>>, IEnumerable where K : notnull where V : notnull
    {
        public struct ValueSet : IEnumerable<V>, IEnumerable
        {
            public struct Enumerator : IEnumerator<V>, IEnumerator, IDisposable
            {
                private readonly ValueSet _valueSet;

                private readonly int _count;

                private int _index;

                public V Current => _valueSet[_index];

                object IEnumerator.Current => Current;

                public Enumerator(ValueSet valueSet)
                {
                    _valueSet = valueSet;
                    _count = _valueSet.Count;
                    _index = -1;
                }

                public bool MoveNext()
                {
                    _index++;
                    return _index < _count;
                }

                public void Reset()
                {
                    _index = -1;
                }

                public void Dispose()
                {
                }
            }

            private readonly object _value;

            internal V this[int index]
            {
                get
                {
                    if (!(_value is ArrayBuilder<V> arrayBuilder))
                    {
                        if (index == 0)
                        {
                            return (V)_value;
                        }
                        throw new IndexOutOfRangeException();
                    }
                    return arrayBuilder[index];
                }
            }

            internal ImmutableArray<V> Items
            {
                get
                {
                    if (!(_value is ArrayBuilder<V> arrayBuilder))
                    {
                        return ImmutableArray.Create((V)_value);
                    }
                    return arrayBuilder.ToImmutable();
                }
            }

            internal int Count => (_value as ArrayBuilder<V>)?.Count ?? 1;

            internal ValueSet(V value)
            {
                _value = value;
            }

            internal ValueSet(ArrayBuilder<V> values)
            {
                _value = values;
            }

            internal void Free()
            {
                (_value as ArrayBuilder<V>)?.Free();
            }

            public bool TryGetValue<TArg>(Func<V, TArg, bool> predicate, TArg arg, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out V value)
            {
                if (_value is ArrayBuilder<V> arrayBuilder)
                {
                    ArrayBuilder<V>.Enumerator enumerator = arrayBuilder.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        V current = enumerator.Current;
                        if (predicate(current, arg))
                        {
                            value = current;
                            return true;
                        }
                    }
                }
                else
                {
                    V val = (V)_value;
                    if (predicate(val, arg))
                    {
                        value = val;
                        return true;
                    }
                }
                value = default(V);
                return false;
            }

            internal bool Contains(V item)
            {
                if (_value is ArrayBuilder<V> arrayBuilder)
                {
                    return arrayBuilder.Contains(item);
                }
                return EqualityComparer<V>.Default.Equals(item, (V)_value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator<V> IEnumerable<V>.GetEnumerator()
            {
                return GetEnumerator();
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(this);
            }

            internal ValueSet WithAddedItem(V item)
            {
                ArrayBuilder<V> arrayBuilder = _value as ArrayBuilder<V>;
                if (arrayBuilder == null)
                {
                    arrayBuilder = ArrayBuilder<V>.GetInstance(2);
                    arrayBuilder.Add((V)_value);
                    arrayBuilder.Add(item);
                }
                else
                {
                    arrayBuilder.Add(item);
                }
                return new ValueSet(arrayBuilder);
            }
        }

        private readonly ObjectPool<OrderPreservingMultiDictionary<K, V>>? _pool;

        private static readonly ObjectPool<OrderPreservingMultiDictionary<K, V>> s_poolInstance = CreatePool();

        private static readonly Dictionary<K, ValueSet> s_emptyDictionary = new Dictionary<K, ValueSet>();

        private PooledDictionary<K, ValueSet>? _dictionary;

        public bool IsEmpty => _dictionary == null;

        public ImmutableArray<V> this[K k]
        {
            get
            {
                if (_dictionary != null && _dictionary!.TryGetValue(k, out var value))
                {
                    return value.Items;
                }
                return ImmutableArray<V>.Empty;
            }
        }

        public Dictionary<K, ValueSet>.KeyCollection Keys
        {
            get
            {
                if (_dictionary != null)
                {
                    return _dictionary!.Keys;
                }
                return s_emptyDictionary.Keys;
            }
        }

        private OrderPreservingMultiDictionary(ObjectPool<OrderPreservingMultiDictionary<K, V>> pool)
        {
            _pool = pool;
        }

        public void Free()
        {
            if (_dictionary != null)
            {
                foreach (KeyValuePair<K, ValueSet> item in _dictionary!)
                {
                    item.Value.Free();
                }
                _dictionary!.Free();
                _dictionary = null;
            }
            _pool?.Free(this);
        }

        public static ObjectPool<OrderPreservingMultiDictionary<K, V>> CreatePool()
        {
            return new ObjectPool<OrderPreservingMultiDictionary<K, V>>((ObjectPool<OrderPreservingMultiDictionary<K, V>> pool) => new OrderPreservingMultiDictionary<K, V>(pool), 16);
        }

        public static OrderPreservingMultiDictionary<K, V> GetInstance()
        {
            return s_poolInstance.Allocate();
        }

        public OrderPreservingMultiDictionary()
        {
        }

        private void EnsureDictionary()
        {
            if (_dictionary == null)
            {
                _dictionary = PooledDictionary<K, ValueSet>.GetInstance();
            }
        }

        public void Add(K k, V v)
        {
            if (_dictionary != null && _dictionary!.TryGetValue(k, out var value))
            {
                _dictionary![k] = value.WithAddedItem(v);
                return;
            }
            EnsureDictionary();
            _dictionary![k] = new ValueSet(v);
        }

        public bool TryGetValue<TArg>(K key, Func<V, TArg, bool> predicate, TArg arg, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out V value)
        {
            if (_dictionary != null && _dictionary!.TryGetValue(key, out var value2))
            {
                return value2.TryGetValue(predicate, arg, out value);
            }
            value = default(V);
            return false;
        }

        public Dictionary<K, ValueSet>.Enumerator GetEnumerator()
        {
            if (_dictionary != null)
            {
                return _dictionary!.GetEnumerator();
            }
            return s_emptyDictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<K, ValueSet>> IEnumerable<KeyValuePair<K, ValueSet>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(K key, V value)
        {
            if (_dictionary != null && _dictionary!.TryGetValue(key, out var value2))
            {
                return value2.Contains(value);
            }
            return false;
        }
    }
}
