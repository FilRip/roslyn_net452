using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public readonly struct InferredLambdaReturnType
    {
        internal readonly int NumExpressions;

        internal readonly bool HadExpressionlessReturn;

        internal readonly RefKind RefKind;

        internal readonly TypeWithAnnotations TypeWithAnnotations;

        internal readonly ImmutableArray<DiagnosticInfo> UseSiteDiagnostics;

        internal readonly ImmutableArray<AssemblySymbol> Dependencies;

        public InferredLambdaReturnType(int numExpressions, bool hadExpressionlessReturn, RefKind refKind, TypeWithAnnotations typeWithAnnotations, ImmutableArray<DiagnosticInfo> useSiteDiagnostics, ImmutableArray<AssemblySymbol> dependencies)
        {
            NumExpressions = numExpressions;
            HadExpressionlessReturn = hadExpressionlessReturn;
            RefKind = refKind;
            TypeWithAnnotations = typeWithAnnotations;
            UseSiteDiagnostics = useSiteDiagnostics;
            Dependencies = dependencies;
        }
    }
}
