using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class InstanceTypeSymbol : NamedTypeSymbol
	{
		internal sealed override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
		{
			get
			{
				if (Arity > 0)
				{
					return StaticCast<TypeSymbol>.From(TypeParameters);
				}
				return ImmutableArray<TypeSymbol>.Empty;
			}
		}

		internal sealed override bool HasTypeArgumentsCustomModifiers => false;

		internal override bool CanConstruct => Arity > 0;

		internal override TypeSubstitution TypeSubstitution => null;

		public override NamedTypeSymbol ConstructedFrom => this;

		public sealed override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
		{
			return GetEmptyTypeArgumentCustomModifiers(ordinal);
		}

		public sealed override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			CheckCanConstructAndTypeArguments(typeArguments);
			TypeSubstitution typeSubstitution = TypeSubstitution.Create(this, TypeParameters, typeArguments, allowAlphaRenamedTypeParametersAsArguments: true);
			if (typeSubstitution == null)
			{
				return this;
			}
			return new SubstitutedNamedType.ConstructedInstanceType(typeSubstitution);
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			return new TypeWithModifiers(SubstituteTypeParametersInNamedType(substitution));
		}

		private NamedTypeSymbol SubstituteTypeParametersInNamedType(TypeSubstitution substitution)
		{
			if (substitution != null)
			{
				substitution = substitution.GetSubstitutionForGenericDefinitionOrContainers(this);
			}
			if (substitution == null)
			{
				return this;
			}
			NamedTypeSymbol container;
			if ((object)substitution.TargetGenericDefinition == this)
			{
				if (substitution.Parent == null)
				{
					return new SubstitutedNamedType.ConstructedInstanceType(substitution);
				}
				container = (NamedTypeSymbol)ContainingType.InternalSubstituteTypeParameters(substitution.Parent).AsTypeSymbolOnly();
			}
			else
			{
				container = (NamedTypeSymbol)ContainingType.InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly();
			}
			if (Arity == 0)
			{
				return SubstitutedNamedType.SpecializedNonGenericType.Create(container, this, substitution);
			}
			SubstitutedNamedType.SpecializedGenericType specializedGenericType = SubstitutedNamedType.SpecializedGenericType.Create(container, this);
			if ((object)substitution.TargetGenericDefinition == this)
			{
				return new SubstitutedNamedType.ConstructedSpecializedGenericType(specializedGenericType, substitution);
			}
			return specializedGenericType;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			if ((object)other == this)
			{
				return true;
			}
			if ((object)other == null || (comparison & TypeCompareKind.AllIgnoreOptionsForVB) == 0)
			{
				return false;
			}
			if (other is TupleTypeSymbol tupleTypeSymbol)
			{
				return tupleTypeSymbol.Equals(this, comparison);
			}
			if ((object)other.OriginalDefinition != this)
			{
				return false;
			}
			return other.Equals(this, comparison);
		}

		protected UseSiteInfo<AssemblySymbol> CalculateUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency).AdjustDiagnosticInfo(DeriveUseSiteErrorInfoFromBase());
			if (result.DiagnosticInfo != null)
			{
				return result;
			}
			if (ContainingModule.HasUnifiedReferences)
			{
				HashSet<TypeSymbol> checkedTypes = null;
				DiagnosticInfo unificationUseSiteDiagnosticRecursive = GetUnificationUseSiteDiagnosticRecursive(this, ref checkedTypes);
				if (unificationUseSiteDiagnosticRecursive != null)
				{
					result = new UseSiteInfo<AssemblySymbol>(unificationUseSiteDiagnosticRecursive);
				}
			}
			return result;
		}

		private DiagnosticInfo DeriveUseSiteErrorInfoFromBase()
		{
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = base.BaseTypeNoUseSiteDiagnostics;
			while ((object)baseTypeNoUseSiteDiagnostics != null)
			{
				if (TypeSymbolExtensions.IsErrorType(baseTypeNoUseSiteDiagnostics) && baseTypeNoUseSiteDiagnostics is NoPiaIllegalGenericInstantiationSymbol)
				{
					return baseTypeNoUseSiteDiagnostics.GetUseSiteInfo().DiagnosticInfo;
				}
				baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
			}
			return null;
		}

		internal sealed override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			if (!TypeSymbolExtensions.MarkCheckedIfNecessary(this, ref checkedTypes))
			{
				return null;
			}
			DiagnosticInfo unificationUseSiteErrorInfo = owner.ContainingModule.GetUnificationUseSiteErrorInfo(this);
			if (unificationUseSiteErrorInfo != null)
			{
				return unificationUseSiteErrorInfo;
			}
			NamedTypeSymbol containingType = ContainingType;
			if ((object)containingType != null)
			{
				unificationUseSiteErrorInfo = containingType.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes);
				if (unificationUseSiteErrorInfo != null)
				{
					return unificationUseSiteErrorInfo;
				}
			}
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = base.BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics != null)
			{
				unificationUseSiteErrorInfo = baseTypeNoUseSiteDiagnostics.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes);
				if (unificationUseSiteErrorInfo != null)
				{
					return unificationUseSiteErrorInfo;
				}
			}
			return Symbol.GetUnificationUseSiteDiagnosticRecursive(base.InterfacesNoUseSiteDiagnostics, owner, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(TypeParameters, owner, ref checkedTypes);
		}
	}
}
