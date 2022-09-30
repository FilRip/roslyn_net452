using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class AwaitExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken awaitKeyword;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken AwaitKeyword => awaitKeyword;

        public ExpressionSyntax Expression => expression;

        public AwaitExpressionSyntax(SyntaxKind kind, SyntaxToken awaitKeyword, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(awaitKeyword);
            this.awaitKeyword = awaitKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public AwaitExpressionSyntax(SyntaxKind kind, SyntaxToken awaitKeyword, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(awaitKeyword);
            this.awaitKeyword = awaitKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public AwaitExpressionSyntax(SyntaxKind kind, SyntaxToken awaitKeyword, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(awaitKeyword);
            this.awaitKeyword = awaitKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => awaitKeyword,
                1 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.AwaitExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAwaitExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitAwaitExpression(this);
        }

        public AwaitExpressionSyntax Update(SyntaxToken awaitKeyword, ExpressionSyntax expression)
        {
            if (awaitKeyword != AwaitKeyword || expression != Expression)
            {
                AwaitExpressionSyntax awaitExpressionSyntax = SyntaxFactory.AwaitExpression(awaitKeyword, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    awaitExpressionSyntax = awaitExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    awaitExpressionSyntax = awaitExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return awaitExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new AwaitExpressionSyntax(base.Kind, awaitKeyword, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new AwaitExpressionSyntax(base.Kind, awaitKeyword, expression, GetDiagnostics(), annotations);
        }

        public AwaitExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            awaitKeyword = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            expression = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(awaitKeyword);
            writer.WriteValue(expression);
        }

        static AwaitExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(AwaitExpressionSyntax), (ObjectReader r) => new AwaitExpressionSyntax(r));
        }
    }
}
