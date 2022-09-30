using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class ModuleSymbol : Symbol, IModuleSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.ModuleSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        INamespaceSymbol IModuleSymbol.GlobalNamespace => _underlying.GlobalNamespace.GetPublicSymbol();

        ImmutableArray<IAssemblySymbol> IModuleSymbol.ReferencedAssemblySymbols => _underlying.ReferencedAssemblySymbols.GetPublicSymbols();

        ImmutableArray<AssemblyIdentity> IModuleSymbol.ReferencedAssemblies => _underlying.ReferencedAssemblies;

        public ModuleSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.ModuleSymbol underlying)
        {
            _underlying = underlying;
        }

        INamespaceSymbol IModuleSymbol.GetModuleNamespace(INamespaceSymbol namespaceSymbol)
        {
            return _underlying.GetModuleNamespace(namespaceSymbol).GetPublicSymbol();
        }

        ModuleMetadata IModuleSymbol.GetMetadata()
        {
            return _underlying.GetMetadata();
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitModule(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitModule(this);
        }
    }
}
