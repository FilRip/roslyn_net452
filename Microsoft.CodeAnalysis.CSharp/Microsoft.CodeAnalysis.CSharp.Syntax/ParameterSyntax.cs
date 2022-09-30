#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class ParameterSyntax : BaseParameterSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeSyntax? type;

        private EqualsValueClauseSyntax? @default;

        internal bool IsArgList
        {
            get
            {
                if (Type == null)
                {
                    return Identifier.ContextualKind() == SyntaxKind.ArgListKeyword;
                }
                return false;
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

        public override TypeSyntax? Type => GetRed(ref type, 2);

        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.ParameterSyntax)base.Green).identifier, GetChildPosition(3), GetChildIndex(3));

        public EqualsValueClauseSyntax? Default => GetRed(ref @default, 4);

        internal ParameterSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref type, 2),
                4 => GetRed(ref @default, 4),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => type,
                4 => @default,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitParameter(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitParameter(this);

        public ParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax? type, SyntaxToken identifier, EqualsValueClauseSyntax? @default)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || type != Type || identifier != Identifier || @default != Default)
            {
                ParameterSyntax parameterSyntax = SyntaxFactory.Parameter(attributeLists, modifiers, type, identifier, @default);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return parameterSyntax;
                }
                return parameterSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override BaseParameterSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new ParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, Type, Identifier, Default);
        }

        internal override BaseParameterSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new ParameterSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, Type, Identifier, Default);
        }

        internal override BaseParameterSyntax WithTypeCore(TypeSyntax? type)
        {
            return WithType(type);
        }

        public new ParameterSyntax WithType(TypeSyntax? type)
        {
            return Update(AttributeLists, Modifiers, type, Identifier, Default);
        }

        public ParameterSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(AttributeLists, Modifiers, Type, identifier, Default);
        }

        public ParameterSyntax WithDefault(EqualsValueClauseSyntax? @default)
        {
            return Update(AttributeLists, Modifiers, Type, Identifier, @default);
        }

        internal override BaseParameterSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new ParameterSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override BaseParameterSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new ParameterSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }
    }
}
