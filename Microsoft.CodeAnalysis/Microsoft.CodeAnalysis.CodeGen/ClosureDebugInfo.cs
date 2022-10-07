using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public struct ClosureDebugInfo : IEquatable<ClosureDebugInfo>
    {
        public readonly int SyntaxOffset;

        public readonly DebugId ClosureId;

        public ClosureDebugInfo(int syntaxOffset, DebugId closureId)
        {
            SyntaxOffset = syntaxOffset;
            ClosureId = closureId;
        }

        public bool Equals(ClosureDebugInfo other)
        {
            if (SyntaxOffset == other.SyntaxOffset)
            {
                return ClosureId.Equals(other.ClosureId);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ClosureDebugInfo info)
            {
                return Equals(info);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int syntaxOffset = SyntaxOffset;
            DebugId closureId = ClosureId;
            return Hash.Combine(syntaxOffset, closureId.GetHashCode());
        }

        internal string GetDebuggerDisplay()
        {
            return $"({ClosureId.GetDebuggerDisplay()} @{SyntaxOffset})";
        }
    }
}
