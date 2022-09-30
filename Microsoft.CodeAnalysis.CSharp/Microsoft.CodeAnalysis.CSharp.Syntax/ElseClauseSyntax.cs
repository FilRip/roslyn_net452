#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ElseClauseSyntax : CSharpSyntaxNode
    {
        private StatementSyntax? statement;

        public SyntaxToken ElseKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ElseClauseSyntax)base.Green).elseKeyword, base.Position, 0);

        public StatementSyntax Statement => GetRed(ref statement, 1);

        internal ElseClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref statement, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return statement;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitElseClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitElseClause(this);

        public ElseClauseSyntax Update(SyntaxToken elseKeyword, StatementSyntax statement)
        {
            if (elseKeyword != ElseKeyword || statement != Statement)
            {
                ElseClauseSyntax elseClauseSyntax = SyntaxFactory.ElseClause(elseKeyword, statement);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return elseClauseSyntax;
                }
                return elseClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ElseClauseSyntax WithElseKeyword(SyntaxToken elseKeyword)
        {
            return Update(elseKeyword, Statement);
        }

        public ElseClauseSyntax WithStatement(StatementSyntax statement)
        {
            return Update(ElseKeyword, statement);
        }
    }
}
