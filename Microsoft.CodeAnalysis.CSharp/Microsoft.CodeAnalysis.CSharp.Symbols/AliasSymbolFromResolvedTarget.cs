using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class AliasSymbolFromResolvedTarget : AliasSymbol
    {
        private readonly NamespaceOrTypeSymbol _aliasTarget;

        public override NamespaceOrTypeSymbol Target => _aliasTarget;

        internal override bool RequiresCompletion => false;

        internal AliasSymbolFromResolvedTarget(NamespaceOrTypeSymbol target, string aliasName, Symbol? containingSymbol, ImmutableArray<Location> locations, bool isExtern)
            : base(aliasName, containingSymbol, locations, isExtern)
        {
            _aliasTarget = target;
        }

        internal override NamespaceOrTypeSymbol GetAliasTarget(ConsList<TypeSymbol>? basesBeingResolved)
        {
            return _aliasTarget;
        }
    }
}
