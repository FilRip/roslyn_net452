using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public struct DebugId : IEquatable<DebugId>
    {
        public const int UndefinedOrdinal = -1;

        public readonly int Ordinal;

        public readonly int Generation;

        public DebugId(int ordinal, int generation)
        {
            Ordinal = ordinal;
            Generation = generation;
        }

        public bool Equals(DebugId other)
        {
            if (Ordinal == other.Ordinal)
            {
                return Generation == other.Generation;
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is DebugId)
            {
                return Equals((DebugId)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(Ordinal, Generation);
        }

        internal string GetDebuggerDisplay()
        {
            if (Generation <= 0)
            {
                return Ordinal.ToString();
            }
            return $"{Ordinal}#{Generation}";
        }
    }
}
