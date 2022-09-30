using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundThrowStatement : BoundStatement
    {
        public BoundExpression? ExpressionOpt { get; }

        public BoundThrowStatement(SyntaxNode syntax, BoundExpression? expressionOpt, bool hasErrors = false)
            : base(BoundKind.ThrowStatement, syntax, hasErrors || expressionOpt.HasErrors())
        {
            ExpressionOpt = expressionOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitThrowStatement(this);
        }

        public BoundThrowStatement Update(BoundExpression? expressionOpt)
        {
            if (expressionOpt != ExpressionOpt)
            {
                BoundThrowStatement boundThrowStatement = new BoundThrowStatement(Syntax, expressionOpt, base.HasErrors);
                boundThrowStatement.CopyAttributes(this);
                return boundThrowStatement;
            }
            return this;
        }
    }
}
