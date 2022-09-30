using System;
using System.Collections.Immutable;
using System.Linq;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public readonly struct LoadDirective : IEquatable<LoadDirective>
    {
        public readonly string? ResolvedPath;

        public readonly ImmutableArray<Diagnostic> Diagnostics;

        public LoadDirective(string? resolvedPath, ImmutableArray<Diagnostic> diagnostics)
        {
            ResolvedPath = resolvedPath;
            Diagnostics = diagnostics;
        }

        public bool Equals(LoadDirective other)
        {
            if (ResolvedPath == other.ResolvedPath)
            {
                return Diagnostics.SequenceEqual(other.Diagnostics);
            }
            return false;
        }

        public override bool Equals(object? obj)
        {
            if (obj is LoadDirective)
            {
                return Equals((LoadDirective)obj);
            }
            return false;
        }

        public override int GetHashCode()
        {
            ImmutableArray<Diagnostic> diagnostics = Diagnostics;
            return Hash.Combine(diagnostics.GetHashCode(), ResolvedPath?.GetHashCode() ?? 0);
        }
    }
}
