using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class ThrowExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken throwKeyword;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken ThrowKeyword => throwKeyword;

        public ExpressionSyntax Expression => expression;

        public ThrowExpressionSyntax(SyntaxKind kind, SyntaxToken throwKeyword, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(throwKeyword);
            this.throwKeyword = throwKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public ThrowExpressionSyntax(SyntaxKind kind, SyntaxToken throwKeyword, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(throwKeyword);
            this.throwKeyword = throwKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public ThrowExpressionSyntax(SyntaxKind kind, SyntaxToken throwKeyword, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(throwKeyword);
            this.throwKeyword = throwKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => throwKeyword,
                1 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.ThrowExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitThrowExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitThrowExpression(this);
        }

        public ThrowExpressionSyntax Update(SyntaxToken throwKeyword, ExpressionSyntax expression)
        {
            if (throwKeyword != ThrowKeyword || expression != Expression)
            {
                ThrowExpressionSyntax throwExpressionSyntax = SyntaxFactory.ThrowExpression(throwKeyword, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    throwExpressionSyntax = throwExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    throwExpressionSyntax = throwExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return throwExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new ThrowExpressionSyntax(base.Kind, throwKeyword, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new ThrowExpressionSyntax(base.Kind, throwKeyword, expression, GetDiagnostics(), annotations);
        }

        public ThrowExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            throwKeyword = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            expression = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(throwKeyword);
            writer.WriteValue(expression);
        }

        static ThrowExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(ThrowExpressionSyntax), (ObjectReader r) => new ThrowExpressionSyntax(r));
        }
    }
}
