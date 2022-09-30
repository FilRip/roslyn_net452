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
    public sealed class ForEachVariableStatementSyntax : CommonForEachStatementSyntax
    {
        private SyntaxNode? attributeLists;

        private ExpressionSyntax? variable;

        private ExpressionSyntax? expression;

        private StatementSyntax? statement;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public override SyntaxToken AwaitKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken awaitKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachVariableStatementSyntax)base.Green).awaitKeyword;
                if (awaitKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, awaitKeyword, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public override SyntaxToken ForEachKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachVariableStatementSyntax)base.Green).forEachKeyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachVariableStatementSyntax)base.Green).openParenToken, GetChildPosition(3), GetChildIndex(3));

        public ExpressionSyntax Variable => GetRed(ref variable, 4);

        public override SyntaxToken InKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachVariableStatementSyntax)base.Green).inKeyword, GetChildPosition(5), GetChildIndex(5));

        public override ExpressionSyntax Expression => GetRed(ref expression, 6);

        public override SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ForEachVariableStatementSyntax)base.Green).closeParenToken, GetChildPosition(7), GetChildIndex(7));

        public override StatementSyntax Statement => GetRed(ref statement, 8);

        public ForEachVariableStatementSyntax Update(SyntaxToken forEachKeyword, SyntaxToken openParenToken, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AwaitKeyword, forEachKeyword, openParenToken, variable, inKeyword, expression, closeParenToken, statement);
        }

        public ForEachVariableStatementSyntax Update(SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            return Update(AttributeLists, awaitKeyword, forEachKeyword, openParenToken, variable, inKeyword, expression, closeParenToken, statement);
        }

        internal ForEachVariableStatementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                4 => GetRed(ref variable, 4),
                6 => GetRed(ref expression, 6),
                8 => GetRed(ref statement, 8),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                4 => variable,
                6 => expression,
                8 => statement,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitForEachVariableStatement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitForEachVariableStatement(this);

        public ForEachVariableStatementSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken awaitKeyword, SyntaxToken forEachKeyword, SyntaxToken openParenToken, ExpressionSyntax variable, SyntaxToken inKeyword, ExpressionSyntax expression, SyntaxToken closeParenToken, StatementSyntax statement)
        {
            if (attributeLists != AttributeLists || awaitKeyword != AwaitKeyword || forEachKeyword != ForEachKeyword || openParenToken != OpenParenToken || variable != Variable || inKeyword != InKeyword || expression != Expression || closeParenToken != CloseParenToken || statement != Statement)
            {
                ForEachVariableStatementSyntax forEachVariableStatementSyntax = SyntaxFactory.ForEachVariableStatement(attributeLists, awaitKeyword, forEachKeyword, openParenToken, variable, inKeyword, expression, closeParenToken, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return forEachVariableStatementSyntax;
                }
                return forEachVariableStatementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override StatementSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ForEachVariableStatementSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Variable, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithAwaitKeywordCore(SyntaxToken awaitKeyword)
        {
            return WithAwaitKeyword(awaitKeyword);
        }

        public new ForEachVariableStatementSyntax WithAwaitKeyword(SyntaxToken awaitKeyword)
        {
            return Update(AttributeLists, awaitKeyword, ForEachKeyword, OpenParenToken, Variable, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithForEachKeywordCore(SyntaxToken forEachKeyword)
        {
            return WithForEachKeyword(forEachKeyword);
        }

        public new ForEachVariableStatementSyntax WithForEachKeyword(SyntaxToken forEachKeyword)
        {
            return Update(AttributeLists, AwaitKeyword, forEachKeyword, OpenParenToken, Variable, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithOpenParenTokenCore(SyntaxToken openParenToken)
        {
            return WithOpenParenToken(openParenToken);
        }

        public new ForEachVariableStatementSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, openParenToken, Variable, InKeyword, Expression, CloseParenToken, Statement);
        }

        public ForEachVariableStatementSyntax WithVariable(ExpressionSyntax variable)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, variable, InKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithInKeywordCore(SyntaxToken inKeyword)
        {
            return WithInKeyword(inKeyword);
        }

        public new ForEachVariableStatementSyntax WithInKeyword(SyntaxToken inKeyword)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Variable, inKeyword, Expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithExpressionCore(ExpressionSyntax expression)
        {
            return WithExpression(expression);
        }

        public new ForEachVariableStatementSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Variable, InKeyword, expression, CloseParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithCloseParenTokenCore(SyntaxToken closeParenToken)
        {
            return WithCloseParenToken(closeParenToken);
        }

        public new ForEachVariableStatementSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Variable, InKeyword, Expression, closeParenToken, Statement);
        }

        internal override CommonForEachStatementSyntax WithStatementCore(StatementSyntax statement)
        {
            return WithStatement(statement);
        }

        public new ForEachVariableStatementSyntax WithStatement(StatementSyntax statement)
        {
            return Update(AttributeLists, AwaitKeyword, ForEachKeyword, OpenParenToken, Variable, InKeyword, Expression, CloseParenToken, statement);
        }

        internal override StatementSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ForEachVariableStatementSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }
    }
}
