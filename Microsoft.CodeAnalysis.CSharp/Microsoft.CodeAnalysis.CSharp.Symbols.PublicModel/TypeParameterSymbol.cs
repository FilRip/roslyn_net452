using System;
using System.Collections.Immutable;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel
{
    internal sealed class TypeParameterSymbol : TypeSymbol, ITypeParameterSymbol, ITypeSymbol, INamespaceOrTypeSymbol, ISymbol, IEquatable<ISymbol?>
    {
        private readonly Microsoft.CodeAnalysis.CSharp.Symbols.TypeParameterSymbol _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.TypeSymbol UnderlyingTypeSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbol UnderlyingSymbol => _underlying;

        internal override Microsoft.CodeAnalysis.CSharp.Symbols.NamespaceOrTypeSymbol UnderlyingNamespaceOrTypeSymbol => _underlying;

        internal Microsoft.CodeAnalysis.CSharp.Symbols.TypeParameterSymbol UnderlyingTypeParameterSymbol => _underlying;

        Microsoft.CodeAnalysis.NullableAnnotation ITypeParameterSymbol.ReferenceTypeConstraintNullableAnnotation
        {
            get
            {
                bool? referenceTypeConstraintIsNullable = _underlying.ReferenceTypeConstraintIsNullable;
                if (referenceTypeConstraintIsNullable.HasValue)
                {
                    if (!referenceTypeConstraintIsNullable.GetValueOrDefault())
                    {
                        if (!_underlying.HasReferenceTypeConstraint)
                        {
                            return Microsoft.CodeAnalysis.NullableAnnotation.None;
                        }
                        return Microsoft.CodeAnalysis.NullableAnnotation.NotAnnotated;
                    }
                    return Microsoft.CodeAnalysis.NullableAnnotation.Annotated;
                }
                return Microsoft.CodeAnalysis.NullableAnnotation.None;
            }
        }

        TypeParameterKind ITypeParameterSymbol.TypeParameterKind => _underlying.TypeParameterKind;

        IMethodSymbol ITypeParameterSymbol.DeclaringMethod => _underlying.DeclaringMethod.GetPublicSymbol();

        INamedTypeSymbol ITypeParameterSymbol.DeclaringType => _underlying.DeclaringType.GetPublicSymbol();

        ImmutableArray<ITypeSymbol> ITypeParameterSymbol.ConstraintTypes => _underlying.ConstraintTypesNoUseSiteDiagnostics.GetPublicSymbols();

        ImmutableArray<Microsoft.CodeAnalysis.NullableAnnotation> ITypeParameterSymbol.ConstraintNullableAnnotations => _underlying.ConstraintTypesNoUseSiteDiagnostics.ToPublicAnnotations();

        ITypeParameterSymbol ITypeParameterSymbol.OriginalDefinition => _underlying.OriginalDefinition.GetPublicSymbol();

        ITypeParameterSymbol ITypeParameterSymbol.ReducedFrom => _underlying.ReducedFrom.GetPublicSymbol();

        int ITypeParameterSymbol.Ordinal => _underlying.Ordinal;

        VarianceKind ITypeParameterSymbol.Variance => _underlying.Variance;

        bool ITypeParameterSymbol.HasReferenceTypeConstraint => _underlying.HasReferenceTypeConstraint;

        bool ITypeParameterSymbol.HasValueTypeConstraint => _underlying.HasValueTypeConstraint;

        bool ITypeParameterSymbol.HasUnmanagedTypeConstraint => _underlying.HasUnmanagedTypeConstraint;

        bool ITypeParameterSymbol.HasNotNullConstraint => _underlying.HasNotNullConstraint;

        bool ITypeParameterSymbol.HasConstructorConstraint => _underlying.HasConstructorConstraint;

        public TypeParameterSymbol(Microsoft.CodeAnalysis.CSharp.Symbols.TypeParameterSymbol underlying, Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
            : base(nullableAnnotation)
        {
            _underlying = underlying;
        }

        protected override ITypeSymbol WithNullableAnnotation(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new TypeParameterSymbol(_underlying, nullableAnnotation);
        }

        protected override void Accept(SymbolVisitor visitor)
        {
            visitor.VisitTypeParameter(this);
        }

        protected override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
        {
            return visitor.VisitTypeParameter(this);
        }
    }
}
