using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class RefExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken refKeyword;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken RefKeyword => refKeyword;

        public ExpressionSyntax Expression => expression;

        public RefExpressionSyntax(SyntaxKind kind, SyntaxToken refKeyword, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(refKeyword);
            this.refKeyword = refKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public RefExpressionSyntax(SyntaxKind kind, SyntaxToken refKeyword, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(refKeyword);
            this.refKeyword = refKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public RefExpressionSyntax(SyntaxKind kind, SyntaxToken refKeyword, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(refKeyword);
            this.refKeyword = refKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => refKeyword,
                1 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.RefExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRefExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitRefExpression(this);
        }

        public RefExpressionSyntax Update(SyntaxToken refKeyword, ExpressionSyntax expression)
        {
            if (refKeyword != RefKeyword || expression != Expression)
            {
                RefExpressionSyntax refExpressionSyntax = SyntaxFactory.RefExpression(refKeyword, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    refExpressionSyntax = refExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    refExpressionSyntax = refExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return refExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new RefExpressionSyntax(base.Kind, refKeyword, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new RefExpressionSyntax(base.Kind, refKeyword, expression, GetDiagnostics(), annotations);
        }

        public RefExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            refKeyword = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            expression = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(refKeyword);
            writer.WriteValue(expression);
        }

        static RefExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(RefExpressionSyntax), (ObjectReader r) => new RefExpressionSyntax(r));
        }
    }
}
