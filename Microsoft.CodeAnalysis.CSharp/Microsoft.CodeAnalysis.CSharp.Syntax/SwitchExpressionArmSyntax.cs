#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SwitchExpressionArmSyntax : CSharpSyntaxNode
    {
        private PatternSyntax? pattern;

        private WhenClauseSyntax? whenClause;

        private ExpressionSyntax? expression;

        public PatternSyntax Pattern => GetRedAtZero(ref pattern);

        public WhenClauseSyntax? WhenClause => GetRed(ref whenClause, 1);

        public SyntaxToken EqualsGreaterThanToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchExpressionArmSyntax)base.Green).equalsGreaterThanToken, GetChildPosition(2), GetChildIndex(2));

        public ExpressionSyntax Expression => GetRed(ref expression, 3);

        internal SwitchExpressionArmSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref pattern),
                1 => GetRed(ref whenClause, 1),
                3 => GetRed(ref expression, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => pattern,
                1 => whenClause,
                3 => expression,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchExpressionArm(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchExpressionArm(this);

        public SwitchExpressionArmSyntax Update(PatternSyntax pattern, WhenClauseSyntax? whenClause, SyntaxToken equalsGreaterThanToken, ExpressionSyntax expression)
        {
            if (pattern != Pattern || whenClause != WhenClause || equalsGreaterThanToken != EqualsGreaterThanToken || expression != Expression)
            {
                SwitchExpressionArmSyntax switchExpressionArmSyntax = SyntaxFactory.SwitchExpressionArm(pattern, whenClause, equalsGreaterThanToken, expression);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return switchExpressionArmSyntax;
                }
                return switchExpressionArmSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SwitchExpressionArmSyntax WithPattern(PatternSyntax pattern)
        {
            return Update(pattern, WhenClause, EqualsGreaterThanToken, Expression);
        }

        public SwitchExpressionArmSyntax WithWhenClause(WhenClauseSyntax? whenClause)
        {
            return Update(Pattern, whenClause, EqualsGreaterThanToken, Expression);
        }

        public SwitchExpressionArmSyntax WithEqualsGreaterThanToken(SyntaxToken equalsGreaterThanToken)
        {
            return Update(Pattern, WhenClause, equalsGreaterThanToken, Expression);
        }

        public SwitchExpressionArmSyntax WithExpression(ExpressionSyntax expression)
        {
            return Update(Pattern, WhenClause, EqualsGreaterThanToken, expression);
        }
    }
}
