#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class EventDeclarationSyntax : BasePropertyDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeSyntax? type;

        private ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier;

        private AccessorListSyntax? accessorList;

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

        public SyntaxToken EventKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventDeclarationSyntax)base.Green).eventKeyword, GetChildPosition(2), GetChildIndex(2));

        public override TypeSyntax Type => GetRed(ref type, 3);

        public override ExplicitInterfaceSpecifierSyntax? ExplicitInterfaceSpecifier => GetRed(ref explicitInterfaceSpecifier, 4);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventDeclarationSyntax)base.Green).identifier, GetChildPosition(5), GetChildIndex(5));

        public override AccessorListSyntax? AccessorList => GetRed(ref accessorList, 6);

        public SyntaxToken SemicolonToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken semicolonToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventDeclarationSyntax)base.Green).semicolonToken;
                if (semicolonToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, semicolonToken, GetChildPosition(7), GetChildIndex(7));
            }
        }

        public EventDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax accessorList)
        {
            return Update(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, SemicolonToken);
        }

        public EventDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier, SyntaxToken identifier, SyntaxToken semicolonToken)
        {
            return Update(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, AccessorList, semicolonToken);
        }

        internal EventDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref type, 3),
                4 => GetRed(ref explicitInterfaceSpecifier, 4),
                6 => GetRed(ref accessorList, 6),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => type,
                4 => explicitInterfaceSpecifier,
                6 => accessorList,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEventDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEventDeclaration(this);

        public EventDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, TypeSyntax type, ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier, SyntaxToken identifier, AccessorListSyntax? accessorList, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || eventKeyword != EventKeyword || type != Type || explicitInterfaceSpecifier != ExplicitInterfaceSpecifier || identifier != Identifier || accessorList != AccessorList || semicolonToken != SemicolonToken)
            {
                EventDeclarationSyntax eventDeclarationSyntax = SyntaxFactory.EventDeclaration(attributeLists, modifiers, eventKeyword, type, explicitInterfaceSpecifier, identifier, accessorList, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return eventDeclarationSyntax;
                }
                return eventDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new EventDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, EventKeyword, Type, ExplicitInterfaceSpecifier, Identifier, AccessorList, SemicolonToken);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new EventDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, EventKeyword, Type, ExplicitInterfaceSpecifier, Identifier, AccessorList, SemicolonToken);
        }

        public EventDeclarationSyntax WithEventKeyword(SyntaxToken eventKeyword)
        {
            return Update(AttributeLists, Modifiers, eventKeyword, Type, ExplicitInterfaceSpecifier, Identifier, AccessorList, SemicolonToken);
        }

        internal override BasePropertyDeclarationSyntax WithTypeCore(TypeSyntax type)
        {
            return WithType(type);
        }

        public new EventDeclarationSyntax WithType(TypeSyntax type)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, type, ExplicitInterfaceSpecifier, Identifier, AccessorList, SemicolonToken);
        }

        internal override BasePropertyDeclarationSyntax WithExplicitInterfaceSpecifierCore(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier)
        {
            return WithExplicitInterfaceSpecifier(explicitInterfaceSpecifier);
        }

        public new EventDeclarationSyntax WithExplicitInterfaceSpecifier(ExplicitInterfaceSpecifierSyntax? explicitInterfaceSpecifier)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, Type, explicitInterfaceSpecifier, Identifier, AccessorList, SemicolonToken);
        }

        public EventDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, Type, ExplicitInterfaceSpecifier, identifier, AccessorList, SemicolonToken);
        }

        internal override BasePropertyDeclarationSyntax WithAccessorListCore(AccessorListSyntax? accessorList)
        {
            return WithAccessorList(accessorList);
        }

        public new EventDeclarationSyntax WithAccessorList(AccessorListSyntax? accessorList)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, Type, ExplicitInterfaceSpecifier, Identifier, accessorList, SemicolonToken);
        }

        public EventDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, Type, ExplicitInterfaceSpecifier, Identifier, AccessorList, semicolonToken);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new EventDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new EventDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        internal override BasePropertyDeclarationSyntax AddAccessorListAccessorsCore(params AccessorDeclarationSyntax[] items)
        {
            return AddAccessorListAccessors(items);
        }

        public new EventDeclarationSyntax AddAccessorListAccessors(params AccessorDeclarationSyntax[] items)
        {
            AccessorListSyntax accessorListSyntax = AccessorList ?? SyntaxFactory.AccessorList();
            return WithAccessorList(accessorListSyntax.WithAccessors(accessorListSyntax.Accessors.AddRange(items)));
        }
    }
}
