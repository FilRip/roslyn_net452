using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class LambdaParameterSymbol : SourceComplexParameterSymbol
    {
        private readonly SyntaxList<AttributeListSyntax> _attributeLists;

        public override bool IsDiscard { get; }

        internal override bool IsMetadataOptional => false;

        public override bool IsParams => false;

        internal override bool HasDefaultArgumentSyntax => false;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        internal override bool IsExtensionMethodThis => false;

        public LambdaParameterSymbol(LambdaSymbol owner, SyntaxList<AttributeListSyntax> attributeLists, TypeWithAnnotations parameterType, int ordinal, RefKind refKind, string name, bool isDiscard, ImmutableArray<Location> locations)
            : base(owner, ordinal, parameterType, refKind, name, locations, null, isParams: false, isExtensionMethodThis: false)
        {
            _attributeLists = attributeLists;
            IsDiscard = isDiscard;
        }

        internal override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
        {
            return OneOrMany.Create(_attributeLists);
        }
    }
}
