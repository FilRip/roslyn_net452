using System;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class FunctionPointerParameterSyntax : BaseParameterSyntax
    {
        private SyntaxNode? attributeLists;

        private TypeSyntax? type;

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

        public override TypeSyntax Type => GetRed(ref type, 2);

        internal FunctionPointerParameterSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref attributeLists),
                2 => GetRed(ref type, 2),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => attributeLists,
                2 => type,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitFunctionPointerParameter(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitFunctionPointerParameter(this);

        public FunctionPointerParameterSyntax Update(SyntaxList<AttributeListSyntax> attributeLists, SyntaxTokenList modifiers, TypeSyntax type)
        {
            if (attributeLists != AttributeLists || modifiers != Modifiers || type != Type)
            {
                FunctionPointerParameterSyntax functionPointerParameterSyntax = SyntaxFactory.FunctionPointerParameter(attributeLists, modifiers, type);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return functionPointerParameterSyntax;
                }
                return functionPointerParameterSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        internal override BaseParameterSyntax WithAttributeListsCore(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return WithAttributeLists(attributeLists);
        }

        public new FunctionPointerParameterSyntax WithAttributeLists(SyntaxList<AttributeListSyntax> attributeLists)
        {
            return Update(attributeLists, Modifiers, Type);
        }

        internal override BaseParameterSyntax WithModifiersCore(SyntaxTokenList modifiers)
        {
            return WithModifiers(modifiers);
        }

        public new FunctionPointerParameterSyntax WithModifiers(SyntaxTokenList modifiers)
        {
            return Update(AttributeLists, modifiers, Type);
        }

        internal override BaseParameterSyntax WithTypeCore(TypeSyntax? type)
        {
            return WithType(type ?? throw new ArgumentNullException("type"));
        }

        public new FunctionPointerParameterSyntax WithType(TypeSyntax type)
        {
            return Update(AttributeLists, Modifiers, type);
        }

        internal override BaseParameterSyntax AddAttributeListsCore(params AttributeListSyntax[] items)
        {
            return AddAttributeLists(items);
        }

        public new FunctionPointerParameterSyntax AddAttributeLists(params AttributeListSyntax[] items)
        {
            return WithAttributeLists(AttributeLists.AddRange(items));
        }

        internal override BaseParameterSyntax AddModifiersCore(params SyntaxToken[] items)
        {
            return AddModifiers(items);
        }

        public new FunctionPointerParameterSyntax AddModifiers(params SyntaxToken[] items)
        {
            return WithModifiers(Modifiers.AddRange(items));
        }
    }
}
