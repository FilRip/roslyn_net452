using System.Collections.Generic;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    internal static class HashSetExtensions
    {
        internal static bool IsNullOrEmpty<T>([System.Diagnostics.CodeAnalysis.NotNullWhen(false)] this HashSet<T>? hashSet)
        {
            if (hashSet != null)
            {
                return hashSet!.Count == 0;
            }
            return true;
        }

        internal static bool InitializeAndAdd<T>([System.Diagnostics.CodeAnalysis.NotNullIfNotNull("item")][System.Diagnostics.CodeAnalysis.NotNullWhen(true)] ref HashSet<T>? hashSet, [System.Diagnostics.CodeAnalysis.NotNullWhen(true)] T? item) where T : class
        {
            if (item == null)
            {
                return false;
            }
            if (hashSet == null)
            {
                hashSet = new HashSet<T>();
            }
            return hashSet!.Add(item);
        }
    }
}
