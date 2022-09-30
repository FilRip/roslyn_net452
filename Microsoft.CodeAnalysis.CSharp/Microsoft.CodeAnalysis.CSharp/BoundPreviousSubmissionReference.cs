using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundPreviousSubmissionReference : BoundExpression
    {
        public new TypeSymbol Type => base.Type;

        public BoundPreviousSubmissionReference(SyntaxNode syntax, TypeSymbol type, bool hasErrors)
            : base(BoundKind.PreviousSubmissionReference, syntax, type, hasErrors)
        {
        }

        public BoundPreviousSubmissionReference(SyntaxNode syntax, TypeSymbol type)
            : base(BoundKind.PreviousSubmissionReference, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitPreviousSubmissionReference(this);
        }

        public BoundPreviousSubmissionReference Update(TypeSymbol type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundPreviousSubmissionReference boundPreviousSubmissionReference = new BoundPreviousSubmissionReference(Syntax, type, base.HasErrors);
                boundPreviousSubmissionReference.CopyAttributes(this);
                return boundPreviousSubmissionReference;
            }
            return this;
        }
    }
}
