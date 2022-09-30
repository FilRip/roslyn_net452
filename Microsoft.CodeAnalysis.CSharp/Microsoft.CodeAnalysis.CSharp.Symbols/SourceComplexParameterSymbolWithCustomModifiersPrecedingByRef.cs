using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SourceComplexParameterSymbolWithCustomModifiersPrecedingByRef : SourceComplexParameterSymbol
    {
        private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

        internal SourceComplexParameterSymbolWithCustomModifiersPrecedingByRef(Symbol owner, int ordinal, TypeWithAnnotations parameterType, RefKind refKind, ImmutableArray<CustomModifier> refCustomModifiers, string name, ImmutableArray<Location> locations, SyntaxReference syntaxRef, bool isParams, bool isExtensionMethodThis)
            : base(owner, ordinal, parameterType, refKind, name, locations, syntaxRef, isParams, isExtensionMethodThis)
        {
            _refCustomModifiers = refCustomModifiers;
        }
    }
}
