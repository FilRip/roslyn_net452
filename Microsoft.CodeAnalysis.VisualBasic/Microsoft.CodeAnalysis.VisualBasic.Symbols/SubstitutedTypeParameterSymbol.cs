using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SubstitutedTypeParameterSymbol : TypeParameterSymbol
	{
		private Symbol _containingSymbol;

		private readonly TypeParameterSymbol _originalDefinition;

		public override TypeParameterKind TypeParameterKind => _originalDefinition.TypeParameterKind;

		public override string Name => _originalDefinition.Name;

		public override string MetadataName => _originalDefinition.MetadataName;

		public override TypeParameterSymbol OriginalDefinition => _originalDefinition;

		public override TypeParameterSymbol ReducedFrom => _originalDefinition.ReducedFrom;

		private TypeSubstitution TypeSubstitution
		{
			get
			{
				if (_containingSymbol.Kind != SymbolKind.Method)
				{
					return ((NamedTypeSymbol)_containingSymbol).TypeSubstitution;
				}
				return ((SubstitutedMethodSymbol)_containingSymbol).TypeSubstitution;
			}
		}

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics => TypeParameterSymbol.InternalSubstituteTypeParametersDistinct(TypeSubstitution, _originalDefinition.ConstraintTypesNoUseSiteDiagnostics);

		public override Symbol ContainingSymbol => _containingSymbol;

		public override bool HasConstructorConstraint => _originalDefinition.HasConstructorConstraint;

		public override bool HasReferenceTypeConstraint => _originalDefinition.HasReferenceTypeConstraint;

		public override bool HasValueTypeConstraint => _originalDefinition.HasValueTypeConstraint;

		public override bool IsImplicitlyDeclared => _originalDefinition.IsImplicitlyDeclared;

		public override ImmutableArray<Location> Locations => _originalDefinition.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _originalDefinition.DeclaringSyntaxReferences;

		public override int Ordinal => _originalDefinition.Ordinal;

		public override VarianceKind Variance => _originalDefinition.Variance;

		public SubstitutedTypeParameterSymbol(TypeParameterSymbol originalDefinition)
		{
			_originalDefinition = originalDefinition;
		}

		public void SetContainingSymbol(Symbol container)
		{
			_containingSymbol = container;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _originalDefinition.GetAttributes();
		}

		public override int GetHashCode()
		{
			if (_containingSymbol is SubstitutedNamedType substitutedNamedType && substitutedNamedType.TypeSubstitution.WasConstructedForModifiers())
			{
				return _originalDefinition.GetHashCode();
			}
			return Hash.Combine(Ordinal.GetHashCode(), _containingSymbol.GetHashCode());
		}

		public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			return Equals(other as TypeParameterSymbol, comparison);
		}

		private bool Equals(TypeParameterSymbol other, TypeCompareKind comparison)
		{
			if ((object)this == other)
			{
				return true;
			}
			return (object)other != null && OriginalDefinition.Equals(other.OriginalDefinition) && ContainingSymbol.Equals(other.ContainingSymbol, comparison);
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			if (substitution != null)
			{
				if ((object)substitution.TargetGenericDefinition == _containingSymbol)
				{
					return substitution.GetSubstitutionFor(this);
				}
				throw ExceptionUtilities.Unreachable;
			}
			return new TypeWithModifiers(this);
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
			_originalDefinition.EnsureAllConstraintsAreResolved();
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return _originalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}
	}
}
