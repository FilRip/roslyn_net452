using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Roslyn.Utilities
{
    public sealed class MultiDictionary<K, V> : IEnumerable<KeyValuePair<K, MultiDictionary<K, V>.ValueSet>>, IEnumerable where K : notnull
    {
        public struct ValueSet : IEnumerable<V>, IEnumerable
        {
            public struct Enumerator : IEnumerator<V>, IEnumerator, IDisposable
            {
                [System.Diagnostics.CodeAnalysis.AllowNull]
                private readonly V _value;

                private ImmutableHashSet<V>.Enumerator _values;

                private int _count;

                object? IEnumerator.Current => Current;

                public V Current
                {
                    get
                    {
                        if (_count <= 1)
                        {
                            return _value;
                        }
                        return _values.Current;
                    }
                }

                public Enumerator(ValueSet v)
                {
                    if (v._value == null)
                    {
                        _value = default(V);
                        _values = default(ImmutableHashSet<V>.Enumerator);
                        _count = 0;
                    }
                    else if (!(v._value is ImmutableHashSet<V> immutableHashSet))
                    {
                        _value = (V)v._value;
                        _values = default(ImmutableHashSet<V>.Enumerator);
                        _count = 1;
                    }
                    else
                    {
                        _value = default(V);
                        _values = immutableHashSet.GetEnumerator();
                        _count = immutableHashSet.Count;
                    }
                }

                public void Dispose()
                {
                }

                public void Reset()
                {
                    throw new NotSupportedException();
                }

                public bool MoveNext()
                {
                    switch (_count)
                    {
                        case 0:
                            return false;
                        case 1:
                            _count = 0;
                            return true;
                        default:
                            if (_values.MoveNext())
                            {
                                return true;
                            }
                            _count = 0;
                            return false;
                    }
                }
            }

            private readonly object? _value;

            private readonly IEqualityComparer<V> _equalityComparer;

            public int Count
            {
                get
                {
                    if (_value == null)
                    {
                        return 0;
                    }
                    if (!(_value is ImmutableHashSet<V> immutableHashSet))
                    {
                        return 1;
                    }
                    return immutableHashSet.Count;
                }
            }

            public ValueSet(object? value, IEqualityComparer<V>? equalityComparer = null)
            {
                _value = value;
                _equalityComparer = equalityComparer ?? ImmutableHashSet<V>.Empty.KeyComparer;
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

            public ValueSet Add(V v)
            {
                ImmutableHashSet<V> immutableHashSet = _value as ImmutableHashSet<V>;
                if (immutableHashSet == null)
                {
                    if (_equalityComparer.Equals((V)_value, v))
                    {
                        return this;
                    }
                    immutableHashSet = ImmutableHashSet.Create(_equalityComparer, (V)_value);
                }
                return new ValueSet(immutableHashSet.Add(v), _equalityComparer);
            }

            public bool Contains(V v)
            {
                if (!(_value is ImmutableHashSet<V> immutableHashSet))
                {
                    return _equalityComparer.Equals((V)_value, v);
                }
                return immutableHashSet.Contains(v);
            }

            public bool Contains(V v, IEqualityComparer<V> comparer)
            {
                using (Enumerator enumerator = GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        V current = enumerator.Current;
                        if (comparer.Equals(current, v))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            public V Single()
            {
                return (V)_value;
            }

            public bool Equals(ValueSet other)
            {
                return _value == other._value;
            }
        }

        private readonly Dictionary<K, ValueSet> _dictionary;

        private readonly IEqualityComparer<V>? _valueComparer;

        private readonly ValueSet _emptySet = new ValueSet(null);

        public int Count => _dictionary.Count;

        public bool IsEmpty => _dictionary.Count == 0;

        public Dictionary<K, ValueSet>.KeyCollection Keys => _dictionary.Keys;

        public Dictionary<K, ValueSet>.ValueCollection Values => _dictionary.Values;

        public ValueSet this[K k]
        {
            get
            {
                if (!_dictionary.TryGetValue(k, out var value))
                {
                    return _emptySet;
                }
                return value;
            }
        }

        public MultiDictionary()
        {
            _dictionary = new Dictionary<K, ValueSet>();
        }

        public MultiDictionary(IEqualityComparer<K> comparer)
        {
            _dictionary = new Dictionary<K, ValueSet>(comparer);
        }

        public MultiDictionary(int capacity, IEqualityComparer<K> comparer, IEqualityComparer<V>? valueComparer = null)
        {
            _dictionary = new Dictionary<K, ValueSet>(capacity, comparer);
            _valueComparer = valueComparer;
        }

        public bool Add(K k, V v)
        {
            ValueSet value2;
            if (_dictionary.TryGetValue(k, out var value))
            {
                value2 = value.Add(v);
                if (value2.Equals(value))
                {
                    return false;
                }
            }
            else
            {
                value2 = new ValueSet(v, _valueComparer);
            }
            _dictionary[k] = value2;
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Dictionary<K, ValueSet>.Enumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator<KeyValuePair<K, ValueSet>> IEnumerable<KeyValuePair<K, ValueSet>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool ContainsKey(K k)
        {
            return _dictionary.ContainsKey(k);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public void Remove(K key)
        {
            _dictionary.Remove(key);
        }
    }
}
