using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class SelectClauseSyntax : SelectOrGroupClauseSyntax
    {
        internal readonly SyntaxToken selectKeyword;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken SelectKeyword => selectKeyword;

        public ExpressionSyntax Expression => expression;

        public SelectClauseSyntax(SyntaxKind kind, SyntaxToken selectKeyword, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(selectKeyword);
            this.selectKeyword = selectKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public SelectClauseSyntax(SyntaxKind kind, SyntaxToken selectKeyword, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 2;
            AdjustFlagsAndWidth(selectKeyword);
            this.selectKeyword = selectKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public SelectClauseSyntax(SyntaxKind kind, SyntaxToken selectKeyword, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 2;
            AdjustFlagsAndWidth(selectKeyword);
            this.selectKeyword = selectKeyword;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => selectKeyword,
                1 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.SelectClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSelectClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitSelectClause(this);
        }

        public SelectClauseSyntax Update(SyntaxToken selectKeyword, ExpressionSyntax expression)
        {
            if (selectKeyword != SelectKeyword || expression != Expression)
            {
                SelectClauseSyntax selectClauseSyntax = SyntaxFactory.SelectClause(selectKeyword, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    selectClauseSyntax = selectClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    selectClauseSyntax = selectClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return selectClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new SelectClauseSyntax(base.Kind, selectKeyword, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new SelectClauseSyntax(base.Kind, selectKeyword, expression, GetDiagnostics(), annotations);
        }

        public SelectClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 2;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            selectKeyword = node;
            ExpressionSyntax node2 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            expression = node2;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(selectKeyword);
            writer.WriteValue(expression);
        }

        static SelectClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(SelectClauseSyntax), (ObjectReader r) => new SelectClauseSyntax(r));
        }
    }
}
