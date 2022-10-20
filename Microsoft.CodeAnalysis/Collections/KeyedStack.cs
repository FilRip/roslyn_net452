// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.CodeAnalysis.Collections
{
    public class KeyedStack<T, R>
        where T : notnull
    {
        private readonly Dictionary<T, Stack<R>> _dict = new Dictionary<T, Stack<R>>();

        public void Push(T key, R value)
        {
            if (!_dict.TryGetValue(key, out Stack<R> store))
            {
                store = new Stack<R>();
                _dict.Add(key, store);
            }

            store.Push(value);
        }

        public bool TryPop(T key, [MaybeNullWhen(returnValue: false)] out R value)
        {
            if (_dict.TryGetValue(key, out Stack<R> store) && store.Count > 0)
            {
                value = store.Pop();
                return true;
            }

            value = default!;
            return false;
        }
    }
}
