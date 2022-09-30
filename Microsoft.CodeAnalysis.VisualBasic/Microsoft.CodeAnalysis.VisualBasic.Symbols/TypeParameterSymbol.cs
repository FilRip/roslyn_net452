using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class TypeParameterSymbol : TypeSymbol, IGenericParameterReference, IGenericMethodParameterReference, IGenericTypeParameterReference, IGenericParameter, IGenericMethodParameter, IGenericTypeParameter, ITypeParameterSymbol, ITypeParameterSymbolInternal
	{
		private static readonly Func<TypeSymbol, TypeSubstitution, TypeSymbol> s_substituteFunc = (TypeSymbol type, TypeSubstitution substitution) => type.InternalSubstituteTypeParameters(substitution).Type;

		private bool ITypeReferenceIsEnum => false;

		private bool ITypeReferenceIsValueType => false;

		private Microsoft.Cci.PrimitiveTypeCode ITypeReferenceTypeCode => Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;

		private TypeDefinitionHandle ITypeReferenceTypeDef => default(TypeDefinitionHandle);

		private IGenericMethodParameter IGenericParameterAsGenericMethodParameter
		{
			get
			{
				if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.Method)
				{
					return this;
				}
				return null;
			}
		}

		private IGenericMethodParameterReference ITypeReferenceAsGenericMethodParameterReference
		{
			get
			{
				if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.Method)
				{
					return this;
				}
				return null;
			}
		}

		private IGenericTypeInstanceReference ITypeReferenceAsGenericTypeInstanceReference => null;

		private IGenericTypeParameter IGenericParameterAsGenericTypeParameter
		{
			get
			{
				if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.NamedType)
				{
					return this;
				}
				return null;
			}
		}

		private IGenericTypeParameterReference ITypeReferenceAsGenericTypeParameterReference
		{
			get
			{
				if (AdaptedTypeParameterSymbol.ContainingSymbol.Kind == SymbolKind.NamedType)
				{
					return this;
				}
				return null;
			}
		}

		private INamespaceTypeReference ITypeReferenceAsNamespaceTypeReference => null;

		private INestedTypeReference ITypeReferenceAsNestedTypeReference => null;

		private ISpecializedNestedTypeReference ITypeReferenceAsSpecializedNestedTypeReference => null;

		private string INamedEntityName => AdaptedTypeParameterSymbol.MetadataName;

		private ushort IParameterListEntryIndex => (ushort)AdaptedTypeParameterSymbol.Ordinal;

		private IMethodReference IGenericMethodParameterReferenceDefiningMethod => ((MethodSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

		private ITypeReference IGenericTypeParameterReferenceDefiningType => ((NamedTypeSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

		private bool IGenericParameterMustBeReferenceType => AdaptedTypeParameterSymbol.HasReferenceTypeConstraint;

		private bool IGenericParameterMustBeValueType => AdaptedTypeParameterSymbol.HasValueTypeConstraint;

		private bool IGenericParameterMustHaveDefaultConstructor
		{
			get
			{
				if (!AdaptedTypeParameterSymbol.HasConstructorConstraint)
				{
					return AdaptedTypeParameterSymbol.HasValueTypeConstraint;
				}
				return true;
			}
		}

		private TypeParameterVariance IGenericParameterVariance => AdaptedTypeParameterSymbol.Variance switch
		{
			VarianceKind.None => TypeParameterVariance.NonVariant, 
			VarianceKind.In => TypeParameterVariance.Contravariant, 
			VarianceKind.Out => TypeParameterVariance.Covariant, 
			_ => throw ExceptionUtilities.UnexpectedValue(AdaptedTypeParameterSymbol.Variance), 
		};

		private IMethodDefinition IGenericMethodParameterDefiningMethod => ((MethodSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

		private ITypeDefinition IGenericTypeParameterDefiningType => ((NamedTypeSymbol)AdaptedTypeParameterSymbol.ContainingSymbol).GetCciAdapter();

		internal TypeParameterSymbol AdaptedTypeParameterSymbol => this;

		public new virtual TypeParameterSymbol OriginalDefinition => this;

		protected sealed override TypeSymbol OriginalTypeSymbolDefinition => OriginalDefinition;

		public abstract int Ordinal { get; }

		internal abstract ImmutableArray<TypeSymbol> ConstraintTypesNoUseSiteDiagnostics { get; }

		public abstract bool HasConstructorConstraint { get; }

		public abstract TypeParameterKind TypeParameterKind { get; }

		public MethodSymbol DeclaringMethod => ContainingSymbol as MethodSymbol;

		public NamedTypeSymbol DeclaringType => ContainingSymbol as NamedTypeSymbol;

		public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public sealed override SymbolKind Kind => SymbolKind.TypeParameter;

		public sealed override TypeKind TypeKind => TypeKind.TypeParameter;

		internal sealed override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => null;

		internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics => ImmutableArray<NamedTypeSymbol>.Empty;

		public sealed override bool IsReferenceType
		{
			get
			{
				if (HasReferenceTypeConstraint)
				{
					return true;
				}
				return IsReferenceTypeIgnoringIsClass();
			}
		}

		public override bool IsValueType
		{
			get
			{
				if (HasValueTypeConstraint)
				{
					return true;
				}
				ImmutableArray<TypeSymbol>.Enumerator enumerator = ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.IsValueType)
					{
						return true;
					}
				}
				return false;
			}
		}

		public abstract bool HasReferenceTypeConstraint { get; }

		public abstract bool HasValueTypeConstraint { get; }

		private bool HasUnmanagedTypeConstraint => false;

		private bool HasNotNullConstraint => false;

		public abstract VarianceKind Variance { get; }

		public virtual TypeParameterSymbol ReducedFrom => null;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => ContainingSymbol.EmbeddedSymbolKind;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		private NullableAnnotation ITypeParameterSymbol_ReferenceTypeConstraintNullableAnnotation => NullableAnnotation.None;

		private IMethodSymbol ITypeParameterSymbol_DeclaringMethod => DeclaringMethod;

		private INamedTypeSymbol ITypeParameterSymbol_DeclaringType => DeclaringType;

		private int ITypeParameterSymbol_Ordinal => Ordinal;

		private ImmutableArray<ITypeSymbol> ITypeParameterSymbol_ConstraintTypes => StaticCast<ITypeSymbol>.From(ConstraintTypesNoUseSiteDiagnostics);

		private ImmutableArray<NullableAnnotation> ITypeParameterSymbol_ConstraintNullableAnnotations => ConstraintTypesNoUseSiteDiagnostics.SelectAsArray((TypeSymbol t) => NullableAnnotation.None);

		private ITypeParameterSymbol ITypeParameterSymbol_OriginalDefinition => OriginalDefinition;

		private ITypeParameterSymbol ITypeParameterSymbol_ReducedFrom => ReducedFrom;

		private ITypeDefinition ITypeReferenceGetResolvedType(EmitContext context)
		{
			return null;
		}

		ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceGetResolvedType
			return this.ITypeReferenceGetResolvedType(context);
		}

		private INamespaceTypeDefinition ITypeReferenceAsNamespaceTypeDefinition(EmitContext context)
		{
			return null;
		}

		INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsNamespaceTypeDefinition
			return this.ITypeReferenceAsNamespaceTypeDefinition(context);
		}

		private INestedTypeDefinition ITypeReferenceAsNestedTypeDefinition(EmitContext context)
		{
			return null;
		}

		INestedTypeDefinition ITypeReference.AsNestedTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsNestedTypeDefinition
			return this.ITypeReferenceAsNestedTypeDefinition(context);
		}

		private ITypeDefinition ITypeReferenceAsTypeDefinition(EmitContext context)
		{
			return null;
		}

		ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsTypeDefinition
			return this.ITypeReferenceAsTypeDefinition(context);
		}

		internal sealed override void IReferenceDispatch(MetadataVisitor visitor)
		{
			SymbolKind kind = AdaptedTypeParameterSymbol.ContainingSymbol.Kind;
			if (((PEModuleBuilder)visitor.Context.Module).SourceModule == AdaptedTypeParameterSymbol.ContainingModule)
			{
				switch (kind)
				{
				case SymbolKind.NamedType:
					visitor.Visit((IGenericTypeParameter)this);
					break;
				case SymbolKind.Method:
					visitor.Visit((IGenericMethodParameter)this);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(kind);
				}
			}
			else
			{
				switch (kind)
				{
				case SymbolKind.NamedType:
					visitor.Visit((IGenericTypeParameterReference)this);
					break;
				case SymbolKind.Method:
					visitor.Visit((IGenericMethodParameterReference)this);
					break;
				default:
					throw ExceptionUtilities.UnexpectedValue(kind);
				}
			}
		}

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			return null;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_39_IGenericParameterGetConstraints))]
		private IEnumerable<TypeReferenceWithAttributes> IGenericParameterGetConstraints(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_39_IGenericParameterGetConstraints(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<TypeReferenceWithAttributes> IGenericParameter.GetConstraints(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericParameterGetConstraints
			return this.IGenericParameterGetConstraints(context);
		}

		internal new TypeParameterSymbol GetCciAdapter()
		{
			return this;
		}

		internal override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			return null;
		}

		internal virtual UseSiteInfo<AssemblySymbol> GetConstraintsUseSiteInfo()
		{
			return default(UseSiteInfo<AssemblySymbol>);
		}

		internal ImmutableArray<TypeSymbol> ConstraintTypesWithDefinitionUseSiteDiagnostics([In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<TypeSymbol> constraintTypesNoUseSiteDiagnostics = ConstraintTypesNoUseSiteDiagnostics;
			TypeSymbolExtensions.AddConstraintsUseSiteInfo(this, ref useSiteInfo);
			ImmutableArray<TypeSymbol>.Enumerator enumerator = constraintTypesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbolExtensions.AddUseSiteInfo(enumerator.Current.OriginalDefinition, ref useSiteInfo);
			}
			return constraintTypesNoUseSiteDiagnostics;
		}

		public sealed override ImmutableArray<Symbol> GetMembers()
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public sealed override ImmutableArray<Symbol> GetMembers(string name)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public sealed override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitTypeParameter(this, arg);
		}

		internal TypeParameterSymbol()
		{
		}

		internal abstract void EnsureAllConstraintsAreResolved();

		internal static void EnsureAllConstraintsAreResolved(ImmutableArray<TypeParameterSymbol> typeParameters)
		{
			ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = typeParameters.GetEnumerator();
			while (enumerator.MoveNext())
			{
				enumerator.Current.ResolveConstraints(ConsList<TypeParameterSymbol>.Empty);
			}
		}

		internal void GetConstraints(ArrayBuilder<TypeParameterConstraint> constraintsBuilder)
		{
			constraintsBuilder.AddRange(GetConstraints());
		}

		internal virtual ImmutableArray<TypeParameterConstraint> GetConstraints()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal virtual void ResolveConstraints(ConsList<TypeParameterSymbol> inProgress)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal static ImmutableArray<TypeSymbol> GetConstraintTypesOnly(ImmutableArray<TypeParameterConstraint> constraints)
		{
			if (constraints.IsEmpty)
			{
				return ImmutableArray<TypeSymbol>.Empty;
			}
			ArrayBuilder<TypeSymbol> instance = ArrayBuilder<TypeSymbol>.GetInstance();
			ImmutableArray<TypeParameterConstraint>.Enumerator enumerator = constraints.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbol typeConstraint = enumerator.Current.TypeConstraint;
				if ((object)typeConstraint != null)
				{
					instance.Add(typeConstraint);
				}
			}
			return instance.ToImmutableAndFree();
		}

		private bool IsReferenceTypeIgnoringIsClass()
		{
			ImmutableArray<TypeSymbol>.Enumerator enumerator = ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				if (ConstraintImpliesReferenceType(enumerator.Current))
				{
					return true;
				}
			}
			return false;
		}

		private static bool ConstraintImpliesReferenceType(TypeSymbol constraint)
		{
			if (constraint.TypeKind == TypeKind.TypeParameter)
			{
				return ((TypeParameterSymbol)constraint).IsReferenceTypeIgnoringIsClass();
			}
			if (constraint.IsReferenceType)
			{
				if (TypeSymbolExtensions.IsInterfaceType(constraint))
				{
					return false;
				}
				SpecialType specialType = constraint.SpecialType;
				if ((uint)(specialType - 1) <= 1u || specialType == SpecialType.System_ValueType)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
		{
			return substitution?.GetSubstitutionFor(this) ?? new TypeWithModifiers(this);
		}

		internal static ImmutableArray<TypeSymbol> InternalSubstituteTypeParametersDistinct(TypeSubstitution substitution, ImmutableArray<TypeSymbol> types)
		{
			return types.SelectAsArray(s_substituteFunc, substitution).Distinct();
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitTypeParameter(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitTypeParameter(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitTypeParameter(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitTypeParameter(this);
		}
	}
}
