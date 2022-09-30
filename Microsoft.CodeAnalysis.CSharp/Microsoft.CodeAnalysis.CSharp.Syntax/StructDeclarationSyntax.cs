#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

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
    public sealed class StructDeclarationSyntax : TypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeParameterListSyntax? typeParameterList;

        private BaseListSyntax? baseList;

        private SyntaxNode? constraintClauses;

        private SyntaxNode? members;

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

        public override SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StructDeclarationSyntax)base.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StructDeclarationSyntax)base.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public override TypeParameterListSyntax? TypeParameterList => GetRed(ref typeParameterList, 4);

        public override BaseListSyntax? BaseList => GetRed(ref baseList, 5);

        public override SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref constraintClauses, 6));

        public override SyntaxToken OpenBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StructDeclarationSyntax)base.Green).openBraceToken, GetChildPosition(7), GetChildIndex(7));

        public override SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref members, 8));

        public override SyntaxToken CloseBraceToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StructDeclarationSyntax)base.Green).closeBraceToken, GetChildPosition(9), GetChildIndex(9));

        public override SyntaxToken SemicolonToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken semicolonToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.StructDeclarationSyntax)base.Green).semicolonToken;
                if (semicolonToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, semicolonToken, GetChildPosition(10), GetChildIndex(10));
            }
        }

        internal StructDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                4 => GetRed(ref typeParameterList, 4),
                5 => GetRed(ref baseList, 5),
                6 => GetRed(ref constraintClauses, 6),
                8 => GetRed(ref members, 8),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                4 => typeParameterList,
                5 => baseList,
                6 => constraintClauses,
                8 => members,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitStructDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitStructDeclaration(this);

        public StructDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || keyword != Keyword || identifier != Identifier || typeParameterList != TypeParameterList || baseList != BaseList || constraintClauses != ConstraintClauses || openBraceToken != OpenBraceToken || members != Members || closeBraceToken != CloseBraceToken || semicolonToken != SemicolonToken)
            {
                StructDeclarationSyntax structDeclarationSyntax = SyntaxFactory.StructDeclaration(attributeLists, modifiers, keyword, identifier, typeParameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return structDeclarationSyntax;
                }
                return structDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new StructDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, Keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new StructDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, Keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword)
        {
            return WithKeyword(keyword);
        }

        public new StructDeclarationSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(AttributeLists, Modifiers, keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier)
        {
            return WithIdentifier(identifier);
        }

        public new StructDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, Keyword, identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList)
        {
            return WithTypeParameterList(typeParameterList);
        }

        public new StructDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, typeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList)
        {
            return WithBaseList(baseList);
        }

        public new StructDeclarationSyntax WithBaseList(BaseListSyntax? baseList)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, TypeParameterList, baseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return WithConstraintClauses(constraintClauses);
        }

        public new StructDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, TypeParameterList, BaseList, constraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken)
        {
            return WithOpenBraceToken(openBraceToken);
        }

        public new StructDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, openBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members)
        {
            return WithMembers(members);
        }

        public new StructDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken)
        {
            return WithCloseBraceToken(closeBraceToken);
        }

        public new StructDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, closeBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken)
        {
            return WithSemicolonToken(semicolonToken);
        }

        public new StructDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, Keyword, Identifier, TypeParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, semicolonToken);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new StructDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new StructDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        internal override TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items)
        {
            return AddTypeParameterListParameters(items);
        }

        public new StructDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            TypeParameterListSyntax typeParameterListSyntax = TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterListSyntax.WithParameters(typeParameterListSyntax.Parameters.AddRange(items)));
        }

        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items)
        {
            return AddBaseListTypes(items);
        }

        public new StructDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            BaseListSyntax baseListSyntax = BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseListSyntax.WithTypes(baseListSyntax.Types.AddRange(items)));
        }

        internal override TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items)
        {
            return AddConstraintClauses(items);
        }

        public new StructDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items)
        {
            return WithConstraintClauses(ConstraintClauses.AddRange(items));
        }

        internal override TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items)
        {
            return AddMembers(items);
        }

        public new StructDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items)
        {
            return WithMembers(Members.AddRange(items));
        }
    }
}
