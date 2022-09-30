using System;
using System.Collections.Immutable;

using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedEmbeddedAttributeConstructorWithBodySymbol : SynthesizedInstanceConstructor
    {
        private readonly ImmutableArray<ParameterSymbol> _parameters;

        private readonly Action<SyntheticBoundNodeFactory, ArrayBuilder<BoundStatement>, ImmutableArray<ParameterSymbol>> _getConstructorBody;

        public override ImmutableArray<ParameterSymbol> Parameters => _parameters;

        internal SynthesizedEmbeddedAttributeConstructorWithBodySymbol(NamedTypeSymbol containingType, Func<MethodSymbol, ImmutableArray<ParameterSymbol>> getParameters, Action<SyntheticBoundNodeFactory, ArrayBuilder<BoundStatement>, ImmutableArray<ParameterSymbol>> getConstructorBody)
            : base(containingType)
        {
            _parameters = getParameters(this);
            _getConstructorBody = getConstructorBody;
        }

        internal override void GenerateMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics)
        {
            GenerateMethodBodyCore(compilationState, diagnostics);
        }

        internal override void GenerateMethodBodyStatements(SyntheticBoundNodeFactory factory, ArrayBuilder<BoundStatement> statements, BindingDiagnosticBag diagnostics)
        {
            _getConstructorBody(factory, statements, _parameters);
        }
    }
}
