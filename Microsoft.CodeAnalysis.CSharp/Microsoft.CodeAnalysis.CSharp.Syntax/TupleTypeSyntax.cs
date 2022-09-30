#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TupleTypeSyntax : TypeSyntax
    {
        private SyntaxNode? elements;

        public SyntaxToken OpenParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TupleTypeSyntax)base.Green).openParenToken, base.Position, 0);

        public SeparatedSyntaxList<TupleElementSyntax> Elements
        {
            get
            {
                SyntaxNode red = GetRed(ref elements, 1);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<TupleElementSyntax>);
                }
                return new SeparatedSyntaxList<TupleElementSyntax>(red, GetChildIndex(1));
            }
        }

        public SyntaxToken CloseParenToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TupleTypeSyntax)base.Green).closeParenToken, GetChildPosition(2), GetChildIndex(2));

        internal TupleTypeSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref elements, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return elements;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTupleType(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTupleType(this);

        public TupleTypeSyntax Update(SyntaxToken openParenToken, SeparatedSyntaxList<TupleElementSyntax> elements, SyntaxToken closeParenToken)
        {
            if (openParenToken != OpenParenToken || elements != Elements || closeParenToken != CloseParenToken)
            {
                TupleTypeSyntax tupleTypeSyntax = SyntaxFactory.TupleType(openParenToken, elements, closeParenToken);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return tupleTypeSyntax;
                }
                return tupleTypeSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TupleTypeSyntax WithOpenParenToken(SyntaxToken openParenToken)
        {
            return Update(openParenToken, Elements, CloseParenToken);
        }

        public TupleTypeSyntax WithElements(SeparatedSyntaxList<TupleElementSyntax> elements)
        {
            return Update(OpenParenToken, elements, CloseParenToken);
        }

        public TupleTypeSyntax WithCloseParenToken(SyntaxToken closeParenToken)
        {
            return Update(OpenParenToken, Elements, closeParenToken);
        }

        public TupleTypeSyntax AddElements(params TupleElementSyntax[] items)
        {
            return WithElements(Elements.AddRange(items));
        }
    }
}
