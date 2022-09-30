using System.Diagnostics;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundSavePreviousSequencePoint : BoundStatement
    {
        public object Identifier { get; }

        public BoundSavePreviousSequencePoint(SyntaxNode syntax, object identifier, bool hasErrors)
            : base(BoundKind.SavePreviousSequencePoint, syntax, hasErrors)
        {
            Identifier = identifier;
        }

        public BoundSavePreviousSequencePoint(SyntaxNode syntax, object identifier)
            : base(BoundKind.SavePreviousSequencePoint, syntax)
        {
            Identifier = identifier;
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitSavePreviousSequencePoint(this);
        }

        public BoundSavePreviousSequencePoint Update(object identifier)
        {
            if (identifier != Identifier)
            {
                BoundSavePreviousSequencePoint boundSavePreviousSequencePoint = new BoundSavePreviousSequencePoint(Syntax, identifier, base.HasErrors);
                boundSavePreviousSequencePoint.CopyAttributes(this);
                return boundSavePreviousSequencePoint;
            }
            return this;
        }
    }
}
