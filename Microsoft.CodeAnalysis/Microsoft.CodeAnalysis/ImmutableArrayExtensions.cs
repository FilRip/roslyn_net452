using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class ImmutableArrayExtensions
    {
        private struct ImmutableArrayProxy<T>
        {
            internal T[] MutableArray;
        }

        public static ImmutableArray<T> AsImmutable<T>(this IEnumerable<T> items)
        {
            return ImmutableArray.CreateRange(items);
        }

        public static ImmutableArray<T> AsImmutableOrEmpty<T>(this IEnumerable<T>? items)
        {
            if (items == null)
            {
                return ImmutableArray<T>.Empty;
            }
            return ImmutableArray.CreateRange(items);
        }

        public static ImmutableArray<T> AsImmutableOrNull<T>(this IEnumerable<T>? items)
        {
            if (items == null)
            {
                return default(ImmutableArray<T>);
            }
            return ImmutableArray.CreateRange(items);
        }

        public static ImmutableArray<T> AsImmutable<T>(this T[] items)
        {
            return ImmutableArray.Create(items);
        }

        public static ImmutableArray<T> AsImmutableOrNull<T>(this T[]? items)
        {
            if (items == null)
            {
                return default(ImmutableArray<T>);
            }
            return ImmutableArray.Create(items);
        }

        public static ImmutableArray<T> AsImmutableOrEmpty<T>(this T[]? items)
        {
            if (items == null)
            {
                return ImmutableArray<T>.Empty;
            }
            return ImmutableArray.Create(items);
        }

        public static ImmutableArray<byte> ToImmutable(this MemoryStream stream)
        {
            return ImmutableArray.Create(stream.ToArray());
        }

        public static ImmutableArray<TResult> SelectAsArray<TItem, TResult>(this ImmutableArray<TItem> items, Func<TItem, TResult> map)
        {
            return ImmutableArray.CreateRange(items, map);
        }

        public static ImmutableArray<TResult> SelectAsArray<TItem, TArg, TResult>(this ImmutableArray<TItem> items, Func<TItem, TArg, TResult> map, TArg arg)
        {
            return ImmutableArray.CreateRange(items, map, arg);
        }

        public static ImmutableArray<TResult> SelectAsArray<TItem, TArg, TResult>(this ImmutableArray<TItem> items, Func<TItem, int, TArg, TResult> map, TArg arg)
        {
            switch (items.Length)
            {
                case 0:
                    return ImmutableArray<TResult>.Empty;
                case 1:
                    return ImmutableArray.Create(map(items[0], 0, arg));
                case 2:
                    return ImmutableArray.Create(map(items[0], 0, arg), map(items[1], 1, arg));
                case 3:
                    return ImmutableArray.Create(map(items[0], 0, arg), map(items[1], 1, arg), map(items[2], 2, arg));
                case 4:
                    return ImmutableArray.Create(map(items[0], 0, arg), map(items[1], 1, arg), map(items[2], 2, arg), map(items[3], 3, arg));
                default:
                    {
                        ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance(items.Length);
                        for (int i = 0; i < items.Length; i++)
                        {
                            instance.Add(map(items[i], i, arg));
                        }
                        return instance.ToImmutableAndFree();
                    }
            }
        }

        public static ImmutableArray<TResult> SelectAsArray<TItem, TResult>(this ImmutableArray<TItem> array, Func<TItem, bool> predicate, Func<TItem, TResult> selector)
        {
            if (array.Length == 0)
            {
                return ImmutableArray<TResult>.Empty;
            }
            ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance();
            ImmutableArray<TItem>.Enumerator enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TItem current = enumerator.Current;
                if (predicate(current))
                {
                    instance.Add(selector(current));
                }
            }
            return instance.ToImmutableAndFree();
        }

        public static async ValueTask<ImmutableArray<TResult>> SelectAsArrayAsync<TItem, TResult>(this ImmutableArray<TItem> array, Func<TItem, CancellationToken, ValueTask<TResult>> selector, CancellationToken cancellationToken)
        {
            ArrayBuilder<TResult> builder = ArrayBuilder<TResult>.GetInstance(array.Length);
            ImmutableArray<TItem>.Enumerator enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TItem current = enumerator.Current;
                ArrayBuilder<TResult> arrayBuilder = builder;
                arrayBuilder.Add(await selector(current, cancellationToken).ConfigureAwait(continueOnCapturedContext: false));
            }
            return builder.ToImmutableAndFree();
        }

        public static ImmutableArray<TResult> ZipAsArray<T1, T2, TResult>(this ImmutableArray<T1> self, ImmutableArray<T2> other, Func<T1, T2, TResult> map)
        {
            switch (self.Length)
            {
                case 0:
                    return ImmutableArray<TResult>.Empty;
                case 1:
                    return ImmutableArray.Create(map(self[0], other[0]));
                case 2:
                    return ImmutableArray.Create(map(self[0], other[0]), map(self[1], other[1]));
                case 3:
                    return ImmutableArray.Create(map(self[0], other[0]), map(self[1], other[1]), map(self[2], other[2]));
                case 4:
                    return ImmutableArray.Create(map(self[0], other[0]), map(self[1], other[1]), map(self[2], other[2]), map(self[3], other[3]));
                default:
                    {
                        ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance(self.Length);
                        for (int i = 0; i < self.Length; i++)
                        {
                            instance.Add(map(self[i], other[i]));
                        }
                        return instance.ToImmutableAndFree();
                    }
            }
        }

        public static ImmutableArray<TResult> ZipAsArray<T1, T2, TArg, TResult>(this ImmutableArray<T1> self, ImmutableArray<T2> other, TArg arg, Func<T1, T2, int, TArg, TResult> map)
        {
            if (self.IsEmpty)
            {
                return ImmutableArray<TResult>.Empty;
            }
            ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance(self.Length);
            for (int i = 0; i < self.Length; i++)
            {
                instance.Add(map(self[i], other[i], i, arg));
            }
            return instance.ToImmutableAndFree();
        }

        public static ImmutableArray<T> WhereAsArray<T>(this ImmutableArray<T> array, Func<T, bool> predicate)
        {
            return WhereAsArrayImpl<T, object>(array, predicate, null, null);
        }

        public static ImmutableArray<T> WhereAsArray<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
        {
            return WhereAsArrayImpl(array, null, predicate, arg);
        }

        private static ImmutableArray<T> WhereAsArrayImpl<T, TArg>(ImmutableArray<T> array, Func<T, bool>? predicateWithoutArg, Func<T, TArg, bool>? predicateWithArg, TArg arg)
        {
            ArrayBuilder<T> arrayBuilder = null;
            bool flag = true;
            bool flag2 = true;
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                T val = array[i];
                if (predicateWithoutArg?.Invoke(val) ?? predicateWithArg!(val, arg))
                {
                    flag = false;
                    if (!flag2)
                    {
                        if (arrayBuilder == null)
                        {
                            arrayBuilder = ArrayBuilder<T>.GetInstance();
                        }
                        arrayBuilder.Add(val);
                    }
                }
                else if (flag)
                {
                    flag2 = false;
                }
                else if (flag2)
                {
                    flag2 = false;
                    arrayBuilder = ArrayBuilder<T>.GetInstance();
                    for (int j = 0; j < i; j++)
                    {
                        arrayBuilder.Add(array[j]);
                    }
                }
            }
            if (arrayBuilder != null)
            {
                return arrayBuilder.ToImmutableAndFree();
            }
            if (flag2)
            {
                return array;
            }
            return ImmutableArray<T>.Empty;
        }

        public static bool Any<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                T arg2 = array[i];
                if (predicate(arg2, arg))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool All<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, bool> predicate, TArg arg)
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                T arg2 = array[i];
                if (!predicate(arg2, arg))
                {
                    return false;
                }
            }
            return true;
        }

        public static async Task<bool> AnyAsync<T>(this ImmutableArray<T> array, Func<T, Task<bool>> predicateAsync)
        {
            int j = array.Length;
            for (int i = 0; i < j; i++)
            {
                T arg = array[i];
                if (await predicateAsync(arg).ConfigureAwait(continueOnCapturedContext: false))
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<bool> AnyAsync<T, TArg>(this ImmutableArray<T> array, Func<T, TArg, Task<bool>> predicateAsync, TArg arg)
        {
            int j = array.Length;
            for (int i = 0; i < j; i++)
            {
                T arg2 = array[i];
                if (await predicateAsync(arg2, arg).ConfigureAwait(continueOnCapturedContext: false))
                {
                    return true;
                }
            }
            return false;
        }

        public static async ValueTask<T?> FirstOrDefaultAsync<T>(this ImmutableArray<T> array, Func<T, Task<bool>> predicateAsync)
        {
            int j = array.Length;
            for (int i = 0; i < j; i++)
            {
                T a = array[i];
                if (await predicateAsync(a).ConfigureAwait(continueOnCapturedContext: false))
                {
                    return a;
                }
            }
            return default(T);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ImmutableArray<TBase> Cast<TDerived, TBase>(this ImmutableArray<TDerived> items) where TDerived : class, TBase
        {
            return ImmutableArray<TBase>.CastUp<TDerived>(items);
        }

        public static bool SetEquals<T>(this ImmutableArray<T> array1, ImmutableArray<T> array2, IEqualityComparer<T> comparer)
        {
            if (array1.IsDefault)
            {
                return array2.IsDefault;
            }
            if (array2.IsDefault)
            {
                return false;
            }
            int length = array1.Length;
            int length2 = array2.Length;
            if (length == 0)
            {
                return length2 == 0;
            }
            if (length2 == 0)
            {
                return false;
            }
            if (length == 1 && length2 == 1)
            {
                T x = array1[0];
                T y = array2[0];
                return comparer.Equals(x, y);
            }
            HashSet<T> hashSet = new HashSet<T>(array1, comparer);
            HashSet<T> equals = new HashSet<T>(array2, comparer);
            return hashSet.SetEquals(equals);
        }

        public static ImmutableArray<T> NullToEmpty<T>(this ImmutableArray<T> array)
        {
            if (!array.IsDefault)
            {
                return array;
            }
            return ImmutableArray<T>.Empty;
        }

        public static ImmutableArray<T> Distinct<T>(this ImmutableArray<T> array, IEqualityComparer<T>? comparer = null)
        {
            if (array.Length < 2)
            {
                return array;
            }
            HashSet<T> hashSet = new HashSet<T>(comparer);
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance();
            ImmutableArray<T>.Enumerator enumerator = array.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (hashSet.Add(current))
                {
                    instance.Add(current);
                }
            }
            object result = ((instance.Count == array.Length) ? array : ((object)instance.ToImmutable()));
            instance.Free();
            return (ImmutableArray<T>)result;
        }

        public static bool HasAnyErrors<T>(this ImmutableArray<T> diagnostics) where T : Diagnostic
        {
            ImmutableArray<T>.Enumerator enumerator = diagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Severity == DiagnosticSeverity.Error)
                {
                    return true;
                }
            }
            return false;
        }

        public static ImmutableArray<T> ConditionallyDeOrder<T>(this ImmutableArray<T> array)
        {
            return array;
        }

        public static ImmutableArray<TValue> Flatten<TKey, TValue>(this Dictionary<TKey, ImmutableArray<TValue>> dictionary, IComparer<TValue>? comparer = null) where TKey : notnull
        {
            if (dictionary.Count == 0)
            {
                return ImmutableArray<TValue>.Empty;
            }
            ArrayBuilder<TValue> instance = ArrayBuilder<TValue>.GetInstance();
            foreach (KeyValuePair<TKey, ImmutableArray<TValue>> item in dictionary)
            {
                instance.AddRange(item.Value);
            }
            if (comparer != null && instance.Count > 1)
            {
                instance.Sort(comparer);
            }
            return instance.ToImmutableAndFree();
        }

        public static ImmutableArray<T> Concat<T>(this ImmutableArray<T> first, ImmutableArray<T> second)
        {
            return first.AddRange(second);
        }

        public static ImmutableArray<T> Concat<T>(this ImmutableArray<T> first, ImmutableArray<T> second, ImmutableArray<T> third)
        {
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance(first.Length + second.Length + third.Length);
            instance.AddRange(first);
            instance.AddRange(second);
            instance.AddRange(third);
            return instance.ToImmutableAndFree();
        }

        public static ImmutableArray<T> Concat<T>(this ImmutableArray<T> first, ImmutableArray<T> second, ImmutableArray<T> third, ImmutableArray<T> fourth)
        {
            ArrayBuilder<T> instance = ArrayBuilder<T>.GetInstance(first.Length + second.Length + third.Length + fourth.Length);
            instance.AddRange(first);
            instance.AddRange(second);
            instance.AddRange(third);
            instance.AddRange(fourth);
            return instance.ToImmutableAndFree();
        }

        public static ImmutableArray<T> Concat<T>(this ImmutableArray<T> first, T second)
        {
            return first.Add(second);
        }

        public static bool HasDuplicates<T>(this ImmutableArray<T> array, IEqualityComparer<T> comparer)
        {
            switch (array.Length)
            {
                case 0:
                case 1:
                    return false;
                case 2:
                    return comparer.Equals(array[0], array[1]);
                default:
                    {
                        HashSet<T> hashSet = new HashSet<T>(comparer);
                        ImmutableArray<T>.Enumerator enumerator = array.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            T current = enumerator.Current;
                            if (!hashSet.Add(current))
                            {
                                return true;
                            }
                        }
                        return false;
                    }
            }
        }

        public static int Count<T>(this ImmutableArray<T> items, Func<T, bool> predicate)
        {
            if (items.IsEmpty)
            {
                return 0;
            }
            int num = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (predicate(items[i]))
                {
                    num++;
                }
            }
            return num;
        }

        public static T[] DangerousGetUnderlyingArray<T>(this ImmutableArray<T> array)
        {
            return Unsafe.As<ImmutableArray<T>, ImmutableArrayProxy<T>>(ref array).MutableArray;
        }

        public static ReadOnlySpan<T> AsSpan<T>(this ImmutableArray<T> array)
        {
            return array.DangerousGetUnderlyingArray();
        }

        internal static ImmutableArray<T> DangerousCreateFromUnderlyingArray<T>([System.Diagnostics.CodeAnalysis.MaybeNull] ref T[] array)
        {
            ImmutableArrayProxy<T> immutableArrayProxy = default(ImmutableArrayProxy<T>);
            immutableArrayProxy.MutableArray = array;
            ImmutableArrayProxy<T> source = immutableArrayProxy;
            array = null;
            return Unsafe.As<ImmutableArrayProxy<T>, ImmutableArray<T>>(ref source);
        }

        public static Dictionary<K, ImmutableArray<T>> ToDictionary<K, T>(this ImmutableArray<T> items, Func<T, K> keySelector, IEqualityComparer<K>? comparer = null) where K : notnull
        {
            if (items.Length == 1)
            {
                Dictionary<K, ImmutableArray<T>> dictionary = new Dictionary<K, ImmutableArray<T>>(1, comparer);
                T val = items[0];
                dictionary.Add(keySelector(val), ImmutableArray.Create(val));
                return dictionary;
            }
            if (items.Length == 0)
            {
                return new Dictionary<K, ImmutableArray<T>>(comparer);
            }
            Dictionary<K, ArrayBuilder<T>> dictionary2 = new Dictionary<K, ArrayBuilder<T>>(items.Length, comparer);
            for (int i = 0; i < items.Length; i++)
            {
                T val2 = items[i];
                K key = keySelector(val2);
                if (!dictionary2.TryGetValue(key, out var value))
                {
                    value = ArrayBuilder<T>.GetInstance();
                    dictionary2.Add(key, value);
                }
                value.Add(val2);
            }
            Dictionary<K, ImmutableArray<T>> dictionary3 = new Dictionary<K, ImmutableArray<T>>(dictionary2.Count, comparer);
            foreach (KeyValuePair<K, ArrayBuilder<T>> item in dictionary2)
            {
                dictionary3.Add(item.Key, item.Value.ToImmutableAndFree());
            }
            return dictionary3;
        }

        public static Location FirstOrNone(this ImmutableArray<Location> items)
        {
            if (!items.IsEmpty)
            {
                return items[0];
            }
            return Location.None;
        }

        public static bool SequenceEqual<TElement, TArg>(this ImmutableArray<TElement> array1, ImmutableArray<TElement> array2, TArg arg, Func<TElement, TElement, TArg, bool> predicate)
        {
            if (array1.IsDefault)
            {
                throw new NullReferenceException();
            }
            if (array2.IsDefault)
            {
                throw new NullReferenceException();
            }
            if (array1.Length != array2.Length)
            {
                return false;
            }
            for (int i = 0; i < array1.Length; i++)
            {
                if (!predicate(array1[i], array2[i], arg))
                {
                    return false;
                }
            }
            return true;
        }

        public static int IndexOf<T>(this ImmutableArray<T> array, T item, IEqualityComparer<T> comparer)
        {
            return array.IndexOf(item, 0, comparer);
        }
    }
}
