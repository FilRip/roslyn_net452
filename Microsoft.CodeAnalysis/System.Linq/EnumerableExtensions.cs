using System.Collections.Generic;

#nullable enable

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static bool SequenceEqual<T>(this IEnumerable<T>? first, IEnumerable<T>? second, Func<T, T, bool> comparer)
        {
            if (first == second)
            {
                return true;
            }
            if (first == null || second == null)
            {
                return false;
            }
            using (IEnumerator<T> enumerator = first!.GetEnumerator())
            {
                using IEnumerator<T> enumerator2 = second!.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (!enumerator2.MoveNext() || !comparer(enumerator.Current, enumerator2.Current))
                    {
                        return false;
                    }
                }
                if (enumerator2.MoveNext())
                {
                    return false;
                }
            }
            return true;
        }

        public static T? AggregateOrDefault<T>(this IEnumerable<T> source, Func<T, T, T> func)
        {
            using IEnumerator<T> enumerator = source.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(T);
            }
            T val = enumerator.Current;
            while (enumerator.MoveNext())
            {
                val = func(val, enumerator.Current);
            }
            return val;
        }
    }
}
