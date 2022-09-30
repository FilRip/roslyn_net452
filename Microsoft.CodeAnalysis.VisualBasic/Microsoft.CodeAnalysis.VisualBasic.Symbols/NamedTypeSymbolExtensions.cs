using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class NamedTypeSymbolExtensions
	{
		internal static bool IsOrInGenericType(this NamedTypeSymbol toCheck)
		{
			return toCheck?.IsGenericType ?? false;
		}

		internal static Symbol FindMember(this NamedTypeSymbol container, string symbolName, SymbolKind kind, TextSpan nameSpan, SyntaxTree tree)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = container.GetMembers(symbolName).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != kind)
				{
					continue;
				}
				ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Location current2 = enumerator2.Current;
					if (current2.IsInSource && current2.SourceTree == tree && current2.SourceSpan == nameSpan)
					{
						return current;
					}
				}
				if (kind != SymbolKind.Method)
				{
					continue;
				}
				MethodSymbol partialImplementationPart = ((MethodSymbol)current).PartialImplementationPart;
				if ((object)partialImplementationPart == null)
				{
					continue;
				}
				ImmutableArray<Location>.Enumerator enumerator3 = partialImplementationPart.Locations.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					Location current3 = enumerator3.Current;
					if (current3.IsInSource && current3.SourceTree == tree && current3.SourceSpan == nameSpan)
					{
						return partialImplementationPart;
					}
				}
			}
			return null;
		}

		internal static Symbol FindFieldOrProperty(this NamedTypeSymbol container, string symbolName, TextSpan nameSpan, SyntaxTree tree)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = container.GetMembers(symbolName).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind != SymbolKind.Field && current.Kind != SymbolKind.Property)
				{
					continue;
				}
				ImmutableArray<Location>.Enumerator enumerator2 = current.Locations.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					Location current2 = enumerator2.Current;
					if (current2.IsInSource && current2.SourceTree == tree && current2.SourceSpan == nameSpan)
					{
						return current;
					}
				}
			}
			return null;
		}

		public static NamedTypeSymbol AsUnboundGenericType(this NamedTypeSymbol @this)
		{
			return UnboundGenericType.Create(@this);
		}

		internal static bool HasVariance(this NamedTypeSymbol @this)
		{
			NamedTypeSymbol namedTypeSymbol = @this;
			do
			{
				if (HaveVariance(namedTypeSymbol.TypeParameters))
				{
					return true;
				}
				namedTypeSymbol = namedTypeSymbol.ContainingType;
			}
			while ((object)namedTypeSymbol != null);
			return false;
		}

		internal static bool HaveVariance(this ImmutableArray<TypeParameterSymbol> @this)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = @this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				VarianceKind variance = enumerator.Current.Variance;
				if ((uint)(variance - 1) <= 1u)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool AllowsExtensionMethods(this NamedTypeSymbol container)
		{
			if (container.TypeKind != TypeKind.Module)
			{
				return container.IsScriptClass;
			}
			return true;
		}
	}
}
