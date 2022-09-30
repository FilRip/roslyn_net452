using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class ArrayTypeSymbol : TypeSymbol, IArrayTypeReference, IArrayTypeSymbol
	{
		private abstract class SZOrMDArray : ArrayTypeSymbol
		{
			private readonly TypeSymbol _elementType;

			private readonly ImmutableArray<CustomModifier> _customModifiers;

			private readonly NamedTypeSymbol _systemArray;

			public sealed override ImmutableArray<CustomModifier> CustomModifiers => _customModifiers;

			public sealed override TypeSymbol ElementType => _elementType;

			internal sealed override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics => _systemArray;

			public SZOrMDArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, NamedTypeSymbol systemArray)
			{
				_elementType = elementType;
				_systemArray = systemArray;
				_customModifiers = customModifiers.NullToEmpty();
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution)
			{
				TypeWithModifiers typeWithModifiers = new TypeWithModifiers(_elementType, _customModifiers);
				TypeWithModifiers typeWithModifiers2 = typeWithModifiers.InternalSubstituteTypeParameters(substitution);
				TypeWithModifiers result;
				if (typeWithModifiers2 != typeWithModifiers)
				{
					ArrayTypeSymbol type;
					if (!IsSZArray)
					{
						type = ((!HasDefaultSizesAndLowerBounds) ? ((MDArray)new MDArrayWithSizesAndBounds(typeWithModifiers2.Type, typeWithModifiers2.CustomModifiers, Rank, Sizes, LowerBounds, _systemArray)) : ((MDArray)new MDArrayNoSizesOrBounds(typeWithModifiers2.Type, typeWithModifiers2.CustomModifiers, Rank, _systemArray)));
					}
					else
					{
						ImmutableArray<NamedTypeSymbol> immutableArray = InterfacesNoUseSiteDiagnostics;
						if (immutableArray.Length > 0)
						{
							immutableArray = immutableArray.SelectAsArray((NamedTypeSymbol @interface, TypeSubstitution map) => (NamedTypeSymbol)@interface.InternalSubstituteTypeParameters(map).AsTypeSymbolOnly(), substitution);
						}
						type = new SZArray(typeWithModifiers2.Type, typeWithModifiers2.CustomModifiers, _systemArray, immutableArray);
					}
					result = new TypeWithModifiers(type);
				}
				else
				{
					result = new TypeWithModifiers(this);
				}
				return result;
			}
		}

		private sealed class SZArray : SZOrMDArray
		{
			private readonly ImmutableArray<NamedTypeSymbol> _interfaces;

			public override int Rank => 1;

			internal override bool IsSZArray => true;

			internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics => _interfaces;

			internal override bool HasDefaultSizesAndLowerBounds => true;

			public SZArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, NamedTypeSymbol systemArray, ImmutableArray<NamedTypeSymbol> interfaces)
				: base(elementType, customModifiers, systemArray)
			{
				_interfaces = interfaces;
			}

			internal override ArrayTypeSymbol WithElementType(TypeSymbol newElementType)
			{
				ImmutableArray<NamedTypeSymbol> immutableArray = _interfaces;
				if (immutableArray.Length > 0)
				{
					immutableArray = immutableArray.SelectAsArray((NamedTypeSymbol i) => i.OriginalDefinition.Construct(newElementType));
				}
				return new SZArray(newElementType, base.CustomModifiers, base.BaseTypeNoUseSiteDiagnostics, immutableArray);
			}
		}

		private abstract class MDArray : SZOrMDArray
		{
			private readonly int _rank;

			public sealed override int Rank => _rank;

			internal sealed override bool IsSZArray => false;

			internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics => ImmutableArray<NamedTypeSymbol>.Empty;

			public MDArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, int rank, NamedTypeSymbol systemArray)
				: base(elementType, customModifiers, systemArray)
			{
				_rank = rank;
			}
		}

		private sealed class MDArrayNoSizesOrBounds : MDArray
		{
			internal override bool HasDefaultSizesAndLowerBounds => true;

			public MDArrayNoSizesOrBounds(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, int rank, NamedTypeSymbol systemArray)
				: base(elementType, customModifiers, rank, systemArray)
			{
			}

			internal override ArrayTypeSymbol WithElementType(TypeSymbol newElementType)
			{
				return new MDArrayNoSizesOrBounds(newElementType, base.CustomModifiers, base.Rank, base.BaseTypeNoUseSiteDiagnostics);
			}
		}

		private sealed class MDArrayWithSizesAndBounds : MDArray
		{
			private readonly ImmutableArray<int> _sizes;

			private readonly ImmutableArray<int> _lowerBounds;

			internal override ImmutableArray<int> Sizes => _sizes;

			internal override ImmutableArray<int> LowerBounds => _lowerBounds;

			internal override bool HasDefaultSizesAndLowerBounds => false;

			public MDArrayWithSizesAndBounds(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds, NamedTypeSymbol systemArray)
				: base(elementType, customModifiers, rank, systemArray)
			{
				_sizes = sizes.NullToEmpty();
				_lowerBounds = lowerBounds;
			}

			internal override ArrayTypeSymbol WithElementType(TypeSymbol newElementType)
			{
				return new MDArrayWithSizesAndBounds(newElementType, base.CustomModifiers, base.Rank, Sizes, LowerBounds, base.BaseTypeNoUseSiteDiagnostics);
			}
		}

		private bool IArrayTypeReferenceIsSZArray => AdaptedArrayTypeSymbol.IsSZArray;

		private ImmutableArray<int> IArrayTypeReferenceLowerBounds => AdaptedArrayTypeSymbol.LowerBounds;

		private int IArrayTypeReferenceRank => AdaptedArrayTypeSymbol.Rank;

		private ImmutableArray<int> IArrayTypeReferenceSizes => AdaptedArrayTypeSymbol.Sizes;

		private bool ITypeReferenceIsEnum => false;

		private bool ITypeReferenceIsValueType => false;

		private Microsoft.Cci.PrimitiveTypeCode ITypeReferenceTypeCode => Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;

		private TypeDefinitionHandle ITypeReferenceTypeDef => default(TypeDefinitionHandle);

		private IGenericMethodParameterReference ITypeReferenceAsGenericMethodParameterReference => null;

		private IGenericTypeInstanceReference ITypeReferenceAsGenericTypeInstanceReference => null;

		private IGenericTypeParameterReference ITypeReferenceAsGenericTypeParameterReference => null;

		private INamespaceTypeReference ITypeReferenceAsNamespaceTypeReference => null;

		private INestedTypeReference ITypeReferenceAsNestedTypeReference => null;

		private ISpecializedNestedTypeReference ITypeReferenceAsSpecializedNestedTypeReference => null;

		internal ArrayTypeSymbol AdaptedArrayTypeSymbol => this;

		public abstract ImmutableArray<CustomModifier> CustomModifiers { get; }

		public abstract int Rank { get; }

		internal abstract bool IsSZArray { get; }

		internal virtual ImmutableArray<int> Sizes => ImmutableArray<int>.Empty;

		internal virtual ImmutableArray<int> LowerBounds => default(ImmutableArray<int>);

		internal abstract bool HasDefaultSizesAndLowerBounds { get; }

		public abstract TypeSymbol ElementType { get; }

		internal abstract override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics { get; }

		public override bool IsReferenceType => true;

		public override bool IsValueType => false;

		public override SymbolKind Kind => SymbolKind.ArrayType;

		public override TypeKind TypeKind => TypeKind.Array;

		public override Symbol ContainingSymbol => null;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

		public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		private ITypeSymbol IArrayTypeSymbol_ElementType => ElementType;

		private NullableAnnotation IArrayTypeSymbol_ElementNullableAnnotation => NullableAnnotation.None;

		private int IArrayTypeSymbol_Rank => Rank;

		private bool IArrayTypeSymbol_IsSZArray => IsSZArray;

		private ImmutableArray<int> IArrayTypeSymbol_Sizes => Sizes;

		private ImmutableArray<int> IArrayTypeSymbol_LowerBounds => LowerBounds;

		private ImmutableArray<CustomModifier> IArrayTypeSymbol_CustomModifiers => CustomModifiers;

		private ITypeReference IArrayTypeReferenceGetElementType(EmitContext context)
		{
			PEModuleBuilder obj = (PEModuleBuilder)context.Module;
			ImmutableArray<CustomModifier> customModifiers = AdaptedArrayTypeSymbol.CustomModifiers;
			ITypeReference typeReference = obj.Translate(AdaptedArrayTypeSymbol.ElementType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			if (customModifiers.Length == 0)
			{
				return typeReference;
			}
			return new ModifiedTypeReference(typeReference, customModifiers.As<ICustomModifier>());
		}

		ITypeReference IArrayTypeReference.GetElementType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IArrayTypeReferenceGetElementType
			return this.IArrayTypeReferenceGetElementType(context);
		}

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
			visitor.Visit(this);
		}

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			return null;
		}

		internal new ArrayTypeSymbol GetCciAdapter()
		{
			return this;
		}

		internal static ArrayTypeSymbol CreateVBArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, int rank, VisualBasicCompilation compilation)
		{
			return CreateVBArray(elementType, customModifiers, rank, compilation.Assembly);
		}

		internal static ArrayTypeSymbol CreateVBArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, int rank, AssemblySymbol declaringAssembly)
		{
			if (rank == 1)
			{
				return CreateSZArray(elementType, customModifiers, declaringAssembly);
			}
			return CreateMDArray(elementType, customModifiers, rank, default(ImmutableArray<int>), default(ImmutableArray<int>), declaringAssembly);
		}

		internal static ArrayTypeSymbol CreateMDArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, int rank, ImmutableArray<int> sizes, ImmutableArray<int> lowerBounds, AssemblySymbol declaringAssembly)
		{
			NamedTypeSymbol specialType = declaringAssembly.GetSpecialType(SpecialType.System_Array);
			if (sizes.IsDefaultOrEmpty && lowerBounds.IsDefault)
			{
				return new MDArrayNoSizesOrBounds(elementType, customModifiers, rank, specialType);
			}
			return new MDArrayWithSizesAndBounds(elementType, customModifiers, rank, sizes, lowerBounds, specialType);
		}

		internal static ArrayTypeSymbol CreateSZArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, VisualBasicCompilation compilation)
		{
			return CreateSZArray(elementType, customModifiers, compilation.Assembly);
		}

		internal static ArrayTypeSymbol CreateSZArray(TypeSymbol elementType, ImmutableArray<CustomModifier> customModifiers, AssemblySymbol declaringAssembly)
		{
			return new SZArray(elementType, customModifiers, declaringAssembly.GetSpecialType(SpecialType.System_Array), GetSZArrayInterfaces(elementType, declaringAssembly));
		}

		private static ImmutableArray<NamedTypeSymbol> GetSZArrayInterfaces(TypeSymbol elementType, AssemblySymbol declaringAssembly)
		{
			NamedTypeSymbol specialType = declaringAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IList_T);
			NamedTypeSymbol specialType2 = declaringAssembly.GetSpecialType(SpecialType.System_Collections_Generic_IReadOnlyList_T);
			if (TypeSymbolExtensions.IsErrorType(specialType))
			{
				if (!TypeSymbolExtensions.IsErrorType(specialType2))
				{
					return ImmutableArray.Create(specialType2.Construct(elementType));
				}
				return ImmutableArray<NamedTypeSymbol>.Empty;
			}
			if (TypeSymbolExtensions.IsErrorType(specialType2))
			{
				return ImmutableArray.Create(specialType.Construct(elementType));
			}
			return ImmutableArray.Create(specialType.Construct(elementType), specialType2.Construct(elementType));
		}

		internal bool HasSameShapeAs(ArrayTypeSymbol other)
		{
			if (Rank == other.Rank)
			{
				return IsSZArray == other.IsSZArray;
			}
			return false;
		}

		internal bool HasSameSizesAndLowerBoundsAs(ArrayTypeSymbol other)
		{
			if (Sizes.SequenceEqual(other.Sizes))
			{
				ImmutableArray<int> lowerBounds = LowerBounds;
				if (lowerBounds.IsDefault)
				{
					return other.LowerBounds.IsDefault;
				}
				ImmutableArray<int> lowerBounds2 = other.LowerBounds;
				return !lowerBounds2.IsDefault && lowerBounds.SequenceEqual(lowerBounds2);
			}
			return false;
		}

		public override ImmutableArray<Symbol> GetMembers()
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public override ImmutableArray<Symbol> GetMembers(string name)
		{
			return ImmutableArray<Symbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitArrayType(this, arg);
		}

		internal abstract override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution);

		public sealed override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			return Equals(other as ArrayTypeSymbol, comparison);
		}

		public bool Equals(ArrayTypeSymbol other, TypeCompareKind compareKind)
		{
			if ((object)this == other)
			{
				return true;
			}
			if ((object)other == null || !other.HasSameShapeAs(this) || !other.ElementType.Equals(ElementType, compareKind))
			{
				return false;
			}
			if ((compareKind & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) == 0)
			{
				ImmutableArray<CustomModifier> customModifiers = CustomModifiers;
				ImmutableArray<CustomModifier> customModifiers2 = other.CustomModifiers;
				if (!TypeSymbolExtensions.AreSameCustomModifiers(customModifiers, customModifiers2))
				{
					return false;
				}
				return HasSameSizesAndLowerBoundsAs(other);
			}
			return true;
		}

		public sealed override int GetHashCode()
		{
			int currentKey = 0;
			TypeSymbol typeSymbol = this;
			while (typeSymbol.TypeKind == TypeKind.Array)
			{
				ArrayTypeSymbol obj = (ArrayTypeSymbol)typeSymbol;
				currentKey = Hash.Combine(obj.Rank, currentKey);
				typeSymbol = obj.ElementType;
			}
			return Hash.Combine(typeSymbol, currentKey);
		}

		internal abstract ArrayTypeSymbol WithElementType(TypeSymbol elementType);

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = DeriveUseSiteInfoFromType(ElementType);
			DiagnosticInfo? diagnosticInfo = useSiteInfo.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == 30649)
			{
				return useSiteInfo;
			}
			UseSiteInfo<AssemblySymbol> second = DeriveUseSiteInfoFromCustomModifiers(CustomModifiers);
			return MergeUseSiteInfo(useSiteInfo, second);
		}

		internal override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			return ElementType.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes) ?? (((object)BaseTypeNoUseSiteDiagnostics != null) ? BaseTypeNoUseSiteDiagnostics.GetUnificationUseSiteDiagnosticRecursive(owner, ref checkedTypes) : null) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(InterfacesNoUseSiteDiagnostics, owner, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(CustomModifiers, owner, ref checkedTypes);
		}

		private bool IArrayTypeSymbol_Equals(IArrayTypeSymbol symbol)
		{
			return Equals(symbol as ArrayTypeSymbol);
		}

		bool IArrayTypeSymbol.Equals(IArrayTypeSymbol symbol)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IArrayTypeSymbol_Equals
			return this.IArrayTypeSymbol_Equals(symbol);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitArrayType(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitArrayType(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitArrayType(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitArrayType(this);
		}
	}
}
