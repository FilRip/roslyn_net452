using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class TypeSymbol : NamespaceOrTypeSymbol, ITypeSymbol, ITypeSymbolInternal
	{
		protected class ExplicitInterfaceImplementationTargetMemberEqualityComparer : IEqualityComparer<Symbol>
		{
			public static readonly ExplicitInterfaceImplementationTargetMemberEqualityComparer Instance = new ExplicitInterfaceImplementationTargetMemberEqualityComparer();

			private ExplicitInterfaceImplementationTargetMemberEqualityComparer()
			{
			}

			public bool Equals(Symbol x, Symbol y)
			{
				if (x.OriginalDefinition == y.OriginalDefinition)
				{
					return EqualsIgnoringComparer.InstanceCLRSignatureCompare.Equals(x.ContainingType, y.ContainingType);
				}
				return false;
			}

			bool IEqualityComparer<Symbol>.Equals(Symbol x, Symbol y)
			{
				//ILSpy generated this explicit interface implementation from .override directive in Equals
				return this.Equals(x, y);
			}

			public int GetHashCode(Symbol obj)
			{
				return obj.OriginalDefinition.GetHashCode();
			}

			int IEqualityComparer<Symbol>.GetHashCode(Symbol obj)
			{
				//ILSpy generated this explicit interface implementation from .override directive in GetHashCode
				return this.GetHashCode(obj);
			}
		}

		internal const string ImplicitTypeName = "<invalid-global-code>";

		private static readonly TypeSymbol[] s_EmptyTypeSymbols = Array.Empty<TypeSymbol>();

		private ImmutableArray<NamedTypeSymbol> _lazyAllInterfaces;

		private MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> _lazyInterfacesAndTheirBaseInterfaces;

		private static readonly MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> EmptyInterfacesAndTheirBaseInterfaces = new MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>(0, EqualsIgnoringComparer.InstanceCLRSignatureCompare);

		private ConcurrentDictionary<Symbol, Symbol> _lazyImplementationForInterfaceMemberMap;

		protected MultiDictionary<Symbol, Symbol> m_lazyExplicitInterfaceImplementationMap;

		protected static readonly MultiDictionary<Symbol, Symbol> EmptyExplicitImplementationMap = new MultiDictionary<Symbol, Symbol>();

		public static IList<TypeSymbol> EmptyTypeSymbolsList => s_EmptyTypeSymbols;

		public new TypeSymbol OriginalDefinition => OriginalTypeSymbolDefinition;

		protected virtual TypeSymbol OriginalTypeSymbolDefinition => this;

		protected sealed override Symbol OriginalSymbolDefinition => OriginalTypeSymbolDefinition;

		internal abstract NamedTypeSymbol BaseTypeNoUseSiteDiagnostics { get; }

		internal abstract ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics { get; }

		internal ImmutableArray<NamedTypeSymbol> AllInterfacesNoUseSiteDiagnostics
		{
			get
			{
				if (_lazyAllInterfaces.IsDefault)
				{
					ImmutableInterlocked.InterlockedInitialize(ref _lazyAllInterfaces, MakeAllInterfaces());
				}
				return _lazyAllInterfaces;
			}
		}

		internal MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics
		{
			get
			{
				if (_lazyInterfacesAndTheirBaseInterfaces == null)
				{
					Interlocked.CompareExchange(ref _lazyInterfacesAndTheirBaseInterfaces, MakeInterfacesAndTheirBaseInterfaces(InterfacesNoUseSiteDiagnostics), null);
				}
				return _lazyInterfacesAndTheirBaseInterfaces;
			}
		}

		public abstract bool IsReferenceType { get; }

		public abstract bool IsValueType { get; }

		public virtual bool IsAnonymousType => false;

		public sealed override bool IsShared => false;

		public abstract TypeKind TypeKind { get; }

		public virtual SpecialType SpecialType => SpecialType.None;

		internal PrimitiveTypeCode PrimitiveTypeCode => SpecialTypes.GetTypeCode(SpecialType);

		public virtual bool IsTupleType => false;

		public virtual NamedTypeSymbol TupleUnderlyingType => null;

		public virtual ImmutableArray<FieldSymbol> TupleElements => default(ImmutableArray<FieldSymbol>);

		public virtual ImmutableArray<TypeSymbol> TupleElementTypes => default(ImmutableArray<TypeSymbol>);

		public virtual ImmutableArray<string> TupleElementNames => default(ImmutableArray<string>);

		protected override int HighestPriorityUseSiteError => 30649;

		public sealed override bool HasUnsupportedMetadata
		{
			get
			{
				DiagnosticInfo diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					return diagnosticInfo.Code == 30649;
				}
				return false;
			}
		}

		private ImmutableArray<INamedTypeSymbol> ITypeSymbol_AllInterfaces => StaticCast<INamedTypeSymbol>.From(AllInterfacesNoUseSiteDiagnostics);

		private INamedTypeSymbol ITypeSymbol_BaseType => BaseTypeNoUseSiteDiagnostics;

		private ImmutableArray<INamedTypeSymbol> ITypeSymbol_Interfaces => StaticCast<INamedTypeSymbol>.From(InterfacesNoUseSiteDiagnostics);

		private ITypeSymbol ITypeSymbol_OriginalDefinition => OriginalDefinition;

		private bool ITypeSymbol_IsTupleSymbol => IsTupleType;

		private bool ITypeSymbol_IsNativeIntegerType => false;

		private TypeKind ITypeSymbol_TypeKind => TypeKind;

		private bool ITypeSymbol_IsRefLikeType => false;

		private bool ITypeSymbol_IsUnmanagedType => false;

		private bool ITypeSymbol_IsReadOnly => false;

		private bool ITypeSymbol_IsRecord => false;

		private ConcurrentDictionary<Symbol, Symbol> ImplementationForInterfaceMemberMap
		{
			get
			{
				ConcurrentDictionary<Symbol, Symbol> lazyImplementationForInterfaceMemberMap = _lazyImplementationForInterfaceMemberMap;
				if (lazyImplementationForInterfaceMemberMap != null)
				{
					return lazyImplementationForInterfaceMemberMap;
				}
				lazyImplementationForInterfaceMemberMap = new ConcurrentDictionary<Symbol, Symbol>(1, 1);
				return Interlocked.CompareExchange(ref _lazyImplementationForInterfaceMemberMap, lazyImplementationForInterfaceMemberMap, null) ?? lazyImplementationForInterfaceMemberMap;
			}
		}

		internal virtual MultiDictionary<Symbol, Symbol> ExplicitInterfaceImplementationMap
		{
			get
			{
				if (m_lazyExplicitInterfaceImplementationMap == null)
				{
					Interlocked.CompareExchange(ref m_lazyExplicitInterfaceImplementationMap, MakeExplicitInterfaceImplementationMap(), null);
				}
				return m_lazyExplicitInterfaceImplementationMap;
			}
		}

		private NullableAnnotation ITypeSymbol_NullableAnnotation => NullableAnnotation.None;

		internal NamedTypeSymbol BaseTypeWithDefinitionUseSiteDiagnostics([In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
			if ((object)baseTypeNoUseSiteDiagnostics != null)
			{
				TypeSymbolExtensions.AddUseSiteInfo(baseTypeNoUseSiteDiagnostics.OriginalDefinition, ref useSiteInfo);
			}
			return baseTypeNoUseSiteDiagnostics;
		}

		internal NamedTypeSymbol BaseTypeOriginalDefinition([In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol namedTypeSymbol = BaseTypeNoUseSiteDiagnostics;
			if ((object)namedTypeSymbol != null)
			{
				namedTypeSymbol = namedTypeSymbol.OriginalDefinition;
				TypeSymbolExtensions.AddUseSiteInfo(namedTypeSymbol, ref useSiteInfo);
			}
			return namedTypeSymbol;
		}

		internal ImmutableArray<NamedTypeSymbol> AllInterfacesWithDefinitionUseSiteDiagnostics([In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<NamedTypeSymbol> allInterfacesNoUseSiteDiagnostics = AllInterfacesNoUseSiteDiagnostics;
			TypeSymbolExtensions.AddUseSiteDiagnosticsForBaseDefinitions(this, ref useSiteInfo);
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = allInterfacesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbolExtensions.AddUseSiteInfo(enumerator.Current.OriginalDefinition, ref useSiteInfo);
			}
			return allInterfacesNoUseSiteDiagnostics;
		}

		protected virtual ImmutableArray<NamedTypeSymbol> MakeAllInterfaces()
		{
			ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
			HashSet<NamedTypeSymbol> visited = new HashSet<NamedTypeSymbol>();
			TypeSymbol typeSymbol = this;
			while ((object)typeSymbol != null)
			{
				ImmutableArray<NamedTypeSymbol> interfacesNoUseSiteDiagnostics = typeSymbol.InterfacesNoUseSiteDiagnostics;
				for (int i = interfacesNoUseSiteDiagnostics.Length - 1; i >= 0; i += -1)
				{
					MakeAllInterfacesInternal(interfacesNoUseSiteDiagnostics[i], visited, instance);
				}
				typeSymbol = typeSymbol.BaseTypeNoUseSiteDiagnostics;
			}
			instance.ReverseContents();
			return instance.ToImmutableAndFree();
		}

		private static void MakeAllInterfacesInternal(NamedTypeSymbol i, HashSet<NamedTypeSymbol> visited, ArrayBuilder<NamedTypeSymbol> result)
		{
			if (visited.Add(i))
			{
				ImmutableArray<NamedTypeSymbol> interfacesNoUseSiteDiagnostics = i.InterfacesNoUseSiteDiagnostics;
				for (int j = interfacesNoUseSiteDiagnostics.Length - 1; j >= 0; j += -1)
				{
					MakeAllInterfacesInternal(interfacesNoUseSiteDiagnostics[j], visited, result);
				}
				result.Add(i);
			}
		}

		private static MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> MakeInterfacesAndTheirBaseInterfaces(ImmutableArray<NamedTypeSymbol> declaredInterfaces)
		{
			if (declaredInterfaces.IsEmpty)
			{
				return EmptyInterfacesAndTheirBaseInterfaces;
			}
			MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> multiDictionary = new MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>(declaredInterfaces.Length, EqualsIgnoringComparer.InstanceCLRSignatureCompare);
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = declaredInterfaces.GetEnumerator();
			while (enumerator.MoveNext())
			{
				NamedTypeSymbol current = enumerator.Current;
				if (multiDictionary.Add(current, current))
				{
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = current.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						NamedTypeSymbol current2 = enumerator2.Current;
						multiDictionary.Add(current2, current2);
					}
				}
			}
			return multiDictionary;
		}

		internal TypeSymbol()
		{
		}

		internal abstract TypeWithModifiers InternalSubstituteTypeParameters(TypeSubstitution substitution);

		[Obsolete("Use TypeWithModifiers.Is method.", true)]
		internal bool Equals(TypeWithModifiers other)
		{
			return other.Is(this);
		}

		[Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", true)]
		public static bool operator ==(TypeSymbol left, TypeSymbol right)
		{
			throw ExceptionUtilities.Unreachable;
		}

		[Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", true)]
		public static bool operator !=(TypeSymbol left, TypeSymbol right)
		{
			throw ExceptionUtilities.Unreachable;
		}

		[Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", true)]
		public static bool operator ==(Symbol left, TypeSymbol right)
		{
			throw ExceptionUtilities.Unreachable;
		}

		[Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", true)]
		public static bool operator !=(Symbol left, TypeSymbol right)
		{
			throw ExceptionUtilities.Unreachable;
		}

		[Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", true)]
		public static bool operator ==(TypeSymbol left, Symbol right)
		{
			throw ExceptionUtilities.Unreachable;
		}

		[Obsolete("Use TypeSymbol.Equals(TypeSymbol, TypeSymbol, TypeCompareKind) method.", true)]
		public static bool operator !=(TypeSymbol left, Symbol right)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public static bool Equals(TypeSymbol left, TypeSymbol right, TypeCompareKind comparison)
		{
			return left?.Equals(right, comparison) ?? ((object)right == null);
		}

		public sealed override bool Equals(object obj)
		{
			return Equals(obj as TypeSymbol, TypeCompareKind.ConsiderEverything);
		}

		public sealed override bool Equals(Symbol other, TypeCompareKind compareKind)
		{
			return Equals(other as TypeSymbol, compareKind);
		}

		public abstract override int GetHashCode();

		public abstract bool Equals(TypeSymbol other, TypeCompareKind comparison);

		internal virtual NamedTypeSymbol LookupMetadataType(ref MetadataTypeName emittedTypeName)
		{
			NamedTypeSymbol namedTypeSymbol = null;
			if (Kind != SymbolKind.ErrorType)
			{
				if (emittedTypeName.IsMangled && (emittedTypeName.ForcedArity == -1 || emittedTypeName.ForcedArity == emittedTypeName.InferredArity))
				{
					ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = GetTypeMembers(emittedTypeName.UnmangledTypeName).GetEnumerator();
					while (enumerator.MoveNext())
					{
						NamedTypeSymbol current = enumerator.Current;
						if (emittedTypeName.InferredArity == current.Arity && current.MangleName && string.Equals(current.Name, emittedTypeName.UnmangledTypeName, StringComparison.Ordinal))
						{
							if ((object)namedTypeSymbol != null)
							{
								namedTypeSymbol = null;
								break;
							}
							namedTypeSymbol = current;
						}
					}
				}
				int num = emittedTypeName.ForcedArity;
				if (emittedTypeName.UseCLSCompliantNameArityEncoding)
				{
					if (emittedTypeName.InferredArity > 0)
					{
						goto IL_0111;
					}
					if (num == -1)
					{
						num = 0;
					}
					else if (num != 0)
					{
						goto IL_0111;
					}
				}
				ImmutableArray<NamedTypeSymbol>.Enumerator enumerator2 = GetTypeMembers(emittedTypeName.TypeName).GetEnumerator();
				while (enumerator2.MoveNext())
				{
					NamedTypeSymbol current2 = enumerator2.Current;
					if (!current2.MangleName && (num == -1 || num == current2.Arity) && string.Equals(current2.Name, emittedTypeName.TypeName, StringComparison.Ordinal))
					{
						if ((object)namedTypeSymbol != null)
						{
							namedTypeSymbol = null;
							break;
						}
						namedTypeSymbol = current2;
					}
				}
			}
			goto IL_0111;
			IL_0111:
			if ((object)namedTypeSymbol == null)
			{
				return new MissingMetadataTypeSymbol.Nested((NamedTypeSymbol)this, ref emittedTypeName);
			}
			return namedTypeSymbol;
		}

		internal virtual NamedTypeSymbol GetDirectBaseTypeNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			return BaseTypeNoUseSiteDiagnostics;
		}

		internal virtual NamedTypeSymbol GetDirectBaseTypeWithDefinitionUseSiteDiagnostics(BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			NamedTypeSymbol directBaseTypeNoUseSiteDiagnostics = GetDirectBaseTypeNoUseSiteDiagnostics(basesBeingResolved);
			if ((object)directBaseTypeNoUseSiteDiagnostics != null)
			{
				TypeSymbolExtensions.AddUseSiteInfo(directBaseTypeNoUseSiteDiagnostics.OriginalDefinition, ref useSiteInfo);
			}
			return directBaseTypeNoUseSiteDiagnostics;
		}

		public virtual bool IsTupleCompatible(out int tupleCardinality)
		{
			tupleCardinality = 0;
			return false;
		}

		public bool IsTupleCompatible()
		{
			int tupleCardinality;
			return IsTupleCompatible(out tupleCardinality);
		}

		public bool IsTupleOrCompatibleWithTupleOfCardinality(int targetCardinality)
		{
			if (IsTupleType)
			{
				return TupleElementTypes.Length == targetCardinality;
			}
			int tupleCardinality;
			return IsTupleCompatible(out tupleCardinality) && tupleCardinality == targetCardinality;
		}

		internal abstract DiagnosticInfo GetUnificationUseSiteDiagnosticRecursive(Symbol owner, ref HashSet<TypeSymbol> checkedTypes);

		private ISymbol ITypeSymbol_FindImplementationForInterfaceMember(ISymbol interfaceMember)
		{
			if (!(interfaceMember is Symbol))
			{
				return null;
			}
			return FindImplementationForInterfaceMember((Symbol)interfaceMember);
		}

		ISymbol ITypeSymbol.FindImplementationForInterfaceMember(ISymbol interfaceMember)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbol_FindImplementationForInterfaceMember
			return this.ITypeSymbol_FindImplementationForInterfaceMember(interfaceMember);
		}

		private string ITypeSymbol_ToDisplayString(NullableFlowState topLevelNullability, SymbolDisplayFormat format = null)
		{
			return ToDisplayString(format);
		}

		string ITypeSymbol.ToDisplayString(NullableFlowState topLevelNullability, SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbol_ToDisplayString
			return this.ITypeSymbol_ToDisplayString(topLevelNullability, format);
		}

		private ImmutableArray<SymbolDisplayPart> ITypeSymbol_ToDisplayParts(NullableFlowState topLevelNullability, SymbolDisplayFormat format = null)
		{
			return ToDisplayParts(format);
		}

		ImmutableArray<SymbolDisplayPart> ITypeSymbol.ToDisplayParts(NullableFlowState topLevelNullability, SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbol_ToDisplayParts
			return this.ITypeSymbol_ToDisplayParts(topLevelNullability, format);
		}

		private string ITypeSymbol_ToMinimalDisplayString(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat format = null)
		{
			return ToMinimalDisplayString(semanticModel, position, format);
		}

		string ITypeSymbol.ToMinimalDisplayString(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbol_ToMinimalDisplayString
			return this.ITypeSymbol_ToMinimalDisplayString(semanticModel, topLevelNullability, position, format);
		}

		private ImmutableArray<SymbolDisplayPart> ITypeSymbol_ToMinimalDisplayParts(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat format = null)
		{
			return ToMinimalDisplayParts(semanticModel, position, format);
		}

		ImmutableArray<SymbolDisplayPart> ITypeSymbol.ToMinimalDisplayParts(SemanticModel semanticModel, NullableFlowState topLevelNullability, int position, SymbolDisplayFormat format = null)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbol_ToMinimalDisplayParts
			return this.ITypeSymbol_ToMinimalDisplayParts(semanticModel, topLevelNullability, position, format);
		}

		public Symbol FindImplementationForInterfaceMember(Symbol interfaceMember)
		{
			if ((object)interfaceMember == null)
			{
				throw new ArgumentNullException("interfaceMember");
			}
			if (SymbolExtensions.RequiresImplementation(interfaceMember) && !TypeSymbolExtensions.IsInterfaceType(this))
			{
				NamedTypeSymbol containingType = interfaceMember.ContainingType;
				EqualsIgnoringComparer instanceCLRSignatureCompare = EqualsIgnoringComparer.InstanceCLRSignatureCompare;
				CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
				if (TypeSymbolExtensions.ImplementsInterface(this, containingType, instanceCLRSignatureCompare, ref useSiteInfo))
				{
					ConcurrentDictionary<Symbol, Symbol> implementationForInterfaceMemberMap = ImplementationForInterfaceMemberMap;
					Symbol value = null;
					if (implementationForInterfaceMemberMap.TryGetValue(interfaceMember, out value))
					{
						return value;
					}
					value = ComputeImplementationForInterfaceMember(interfaceMember);
					implementationForInterfaceMemberMap.TryAdd(interfaceMember, value);
					return value;
				}
			}
			return null;
		}

		private Symbol ComputeImplementationForInterfaceMember(Symbol interfaceMember)
		{
			return interfaceMember.Kind switch
			{
				SymbolKind.Method => ImplementsHelper.ComputeImplementationForInterfaceMember((MethodSymbol)interfaceMember, this, MethodSignatureComparer.RuntimeMethodSignatureComparer), 
				SymbolKind.Property => ImplementsHelper.ComputeImplementationForInterfaceMember((PropertySymbol)interfaceMember, this, PropertySignatureComparer.RuntimePropertySignatureComparer), 
				SymbolKind.Event => ImplementsHelper.ComputeImplementationForInterfaceMember((EventSymbol)interfaceMember, this, EventSignatureComparer.RuntimeEventSignatureComparer), 
				_ => null, 
			};
		}

		private MultiDictionary<Symbol, Symbol> MakeExplicitInterfaceImplementationMap()
		{
			if (TypeSymbolExtensions.IsClassType(this) || TypeSymbolExtensions.IsStructureType(this))
			{
				MultiDictionary<Symbol, Symbol> multiDictionary = new MultiDictionary<Symbol, Symbol>(ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance);
				ImmutableArray<Symbol>.Enumerator enumerator = GetMembersUnordered().GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					ImmutableArray<Symbol>.Enumerator enumerator2 = ImplementsHelper.GetExplicitInterfaceImplementations(current).GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Symbol current2 = enumerator2.Current;
						multiDictionary.Add(current2, current);
					}
				}
				if (multiDictionary.Count > 0)
				{
					return multiDictionary;
				}
				return EmptyExplicitImplementationMap;
			}
			return EmptyExplicitImplementationMap;
		}

		private ITypeSymbol ITypeSymbol_WithNullability(NullableAnnotation nullableAnnotation)
		{
			return this;
		}

		ITypeSymbol ITypeSymbol.WithNullableAnnotation(NullableAnnotation nullableAnnotation)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbol_WithNullability
			return this.ITypeSymbol_WithNullability(nullableAnnotation);
		}

		private ITypeSymbol ITypeSymbolInternal_GetITypeSymbol()
		{
			return this;
		}

		ITypeSymbol ITypeSymbolInternal.GetITypeSymbol()
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeSymbolInternal_GetITypeSymbol
			return this.ITypeSymbolInternal_GetITypeSymbol();
		}
	}
}
