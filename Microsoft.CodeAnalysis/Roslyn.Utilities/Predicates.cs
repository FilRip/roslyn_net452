using System;

namespace Roslyn.Utilities
{
    internal static class Predicates<T>
    {
        public static readonly Predicate<T> True = (T t) => true;
    }
}
