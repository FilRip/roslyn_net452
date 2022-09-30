#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class PrimaryConstructorBaseTypeSyntax : BaseTypeSyntax
    {
        private TypeSyntax? type;

        private ArgumentListSyntax? argumentList;

        public override TypeSyntax Type => GetRedAtZero(ref type);

        public ArgumentListSyntax ArgumentList => GetRed(ref argumentList, 1);

        internal PrimaryConstructorBaseTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref type),
                1 => GetRed(ref argumentList, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => type,
                1 => argumentList,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitPrimaryConstructorBaseType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitPrimaryConstructorBaseType(this);

        public PrimaryConstructorBaseTypeSyntax Update(TypeSyntax type, ArgumentListSyntax argumentList)
        {
            if (type != Type || argumentList != ArgumentList)
            {
                PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeSyntax = SyntaxFactory.PrimaryConstructorBaseType(type, argumentList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return primaryConstructorBaseTypeSyntax;
                }
                return primaryConstructorBaseTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override BaseTypeSyntax WithTypeCore(TypeSyntax type)
        {
            return WithType(type);
        }

        public new PrimaryConstructorBaseTypeSyntax WithType(TypeSyntax type)
        {
            return Update(type, ArgumentList);
        }

        public PrimaryConstructorBaseTypeSyntax WithArgumentList(ArgumentListSyntax argumentList)
        {
            return Update(Type, argumentList);
        }

        public PrimaryConstructorBaseTypeSyntax AddArgumentListArguments(params ArgumentSyntax[] items)
        {
            return WithArgumentList(ArgumentList.WithArguments(ArgumentList.Arguments.AddRange(items)));
        }
    }
}
