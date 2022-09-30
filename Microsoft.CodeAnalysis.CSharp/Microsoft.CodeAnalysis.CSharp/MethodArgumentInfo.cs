using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed record MethodArgumentInfo(MethodSymbol Method, ImmutableArray<BoundExpression> Arguments, ImmutableArray<int> ArgsToParamsOpt, BitVector DefaultArguments, bool Expanded)
    {
        public static MethodArgumentInfo CreateParameterlessMethod(MethodSymbol method)
        {
            return new MethodArgumentInfo(method, ImmutableArray<BoundExpression>.Empty, default(ImmutableArray<int>), default(BitVector), Expanded: false);
        }
    }
}
