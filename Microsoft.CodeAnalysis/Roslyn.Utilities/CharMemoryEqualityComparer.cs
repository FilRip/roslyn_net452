using System;
using System.Collections.Generic;

namespace Roslyn.Utilities
{
    internal sealed class CharMemoryEqualityComparer : IEqualityComparer<ReadOnlyMemory<char>>
    {
        public static readonly CharMemoryEqualityComparer Instance = new CharMemoryEqualityComparer();

        private CharMemoryEqualityComparer()
        {
        }

        public bool Equals(ReadOnlyMemory<char> x, ReadOnlyMemory<char> y)
        {
            return x.Span.SequenceEqual(y.Span);
        }

        public int GetHashCode(ReadOnlyMemory<char> mem)
        {
            return Hash.GetFNVHashCode(mem.Span);
        }
    }
}
