#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class GroupClauseSyntax : SelectOrGroupClauseSyntax
    {
        private ExpressionSyntax? groupExpression;

        private ExpressionSyntax? byExpression;

        public SyntaxToken GroupKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.GroupClauseSyntax)base.Green).groupKeyword, base.Position, 0);

        public ExpressionSyntax GroupExpression => GetRed(ref groupExpression, 1);

        public SyntaxToken ByKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.GroupClauseSyntax)base.Green).byKeyword, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax ByExpression => GetRed(ref byExpression, 3);

        internal GroupClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref groupExpression, 1),
                3 => GetRed(ref byExpression, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => groupExpression,
                3 => byExpression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitGroupClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitGroupClause(this);

        public GroupClauseSyntax Update(SyntaxToken groupKeyword, ExpressionSyntax groupExpression, SyntaxToken byKeyword, ExpressionSyntax byExpression)
        {
            if (groupKeyword != GroupKeyword || groupExpression != GroupExpression || byKeyword != ByKeyword || byExpression != ByExpression)
            {
                GroupClauseSyntax groupClauseSyntax = SyntaxFactory.GroupClause(groupKeyword, groupExpression, byKeyword, byExpression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return groupClauseSyntax;
                }
                return groupClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public GroupClauseSyntax WithGroupKeyword(SyntaxToken groupKeyword)
        {
            return Update(groupKeyword, GroupExpression, ByKeyword, ByExpression);
        }

        public GroupClauseSyntax WithGroupExpression(ExpressionSyntax groupExpression)
        {
            return Update(GroupKeyword, groupExpression, ByKeyword, ByExpression);
        }

        public GroupClauseSyntax WithByKeyword(SyntaxToken byKeyword)
        {
            return Update(GroupKeyword, GroupExpression, byKeyword, ByExpression);
        }

        public GroupClauseSyntax WithByExpression(ExpressionSyntax byExpression)
        {
            return Update(GroupKeyword, GroupExpression, ByKeyword, byExpression);
        }
    }
}
