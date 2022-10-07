using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.Collections
{
    internal static class ImmutableSegmentedDictionary
    {
        public static ImmutableSegmentedDictionary<TKey, TValue> Create<TKey, TValue>() where TKey : notnull
        {
            return ImmutableSegmentedDictionary<TKey, TValue>.Empty;
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> Create<TKey, TValue>(IEqualityComparer<TKey>? keyComparer) where TKey : notnull
        {
            return ImmutableSegmentedDictionary<TKey, TValue>.Empty.WithComparer(keyComparer);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue>.Builder CreateBuilder<TKey, TValue>() where TKey : notnull
        {
            return Create<TKey, TValue>().ToBuilder();
        }

        public static ImmutableSegmentedDictionary<TKey, TValue>.Builder CreateBuilder<TKey, TValue>(IEqualityComparer<TKey>? keyComparer) where TKey : notnull
        {
            return Create<TKey, TValue>(keyComparer).ToBuilder();
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
        {
            return ImmutableSegmentedDictionary<TKey, TValue>.Empty.AddRange(items);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> CreateRange<TKey, TValue>(IEqualityComparer<TKey>? keyComparer, IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
        {
            return ImmutableSegmentedDictionary<TKey, TValue>.Empty.WithComparer(keyComparer).AddRange(items);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> ToImmutableSegmentedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items) where TKey : notnull
        {
            return items.ToImmutableSegmentedDictionary(null);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> ToImmutableSegmentedDictionary<TKey, TValue>(this ImmutableSegmentedDictionary<TKey, TValue>.Builder builder) where TKey : notnull
        {
            if (builder == null)
            {
                throw new ArgumentNullException("builder");
            }
            return builder.ToImmutable();
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> ToImmutableSegmentedDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items, IEqualityComparer<TKey>? keyComparer) where TKey : notnull
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            if (items is ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary)
            {
                return immutableSegmentedDictionary.WithComparer(keyComparer);
            }
            return ImmutableSegmentedDictionary<TKey, TValue>.Empty.WithComparer(keyComparer).AddRange(items);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> ToImmutableSegmentedDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector) where TKey : notnull
        {
            return source.ToImmutableSegmentedDictionary(keySelector, elementSelector, null);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> ToImmutableSegmentedDictionary<TSource, TKey, TValue>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TValue> elementSelector, IEqualityComparer<TKey>? keyComparer) where TKey : notnull
        {
            Func<TSource, TKey> keySelector2 = keySelector;
            Func<TSource, TValue> elementSelector2 = elementSelector;
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector2 == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector2 == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            return ImmutableSegmentedDictionary<TKey, TValue>.Empty.WithComparer(keyComparer).AddRange(source.Select<TSource, KeyValuePair<TKey, TValue>>((TSource element) => new KeyValuePair<TKey, TValue>(keySelector2(element), elementSelector2(element))));
        }

        public static ImmutableSegmentedDictionary<TKey, TSource> ToImmutableSegmentedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector) where TKey : notnull
        {
            return source.ToImmutableSegmentedDictionary(keySelector, (TSource x) => x, null);
        }

        public static ImmutableSegmentedDictionary<TKey, TSource> ToImmutableSegmentedDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, IEqualityComparer<TKey>? keyComparer) where TKey : notnull
        {
            return source.ToImmutableSegmentedDictionary(keySelector, (TSource x) => x, keyComparer);
        }
    }
    public readonly struct ImmutableSegmentedDictionary<TKey, TValue> : IImmutableDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEquatable<ImmutableSegmentedDictionary<TKey, TValue>> where TKey : notnull
    {
        public sealed class Builder : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IDictionary, ICollection
        {
            public readonly struct KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, IReadOnlyCollection<TKey>, ICollection
            {
                private readonly ImmutableSegmentedDictionary<TKey, TValue>.Builder _dictionary;

                public int Count => _dictionary.Count;

                bool ICollection<TKey>.IsReadOnly => false;

                bool ICollection.IsSynchronized => false;

                object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

                internal KeyCollection(ImmutableSegmentedDictionary<TKey, TValue>.Builder dictionary)
                {
                    _dictionary = dictionary;
                }

                void ICollection<TKey>.Add(TKey item)
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    _dictionary.Clear();
                }

                public bool Contains(TKey item)
                {
                    return _dictionary.ContainsKey(item);
                }

                public void CopyTo(TKey[] array, int arrayIndex)
                {
                    ((ICollection<TKey>)_dictionary.ReadOnlyDictionary.Keys).CopyTo(array, arrayIndex);
                }

                public ImmutableSegmentedDictionary<TKey, TValue>.KeyCollection.Enumerator GetEnumerator()
                {
                    return new ImmutableSegmentedDictionary<TKey, TValue>.KeyCollection.Enumerator(_dictionary.GetEnumerator());
                }

                public bool Remove(TKey item)
                {
                    return _dictionary.Remove(item);
                }

                void ICollection.CopyTo(Array array, int index)
                {
                    ((ICollection)_dictionary.ReadOnlyDictionary.Keys).CopyTo(array, index);
                }

                IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            public readonly struct ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, IReadOnlyCollection<TValue>, ICollection
            {
                private readonly ImmutableSegmentedDictionary<TKey, TValue>.Builder _dictionary;

                public int Count => _dictionary.Count;

                bool ICollection<TValue>.IsReadOnly => false;

                bool ICollection.IsSynchronized => false;

                object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

                internal ValueCollection(ImmutableSegmentedDictionary<TKey, TValue>.Builder dictionary)
                {
                    _dictionary = dictionary;
                }

                void ICollection<TValue>.Add(TValue item)
                {
                    throw new NotSupportedException();
                }

                public void Clear()
                {
                    _dictionary.Clear();
                }

                public bool Contains(TValue item)
                {
                    return _dictionary.ContainsValue(item);
                }

                public void CopyTo(TValue[] array, int arrayIndex)
                {
                    ((ICollection<TValue>)_dictionary.ReadOnlyDictionary.Values).CopyTo(array, arrayIndex);
                }

                public ImmutableSegmentedDictionary<TKey, TValue>.ValueCollection.Enumerator GetEnumerator()
                {
                    return new ImmutableSegmentedDictionary<TKey, TValue>.ValueCollection.Enumerator(_dictionary.GetEnumerator());
                }

                bool ICollection<TValue>.Remove(TValue item)
                {
                    throw new NotSupportedException();
                }

                void ICollection.CopyTo(Array array, int index)
                {
                    ((ICollection)_dictionary.ReadOnlyDictionary.Values).CopyTo(array, index);
                }

                IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            private ImmutableSegmentedDictionary<TKey, TValue> _dictionary;

            private SegmentedDictionary<TKey, TValue>? _mutableDictionary;

            public IEqualityComparer<TKey> KeyComparer
            {
                get
                {
                    return ReadOnlyDictionary.Comparer;
                }
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException("value");
                    }
                    if (value != KeyComparer)
                    {
                        SegmentedDictionary<TKey, TValue> readOnlyDictionary = ReadOnlyDictionary;
                        _mutableDictionary = new SegmentedDictionary<TKey, TValue>(value);
                        AddRange(readOnlyDictionary);
                    }
                }
            }

            public int Count => ReadOnlyDictionary.Count;

            public KeyCollection Keys => new KeyCollection(this);

            public ValueCollection Values => new ValueCollection(this);

            private SegmentedDictionary<TKey, TValue> ReadOnlyDictionary => _mutableDictionary ?? _dictionary._dictionary;

            IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

            IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

            ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

            ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

            bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

            ICollection IDictionary.Keys => Keys;

            ICollection IDictionary.Values => Values;

            bool IDictionary.IsReadOnly => false;

            bool IDictionary.IsFixedSize => false;

            object ICollection.SyncRoot => this;

            bool ICollection.IsSynchronized => false;

            public TValue this[TKey key]
            {
                get
                {
                    return ReadOnlyDictionary[key];
                }
                set
                {
                    GetOrCreateMutableDictionary()[key] = value;
                }
            }

            object? IDictionary.this[object key]
            {
                get
                {
                    return ((IDictionary)ReadOnlyDictionary)[key];
                }
                set
                {
                    ((IDictionary)GetOrCreateMutableDictionary())[key] = value;
                }
            }

            internal Builder(ImmutableSegmentedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            private SegmentedDictionary<TKey, TValue> GetOrCreateMutableDictionary()
            {
                return _mutableDictionary ??= new SegmentedDictionary<TKey, TValue>(_dictionary._dictionary, _dictionary.KeyComparer);
            }

            public void Add(TKey key, TValue value)
            {
                if (!Contains(new KeyValuePair<TKey, TValue>(key, value)))
                {
                    GetOrCreateMutableDictionary().Add(key, value);
                }
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                Add(item.Key, item.Value);
            }

            public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> items)
            {
                if (items == null)
                {
                    throw new ArgumentNullException("items");
                }
                foreach (KeyValuePair<TKey, TValue> item in items)
                {
                    Add(item.Key, item.Value);
                }
            }

            public void Clear()
            {
                if (ReadOnlyDictionary.Count != 0)
                {
                    if (_mutableDictionary == null)
                    {
                        _mutableDictionary = new SegmentedDictionary<TKey, TValue>(KeyComparer);
                    }
                    else
                    {
                        _mutableDictionary!.Clear();
                    }
                }
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                if (TryGetValue(item.Key, out var value))
                {
                    return EqualityComparer<TValue>.Default.Equals(value, item.Value);
                }
                return false;
            }

            public bool ContainsKey(TKey key)
            {
                return ReadOnlyDictionary.ContainsKey(key);
            }

            public bool ContainsValue(TValue value)
            {
                return _dictionary.ContainsValue(value);
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(GetOrCreateMutableDictionary(), Enumerator.ReturnType.KeyValuePair);
            }

            public TValue? GetValueOrDefault(TKey key)
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
                return default;
            }

            public TValue GetValueOrDefault(TKey key, TValue defaultValue)
            {
                if (TryGetValue(key, out var value))
                {
                    return value;
                }
                return defaultValue;
            }

            public bool Remove(TKey key)
            {
                if (_mutableDictionary == null && !ContainsKey(key))
                {
                    return false;
                }
                return GetOrCreateMutableDictionary().Remove(key);
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                if (!Contains(item))
                {
                    return false;
                }
                GetOrCreateMutableDictionary().Remove(item.Key);
                return true;
            }

            public void RemoveRange(IEnumerable<TKey> keys)
            {
                if (keys == null)
                {
                    throw new ArgumentNullException("keys");
                }
                foreach (TKey key in keys)
                {
                    Remove(key);
                }
            }

            public bool TryGetKey(TKey equalKey, out TKey actualKey)
            {
                foreach (TKey key in Keys)
                {
                    if (KeyComparer.Equals(key, equalKey))
                    {
                        actualKey = key;
                        return true;
                    }
                }
                actualKey = equalKey;
                return false;
            }

            public bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
            {
                return ReadOnlyDictionary.TryGetValue(key, out value);
            }

            public ImmutableSegmentedDictionary<TKey, TValue> ToImmutable()
            {
                _dictionary = new ImmutableSegmentedDictionary<TKey, TValue>(ReadOnlyDictionary);
                _mutableDictionary = null;
                return _dictionary;
            }

            void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                ((ICollection<KeyValuePair<TKey, TValue>>)ReadOnlyDictionary).CopyTo(array, arrayIndex);
            }

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return new Enumerator(GetOrCreateMutableDictionary(), Enumerator.ReturnType.KeyValuePair);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(GetOrCreateMutableDictionary(), Enumerator.ReturnType.KeyValuePair);
            }

            bool IDictionary.Contains(object key)
            {
                return ((IDictionary)ReadOnlyDictionary).Contains(key);
            }

            void IDictionary.Add(object key, object? value)
            {
                ((IDictionary)GetOrCreateMutableDictionary()).Add(key, value);
            }

            IDictionaryEnumerator IDictionary.GetEnumerator()
            {
                return new Enumerator(GetOrCreateMutableDictionary(), Enumerator.ReturnType.DictionaryEntry);
            }

            void IDictionary.Remove(object key)
            {
                ((IDictionary)GetOrCreateMutableDictionary()).Remove(key);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)ReadOnlyDictionary).CopyTo(array, index);
            }
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable, IDictionaryEnumerator
        {
            internal enum ReturnType
            {
                KeyValuePair,
                DictionaryEntry
            }

            private readonly SegmentedDictionary<TKey, TValue> _dictionary;

            private readonly ReturnType _returnType;

            private SegmentedDictionary<TKey, TValue>.Enumerator _enumerator;

            public KeyValuePair<TKey, TValue> Current => _enumerator.Current;

            object IEnumerator.Current
            {
                get
                {
                    if (_returnType != ReturnType.DictionaryEntry)
                    {
                        return Current;
                    }
                    return ((IDictionaryEnumerator)this).Entry;
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry => new DictionaryEntry(Current.Key, Current.Value);

            object IDictionaryEnumerator.Key => Current.Key;

            object? IDictionaryEnumerator.Value => Current.Value;

            internal Enumerator(SegmentedDictionary<TKey, TValue> dictionary, ReturnType returnType)
            {
                _dictionary = dictionary;
                _returnType = returnType;
                _enumerator = dictionary.GetEnumerator();
            }

            public void Dispose()
            {
                _enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return _enumerator.MoveNext();
            }

            public void Reset()
            {
                _enumerator = _dictionary.GetEnumerator();
            }
        }

        public readonly struct KeyCollection : IReadOnlyCollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection<TKey>, ICollection
        {
            public struct Enumerator : IEnumerator<TKey>, IEnumerator, IDisposable
            {
                private ImmutableSegmentedDictionary<TKey, TValue>.Enumerator _enumerator;

                public TKey Current => _enumerator.Current.Key;

                object IEnumerator.Current => Current;

                internal Enumerator(ImmutableSegmentedDictionary<TKey, TValue>.Enumerator enumerator)
                {
                    _enumerator = enumerator;
                }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return _enumerator.MoveNext();
                }

                public void Reset()
                {
                    _enumerator.Reset();
                }
            }

            private readonly ImmutableSegmentedDictionary<TKey, TValue> _dictionary;

            public int Count => _dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => true;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            internal KeyCollection(ImmutableSegmentedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary.GetEnumerator());
            }

            public bool Contains(TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection<TKey>.CopyTo(TKey[] array, int arrayIndex)
            {
                ((ICollection<TKey>)_dictionary._dictionary.Keys).CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_dictionary._dictionary.Keys).CopyTo(array, index);
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException();
            }
        }

        internal static class PrivateInterlocked
        {
            internal static ImmutableSegmentedDictionary<TKey, TValue> VolatileRead(in ImmutableSegmentedDictionary<TKey, TValue> location)
            {
                SegmentedDictionary<TKey, TValue> segmentedDictionary = Volatile.Read(ref Unsafe.AsRef(in location._dictionary));
                if (segmentedDictionary == null)
                {
                    return default;
                }
                return new ImmutableSegmentedDictionary<TKey, TValue>(segmentedDictionary);
            }

            internal static ImmutableSegmentedDictionary<TKey, TValue> InterlockedExchange(ref ImmutableSegmentedDictionary<TKey, TValue> location, ImmutableSegmentedDictionary<TKey, TValue> value)
            {
                SegmentedDictionary<TKey, TValue> segmentedDictionary = Interlocked.Exchange(ref Unsafe.AsRef(in location._dictionary), value._dictionary);
                if (segmentedDictionary == null)
                {
                    return default;
                }
                return new ImmutableSegmentedDictionary<TKey, TValue>(segmentedDictionary);
            }

            internal static ImmutableSegmentedDictionary<TKey, TValue> InterlockedCompareExchange(ref ImmutableSegmentedDictionary<TKey, TValue> location, ImmutableSegmentedDictionary<TKey, TValue> value, ImmutableSegmentedDictionary<TKey, TValue> comparand)
            {
                SegmentedDictionary<TKey, TValue> segmentedDictionary = Interlocked.CompareExchange(ref Unsafe.AsRef(in location._dictionary), value._dictionary, comparand._dictionary);
                if (segmentedDictionary == null)
                {
                    return default;
                }
                return new ImmutableSegmentedDictionary<TKey, TValue>(segmentedDictionary);
            }
        }

        public readonly struct ValueCollection : IReadOnlyCollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection<TValue>, ICollection
        {
            public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
            {
                private ImmutableSegmentedDictionary<TKey, TValue>.Enumerator _enumerator;

                public TValue Current => _enumerator.Current.Value;

                object? IEnumerator.Current => Current;

                internal Enumerator(ImmutableSegmentedDictionary<TKey, TValue>.Enumerator enumerator)
                {
                    _enumerator = enumerator;
                }

                public void Dispose()
                {
                    _enumerator.Dispose();
                }

                public bool MoveNext()
                {
                    return _enumerator.MoveNext();
                }

                public void Reset()
                {
                    _enumerator.Reset();
                }
            }

            private readonly ImmutableSegmentedDictionary<TKey, TValue> _dictionary;

            public int Count => _dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection.IsSynchronized => true;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            internal ValueCollection(ImmutableSegmentedDictionary<TKey, TValue> dictionary)
            {
                _dictionary = dictionary;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary.GetEnumerator());
            }

            public bool Contains(TValue item)
            {
                return _dictionary.ContainsValue(item);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            void ICollection<TValue>.CopyTo(TValue[] array, int arrayIndex)
            {
                ((ICollection<TValue>)_dictionary._dictionary.Values).CopyTo(array, arrayIndex);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                ((ICollection)_dictionary._dictionary.Values).CopyTo(array, index);
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException();
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException();
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException();
            }
        }

        public static readonly ImmutableSegmentedDictionary<TKey, TValue> Empty = new ImmutableSegmentedDictionary<TKey, TValue>(new SegmentedDictionary<TKey, TValue>());

        private readonly SegmentedDictionary<TKey, TValue> _dictionary;

        public IEqualityComparer<TKey> KeyComparer => _dictionary.Comparer;

        public int Count => _dictionary.Count;

        public bool IsEmpty => _dictionary.Count == 0;

        public bool IsDefault => _dictionary == null;

        public bool IsDefaultOrEmpty
        {
            get
            {
                int? num = _dictionary?.Count;
                return !num.HasValue || num.GetValueOrDefault() == 0;
            }
        }

        public KeyCollection Keys => new KeyCollection(this);

        public ValueCollection Values => new ValueCollection(this);

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => true;

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        bool IDictionary.IsReadOnly => true;

        bool IDictionary.IsFixedSize => true;

        object ICollection.SyncRoot => _dictionary;

        bool ICollection.IsSynchronized => true;

        public TValue this[TKey key] => _dictionary[key];

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        object? IDictionary.this[object key]
        {
            get
            {
                return ((IDictionary)_dictionary)[key];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        private ImmutableSegmentedDictionary(SegmentedDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary ?? throw new ArgumentNullException("dictionary");
        }

        public static bool operator ==(ImmutableSegmentedDictionary<TKey, TValue> left, ImmutableSegmentedDictionary<TKey, TValue> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ImmutableSegmentedDictionary<TKey, TValue> left, ImmutableSegmentedDictionary<TKey, TValue> right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(ImmutableSegmentedDictionary<TKey, TValue>? left, ImmutableSegmentedDictionary<TKey, TValue>? right)
        {
            return left.GetValueOrDefault().Equals(right.GetValueOrDefault());
        }

        public static bool operator !=(ImmutableSegmentedDictionary<TKey, TValue>? left, ImmutableSegmentedDictionary<TKey, TValue>? right)
        {
            return !left.GetValueOrDefault().Equals(right.GetValueOrDefault());
        }

        public ImmutableSegmentedDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            ImmutableSegmentedDictionary<TKey, TValue> result = this;
            if (result.Contains(new KeyValuePair<TKey, TValue>(key, value)))
            {
                return result;
            }
            return new ImmutableSegmentedDictionary<TKey, TValue>(new SegmentedDictionary<TKey, TValue>(result._dictionary, result._dictionary.Comparer) { { key, value } });
        }

        public ImmutableSegmentedDictionary<TKey, TValue> AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            ImmutableSegmentedDictionary<TKey, TValue> result = this;
            if (result.IsEmpty && TryCastToImmutableSegmentedDictionary(pairs, out var other) && result.KeyComparer == other.KeyComparer)
            {
                return other;
            }
            SegmentedDictionary<TKey, TValue> segmentedDictionary = null;
            foreach (KeyValuePair<TKey, TValue> pair in pairs)
            {
                if (!((ICollection<KeyValuePair<TKey, TValue>>)(segmentedDictionary ?? result._dictionary)).Contains(pair))
                {
                    if (segmentedDictionary == null)
                    {
                        segmentedDictionary = new SegmentedDictionary<TKey, TValue>(result._dictionary, result._dictionary.Comparer);
                    }
                    segmentedDictionary.Add(pair.Key, pair.Value);
                }
            }
            if (segmentedDictionary == null)
            {
                return result;
            }
            return new ImmutableSegmentedDictionary<TKey, TValue>(segmentedDictionary);
        }

        public ImmutableSegmentedDictionary<TKey, TValue> Clear()
        {
            ImmutableSegmentedDictionary<TKey, TValue> result = this;
            if (result.IsEmpty)
            {
                return result;
            }
            return Empty.WithComparer(result.KeyComparer);
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            if (TryGetValue(pair.Key, out var value))
            {
                return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
            }
            return false;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool ContainsValue(TValue value)
        {
            return _dictionary.ContainsValue(value);
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_dictionary, Enumerator.ReturnType.KeyValuePair);
        }

        public ImmutableSegmentedDictionary<TKey, TValue> Remove(TKey key)
        {
            ImmutableSegmentedDictionary<TKey, TValue> result = this;
            if (!result._dictionary.ContainsKey(key))
            {
                return result;
            }
            SegmentedDictionary<TKey, TValue> segmentedDictionary = new SegmentedDictionary<TKey, TValue>(result._dictionary, result._dictionary.Comparer);
            segmentedDictionary.Remove(key);
            return new ImmutableSegmentedDictionary<TKey, TValue>(segmentedDictionary);
        }

        public ImmutableSegmentedDictionary<TKey, TValue> RemoveRange(IEnumerable<TKey> keys)
        {
            if (keys == null)
            {
                throw new ArgumentNullException("keys");
            }
            Builder builder = ToBuilder();
            builder.RemoveRange(keys);
            return builder.ToImmutable();
        }

        public ImmutableSegmentedDictionary<TKey, TValue> SetItem(TKey key, TValue value)
        {
            ImmutableSegmentedDictionary<TKey, TValue> result = this;
            if (result.Contains(new KeyValuePair<TKey, TValue>(key, value)))
            {
                return result;
            }
            return new ImmutableSegmentedDictionary<TKey, TValue>(new SegmentedDictionary<TKey, TValue>(result._dictionary, result._dictionary.Comparer) { [key] = value });
        }

        public ImmutableSegmentedDictionary<TKey, TValue> SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }
            Builder builder = ToBuilder();
            foreach (KeyValuePair<TKey, TValue> item in items)
            {
                builder[item.Key] = item.Value;
            }
            return builder.ToImmutable();
        }

        public bool TryGetKey(TKey equalKey, out TKey actualKey)
        {
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = this;
            foreach (TKey key in immutableSegmentedDictionary.Keys)
            {
                if (immutableSegmentedDictionary.KeyComparer.Equals(key, equalKey))
                {
                    actualKey = key;
                    return true;
                }
            }
            actualKey = equalKey;
            return false;
        }

        public bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ImmutableSegmentedDictionary<TKey, TValue> WithComparer(IEqualityComparer<TKey>? keyComparer)
        {
            if (keyComparer == null)
            {
                keyComparer = EqualityComparer<TKey>.Default;
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = this;
            if (immutableSegmentedDictionary.KeyComparer == keyComparer)
            {
                return immutableSegmentedDictionary;
            }
            if (immutableSegmentedDictionary.IsEmpty)
            {
                if (keyComparer == Empty.KeyComparer)
                {
                    return Empty;
                }
                return new ImmutableSegmentedDictionary<TKey, TValue>(new SegmentedDictionary<TKey, TValue>(keyComparer));
            }
            return ImmutableSegmentedDictionary.CreateRange(keyComparer, immutableSegmentedDictionary);
        }

        public Builder ToBuilder()
        {
            return new Builder(this);
        }

        public override int GetHashCode()
        {
            return _dictionary?.GetHashCode() ?? 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ImmutableSegmentedDictionary<TKey, TValue> other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(ImmutableSegmentedDictionary<TKey, TValue> other)
        {
            return _dictionary == other._dictionary;
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Clear()
        {
            return Clear();
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            return Add(key, value);
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            return AddRange(pairs);
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItem(TKey key, TValue value)
        {
            return SetItem(key, value);
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.SetItems(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return SetItems(items);
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.RemoveRange(IEnumerable<TKey> keys)
        {
            return RemoveRange(keys);
        }

        IImmutableDictionary<TKey, TValue> IImmutableDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(_dictionary, Enumerator.ReturnType.KeyValuePair);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(_dictionary, Enumerator.ReturnType.DictionaryEntry);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(_dictionary, Enumerator.ReturnType.KeyValuePair);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
        }

        bool IDictionary.Contains(object key)
        {
            return ((IDictionary)_dictionary).Contains(key);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_dictionary).CopyTo(array, index);
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw new NotSupportedException();
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new NotSupportedException();
        }

        void IDictionary.Add(object key, object? value)
        {
            throw new NotSupportedException();
        }

        void IDictionary.Clear()
        {
            throw new NotSupportedException();
        }

        void IDictionary.Remove(object key)
        {
            throw new NotSupportedException();
        }

        private static bool TryCastToImmutableSegmentedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> pairs, out ImmutableSegmentedDictionary<TKey, TValue> other)
        {
            if (pairs is ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary)
            {
                other = immutableSegmentedDictionary;
                return true;
            }
            if (pairs is Builder builder)
            {
                other = builder.ToImmutable();
                return true;
            }
            other = default;
            return false;
        }
    }
}
