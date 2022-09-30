using System.Collections.Immutable;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class BoundInterpolatedStringBase : BoundExpression
    {
        public sealed override ConstantValue? ConstantValue => ConstantValueOpt;

        public ImmutableArray<BoundExpression> Parts { get; }

        public ConstantValue? ConstantValueOpt { get; }

        protected BoundInterpolatedStringBase(BoundKind kind, SyntaxNode syntax, ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors = false)
            : base(kind, syntax, type, hasErrors)
        {
            Parts = parts;
            ConstantValueOpt = constantValueOpt;
        }
    }
}
