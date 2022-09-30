using System;
using System.Diagnostics;
using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.PooledObjects
{
    public class ObjectPool<T> where T : class
    {
        [DebuggerDisplay("{Value,nq}")]
        private struct Element
        {
            internal T? Value;
        }

        public delegate T Factory();

        private T? _firstItem;

        private readonly Element[] _items;

        private readonly Factory _factory;

        public ObjectPool(Factory factory)
            : this(factory, Environment.ProcessorCount * 2)
        {
        }

        public ObjectPool(Factory factory, int size)
        {
            _factory = factory;
            _items = new Element[size - 1];
        }

        public ObjectPool(Func<ObjectPool<T>, T> factory, int size)
        {
            Func<ObjectPool<T>, T> factory2 = factory;
            //base._002Ector();
            ObjectPool<T> arg = this;
            _factory = () => factory2(arg);
            _items = new Element[size - 1];
        }

        private T CreateInstance()
        {
            return _factory();
        }

        public T Allocate()
        {
            T val = _firstItem;
            if (val == null || val != Interlocked.CompareExchange(ref _firstItem, null, val))
            {
                val = AllocateSlow();
            }
            return val;
        }

        private T AllocateSlow()
        {
            Element[] items = _items;
            for (int i = 0; i < items.Length; i++)
            {
                T value = items[i].Value;
                if (value != null && value == Interlocked.CompareExchange(ref items[i].Value, null, value))
                {
                    return value;
                }
            }
            return CreateInstance();
        }

        public void Free(T obj)
        {
            if (_firstItem == null)
            {
                _firstItem = obj;
            }
            else
            {
                FreeSlow(obj);
            }
        }

        private void FreeSlow(T obj)
        {
            Element[] items = _items;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Value == null)
                {
                    items[i].Value = obj;
                    break;
                }
            }
        }

        [Conditional("DEBUG")]
        internal void ForgetTrackedObject(T old, T? replacement = null)
        {
        }

        [Conditional("DEBUG")]
        private void Validate(object obj)
        {
            Element[] items = _items;
            for (int i = 0; i < items.Length && items[i].Value != null; i++)
            {
            }
        }
    }
}
