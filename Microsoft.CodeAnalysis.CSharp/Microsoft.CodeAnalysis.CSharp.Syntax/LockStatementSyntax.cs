#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class LockStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? expression;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken LockKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LockStatementSyntax)base.Green).lockKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LockStatementSyntax)base.Green).openParenToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref expression, 3);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.LockStatementSyntax)base.Green).closeParenToken, GetChildPosition(4), GetChildIndex(4));

        public StatementSyntax Statement => GetRed(ref statement, 5);

        public LockStatementSyntax Update(SyntaxToken lockKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, lockKeyword, openParenToken, expression, closeParenToken, statement);
        }

        internal LockStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref expression, 3),
                5 => GetRed(ref statement, 5),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => expression,
                5 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitLockStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitLockStatement(this);

        public LockStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken lockKeyword, SyntaxToken openParenToken, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || lockKeyword != LockKeyword || openParenToken != OpenParenToken || expression != Expression || closeParenToken != CloseParenToken || statement != Statement)
            {
                LockStatementSyntax lockStatementSyntax = SyntaxFactory.LockStatement(attributeLists, lockKeyword, openParenToken, expression, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return lockStatementSyntax;
                }
                return lockStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new LockStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, LockKeyword, OpenParenToken, Expression, CloseParenToken, Statement);
        }

        public LockStatementSyntax WithLockKeyword(SyntaxToken lockKeyword)
        {
            return Update(AttributeLists, lockKeyword, OpenParenToken, Expression, CloseParenToken, Statement);
        }

        public LockStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, LockKeyword, openParenToken, Expression, CloseParenToken, Statement);
        }

        public LockStatementSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(AttributeLists, LockKeyword, OpenParenToken, expression, CloseParenToken, Statement);
        }

        public LockStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, LockKeyword, OpenParenToken, Expression, closeParenToken, Statement);
        }

        public LockStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, LockKeyword, OpenParenToken, Expression, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new LockStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
