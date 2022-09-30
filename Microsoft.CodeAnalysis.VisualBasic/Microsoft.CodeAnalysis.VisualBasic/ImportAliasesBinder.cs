using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class ImportAliasesBinder : Binder
	{
		private readonly IReadOnlyDictionary<string, AliasAndImportsClausePosition> _importedAliases;

		public override Symbol ContainingMember => base.Compilation.SourceModule;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		public ImportAliasesBinder(Binder containingBinder, IReadOnlyDictionary<string, AliasAndImportsClausePosition> importedAliases)
			: base(containingBinder)
		{
			_importedAliases = importedAliases;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			AliasAndImportsClausePosition value = default(AliasAndImportsClausePosition);
			if (_importedAliases.TryGetValue(name, out value))
			{
				SingleLookupResult from = CheckViability(value.Alias, arity, options, null, ref useSiteInfo);
				if (from.IsGoodOrAmbiguous && !originalBinder.IsSemanticModelBinder)
				{
					base.Compilation.MarkImportDirectiveAsUsed(base.SyntaxTree, value.ImportsClausePosition);
				}
				lookupResult.SetFrom(from);
			}
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			foreach (AliasAndImportsClausePosition value in _importedAliases.Values)
			{
				NamespaceOrTypeSymbol target = value.Alias.Target;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				if (originalBinder.CheckViability(target, -1, options, null, ref useSiteInfo).IsGoodOrAmbiguous)
				{
					nameSet.AddSymbol(value.Alias, value.Alias.Name, 0);
				}
			}
		}
	}
}
