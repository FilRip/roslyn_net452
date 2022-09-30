#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class AttributeSyntax : CSharpSyntaxNode
    {
        private NameSyntax? name;

        private AttributeArgumentListSyntax? argumentList;

        public NameSyntax Name => GetRedAtZero(ref name);

        public AttributeArgumentListSyntax? ArgumentList => GetRed(ref argumentList, 1);

        internal string GetErrorDisplayName()
        {
            return Name.ErrorDisplayName();
        }

        internal AttributeArgumentSyntax? GetNamedArgumentSyntax(string namedArgName)
        {
            if (argumentList != null)
            {
                SeparatedSyntaxList<AttributeArgumentSyntax>.Enumerator enumerator = argumentList!.Arguments.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AttributeArgumentSyntax current = enumerator.Current;
                    if (current.NameEquals != null && current.NameEquals!.Name.Identifier.ValueText == namedArgName)
                    {
                        return current;
                    }
                }
            }
            return null;
        }

        internal AttributeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref name),
                1 => GetRed(ref argumentList, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => name,
                1 => argumentList,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitAttribute(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitAttribute(this);

        public AttributeSyntax Update(NameSyntax name, AttributeArgumentListSyntax? argumentList)
        {
            if (name != Name || argumentList != ArgumentList)
            {
                AttributeSyntax attributeSyntax = SyntaxFactory.Attribute(name, argumentList);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return attributeSyntax;
                }
                return attributeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public AttributeSyntax WithName(NameSyntax name)
        {
            return Update(name, ArgumentList);
        }

        public AttributeSyntax WithArgumentList(AttributeArgumentListSyntax? argumentList)
        {
            return Update(Name, argumentList);
        }

        public AttributeSyntax AddArgumentListArguments(params AttributeArgumentSyntax[] items)
        {
            AttributeArgumentListSyntax attributeArgumentListSyntax = ArgumentList ?? SyntaxFactory.AttributeArgumentList();
            return WithArgumentList(attributeArgumentListSyntax.WithArguments(attributeArgumentListSyntax.Arguments.AddRange(items)));
        }
    }
}
