using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ImportedTypesAndNamespacesMembersBinder : Binder
	{
		private readonly ImmutableArray<NamespaceOrTypeAndImportsClausePosition> _importedSymbols;

		internal static Func<ImmutableArray<Symbol>, AmbiguousSymbolDiagnostic> GenerateAmbiguityError = (ImmutableArray<Symbol> ambiguousSymbols) => new AmbiguousSymbolDiagnostic(ERRID.ERR_AmbiguousInImports2, ambiguousSymbols, ambiguousSymbols[0].Name, new FormattedSymbolList(ambiguousSymbols.Select((Symbol sym) => sym.ContainingSymbol)));

		public ImportedTypesAndNamespacesMembersBinder(Binder containingBinder, ImmutableArray<NamespaceOrTypeAndImportsClausePosition> importedSymbols)
			: base(containingBinder)
		{
			_importedSymbols = importedSymbols;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			options |= LookupOptions.IgnoreExtensionMethods;
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceOrTypeAndImportsClausePosition current = enumerator.Current;
				LookupResult instance = LookupResult.GetInstance();
				if (current.NamespaceOrType.IsNamespace)
				{
					originalBinder.LookupMemberImmediate(instance, (NamespaceSymbol)current.NamespaceOrType, name, arity, options, ref useSiteInfo);
				}
				else
				{
					originalBinder.LookupMember(instance, current.NamespaceOrType, name, arity, options, ref useSiteInfo);
				}
				if (instance.IsGoodOrAmbiguous && !originalBinder.IsSemanticModelBinder)
				{
					base.Compilation.MarkImportDirectiveAsUsed(base.SyntaxTree, current.ImportsClausePosition);
				}
				bool isAmbiguous = instance.IsAmbiguous;
				if (isAmbiguous)
				{
					lookupResult.SetFrom(instance);
				}
				else if (!instance.IsGood || !instance.HasSingleSymbol || instance.SingleSymbol.Kind != SymbolKind.Namespace || ((NamespaceSymbol)instance.SingleSymbol).ContainsTypesAccessibleFrom(base.Compilation.Assembly))
				{
					if (lookupResult.StopFurtherLookup && instance.StopFurtherLookup)
					{
						bool flag = lookupResult.Symbols[0].Kind == SymbolKind.Namespace;
						bool flag2 = instance.Symbols[0].Kind == SymbolKind.Namespace;
						if (flag && !flag2)
						{
							lookupResult.SetFrom(instance);
						}
						else if ((!flag2 || flag) && (lookupResult.Symbols.Count != instance.Symbols.Count || !lookupResult.Symbols[0].Equals(instance.Symbols[0])))
						{
							if (flag && instance.IsGood && lookupResult.IsGood)
							{
								lookupResult.Symbols.AddRange(instance.Symbols);
							}
							else
							{
								lookupResult.MergeAmbiguous(instance, GenerateAmbiguityError);
							}
						}
					}
					else
					{
						lookupResult.MergeAmbiguous(instance, GenerateAmbiguityError);
					}
				}
				instance.Free();
				if (isAmbiguous)
				{
					break;
				}
			}
			if (lookupResult.IsGood && lookupResult.Symbols.Count > 1 && lookupResult.Symbols[0].Kind == SymbolKind.Namespace)
			{
				lookupResult.SetFrom(MergedNamespaceSymbol.CreateNamespaceGroup(lookupResult.Symbols.Cast<NamespaceSymbol>()));
			}
		}

		protected override void CollectProbableExtensionMethodsInSingleBinder(string name, ArrayBuilder<MethodSymbol> methods, Binder originalBinder)
		{
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceOrTypeAndImportsClausePosition current = enumerator.Current;
				if (current.NamespaceOrType.Kind == SymbolKind.NamedType)
				{
					((NamedTypeSymbol)current.NamespaceOrType).AppendProbableExtensionMethods(name, methods);
					if (methods.Count != 0 && !originalBinder.IsSemanticModelBinder)
					{
						base.Compilation.MarkImportDirectiveAsUsed(base.SyntaxTree, current.ImportsClausePosition);
					}
				}
			}
		}

		protected override void AddExtensionMethodLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceOrTypeAndImportsClausePosition current = enumerator.Current;
				if (current.NamespaceOrType.Kind == SymbolKind.NamedType)
				{
					((NamedTypeSymbol)current.NamespaceOrType).AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder);
				}
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				originalBinder.AddMemberLookupSymbolsInfo(nameSet, enumerator.Current.NamespaceOrType, options | LookupOptions.IgnoreExtensionMethods);
			}
		}
	}
}
