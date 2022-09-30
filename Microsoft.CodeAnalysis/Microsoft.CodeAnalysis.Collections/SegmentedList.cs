using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

#nullable enable

namespace Microsoft.CodeAnalysis.Collections
{
    [DebuggerTypeProxy(typeof(ICollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    internal class SegmentedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IList, ICollection, IReadOnlyList<T>, IReadOnlyCollection<T>
    {
        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private readonly SegmentedList<T> _list;

            private int _index;

            private readonly int _version;

            private T? _current;

            public T Current => _current;

            object? IEnumerator.Current
            {
                get
                {
                    if (_index == 0 || _index == _list._size + 1)
                    {
                        ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumOpCantHappen();
                    }
                    return Current;
                }
            }

            internal Enumerator(SegmentedList<T> list)
            {
                _list = list;
                _index = 0;
                _version = list._version;
                _current = default(T);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                SegmentedList<T> list = _list;
                if (_version == list._version && (uint)_index < (uint)list._size)
                {
                    _current = list._items[_index];
                    _index++;
                    return true;
                }
                return MoveNextRare();
            }

            private bool MoveNextRare()
            {
                if (_version != _list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }
                _index = _list._size + 1;
                _current = default(T);
                return false;
            }

            void IEnumerator.Reset()
            {
                if (_version != _list._version)
                {
                    ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
                }
                _index = 0;
                _current = default(T);
            }
        }

        private const int DefaultCapacity = 4;

        private const int MaxArrayLength = 2146435071;

        internal SegmentedArray<T> _items;

        internal int _size;

        private int _version;

        private static readonly SegmentedArray<T> s_emptyArray = new SegmentedArray<T>(0);

        public int Capacity
        {
            get
            {
                return _items.Length;
            }
            set
            {
                if (value < _size)
                {
                    ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.value, ExceptionResource.ArgumentOutOfRange_SmallCapacity);
                }
                if (value == _items.Length)
                {
                    return;
                }
                if (value > 0)
                {
                    SegmentedArray<T> segmentedArray = new SegmentedArray<T>(value);
                    if (_size > 0)
                    {
                        SegmentedArray.Copy(_items, segmentedArray, _size);
                    }
                    _items = segmentedArray;
                }
                else
                {
                    _items = s_emptyArray;
                }
            }
        }

        public int Count => _size;

        bool IList.IsFixedSize => false;

        bool ICollection<T>.IsReadOnly => false;

        bool IList.IsReadOnly => false;

        bool ICollection.IsSynchronized => false;

        object ICollection.SyncRoot => this;

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)_size)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexException();
                }
                return _items[index];
            }
            set
            {
                if ((uint)index >= (uint)_size)
                {
                    ThrowHelper.ThrowArgumentOutOfRange_IndexException();
                }
                _items[index] = value;
                _version++;
            }
        }

        object? IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(value, ExceptionArgument.value);
                try
                {
                    this[index] = (T)value;
                }
                catch (InvalidCastException)
                {
                    ThrowHelper.ThrowWrongValueTypeArgumentException(value, typeof(T));
                }
            }
        }

        public SegmentedList()
        {
            _items = s_emptyArray;
        }

        public SegmentedList(int capacity)
        {
            if (capacity < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (capacity == 0)
            {
                _items = s_emptyArray;
            }
            else
            {
                _items = new SegmentedArray<T>(capacity);
            }
        }

        public SegmentedList(IEnumerable<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }
            if (collection is ICollection<T> collection2)
            {
                int count = collection2.Count;
                if (count == 0)
                {
                    _items = s_emptyArray;
                    return;
                }
                _items = new SegmentedArray<T>(count);
                T[][] array = (T[][])_items.SyncRoot;
                if (array != null && array.Length == 1)
                {
                    collection2.CopyTo(array[0], 0);
                    _size = count;
                    return;
                }
            }
            else
            {
                _items = s_emptyArray;
            }
            foreach (T item in collection)
            {
                Add(item);
            }
        }

        private static bool IsCompatibleObject(object? value)
        {
            if (!(value is T))
            {
                if (value == null)
                {
                    return default(T) == null;
                }
                return false;
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            _version++;
            SegmentedArray<T> items = _items;
            int size = _size;
            if ((uint)size < (uint)items.Length)
            {
                _size = size + 1;
                items[size] = item;
            }
            else
            {
                AddWithResize(item);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AddWithResize(T item)
        {
            int size = _size;
            EnsureCapacity(size + 1);
            _size = size + 1;
            _items[size] = item;
        }

        int IList.Add(object? item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
            try
            {
                Add((T)item);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }
            return Count - 1;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            InsertRange(_size, collection);
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            return new ReadOnlyCollection<T>(this);
        }

        public int BinarySearch(int index, int count, T item, IComparer<T>? comparer)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            return SegmentedArray.BinarySearch(_items, index, count, item, comparer);
        }

        public int BinarySearch(T item)
        {
            return BinarySearch(0, Count, item, null);
        }

        public int BinarySearch(T item, IComparer<T>? comparer)
        {
            return BinarySearch(0, Count, item, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _version++;
            int size = _size;
            _size = 0;
            if (size > 0)
            {
                SegmentedArray.Clear(_items, 0, size);
            }
        }

        public bool Contains(T item)
        {
            if (_size != 0)
            {
                return IndexOf(item) != -1;
            }
            return false;
        }

        bool IList.Contains(object? item)
        {
            if (IsCompatibleObject(item))
            {
                return Contains((T)item);
            }
            return false;
        }

        public SegmentedList<TOutput> ConvertAll<TOutput>(Converter<T, TOutput> converter)
        {
            if (converter == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.converter);
            }
            SegmentedList<TOutput> segmentedList = new SegmentedList<TOutput>(_size);
            for (int i = 0; i < _size; i++)
            {
                segmentedList._items[i] = converter(_items[i]);
            }
            segmentedList._size = _size;
            return segmentedList;
        }

        public void CopyTo(T[] array)
        {
            CopyTo(array, 0);
        }

        void ICollection.CopyTo(Array array, int arrayIndex)
        {
            if (array != null && array.Rank != 1)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
            }
            try
            {
                SegmentedArray.Copy(_items, 0, array, arrayIndex, _size);
            }
            catch (ArrayTypeMismatchException)
            {
                ThrowHelper.ThrowArgumentException_Argument_InvalidArrayType();
            }
        }

        public void CopyTo(int index, T[] array, int arrayIndex, int count)
        {
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            SegmentedArray.Copy(_items, index, array, arrayIndex, count);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            SegmentedArray.Copy(_items, 0, array, arrayIndex, _size);
        }

        private void EnsureCapacity(int min)
        {
            if (_items.Length < min)
            {
                int num = ((_items.Length == 0) ? 4 : (_items.Length * 2));
                if ((uint)num > 2146435071u)
                {
                    num = 2146435071;
                }
                if (num < min)
                {
                    num = min;
                }
                Capacity = num;
            }
        }

        public bool Exists(Predicate<T> match)
        {
            return FindIndex(match) != -1;
        }

        public T? Find(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    return _items[i];
                }
            }
            return default(T);
        }

        public SegmentedList<T> FindAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            SegmentedList<T> segmentedList = new SegmentedList<T>();
            for (int i = 0; i < _size; i++)
            {
                if (match(_items[i]))
                {
                    segmentedList.Add(_items[i]);
                }
            }
            return segmentedList;
        }

        public int FindIndex(Predicate<T> match)
        {
            return FindIndex(0, _size, match);
        }

        public int FindIndex(int startIndex, Predicate<T> match)
        {
            return FindIndex(startIndex, _size - startIndex, match);
        }

        public int FindIndex(int startIndex, int count, Predicate<T> match)
        {
            if ((uint)startIndex > (uint)_size)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
            }
            if (count < 0 || startIndex > _size - count)
            {
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
            }
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (match(_items[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public T? FindLast(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            for (int num = _size - 1; num >= 0; num--)
            {
                if (match(_items[num]))
                {
                    return _items[num];
                }
            }
            return default(T);
        }

        public int FindLastIndex(Predicate<T> match)
        {
            return FindLastIndex(_size - 1, _size, match);
        }

        public int FindLastIndex(int startIndex, Predicate<T> match)
        {
            return FindLastIndex(startIndex, startIndex + 1, match);
        }

        public int FindLastIndex(int startIndex, int count, Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            if (_size == 0)
            {
                if (startIndex != -1)
                {
                    ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
                }
            }
            else if ((uint)startIndex >= (uint)_size)
            {
                ThrowHelper.ThrowStartIndexArgumentOutOfRange_ArgumentOutOfRange_Index();
            }
            if (count < 0 || startIndex - count + 1 < 0)
            {
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
            }
            int num = startIndex - count;
            for (int num2 = startIndex; num2 > num; num2--)
            {
                if (match(_items[num2]))
                {
                    return num2;
                }
            }
            return -1;
        }

        public void ForEach(Action<T> action)
        {
            if (action == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.action);
            }
            int version = _version;
            for (int i = 0; i < _size; i++)
            {
                if (version != _version)
                {
                    break;
                }
                action(_items[i]);
            }
            if (version != _version)
            {
                ThrowHelper.ThrowInvalidOperationException_InvalidOperation_EnumFailedVersion();
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public SegmentedList<T> GetRange(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            SegmentedList<T> segmentedList = new SegmentedList<T>(count);
            SegmentedArray.Copy(_items, index, segmentedList._items, 0, count);
            segmentedList._size = count;
            return segmentedList;
        }

        public int IndexOf(T item)
        {
            return SegmentedArray.IndexOf(_items, item, 0, _size);
        }

        int IList.IndexOf(object? item)
        {
            if (IsCompatibleObject(item))
            {
                return IndexOf((T)item);
            }
            return -1;
        }

        public int IndexOf(T item, int index)
        {
            if (index > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            return SegmentedArray.IndexOf(_items, item, index, _size - index);
        }

        public int IndexOf(T item, int index, int count)
        {
            if (index > _size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            if (count < 0 || index > _size - count)
            {
                ThrowHelper.ThrowCountArgumentOutOfRange_ArgumentOutOfRange_Count();
            }
            return SegmentedArray.IndexOf(_items, item, index, count);
        }

        public void Insert(int index, T item)
        {
            if ((uint)index > (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_ListInsert);
            }
            if (_size == _items.Length)
            {
                EnsureCapacity(_size + 1);
            }
            if (index < _size)
            {
                SegmentedArray.Copy(_items, index, _items, index + 1, _size - index);
            }
            _items[index] = item;
            _size++;
            _version++;
        }

        void IList.Insert(int index, object? item)
        {
            ThrowHelper.IfNullAndNullsAreIllegalThenThrow<T>(item, ExceptionArgument.item);
            try
            {
                Insert(index, (T)item);
            }
            catch (InvalidCastException)
            {
                ThrowHelper.ThrowWrongValueTypeArgumentException(item, typeof(T));
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
            }
            if ((uint)index > (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            if (collection is ICollection<T> collection2)
            {
                int count = collection2.Count;
                if (count > 0)
                {
                    EnsureCapacity(_size + count);
                    if (index < _size)
                    {
                        SegmentedArray.Copy(_items, index, _items, index + count, _size - index);
                    }
                    if (this == collection2)
                    {
                        SegmentedArray.Copy(_items, 0, _items, index, index);
                        SegmentedArray.Copy(_items, index + count, _items, index * 2, _size - index);
                    }
                    else if (collection2 is SegmentedList<T> segmentedList)
                    {
                        SegmentedArray.Copy(segmentedList._items, 0, _items, index, segmentedList.Count);
                    }
                    else if (collection2 is SegmentedArray<T> sourceArray)
                    {
                        SegmentedArray.Copy(sourceArray, 0, _items, index, sourceArray.Length);
                    }
                    else
                    {
                        int num = index;
                        foreach (T item in collection2)
                        {
                            _items[num++] = item;
                        }
                    }
                    _size += count;
                }
            }
            else
            {
                using IEnumerator<T> enumerator2 = collection.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Insert(index++, enumerator2.Current);
                }
            }
            _version++;
        }

        public int LastIndexOf(T item)
        {
            if (_size == 0)
            {
                return -1;
            }
            return LastIndexOf(item, _size - 1, _size);
        }

        public int LastIndexOf(T item, int index)
        {
            if (index >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            return LastIndexOf(item, index, index + 1);
        }

        public int LastIndexOf(T item, int index, int count)
        {
            if (Count != 0 && index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (Count != 0 && count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size == 0)
            {
                return -1;
            }
            if (index >= _size)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }
            if (count > index + 1)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_BiggerThanCollection);
            }
            return SegmentedArray.LastIndexOf(_items, item, index, count);
        }

        public bool Remove(T item)
        {
            int num = IndexOf(item);
            if (num >= 0)
            {
                RemoveAt(num);
                return true;
            }
            return false;
        }

        void IList.Remove(object? item)
        {
            if (IsCompatibleObject(item))
            {
                Remove((T)item);
            }
        }

        public int RemoveAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            int i;
            for (i = 0; i < _size && !match(_items[i]); i++)
            {
            }
            if (i >= _size)
            {
                return 0;
            }
            int j = i + 1;
            while (j < _size)
            {
                for (; j < _size && match(_items[j]); j++)
                {
                }
                if (j < _size)
                {
                    _items[i++] = _items[j++];
                }
            }
            SegmentedArray.Clear(_items, i, _size - i);
            int result = _size - i;
            _size = i;
            _version++;
            return result;
        }

        public void RemoveAt(int index)
        {
            if ((uint)index >= (uint)_size)
            {
                ThrowHelper.ThrowArgumentOutOfRange_IndexException();
            }
            _size--;
            if (index < _size)
            {
                SegmentedArray.Copy(_items, index + 1, _items, index, _size - index);
            }
            _items[_size] = default(T);
            _version++;
        }

        public void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            if (count > 0)
            {
                _size -= count;
                if (index < _size)
                {
                    SegmentedArray.Copy(_items, index + count, _items, index, _size - index);
                }
                _version++;
                SegmentedArray.Clear(_items, _size, count);
            }
        }

        public void Reverse()
        {
            Reverse(0, Count);
        }

        public void Reverse(int index, int count)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            if (count > 1)
            {
                SegmentedArray.Reverse(_items, index, count);
            }
            _version++;
        }

        public void Sort()
        {
            Sort(0, Count, null);
        }

        public void Sort(IComparer<T>? comparer)
        {
            Sort(0, Count, comparer);
        }

        public void Sort(int index, int count, IComparer<T>? comparer)
        {
            if (index < 0)
            {
                ThrowHelper.ThrowIndexArgumentOutOfRange_NeedNonNegNumException();
            }
            if (count < 0)
            {
                ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.count, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (_size - index < count)
            {
                ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
            }
            if (count > 1)
            {
                SegmentedArray.Sort(_items, index, count, comparer);
            }
            _version++;
        }

        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.comparison);
            }
            if (_size > 1)
            {
                SegmentedArray.Sort(_items, 0, _size, Comparer<T>.Create(comparison));
            }
            _version++;
        }

        public T[] ToArray()
        {
            if (_size == 0)
            {
                return new T[0];
            }
            T[] array = new T[_size];
            SegmentedArray.Copy(_items, array, _size);
            return array;
        }

        public void TrimExcess()
        {
            int num = (int)(_items.Length * 0.9);
            if (_size < num)
            {
                Capacity = _size;
            }
        }

        public bool TrueForAll(Predicate<T> match)
        {
            if (match == null)
            {
                ThrowHelper.ThrowArgumentNullException(ExceptionArgument.match);
            }
            for (int i = 0; i < _size; i++)
            {
                if (!match(_items[i]))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
