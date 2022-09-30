using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	[StandardModule]
	internal sealed class OverloadingHelper
	{
		public static void SetMetadataNameForAllOverloads(string name, SymbolKind kind, NamedTypeSymbol container)
		{
			VisualBasicCompilation declaringCompilation = container.DeclaringCompilation;
			ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
			bool hasOverloadSpecifier = false;
			bool hasOverrideSpecifier = false;
			string text = null;
			try
			{
				FindOverloads(name, kind, container, instance, ref hasOverloadSpecifier, ref hasOverrideSpecifier);
				if (instance.Count == 1 && !hasOverloadSpecifier && !hasOverrideSpecifier)
				{
					instance[0].SetMetadataName(instance[0].Name);
					return;
				}
				if (hasOverrideSpecifier)
				{
					text = SetMetadataNamesOfOverrides(instance, declaringCompilation);
				}
				else if (hasOverloadSpecifier)
				{
					text = GetBaseMemberMetadataName(name, kind, container);
				}
				if (text == null)
				{
					text = NameOfFirstMember(instance, declaringCompilation);
				}
				ArrayBuilder<Symbol>.Enumerator enumerator = instance.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (!current.IsOverrides || (object)SymbolExtensions.OverriddenMember(current) == null)
					{
						current.SetMetadataName(text);
					}
				}
			}
			finally
			{
				instance.Free();
			}
		}

		private static void FindOverloads(string name, SymbolKind kind, NamedTypeSymbol container, ArrayBuilder<Symbol> overloadsMembers, ref bool hasOverloadSpecifier, ref bool hasOverrideSpecifier)
		{
			ImmutableArray<Symbol>.Enumerator enumerator = container.GetMembers(name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (IsCandidateMember(current, kind))
				{
					overloadsMembers.Add(current);
					if (current.IsOverrides)
					{
						hasOverrideSpecifier = true;
					}
					else if (SymbolExtensions.IsOverloads(current))
					{
						hasOverloadSpecifier = true;
					}
				}
			}
		}

		private static string SetMetadataNamesOfOverrides(ArrayBuilder<Symbol> overloadedMembers, VisualBasicCompilation compilation)
		{
			Location second = null;
			string text = null;
			ArrayBuilder<Symbol>.Enumerator enumerator = overloadedMembers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (!current.IsOverrides)
				{
					continue;
				}
				Symbol symbol = SymbolExtensions.OverriddenMember(current);
				if ((object)symbol != null)
				{
					string metadataName = symbol.MetadataName;
					current.SetMetadataName(metadataName);
					if (text == null || compilation.CompareSourceLocations(current.Locations[0], second) < 0)
					{
						text = metadataName;
						second = current.Locations[0];
					}
				}
			}
			return text;
		}

		private static string NameOfFirstMember(ArrayBuilder<Symbol> overloadedMembers, VisualBasicCompilation compilation)
		{
			string text = null;
			Location second = null;
			ArrayBuilder<Symbol>.Enumerator enumerator = overloadedMembers.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				Location location = current.Locations[0];
				if (text == null || compilation.CompareSourceLocations(location, second) < 0)
				{
					text = current.Name;
					second = location;
				}
			}
			return text;
		}

		private static string GetBaseMemberMetadataName(string name, SymbolKind kind, NamedTypeSymbol container)
		{
			string text = null;
			Binder binder = BinderBuilder.CreateBinderForType((SourceModuleSymbol)container.ContainingModule, LocationExtensions.PossiblyEmbeddedOrMySourceTree(container.Locations[0]), container);
			LookupResult instance = LookupResult.GetInstance();
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			binder.LookupMember(instance, container, name, 0, LookupOptions.AllMethodsOfAnyArity | LookupOptions.IgnoreExtensionMethods, ref useSiteInfo);
			if (instance.IsGoodOrAmbiguous)
			{
				ArrayBuilder<Symbol> symbols = instance.Symbols;
				if (instance.Kind == LookupResultKind.Ambiguous && instance.HasDiagnostic && instance.Diagnostic is AmbiguousSymbolDiagnostic)
				{
					symbols.AddRange(((AmbiguousSymbolDiagnostic)instance.Diagnostic).AmbiguousSymbols);
				}
				ArrayBuilder<Symbol>.Enumerator enumerator = symbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (IsCandidateMember(current, kind) && (object)current.ContainingType != container)
					{
						if (text == null)
						{
							text = current.MetadataName;
						}
						else if (!string.Equals(text, current.MetadataName, StringComparison.Ordinal))
						{
							text = null;
							break;
						}
					}
				}
			}
			instance.Free();
			return text;
		}

		private static bool IsCandidateMember(Symbol member, SymbolKind kind)
		{
			if (member.Kind == kind)
			{
				return !SymbolExtensions.IsAccessor(member);
			}
			return false;
		}
	}
}
