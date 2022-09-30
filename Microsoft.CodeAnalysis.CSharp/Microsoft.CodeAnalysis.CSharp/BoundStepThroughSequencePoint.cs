using System.Diagnostics;

using Microsoft.CodeAnalysis.Text;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundStepThroughSequencePoint : BoundStatement
    {
        public TextSpan Span { get; }

        public BoundStepThroughSequencePoint(SyntaxNode syntax, TextSpan span, bool hasErrors)
            : base(BoundKind.StepThroughSequencePoint, syntax, hasErrors)
        {
            Span = span;
        }

        public BoundStepThroughSequencePoint(SyntaxNode syntax, TextSpan span)
            : base(BoundKind.StepThroughSequencePoint, syntax)
        {
            Span = span;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitStepThroughSequencePoint(this);
        }

        public BoundStepThroughSequencePoint Update(TextSpan span)
        {
            if (span != Span)
            {
                BoundStepThroughSequencePoint boundStepThroughSequencePoint = new BoundStepThroughSequencePoint(Syntax, span, base.HasErrors);
                boundStepThroughSequencePoint.CopyAttributes(this);
                return boundStepThroughSequencePoint;
            }
            return this;
        }
    }
}
