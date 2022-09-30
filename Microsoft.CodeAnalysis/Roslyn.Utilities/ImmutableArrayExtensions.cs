using System;
using System.Collections.Generic;
using System.Collections.Immutable;

#nullable enable

namespace Roslyn.Utilities
{
    public static class ImmutableArrayExtensions
    {
        public static ImmutableArray<T> ToImmutableArrayOrEmpty<T>(this IEnumerable<T>? items)
        {
            if (items != null)
            {
                return ImmutableArray.CreateRange(items);
            }
            return ImmutableArray<T>.Empty;
        }

        public static ImmutableArray<T> ToImmutableArrayOrEmpty<T>(this ImmutableArray<T> items)
        {
            if (!items.IsDefault)
            {
                return items;
            }
            return ImmutableArray<T>.Empty;
        }

        public static int BinarySearch<TElement, TValue>(this ImmutableArray<TElement> array, TValue value, Func<TElement, TValue, int> comparer)
        {
            int num = 0;
            int num2 = array.Length - 1;
            while (num <= num2)
            {
                int num3 = num + (num2 - num >> 1);
                int num4 = comparer(array[num3], value);
                if (num4 == 0)
                {
                    return num3;
                }
                if (num4 > 0)
                {
                    num2 = num3 - 1;
                }
                else
                {
                    num = num3 + 1;
                }
            }
            return ~num;
        }

        public static ImmutableArray<TDerived> CastDown<TOriginal, TDerived>(this ImmutableArray<TOriginal> array) where TDerived : class, TOriginal
        {
            return array.CastArray<TDerived>();
        }
    }
}
