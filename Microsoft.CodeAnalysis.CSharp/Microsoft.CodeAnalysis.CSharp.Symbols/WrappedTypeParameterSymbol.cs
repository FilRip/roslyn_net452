using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class WrappedTypeParameterSymbol : TypeParameterSymbol
    {
        protected readonly TypeParameterSymbol _underlyingTypeParameter;

        public TypeParameterSymbol UnderlyingTypeParameter => _underlyingTypeParameter;

        public override bool IsImplicitlyDeclared => _underlyingTypeParameter.IsImplicitlyDeclared;

        public override TypeParameterKind TypeParameterKind => _underlyingTypeParameter.TypeParameterKind;

        public override int Ordinal => _underlyingTypeParameter.Ordinal;

        public override bool HasConstructorConstraint => _underlyingTypeParameter.HasConstructorConstraint;

        public override bool HasReferenceTypeConstraint => _underlyingTypeParameter.HasReferenceTypeConstraint;

        public override bool IsReferenceTypeFromConstraintTypes
        {
            get
            {
                if (!_underlyingTypeParameter.IsReferenceTypeFromConstraintTypes)
                {
                    return TypeParameterSymbol.CalculateIsReferenceTypeFromConstraintTypes(base.ConstraintTypesNoUseSiteDiagnostics);
                }
                return true;
            }
        }

        internal override bool? ReferenceTypeConstraintIsNullable => _underlyingTypeParameter.ReferenceTypeConstraintIsNullable;

        public override bool HasNotNullConstraint => _underlyingTypeParameter.HasNotNullConstraint;

        public override bool HasUnmanagedTypeConstraint => _underlyingTypeParameter.HasUnmanagedTypeConstraint;

        public override bool HasValueTypeConstraint => _underlyingTypeParameter.HasValueTypeConstraint;

        public override bool IsValueTypeFromConstraintTypes
        {
            get
            {
                if (!_underlyingTypeParameter.IsValueTypeFromConstraintTypes)
                {
                    return TypeParameterSymbol.CalculateIsValueTypeFromConstraintTypes(base.ConstraintTypesNoUseSiteDiagnostics);
                }
                return true;
            }
        }

        public override VarianceKind Variance => _underlyingTypeParameter.Variance;

        public override ImmutableArray<Location> Locations => _underlyingTypeParameter.Locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _underlyingTypeParameter.DeclaringSyntaxReferences;

        public override string Name => _underlyingTypeParameter.Name;

        public WrappedTypeParameterSymbol(TypeParameterSymbol underlyingTypeParameter)
        {
            _underlyingTypeParameter = underlyingTypeParameter;
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return _underlyingTypeParameter.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
        }

        internal override void EnsureAllConstraintsAreResolved()
        {
            _underlyingTypeParameter.EnsureAllConstraintsAreResolved();
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return _underlyingTypeParameter.GetAttributes();
        }
    }
}
