using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class DynamicSiteContainer : SynthesizedContainer, ISynthesizedMethodBodyImplementationSymbol, ISymbolInternal
    {
        private readonly MethodSymbol _topLevelMethod;

        public override Symbol ContainingSymbol => _topLevelMethod.ContainingSymbol;

        public override TypeKind TypeKind => TypeKind.Class;

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        bool ISynthesizedMethodBodyImplementationSymbol.HasMethodBodyDependency => true;

        IMethodSymbolInternal ISynthesizedMethodBodyImplementationSymbol.Method => _topLevelMethod;

        internal DynamicSiteContainer(string name, MethodSymbol topLevelMethod, MethodSymbol containingMethod)
            : base(name, containingMethod)
        {
            _topLevelMethod = topLevelMethod;
        }

        internal override bool HasPossibleWellKnownCloneMethod()
        {
            return false;
        }
    }
}
