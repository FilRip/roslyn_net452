using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class CheckedExpressionSyntax : ExpressionSyntax
    {
        internal readonly SyntaxToken keyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly ExpressionSyntax expression;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken Keyword => keyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public ExpressionSyntax Expression => expression;

        public SyntaxToken CloseParenToken => closeParenToken;

        public CheckedExpressionSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CheckedExpressionSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CheckedExpressionSyntax(SyntaxKind kind, SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(keyword);
            this.keyword = keyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => keyword,
                1 => openParenToken,
                2 => expression,
                3 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CheckedExpressionSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCheckedExpression(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCheckedExpression(this);
        }

        public CheckedExpressionSyntax Update(SyntaxToken keyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken)
        {
            if (keyword != Keyword || openParenToken != OpenParenToken || expression != Expression || closeParenToken != CloseParenToken)
            {
                CheckedExpressionSyntax checkedExpressionSyntax = SyntaxFactory.CheckedExpression(base.Kind, keyword, openParenToken, expression, closeParenToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    checkedExpressionSyntax = checkedExpressionSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    checkedExpressionSyntax = checkedExpressionSyntax.WithAnnotationsGreen(annotations);
                }
                return checkedExpressionSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CheckedExpressionSyntax(base.Kind, keyword, openParenToken, expression, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CheckedExpressionSyntax(base.Kind, keyword, openParenToken, expression, closeParenToken, GetDiagnostics(), annotations);
        }

        public CheckedExpressionSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            keyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            expression = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeParenToken = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(keyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(expression);
            writer.WriteValue(closeParenToken);
        }

        static CheckedExpressionSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CheckedExpressionSyntax), (ObjectReader r) => new CheckedExpressionSyntax(r));
        }
    }
}