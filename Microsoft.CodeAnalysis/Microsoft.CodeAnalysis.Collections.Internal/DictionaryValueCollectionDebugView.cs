using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal sealed class DictionaryValueCollectionDebugView<TKey, TValue>
    {
        private readonly ICollection<TValue> _collection;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public TValue[] Items
        {
            get
            {
                TValue[] array = new TValue[_collection.Count];
                _collection.CopyTo(array, 0);
                return array;
            }
        }

        public DictionaryValueCollectionDebugView(ICollection<TValue> collection)
        {
            _collection = collection ?? throw new ArgumentNullException("collection");
        }
    }
}
