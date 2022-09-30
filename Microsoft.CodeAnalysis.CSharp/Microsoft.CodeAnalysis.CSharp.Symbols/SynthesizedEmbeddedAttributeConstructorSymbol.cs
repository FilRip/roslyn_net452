using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEmbeddedAttributeConstructorSymbol : SynthesizedInstanceConstructor
    {
        private readonly ImmutableArray<ParameterSymbol> _parameters;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        internal SynthesizedEmbeddedAttributeConstructorSymbol(NamedTypeSymbol containingType, Func<MethodSymbol, ImmutableArray<ParameterSymbol>> getParameters)
            : base(containingType)
        {
            _parameters = getParameters(this);
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            GenerateMethodBodyCore(compilationState, diagnostics);
        }
    }
}
