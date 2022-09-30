using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class CollectionsExtensions
    {
        public static bool IsNullOrEmpty<T>([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] this ICollection<T>? collection)
        {
            if (collection != null)
            {
                return collection!.Count == 0;
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] this IReadOnlyCollection<T>? collection)
        {
            if (collection != null)
            {
                return collection!.Count == 0;
            }
            return true;
        }

        public static bool IsNullOrEmpty<T>([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] this ImmutableHashSet<T>? hashSet)
        {
            if (hashSet != null)
            {
                return hashSet!.Count == 0;
            }
            return true;
        }
    }
}
