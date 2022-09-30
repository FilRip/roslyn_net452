#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ReturnStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken ReturnKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ReturnStatementSyntax)base.Green).returnKeyword, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax? Expression => GetRed(ref expression, 2);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ReturnStatementSyntax)base.Green).semicolonToken, GetChildPosition(3), GetChildIndex(3));

        public ReturnStatementSyntax Update(SyntaxToken returnKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, returnKeyword, expression, semicolonToken);
        }

        internal ReturnStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref expression, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitReturnStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitReturnStatement(this);

        public ReturnStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken returnKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || returnKeyword != ReturnKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                ReturnStatementSyntax returnStatementSyntax = SyntaxFactory.ReturnStatement(attributeLists, returnKeyword, expression, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return returnStatementSyntax;
                }
                return returnStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ReturnStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, ReturnKeyword, Expression, SemicolonToken);
        }

        public ReturnStatementSyntax WithReturnKeyword(SyntaxToken returnKeyword)
        {
            return Update(AttributeLists, returnKeyword, Expression, SemicolonToken);
        }

        public ReturnStatementSyntax WithExpression(ExpressionSyntax? expression)
        {
            return Update(AttributeLists, ReturnKeyword, expression, SemicolonToken);
        }

        public ReturnStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, ReturnKeyword, Expression, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ReturnStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
