using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class InstanceErrorTypeSymbol : ErrorTypeSymbol
	{
		private sealed class ErrorTypeParameterSymbol : TypeParameterSymbol
		{
			private readonly InstanceErrorTypeSymbol _container;

			private readonly int _ordinal;

			public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

			public override string Name => string.Empty;

			internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics => ImmutableArray<TypeSymbol>.Empty;

			public override Symbol ContainingSymbol => _container;

			public override bool HasConstructorConstraint => false;

			public override bool HasReferenceTypeConstraint => false;

			public override bool HasValueTypeConstraint => false;

			public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

			public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

			public override int Ordinal => _ordinal;

			public override VarianceKind Variance => VarianceKind.None;

			public ErrorTypeParameterSymbol(InstanceErrorTypeSymbol container, int ordinal)
			{
				_container = container;
				_ordinal = ordinal;
			}

			internal override ImmutableArray<TypeParameterConstraint> GetConstraints()
			{
				return ImmutableArray<TypeParameterConstraint>.Empty;
			}

			public override int GetHashCode()
			{
				return Hash.Combine(_container.GetHashCode(), _ordinal);
			}

			public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
			{
				if ((object)obj == null)
				{
					return false;
				}
				if ((object)obj == this)
				{
					return true;
				}
				return obj is ErrorTypeParameterSymbol errorTypeParameterSymbol && errorTypeParameterSymbol._ordinal == _ordinal && errorTypeParameterSymbol._container.Equals(_container, comparison);
			}

			internal override void EnsureAllConstraintsAreResolved()
			{
			}
		}

		protected readonly int _arity;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		public sealed override int Arity => _arity;

		internal override bool CanConstruct => _arity > 0;

		internal override TypeSubstitution TypeSubstitution => null;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				if (_lazyTypeParameters.IsDefault)
				{
					TypeParameterSymbol[] array = new TypeParameterSymbol[_arity - 1 + 1];
					int num = _arity - 1;
					for (int i = 0; i <= num; i++)
					{
						array[i] = new ErrorTypeParameterSymbol(this, i);
					}
					ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeParameters, array.AsImmutableOrNull(), default(ImmutableArray<TypeParameterSymbol>));
				}
				return _lazyTypeParameters;
			}
		}

		internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => StaticCast<TypeSymbol>.From(TypeParameters);

		internal sealed override bool HasTypeArgumentsCustomModifiers => false;

		internal InstanceErrorTypeSymbol(int arity)
		{
			_arity = arity;
			if (arity == 0)
			{
				_lazyTypeParameters = ImmutableArray<TypeParameterSymbol>.Empty;
			}
		}

		public sealed override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			CheckCanConstructAndTypeArguments(typeArguments);
			TypeSubstitution typeSubstitution = TypeSubstitution.Create(this, TypeParameters, typeArguments, allowAlphaRenamedTypeParametersAsArguments: true);
			if (typeSubstitution == null)
			{
				return this;
			}
			return new SubstitutedErrorType(ContainingSymbol, this, typeSubstitution);
		}

		internal sealed override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			return new TypeWithModifiers(InternalSubstituteTypeParametersInInstanceErrorTypeSymbol(substitution));
		}

		private NamedTypeSymbol InternalSubstituteTypeParametersInInstanceErrorTypeSymbol(TypeSubstitution substitution)
		{
			if (substitution != null)
			{
				substitution = substitution.GetSubstitutionForGenericDefinitionOrContainers(this);
			}
			if (substitution == null)
			{
				return this;
			}
			Symbol containingSymbol = ContainingSymbol;
			if (!(containingSymbol is NamedTypeSymbol namedTypeSymbol))
			{
				return new SubstitutedErrorType(containingSymbol, this, substitution);
			}
			NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)namedTypeSymbol.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly();
			if ((object)substitution.TargetGenericDefinition != this)
			{
				substitution = TypeSubstitution.Concat(this, namedTypeSymbol2.TypeSubstitution, null);
			}
			return new SubstitutedErrorType(namedTypeSymbol2, this, substitution);
		}

		public sealed override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
		{
			return GetEmptyTypeArgumentCustomModifiers(ordinal);
		}

		public abstract override int GetHashCode();

		public sealed override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			if ((object)other == this)
			{
				return true;
			}
			if ((object)other == null)
			{
				return false;
			}
			InstanceErrorTypeSymbol instanceErrorTypeSymbol = other as InstanceErrorTypeSymbol;
			if ((object)instanceErrorTypeSymbol == null && (comparison & TypeCompareKind.AllIgnoreOptionsForVB) == 0)
			{
				return false;
			}
			if (other is TupleTypeSymbol tupleTypeSymbol)
			{
				return tupleTypeSymbol.Equals(this, comparison);
			}
			if ((object)instanceErrorTypeSymbol != null)
			{
				return SpecializedEquals(instanceErrorTypeSymbol);
			}
			if (!Equals(other.OriginalDefinition))
			{
				return false;
			}
			return other.Equals(this, comparison);
		}

		protected abstract bool SpecializedEquals(InstanceErrorTypeSymbol other);
	}
}
