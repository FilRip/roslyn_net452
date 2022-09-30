using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedNamespaceSymbol : NamespaceSymbol
    {
        private readonly string _name;

        private readonly NamespaceSymbol _containingSymbol;

        internal override NamespaceExtent Extent => _containingSymbol.Extent;

        public override string Name => _name;

        public override Symbol ContainingSymbol => _containingSymbol;

        public override AssemblySymbol ContainingAssembly => _containingSymbol.ContainingAssembly;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public SynthesizedNamespaceSymbol(NamespaceSymbol containingNamespace, string name)
        {
            _containingSymbol = containingNamespace;
            _name = name;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(_containingSymbol.GetHashCode(), _name.GetHashCode());
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            if (obj is SynthesizedNamespaceSymbol other)
            {
                return Equals(other, compareKind);
            }
            return false;
        }

        public bool Equals(SynthesizedNamespaceSymbol other, TypeCompareKind compareKind)
        {
            if ((object)this == other)
            {
                return true;
            }
            if ((object)other != null && _name.Equals(other._name))
            {
                return _containingSymbol.Equals(other._containingSymbol);
            }
            return false;
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

        public override ImmutableArray<Symbol> GetMembers()
        {
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return ImmutableArray<Symbol>.Empty;
        }
    }
}
