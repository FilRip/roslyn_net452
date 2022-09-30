#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class BaseListSyntax : CSharpSyntaxNode
    {
        private SyntaxNode? types;

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.BaseListSyntax)base.Green).colonToken, base.Position, 0);

        public SeparatedSyntaxList<BaseTypeSyntax> Types
        {
            get
            {
                SyntaxNode red = GetRed(ref types, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<BaseTypeSyntax>);
                }
                return new SeparatedSyntaxList<BaseTypeSyntax>(red, GetChildIndex(1));
            }
        }

        internal BaseListSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref types, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return types;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitBaseList(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitBaseList(this);

        public BaseListSyntax Update(SyntaxToken colonToken, SeparatedSyntaxList<BaseTypeSyntax> types)
        {
            if (colonToken != ColonToken || types != Types)
            {
                BaseListSyntax baseListSyntax = SyntaxFactory.BaseList(colonToken, types);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return baseListSyntax;
                }
                return baseListSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public BaseListSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(colonToken, Types);
        }

        public BaseListSyntax WithTypes(SeparatedSyntaxList<BaseTypeSyntax> types)
        {
            return Update(ColonToken, types);
        }

        public BaseListSyntax AddTypes(params BaseTypeSyntax[] items)
        {
            return WithTypes(Types.AddRange(items));
        }
    }
}
