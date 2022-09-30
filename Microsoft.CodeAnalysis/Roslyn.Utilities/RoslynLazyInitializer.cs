using System;
using System.Threading;

#nullable enable

namespace Roslyn.Utilities
{
    internal static class RoslynLazyInitializer
    {
        public static T EnsureInitialized<T>([System.Diagnostics.CodeAnalysis.NotNull] ref T? target) where T : class
        {
            return LazyInitializer.EnsureInitialized(ref target);
        }

        public static T EnsureInitialized<T>([System.Diagnostics.CodeAnalysis.NotNull] ref T? target, Func<T> valueFactory) where T : class
        {
            return LazyInitializer.EnsureInitialized(ref target, valueFactory);
        }

        public static T EnsureInitialized<T>([System.Diagnostics.CodeAnalysis.NotNull] ref T? target, ref bool initialized, [System.Diagnostics.CodeAnalysis.NotNullIfNotNull("syncLock")] ref object? syncLock)
        {
            return LazyInitializer.EnsureInitialized(ref target, ref initialized, ref syncLock);
        }

        public static T EnsureInitialized<T>([System.Diagnostics.CodeAnalysis.NotNull] ref T? target, ref bool initialized, [System.Diagnostics.CodeAnalysis.NotNullIfNotNull("syncLock")] ref object? syncLock, Func<T> valueFactory)
        {
            return LazyInitializer.EnsureInitialized<T>(ref target, ref initialized, ref syncLock, valueFactory);
        }
    }
}
