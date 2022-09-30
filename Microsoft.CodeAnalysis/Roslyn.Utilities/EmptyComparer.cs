using System.Collections.Generic;

#nullable enable

#nullable enable

namespace Roslyn.Utilities
{
    public sealed class EmptyComparer : IEqualityComparer<object>
    {
        public static readonly EmptyComparer Instance = new();

        private EmptyComparer()
        {
        }

        bool IEqualityComparer<object>.Equals(object? a, object? b)
        {
            return false;
        }

        int IEqualityComparer<object>.GetHashCode(object s)
        {
            return 0;
        }
    }
}
