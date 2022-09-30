#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class CatchFilterClauseSyntax : CSharpSyntaxNode
    {
        private ExpressionSyntax? filterExpression;

        public SyntaxToken WhenKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchFilterClauseSyntax)base.Green).whenKeyword, base.Position, 0);

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchFilterClauseSyntax)base.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax FilterExpression => GetRed(ref filterExpression, 2);

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CatchFilterClauseSyntax)base.Green).closeParenToken, GetChildPosition(3), GetChildIndex(3));

        internal CatchFilterClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return GetRed(ref filterExpression, 2);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return filterExpression;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitCatchFilterClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitCatchFilterClause(this);

        public CatchFilterClauseSyntax Update(SyntaxToken whenKeyword, SyntaxToken openParenToken, ExpressionSyntax filterExpression, SyntaxToken closeParenToken)
        {
            if (whenKeyword != WhenKeyword || openParenToken != OpenParenToken || filterExpression != FilterExpression || closeParenToken != CloseParenToken)
            {
                CatchFilterClauseSyntax catchFilterClauseSyntax = SyntaxFactory.CatchFilterClause(whenKeyword, openParenToken, filterExpression, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return catchFilterClauseSyntax;
                }
                return catchFilterClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public CatchFilterClauseSyntax WithWhenKeyword(SyntaxToken whenKeyword)
        {
            return Update(whenKeyword, OpenParenToken, FilterExpression, CloseParenToken);
        }

        public CatchFilterClauseSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(WhenKeyword, openParenToken, FilterExpression, CloseParenToken);
        }

        public CatchFilterClauseSyntax WithFilterExpression(ExpressionSyntax filterExpression)
        {
            return Update(WhenKeyword, OpenParenToken, filterExpression, CloseParenToken);
        }

        public CatchFilterClauseSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(WhenKeyword, OpenParenToken, FilterExpression, closeParenToken);
        }
    }
}
