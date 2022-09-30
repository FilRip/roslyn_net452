using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MissingNamespaceSymbol : NamespaceSymbol
	{
		private readonly string _name;

		private readonly Symbol _containingSymbol;

		public override string Name => _name;

		public override Symbol ContainingSymbol => _containingSymbol;

		public override AssemblySymbol ContainingAssembly => _containingSymbol.ContainingAssembly;

		internal override NamespaceExtent Extent
		{
			get
			{
				if (_containingSymbol.Kind == SymbolKind.NetModule)
				{
					return new NamespaceExtent((ModuleSymbol)_containingSymbol);
				}
				return ((NamespaceSymbol)_containingSymbol).Extent;
			}
		}

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		internal override Accessibility DeclaredAccessibilityOfMostAccessibleDescendantType => Accessibility.Private;

		internal override ImmutableArray<NamedTypeSymbol> TypesToCheckForExtensionMethods => ImmutableArray<NamedTypeSymbol>.Empty;

		public MissingNamespaceSymbol(MissingModuleSymbol containingModule)
		{
			_containingSymbol = containingModule;
			_name = string.Empty;
		}

		public MissingNamespaceSymbol(NamespaceSymbol containingNamespace, string name)
		{
			_containingSymbol = containingNamespace;
			_name = name;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_containingSymbol.GetHashCode(), _name.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			return obj is MissingNamespaceSymbol missingNamespaceSymbol && string.Equals(_name, missingNamespaceSymbol._name, StringComparison.Ordinal) && _containingSymbol.Equals(missingNamespaceSymbol._containingSymbol);
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetModuleMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		internal override void AppendProbableExtensionMethods(string name, ArrayBuilder<MethodSymbol> methods)
		{
		}

		internal override void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamespaceSymbol appendThrough)
		{
		}
	}
}
