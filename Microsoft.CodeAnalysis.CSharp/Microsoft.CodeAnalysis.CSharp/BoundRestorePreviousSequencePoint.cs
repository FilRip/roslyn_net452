using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundRestorePreviousSequencePoint : BoundStatement
    {
        public object Identifier { get; }

        public BoundRestorePreviousSequencePoint(SyntaxNode syntax, object identifier, bool hasErrors)
            : base(BoundKind.RestorePreviousSequencePoint, syntax, hasErrors)
        {
            Identifier = identifier;
        }

        public BoundRestorePreviousSequencePoint(SyntaxNode syntax, object identifier)
            : base(BoundKind.RestorePreviousSequencePoint, syntax)
        {
            Identifier = identifier;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitRestorePreviousSequencePoint(this);
        }

        public BoundRestorePreviousSequencePoint Update(object identifier)
        {
            if (identifier != Identifier)
            {
                BoundRestorePreviousSequencePoint boundRestorePreviousSequencePoint = new BoundRestorePreviousSequencePoint(Syntax, identifier, base.HasErrors);
                boundRestorePreviousSequencePoint.CopyAttributes(this);
                return boundRestorePreviousSequencePoint;
            }
            return this;
        }
    }
}
