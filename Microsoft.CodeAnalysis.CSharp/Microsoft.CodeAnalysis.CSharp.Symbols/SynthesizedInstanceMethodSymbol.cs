using System.Collections.Immutable;
using System.Threading;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class SynthesizedInstanceMethodSymbol : MethodSymbol
    {
        private ParameterSymbol _lazyThisParameter;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public sealed override bool IsImplicitlyDeclared => true;

        public sealed override bool AreLocalsZeroed => ContainingType.AreLocalsZeroed;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal override bool IsDeclaredReadOnly => false;

        internal override bool IsInitOnly => false;

        public sealed override FlowAnalysisAnnotations FlowAnalysisAnnotations => FlowAnalysisAnnotations.None;

        internal override bool TryGetThisParameter(out ParameterSymbol thisParameter)
        {
            if ((object)_lazyThisParameter == null)
            {
                Interlocked.CompareExchange(ref _lazyThisParameter, new ThisParameterSymbol(this), null);
            }
            thisParameter = _lazyThisParameter;
            return true;
        }

        internal sealed override UnmanagedCallersOnlyAttributeData GetUnmanagedCallersOnlyAttributeData(bool forceComplete)
        {
            return null;
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            return false;
        }
    }
}
