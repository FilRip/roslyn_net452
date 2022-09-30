using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundInterpolatedString : BoundInterpolatedStringBase
    {
        public BoundInterpolatedString(SyntaxNode syntax, ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type, bool hasErrors = false)
            : base(BoundKind.InterpolatedString, syntax, parts, constantValueOpt, type, hasErrors || parts.HasErrors())
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitInterpolatedString(this);
        }

        public BoundInterpolatedString Update(ImmutableArray<BoundExpression> parts, ConstantValue? constantValueOpt, TypeSymbol? type)
        {
            if (parts != base.Parts || constantValueOpt != base.ConstantValueOpt || !TypeSymbol.Equals(type, base.Type, TypeCompareKind.ConsiderEverything))
            {
                BoundInterpolatedString boundInterpolatedString = new BoundInterpolatedString(Syntax, parts, constantValueOpt, type, base.HasErrors);
                boundInterpolatedString.CopyAttributes(this);
                return boundInterpolatedString;
            }
            return this;
        }
    }
}
