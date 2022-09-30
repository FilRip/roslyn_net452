using System;
using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.Collections
{
    internal static class RoslynImmutableInterlocked
    {
        public static bool Update<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, Func<ImmutableSegmentedDictionary<TKey, TValue>, ImmutableSegmentedDictionary<TKey, TValue>> transformer) where TKey : notnull
        {
            if (transformer == null)
            {
                throw new ArgumentNullException("transformer");
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            while (true)
            {
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = transformer(immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    return false;
                }
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary3 = InterlockedCompareExchange(ref location, immutableSegmentedDictionary2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary3)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary3;
            }
            return true;
        }

        public static bool Update<TKey, TValue, TArg>(ref ImmutableSegmentedDictionary<TKey, TValue> location, Func<ImmutableSegmentedDictionary<TKey, TValue>, TArg, ImmutableSegmentedDictionary<TKey, TValue>> transformer, TArg transformerArgument) where TKey : notnull
        {
            if (transformer == null)
            {
                throw new ArgumentNullException("transformer");
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            while (true)
            {
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = transformer(immutableSegmentedDictionary, transformerArgument);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    return false;
                }
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary3 = InterlockedCompareExchange(ref location, immutableSegmentedDictionary2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary3)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary3;
            }
            return true;
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> InterlockedExchange<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, ImmutableSegmentedDictionary<TKey, TValue> value) where TKey : notnull
        {
            return ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.InterlockedExchange(ref location, value);
        }

        public static ImmutableSegmentedDictionary<TKey, TValue> InterlockedCompareExchange<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, ImmutableSegmentedDictionary<TKey, TValue> value, ImmutableSegmentedDictionary<TKey, TValue> comparand) where TKey : notnull
        {
            return ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.InterlockedCompareExchange(ref location, value, comparand);
        }

        public static bool InterlockedInitialize<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, ImmutableSegmentedDictionary<TKey, TValue> value) where TKey : notnull
        {
            return InterlockedCompareExchange(ref location, value, default(ImmutableSegmentedDictionary<TKey, TValue>)).IsDefault;
        }

        public static TValue GetOrAdd<TKey, TValue, TArg>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, Func<TKey, TArg, TValue> valueFactory, TArg factoryArgument) where TKey : notnull
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            if (immutableSegmentedDictionary.IsDefault)
            {
                throw new ArgumentNullException("location");
            }
            if (immutableSegmentedDictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            value = valueFactory(key, factoryArgument);
            return GetOrAdd(ref location, key, value);
        }

        public static TValue GetOrAdd<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, Func<TKey, TValue> valueFactory) where TKey : notnull
        {
            if (valueFactory == null)
            {
                throw new ArgumentNullException("valueFactory");
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            if (immutableSegmentedDictionary.IsDefault)
            {
                throw new ArgumentNullException("location");
            }
            if (immutableSegmentedDictionary.TryGetValue(key, out var value))
            {
                return value;
            }
            value = valueFactory(key);
            return GetOrAdd(ref location, key, value);
        }

        public static TValue GetOrAdd<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, TValue value) where TKey : notnull
        {
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            while (true)
            {
                if (immutableSegmentedDictionary.IsDefault)
                {
                    throw new ArgumentNullException("location");
                }
                if (immutableSegmentedDictionary.TryGetValue(key, out var value2))
                {
                    return value2;
                }
                ImmutableSegmentedDictionary<TKey, TValue> value3 = immutableSegmentedDictionary.Add(key, value);
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = InterlockedCompareExchange(ref location, value3, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary2;
            }
            return value;
        }

        public static TValue AddOrUpdate<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory) where TKey : notnull
        {
            if (addValueFactory == null)
            {
                throw new ArgumentNullException("addValueFactory");
            }
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            TValue val;
            while (true)
            {
                if (immutableSegmentedDictionary.IsDefault)
                {
                    throw new ArgumentNullException("location");
                }
                val = ((!immutableSegmentedDictionary.TryGetValue(key, out var value)) ? addValueFactory(key) : updateValueFactory(key, value));
                ImmutableSegmentedDictionary<TKey, TValue> value2 = immutableSegmentedDictionary.SetItem(key, val);
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = InterlockedCompareExchange(ref location, value2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary2;
            }
            return val;
        }

        public static TValue AddOrUpdate<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory) where TKey : notnull
        {
            if (updateValueFactory == null)
            {
                throw new ArgumentNullException("updateValueFactory");
            }
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            TValue val;
            while (true)
            {
                if (immutableSegmentedDictionary.IsDefault)
                {
                    throw new ArgumentNullException("location");
                }
                val = (TValue)((!immutableSegmentedDictionary.TryGetValue(key, out var value)) ? addValue : ((object)updateValueFactory(key, value)));
                ImmutableSegmentedDictionary<TKey, TValue> value2 = immutableSegmentedDictionary.SetItem(key, val);
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = InterlockedCompareExchange(ref location, value2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary2;
            }
            return val;
        }

        public static bool TryAdd<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, TValue value) where TKey : notnull
        {
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            while (true)
            {
                if (immutableSegmentedDictionary.IsDefault)
                {
                    throw new ArgumentNullException("location");
                }
                if (immutableSegmentedDictionary.ContainsKey(key))
                {
                    return false;
                }
                ImmutableSegmentedDictionary<TKey, TValue> value2 = immutableSegmentedDictionary.Add(key, value);
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = InterlockedCompareExchange(ref location, value2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary2;
            }
            return true;
        }

        public static bool TryUpdate<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, TValue newValue, TValue comparisonValue) where TKey : notnull
        {
            EqualityComparer<TValue> @default = EqualityComparer<TValue>.Default;
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            while (true)
            {
                if (immutableSegmentedDictionary.IsDefault)
                {
                    throw new ArgumentNullException("location");
                }
                if (!immutableSegmentedDictionary.TryGetValue(key, out var value) || !@default.Equals(value, comparisonValue))
                {
                    return false;
                }
                ImmutableSegmentedDictionary<TKey, TValue> value2 = immutableSegmentedDictionary.SetItem(key, newValue);
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = InterlockedCompareExchange(ref location, value2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary2;
            }
            return true;
        }

        public static bool TryRemove<TKey, TValue>(ref ImmutableSegmentedDictionary<TKey, TValue> location, TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value) where TKey : notnull
        {
            ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary = ImmutableSegmentedDictionary<TKey, TValue>.PrivateInterlocked.VolatileRead(in location);
            while (true)
            {
                if (immutableSegmentedDictionary.IsDefault)
                {
                    throw new ArgumentNullException("location");
                }
                if (!immutableSegmentedDictionary.TryGetValue(key, out value))
                {
                    return false;
                }
                ImmutableSegmentedDictionary<TKey, TValue> value2 = immutableSegmentedDictionary.Remove(key);
                ImmutableSegmentedDictionary<TKey, TValue> immutableSegmentedDictionary2 = InterlockedCompareExchange(ref location, value2, immutableSegmentedDictionary);
                if (immutableSegmentedDictionary == immutableSegmentedDictionary2)
                {
                    break;
                }
                immutableSegmentedDictionary = immutableSegmentedDictionary2;
            }
            return true;
        }
    }
}
