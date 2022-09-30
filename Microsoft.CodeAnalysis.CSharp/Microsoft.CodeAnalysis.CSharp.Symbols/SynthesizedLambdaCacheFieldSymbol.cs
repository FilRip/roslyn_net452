using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class SynthesizedLambdaCacheFieldSymbol : SynthesizedFieldSymbolBase, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        private readonly TypeWithAnnotations _type;

        private readonly MethodSymbol _topLevelMethod;

        internal override bool SuppressDynamicAttribute => true;

        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => _topLevelMethod;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => false;

        public SynthesizedLambdaCacheFieldSymbol(NamedTypeSymbol containingType, TypeSymbol type, string name, MethodSymbol topLevelMethod, bool isReadOnly, bool isStatic)
            : base(containingType, name, isPublic: true, isReadOnly, isStatic)
        {
            _type = TypeWithAnnotations.Create(type);
            _topLevelMethod = topLevelMethod;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _type;
        }
    }
}
