using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundAwaitExpression : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundExpression Expression { get; }

        public BoundAwaitableInfo AwaitableInfo { get; }

        public BoundAwaitExpression(SyntaxNode syntax, BoundExpression expression, BoundAwaitableInfo awaitableInfo, TypeSymbol type, bool hasErrors = false)
            : base(BoundKind.AwaitExpression, syntax, type, hasErrors || expression.HasErrors() || awaitableInfo.HasErrors())
        {
            Expression = expression;
            AwaitableInfo = awaitableInfo;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitAwaitExpression(this);
        }

        public BoundAwaitExpression Update(BoundExpression expression, BoundAwaitableInfo awaitableInfo, TypeSymbol type)
        {
            if (expression != Expression || awaitableInfo != AwaitableInfo || !TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundAwaitExpression boundAwaitExpression = new BoundAwaitExpression(Syntax, expression, awaitableInfo, type, base.HasErrors);
                boundAwaitExpression.CopyAttributes(this);
                return boundAwaitExpression;
            }
            return this;
        }
    }
}
