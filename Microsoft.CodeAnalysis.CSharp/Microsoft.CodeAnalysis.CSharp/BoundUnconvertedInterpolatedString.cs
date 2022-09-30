using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundUnconvertedInterpolatedString : BoundInterpolatedStringBase
    {
        public BoundUnconvertedInterpolatedString(SyntaxNode syntax, ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.UnconvertedInterpolatedString, syntax, parts, constantValueOpt, type, hasErrors || parts.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitUnconvertedInterpolatedString(this);
        }

        public BoundUnconvertedInterpolatedString Update(ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type)
        {
            if (parts != base.Parts || constantValueOpt != base.ConstantValueOpt || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundUnconvertedInterpolatedString boundUnconvertedInterpolatedString = new BoundUnconvertedInterpolatedString(Syntax, parts, constantValueOpt, type, base.HasErrors);
                boundUnconvertedInterpolatedString.CopyAttributes(this);
                return boundUnconvertedInterpolatedString;
            }
            return this;
        }
    }
}
