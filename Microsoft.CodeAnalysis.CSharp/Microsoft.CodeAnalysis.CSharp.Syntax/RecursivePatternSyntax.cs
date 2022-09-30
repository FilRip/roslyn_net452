#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Syntax
{
    public sealed class RecursivePatternSyntax : PatternSyntax
    {
        private TypeSyntax? type;

        private PositionalPatternClauseSyntax? positionalPatternClause;

        private PropertyPatternClauseSyntax? propertyPatternClause;

        private VariableDesignationSyntax? designation;

        public TypeSyntax? Type => GetRedAtZero(ref type);

        public PositionalPatternClauseSyntax? PositionalPatternClause => GetRed(ref positionalPatternClause, 1);

        public PropertyPatternClauseSyntax? PropertyPatternClause => GetRed(ref propertyPatternClause, 2);

        public VariableDesignationSyntax? Designation => GetRed(ref designation, 3);

        internal RecursivePatternSyntax(Microsoft.CodeAnalysis.CSharp.Syntax.InternalSyntax.CSharpSyntaxNode green, SyntaxNode? parent, int position)
            : base(green, parent, position)
        {
        }

        public override SyntaxNode? GetNodeSlot(int index)
        {
            return index switch
            {
                0 => GetRedAtZero(ref type),
                1 => GetRed(ref positionalPatternClause, 1),
                2 => GetRed(ref propertyPatternClause, 2),
                3 => GetRed(ref designation, 3),
                _ => null,
            };
        }

        public override SyntaxNode? GetCachedSlot(int index)
        {
            return index switch
            {
                0 => type,
                1 => positionalPatternClause,
                2 => propertyPatternClause,
                3 => designation,
                _ => null,
            };
        }

        public override void Accept(CSharpSyntaxVisitor visitor)
        {
            visitor.VisitRecursivePattern(this);
        }

        public override TResult? Accept<TResult>(CSharpSyntaxVisitor<TResult> visitor) where TResult : default => visitor.VisitRecursivePattern(this);

        public RecursivePatternSyntax Update(TypeSyntax? type, PositionalPatternClauseSyntax? positionalPatternClause, PropertyPatternClauseSyntax? propertyPatternClause, VariableDesignationSyntax? designation)
        {
            if (type != Type || positionalPatternClause != PositionalPatternClause || propertyPatternClause != PropertyPatternClause || designation != Designation)
            {
                RecursivePatternSyntax recursivePatternSyntax = SyntaxFactory.RecursivePattern(type, positionalPatternClause, propertyPatternClause, designation);
                SyntaxAnnotation[] annotations = GetAnnotations();
                if (annotations == null || annotations.Length == 0)
                {
                    return recursivePatternSyntax;
                }
                return recursivePatternSyntax.WithAnnotations(annotations);
            }
            return this;
        }

        public RecursivePatternSyntax WithType(TypeSyntax? type)
        {
            return Update(type, PositionalPatternClause, PropertyPatternClause, Designation);
        }

        public RecursivePatternSyntax WithPositionalPatternClause(PositionalPatternClauseSyntax? positionalPatternClause)
        {
            return Update(Type, positionalPatternClause, PropertyPatternClause, Designation);
        }

        public RecursivePatternSyntax WithPropertyPatternClause(PropertyPatternClauseSyntax? propertyPatternClause)
        {
            return Update(Type, PositionalPatternClause, propertyPatternClause, Designation);
        }

        public RecursivePatternSyntax WithDesignation(VariableDesignationSyntax? designation)
        {
            return Update(Type, PositionalPatternClause, PropertyPatternClause, designation);
        }

        public RecursivePatternSyntax AddPositionalPatternClauseSubpatterns(params SubpatternSyntax[] items)
        {
            PositionalPatternClauseSyntax positionalPatternClauseSyntax = PositionalPatternClause ?? SyntaxFactory.PositionalPatternClause();
            return WithPositionalPatternClause(positionalPatternClauseSyntax.WithSubpatterns(positionalPatternClauseSyntax.Subpatterns.AddRange(items)));
        }

        public RecursivePatternSyntax AddPropertyPatternClauseSubpatterns(params SubpatternSyntax[] items)
        {
            PropertyPatternClauseSyntax propertyPatternClauseSyntax = PropertyPatternClause ?? SyntaxFactory.PropertyPatternClause();
            return WithPropertyPatternClause(propertyPatternClauseSyntax.WithSubpatterns(propertyPatternClauseSyntax.Subpatterns.AddRange(items)));
        }
    }
}
