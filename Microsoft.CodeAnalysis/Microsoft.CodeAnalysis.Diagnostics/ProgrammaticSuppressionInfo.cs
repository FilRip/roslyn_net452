using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal sealed class ProgrammaticSuppressionInfo : IEquatable<ProgrammaticSuppressionInfo?>
    {
        public ImmutableHashSet<(string Id, LocalizableString Justification)> Suppressions { get; }

        internal ProgrammaticSuppressionInfo(ImmutableHashSet<(string Id, LocalizableString Justification)> suppressions)
        {
            Suppressions = suppressions;
        }

        public bool Equals(ProgrammaticSuppressionInfo? other)
        {
            if (this == other)
            {
                return true;
            }
            if (other != null)
            {
                return Suppressions.SetEquals(other!.Suppressions);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ProgrammaticSuppressionInfo);
        }

        public override int GetHashCode()
        {
            return Suppressions.Count;
        }
    }
}
