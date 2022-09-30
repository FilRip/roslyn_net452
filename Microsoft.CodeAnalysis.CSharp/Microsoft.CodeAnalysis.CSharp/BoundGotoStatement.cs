using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundGotoStatement : BoundStatement
    {
        public LabelSymbol Label { get; }

        public BoundExpression? CaseExpressionOpt { get; }

        public BoundLabel? LabelExpressionOpt { get; }

        public BoundGotoStatement(SyntaxNode syntax, LabelSymbol label, bool hasErrors = false)
            : this(syntax, label, null, null, hasErrors)
        {
        }

        public BoundGotoStatement(SyntaxNode syntax, LabelSymbol label, BoundExpression? caseExpressionOpt, BoundLabel? labelExpressionOpt, bool hasErrors = false)
            : base(BoundKind.GotoStatement, syntax, hasErrors || caseExpressionOpt.HasErrors() || labelExpressionOpt.HasErrors())
        {
            Label = label;
            CaseExpressionOpt = caseExpressionOpt;
            LabelExpressionOpt = labelExpressionOpt;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitGotoStatement(this);
        }

        public BoundGotoStatement Update(LabelSymbol label, BoundExpression? caseExpressionOpt, BoundLabel? labelExpressionOpt)
        {
            if (!Microsoft.CodeAnalysis.CSharp.Symbols.SymbolEqualityComparer.ConsiderEverything.Equals(label, Label) || caseExpressionOpt != CaseExpressionOpt || labelExpressionOpt != LabelExpressionOpt)
            {
                BoundGotoStatement boundGotoStatement = new BoundGotoStatement(Syntax, label, caseExpressionOpt, labelExpressionOpt, base.HasErrors);
                boundGotoStatement.CopyAttributes(this);
                return boundGotoStatement;
            }
            return this;
        }
    }
}
