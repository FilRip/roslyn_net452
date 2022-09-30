using System.Diagnostics;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundIfStatement : BoundStatement
    {
        public BoundExpression Condition { get; }

        public BoundStatement Consequence { get; }

        public BoundStatement? AlternativeOpt { get; }

        public BoundIfStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement consequence, BoundStatement? alternativeOpt, bool hasErrors = false)
            : base(BoundKind.IfStatement, syntax, hasErrors || condition.HasErrors() || consequence.HasErrors() || alternativeOpt.HasErrors())
        {
            Condition = condition;
            Consequence = consequence;
            AlternativeOpt = alternativeOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitIfStatement(this);
        }

        public BoundIfStatement Update(BoundExpression condition, BoundStatement consequence, BoundStatement? alternativeOpt)
        {
            if (condition != Condition || consequence != Consequence || alternativeOpt != AlternativeOpt)
            {
                BoundIfStatement boundIfStatement = new BoundIfStatement(Syntax, condition, consequence, alternativeOpt, base.HasErrors);
                boundIfStatement.CopyAttributes(this);
                return boundIfStatement;
            }
            return this;
        }
    }
}
