using System.Collections.Generic;
using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class PEPropertyOrEventHelpers
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
			Accessibility declaredAccessibility = accessor1.DeclaredAccessibility;
			Accessibility declaredAccessibility2 = accessor2.DeclaredAccessibility;
			Accessibility num = ((declaredAccessibility > declaredAccessibility2) ? declaredAccessibility2 : declaredAccessibility);
			Accessibility accessibility = ((declaredAccessibility > declaredAccessibility2) ? declaredAccessibility : declaredAccessibility2);
			return (num == Accessibility.Protected && accessibility == Accessibility.Internal) ? Accessibility.ProtectedOrInternal : accessibility;
		}
	}
}
