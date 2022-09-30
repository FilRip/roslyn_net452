#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ExpressionStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        public bool AllowsAnyExpression
        {
            get
            {
                SyntaxToken semicolonToken = SemicolonToken;
                if (semicolonToken.IsMissing)
                {
                    return !semicolonToken.ContainsDiagnostics;
                }
                return false;
            }
        }

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public ExpressionSyntax Expression => GetRed(ref expression, 1);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ExpressionStatementSyntax)base.Green).semicolonToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionStatementSyntax Update(ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, expression, semicolonToken);
        }

        internal ExpressionStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                1 => GetRed(ref expression, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                1 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitExpressionStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitExpressionStatement(this);

        public ExpressionStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || expression != Expression || semicolonToken != SemicolonToken)
            {
                ExpressionStatementSyntax expressionStatementSyntax = SyntaxFactory.ExpressionStatement(attributeLists, expression, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return expressionStatementSyntax;
                }
                return expressionStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ExpressionStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Expression, SemicolonToken);
        }

        public ExpressionStatementSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(AttributeLists, expression, SemicolonToken);
        }

        public ExpressionStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Expression, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ExpressionStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
