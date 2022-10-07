using System;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public struct LocalDebugId : IEquatable<LocalDebugId>
    {
        public readonly int SyntaxOffset;

        public readonly int Ordinal;

        public static readonly LocalDebugId None = new LocalDebugId(isNone: true);

        public bool IsNone => Ordinal == -1;

        private LocalDebugId(bool isNone)
        {
            SyntaxOffset = -1;
            Ordinal = -1;
        }

        public LocalDebugId(int syntaxOffset, int ordinal = 0)
        {
            SyntaxOffset = syntaxOffset;
            Ordinal = ordinal;
        }

        public bool Equals(LocalDebugId other)
        {
            if (SyntaxOffset == other.SyntaxOffset)
            {
                return Ordinal == other.Ordinal;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(SyntaxOffset, Ordinal);
        }

        public override bool Equals(object? obj)
        {
            if (obj is LocalDebugId id)
            {
                return Equals(id);
            }
            return false;
        }

        public override string ToString()
        {
            return SyntaxOffset + ":" + Ordinal;
        }
    }
}
