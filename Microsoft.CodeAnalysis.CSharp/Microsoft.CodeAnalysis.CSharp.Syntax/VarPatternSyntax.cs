#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class VarPatternSyntax : PatternSyntax
    {
        private VariableDesignationSyntax? designation;

        public SyntaxToken VarKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.VarPatternSyntax)base.Green).varKeyword, base.Position, 0);

        public VariableDesignationSyntax Designation => GetRed(ref designation, 1);

        internal VarPatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return GetRed(ref designation, 1);
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            if (index != 1)
            {
                return null;
            }
            return designation;
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitVarPattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitVarPattern(this);

        public VarPatternSyntax Update(SyntaxToken varKeyword, VariableDesignationSyntax designation)
        {
            if (varKeyword != VarKeyword || designation != Designation)
            {
                VarPatternSyntax varPatternSyntax = SyntaxFactory.VarPattern(varKeyword, designation);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return varPatternSyntax;
                }
                return varPatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public VarPatternSyntax WithVarKeyword(SyntaxToken varKeyword)
        {
            return Update(varKeyword, Designation);
        }

        public VarPatternSyntax WithDesignation(VariableDesignationSyntax designation)
        {
            return Update(VarKeyword, designation);
        }
    }
}
