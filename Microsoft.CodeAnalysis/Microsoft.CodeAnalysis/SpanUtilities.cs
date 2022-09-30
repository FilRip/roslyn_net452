using System;

namespace Microsoft.CodeAnalysis
{
    internal static class SpanUtilities
    {
        public static bool All<TElement, TParam>(this ReadOnlySpan<TElement> span, TParam param, Func<TElement, TParam, bool> predicate)
        {
            ReadOnlySpan<TElement> readOnlySpan = span;
            for (int i = 0; i < readOnlySpan.Length; i++)
            {
                TElement arg = readOnlySpan[i];
                if (!predicate(arg, param))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
