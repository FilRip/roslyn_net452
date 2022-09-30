using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundExpressionStatement : BoundStatement
    {
        public BoundExpression Expression { get; }

        public BoundExpressionStatement(SyntaxNode syntax, BoundExpression expression, bool hasErrors = false)
            : base(BoundKind.ExpressionStatement, syntax, hasErrors || expression.HasErrors())
        {
            Expression = expression;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitExpressionStatement(this);
        }

        public BoundExpressionStatement Update(BoundExpression expression)
        {
            if (expression != Expression)
            {
                BoundExpressionStatement boundExpressionStatement = new BoundExpressionStatement(Syntax, expression, base.HasErrors);
                boundExpressionStatement.CopyAttributes(this);
                return boundExpressionStatement;
            }
            return this;
        }
    }
}
