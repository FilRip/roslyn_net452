#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class WhileStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? condition;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken WhileKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhileStatementSyntax)base.Green).whileKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhileStatementSyntax)base.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Condition => GetRed(ref condition, 3);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.WhileStatementSyntax)base.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref statement, 5);

        public WhileStatementSyntax Update(SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, whileKeyword, openParenToken, condition, closeParenToken, statement);
        }

        internal WhileStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref condition, 3),
                5 => GetRed(ref statement, 5),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => condition,
                5 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitWhileStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitWhileStatement(this);

        public WhileStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken whileKeyword, SyntaxToken openParenToken, ExpressionSyntax condition, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || whileKeyword != WhileKeyword || openParenToken != OpenParenToken || condition != Condition || closeParenToken != CloseParenToken || statement != Statement)
            {
                WhileStatementSyntax whileStatementSyntax = SyntaxFactory.WhileStatement(attributeLists, whileKeyword, openParenToken, condition, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return whileStatementSyntax;
                }
                return whileStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new WhileStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, WhileKeyword, OpenParenToken, Condition, CloseParenToken, Statement);
        }

        public WhileStatementSyntax WithWhileKeyword(SyntaxToken whileKeyword)
        {
            return Update(AttributeLists, whileKeyword, OpenParenToken, Condition, CloseParenToken, Statement);
        }

        public WhileStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, WhileKeyword, openParenToken, Condition, CloseParenToken, Statement);
        }

        public WhileStatementSyntax WithCondition(ExpressionSyntax condition)
        {
            return Update(AttributeLists, WhileKeyword, OpenParenToken, condition, CloseParenToken, Statement);
        }

        public WhileStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, WhileKeyword, OpenParenToken, Condition, closeParenToken, Statement);
        }

        public WhileStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, WhileKeyword, OpenParenToken, Condition, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new WhileStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
