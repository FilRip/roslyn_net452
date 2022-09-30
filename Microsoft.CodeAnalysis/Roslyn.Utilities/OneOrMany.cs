using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Roslyn.Utilities
{
    public struct OneOrMany<T> where T : notnull
    {
        public struct Enumerator
        {
            private readonly OneOrMany<T> _collection;

            private int _index;

            public T Current => _collection[_index];

            internal Enumerator(OneOrMany<T> collection)
            {
                _collection = collection;
                _index = -1;
            }

            public bool MoveNext()
            {
                _index++;
                return _index < _collection.Count;
            }
        }

        private readonly T? _one;

        private readonly ImmutableArray<T> _many;

        public T this[int index]
        {
            get
            {
                if (_many.IsDefault)
                {
                    if (index != 0)
                    {
                        throw new IndexOutOfRangeException();
                    }
                    return _one;
                }
                return _many[index];
            }
        }

        public int Count
        {
            get
            {
                if (!_many.IsDefault)
                {
                    return _many.Length;
                }
                return 1;
            }
        }

        public OneOrMany(T one)
        {
            _one = one;
            _many = default(ImmutableArray<T>);
        }

        public OneOrMany(ImmutableArray<T> many)
        {
            if (many.IsDefault)
            {
                throw new ArgumentNullException("many");
            }
            _one = default(T);
            _many = many;
        }

        public OneOrMany<T> Add(T one)
        {
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance();
            if (_many.IsDefault)
            {
                instance.Add(_one);
            }
            else
            {
                instance.AddRange(_many);
            }
            instance.Add(one);
            return new OneOrMany<T>(instance.ToImmutableAndFree());
        }

        public bool Contains(T item)
        {
            if (Count == 1)
            {
                return item.Equals(_one);
            }
            Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (item.Equals(enumerator.Current))
                {
                    return true;
                }
            }
            return false;
        }

        public OneOrMany<T> RemoveAll(T item)
        {
            if (_many.IsDefault)
            {
                if (!item.Equals(_one))
                {
                    return this;
                }
                return default(OneOrMany<T>);
            }
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance();
            Enumerator enumerator = GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!item.Equals(enumerator.Current))
                {
                    instance.Add(enumerator.Current);
                }
            }
            if (instance.Count == 0)
            {
                return default(OneOrMany<T>);
            }
            if (instance.Count != Count)
            {
                return new OneOrMany<T>(instance.ToImmutableAndFree());
            }
            return this;
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
    public static class OneOrMany
    {
        public static OneOrMany<T> Create<T>(T one) where T : notnull
        {
            return new OneOrMany<T>(one);
        }

        public static OneOrMany<T> Create<T>(ImmutableArray<T> many) where T : notnull
        {
            return new OneOrMany<T>(many);
        }
    }
}
