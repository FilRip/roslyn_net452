using System.Diagnostics;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public sealed class BoundDiscardExpression : BoundExpression
    {
        public override Symbol ExpressionSymbol => new DiscardSymbol(TypeWithAnnotations.Create(Type, base.TopLevelNullability.Annotation.ToInternalAnnotation()));

        public override object Display => Type ?? ((object)"_");

        public new TypeSymbol? Type => base.Type;

        public BoundExpression SetInferredTypeWithAnnotations(TypeWithAnnotations type)
        {
            return Update(type.Type);
        }

        public BoundDiscardExpression FailInference(Binder binder, BindingDiagnosticBag? diagnosticsOpt)
        {
            if (diagnosticsOpt?.DiagnosticBag != null)
            {
                Binder.Error(diagnosticsOpt, ErrorCode.ERR_DiscardTypeInferenceFailed, Syntax);
            }
            return Update(binder.CreateErrorType("var"));
        }

        public BoundDiscardExpression(SyntaxNode syntax, TypeSymbol? type, bool hasErrors)
            : base(BoundKind.DiscardExpression, syntax, type, hasErrors)
        {
        }

        public BoundDiscardExpression(SyntaxNode syntax, TypeSymbol? type)
            : base(BoundKind.DiscardExpression, syntax, type)
        {
        }

        [DebuggerStepThrough]
        public override BoundNode? Accept(BoundTreeVisitor visitor)
        {
            return visitor.VisitDiscardExpression(this);
        }

        public BoundDiscardExpression Update(TypeSymbol? type)
        {
            if (!TypeSymbol.Equals(type, Type, TypeCompareKind.ConsiderEverything))
            {
                BoundDiscardExpression boundDiscardExpression = new BoundDiscardExpression(Syntax, type, base.HasErrors);
                boundDiscardExpression.CopyAttributes(this);
                return boundDiscardExpression;
            }
            return this;
        }
    }
}
