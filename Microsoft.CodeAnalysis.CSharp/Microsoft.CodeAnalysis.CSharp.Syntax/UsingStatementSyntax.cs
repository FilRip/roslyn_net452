#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class UsingStatementSyntax : StatementSyntax
    {
        private SyntaxNode? attributeLists;

        private VariableDeclarationSyntax? declaration;

        private ExpressionSyntax? expression;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public SyntaxToken AwaitKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken awaitKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingStatementSyntax)base.Green).awaitKeyword;
                if (awaitKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, awaitKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public SyntaxToken UsingKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingStatementSyntax)base.Green).usingKeyword, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingStatementSyntax)base.Green).openParenToken, GetChildPosition(3), GetChildIndex(3));

        public VariableDeclarationSyntax? Declaration => GetRed(ref declaration, 4);

        public ExpressionSyntax? Expression => GetRed(ref expression, 5);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.UsingStatementSyntax)base.Green).closeParenToken, GetChildPosition(6), GetChildIndex(6));

        public StatementSyntax Statement => GetRed(ref statement, 7);

        public UsingStatementSyntax Update(SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AwaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
        }

        public UsingStatementSyntax Update(SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
        }

        internal UsingStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                4 => GetRed(ref declaration, 4),
                5 => GetRed(ref expression, 5),
                7 => GetRed(ref statement, 7),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                4 => declaration,
                5 => expression,
                7 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitUsingStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitUsingStatement(this);

        public UsingStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken usingKeyword, SyntaxToken openParenToken, VariableDeclarationSyntax? declaration, ExpressionSyntax? expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || usingKeyword != UsingKeyword || openParenToken != OpenParenToken || declaration != Declaration || expression != Expression || closeParenToken != CloseParenToken || statement != Statement)
            {
                UsingStatementSyntax usingStatementSyntax = SyntaxFactory.UsingStatement(attributeLists, awaitKeyword, usingKeyword, openParenToken, declaration, expression, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return usingStatementSyntax;
                }
                return usingStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new UsingStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, AwaitKeyword, UsingKeyword, OpenParenToken, Declaration, Expression, CloseParenToken, Statement);
        }

        public UsingStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return Update(AttributeLists, awaitKeyword, UsingKeyword, OpenParenToken, Declaration, Expression, CloseParenToken, Statement);
        }

        public UsingStatementSyntax WithUsingKeyword(SyntaxToken usingKeyword)
        {
            return Update(AttributeLists, AwaitKeyword, usingKeyword, OpenParenToken, Declaration, Expression, CloseParenToken, Statement);
        }

        public UsingStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, openParenToken, Declaration, Expression, CloseParenToken, Statement);
        }

        public UsingStatementSyntax WithDeclaration(VariableDeclarationSyntax? declaration)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, OpenParenToken, declaration, Expression, CloseParenToken, Statement);
        }

        public UsingStatementSyntax WithExpression(ExpressionSyntax? expression)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, OpenParenToken, Declaration, expression, CloseParenToken, Statement);
        }

        public UsingStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, OpenParenToken, Declaration, Expression, closeParenToken, Statement);
        }

        public UsingStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, AwaitKeyword, UsingKeyword, OpenParenToken, Declaration, Expression, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new UsingStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
