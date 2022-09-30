#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TupleElementSyntax : CSharpSyntaxNode
    {
        private TypeSyntax? type;

        public TypeSyntax Type => GetRedAtZero(ref type);

        public SyntaxToken Identifier
        {
            get
            {
                Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SyntaxToken identifier = ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TupleElementSyntax)base.Green).identifier;
                if (identifier == null)
                {
                    return default(SyntaxToken);
                }
                return new SyntaxToken(this, identifier, GetChildPosition(1), GetChildIndex(1));
            }
        }

        internal TupleElementSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return GetRedAtZero(ref type);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 0)
            {
                return null;
            }
            return type;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTupleElement(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTupleElement(this);

        public TupleElementSyntax Update(TypeSyntax type, SyntaxToken identifier)
        {
            if (type != Type || identifier != Identifier)
            {
                TupleElementSyntax tupleElementSyntax = SyntaxFactory.TupleElement(type, identifier);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return tupleElementSyntax;
                }
                return tupleElementSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TupleElementSyntax WithType(TypeSyntax type)
        {
            return Update(type, Identifier);
        }

        public TupleElementSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(Type, identifier);
        }
    }
}
