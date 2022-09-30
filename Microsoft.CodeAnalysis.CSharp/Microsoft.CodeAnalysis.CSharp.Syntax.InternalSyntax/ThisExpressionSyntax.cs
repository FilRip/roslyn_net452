using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ThisExpressionSyntax : InstanceExpressionSyntax
    {
        internal readonly SyntaxToken token;

        public SyntaxToken Token => token;

        public ThisExpressionSyntax(SyntaxKind kind, SyntaxToken token, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public ThisExpressionSyntax(SyntaxKind kind, SyntaxToken token, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public ThisExpressionSyntax(SyntaxKind kind, SyntaxToken token)
            : base(kind)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public override GreenNode? GetSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return token;
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ThisExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitThisExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitThisExpression(this);
        }

        public ThisExpressionSyntax Update(SyntaxToken token)
        {
            if (token != Token)
            {
                ThisExpressionSyntax thisExpressionSyntax = SyntaxFactory.ThisExpression(token);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    thisExpressionSyntax = thisExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    thisExpressionSyntax = thisExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return thisExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ThisExpressionSyntax(base.Kind, token, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ThisExpressionSyntax(base.Kind, token, GetDiagnostics(), annotations);
        }

        public ThisExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 1;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            token = node;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(token);
        }

        static ThisExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ThisExpressionSyntax), (ObjectReader r) => new ThisExpressionSyntax(r));
        }
    }
}
