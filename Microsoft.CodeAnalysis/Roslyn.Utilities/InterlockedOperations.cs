using System.Collections.Immutable;
using System.Threading;

#nullable enable

namespace Roslyn.Utilities
{
    public static class InterlockedOperations
    {
        public static T Initialize<T>([System.Diagnostics.CodeAnalysis.NotNull] ref T? target, T value) where T : class
        {
            return Interlocked.CompareExchange(ref target, value, null) ?? value;
        }

        [return: System.Diagnostics.CodeAnalysis.NotNullIfNotNull("initializedValue")]
        public static T Initialize<T>(ref T target, T initializedValue, T uninitializedValue) where T : class?
        {
            T val = Interlocked.CompareExchange(ref target, initializedValue, uninitializedValue);
            if (val != uninitializedValue)
            {
                return val;
            }
            return initializedValue;
        }

        public static ImmutableArray<T> Initialize<T>(ref ImmutableArray<T> target, ImmutableArray<T> initializedValue)
        {
            ImmutableArray<T> result = ImmutableInterlocked.InterlockedCompareExchange(ref target, initializedValue, default(ImmutableArray<T>));
            if (!result.IsDefault)
            {
                return result;
            }
            return initializedValue;
        }
    }
}
