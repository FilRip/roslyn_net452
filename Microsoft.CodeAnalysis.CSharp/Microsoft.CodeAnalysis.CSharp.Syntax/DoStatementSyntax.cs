#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DoStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private StatementSyntax? statement;

        private ExpressionSyntax? condition;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken DoKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DoStatementSyntax)base.Green).doKeyword, GetChildPosition(1), GetChildIndex(1));

        public StatementSyntax Statement => GetRed(ref statement, 2);

        public SyntaxToken WhileKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DoStatementSyntax)base.Green).whileKeyword, GetChildPosition(3), GetChildIndex(3));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DoStatementSyntax)base.Green).openParenToken, GetChildPosition(4), GetChildIndex(4));

        public ExpressionSyntax Condition => GetRed(ref condition, 5);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DoStatementSyntax)base.Green).closeParenToken, GetChildPosition(6), GetChildIndex(6));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DoStatementSyntax)base.Green).semicolonToken, GetChildPosition(7), GetChildIndex(7));

        public DoStatementSyntax Update(SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken);
        }

        internal DoStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref statement, 2),
                5 => GetRed(ref condition, 5),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => statement,
                5 => condition,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDoStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDoStatement(this);

        public DoStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken doKeyword, StatementSyntax statement, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || doKeyword != DoKeyword || statement != Statement || whileKeyword != WhileKeyword || openParenToken != OpenParenToken || condition != Condition || closeParenToken != CloseParenToken || semicolonToken != SemicolonToken)
            {
                DoStatementSyntax doStatementSyntax = SyntaxFactory.DoStatement(attributeLists, doKeyword, statement, whileKeyword, openParenToken, condition, closeParenToken, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return doStatementSyntax;
                }
                return doStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new DoStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, DoKeyword, Statement, WhileKeyword, OpenParenToken, Condition, CloseParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithDoKeyword(SyntaxToken doKeyword)
        {
            return Update(AttributeLists, doKeyword, Statement, WhileKeyword, OpenParenToken, Condition, CloseParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, DoKeyword, statement, WhileKeyword, OpenParenToken, Condition, CloseParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithWhileKeyword(SyntaxToken whileKeyword)
        {
            return Update(AttributeLists, DoKeyword, Statement, whileKeyword, OpenParenToken, Condition, CloseParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, DoKeyword, Statement, WhileKeyword, openParenToken, Condition, CloseParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithCondition(ExpressionSyntax condition)
        {
            return Update(AttributeLists, DoKeyword, Statement, WhileKeyword, OpenParenToken, condition, CloseParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, DoKeyword, Statement, WhileKeyword, OpenParenToken, Condition, closeParenToken, SemicolonToken);
        }

        public DoStatementSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, DoKeyword, Statement, WhileKeyword, OpenParenToken, Condition, CloseParenToken, semicolonToken);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new DoStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
