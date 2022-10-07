using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.Diagnostics
{
    internal class AnalysisValueProvider<TKey, TValue> where TKey : class
    {
        private sealed class WrappedValue
        {
            public TValue Value { get; }

            public WrappedValue(TValue value)
            {
                Value = value;
            }
        }

        private readonly Func<TKey, TValue> _computeValue;

        private readonly ConditionalWeakTable<TKey, WrappedValue> _valueCache;

        private readonly ConditionalWeakTable<TKey, WrappedValue>.CreateValueCallback _valueCacheCallback;

        internal IEqualityComparer<TKey> KeyComparer { get; private set; }

        public AnalysisValueProvider(Func<TKey, TValue> computeValue, IEqualityComparer<TKey> keyComparer)
        {
            _computeValue = computeValue;
            KeyComparer = keyComparer ?? EqualityComparer<TKey>.Default;
            _valueCache = new ConditionalWeakTable<TKey, WrappedValue>();
            _valueCacheCallback = ComputeValue;
        }

        private WrappedValue ComputeValue(TKey key)
        {
            return new WrappedValue(_computeValue(key));
        }

        internal bool TryGetValue(TKey key, [System.Diagnostics.CodeAnalysis.MaybeNullWhen(false)] out TValue value)
        {
            try
            {
                value = _valueCache.GetValue(key, _valueCacheCallback).Value;
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }
    }
}
