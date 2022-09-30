using System.Collections.Generic;

using Microsoft.CodeAnalysis.Collections;

namespace System.Linq
{
    internal static class RoslynEnumerable
    {
        public static SegmentedList<TSource> ToSegmentedList<TSource>(this IEnumerable<TSource> source)
        {
            if (source == null)
            {
                Microsoft.CodeAnalysis.Collections.Internal.ThrowHelper.ThrowArgumentNullException(Microsoft.CodeAnalysis.Collections.Internal.ExceptionArgument.source);
            }
            return new SegmentedList<TSource>(source);
        }
    }
}
