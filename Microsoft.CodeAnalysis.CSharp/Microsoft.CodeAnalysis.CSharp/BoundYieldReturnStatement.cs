using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundYieldReturnStatement : BoundStatement
    {
        public BoundExpression Expression { get; }

        public BoundYieldReturnStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors = false)
            : base(BoundKind.YieldReturnStatement, syntax, hasErrors || expression.HasErrors())
        {
            Expression = expression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitYieldReturnStatement(this);
        }

        public BoundYieldReturnStatement Update(BoundExpression expression)
        {
            if (expression != Expression)
            {
                BoundYieldReturnStatement boundYieldReturnStatement = new BoundYieldReturnStatement(Syntax, expression, base.HasErrors);
                boundYieldReturnStatement.CopyAttributes(this);
                return boundYieldReturnStatement;
            }
            return this;
        }
    }
}
