#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class SingleVariableDesignationSyntax : VariableDesignationSyntax
    {
        public SyntaxToken Identifier => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.SingleVariableDesignationSyntax)base.Green).identifier, base.Position, 0);

        internal SingleVariableDesignationSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return null;
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return null;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitSingleVariableDesignation(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitSingleVariableDesignation(this);

        public SingleVariableDesignationSyntax Update(SyntaxToken identifier)
        {
            if (identifier != Identifier)
            {
                SingleVariableDesignationSyntax singleVariableDesignationSyntax = SyntaxFactory.SingleVariableDesignation(identifier);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return singleVariableDesignationSyntax;
                }
                return singleVariableDesignationSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public SingleVariableDesignationSyntax WithIdentifier(SyntaxToken identifier)
        {
            return Update(identifier);
        }
    }
}
