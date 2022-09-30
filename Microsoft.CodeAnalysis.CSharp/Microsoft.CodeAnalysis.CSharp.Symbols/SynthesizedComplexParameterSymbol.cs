using System.Collections.Immutable;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedComplexParameterSymbol : SynthesizedParameterSymbolBase
    {
        private readonly ImmutableArray<CustomModifier> _refCustomModifiers;

        private readonly SourceComplexParameterSymbol? _baseParameterForAttributes;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => _refCustomModifiers;

        public bool HasEnumeratorCancellationAttribute => _baseParameterForAttributes?.HasEnumeratorCancellationAttribute ?? false;

        internal override MarshalPseudoCustomAttributeData? MarshallingInformation => _baseParameterForAttributes?.MarshallingInformation;

        public SynthesizedComplexParameterSymbol(MethodSymbol? container, TypeWithAnnotations type, int ordinal, RefKind refKind, string name, ImmutableArray<CustomModifier> refCustomModifiers, SourceComplexParameterSymbol? baseParameterForAttributes)
            : base(container, type, ordinal, refKind, name)
        {
            _refCustomModifiers = refCustomModifiers;
            _baseParameterForAttributes = baseParameterForAttributes;
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return _baseParameterForAttributes?.GetAttributes() ?? ImmutableArray<CSharpAttributeData>.Empty;
        }
    }
}
