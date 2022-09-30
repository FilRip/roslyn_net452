using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax
{
    public sealed class LetClauseSyntax : QueryClauseSyntax
    {
        internal readonly SyntaxToken letKeyword;

        internal readonly SyntaxToken identifier;

        internal readonly SyntaxToken equalsToken;

        internal readonly ExpressionSyntax expression;

        public SyntaxToken LetKeyword => letKeyword;

        public SyntaxToken Identifier => identifier;

        public SyntaxToken EqualsToken => equalsToken;

        public ExpressionSyntax Expression => expression;

        public LetClauseSyntax(SyntaxKind kind, SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression, DiagnosticInfo[]? diagnostics, SyntaxAnnotation[]? annotations)
            : base(kind, diagnostics, annotations)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(letKeyword);
            this.letKeyword = letKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public LetClauseSyntax(SyntaxKind kind, SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression, SyntaxFactoryContext context)
            : base(kind)
        {
            SetFactoryContext(context);
            base.SlotCount = 4;
            AdjustFlagsAndWidth(letKeyword);
            this.letKeyword = letKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public LetClauseSyntax(SyntaxKind kind, SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
            : base(kind)
        {
            base.SlotCount = 4;
            AdjustFlagsAndWidth(letKeyword);
            this.letKeyword = letKeyword;
            AdjustFlagsAndWidth(identifier);
            this.identifier = identifier;
            AdjustFlagsAndWidth(equalsToken);
            this.equalsToken = equalsToken;
            AdjustFlagsAndWidth(expression);
            this.expression = expression;
        }

        public override GreenNode? GetSlot(int index)
        {
            return index switch
            {
                0 => letKeyword,
                1 => identifier,
                2 => equalsToken,
                3 => expression,
                _ => null,
            };
        }

        public override SyntaxNode CreateRed(SyntaxNode? parent, int position)
        {
            return new Microsoft.CodeAnalysis.CSharp.Syntax.LetClauseSyntax(this, parent, position);
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLetClause(this);
        }

        public override TResult Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor)
        {
            return visitor.VisitLetClause(this);
        }

        public LetClauseSyntax Update(SyntaxToken letKeyword, SyntaxToken identifier, SyntaxToken equalsToken, ExpressionSyntax expression)
        {
            if (letKeyword != LetKeyword || identifier != Identifier || equalsToken != EqualsToken || expression != Expression)
            {
                LetClauseSyntax letClauseSyntax = SyntaxFactory.LetClause(letKeyword, identifier, equalsToken, expression);
                DiagnosticInfo[] diagnostics = GetDiagnostics();
                if (diagnostics != null && diagnostics.Length != 0)
                {
                    letClauseSyntax = letClauseSyntax.WithDiagnosticsGreen(diagnostics);
                }
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations != null && annotations.Length != 0)
                {
                    letClauseSyntax = letClauseSyntax.WithAnnotationsGreen(annotations);
                }
                return letClauseSyntax;
            }
            return this;
        }

        public override GreenNode SetDiagnostics(DiagnosticInfo[]? diagnostics)
        {
            return new LetClauseSyntax(base.Kind, letKeyword, identifier, equalsToken, expression, diagnostics, GetAnnotations());
        }

        public override GreenNode SetAnnotations(SyntaxAnnotation[]? annotations)
        {
            return new LetClauseSyntax(base.Kind, letKeyword, identifier, equalsToken, expression, GetDiagnostics(), annotations);
        }

        public LetClauseSyntax(ObjectReader reader)
            : base(reader)
        {
            base.SlotCount = 4;
            SyntaxToken node = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node);
            letKeyword = node;
            SyntaxToken node2 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node2);
            identifier = node2;
            SyntaxToken node3 = (SyntaxToken)reader.ReadValue();
            AdjustFlagsAndWidth(node3);
            equalsToken = node3;
            ExpressionSyntax node4 = (ExpressionSyntax)reader.ReadValue();
            AdjustFlagsAndWidth(node4);
            expression = node4;
        }

        public override void WriteTo(ObjectWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteValue(letKeyword);
            writer.WriteValue(identifier);
            writer.WriteValue(equalsToken);
            writer.WriteValue(expression);
        }

        static LetClauseSyntax()
        {
            ObjectBinder.RegisterTypeReader(typeof(LetClauseSyntax), (ObjectReader r) => new LetClauseSyntax(r));
        }
    }
}
