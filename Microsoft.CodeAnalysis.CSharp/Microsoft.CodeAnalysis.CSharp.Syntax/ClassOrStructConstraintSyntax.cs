#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ClassOrStructConstraintSyntax : TypeParameterConstraintSyntax
    {
        public SyntaxToken ClassOrStructKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ClassOrStructConstraintSyntax)base.Green).classOrStructKeyword, base.Position, 0);

        public SyntaxToken QuestionToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken questionToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ClassOrStructConstraintSyntax)base.Green).questionToken;
                if (questionToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, questionToken, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public ClassOrStructConstraintSyntax Update(SyntaxToken classOrStructKeyword)
        {
            return Update(classOrStructKeyword, QuestionToken);
        }

        internal ClassOrStructConstraintSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitClassOrStructConstraint(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitClassOrStructConstraint(this);

        public ClassOrStructConstraintSyntax Update(SyntaxToken classOrStructKeyword, SyntaxToken questionToken)
        {
            if (classOrStructKeyword != ClassOrStructKeyword || questionToken != QuestionToken)
            {
                ClassOrStructConstraintSyntax classOrStructConstraintSyntax = SyntaxFactory.ClassOrStructConstraint(Kind(), classOrStructKeyword, questionToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return classOrStructConstraintSyntax;
                }
                return classOrStructConstraintSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public ClassOrStructConstraintSyntax WithClassOrStructKeyword(SyntaxToken classOrStructKeyword)
        {
            return Update(classOrStructKeyword, QuestionToken);
        }

        public ClassOrStructConstraintSyntax WithQuestionToken(SyntaxToken questionToken)
        {
            return Update(ClassOrStructKeyword, questionToken);
        }
    }
}
