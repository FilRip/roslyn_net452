using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class TypesOfImportedNamespacesMembersBinder : Binder
	{
		private readonly ImmutableArray<NamespaceOrTypeAndImportsClausePosition> _importedSymbols;

		public TypesOfImportedNamespacesMembersBinder(Binder containingBinder, ImmutableArray<NamespaceOrTypeAndImportsClausePosition> importedSymbols)
			: base(containingBinder)
		{
			_importedSymbols = importedSymbols;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceOrTypeAndImportsClausePosition current = enumerator.Current;
				if (current.NamespaceOrType.IsNamespace)
				{
					LookupResult instance = LookupResult.GetInstance();
					originalBinder.LookupMemberInModules(instance, (NamespaceSymbol)current.NamespaceOrType, name, arity, options, ref useSiteInfo);
					if (instance.IsGood && !originalBinder.IsSemanticModelBinder)
					{
						base.Compilation.MarkImportDirectiveAsUsed(base.SyntaxTree, current.ImportsClausePosition);
					}
					lookupResult.MergeAmbiguous(instance, ImportedTypesAndNamespacesMembersBinder.GenerateAmbiguityError);
					instance.Free();
				}
			}
		}

		protected override void CollectProbableExtensionMethodsInSingleBinder(string name, ArrayBuilder<MethodSymbol> methods, Binder originalBinder)
		{
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceOrTypeAndImportsClausePosition current = enumerator.Current;
				if (current.NamespaceOrType.IsNamespace)
				{
					int count = methods.Count;
					((NamespaceSymbol)current.NamespaceOrType).AppendProbableExtensionMethods(name, methods);
					if (methods.Count != count && !originalBinder.IsSemanticModelBinder)
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
				if (current.NamespaceOrType.IsNamespace)
				{
					((NamespaceSymbol)current.NamespaceOrType).AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder);
				}
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			options |= LookupOptions.IgnoreExtensionMethods;
			ImmutableArray<NamespaceOrTypeAndImportsClausePosition>.Enumerator enumerator = _importedSymbols.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamespaceOrTypeAndImportsClausePosition current = enumerator.Current;
				if (current.NamespaceOrType.IsNamespace)
				{
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = ((NamespaceSymbol)current.NamespaceOrType).GetModuleMembers().GetEnumerator();
					while (enumerator2.MoveNext())
					{
						NamedTypeSymbol current2 = enumerator2.Current;
						originalBinder.AddMemberLookupSymbolsInfo(nameSet, current2, options);
					}
				}
			}
		}
	}
}
