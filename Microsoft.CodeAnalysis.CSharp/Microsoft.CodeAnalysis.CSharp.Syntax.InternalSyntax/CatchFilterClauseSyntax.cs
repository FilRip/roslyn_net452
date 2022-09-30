using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class CatchFilterClauseSyntax : CSharpSyntaxNode
    {
        internal readonly SyntaxToken whenKeyword;

        internal readonly SyntaxToken openParenToken;

        internal readonly ExpressionSyntax filterExpression;

        internal readonly SyntaxToken closeParenToken;

        public SyntaxToken WhenKeyword => whenKeyword;

        public SyntaxToken OpenParenToken => openParenToken;

        public ExpressionSyntax FilterExpression => filterExpression;

        public SyntaxToken CloseParenToken => closeParenToken;

        public CatchFilterClauseSyntax(SyntaxKind kind, SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(whenKeyword);
            this.whenKeyword = whenKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(filterExpression);
            this.filterExpression = filterExpression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CatchFilterClauseSyntax(SyntaxKind kind, SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(whenKeyword);
            this.whenKeyword = whenKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(filterExpression);
            this.filterExpression = filterExpression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public CatchFilterClauseSyntax(SyntaxKind kind, SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(whenKeyword);
            this.whenKeyword = whenKeyword;
            AdjustFlagsAndWidth(openParenToken);
            this.openParenToken = openParenToken;
            AdjustFlagsAndWidth(filterExpression);
            this.filterExpression = filterExpression;
            AdjustFlagsAndWidth(closeParenToken);
            this.closeParenToken = closeParenToken;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => whenKeyword,
                1 => openParenToken,
                2 => filterExpression,
                3 => closeParenToken,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.CatchFilterClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCatchFilterClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitCatchFilterClause(this);
        }

        public CatchFilterClauseSyntax Update(SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken)
        {
            if (whenKeyword != WhenKeyword || openParenToken != OpenParenToken || filterExpression != FilterExpression || closeParenToken != CloseParenToken)
            {
                CatchFilterClauseSyntax catchFilterClauseSyntax = SyntaxFactory.CatchFilterClause(whenKeyword, openParenToken, filterExpression, closeParenToken);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    catchFilterClauseSyntax = catchFilterClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    catchFilterClauseSyntax = catchFilterClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return catchFilterClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new CatchFilterClauseSyntax(base.Kind, whenKeyword, openParenToken, filterExpression, closeParenToken, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new CatchFilterClauseSyntax(base.Kind, whenKeyword, openParenToken, filterExpression, closeParenToken, GetDiagnostics(), annotations);
        }

        public CatchFilterClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            whenKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            openParenToken = node2;
            ExpressionSyntax node3 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            filterExpression = node3;
            SyntaxToken node4 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            closeParenToken = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(whenKeyword);
            writer.WriteValue(openParenToken);
            writer.WriteValue(filterExpression);
            writer.WriteValue(closeParenToken);
        }

        static CatchFilterClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(CatchFilterClauseSyntax), (ObjectReader r) => new CatchFilterClauseSyntax(r));
        }
    }
}
