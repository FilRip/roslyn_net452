// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;

#nullable enable

namespace Microsoft.CodeAnalysis.Collections
{
    public readonly partial struct ImmutableSegmentedDictionary<TKey, TValue>
    {
        public partial struct ValueCollection
        {
            public struct Enumerator : IEnumerator<TValue>
            {
                private ImmutableSegmentedDictionary<TKey, TValue>.Enumerator _enumerator;

                internal Enumerator(ImmutableSegmentedDictionary<TKey, TValue>.Enumerator enumerator)
                {
                    _enumerator = enumerator;
                }

                public TValue Current => _enumerator.Current.Value;

                object? IEnumerator.Current => Current;

                public void Dispose()
                    => _enumerator.Dispose();

                public bool MoveNext()
                    => _enumerator.MoveNext();

                public void Reset()
                    => _enumerator.Reset();
            }
        }
    }
}
