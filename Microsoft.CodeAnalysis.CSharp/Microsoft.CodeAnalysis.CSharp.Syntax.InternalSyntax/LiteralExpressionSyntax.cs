using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class LiteralExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken token;

        public SyntaxToken Token => token;

        public LiteralExpressionSyntax(SyntaxKind kind, SyntaxToken token, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public LiteralExpressionSyntax(SyntaxKind kind, SyntaxToken token, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 1;
            AdjustFlagsAndWidth(token);
            this.token = token;
        }

        public LiteralExpressionSyntax(SyntaxKind kind, SyntaxToken token)
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
            return new Microsoft.CodeAnalysis.CSharp.Syntax.LiteralExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLiteralExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitLiteralExpression(this);
        }

        public LiteralExpressionSyntax Update(SyntaxToken token)
        {
            if (token != Token)
            {
                LiteralExpressionSyntax literalExpressionSyntax = SyntaxFactory.LiteralExpression(base.Kind, token);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    literalExpressionSyntax = literalExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    literalExpressionSyntax = literalExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return literalExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new LiteralExpressionSyntax(base.Kind, token, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new LiteralExpressionSyntax(base.Kind, token, GetDiagnostics(), annotations);
        }

        public LiteralExpressionSyntax(ObjectReader reader)
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

        static LiteralExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(LiteralExpressionSyntax), (ObjectReader r) => new LiteralExpressionSyntax(r));
        }
    }
}
