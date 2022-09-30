using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SubstitutedErrorType : ErrorTypeSymbol
	{
		private readonly InstanceErrorTypeSymbol _fullInstanceType;

		private readonly TypeSubstitution _substitution;

		private readonly Symbol _container;

		private bool IdentitySubstitutionOnMyTypeParameters => _substitution.Pairs.Length == 0;

		public override string Name => _fullInstanceType.Name;

		internal override bool MangleName => _fullInstanceType.MangleName;

		public override string MetadataName => _fullInstanceType.MetadataName;

		public override bool IsImplicitlyDeclared => _fullInstanceType.IsImplicitlyDeclared;

		public override NamedTypeSymbol OriginalDefinition => _fullInstanceType;

		internal override DiagnosticInfo ErrorInfo => _fullInstanceType.ErrorInfo;

		private bool ConstructedFromItself
		{
			get
			{
				if (_fullInstanceType.Arity != 0)
				{
					return IdentitySubstitutionOnMyTypeParameters;
				}
				return true;
			}
		}

		public override NamedTypeSymbol ConstructedFrom
		{
			get
			{
				if (ConstructedFromItself)
				{
					return this;
				}
				if ((object)ContainingSymbol == null || ContainingSymbol.IsDefinition)
				{
					return _fullInstanceType;
				}
				TypeSubstitution parent = _substitution.Parent;
				parent = TypeSubstitution.Concat(_fullInstanceType, parent, null);
				return new SubstitutedErrorType(ContainingSymbol, _fullInstanceType, parent);
			}
		}

		internal override TypeSubstitution TypeSubstitution => _substitution;

		public override AssemblySymbol ContainingAssembly => _fullInstanceType.ContainingAssembly;

		public override int Arity => _fullInstanceType.Arity;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters => _fullInstanceType.TypeParameters;

		internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
		{
			get
			{
				if (IdentitySubstitutionOnMyTypeParameters)
				{
					return StaticCast<TypeSymbol>.From(TypeParameters);
				}
				TypeSubstitution substitution = _substitution;
				InstanceErrorTypeSymbol fullInstanceType = _fullInstanceType;
				bool hasTypeArgumentsCustomModifiers = false;
				return substitution.GetTypeArgumentsFor(fullInstanceType, out hasTypeArgumentsCustomModifiers);
			}
		}

		internal override bool HasTypeArgumentsCustomModifiers
		{
			get
			{
				if (IdentitySubstitutionOnMyTypeParameters)
				{
					return false;
				}
				return _substitution.HasTypeArgumentsCustomModifiersFor(_fullInstanceType);
			}
		}

		internal override bool CanConstruct
		{
			get
			{
				if (Arity > 0)
				{
					return IdentitySubstitutionOnMyTypeParameters;
				}
				return false;
			}
		}

		public override Symbol ContainingSymbol => _container;

		public override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
		{
			if (IdentitySubstitutionOnMyTypeParameters)
			{
				return GetEmptyTypeArgumentCustomModifiers(ordinal);
			}
			return _substitution.GetTypeArgumentsCustomModifiersFor(_fullInstanceType.TypeParameters[ordinal]);
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
		{
			return new TypeWithModifiers(InternalSubstituteTypeParametersInSubstitutedErrorType(additionalSubstitution));
		}

		private NamedTypeSymbol InternalSubstituteTypeParametersInSubstitutedErrorType(TypeSubstitution additionalSubstitution)
		{
			if (additionalSubstitution == null)
			{
				return this;
			}
			Symbol containingSymbol = ContainingSymbol;
			if (!(containingSymbol is NamedTypeSymbol namedTypeSymbol))
			{
				TypeSubstitution typeSubstitution = TypeSubstitution.AdjustForConstruct(null, _substitution, additionalSubstitution);
				if (typeSubstitution == null)
				{
					return _fullInstanceType;
				}
				if (typeSubstitution == _substitution)
				{
					return this;
				}
				return new SubstitutedErrorType(containingSymbol, _fullInstanceType, typeSubstitution);
			}
			NamedTypeSymbol namedTypeSymbol2 = (NamedTypeSymbol)namedTypeSymbol.InternalSubstituteTypeParameters(additionalSubstitution).AsTypeSymbolOnly();
			TypeSubstitution typeSubstitution2 = TypeSubstitution.AdjustForConstruct(namedTypeSymbol2.TypeSubstitution, _substitution, additionalSubstitution);
			if (typeSubstitution2 == null)
			{
				return _fullInstanceType;
			}
			if ((object)namedTypeSymbol2 == namedTypeSymbol && typeSubstitution2 == _substitution)
			{
				return this;
			}
			return new SubstitutedErrorType(namedTypeSymbol2, _fullInstanceType, typeSubstitution2);
		}

		public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			CheckCanConstructAndTypeArguments(typeArguments);
			TypeSubstitution typeSubstitution = TypeSubstitution.Create(_fullInstanceType, _fullInstanceType.TypeParameters, typeArguments, allowAlphaRenamedTypeParametersAsArguments: true);
			if (typeSubstitution == null)
			{
				return this;
			}
			return new SubstitutedErrorType(_container, _fullInstanceType, TypeSubstitution.Concat(_fullInstanceType, _substitution.Parent, typeSubstitution));
		}

		public SubstitutedErrorType(Symbol container, InstanceErrorTypeSymbol fullInstanceType, TypeSubstitution substitution)
		{
			_container = container;
			_fullInstanceType = fullInstanceType;
			_substitution = substitution;
		}

		public override int GetHashCode()
		{
			int hashCode = _fullInstanceType.GetHashCode();
			if (_substitution.WasConstructedForModifiers())
			{
				return hashCode;
			}
			hashCode = Hash.Combine(ContainingType, hashCode);
			if (!ConstructedFromItself)
			{
				ImmutableArray<TypeSymbol>.Enumerator enumerator = TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					hashCode = Hash.Combine(enumerator.Current, hashCode);
				}
			}
			return hashCode;
		}

		public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
		{
			if ((object)this == obj)
			{
				return true;
			}
			if ((object)obj == null)
			{
				return false;
			}
			if ((comparison & TypeCompareKind.AllIgnoreOptionsForVB) == 0 && !GetType().Equals(obj.GetType()))
			{
				return false;
			}
			if (obj is TupleTypeSymbol tupleTypeSymbol)
			{
				return tupleTypeSymbol.Equals(this, comparison);
			}
			if (!_fullInstanceType.Equals(obj.OriginalDefinition))
			{
				return false;
			}
			NamedTypeSymbol containingType = ContainingType;
			if ((object)containingType != null && !containingType.Equals(obj.ContainingType, comparison))
			{
				return false;
			}
			ErrorTypeSymbol errorTypeSymbol = (ErrorTypeSymbol)obj;
			if (ConstructedFromItself && (object)errorTypeSymbol == errorTypeSymbol.ConstructedFrom)
			{
				return true;
			}
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = TypeArgumentsNoUseSiteDiagnostics;
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics2 = errorTypeSymbol.TypeArgumentsNoUseSiteDiagnostics;
			int num = typeArgumentsNoUseSiteDiagnostics.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				if (!typeArgumentsNoUseSiteDiagnostics[i].Equals(typeArgumentsNoUseSiteDiagnostics2[i], comparison))
				{
					return false;
				}
			}
			if ((comparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0 && !TypeSymbolExtensions.HasSameTypeArgumentCustomModifiers(this, errorTypeSymbol))
			{
				return false;
			}
			return true;
		}
	}
}
