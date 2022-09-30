#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SwitchExpressionSyntax : ExpressionSyntax
    {
        private ExpressionSyntax? governingExpression;

        private SyntaxNode? arms;

        public ExpressionSyntax GoverningExpression => GetRedAtZero(ref governingExpression);

        public SyntaxToken SwitchKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchExpressionSyntax)base.Green).switchKeyword, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchExpressionSyntax)base.Green).openBraceToken, GetChildPosition(2), GetChildIndex(2));

        public SeparatedSyntaxList<SwitchExpressionArmSyntax> Arms
        {
            get
            {
                SyntaxNode red = GetRed(ref arms, 3);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<SwitchExpressionArmSyntax>);
                }
                return new SeparatedSyntaxList<SwitchExpressionArmSyntax>(red, GetChildIndex(3));
            }
        }

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SwitchExpressionSyntax)base.Green).closeBraceToken, GetChildPosition(4), GetChildIndex(4));

        internal SwitchExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref governingExpression),
                3 => GetRed(ref arms, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => governingExpression,
                3 => arms,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSwitchExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSwitchExpression(this);

        public SwitchExpressionSyntax Update(ExpressionSyntax governingExpression, SyntaxToken switchKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<SwitchExpressionArmSyntax> arms, SyntaxToken closeBraceToken)
        {
            if (governingExpression != GoverningExpression || switchKeyword != SwitchKeyword || openBraceToken != OpenBraceToken || arms != Arms || closeBraceToken != CloseBraceToken)
            {
                SwitchExpressionSyntax switchExpressionSyntax = SyntaxFactory.SwitchExpression(governingExpression, switchKeyword, openBraceToken, arms, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return switchExpressionSyntax;
                }
                return switchExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SwitchExpressionSyntax WithGoverningExpression(ExpressionSyntax governingExpression)
        {
            return Update(governingExpression, SwitchKeyword, OpenBraceToken, Arms, CloseBraceToken);
        }

        public SwitchExpressionSyntax WithSwitchKeyword(SyntaxToken switchKeyword)
        {
            return Update(GoverningExpression, switchKeyword, OpenBraceToken, Arms, CloseBraceToken);
        }

        public SwitchExpressionSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(GoverningExpression, SwitchKeyword, openBraceToken, Arms, CloseBraceToken);
        }

        public SwitchExpressionSyntax WithArms(SeparatedSyntaxList<SwitchExpressionArmSyntax> arms)
        {
            return Update(GoverningExpression, SwitchKeyword, OpenBraceToken, arms, CloseBraceToken);
        }

        public SwitchExpressionSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(GoverningExpression, SwitchKeyword, OpenBraceToken, Arms, closeBraceToken);
        }

        public SwitchExpressionSyntax AddArms(params SwitchExpressionArmSyntax[] items)
        {
            return WithArms(Arms.AddRange(items));
        }
    }
}
