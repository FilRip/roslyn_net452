using System.Collections.Generic;
using System.Collections.Immutable;

namespace Roslyn.Utilities
{
    public static class ISetExtensions
    {
        public static bool AddAll<T>(this ISet<T> set, IEnumerable<T> values)
        {
            bool flag = false;
            foreach (T value in values)
            {
                flag |= set.Add(value);
            }
            return flag;
        }

        public static bool AddAll<T>(this ISet<T> set, ImmutableArray<T> values)
        {
            bool flag = false;
            ImmutableArray<T>.Enumerator enumerator = values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                flag |= set.Add(current);
            }
            return flag;
        }

        public static bool RemoveAll<T>(this ISet<T> set, IEnumerable<T> values)
        {
            bool flag = false;
            foreach (T value in values)
            {
                flag |= set.Remove(value);
            }
            return flag;
        }

        public static bool RemoveAll<T>(this ISet<T> set, ImmutableArray<T> values)
        {
            bool flag = false;
            ImmutableArray<T>.Enumerator enumerator = values.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                flag |= set.Remove(current);
            }
            return flag;
        }
    }
}
