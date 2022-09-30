#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class DeclarationExpressionSyntax : ExpressionSyntax
    {
        private TypeSyntax? type;

        private VariableDesignationSyntax? designation;

        public TypeSyntax Type => GetRedAtZero(ref type);

        public VariableDesignationSyntax Designation => GetRed(ref designation, 1);

        internal DeclarationExpressionSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref type),
                1 => GetRed(ref designation, 1),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => type,
                1 => designation,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitDeclarationExpression(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitDeclarationExpression(this);

        public DeclarationExpressionSyntax Update(TypeSyntax type, VariableDesignationSyntax designation)
        {
            if (type != Type || designation != Designation)
            {
                DeclarationExpressionSyntax declarationExpressionSyntax = SyntaxFactory.DeclarationExpression(type, designation);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return declarationExpressionSyntax;
                }
                return declarationExpressionSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public DeclarationExpressionSyntax WithType(TypeSyntax type)
        {
            return Update(type, Designation);
        }

        public DeclarationExpressionSyntax WithDesignation(VariableDesignationSyntax designation)
        {
            return Update(Type, designation);
        }
    }
}
