#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class YieldStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken YieldKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.YieldStatementSyntax)base.Green).yieldKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken ReturnOrBreakKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.YieldStatementSyntax)base.Green).returnOrBreakKeyword, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax? Expression => GetRed(ref expression, 3);

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.YieldStatementSyntax)base.Green).semicolonToken, GetChildPosition(4), GetChildIndex(4));

        public YieldStatementSyntax Update(SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax expression, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, yieldKeyword, returnOrBreakKeyword, expression, semicolonToken);
        }

        internal YieldStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref expression, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitYieldStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitYieldStatement(this);

        public YieldStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken yieldKeyword, SyntaxToken returnOrBreakKeyword, ExpressionSyntax? expression, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || yieldKeyword != YieldKeyword || returnOrBreakKeyword != ReturnOrBreakKeyword || expression != Expression || semicolonToken != SemicolonToken)
            {
                YieldStatementSyntax yieldStatementSyntax = SyntaxFactory.YieldStatement(Kind(), attributeLists, yieldKeyword, returnOrBreakKeyword, expression, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return yieldStatementSyntax;
                }
                return yieldStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new YieldStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, YieldKeyword, ReturnOrBreakKeyword, Expression, SemicolonToken);
        }

        public YieldStatementSyntax WithYieldKeyword(SyntaxToken yieldKeyword)
        {
            return Update(AttributeLists, yieldKeyword, ReturnOrBreakKeyword, Expression, SemicolonToken);
        }

        public YieldStatementSyntax WithReturnOrBreakKeyword(SyntaxToken returnOrBreakKeyword)
        {
            return Update(AttributeLists, YieldKeyword, returnOrBreakKeyword, Expression, SemicolonToken);
        }

        public YieldStatementSyntax WithExpression(ExpressionSyntax? expression)
        {
            return Update(AttributeLists, YieldKeyword, ReturnOrBreakKeyword, expression, SemicolonToken);
        }

        public YieldStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, YieldKeyword, ReturnOrBreakKeyword, Expression, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new YieldStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
