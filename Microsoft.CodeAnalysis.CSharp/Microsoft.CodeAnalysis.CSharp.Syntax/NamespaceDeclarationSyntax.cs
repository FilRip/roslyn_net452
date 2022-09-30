#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class NamespaceDeclarationSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private NameSyntax? name;

        private SyntaxNode? externs;

        private SyntaxNode? usings;

        private SyntaxNode? members;

        internal new Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NamespaceDeclarationSyntax Green => (Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.NamespaceDeclarationSyntax)base.Green;

        public override SyntaxList<AttributeListSyntax> AttributeLists => new SyntaxList<AttributeListSyntax>(GetRed(ref attributeLists, 0));

        public override SyntaxTokenList Modifiers
        {
            get
            {
                GreenNode slot = Green.GetSlot(1);
                if (slot == null)
                {
                    return default(SyntaxTokenList);
                }
                return new SyntaxTokenList(this, slot, GetChildPosition(1), GetChildIndex(1));
            }
        }

        public SyntaxToken NamespaceKeyword => new SyntaxToken(this, Green.namespaceKeyword, GetChildPosition(2), GetChildIndex(2));

        public NameSyntax Name => GetRed(ref name, 3);

        public SyntaxToken OpenBraceToken => new SyntaxToken(this, Green.openBraceToken, GetChildPosition(4), GetChildIndex(4));

        public SyntaxList<ExternAliasDirectiveSyntax> Externs => new SyntaxList<ExternAliasDirectiveSyntax>(GetRed(ref externs, 5));

        public SyntaxList<UsingDirectiveSyntax> Usings => new SyntaxList<UsingDirectiveSyntax>(GetRed(ref usings, 6));

        public SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref members, 7));

        public SyntaxToken CloseBraceToken => new SyntaxToken(this, Green.closeBraceToken, GetChildPosition(8), GetChildIndex(8));

        public SyntaxToken SemicolonToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken semicolonToken = Green.semicolonToken;
                if (semicolonToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, semicolonToken, GetChildPosition(9), GetChildIndex(9));
            }
        }

        public NamespaceDeclarationSyntax Update(SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken);
        }

        internal NamespaceDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref name, 3),
                5 => GetRed(ref externs, 5),
                6 => GetRed(ref usings, 6),
                7 => GetRed(ref members, 7),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => name,
                5 => externs,
                6 => usings,
                7 => members,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitNamespaceDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitNamespaceDeclaration(this);

        public NamespaceDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken namespaceKeyword, NameSyntax name, SyntaxToken openBraceToken, SyntaxList<ExternAliasDirectiveSyntax> externs, SyntaxList<UsingDirectiveSyntax> usings, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || namespaceKeyword != NamespaceKeyword || name != Name || openBraceToken != OpenBraceToken || externs != Externs || usings != Usings || members != Members || closeBraceToken != CloseBraceToken || semicolonToken != SemicolonToken)
            {
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = SyntaxFactory.NamespaceDeclaration(attributeLists, modifiers, namespaceKeyword, name, openBraceToken, externs, usings, members, closeBraceToken, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return namespaceDeclarationSyntax;
                }
                return namespaceDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new NamespaceDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, NamespaceKeyword, Name, OpenBraceToken, Externs, Usings, Members, CloseBraceToken, SemicolonToken);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new NamespaceDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, NamespaceKeyword, Name, OpenBraceToken, Externs, Usings, Members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithNamespaceKeyword(SyntaxToken namespaceKeyword)
        {
            return Update(AttributeLists, Modifiers, namespaceKeyword, Name, OpenBraceToken, Externs, Usings, Members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithName(NameSyntax name)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, name, OpenBraceToken, Externs, Usings, Members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, Name, openBraceToken, Externs, Usings, Members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithExterns(SyntaxList<ExternAliasDirectiveSyntax> externs)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, Name, OpenBraceToken, externs, Usings, Members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithUsings(SyntaxList<UsingDirectiveSyntax> usings)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, Name, OpenBraceToken, Externs, usings, Members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, Name, OpenBraceToken, Externs, Usings, members, CloseBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, Name, OpenBraceToken, Externs, Usings, Members, closeBraceToken, SemicolonToken);
        }

        public NamespaceDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, NamespaceKeyword, Name, OpenBraceToken, Externs, Usings, Members, CloseBraceToken, semicolonToken);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new NamespaceDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new NamespaceDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public NamespaceDeclarationSyntax AddExterns(params ExternAliasDirectiveSyntax[] items)
        {
            return WithExterns(Externs.AddRange(items));
        }

        public NamespaceDeclarationSyntax AddUsings(params UsingDirectiveSyntax[] items)
        {
            return WithUsings(Usings.AddRange(items));
        }

        public NamespaceDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items)
        {
            return WithMembers(Members.AddRange(items));
        }
    }
}
