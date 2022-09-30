using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis
{
    public static class ArrayBuilderExtensions
    {
        public static bool Any<T>(this ArrayBuilder<T> builder, Func<T, bool> predicate)
        {
            ArrayBuilder<T>.Enumerator enumerator = builder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (predicate(current))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool Any<T, A>(this ArrayBuilder<T> builder, Func<T, A, bool> predicate, A arg)
        {
            ArrayBuilder<T>.Enumerator enumerator = builder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (predicate(current, arg))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool All<T>(this ArrayBuilder<T> builder, Func<T, bool> predicate)
        {
            ArrayBuilder<T>.Enumerator enumerator = builder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (!predicate(current))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool All<T, A>(this ArrayBuilder<T> builder, Func<T, A, bool> predicate, A arg)
        {
            ArrayBuilder<T>.Enumerator enumerator = builder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (!predicate(current, arg))
                {
                    return false;
                }
            }
            return true;
        }

        public static ImmutableArray<TResult> SelectAsArray<TItem, TResult>(this ArrayBuilder<TItem> items, Func<TItem, TResult> map)
        {
            switch (items.Count)
            {
                case 0:
                    return ImmutableArray<TResult>.Empty;
                case 1:
                    return ImmutableArray.Create(map(items[0]));
                case 2:
                    return ImmutableArray.Create(map(items[0]), map(items[1]));
                case 3:
                    return ImmutableArray.Create(map(items[0]), map(items[1]), map(items[2]));
                case 4:
                    return ImmutableArray.Create(map(items[0]), map(items[1]), map(items[2]), map(items[3]));
                default:
                    {
                        ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance(items.Count);
                        ArrayBuilder<TItem>.Enumerator enumerator = items.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            TItem current = enumerator.Current;
                            instance.Add(map(current));
                        }
                        return instance.ToImmutableAndFree();
                    }
            }
        }

        public static ImmutableArray<TResult> SelectAsArray<TItem, TArg, TResult>(this ArrayBuilder<TItem> items, Func<TItem, TArg, TResult> map, TArg arg)
        {
            switch (items.Count)
            {
                case 0:
                    return ImmutableArray<TResult>.Empty;
                case 1:
                    return ImmutableArray.Create(map(items[0], arg));
                case 2:
                    return ImmutableArray.Create(map(items[0], arg), map(items[1], arg));
                case 3:
                    return ImmutableArray.Create(map(items[0], arg), map(items[1], arg), map(items[2], arg));
                case 4:
                    return ImmutableArray.Create(map(items[0], arg), map(items[1], arg), map(items[2], arg), map(items[3], arg));
                default:
                    {
                        ArrayBuilder<TResult> instance = ArrayBuilder<TResult>.GetInstance(items.Count);
                        ArrayBuilder<TItem>.Enumerator enumerator = items.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            TItem current = enumerator.Current;
                            instance.Add(map(current, arg));
                        }
                        return instance.ToImmutableAndFree();
                    }
            }
        }

        public static void AddOptional<T>(this ArrayBuilder<T> builder, T? item) where T : class
        {
            if (item != null)
            {
                builder.Add(item);
            }
        }

        public static void Push<T>(this ArrayBuilder<T> builder, T e)
        {
            builder.Add(e);
        }

        public static T Pop<T>(this ArrayBuilder<T> builder)
        {
            T result = builder.Peek();
            builder.RemoveAt(builder.Count - 1);
            return result;
        }

        public static bool TryPop<T>(this ArrayBuilder<T> builder, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out T result)
        {
            if (builder.Count > 0)
            {
                result = builder.Pop();
                return true;
            }
            result = default(T);
            return false;
        }

        public static T Peek<T>(this ArrayBuilder<T> builder)
        {
            return builder[builder.Count - 1];
        }

        public static ImmutableArray<T> ToImmutableOrEmptyAndFree<T>(this ArrayBuilder<T>? builder)
        {
            return builder?.ToImmutableAndFree() ?? ImmutableArray<T>.Empty;
        }

        public static void AddIfNotNull<T>(this ArrayBuilder<T> builder, T? value) where T : struct
        {
            if (value.HasValue)
            {
                builder.Add(value.Value);
            }
        }

        public static void AddIfNotNull<T>(this ArrayBuilder<T> builder, T? value) where T : class
        {
            if (value != null)
            {
                builder.Add(value);
            }
        }

        public static void FreeAll<T>(this ArrayBuilder<T> builder, Func<T, ArrayBuilder<T>> getNested)
        {
            ArrayBuilder<T>.Enumerator enumerator = builder.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                getNested(current)?.FreeAll(getNested);
            }
            builder.Free();
        }
    }
}
