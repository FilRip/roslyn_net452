using System.Diagnostics;

using Microsoft.CodeAnalysis.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSequencePointWithSpan : BoundStatement
    {
        public BoundStatement? StatementOpt { get; }

        public TextSpan Span { get; }

        public BoundSequencePointWithSpan(SyntaxNode syntax, BoundStatement? statementOpt, TextSpan span, bool hasErrors = false)
            : base(BoundKind.SequencePointWithSpan, syntax, hasErrors || statementOpt.HasErrors())
        {
            StatementOpt = statementOpt;
            Span = span;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSequencePointWithSpan(this);
        }

        public BoundSequencePointWithSpan Update(BoundStatement? statementOpt, TextSpan span)
        {
            if (statementOpt != StatementOpt || span != Span)
            {
                BoundSequencePointWithSpan boundSequencePointWithSpan = new BoundSequencePointWithSpan(Syntax, statementOpt, span, base.HasErrors);
                boundSequencePointWithSpan.CopyAttributes(this);
                return boundSequencePointWithSpan;
            }
            return this;
        }
    }
}
