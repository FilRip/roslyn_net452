#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class EnumMemberDeclarationSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private EqualsValueClauseSyntax? equalsValue;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                GreenNode slot = base.Green.GetSlot(1);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EnumMemberDeclarationSyntax)base.Green).identifier, GetChildPosition(2), GetChildIndex(2));

        public EqualsValueClauseSyntax? EqualsValue => GetRed(ref equalsValue, 3);

        public EnumMemberDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxToken identifier, EqualsValueClauseSyntax equalsValue)
        {
            return Update(attributeLists, Modifiers, identifier, equalsValue);
        }

        internal EnumMemberDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref equalsValue, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => equalsValue,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEnumMemberDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEnumMemberDeclaration(this);

        public EnumMemberDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken identifier, EqualsValueClauseSyntax? equalsValue)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || identifier != Identifier || equalsValue != EqualsValue)
            {
                EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = SyntaxFactory.EnumMemberDeclaration(attributeLists, modifiers, identifier, equalsValue);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return enumMemberDeclarationSyntax;
                }
                return enumMemberDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new EnumMemberDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, Identifier, EqualsValue);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new EnumMemberDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, Identifier, EqualsValue);
        }

        public EnumMemberDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, identifier, EqualsValue);
        }

        public EnumMemberDeclarationSyntax WithEqualsValue(EqualsValueClauseSyntax? equalsValue)
        {
            return Update(AttributeLists, Modifiers, Identifier, equalsValue);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new EnumMemberDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new EnumMemberDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }
    }
}
