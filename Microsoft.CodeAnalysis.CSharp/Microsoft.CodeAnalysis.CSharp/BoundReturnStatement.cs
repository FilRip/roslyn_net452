using System.Diagnostics;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundReturnStatement : BoundStatement
    {
        public RefKind RefKind { get; }

        public BoundExpression? ExpressionOpt { get; }

        public static BoundReturnStatement Synthesized(SyntaxNode syntax, RefKind refKind, BoundExpression expression, bool hasErrors = false)
        {
            return new BoundReturnStatement(syntax, refKind, expression, hasErrors)
            {
                WasCompilerGenerated = true
            };
        }

        public BoundReturnStatement(SyntaxNode syntax, RefKind refKind, BoundExpression? expressionOpt, bool hasErrors = false)
            : base(BoundKind.ReturnStatement, syntax, hasErrors || expressionOpt.HasErrors())
        {
            RefKind = refKind;
            ExpressionOpt = expressionOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitReturnStatement(this);
        }

        public BoundReturnStatement Update(RefKind refKind, BoundExpression? expressionOpt)
        {
            if (refKind != RefKind || expressionOpt != ExpressionOpt)
            {
                BoundReturnStatement boundReturnStatement = new BoundReturnStatement(Syntax, refKind, expressionOpt, base.HasErrors);
                boundReturnStatement.CopyAttributes(this);
                return boundReturnStatement;
            }
            return this;
        }
    }
}
