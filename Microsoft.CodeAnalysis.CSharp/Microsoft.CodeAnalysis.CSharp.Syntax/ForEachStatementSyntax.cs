#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ForEachStatementSyntax : CommonForEachStatementSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeSyntax? type;

        private ExpressionSyntax? expression;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public override SyntaxToken AwaitKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken awaitKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green).awaitKeyword;
                if (awaitKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, awaitKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public override SyntaxToken ForEachKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green).forEachKeyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green).openParenToken, GetChildPosition(3), GetChildIndex(3));

        public TypeSyntax Type => GetRed(ref type, 4);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green).identifier, GetChildPosition(5), GetChildIndex(5));

        public override SyntaxToken InKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green).inKeyword, GetChildPosition(6), GetChildIndex(6));

        public override ExpressionSyntax Expression => GetRed(ref expression, 7);

        public override SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachStatementSyntax)base.Green).closeParenToken, GetChildPosition(8), GetChildIndex(8));

        public override StatementSyntax Statement => GetRed(ref statement, 9);

        public ForEachStatementSyntax Update(SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AwaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
        }

        public ForEachStatementSyntax Update(SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
        }

        internal ForEachStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                4 => GetRed(ref type, 4),
                7 => GetRed(ref expression, 7),
                9 => GetRed(ref statement, 9),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                4 => type,
                7 => expression,
                9 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitForEachStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitForEachStatement(this);

        public ForEachStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, TypeSyntax type, SyntaxToken identifier, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || forEachKeyword != ForEachKeyword || openParenToken != OpenParenToken || type != Type || identifier != Identifier || inKeyword != InKeyword || expression != Expression || closeParenToken != CloseParenToken || statement != Statement)
            {
                ForEachStatementSyntax forEachStatementSyntax = SyntaxFactory.ForEachStatement(attributeLists, awaitKeyword, forEachKeyword, openParenToken, type, identifier, inKeyword, expression, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return forEachStatementSyntax;
                }
                return forEachStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ForEachStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Type, Identifier, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithAwaitKeywordCore(SyntaxToken awaitKeyword)
        {
            return WithAwaitKeyword(awaitKeyword);
        }

        public new ForEachStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return Update(AttributeLists, awaitKeyword, ForEachKeyword, OpenParenToken, Type, Identifier, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithForEachKeywordCore(SyntaxToken forEachKeyword)
        {
            return WithForEachKeyword(forEachKeyword);
        }

        public new ForEachStatementSyntax WithForEachKeyword(SyntaxToken forEachKeyword)
        {
            return Update(AttributeLists, AwaitKeyword, forEachKeyword, OpenParenToken, Type, Identifier, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithOpenParenTokenCore(SyntaxToken openParenToken)
        {
            return WithOpenParenToken(openParenToken);
        }

        public new ForEachStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, openParenToken, Type, Identifier, InKeyword, Expression, CloseParenToken, Statement);
        }

        public ForEachStatementSyntax WithType(TypeSyntax type)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, type, Identifier, InKeyword, Expression, CloseParenToken, Statement);
        }

        public ForEachStatementSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Type, identifier, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithInKeywordCore(SyntaxToken inKeyword)
        {
            return WithInKeyword(inKeyword);
        }

        public new ForEachStatementSyntax WithInKeyword(SyntaxToken inKeyword)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Type, Identifier, inKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithExpressionCore(ExpressionSyntax expression)
        {
            return WithExpression(expression);
        }

        public new ForEachStatementSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Type, Identifier, InKeyword, expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken)
        {
            return WithCloseParenToken(closeParenToken);
        }

        public new ForEachStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Type, Identifier, InKeyword, Expression, closeParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithStatementCore(StatementSyntax statement)
        {
            return WithStatement(statement);
        }

        public new ForEachStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Type, Identifier, InKeyword, Expression, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ForEachStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
