#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class TypeParameterConstraintClauseSyntax : CSharpSyntaxNode
    {
        private IdentifierNameSyntax? name;

        private SyntaxNode? constraints;

        public SyntaxToken WhereKeyword => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)base.Green).whereKeyword, base.Position, 0);

        public IdentifierNameSyntax Name => GetRed(ref name, 1);

        public SyntaxToken ColonToken => new SyntaxToken(this, ((Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.TypeParameterConstraintClauseSyntax)base.Green).colonToken, GetChildPosition(2), GetChildIndex(2));

        public SeparatedSyntaxList<TypeParameterConstraintSyntax> Constraints
        {
            get
            {
                SyntaxNode red = GetRed(ref constraints, 3);
                if (red == null)
                {
                    return default(SeparatedSyntaxList<TypeParameterConstraintSyntax>);
                }
                return new SeparatedSyntaxList<TypeParameterConstraintSyntax>(red, GetChildIndex(3));
            }
        }

        internal TypeParameterConstraintClauseSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                1 => GetRed(ref name, 1),
                3 => GetRed(ref constraints, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                1 => name,
                3 => constraints,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitTypeParameterConstraintClause(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitTypeParameterConstraintClause(this);

        public TypeParameterConstraintClauseSyntax Update(SyntaxToken whereKeyword, IdentifierNameSyntax name, SyntaxToken colonToken, SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            if (whereKeyword != WhereKeyword || name != Name || colonToken != ColonToken || constraints != Constraints)
            {
                TypeParameterConstraintClauseSyntax typeParameterConstraintClauseSyntax = SyntaxFactory.TypeParameterConstraintClause(whereKeyword, name, colonToken, constraints);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return typeParameterConstraintClauseSyntax;
                }
                return typeParameterConstraintClauseSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public TypeParameterConstraintClauseSyntax WithWhereKeyword(SyntaxToken whereKeyword)
        {
            return Update(whereKeyword, Name, ColonToken, Constraints);
        }

        public TypeParameterConstraintClauseSyntax WithName(IdentifierNameSyntax name)
        {
            return Update(WhereKeyword, name, ColonToken, Constraints);
        }

        public TypeParameterConstraintClauseSyntax WithColonToken(SyntaxToken colonToken)
        {
            return Update(WhereKeyword, Name, colonToken, Constraints);
        }

        public TypeParameterConstraintClauseSyntax WithConstraints(SeparatedSyntaxList<TypeParameterConstraintSyntax> constraints)
        {
            return Update(WhereKeyword, Name, ColonToken, constraints);
        }

        public TypeParameterConstraintClauseSyntax AddConstraints(params TypeParameterConstraintSyntax[] items)
        {
            return WithConstraints(Constraints.AddRange(items));
        }
    }
}
