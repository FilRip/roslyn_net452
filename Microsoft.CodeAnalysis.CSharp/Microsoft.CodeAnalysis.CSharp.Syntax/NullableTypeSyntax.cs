#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class NullableTypeSyntax : TypeSyntax
    {
        private TypeSyntax? elementType;

        public TypeSyntax ElementType => GetRedAtZero(ref elementType);

        public SyntaxToken QuestionToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NullableTypeSyntax)base.Green).questionToken, GetChildPosition(1), GetChildIndex(1));

        internal NullableTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref elementType);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return elementType;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNullableType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNullableType(this);

        public NullableTypeSyntax Update(TypeSyntax elementType, SyntaxToken questionToken)
        {
            if (elementType != ElementType || questionToken != QuestionToken)
            {
                NullableTypeSyntax nullableTypeSyntax = SyntaxFactory.NullableType(elementType, questionToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return nullableTypeSyntax;
                }
                return nullableTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public NullableTypeSyntax WithElementType(TypeSyntax elementType)
        {
            return Update(elementType, QuestionToken);
        }

        public NullableTypeSyntax WithQuestionToken(SyntaxToken questionToken)
        {
            return Update(ElementType, questionToken);
        }
    }
}
