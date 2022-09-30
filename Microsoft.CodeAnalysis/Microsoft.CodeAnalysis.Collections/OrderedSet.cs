using System.Collections;
using System.Collections.Generic;

using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Collections
{
    public sealed class OrderedSet<T> : IEnumerable<T>, IEnumerable, IReadOnlySet<T>, IReadOnlyList<T>, IReadOnlyCollection<T>, IOrderedReadOnlySet<T>
    {
        private readonly HashSet<T> _set;

        private readonly ArrayBuilder<T> _list;

        public int Count => _list.Count;

        public T this[int index] => _list[index];

        public OrderedSet()
        {
            _set = new HashSet<T>();
            _list = new ArrayBuilder<T>();
        }

        public OrderedSet(IEnumerable<T> items)
            : this()
        {
            AddRange(items);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                Add(item);
            }
        }

        public bool Add(T item)
        {
            if (_set.Add(item))
            {
                _list.Add(item);
                return true;
            }
            return false;
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public ArrayBuilder<T>.Enumerator GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        public void Clear()
        {
            _set.Clear();
            _list.Clear();
        }
    }
}
