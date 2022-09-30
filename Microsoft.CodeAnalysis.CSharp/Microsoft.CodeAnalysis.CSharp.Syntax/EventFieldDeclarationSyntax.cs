#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class EventFieldDeclarationSyntax : BaseFieldDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private VariableDeclarationSyntax? declaration;

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

        public SyntaxToken EventKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventFieldDeclarationSyntax)base.Green).eventKeyword, GetChildPosition(2), GetChildIndex(2));

        public override VariableDeclarationSyntax Declaration => GetRed(ref declaration, 3);

        public override SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.EventFieldDeclarationSyntax)base.Green).semicolonToken, GetChildPosition(4), GetChildIndex(4));

        internal EventFieldDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref declaration, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => declaration,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitEventFieldDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitEventFieldDeclaration(this);

        public EventFieldDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken eventKeyword, VariableDeclarationSyntax declaration, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || eventKeyword != EventKeyword || declaration != Declaration || semicolonToken != SemicolonToken)
            {
                EventFieldDeclarationSyntax eventFieldDeclarationSyntax = SyntaxFactory.EventFieldDeclaration(attributeLists, modifiers, eventKeyword, declaration, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return eventFieldDeclarationSyntax;
                }
                return eventFieldDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new EventFieldDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, EventKeyword, Declaration, SemicolonToken);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new EventFieldDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, EventKeyword, Declaration, SemicolonToken);
        }

        public EventFieldDeclarationSyntax WithEventKeyword(SyntaxToken eventKeyword)
        {
            return Update(AttributeLists, Modifiers, eventKeyword, Declaration, SemicolonToken);
        }

        internal override BaseFieldDeclarationSyntax WithDeclarationCore(VariableDeclarationSyntax declaration)
        {
            return WithDeclaration(declaration);
        }

        public new EventFieldDeclarationSyntax WithDeclaration(VariableDeclarationSyntax declaration)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, declaration, SemicolonToken);
        }

        internal override BaseFieldDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken)
        {
            return WithSemicolonToken(semicolonToken);
        }

        public new EventFieldDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, EventKeyword, Declaration, semicolonToken);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new EventFieldDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new EventFieldDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        internal override BaseFieldDeclarationSyntax AddDeclarationVariablesCore(params VariableDeclaratorSyntax[] items)
        {
            return AddDeclarationVariables(items);
        }

        public new EventFieldDeclarationSyntax AddDeclarationVariables(params VariableDeclaratorSyntax[] items)
        {
            return WithDeclaration(Declaration.WithVariables(Declaration.Variables.AddRange(items)));
        }
    }
}
