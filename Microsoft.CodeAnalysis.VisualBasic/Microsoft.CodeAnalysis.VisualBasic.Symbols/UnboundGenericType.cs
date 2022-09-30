using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class UnboundGenericType : NamedTypeSymbol
	{
		private sealed class ConstructedSymbol : UnboundGenericType
		{
			private readonly NamedTypeSymbol _originalDefinition;

			private Symbol _lazyContainingSymbol;

			private NamedTypeSymbol _lazyConstructedFrom;

			private ImmutableArray<TypeSymbol> _lazyTypeArguments;

			private TypeSubstitution _lazyTypeSubstitution;

			public override bool IsUnboundGenericType => true;

			public override NamedTypeSymbol OriginalDefinition => _originalDefinition;

			public override Symbol ContainingSymbol
			{
				get
				{
					if ((object)_lazyContainingSymbol == null)
					{
						NamedTypeSymbol containingType = OriginalDefinition.ContainingType;
						Interlocked.CompareExchange(value: ((object)containingType == null) ? OriginalDefinition.ContainingSymbol : ((!containingType.IsGenericType) ? containingType : Create(containingType)), location1: ref _lazyContainingSymbol, comparand: null);
					}
					return _lazyContainingSymbol;
				}
			}

			public override NamedTypeSymbol ConstructedFrom
			{
				get
				{
					if ((object)_lazyConstructedFrom == null)
					{
						NamedTypeSymbol containingType = OriginalDefinition.ContainingType;
						Interlocked.CompareExchange(value: ((object)containingType == null || !containingType.IsGenericType) ? OriginalDefinition : ((OriginalDefinition.Arity != 0) ? ((UnboundGenericType)new ConstructedFromSymbol(this)) : ((UnboundGenericType)this)), location1: ref _lazyConstructedFrom, comparand: null);
					}
					return _lazyConstructedFrom;
				}
			}

			public override ImmutableArray<TypeParameterSymbol> TypeParameters
			{
				get
				{
					if (OriginalDefinition.Arity == 0)
					{
						return ImmutableArray<TypeParameterSymbol>.Empty;
					}
					return ConstructedFrom.TypeParameters;
				}
			}

			internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics
			{
				get
				{
					if (_lazyTypeArguments.IsDefault)
					{
						TypeSymbol[] array = new TypeSymbol[OriginalDefinition.Arity - 1 + 1];
						int num = array.Length - 1;
						for (int i = 0; i <= num; i++)
						{
							array[i] = UnboundTypeArgument;
						}
						ImmutableInterlocked.InterlockedInitialize(ref _lazyTypeArguments, array.AsImmutableOrNull());
					}
					return _lazyTypeArguments;
				}
			}

			internal override TypeSubstitution TypeSubstitution
			{
				get
				{
					if (_lazyTypeSubstitution == null)
					{
						Interlocked.CompareExchange(value: (!(ContainingSymbol is ConstructedSymbol constructedSymbol)) ? TypeSubstitution.Create(OriginalDefinition, OriginalDefinition.TypeParameters, TypeArgumentsNoUseSiteDiagnostics) : ((OriginalDefinition.Arity != 0) ? TypeSubstitution.Create(constructedSymbol.TypeSubstitution, OriginalDefinition, TypeArgumentsNoUseSiteDiagnostics) : TypeSubstitution.Concat(OriginalDefinition, constructedSymbol.TypeSubstitution, null)), location1: ref _lazyTypeSubstitution, comparand: null);
					}
					return _lazyTypeSubstitution;
				}
			}

			public override IEnumerable<string> MemberNames => new List<string>((from t in OriginalDefinition.GetTypeMembersUnordered()
				select t.Name).Distinct());

			public ConstructedSymbol(NamedTypeSymbol originalDefinition)
			{
				if (originalDefinition.Arity == 0)
				{
					_lazyTypeArguments = ImmutableArray<TypeSymbol>.Empty;
				}
				_originalDefinition = originalDefinition;
			}

			public override ImmutableArray<Symbol> GetMembers()
			{
				ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
				ImmutableArray<Symbol>.Enumerator enumerator = OriginalDefinition.GetMembers().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.NamedType)
					{
						instance.AddRange((NamedTypeSymbol)current);
					}
				}
				return StaticCast<Symbol>.From(GetTypeMembers(instance.ToImmutableAndFree()));
			}

			public override ImmutableArray<Symbol> GetMembers(string name)
			{
				return StaticCast<Symbol>.From(GetTypeMembers(name));
			}

			internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
			{
				return GetTypeMembers(OriginalDefinition.GetTypeMembersUnordered());
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
			{
				return GetTypeMembers(OriginalDefinition.GetTypeMembers());
			}

			private static ImmutableArray<NamedTypeSymbol> GetTypeMembers(ImmutableArray<NamedTypeSymbol> originalTypeMembers)
			{
				if (originalTypeMembers.IsEmpty)
				{
					return originalTypeMembers;
				}
				NamedTypeSymbol[] array = new NamedTypeSymbol[originalTypeMembers.Length - 1 + 1];
				int num = array.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = new ConstructedSymbol(originalTypeMembers[i]).ConstructedFrom;
				}
				return array.AsImmutableOrNull();
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
			{
				return GetTypeMembers(OriginalDefinition.GetTypeMembers(name));
			}

			public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
			{
				return OriginalDefinition.GetTypeMembers(name, arity).SelectAsArray((NamedTypeSymbol t) => new ConstructedSymbol(t).ConstructedFrom);
			}

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
			{
				return new TypeWithModifiers(this);
			}
		}

		private sealed class ConstructedFromSymbol : UnboundGenericType
		{
			public readonly ConstructedSymbol Constructed;

			private readonly ImmutableArray<TypeParameterSymbol> _typeParameters;

			private readonly TypeSubstitution _typeSubstitution;

			public override bool IsUnboundGenericType => false;

			public override NamedTypeSymbol OriginalDefinition => Constructed.OriginalDefinition;

			public override NamedTypeSymbol ConstructedFrom => this;

			public override Symbol ContainingSymbol => Constructed.ContainingSymbol;

			public override ImmutableArray<TypeParameterSymbol> TypeParameters => _typeParameters;

			internal override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics => StaticCast<TypeSymbol>.From(_typeParameters);

			internal override TypeSubstitution TypeSubstitution => _typeSubstitution;

			public override IEnumerable<string> MemberNames => SpecializedCollections.EmptyCollection<string>();

			public ConstructedFromSymbol(ConstructedSymbol constructed)
			{
				NamedTypeSymbol originalDefinition = constructed.OriginalDefinition;
				Constructed = constructed;
				ImmutableArray<TypeParameterSymbol> typeParameters = originalDefinition.TypeParameters;
				SubstitutedTypeParameterSymbol[] array = new SubstitutedTypeParameterSymbol[typeParameters.Length - 1 + 1];
				int num = typeParameters.Length - 1;
				for (int i = 0; i <= num; i++)
				{
					array[i] = new SubstitutedTypeParameterSymbol(typeParameters[i]);
				}
				ImmutableArray<SubstitutedTypeParameterSymbol> immutableArray = array.AsImmutableOrNull();
				ConstructedSymbol obj = (ConstructedSymbol)constructed.ContainingSymbol;
				_typeParameters = StaticCast<TypeParameterSymbol>.From(immutableArray);
				TypeSubstitution typeSubstitution = TypeSubstitution.CreateForAlphaRename(obj.TypeSubstitution, immutableArray);
				SubstitutedTypeParameterSymbol[] array2 = array;
				for (int j = 0; j < array2.Length; j = checked(j + 1))
				{
					array2[j].SetContainingSymbol(this);
				}
				_typeSubstitution = typeSubstitution;
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

			internal override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution)
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal static readonly ErrorTypeSymbol UnboundTypeArgument = new ErrorTypeSymbol();

		public override string Name => OriginalDefinition.Name;

		internal override bool MangleName => OriginalDefinition.MangleName;

		internal sealed override bool HasSpecialName => OriginalDefinition.HasSpecialName;

		public sealed override bool IsSerializable => OriginalDefinition.IsSerializable;

		internal override TypeLayout Layout => OriginalDefinition.Layout;

		internal override CharSet MarshallingCharSet => OriginalDefinition.MarshallingCharSet;

		public abstract override bool IsUnboundGenericType { get; }

		public override bool IsAnonymousType => OriginalDefinition.IsAnonymousType;

		public abstract override NamedTypeSymbol OriginalDefinition { get; }

		public abstract override Symbol ContainingSymbol { get; }

		public override int Arity => OriginalDefinition.Arity;

		public abstract override ImmutableArray<TypeParameterSymbol> TypeParameters { get; }

		public abstract override NamedTypeSymbol ConstructedFrom { get; }

		internal abstract override ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics { get; }

		internal sealed override bool HasTypeArgumentsCustomModifiers => false;

		public override NamedTypeSymbol EnumUnderlyingType => OriginalDefinition.EnumUnderlyingType;

		public override bool MightContainExtensionMethods => false;

		internal override bool HasCodeAnalysisEmbeddedAttribute => OriginalDefinition.HasCodeAnalysisEmbeddedAttribute;

		internal override bool HasVisualBasicEmbeddedAttribute => OriginalDefinition.HasVisualBasicEmbeddedAttribute;

		internal override bool IsExtensibleInterfaceNoUseSiteDiagnostics => OriginalDefinition.IsExtensibleInterfaceNoUseSiteDiagnostics;

		internal override bool IsWindowsRuntimeImport => OriginalDefinition.IsWindowsRuntimeImport;

		internal override bool ShouldAddWinRTMembers => OriginalDefinition.ShouldAddWinRTMembers;

		internal override bool IsComImport => OriginalDefinition.IsComImport;

		internal override TypeSymbol CoClassType => OriginalDefinition.CoClassType;

		internal override bool HasDeclarativeSecurity => OriginalDefinition.HasDeclarativeSecurity;

		public override Accessibility DeclaredAccessibility => OriginalDefinition.DeclaredAccessibility;

		public override TypeKind TypeKind => OriginalDefinition.TypeKind;

		internal override bool IsInterface => OriginalDefinition.IsInterface;

		public override ImmutableArray<Location> Locations => OriginalDefinition.Locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => OriginalDefinition.DeclaringSyntaxReferences;

		public override bool IsMustInherit => OriginalDefinition.IsMustInherit;

		public override bool IsNotInheritable => OriginalDefinition.IsNotInheritable;

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => OriginalDefinition.ObsoleteAttributeData;

		internal override bool CanConstruct => false;

		internal override string DefaultPropertyName => null;

		internal abstract override TypeSubstitution TypeSubstitution { get; }

		internal static NamedTypeSymbol Create(NamedTypeSymbol type)
		{
			if (type.IsUnboundGenericType)
			{
				return type;
			}
			if (type is ConstructedFromSymbol constructedFromSymbol)
			{
				return constructedFromSymbol.Constructed;
			}
			if (type.IsGenericType)
			{
				return new ConstructedSymbol(type.OriginalDefinition);
			}
			throw new InvalidOperationException();
		}

		private UnboundGenericType()
		{
		}

		public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
		{
			return OriginalDefinition.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken);
		}

		public sealed override ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal)
		{
			return GetEmptyTypeArgumentCustomModifiers(ordinal);
		}

		internal override ImmutableArray<string> GetAppliedConditionalSymbols()
		{
			return OriginalDefinition.GetAppliedConditionalSymbols();
		}

		internal override AttributeUsageInfo GetAttributeUsageInfo()
		{
			return OriginalDefinition.GetAttributeUsageInfo();
		}

		internal override IEnumerable<SecurityAttribute> GetSecurityInformation()
		{
			return OriginalDefinition.GetSecurityInformation();
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return OriginalDefinition.GetAttributes();
		}

		internal override NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal abstract override TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution additionalSubstitution);

		internal override NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return null;
		}

		internal override NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics)
		{
			return null;
		}

		internal override NamedTypeSymbol GetDirectBaseTypeNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			return null;
		}

		internal override NamedTypeSymbol GetDeclaredBase(BasesBeingResolved basesBeingResolved)
		{
			return null;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfacesNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return OriginalDefinition.GetUseSiteInfo();
		}

		public sealed override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			return Equals(other as UnboundGenericType, comparison);
		}

		public bool Equals(UnboundGenericType other, TypeCompareKind comparison)
		{
			if ((object)this == other)
			{
				return true;
			}
			return (object)other != null && other.GetType() == GetType() && other.OriginalDefinition.Equals(OriginalDefinition, comparison);
		}

		public sealed override int GetHashCode()
		{
			return Hash.Combine(GetType(), OriginalDefinition.GetHashCode());
		}

		public override NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			throw new InvalidOperationException();
		}

		internal sealed override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
		{
			return null;
		}

		internal sealed override IEnumerable<FieldSymbol> GetFieldsToEmit()
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal sealed override IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides()
		{
			return SpecializedCollections.EmptyEnumerable<PropertySymbol>();
		}
	}
}
