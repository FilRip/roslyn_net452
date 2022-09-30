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
    public sealed class DelegateDeclarationSyntax : MemberDeclarationSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeSyntax? returnType;

        private TypeParameterListSyntax? typeParameterList;

        private ParameterListSyntax? parameterList;

        private SyntaxNode? constraintClauses;

        public int Arity
        {
            get
            {
                if (TypeParameterList != null)
                {
                    return TypeParameterList!.Parameters.Count;
                }
                return 0;
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

        public SyntaxToken DelegateKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DelegateDeclarationSyntax)base.Green).delegateKeyword, GetChildPosition(2), GetChildIndex(2));

        public TypeSyntax ReturnType => GetRed(ref returnType, 3);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DelegateDeclarationSyntax)base.Green).identifier, GetChildPosition(4), GetChildIndex(4));

        public TypeParameterListSyntax? TypeParameterList => GetRed(ref typeParameterList, 5);

        public ParameterListSyntax ParameterList => GetRed(ref parameterList, 6);

        public SyntaxList<TypeParameterConstraintClauseSyntax> ConstraintClauses => new SyntaxList<TypeParameterConstraintClauseSyntax>(GetRed(ref constraintClauses, 7));

        public SyntaxToken SemicolonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.DelegateDeclarationSyntax)base.Green).semicolonToken, GetChildPosition(8), GetChildIndex(8));

        internal DelegateDeclarationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                3 => GetRed(ref returnType, 3),
                5 => GetRed(ref typeParameterList, 5),
                6 => GetRed(ref parameterList, 6),
                7 => GetRed(ref constraintClauses, 7),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                3 => returnType,
                5 => typeParameterList,
                6 => parameterList,
                7 => constraintClauses,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDelegateDeclaration(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDelegateDeclaration(this);

        public DelegateDeclarationSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, SyntaxToken delegateKeyword, TypeSyntax returnType, SyntaxToken identifier, TypeParameterListSyntax? typeParameterList, ParameterListSyntax parameterList, SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses, SyntaxToken semicolonToken)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || delegateKeyword != DelegateKeyword || returnType != ReturnType || identifier != Identifier || typeParameterList != TypeParameterList || parameterList != ParameterList || constraintClauses != ConstraintClauses || semicolonToken != SemicolonToken)
            {
                DelegateDeclarationSyntax delegateDeclarationSyntax = SyntaxFactory.DelegateDeclaration(attributeLists, modifiers, delegateKeyword, returnType, identifier, typeParameterList, parameterList, constraintClauses, semicolonToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return delegateDeclarationSyntax;
                }
                return delegateDeclarationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override MemberDeclarationSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new DelegateDeclarationSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, DelegateKeyword, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, SemicolonToken);
        }

        internal override MemberDeclarationSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new DelegateDeclarationSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, DelegateKeyword, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithDelegateKeyword(SyntaxToken delegateKeyword)
        {
            return Update(AttributeLists, Modifiers, delegateKeyword, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithReturnType(TypeSyntax returnType)
        {
            return Update(AttributeLists, Modifiers, DelegateKeyword, returnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, DelegateKeyword, ReturnType, identifier, TypeParameterList, ParameterList, ConstraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithTypeParameterList(TypeParameterListSyntax? typeParameterList)
        {
            return Update(AttributeLists, Modifiers, DelegateKeyword, ReturnType, Identifier, typeParameterList, ParameterList, ConstraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithParameterList(ParameterListSyntax parameterList)
        {
            return Update(AttributeLists, Modifiers, DelegateKeyword, ReturnType, Identifier, TypeParameterList, parameterList, ConstraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithConstraintClauses(SyntaxList<TypeParameterConstraintClauseSyntax> constraintClauses)
        {
            return Update(AttributeLists, Modifiers, DelegateKeyword, ReturnType, Identifier, TypeParameterList, ParameterList, constraintClauses, SemicolonToken);
        }

        public DelegateDeclarationSyntax WithSemicolonToken(SyntaxToken semicolonToken)
        {
            return Update(AttributeLists, Modifiers, DelegateKeyword, ReturnType, Identifier, TypeParameterList, ParameterList, ConstraintClauses, semicolonToken);
        }

        internal override MemberDeclarationSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new DelegateDeclarationSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override MemberDeclarationSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new DelegateDeclarationSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }

        public DelegateDeclarationSyntax AddTypeParameterListParameters(params TypeParameterSyntax[] items)
        {
            TypeParameterListSyntax typeParameterListSyntax = TypeParameterList ?? SyntaxFactory.TypeParameterList();
            return WithTypeParameterList(typeParameterListSyntax.WithParameters(typeParameterListSyntax.Parameters.AddRange(items)));
        }

        public DelegateDeclarationSyntax AddParameterListParameters(params ParameterSyntax[] items)
        {
            return WithParameterList(ParameterList.WithParameters(ParameterList.Parameters.AddRange(items)));
        }

        public DelegateDeclarationSyntax AddConstraintClauses(params TypeParameterConstraintClauseSyntax[] items)
        {
            return WithConstraintClauses(ConstraintClauses.AddRange(items));
        }
    }
}
