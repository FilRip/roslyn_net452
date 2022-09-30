#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AssignmentExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? left;

        private ExpressionSyntax? right;

        public ExpressionSyntax Left => GetRedAtZero(ref left);

        public SyntaxToken OperatorToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AssignmentExpressionSyntax)base.Green).operatorToken, GetChildPosition(1), GetChildIndex(1));

        public ExpressionSyntax Right => GetRed(ref right, 2);

        public AssignmentExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref left),
                2 => GetRed(ref right, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => left,
                2 => right,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAssignmentExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAssignmentExpression(this);

        public AssignmentExpressionSyntax Update(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
        {
            if (left != Left || operatorToken != OperatorToken || right != Right)
            {
                AssignmentExpressionSyntax assignmentExpressionSyntax = SyntaxFactory.AssignmentExpression(Kind(), left, operatorToken, right);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return assignmentExpressionSyntax;
                }
                return assignmentExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AssignmentExpressionSyntax WithLeft(ExpressionSyntax left)
        {
            return Update(left, OperatorToken, Right);
        }

        public AssignmentExpressionSyntax WithOperatorToken(SyntaxToken operatorToken)
        {
            return Update(Left, operatorToken, Right);
        }

        public AssignmentExpressionSyntax WithRight(ExpressionSyntax right)
        {
            return Update(Left, OperatorToken, right);
        }
    }
}
