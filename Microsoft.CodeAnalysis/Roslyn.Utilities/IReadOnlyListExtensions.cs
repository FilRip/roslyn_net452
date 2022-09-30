using System.Collections.Generic;

#nullable enable

namespace Roslyn.Utilities
{
    internal static class IReadOnlyListExtensions
    {
        public static bool Contains<T>(this IReadOnlyList<T> list, T item, IEqualityComparer<T>? comparer = null)
        {
            if (comparer == null)
            {
                comparer = EqualityComparer<T>.Default;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (comparer!.Equals(item, list[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
