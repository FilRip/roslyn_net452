using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class NamedTypeBinder : Binder
	{
		private readonly NamedTypeSymbol _typeSymbol;

		public override NamespaceOrTypeSymbol ContainingNamespaceOrType => _typeSymbol;

		public override NamedTypeSymbol ContainingType => _typeSymbol;

		public override Symbol ContainingMember => _typeSymbol;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		public override bool IsInQuery => false;

		public NamedTypeBinder(Binder containingBinder, NamedTypeSymbol typeSymbol)
			: base(containingBinder)
		{
			_typeSymbol = typeSymbol;
		}

		public override Binder GetBinder(SyntaxNode node)
		{
			if (!_typeSymbol.IsScriptClass)
			{
				return m_containingBinder.GetBinder(node);
			}
			return this;
		}

		public override Binder GetBinder(SyntaxList<StatementSyntax> stmtList)
		{
			if (!_typeSymbol.IsScriptClass)
			{
				return m_containingBinder.GetBinder(stmtList);
			}
			return this;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			originalBinder.LookupMember(lookupResult, _typeSymbol, name, arity, options, ref useSiteInfo);
			if (!lookupResult.StopFurtherLookup)
			{
				LookupResult instance = LookupResult.GetInstance();
				LookupTypeParameter(instance, name, arity, options, originalBinder, ref useSiteInfo);
				lookupResult.MergePrioritized(instance);
				instance.Free();
			}
		}

		protected override void CollectProbableExtensionMethodsInSingleBinder(string name, ArrayBuilder<MethodSymbol> methods, Binder originalBinder)
		{
			_typeSymbol.AppendProbableExtensionMethods(name, methods);
		}

		protected override void AddExtensionMethodLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			_typeSymbol.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder);
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			if (_typeSymbol.Arity > 0)
			{
				ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _typeSymbol.TypeParameters.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeParameterSymbol current = enumerator.Current;
					if (originalBinder.CanAddLookupSymbolInfo(current, options, nameSet, null))
					{
						nameSet.AddSymbol(current, current.Name, 0);
					}
				}
			}
			originalBinder.AddMemberLookupSymbolsInfo(nameSet, _typeSymbol, options);
		}

		private void LookupTypeParameter(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			if (_typeSymbol.Arity <= 0)
			{
				return;
			}
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = _typeSymbol.TypeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeParameterSymbol current = enumerator.Current;
				if (CaseInsensitiveComparison.Equals(current.Name, name))
				{
					lookupResult.SetFrom(originalBinder.CheckViability(current, arity, options, null, ref useSiteInfo));
				}
			}
		}

		public override AccessCheckResult CheckAccessibility(Symbol sym, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeSymbol accessThroughType = null, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			if (!base.IgnoresAccessibility)
			{
				return AccessCheck.CheckSymbolAccessibility(sym, _typeSymbol, accessThroughType, ref useSiteInfo, basesBeingResolved);
			}
			return AccessCheckResult.Accessible;
		}
	}
}
