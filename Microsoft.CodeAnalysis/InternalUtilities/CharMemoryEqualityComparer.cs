// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Roslyn.Utilities
{
    /// <summary>
    /// Provide structural equality for ReadOnlyMemory{char} instances.
    /// </summary>
    internal sealed class CharMemoryEqualityComparer : IEqualityComparer<ReadOnlyMemory<char>>
    {
        public static readonly CharMemoryEqualityComparer Instance = new();

        private CharMemoryEqualityComparer() { }

        public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y) => x.Span.SequenceEqual(y.Span);

        public int GetHashCode(ReadOnlyMemory<char> mem) => Hash.GetFNVHashCode(mem.Span);
    }
}
