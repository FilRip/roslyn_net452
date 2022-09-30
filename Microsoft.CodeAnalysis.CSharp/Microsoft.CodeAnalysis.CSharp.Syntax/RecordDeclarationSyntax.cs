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
    public sealed class RecordDeclarationSyntax : TypeDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeParameterListSyntax? typeParameterList;

        private ParameterListSyntax? parameterList;

        private BaseListSyntax? baseList;

        private SyntaxNode? constraintClauses;

        private SyntaxNode? members;

        internal PrimaryConstructorBaseTypeSyntax? PrimaryConstructorBaseTypeIfClass
        {
            get
            {
                if (Kind() == SyntaxKind.RecordStructDeclaration)
                {
                    return null;
                }
                return BaseList?.Types.FirstOrDefault() as PrimaryConstructorBaseTypeSyntax;
            }
        }

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

        public override SyntaxToken Keyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RecordDeclarationSyntax)base.Green).keyword, GetChildPosition(2), GetChildIndex(2));

        public SyntaxToken ClassOrStructKeyword
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken classOrStructKeyword = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RecordDeclarationSyntax)base.Green).classOrStructKeyword;
                if (classOrStructKeyword == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, classOrStructKeyword, GetChildPosition(3), GetChildIndex(3));
            }
        }

        public override SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RecordDeclarationSyntax)base.Green).identifier, GetChildPosition(4), GetChildIndex(4));

        public override TypeParameterListSyntax? TypeParameterList => GetRed(ref typeParameterList, 5);

        public ParameterListSyntax? ParameterList => GetRed(ref parameterList, 6);

        public override BaseListSyntax? BaseList => GetRed(ref baseList, 7);

        public override SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref constraintClauses, 8));

        public override SyntaxToken OpenBraceToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken openBraceToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RecordDeclarationSyntax)base.Green).openBraceToken;
                if (openBraceToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, openBraceToken, GetChildPosition(9), GetChildIndex(9));
            }
        }

        public override SyntaxList<MemberDeclarationSyntax> Members => new SyntaxList<MemberDeclarationSyntax>(GetRed(ref members, 10));

        public override SyntaxToken CloseBraceToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken closeBraceToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RecordDeclarationSyntax)base.Green).closeBraceToken;
                if (closeBraceToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, closeBraceToken, GetChildPosition(11), GetChildIndex(11));
            }
        }

        public override SyntaxToken SemicolonToken
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken semicolonToken = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.RecordDeclarationSyntax)base.Green).semicolonToken;
                if (semicolonToken == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, semicolonToken, GetChildPosition(12), GetChildIndex(12));
            }
        }

        public RecordDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            return Update(attributeLists, modifiers, keyword, ClassOrStructKeyword, identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
        }

        internal RecordDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                5 => GetRed(ref typeParameterList, 5),
                6 => GetRed(ref parameterList, 6),
                7 => GetRed(ref baseList, 7),
                8 => GetRed(ref constraintClauses, 8),
                10 => GetRed(ref members, 10),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                5 => typeParameterList,
                6 => parameterList,
                7 => baseList,
                8 => constraintClauses,
                10 => members,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRecordDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRecordDeclaration(this);

        public RecordDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken keyword, SyntaxToken classOrStructKeyword, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax? parameterList, BaseListSyntax? baseList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken openBraceToken, SyntaxList<MemberDeclarationSyntax> members, SyntaxToken closeBraceToken, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || keyword != Keyword || classOrStructKeyword != ClassOrStructKeyword || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || baseList != BaseList || constraintClauses != ConstraintClauses || openBraceToken != OpenBraceToken || members != Members || closeBraceToken != CloseBraceToken || semicolonToken != SemicolonToken)
            {
                RecordDeclarationSyntax recordDeclarationSyntax = SyntaxFactory.RecordDeclaration(Kind(), attributeLists, modifiers, keyword, classOrStructKeyword, identifier, typeParameterList, parameterList, baseList, constraintClauses, openBraceToken, members, closeBraceToken, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return recordDeclarationSyntax;
                }
                return recordDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new RecordDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new RecordDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithKeywordCore(SyntaxToken keyword)
        {
            return WithKeyword(keyword);
        }

        public new RecordDeclarationSyntax WithKeyword(SyntaxToken keyword)
        {
            return Update(AttributeLists, Modifiers, keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        public RecordDeclarationSyntax WithClassOrStructKeyword(SyntaxToken classOrStructKeyword)
        {
            return Update(AttributeLists, Modifiers, Keyword, classOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithIdentifierCore(SyntaxToken identifier)
        {
            return WithIdentifier(identifier);
        }

        public new RecordDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithTypeParameterListCore(TypeParameterListSyntax? typeParameterList)
        {
            return WithTypeParameterList(typeParameterList);
        }

        public new RecordDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, typeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        public RecordDeclarationSyntax WithParameterList(ParameterListSyntax? parameterList)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, parameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithBaseListCore(BaseListSyntax? baseList)
        {
            return WithBaseList(baseList);
        }

        public new RecordDeclarationSyntax WithBaseList(BaseListSyntax? baseList)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, baseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithConstraintClausesCore(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return WithConstraintClauses(constraintClauses);
        }

        public new RecordDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, constraintClauses, OpenBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithOpenBraceTokenCore(SyntaxToken openBraceToken)
        {
            return WithOpenBraceToken(openBraceToken);
        }

        public new RecordDeclarationSyntax WithOpenBraceToken(SyntaxToken openBraceToken)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, openBraceToken, Members, CloseBraceToken, SemicolonToken);
        }

        internal override TypeDeclarationSyntax WithMembersCore(SyntaxList<MemberDeclarationSyntax> members)
        {
            return WithMembers(members);
        }

        public new RecordDeclarationSyntax WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, members, CloseBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithCloseBraceTokenCore(SyntaxToken closeBraceToken)
        {
            return WithCloseBraceToken(closeBraceToken);
        }

        public new RecordDeclarationSyntax WithCloseBraceToken(SyntaxToken closeBraceToken)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, closeBraceToken, SemicolonToken);
        }

        internal override BaseTypeDeclarationSyntax WithSemicolonTokenCore(SyntaxToken semicolonToken)
        {
            return WithSemicolonToken(semicolonToken);
        }

        public new RecordDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, Keyword, ClassOrStructKeyword, Identifier, TypeParameterList, ParameterList, BaseList, ConstraintClauses, OpenBraceToken, Members, CloseBraceToken, semicolonToken);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new RecordDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new RecordDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        internal override TypeDeclarationSyntax AddTypeParameterListParametersCore(params TypeParameterSyntax[] items)
        {
            return AddTypeParameterListParameters(items);
        }

        public new RecordDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            TypeParameterListSyntax typeParameterListSyntax = TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterListSyntax.WithParameters(typeParameterListSyntax.Parameters.AddRange(items)));
        }

        public RecordDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            ParameterListSyntax parameterListSyntax = ParameterList ?? SyntaxFactory.ParameterList();
            return WithParameterList(parameterListSyntax.WithParameters(parameterListSyntax.Parameters.AddRange(items)));
        }

        internal override BaseTypeDeclarationSyntax AddBaseListTypesCore(params BaseTypeSyntax[] items)
        {
            return AddBaseListTypes(items);
        }

        public new RecordDeclarationSyntax AddBaseListTypes(params BaseTypeSyntax[] items)
        {
            BaseListSyntax baseListSyntax = BaseList ?? SyntaxFactory.BaseList();
            return WithBaseList(baseListSyntax.WithTypes(baseListSyntax.Types.AddRange(items)));
        }

        internal override TypeDeclarationSyntax AddConstraintClausesCore(params TypeParameterConstraintClauseSyntax[] items)
        {
            return AddConstraintClauses(items);
        }

        public new RecordDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items)
        {
            return WithConstraintClauses(ConstraintClauses.AddRange(items));
        }

        internal override TypeDeclarationSyntax AddMembersCore(params MemberDeclarationSyntax[] items)
        {
            return AddMembers(items);
        }

        public new RecordDeclarationSyntax AddMembers(params MemberDeclarationSyntax[] items)
        {
            return WithMembers(Members.AddRange(items));
        }
    }
}
