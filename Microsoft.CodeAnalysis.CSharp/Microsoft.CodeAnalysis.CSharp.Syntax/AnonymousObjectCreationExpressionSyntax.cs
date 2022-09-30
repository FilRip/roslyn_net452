#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AnonymousObjectCreationExpressionSyntax : ExpressionSyntax
    {
        private SyntaxNode? initializers;

        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)base.Green).newKeyword, base.Position, 0);

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)base.Green).openBraceToken, GetChildPosition(1), GetChildIndex(1));

        public SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> Initializers
        {
            get
            {
                SyntaxNode red = GetRed(ref initializers, 2);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax>);
                }
                return new SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax>(red, GetChildIndex(2));
            }
        }

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.AnonymousObjectCreationExpressionSyntax)base.Green).closeBraceToken, GetChildPosition(3), GetChildIndex(3));

        internal AnonymousObjectCreationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return GetRed(ref initializers, 2);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 2)
            {
                return null;
            }
            return initializers;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAnonymousObjectCreationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAnonymousObjectCreationExpression(this);

        public AnonymousObjectCreationExpressionSyntax Update(SyntaxToken newKeyword, SyntaxToken openBraceToken, SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers, SyntaxToken closeBraceToken)
        {
            if (newKeyword != NewKeyword || openBraceToken != OpenBraceToken || initializers != Initializers || closeBraceToken != CloseBraceToken)
            {
                AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax = SyntaxFactory.AnonymousObjectCreationExpression(newKeyword, openBraceToken, initializers, closeBraceToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return anonymousObjectCreationExpressionSyntax;
                }
                return anonymousObjectCreationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AnonymousObjectCreationExpressionSyntax WithNewKeyword(SyntaxToken newKeyword)
        {
            return Update(newKeyword, OpenBraceToken, Initializers, CloseBraceToken);
        }

        public AnonymousObjectCreationExpressionSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(NewKeyword, openBraceToken, Initializers, CloseBraceToken);
        }

        public AnonymousObjectCreationExpressionSyntax WithInitializers(SeparatedSyntaxList<AnonymousObjectMemberDeclaratorSyntax> initializers)
        {
            return Update(NewKeyword, OpenBraceToken, initializers, CloseBraceToken);
        }

        public AnonymousObjectCreationExpressionSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(NewKeyword, OpenBraceToken, Initializers, closeBraceToken);
        }

        public AnonymousObjectCreationExpressionSyntax AddInitializers(params AnonymousObjectMemberDeclaratorSyntax[] items)
        {
            return WithInitializers(Initializers.AddRange(items));
        }
    }
}
