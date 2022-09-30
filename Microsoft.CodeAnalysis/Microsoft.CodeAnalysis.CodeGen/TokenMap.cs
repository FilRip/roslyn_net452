using System;
using System.Collections.Concurrent;
using System.Threading;

using Microsoft.Cci;

namespace Microsoft.CodeAnalysis.CodeGen
{
    internal sealed class TokenMap
    {
        private readonly ConcurrentDictionary<IReferenceOrISignature, uint> _itemIdentityToToken = new ConcurrentDictionary<IReferenceOrISignature, uint>();

        private object[] _items = new object[0];

        private int _count;

        internal TokenMap()
        {
        }

        public uint GetOrAddTokenFor(IReference item, out bool referenceAdded)
        {
            if (_itemIdentityToToken.TryGetValue(new IReferenceOrISignature(item), out var value))
            {
                referenceAdded = false;
                return value;
            }
            return AddItem(new IReferenceOrISignature(item), out referenceAdded);
        }

        public uint GetOrAddTokenFor(ISignature item, out bool referenceAdded)
        {
            if (_itemIdentityToToken.TryGetValue(new IReferenceOrISignature(item), out var value))
            {
                referenceAdded = false;
                return value;
            }
            return AddItem(new IReferenceOrISignature(item), out referenceAdded);
        }

        private uint AddItem(IReferenceOrISignature item, out bool referenceAdded)
        {
            lock (_itemIdentityToToken)
            {
                if (!_itemIdentityToToken.TryGetValue(item, out var value))
                {
                    value = (uint)_count;
                    referenceAdded = _itemIdentityToToken.TryAdd(item, value);
                    int num = (int)(value + 1);
                    object[] array = _items;
                    if (array.Length > num)
                    {
                        array[value] = item.AsObject();
                    }
                    else
                    {
                        Array.Resize(ref array, Math.Max(8, num * 2));
                        array[value] = item.AsObject();
                        Volatile.Write(ref _items, array);
                    }
                    Volatile.Write(ref _count, num);
                    return value;
                }
                referenceAdded = false;
                return value;
            }
        }

        public object GetItem(uint token)
        {
            return _items[token];
        }

        public ReadOnlySpan<object> GetAllItems()
        {
            int length = Volatile.Read(ref _count);
            return new ReadOnlySpan<object>(Volatile.Read(ref _items), 0, length);
        }
    }
}
