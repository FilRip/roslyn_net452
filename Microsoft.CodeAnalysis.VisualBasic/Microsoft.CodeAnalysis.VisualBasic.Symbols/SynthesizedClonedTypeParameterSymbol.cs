using System;
using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedClonedTypeParameterSymbol : SubstitutableTypeParameterSymbol
	{
		private readonly Func<Symbol, TypeSubstitution> _typeMapFactory;

		private readonly Symbol _container;

		private readonly TypeParameterSymbol _correspondingMethodTypeParameter;

		private readonly string _name;

		private ImmutableArray<TypeSymbol> _lazyConstraints;

		public override TypeParameterKind TypeParameterKind
		{
			get
			{
				if (!(ContainingSymbol is MethodSymbol))
				{
					return TypeParameterKind.Type;
				}
				return TypeParameterKind.Method;
			}
		}

		private TypeSubstitution TypeMap => _typeMapFactory(_container);

		internal override ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics
		{
			get
			{
				if (_lazyConstraints.IsDefault)
				{
					ImmutableArray<TypeSymbol> value = TypeParameterSymbol.InternalSubstituteTypeParametersDistinct(TypeMap, _correspondingMethodTypeParameter.ConstraintTypesNoUseSiteDiagnostics);
					ImmutableInterlocked.InterlockedInitialize(ref _lazyConstraints, value);
				}
				return _lazyConstraints;
			}
		}

		public override Symbol ContainingSymbol => _container;

		public override bool HasConstructorConstraint => _correspondingMethodTypeParameter.HasConstructorConstraint;

		public override bool HasReferenceTypeConstraint => _correspondingMethodTypeParameter.HasReferenceTypeConstraint;

		public override bool HasValueTypeConstraint => _correspondingMethodTypeParameter.HasValueTypeConstraint;

		public override ImmutableArray<Location> Locations => _correspondingMethodTypeParameter.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => _correspondingMethodTypeParameter.DeclaringSyntaxReferences;

		public override int Ordinal => _correspondingMethodTypeParameter.Ordinal;

		public override VarianceKind Variance => _correspondingMethodTypeParameter.Variance;

		public override string Name => _name;

		internal static ImmutableArray<TypeParameterSymbol> MakeTypeParameters(ImmutableArray<TypeParameterSymbol> origParameters, Symbol container, Func<TypeParameterSymbol, Symbol, TypeParameterSymbol> mapFunction)
		{
			return origParameters.SelectAsArray(mapFunction, container);
		}

		internal SynthesizedClonedTypeParameterSymbol(TypeParameterSymbol correspondingMethodTypeParameter, Symbol container, string name, Func<Symbol, TypeSubstitution> typeMapFactory)
		{
			_container = container;
			_correspondingMethodTypeParameter = correspondingMethodTypeParameter;
			_name = name;
			_typeMapFactory = typeMapFactory;
		}

		internal override void EnsureAllConstraintsAreResolved()
		{
			_correspondingMethodTypeParameter.EnsureAllConstraintsAreResolved();
		}
	}
}
