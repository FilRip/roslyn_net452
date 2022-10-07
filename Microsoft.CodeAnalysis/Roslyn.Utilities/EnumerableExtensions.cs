using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

namespace Roslyn.Utilities
{
    public static class EnumerableExtensions
    {
        private static class Comparisons<T> where T : IComparable<T>
        {
            public static readonly Comparison<T> CompareTo = (T t1, T t2) => t1.CompareTo(t2);

            public static readonly IComparer<T> Comparer = Comparer<T>.Create(CompareTo);
        }

        private static readonly Func<object, bool> s_notNullTest = (object x) => x != null;

        public static IEnumerable<T> Do<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }
            if (source is IList<T> list)
            {
                int i = 0;
                for (int count = list.Count; i < count; i++)
                {
                    action(list[i]);
                }
                return source;
            }
            foreach (T item in source)
            {
                action(item);
            }
            return source;
        }

        public static ReadOnlyCollection<T> ToReadOnlyCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new ReadOnlyCollection<T>(source.ToList());
        }

        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T value)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.ConcatWorker(value);
        }

        private static IEnumerable<T> ConcatWorker<T>(this IEnumerable<T> source, T value)
        {
            foreach (T item in source)
            {
                yield return item;
            }
            yield return value;
        }

        public static bool SetEquals<T>(this IEnumerable<T> source1, IEnumerable<T> source2, IEqualityComparer<T>? comparer)
        {
            if (source1 == null)
            {
                throw new ArgumentNullException("source1");
            }
            if (source2 == null)
            {
                throw new ArgumentNullException("source2");
            }
            return source1.ToSet(comparer).SetEquals(source2);
        }

        public static bool SetEquals<T>(this IEnumerable<T> source1, IEnumerable<T> source2)
        {
            if (source1 == null)
            {
                throw new ArgumentNullException("source1");
            }
            if (source2 == null)
            {
                throw new ArgumentNullException("source2");
            }
            return source1.ToSet().SetEquals(source2);
        }

        public static ISet<T> ToSet<T>(this IEnumerable<T> source, IEqualityComparer<T>? comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new HashSet<T>(source, comparer);
        }

        public static ISet<T> ToSet<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return (source as ISet<T>) ?? new HashSet<T>(source);
        }

        public static IReadOnlyCollection<T> ToCollection<T>(this IEnumerable<T> sequence)
        {
            if (!(sequence is IReadOnlyCollection<T> result))
            {
                return sequence.ToList();
            }
            return result;
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> source) where T : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source.Cast<T?>().FirstOrDefault();
        }

        public static T? FirstOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
        {
            Func<T, bool> predicate2 = predicate;
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            return source.Cast<T?>().FirstOrDefault((T? v) => predicate2(v!.Value));
        }

        public static T? LastOrNull<T>(this IEnumerable<T> source) where T : struct
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Cast<T?>().LastOrDefault();
        }

        public static T? SingleOrNull<T>(this IEnumerable<T> source, Func<T, bool> predicate) where T : struct
        {
            Func<T, bool> predicate2 = predicate;
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return source.Cast<T?>().SingleOrDefault((T? v) => predicate2(v!.Value));
        }

        public static bool IsSingle<T>(this IEnumerable<T> list)
        {
            using IEnumerator<T> enumerator = list.GetEnumerator();
            return enumerator.MoveNext() && !enumerator.MoveNext();
        }

        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            if (source is IReadOnlyCollection<T> readOnlyCollection)
            {
                return readOnlyCollection.Count == 0;
            }
            if (source is ICollection<T> collection)
            {
                return collection.Count == 0;
            }
            if (source is ICollection collection2)
            {
                return collection2.Count == 0;
            }
            if (source is string text)
            {
                return text.Length == 0;
            }
            using (IEnumerator<T> enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    _ = enumerator.Current;
                    return false;
                }
            }
            return true;
        }

        public static bool IsEmpty<T>(this IReadOnlyCollection<T> source)
        {
            return source.Count == 0;
        }

        public static bool IsEmpty<T>(this ICollection<T> source)
        {
            return source.Count == 0;
        }

        public static bool IsEmpty(this string source)
        {
            return source.Length == 0;
        }

        public static bool IsEmpty<T>(this T[] source)
        {
            return source.Length == 0;
        }

        public static bool IsEmpty<T>(this List<T> source)
        {
            return source.Count == 0;
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        {
            if (source == null)
            {
                return SpecializedCollections.EmptyEnumerable<T>();
            }
            return source.Where<T>(s_notNullTest);
        }

        public static T[] AsArray<T>(this IEnumerable<T> source)
        {
            return (source as T[]) ?? source.ToArray();
        }

        public static ImmutableArray<TResult> SelectAsArray<TSource, TResult>(this IEnumerable<TSource>? source, Func<TSource, TResult> selector)
        {
            if (source == null)
            {
                return ImmutableArray<TResult>.Empty;
            }
            ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance();
            instance.AddRange(source.Select(selector));
            return instance.ToImmutableAndFree();
        }

        public static async ValueTask<ImmutableArray<TResult>> SelectAsArrayAsync<TItem, TResult>(this IEnumerable<TItem> source, Func<TItem, ValueTask<TResult>> selector)
        {
            ArrayBuilder<TResult> builder = ArrayBuilder<TResult>.GetInstance();
            foreach (TItem item in source)
            {
                ArrayBuilder<TResult> arrayBuilder = builder;
                arrayBuilder.Add(await selector(item).ConfigureAwait(continueOnCapturedContext: false));
            }
            return builder.ToImmutableAndFree();
        }

        public static bool All(this IEnumerable<bool> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            foreach (bool item in source)
            {
                if (!item)
                {
                    return false;
                }
            }
            return true;
        }

        public static int IndexOf<T>(this IEnumerable<T> sequence, T value)
        {
            if (!(sequence is IList<T> list))
            {
                if (sequence is IReadOnlyList<T> list2)
                {
                    return list2.IndexOf(value, EqualityComparer<T>.Default);
                }
                return sequence.EnumeratingIndexOf(value, EqualityComparer<T>.Default);
            }
            return list.IndexOf(value);
        }

        public static int IndexOf<T>(this IEnumerable<T> sequence, T value, IEqualityComparer<T> comparer)
        {
            if (sequence is IReadOnlyList<T> list)
            {
                return list.IndexOf(value, comparer);
            }
            return sequence.EnumeratingIndexOf(value, comparer);
        }

        private static int EnumeratingIndexOf<T>(this IEnumerable<T> sequence, T value, IEqualityComparer<T> comparer)
        {
            int num = 0;
            foreach (T item in sequence)
            {
                if (comparer.Equals(item, value))
                {
                    return num;
                }
                num++;
            }
            return -1;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> list, T value, IEqualityComparer<T> comparer)
        {
            int i = 0;
            for (int count = list.Count; i < count; i++)
            {
                if (comparer.Equals(list[i], value))
                {
                    return i;
                }
            }
            return -1;
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequence)
        {
            if (sequence == null)
            {
                throw new ArgumentNullException("sequence");
            }
            return sequence.SelectMany((IEnumerable<T> s) => s);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, IComparer<T>? comparer)
        {
            return source.OrderBy<T, T>(Functions<T>.Identity, comparer);
        }

        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, IComparer<T>? comparer)
        {
            return source.OrderByDescending<T, T>(Functions<T>.Identity, comparer);
        }

        public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> source, Comparison<T> compare)
        {
            return source.OrderBy(Comparer<T>.Create(compare));
        }

        public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, Comparison<T> compare)
        {
            return source.OrderByDescending(Comparer<T>.Create(compare));
        }

        public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> source) where T : IComparable<T>
        {
            return source.OrderBy(Comparisons<T>.Comparer);
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, IComparer<T>? comparer)
        {
            return source.ThenBy<T, T>(Functions<T>.Identity, comparer);
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source, Comparison<T> compare)
        {
            return source.ThenBy(Comparer<T>.Create(compare));
        }

        public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> source) where T : IComparable<T>
        {
            return source.ThenBy(Comparisons<T>.Comparer);
        }

        public static bool IsSorted<T>(this IEnumerable<T> enumerable, IComparer<T> comparer)
        {
            using IEnumerator<T> enumerator = enumerable.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return true;
            }
            T current = enumerator.Current;
            while (enumerator.MoveNext())
            {
                if (comparer.Compare(current, enumerator.Current) > 0)
                {
                    return false;
                }
                current = enumerator.Current;
            }
            return true;
        }

        public static bool Contains<T>(this IEnumerable<T> sequence, Func<T, bool> predicate)
        {
            return sequence.Any(predicate);
        }

        public static bool Contains(this IEnumerable<string?> sequence, string? s)
        {
            foreach (string item in sequence)
            {
                if (item == s)
                {
                    return true;
                }
            }
            return false;
        }

        public static IComparer<T> ToComparer<T>(this Comparison<T> comparison)
        {
            return Comparer<T>.Create(comparison);
        }

        public static ImmutableDictionary<K, V> ToImmutableDictionaryOrEmpty<K, V>(this IEnumerable<KeyValuePair<K, V>>? items) where K : notnull
        {
            if (items == null)
            {
                return ImmutableDictionary.Create<K, V>();
            }
            return ImmutableDictionary.CreateRange(items);
        }

        public static ImmutableDictionary<K, V> ToImmutableDictionaryOrEmpty<K, V>(this IEnumerable<KeyValuePair<K, V>>? items, IEqualityComparer<K>? keyComparer) where K : notnull
        {
            if (items == null)
            {
                return ImmutableDictionary.Create<K, V>(keyComparer);
            }
            return ImmutableDictionary.CreateRange(keyComparer, items);
        }

        public static IList<IList<T>> Transpose<T>(this IEnumerable<IEnumerable<T>> data)
        {
            return data.TransposeInternal().ToArray();
        }

        private static IEnumerable<IList<T>> TransposeInternal<T>(this IEnumerable<IEnumerable<T>> data)
        {
            List<IEnumerator<T>> enumerators = new List<IEnumerator<T>>();
            int width = 0;
            foreach (IEnumerable<T> datum in data)
            {
                enumerators.Add(datum.GetEnumerator());
                width++;
            }
            try
            {
                while (true)
                {
                    T[] array = null;
                    for (int i = 0; i < width; i++)
                    {
                        IEnumerator<T> enumerator2 = enumerators[i];
                        if (enumerator2.MoveNext())
                        {
                            if (array == null)
                            {
                                array = new T[width];
                            }
                            array[i] = enumerator2.Current;
                            continue;
                        }
                        yield break;
                    }
                    yield return array;
                }
            }
            finally
            {
                foreach (IEnumerator<T> item in enumerators)
                {
                    item.Dispose();
                }
            }
        }

        public static Dictionary<K, ImmutableArray<T>> ToDictionary<K, T>(this IEnumerable<T> data, Func<T, K> keySelector, IEqualityComparer<K>? comparer = null) where K : notnull
        {
            Dictionary<K, ImmutableArray<T>> dictionary = new Dictionary<K, ImmutableArray<T>>(comparer);
            foreach (IGrouping<K, T> item in data.GroupBy(keySelector, comparer))
            {
                ImmutableArray<T> value = item.AsImmutable();
                dictionary.Add(item.Key, value);
            }
            return dictionary;
        }

        public static TSource? AsSingleton<TSource>(this IEnumerable<TSource>? source)
        {
            if (source == null)
            {
                return default(TSource);
            }
            if (source is IList<TSource> list)
            {
                if (list.Count != 1)
                {
                    return default(TSource);
                }
                return list[0];
            }
            using IEnumerator<TSource> enumerator = source!.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                return default(TSource);
            }
            TSource current = enumerator.Current;
            if (enumerator.MoveNext())
            {
                return default(TSource);
            }
            return current;
        }
    }
}
