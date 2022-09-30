#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PredefinedTypeSyntax : TypeSyntax
    {
        public SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.PredefinedTypeSyntax)base.Green).keyword, base.Position, 0);

        internal PredefinedTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
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
            visitor.VisitPredefinedType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPredefinedType(this);

        public PredefinedTypeSyntax Update(SyntaxToken keyword)
        {
            if (keyword != Keyword)
            {
                PredefinedTypeSyntax predefinedTypeSyntax = SyntaxFactory.PredefinedType(keyword);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return predefinedTypeSyntax;
                }
                return predefinedTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public PredefinedTypeSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(keyword);
        }
    }
}
