using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.Collections.Internal
{
    internal sealed class IDictionaryDebugView<K, V> where K : notnull
    {
        private readonly IDictionary<K, V> _dict;

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public KeyValuePair<K, V>[] Items
        {
            get
            {
                KeyValuePair<K, V>[] array = new KeyValuePair<K, V>[_dict.Count];
                _dict.CopyTo(array, 0);
                return array;
            }
        }

        public IDictionaryDebugView(IDictionary<K, V> dictionary)
        {
            _dict = dictionary ?? throw new ArgumentNullException("dictionary");
        }
    }
}
