using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis.Collections.Internal;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.Collections
{
    [DebuggerTypeProxy(typeof(IDictionaryDebugView<,>))]
    [DebuggerDisplay("Count = {Count}")]
    internal sealed class SegmentedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable, IDictionary, ICollection, IReadOnlyDictionary<TKey, TValue>, IReadOnlyCollection<KeyValuePair<TKey, TValue>> where TKey : notnull
    {
        private struct Entry
        {
            public uint _hashCode;

            public int _next;

            public TKey _key;

            public TValue _value;
        }

        public struct Enumerator : IEnumerator<KeyValuePair<TKey, TValue>>, IEnumerator, IDisposable, IDictionaryEnumerator
        {
            private readonly SegmentedDictionary<TKey, TValue> _dictionary;

            private readonly int _version;

            private int _index;

            private KeyValuePair<TKey, TValue> _current;

            private readonly int _getEnumeratorRetType;

            internal const int DictEntry = 1;

            internal const int KeyValuePair = 2;

            public KeyValuePair<TKey, TValue> Current => _current;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    if (_getEnumeratorRetType == 1)
                    {
                        return new DictionaryEntry(_current.Key, _current.Value);
                    }
                    return new KeyValuePair<TKey, TValue>(_current.Key, _current.Value);
                }
            }

            DictionaryEntry IDictionaryEnumerator.Entry
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    return new DictionaryEntry(_current.Key, _current.Value);
                }
            }

            object IDictionaryEnumerator.Key
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    return _current.Key;
                }
            }

            object? IDictionaryEnumerator.Value
            {
                get
                {
                    if (_index == 0 || _index == _dictionary._count + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    return _current.Value;
                }
            }

            internal Enumerator(SegmentedDictionary<TKey, TValue> dictionary, int getEnumeratorRetType)
            {
                _dictionary = dictionary;
                _version = dictionary._version;
                _index = 0;
                _getEnumeratorRetType = getEnumeratorRetType;
                _current = default(KeyValuePair<TKey, TValue>);
            }

            public bool MoveNext()
            {
                if (_version != _dictionary._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }
                while ((uint)_index < (uint)_dictionary._count)
                {
                    ref Entry reference = ref _dictionary._entries[_index++];
                    if (reference._next >= -1)
                    {
                        _current = new KeyValuePair<TKey, TValue>(reference._key, reference._value);
                        return true;
                    }
                }
                _index = _dictionary._count + 1;
                _current = default(KeyValuePair<TKey, TValue>);
                return false;
            }

            public void Dispose()
            {
            }

            void IEnumerator.Reset()
            {
                if (_version != _dictionary._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }
                _index = 0;
                _current = default(KeyValuePair<TKey, TValue>);
            }
        }

        [DebuggerTypeProxy(typeof(DictionaryKeyCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, IEnumerable, ICollection, IReadOnlyCollection<TKey>
        {
            public struct Enumerator : IEnumerator<TKey>, IEnumerator, IDisposable
            {
                private readonly SegmentedDictionary<TKey, TValue> _dictionary;

                private int _index;

                private readonly int _version;

                private TKey? _currentKey;

                public TKey Current => _currentKey;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || _index == _dictionary._count + 1)
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                        }
                        return _currentKey;
                    }
                }

                internal Enumerator(SegmentedDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = 0;
                    _currentKey = default(TKey);
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }
                    while ((uint)_index < (uint)_dictionary._count)
                    {
                        ref Entry reference = ref _dictionary._entries[_index++];
                        if (reference._next >= -1)
                        {
                            _currentKey = reference._key;
                            return true;
                        }
                    }
                    _index = _dictionary._count + 1;
                    _currentKey = default(TKey);
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }
                    _index = 0;
                    _currentKey = default(TKey);
                }
            }

            private readonly SegmentedDictionary<TKey, TValue> _dictionary;

            public int Count => _dictionary.Count;

            bool ICollection<TKey>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            public KeyCollection(SegmentedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                _dictionary = dictionary;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            public void CopyTo(TKey[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (index < 0 || index > array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }
                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = _dictionary._count;
                SegmentedArray<Entry> entries = _dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i]._next >= -1)
                    {
                        array[index++] = entries[i]._key;
                    }
                }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            void ICollection<TKey>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                return _dictionary.ContainsKey(item);
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_KeyCollectionSet);
                return false;
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if ((uint)index > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }
                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                if (array is TKey[] array2)
                {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null)
                {
                    ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                }
                int count = _dictionary._count;
                SegmentedArray<Entry> entries = _dictionary._entries;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i]._next >= -1)
                        {
                            array3[index++] = entries[i]._key;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                }
            }
        }

        [DebuggerTypeProxy(typeof(DictionaryValueCollectionDebugView<,>))]
        [DebuggerDisplay("Count = {Count}")]
        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, IEnumerable, ICollection, IReadOnlyCollection<TValue>
        {
            public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
            {
                private readonly SegmentedDictionary<TKey, TValue> _dictionary;

                private int _index;

                private readonly int _version;

                private TValue? _currentValue;

                public TValue Current => _currentValue;

                object? IEnumerator.Current
                {
                    get
                    {
                        if (_index == 0 || _index == _dictionary._count + 1)
                        {
                            ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                        }
                        return _currentValue;
                    }
                }

                internal Enumerator(SegmentedDictionary<TKey, TValue> dictionary)
                {
                    _dictionary = dictionary;
                    _version = dictionary._version;
                    _index = 0;
                    _currentValue = default(TValue);
                }

                public void Dispose()
                {
                }

                public bool MoveNext()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }
                    while ((uint)_index < (uint)_dictionary._count)
                    {
                        ref Entry reference = ref _dictionary._entries[_index++];
                        if (reference._next >= -1)
                        {
                            _currentValue = reference._value;
                            return true;
                        }
                    }
                    _index = _dictionary._count + 1;
                    _currentValue = default(TValue);
                    return false;
                }

                void IEnumerator.Reset()
                {
                    if (_version != _dictionary._version)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                    }
                    _index = 0;
                    _currentValue = default(TValue);
                }
            }

            private readonly SegmentedDictionary<TKey, TValue> _dictionary;

            public int Count => _dictionary.Count;

            bool ICollection<TValue>.IsReadOnly => true;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => ((ICollection)_dictionary).SyncRoot;

            public ValueCollection(SegmentedDictionary<TKey, TValue> dictionary)
            {
                if (dictionary == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
                }
                _dictionary = dictionary;
            }

            public Enumerator GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            public void CopyTo(TValue[] array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if ((uint)index > array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }
                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                int count = _dictionary._count;
                SegmentedArray<Entry> entries = _dictionary._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i]._next >= -1)
                    {
                        array[index++] = entries[i]._value;
                    }
                }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
                return false;
            }

            void ICollection<TValue>.Clear()
            {
                ThrowHelper.ThrowNotSupportedException(ExceptionResource.NotSupported_ValueCollectionSet);
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                return _dictionary.ContainsValue(item);
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_dictionary);
            }

            void ICollection.CopyTo(Array array, int index)
            {
                if (array == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
                }
                if (array.Rank != 1)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
                }
                if (array.GetLowerBound(0) != 0)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
                }
                if ((uint)index > (uint)array.Length)
                {
                    ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
                }
                if (array.Length - index < _dictionary.Count)
                {
                    ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
                }
                if (array is TValue[] array2)
                {
                    CopyTo(array2, index);
                    return;
                }
                object[] array3 = array as object[];
                if (array3 == null)
                {
                    ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                }
                int count = _dictionary._count;
                SegmentedArray<Entry> entries = _dictionary._entries;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (entries[i]._next >= -1)
                        {
                            array3[index++] = entries[i]._value;
                        }
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
                }
            }
        }

        private const bool SupportsComparerDevirtualization = false;

        private SegmentedArray<int> _buckets;

        private SegmentedArray<Entry> _entries;

        private ulong _fastModMultiplier;

        private int _count;

        private int _freeList;

        private int _freeCount;

        private int _version;

        private readonly IEqualityComparer<TKey> _comparer;

        private KeyCollection? _keys;

        private ValueCollection? _values;

        private const int StartOfFreeList = -3;

        public IEqualityComparer<TKey> Comparer => _comparer ?? EqualityComparer<TKey>.Default;

        public int Count => _count - _freeCount;

        public KeyCollection Keys => _keys ?? (_keys = new KeyCollection(this));

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => Keys;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => Keys;

        public ValueCollection Values => _values ?? (_values = new ValueCollection(this));

        ICollection<TValue> IDictionary<TKey, TValue>.Values => Values;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => Values;

        public TValue this[TKey key]
        {
            get
            {
                ref TValue reference = ref FindValue(key);
                if (!RoslynUnsafe.IsNullRef(ref reference))
                {
                    return reference;
                }
                ThrowHelper.ThrowKeyNotFoundException(key);
                return default(TValue);
            }
            set
            {
                TryInsert(key, value, InsertionBehavior.OverwriteExisting);
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        bool IDictionary.IsFixedSize => false;

        bool IDictionary.IsReadOnly => false;

        ICollection IDictionary.Keys => Keys;

        ICollection IDictionary.Values => Values;

        object? IDictionary.this[object key]
        {
            get
            {
                if (IsCompatibleKey(key))
                {
                    ref TValue reference = ref FindValue((TKey)key);
                    if (!RoslynUnsafe.IsNullRef(ref reference))
                    {
                        return reference;
                    }
                }
                return null;
            }
            set
            {
                if (key == null)
                {
                    ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
                }
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
                try
                {
                    TKey key2 = (TKey)key;
                    try
                    {
                        this[key2] = (TValue)value;
                    }
                    catch (InvalidCastException)
                    {
                        ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                    }
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
                }
            }
        }

        public SegmentedDictionary()
            : this(0, null)
        {
        }

        public SegmentedDictionary(int capacity)
            : this(capacity, null)
        {
        }

        public SegmentedDictionary(IEqualityComparer<TKey>? comparer)
            : this(0, comparer)
        {
        }

        public SegmentedDictionary(int capacity, IEqualityComparer<TKey>? comparer)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }
            if (capacity > 0)
            {
                Initialize(capacity);
            }
            if (comparer != null && comparer != EqualityComparer<TKey>.Default)
            {
                _comparer = comparer;
            }
            if (_comparer == null)
            {
                _comparer = EqualityComparer<TKey>.Default;
            }
        }

        public SegmentedDictionary(IDictionary<TKey, TValue> dictionary)
            : this(dictionary, null)
        {
        }

        public SegmentedDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer)
            : this(dictionary?.Count ?? 0, comparer)
        {
            if (dictionary == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.dictionary);
            }
            if (dictionary.GetType() == typeof(SegmentedDictionary<TKey, TValue>))
            {
                SegmentedDictionary<TKey, TValue> obj = (SegmentedDictionary<TKey, TValue>)dictionary;
                int count = obj._count;
                SegmentedArray<Entry> entries = obj._entries;
                for (int i = 0; i < count; i++)
                {
                    if (entries[i]._next >= -1)
                    {
                        Add(entries[i]._key, entries[i]._value);
                    }
                }
                return;
            }
            foreach (KeyValuePair<TKey, TValue> item in dictionary)
            {
                Add(item.Key, item.Value);
            }
        }

        public SegmentedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection)
            : this(collection, null)
        {
        }

        public SegmentedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer)
            : this((collection as ICollection<KeyValuePair<TKey, TValue>>)?.Count ?? 0, comparer)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }
            foreach (KeyValuePair<TKey, TValue> item in collection)
            {
                Add(item.Key, item.Value);
            }
        }

        public void Add(TKey key, TValue value)
        {
            TryInsert(key, value, InsertionBehavior.ThrowOnExisting);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> keyValuePair)
        {
            Add(keyValuePair.Key, keyValuePair.Value);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> keyValuePair)
        {
            ref TValue reference = ref FindValue(keyValuePair.Key);
            if (!RoslynUnsafe.IsNullRef(ref reference) && EqualityComparer<TValue>.Default.Equals(reference, keyValuePair.Value))
            {
                return true;
            }
            return false;
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> keyValuePair)
        {
            ref TValue reference = ref FindValue(keyValuePair.Key);
            if (!RoslynUnsafe.IsNullRef(ref reference) && EqualityComparer<TValue>.Default.Equals(reference, keyValuePair.Value))
            {
                Remove(keyValuePair.Key);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            int count = _count;
            if (count > 0)
            {
                SegmentedArray.Clear(_buckets, 0, _buckets.Length);
                _count = 0;
                _freeList = -1;
                _freeCount = 0;
                SegmentedArray.Clear(_entries, 0, count);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return !RoslynUnsafe.IsNullRef(ref FindValue(key));
        }

        public bool ContainsValue(TValue value)
        {
            SegmentedArray<Entry> entries = _entries;
            if (value == null)
            {
                for (int i = 0; i < _count; i++)
                {
                    if (entries[i]._next >= -1 && entries[i]._value == null)
                    {
                        return true;
                    }
                }
            }
            else
            {
                EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
                for (int j = 0; j < _count; j++)
                {
                    if (entries[j]._next >= -1 && @default.Equals(entries[j]._value, value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if ((uint)index > (uint)array.Length)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            int count = _count;
            SegmentedArray<Entry> entries = _entries;
            for (int i = 0; i < count; i++)
            {
                if (entries[i]._next >= -1)
                {
                    array[index++] = new KeyValuePair<TKey, TValue>(entries[i]._key, entries[i]._value);
                }
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        private ref TValue FindValue(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            ref Entry reference = ref RoslynUnsafe.NullRef<Entry>();
            if (_buckets.Length > 0)
            {
                IEqualityComparer<TKey> comparer = _comparer;
                uint hashCode = (uint)comparer.GetHashCode(key);
                int bucket = GetBucket(hashCode);
                SegmentedArray<Entry> entries = _entries;
                uint num = 0u;
                bucket--;
                while ((uint)bucket < (uint)entries.Length)
                {
                    reference = ref entries[bucket];
                    if (reference._hashCode != hashCode || !comparer.Equals(reference._key, key))
                    {
                        bucket = reference._next;
                        num++;
                        if (num <= (uint)entries.Length)
                        {
                            continue;
                        }
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                    return ref reference._value;
                }
            }
            return ref RoslynUnsafe.NullRef<TValue>();
        }

        private int Initialize(int capacity)
        {
            int prime = HashHelpers.GetPrime(capacity);
            SegmentedArray<int> buckets = new SegmentedArray<int>(prime);
            SegmentedArray<Entry> entries = new SegmentedArray<Entry>(prime);
            _freeList = -1;
            _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)prime);
            _buckets = buckets;
            _entries = entries;
            return prime;
        }

        private bool TryInsert(TKey key, TValue value, InsertionBehavior behavior)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (_buckets.Length == 0)
            {
                Initialize(0);
            }
            SegmentedArray<Entry> entries = _entries;
            IEqualityComparer<TKey> comparer = _comparer;
            uint hashCode = (uint)comparer.GetHashCode(key);
            uint num = 0u;
            ref int bucket = ref GetBucket(hashCode);
            int num2 = bucket - 1;
            while ((uint)num2 < (uint)entries.Length)
            {
                if (entries[num2]._hashCode == hashCode && comparer.Equals(entries[num2]._key, key))
                {
                    switch (behavior)
                    {
                        case InsertionBehavior.OverwriteExisting:
                            entries[num2]._value = value;
                            return true;
                        case InsertionBehavior.ThrowOnExisting:
                            ThrowHelper.ThrowAddingDuplicateWithKeyArgumentException(key);
                            break;
                    }
                    return false;
                }
                num2 = entries[num2]._next;
                num++;
                if (num > (uint)entries.Length)
                {
                    ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                }
            }
            int num3;
            if (_freeCount > 0)
            {
                num3 = _freeList;
                _freeList = -3 - entries[_freeList]._next;
                _freeCount--;
            }
            else
            {
                int count = _count;
                if (count == entries.Length)
                {
                    Resize();
                    bucket = ref GetBucket(hashCode);
                }
                num3 = count;
                _count = count + 1;
                entries = _entries;
            }
            ref Entry reference = ref entries[num3];
            reference._hashCode = hashCode;
            reference._next = bucket - 1;
            reference._key = key;
            reference._value = value;
            bucket = num3 + 1;
            _version++;
            return true;
        }

        private void Resize()
        {
            Resize(HashHelpers.ExpandPrime(_count));
        }

        private void Resize(int newSize)
        {
            SegmentedArray<Entry> segmentedArray = new SegmentedArray<Entry>(newSize);
            int count = _count;
            SegmentedArray.Copy(_entries, segmentedArray, count);
            _buckets = new SegmentedArray<int>(newSize);
            _fastModMultiplier = HashHelpers.GetFastModMultiplier((uint)newSize);
            for (int i = 0; i < count; i++)
            {
                if (segmentedArray[i]._next >= -1)
                {
                    ref int bucket = ref GetBucket(segmentedArray[i]._hashCode);
                    segmentedArray[i]._next = bucket - 1;
                    bucket = i + 1;
                }
            }
            _entries = segmentedArray;
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (_buckets.Length > 0)
            {
                uint num = 0u;
                uint num2 = (uint)(_comparer?.GetHashCode(key) ?? key.GetHashCode());
                ref int bucket = ref GetBucket(num2);
                SegmentedArray<Entry> entries = _entries;
                int num3 = -1;
                int num4 = bucket - 1;
                while (num4 >= 0)
                {
                    ref Entry reference = ref entries[num4];
                    if (reference._hashCode == num2 && (_comparer?.Equals(reference._key, key) ?? EqualityComparer<TKey>.Default.Equals(reference._key, key)))
                    {
                        if (num3 < 0)
                        {
                            bucket = reference._next + 1;
                        }
                        else
                        {
                            entries[num3]._next = reference._next;
                        }
                        reference._next = -3 - _freeList;
                        reference._key = default(TKey);
                        reference._value = default(TValue);
                        _freeList = num4;
                        _freeCount++;
                        return true;
                    }
                    num3 = num4;
                    num4 = reference._next;
                    num++;
                    if (num > (uint)entries.Length)
                    {
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
            return false;
        }

        public bool Remove(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            if (_buckets.Length > 0)
            {
                uint num = 0u;
                uint num2 = (uint)(_comparer?.GetHashCode(key) ?? key.GetHashCode());
                ref int bucket = ref GetBucket(num2);
                SegmentedArray<Entry> entries = _entries;
                int num3 = -1;
                int num4 = bucket - 1;
                while (num4 >= 0)
                {
                    ref Entry reference = ref entries[num4];
                    if (reference._hashCode == num2 && (_comparer?.Equals(reference._key, key) ?? EqualityComparer<TKey>.Default.Equals(reference._key, key)))
                    {
                        if (num3 < 0)
                        {
                            bucket = reference._next + 1;
                        }
                        else
                        {
                            entries[num3]._next = reference._next;
                        }
                        value = reference._value;
                        reference._next = -3 - _freeList;
                        reference._key = default(TKey);
                        reference._value = default(TValue);
                        _freeList = num4;
                        _freeCount++;
                        return true;
                    }
                    num3 = num4;
                    num4 = reference._next;
                    num++;
                    if (num > (uint)entries.Length)
                    {
                        ThrowHelper.ThrowInvalidOperationException_ConcurrentOperationsNotSupported();
                    }
                }
            }
            value = default(TValue);
            return false;
        }

        public bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            ref TValue reference = ref FindValue(key);
            if (!RoslynUnsafe.IsNullRef(ref reference))
            {
                value = reference;
                return true;
            }
            value = default(TValue);
            return false;
        }

        public bool TryAdd(TKey key, TValue value)
        {
            return TryInsert(key, value, InsertionBehavior.None);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
        {
            CopyTo(array, index);
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
            }
            if (array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            if (array.GetLowerBound(0) != 0)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
            }
            if ((uint)index > (uint)array.Length)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (array.Length - index < Count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_ArrayPlusOffTooSmall);
            }
            if (array is KeyValuePair<TKey, TValue>[] array2)
            {
                CopyTo(array2, index);
                return;
            }
            if (array is DictionaryEntry[] array3)
            {
                SegmentedArray<Entry> entries = _entries;
                for (int i = 0; i < _count; i++)
                {
                    if (entries[i]._next >= -1)
                    {
                        array3[index++] = new DictionaryEntry(entries[i]._key, entries[i]._value);
                    }
                }
                return;
            }
            object[] array4 = array as object[];
            if (array4 == null)
            {
                ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
            }
            try
            {
                int count = _count;
                SegmentedArray<Entry> entries2 = _entries;
                for (int j = 0; j < count; j++)
                {
                    if (entries2[j]._next >= -1)
                    {
                        array4[index++] = new KeyValuePair<TKey, TValue>(entries2[j]._key, entries2[j]._value);
                    }
                }
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 2);
        }

        public int EnsureCapacity(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }
            int length = _entries.Length;
            if (length >= capacity)
            {
                return length;
            }
            _version++;
            if (_buckets.Length == 0)
            {
                return Initialize(capacity);
            }
            int prime = HashHelpers.GetPrime(capacity);
            Resize(prime);
            return prime;
        }

        public void TrimExcess()
        {
            TrimExcess(Count);
        }

        public void TrimExcess(int capacity)
        {
            if (capacity < Count)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity);
            }
            int prime = HashHelpers.GetPrime(capacity);
            SegmentedArray<Entry> entries = _entries;
            int length = entries.Length;
            if (prime >= length)
            {
                return;
            }
            int count = _count;
            _version++;
            Initialize(prime);
            SegmentedArray<Entry> entries2 = _entries;
            int num = 0;
            for (int i = 0; i < count; i++)
            {
                uint hashCode = entries[i]._hashCode;
                if (entries[i]._next >= -1)
                {
                    ref Entry reference = ref entries2[num];
                    reference = entries[i];
                    ref int bucket = ref GetBucket(hashCode);
                    reference._next = bucket - 1;
                    bucket = num + 1;
                    num++;
                }
            }
            _count = num;
            _freeCount = 0;
        }

        private static bool IsCompatibleKey(object key)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            return key is TKey;
        }

        void IDictionary.Add(object key, object? value)
        {
            if (key == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
            }
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<TValue>(value, ExceptionArgument.value);
            try
            {
                TKey key2 = (TKey)key;
                try
                {
                    Add(key2, (TValue)value);
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(TValue));
                }
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongKeyTypeArgumentException(key, typeof(TKey));
            }
        }

        bool IDictionary.Contains(object key)
        {
            if (IsCompatibleKey(key))
            {
                return ContainsKey((TKey)key);
            }
            return false;
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return new Enumerator(this, 1);
        }

        void IDictionary.Remove(object key)
        {
            if (IsCompatibleKey(key))
            {
                Remove((TKey)key);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ref int GetBucket(uint hashCode)
        {
            SegmentedArray<int> buckets = _buckets;
            return ref buckets[(int)HashHelpers.FastMod(hashCode, (uint)buckets.Length, _fastModMultiplier)];
        }
    }
}
