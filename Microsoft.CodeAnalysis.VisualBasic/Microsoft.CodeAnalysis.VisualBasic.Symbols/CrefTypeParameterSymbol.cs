using System.Collections.Immutable;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class CrefTypeParameterSymbol : TypeParameterSymbol
	{
		private readonly int _ordinal;

		private readonly string _name;

		private readonly SyntaxReference _syntaxReference;

		public override int Ordinal => _ordinal;

		public override string Name => _name;

		public override bool HasConstructorConstraint => false;

		public override bool HasReferenceTypeConstraint => false;

		public override bool HasValueTypeConstraint => false;

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

		public override Symbol ContainingSymbol => null;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray.Create(_syntaxReference);

		public override ImmutableArray<Location> Locations => ImmutableArray.Create(_syntaxReference.GetLocation());

		public override TypeParameterKind TypeParameterKind => TypeParameterKind.Cref;

		public override VarianceKind Variance => VarianceKind.None;

		public CrefTypeParameterSymbol(int ordinal, string name, TypeSyntax syntax)
		{
			_ordinal = ordinal;
			_name = name;
			_syntaxReference = syntax.GetReference();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal override ImmutableArray<TypeParameterConstraint> GetConstraints()
		{
			return ImmutableArray<TypeParameterConstraint>.Empty;
		}

		internal override void ResolveConstraints(ConsList<TypeParameterSymbol> inProgress)
		{
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
		}

		public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			return Equals(other as CrefTypeParameterSymbol);
		}

		public bool Equals(CrefTypeParameterSymbol other)
		{
			if ((object)this == other)
			{
				return true;
			}
			if ((object)other == null)
			{
				return false;
			}
			return EmbeddedOperators.CompareString(_name, other._name, TextCompare: false) == 0 && _ordinal == other._ordinal && _syntaxReference.GetSyntax().Equals(other._syntaxReference.GetSyntax());
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_name, _ordinal);
		}
	}
}
