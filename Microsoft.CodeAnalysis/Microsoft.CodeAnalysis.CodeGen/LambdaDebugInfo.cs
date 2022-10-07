using System;
using System.Diagnostics;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CodeGen
{
    public struct LambdaDebugInfo : IEquatable<LambdaDebugInfo>
    {
        public readonly int SyntaxOffset;

        public readonly int ClosureOrdinal;

        public readonly DebugId LambdaId;

        public const int StaticClosureOrdinal = -1;

        public const int ThisOnlyClosureOrdinal = -2;

        public const int MinClosureOrdinal = -2;

        public LambdaDebugInfo(int syntaxOffset, DebugId lambdaId, int closureOrdinal)
        {
            SyntaxOffset = syntaxOffset;
            ClosureOrdinal = closureOrdinal;
            LambdaId = lambdaId;
        }

        public bool Equals(LambdaDebugInfo other)
        {
            if (SyntaxOffset == other.SyntaxOffset && ClosureOrdinal == other.ClosureOrdinal)
            {
                return LambdaId.Equals(other.LambdaId);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LambdaDebugInfo info)
            {
                return Equals(info);
            }
            return false;
        }

        public override int GetHashCode()
        {
            int closureOrdinal = ClosureOrdinal;
            int syntaxOffset = SyntaxOffset;
            DebugId lambdaId = LambdaId;
            return Hash.Combine(closureOrdinal, Hash.Combine(syntaxOffset, lambdaId.GetHashCode()));
        }

        internal string GetDebuggerDisplay()
        {
            if (ClosureOrdinal != -1)
            {
                if (ClosureOrdinal != -2)
                {
                    return $"({LambdaId.GetDebuggerDisplay()} @{SyntaxOffset} in {ClosureOrdinal})";
                }
                return $"(#{LambdaId.GetDebuggerDisplay()} @{SyntaxOffset}, this)";
            }
            return $"({LambdaId.GetDebuggerDisplay()} @{SyntaxOffset}, static)";
        }
    }
}
