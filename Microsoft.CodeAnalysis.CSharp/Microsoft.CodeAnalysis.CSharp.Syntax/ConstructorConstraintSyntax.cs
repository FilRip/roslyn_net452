#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ConstructorConstraintSyntax : TypeParameterConstraintSyntax
    {
        public SyntaxToken NewKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorConstraintSyntax)base.Green).newKeyword, base.Position, 0);

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorConstraintSyntax)base.Green).openParenToken, GetChildPosition(1), GetChildIndex(1));

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ConstructorConstraintSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal ConstructorConstraintSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return null;
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return null;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitConstructorConstraint(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitConstructorConstraint(this);

        public ConstructorConstraintSyntax Update(SyntaxToken newKeyword, SyntaxToken openParenToken, SyntaxToken closeParenToken)
        {
            if (newKeyword != NewKeyword || openParenToken != OpenParenToken || closeParenToken != CloseParenToken)
            {
                ConstructorConstraintSyntax constructorConstraintSyntax = SyntaxFactory.ConstructorConstraint(newKeyword, openParenToken, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return constructorConstraintSyntax;
                }
                return constructorConstraintSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ConstructorConstraintSyntax WithNewKeyword(SyntaxToken newKeyword)
        {
            return Update(newKeyword, OpenParenToken, CloseParenToken);
        }

        public ConstructorConstraintSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(NewKeyword, openParenToken, CloseParenToken);
        }

        public ConstructorConstraintSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(NewKeyword, OpenParenToken, closeParenToken);
        }
    }
}
