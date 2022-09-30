using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class OutDeconstructVarPendingInference : BoundExpression
    {
        public BoundDeconstructValuePlaceholder? Placeholder;

        public override object Display => string.Empty;

        public new TypeSymbol? Type => base.Type;

        public BoundDeconstructValuePlaceholder SetInferredTypeWithAnnotations(TypeWithAnnotations type, Binder binder, bool success)
        {
            Placeholder = new BoundDeconstructValuePlaceholder(Syntax, binder.LocalScopeDepth, type.Type, base.HasErrors || !success);
            return Placeholder;
        }

        public BoundDeconstructValuePlaceholder FailInference(Binder binder)
        {
            return SetInferredTypeWithAnnotations(TypeWithAnnotations.Create(binder.CreateErrorType()), binder, success: false);
        }

        public OutDeconstructVarPendingInference(SyntaxNode syntax, bool hasErrors)
            : base(BoundKind.OutDeconstructVarPendingInference, syntax, null, hasErrors)
        {
        }

        public OutDeconstructVarPendingInference(SyntaxNode syntax)
            : base(BoundKind.OutDeconstructVarPendingInference, syntax, null)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitOutDeconstructVarPendingInference(this);
        }

        public OutDeconstructVarPendingInference Update()
        {
            return this;
        }
    }
}
