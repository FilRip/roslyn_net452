using System.Collections.Immutable;
using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class NamespaceBinder : Binder
	{
		private readonly NamespaceSymbol _nsSymbol;

		public override NamespaceOrTypeSymbol ContainingNamespaceOrType => _nsSymbol;

		public NamespaceSymbol NamespaceSymbol => _nsSymbol;

		public override NamedTypeSymbol ContainingType => null;

		public override Symbol ContainingMember => _nsSymbol;

		public override ImmutableArray<Symbol> AdditionalContainingMembers => ImmutableArray<Symbol>.Empty;

		public override bool IsInQuery => false;

		public NamespaceBinder(Binder containingBinder, NamespaceSymbol nsSymbol)
			: base(containingBinder)
		{
			_nsSymbol = nsSymbol;
		}

		internal override void LookupInSingleBinder(LookupResult lookupResult, string name, int arity, LookupOptions options, Binder originalBinder, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			originalBinder.LookupMember(lookupResult, _nsSymbol, name, arity, options | LookupOptions.IgnoreExtensionMethods, ref useSiteInfo);
		}

		protected override void CollectProbableExtensionMethodsInSingleBinder(string name, ArrayBuilder<MethodSymbol> methods, Binder originalBinder)
		{
			_nsSymbol.AppendProbableExtensionMethods(name, methods);
		}

		protected override void AddExtensionMethodLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			_nsSymbol.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder);
		}

		internal override void AddLookupSymbolsInfoInSingleBinder(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			originalBinder.AddMemberLookupSymbolsInfo(nameSet, _nsSymbol, options | LookupOptions.IgnoreExtensionMethods);
		}
	}
}
