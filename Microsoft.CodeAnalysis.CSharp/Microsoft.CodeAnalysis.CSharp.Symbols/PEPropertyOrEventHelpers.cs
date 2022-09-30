using System.Collections.Generic;
using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal static class PEPropertyOrEventHelpers
    {
        internal static ISet<PropertySymbol> GetPropertiesForExplicitlyImplementedAccessor(MethodSymbol accessor)
        {
            return GetSymbolsForExplicitlyImplementedAccessor<PropertySymbol>(accessor);
        }

        internal static ISet<EventSymbol> GetEventsForExplicitlyImplementedAccessor(MethodSymbol accessor)
        {
            return GetSymbolsForExplicitlyImplementedAccessor<EventSymbol>(accessor);
        }

        private static ISet<T> GetSymbolsForExplicitlyImplementedAccessor<T>(MethodSymbol accessor) where T : Symbol
        {
            if ((object)accessor == null)
            {
                return SpecializedCollections.EmptySet<T>();
            }
            ImmutableArray<MethodSymbol> explicitInterfaceImplementations = accessor.ExplicitInterfaceImplementations;
            if (explicitInterfaceImplementations.Length == 0)
            {
                return SpecializedCollections.EmptySet<T>();
            }
            HashSet<T> hashSet = new HashSet<T>();
            ImmutableArray<MethodSymbol>.Enumerator enumerator = explicitInterfaceImplementations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.AssociatedSymbol is T item)
                {
                    hashSet.Add(item);
                }
            }
            return hashSet;
        }

        internal static Accessibility GetDeclaredAccessibilityFromAccessors(MethodSymbol accessor1, MethodSymbol accessor2)
        {
            if ((object)accessor1 == null)
            {
                return accessor2?.DeclaredAccessibility ?? Accessibility.NotApplicable;
            }
            if ((object)accessor2 == null)
            {
                return accessor1.DeclaredAccessibility;
            }
            return GetDeclaredAccessibilityFromAccessors(accessor1.DeclaredAccessibility, accessor2.DeclaredAccessibility);
        }

        internal static Accessibility GetDeclaredAccessibilityFromAccessors(Accessibility accessibility1, Accessibility accessibility2)
        {
            Accessibility num = ((accessibility1 > accessibility2) ? accessibility2 : accessibility1);
            Accessibility accessibility3 = ((accessibility1 > accessibility2) ? accessibility1 : accessibility2);
            if (num != Accessibility.Protected || accessibility3 != Accessibility.Internal)
            {
                return accessibility3;
            }
            return Accessibility.ProtectedOrInternal;
        }
    }
}
