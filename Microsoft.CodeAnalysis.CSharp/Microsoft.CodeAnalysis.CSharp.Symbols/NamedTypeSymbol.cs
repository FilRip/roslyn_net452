using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Emit.NoPia;
using Microsoft.CodeAnalysis.CSharp.Symbols.Metadata.PE;
using Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.RuntimeMembers;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class NamedTypeSymbol : TypeSymbol, ITypeReference, IReference, ITypeDefinition, IDefinition, INamedTypeReference, INamedEntity, INamedTypeDefinition, INamespaceTypeReference, INamespaceTypeDefinition, INestedTypeReference, ITypeMemberReference, INestedTypeDefinition, ITypeDefinitionMember, IGenericTypeInstanceReference, ISpecializedNestedTypeReference, INamedTypeSymbolInternal, ITypeSymbolInternal, INamespaceOrTypeSymbolInternal, ISymbolInternal
    {
        public sealed class TupleExtraData
        {
            private ImmutableArray<TypeWithAnnotations> _lazyElementTypes;

            private ImmutableArray<FieldSymbol> _lazyDefaultElementFields;

            private SmallDictionary<FieldSymbol, int>? _lazyFieldDefinitionsToIndexMap;

            private SmallDictionary<Symbol, Symbol>? _lazyUnderlyingDefinitionToMemberMap;

            internal ImmutableArray<string?> ElementNames { get; }

            internal ImmutableArray<Location?> ElementLocations { get; }

            internal ImmutableArray<bool> ErrorPositions { get; }

            internal ImmutableArray<Location> Locations { get; }

            internal NamedTypeSymbol TupleUnderlyingType { get; }

            internal SmallDictionary<Symbol, Symbol> UnderlyingDefinitionToMemberMap
            {
                get
                {
                    return _lazyUnderlyingDefinitionToMemberMap ?? (_lazyUnderlyingDefinitionToMemberMap = computeDefinitionToMemberMap());
                    SmallDictionary<Symbol, Symbol> computeDefinitionToMemberMap()
                    {
                        SmallDictionary<Symbol, Symbol> smallDictionary = new SmallDictionary<Symbol, Symbol>(ReferenceEqualityComparer.Instance);
                        ImmutableArray<Symbol> members = TupleUnderlyingType.GetMembers();
                        for (int num = members.Length - 1; num >= 0; num--)
                        {
                            Symbol symbol = members[num];
                            switch (symbol.Kind)
                            {
                                case SymbolKind.Method:
                                case SymbolKind.NamedType:
                                case SymbolKind.Property:
                                    smallDictionary.Add(symbol.OriginalDefinition, symbol);
                                    break;
                                case SymbolKind.Field:
                                    {
                                        FieldSymbol tupleUnderlyingField = ((FieldSymbol)symbol).TupleUnderlyingField;
                                        if ((object)tupleUnderlyingField != null)
                                        {
                                            smallDictionary[tupleUnderlyingField.OriginalDefinition] = symbol;
                                        }
                                        break;
                                    }
                                case SymbolKind.Event:
                                    {
                                        EventSymbol eventSymbol = (EventSymbol)symbol;
                                        FieldSymbol associatedField = eventSymbol.AssociatedField;
                                        if ((object)associatedField != null)
                                        {
                                            smallDictionary.Add(associatedField.OriginalDefinition, associatedField);
                                        }
                                        smallDictionary.Add(eventSymbol.OriginalDefinition, symbol);
                                        break;
                                    }
                                default:
                                    throw ExceptionUtilities.UnexpectedValue(symbol.Kind);
                            }
                        }
                        return smallDictionary;
                    }
                }
            }

            public TupleExtraData(NamedTypeSymbol underlyingType)
            {
                TupleUnderlyingType = underlyingType;
                Locations = ImmutableArray<Location>.Empty;
            }

            public TupleExtraData(NamedTypeSymbol underlyingType, ImmutableArray<string?> elementNames, ImmutableArray<Location?> elementLocations, ImmutableArray<bool> errorPositions, ImmutableArray<Location> locations)
                : this(underlyingType)
            {
                ElementNames = elementNames;
                ElementLocations = elementLocations;
                ErrorPositions = errorPositions;
                Locations = locations.NullToEmpty();
            }

            internal bool EqualsIgnoringTupleUnderlyingType(TupleExtraData? other)
            {
                if (other == null && ElementNames.IsDefault && ElementLocations.IsDefault && ErrorPositions.IsDefault)
                {
                    return true;
                }
                if (other != null && areEqual<string>(ElementNames, other!.ElementNames) && areEqual<Location>(ElementLocations, other!.ElementLocations))
                {
                    return areEqual<bool>(ErrorPositions, other!.ErrorPositions);
                }
                return false;
                static bool areEqual<T>(ImmutableArray<T> one, ImmutableArray<T> other)
                {
                    if (one.IsDefault && other.IsDefault)
                    {
                        return true;
                    }
                    if (one.IsDefault != other.IsDefault)
                    {
                        return false;
                    }
                    return one.SequenceEqual(other);
                }
            }

            public ImmutableArray<TypeWithAnnotations> TupleElementTypesWithAnnotations(NamedTypeSymbol tuple)
            {
                if (_lazyElementTypes.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyElementTypes, collectTupleElementTypesWithAnnotations(tuple));
                }
                return _lazyElementTypes;
                static ImmutableArray<TypeWithAnnotations> collectTupleElementTypesWithAnnotations(NamedTypeSymbol tuple)
                {
                    if (tuple.Arity == 8)
                    {
                        ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = tuple.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type.TupleElementTypesWithAnnotations;
                        ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance(7 + tupleElementTypesWithAnnotations.Length);
                        instance.AddRange(tuple.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics, 7);
                        instance.AddRange(tupleElementTypesWithAnnotations);
                        return instance.ToImmutableAndFree();
                    }
                    return tuple.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                }
            }

            public ImmutableArray<FieldSymbol> TupleElements(NamedTypeSymbol tuple)
            {
                if (_lazyDefaultElementFields.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _lazyDefaultElementFields, collectTupleElementFields(tuple));
                }
                return _lazyDefaultElementFields;
                ImmutableArray<FieldSymbol> collectTupleElementFields(NamedTypeSymbol tuple)
                {
                    ArrayBuilder<FieldSymbol> instance = ArrayBuilder<FieldSymbol>.GetInstance(TupleElementTypesWithAnnotations(tuple).Length, null);
                    ImmutableArray<Symbol>.Enumerator enumerator = tuple.GetMembers().GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbol current = enumerator.Current;
                        if (current.Kind == SymbolKind.Field)
                        {
                            FieldSymbol fieldSymbol = (FieldSymbol)current;
                            int tupleElementIndex = fieldSymbol.TupleElementIndex;
                            if (tupleElementIndex >= 0)
                            {
                                FieldSymbol fieldSymbol2 = instance[tupleElementIndex];
                                if ((object)fieldSymbol2 == null || fieldSymbol2.IsDefaultTupleElement)
                                {
                                    instance[tupleElementIndex] = fieldSymbol;
                                }
                            }
                        }
                    }
                    return instance.ToImmutableAndFree();
                }
            }

            internal SmallDictionary<FieldSymbol, int> GetFieldDefinitionsToIndexMap(NamedTypeSymbol tuple)
            {
                if (_lazyFieldDefinitionsToIndexMap == null)
                {
                    tuple.InitializeTupleFieldDefinitionsToIndexMap();
                }
                return _lazyFieldDefinitionsToIndexMap;
            }

            internal void SetFieldDefinitionsToIndexMap(SmallDictionary<FieldSymbol, int> map)
            {
                Interlocked.CompareExchange(ref _lazyFieldDefinitionsToIndexMap, map, null);
            }

            public TMember? GetTupleMemberSymbolForUnderlyingMember<TMember>(TMember? underlyingMemberOpt) where TMember : Symbol
            {
                if (underlyingMemberOpt == null)
                {
                    return null;
                }
                Symbol symbol = underlyingMemberOpt!.OriginalDefinition;
                if (symbol is TupleElementFieldSymbol tupleElementFieldSymbol)
                {
                    symbol = tupleElementFieldSymbol.UnderlyingField;
                }
                if (TypeSymbol.Equals(symbol.ContainingType, TupleUnderlyingType.OriginalDefinition, TypeCompareKind.ConsiderEverything) && UnderlyingDefinitionToMemberMap.TryGetValue(symbol, out var value))
                {
                    return (TMember)value;
                }
                return null;
            }
        }

        private bool _hasNoBaseCycles;

        protected static Func<Symbol, bool> IsInstanceFieldOrEvent = delegate (Symbol symbol)
        {
            if (!symbol.IsStatic)
            {
                SymbolKind kind = symbol.Kind;
                if ((uint)(kind - 5) <= 1u)
                {
                    return true;
                }
            }
            return false;
        };

        internal static readonly Func<TypeWithAnnotations, bool> TypeWithAnnotationsIsNullFunction = (TypeWithAnnotations type) => !type.HasType;

        internal static readonly Func<TypeWithAnnotations, bool> TypeWithAnnotationsIsErrorType = (TypeWithAnnotations type) => type.HasType && type.Type.IsErrorType();

        internal const int ValueTupleRestPosition = 8;

        internal const int ValueTupleRestIndex = 7;

        internal const string ValueTupleTypeName = "ValueTuple";

        internal const string ValueTupleRestFieldName = "Rest";

        private TupleExtraData? _lazyTupleData;

        private static readonly WellKnownType[] tupleTypes = new WellKnownType[8]
        {
            WellKnownType.System_ValueTuple_T1,
            WellKnownType.System_ValueTuple_T2,
            WellKnownType.System_ValueTuple_T3,
            WellKnownType.System_ValueTuple_T4,
            WellKnownType.System_ValueTuple_T5,
            WellKnownType.System_ValueTuple_T6,
            WellKnownType.System_ValueTuple_T7,
            WellKnownType.System_ValueTuple_TRest
        };

        private static readonly WellKnownMember[] tupleCtors = new WellKnownMember[8]
        {
            WellKnownMember.System_ValueTuple_T1__ctor,
            WellKnownMember.System_ValueTuple_T2__ctor,
            WellKnownMember.System_ValueTuple_T3__ctor,
            WellKnownMember.System_ValueTuple_T4__ctor,
            WellKnownMember.System_ValueTuple_T5__ctor,
            WellKnownMember.System_ValueTuple_T6__ctor,
            WellKnownMember.System_ValueTuple_T7__ctor,
            WellKnownMember.System_ValueTuple_TRest__ctor
        };

        private static readonly WellKnownMember[][] tupleMembers = new WellKnownMember[8][]
        {
            new WellKnownMember[1] { WellKnownMember.System_ValueTuple_T1__Item1 },
            new WellKnownMember[2]
            {
                WellKnownMember.System_ValueTuple_T2__Item1,
                WellKnownMember.System_ValueTuple_T2__Item2
            },
            new WellKnownMember[3]
            {
                WellKnownMember.System_ValueTuple_T3__Item1,
                WellKnownMember.System_ValueTuple_T3__Item2,
                WellKnownMember.System_ValueTuple_T3__Item3
            },
            new WellKnownMember[4]
            {
                WellKnownMember.System_ValueTuple_T4__Item1,
                WellKnownMember.System_ValueTuple_T4__Item2,
                WellKnownMember.System_ValueTuple_T4__Item3,
                WellKnownMember.System_ValueTuple_T4__Item4
            },
            new WellKnownMember[5]
            {
                WellKnownMember.System_ValueTuple_T5__Item1,
                WellKnownMember.System_ValueTuple_T5__Item2,
                WellKnownMember.System_ValueTuple_T5__Item3,
                WellKnownMember.System_ValueTuple_T5__Item4,
                WellKnownMember.System_ValueTuple_T5__Item5
            },
            new WellKnownMember[6]
            {
                WellKnownMember.System_ValueTuple_T6__Item1,
                WellKnownMember.System_ValueTuple_T6__Item2,
                WellKnownMember.System_ValueTuple_T6__Item3,
                WellKnownMember.System_ValueTuple_T6__Item4,
                WellKnownMember.System_ValueTuple_T6__Item5,
                WellKnownMember.System_ValueTuple_T6__Item6
            },
            new WellKnownMember[7]
            {
                WellKnownMember.System_ValueTuple_T7__Item1,
                WellKnownMember.System_ValueTuple_T7__Item2,
                WellKnownMember.System_ValueTuple_T7__Item3,
                WellKnownMember.System_ValueTuple_T7__Item4,
                WellKnownMember.System_ValueTuple_T7__Item5,
                WellKnownMember.System_ValueTuple_T7__Item6,
                WellKnownMember.System_ValueTuple_T7__Item7
            },
            new WellKnownMember[8]
            {
                WellKnownMember.System_ValueTuple_TRest__Item1,
                WellKnownMember.System_ValueTuple_TRest__Item2,
                WellKnownMember.System_ValueTuple_TRest__Item3,
                WellKnownMember.System_ValueTuple_TRest__Item4,
                WellKnownMember.System_ValueTuple_TRest__Item5,
                WellKnownMember.System_ValueTuple_TRest__Item6,
                WellKnownMember.System_ValueTuple_TRest__Item7,
                WellKnownMember.System_ValueTuple_TRest__Rest
            }
        };

        bool ITypeReference.IsEnum => AdaptedNamedTypeSymbol.TypeKind == TypeKind.Enum;

        bool ITypeReference.IsValueType => AdaptedNamedTypeSymbol.IsValueType;

        Microsoft.Cci.PrimitiveTypeCode ITypeReference.TypeCode
        {
            get
            {
                if (AdaptedNamedTypeSymbol.IsDefinition)
                {
                    return AdaptedNamedTypeSymbol.PrimitiveTypeCode;
                }
                return Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;
            }
        }

        TypeDefinitionHandle ITypeReference.TypeDef
        {
            get
            {
                if (AdaptedNamedTypeSymbol is PENamedTypeSymbol pENamedTypeSymbol)
                {
                    return pENamedTypeSymbol.Handle;
                }
                return default(TypeDefinitionHandle);
            }
        }

        IGenericMethodParameterReference ITypeReference.AsGenericMethodParameterReference => null;

        IGenericTypeInstanceReference ITypeReference.AsGenericTypeInstanceReference
        {
            get
            {
                if (!AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.Arity > 0)
                {
                    return this;
                }
                return null;
            }
        }

        IGenericTypeParameterReference ITypeReference.AsGenericTypeParameterReference => null;

        INamespaceTypeReference ITypeReference.AsNamespaceTypeReference
        {
            get
            {
                if (AdaptedNamedTypeSymbol.IsDefinition && (object)AdaptedNamedTypeSymbol.ContainingType == null)
                {
                    return this;
                }
                return null;
            }
        }

        INestedTypeReference ITypeReference.AsNestedTypeReference
        {
            get
            {
                if ((object)AdaptedNamedTypeSymbol.ContainingType != null)
                {
                    return this;
                }
                return null;
            }
        }

        ISpecializedNestedTypeReference ITypeReference.AsSpecializedNestedTypeReference
        {
            get
            {
                if (!AdaptedNamedTypeSymbol.IsDefinition && (AdaptedNamedTypeSymbol.Arity == 0 || PEModuleBuilder.IsGenericType(AdaptedNamedTypeSymbol.ContainingType)))
                {
                    return this;
                }
                return null;
            }
        }

        IEnumerable<IGenericTypeParameter> ITypeDefinition.GenericParameters
        {
            get
            {
                ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.TypeParameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeParameterSymbol current = enumerator.Current;
                    yield return current.GetCciAdapter();
                }
            }
        }

        ushort ITypeDefinition.GenericParameterCount => GenericParameterCountImpl;

        private ushort GenericParameterCountImpl => (ushort)AdaptedNamedTypeSymbol.Arity;

        bool ITypeDefinition.IsAbstract => AdaptedNamedTypeSymbol.IsMetadataAbstract;

        bool ITypeDefinition.IsBeforeFieldInit
        {
            get
            {
                switch (AdaptedNamedTypeSymbol.TypeKind)
                {
                    case TypeKind.Delegate:
                    case TypeKind.Enum:
                    case TypeKind.Interface:
                        return false;
                    default:
                        {
                            ImmutableArray<Symbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.GetMembers(".cctor").GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (!enumerator.Current.IsImplicitlyDeclared)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                }
            }
        }

        bool ITypeDefinition.IsComObject => AdaptedNamedTypeSymbol.IsComImport;

        bool ITypeDefinition.IsGeneric => AdaptedNamedTypeSymbol.Arity != 0;

        bool ITypeDefinition.IsInterface => AdaptedNamedTypeSymbol.IsInterface;

        bool ITypeDefinition.IsDelegate => AdaptedNamedTypeSymbol.IsDelegateType();

        bool ITypeDefinition.IsRuntimeSpecial => false;

        bool ITypeDefinition.IsSerializable => AdaptedNamedTypeSymbol.IsSerializable;

        bool ITypeDefinition.IsSpecialName => AdaptedNamedTypeSymbol.HasSpecialName;

        bool ITypeDefinition.IsWindowsRuntimeImport => AdaptedNamedTypeSymbol.IsWindowsRuntimeImport;

        bool ITypeDefinition.IsSealed => AdaptedNamedTypeSymbol.IsMetadataSealed;

        bool ITypeDefinition.HasDeclarativeSecurity => AdaptedNamedTypeSymbol.HasDeclarativeSecurity;

        IEnumerable<SecurityAttribute> ITypeDefinition.SecurityAttributes => AdaptedNamedTypeSymbol.GetSecurityInformation() ?? SpecializedCollections.EmptyEnumerable<SecurityAttribute>();

        ushort ITypeDefinition.Alignment => (ushort)AdaptedNamedTypeSymbol.Layout.Alignment;

        LayoutKind ITypeDefinition.Layout => AdaptedNamedTypeSymbol.Layout.Kind;

        uint ITypeDefinition.SizeOf => (uint)AdaptedNamedTypeSymbol.Layout.Size;

        CharSet ITypeDefinition.StringFormat => AdaptedNamedTypeSymbol.MarshallingCharSet;

        ushort INamedTypeReference.GenericParameterCount => GenericParameterCountImpl;

        bool INamedTypeReference.MangleName => AdaptedNamedTypeSymbol.MangleName;

        string INamedEntity.Name => AdaptedNamedTypeSymbol.Name;

        string INamespaceTypeReference.NamespaceName => AdaptedNamedTypeSymbol.ContainingNamespace.QualifiedName;

        bool INamespaceTypeDefinition.IsPublic => PEModuleBuilder.MemberVisibility(AdaptedNamedTypeSymbol) == TypeMemberVisibility.Public;

        ITypeDefinition ITypeDefinitionMember.ContainingTypeDefinition => AdaptedNamedTypeSymbol.ContainingType.GetCciAdapter();

        TypeMemberVisibility ITypeDefinitionMember.Visibility => PEModuleBuilder.MemberVisibility(AdaptedNamedTypeSymbol);

        internal NamedTypeSymbol AdaptedNamedTypeSymbol => this;

        internal virtual bool IsMetadataAbstract
        {
            get
            {
                if (!IsAbstract)
                {
                    return IsStatic;
                }
                return true;
            }
        }

        internal virtual bool IsMetadataSealed
        {
            get
            {
                if (!IsSealed)
                {
                    return IsStatic;
                }
                return true;
            }
        }

        public abstract int Arity { get; }

        public abstract ImmutableArray<TypeParameterSymbol> TypeParameters { get; }

        internal abstract ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics { get; }

        public abstract NamedTypeSymbol ConstructedFrom { get; }

        public virtual NamedTypeSymbol EnumUnderlyingType => null;

        public override NamedTypeSymbol ContainingType => ContainingSymbol as NamedTypeSymbol;

        internal virtual bool KnownCircularStruct => false;

        internal bool KnownToHaveNoDeclaredBaseCycles => _hasNoBaseCycles;

        internal virtual bool IsExplicitDefinitionOfNoPiaLocalType => false;

        public MethodSymbol DelegateInvokeMethod
        {
            get
            {
                if (TypeKind != TypeKind.Delegate)
                {
                    return null;
                }
                ImmutableArray<Symbol> members = GetMembers("Invoke");
                if (members.Length != 1)
                {
                    return null;
                }
                return members[0] as MethodSymbol;
            }
        }

        public ImmutableArray<MethodSymbol> InstanceConstructors => GetConstructors(includeInstance: true, includeStatic: false);

        public ImmutableArray<MethodSymbol> StaticConstructors => GetConstructors(includeInstance: false, includeStatic: true);

        public ImmutableArray<MethodSymbol> Constructors => GetConstructors(includeInstance: true, includeStatic: true);

        public ImmutableArray<PropertySymbol> Indexers
        {
            get
            {
                ImmutableArray<Symbol> simpleNonTypeMembers = GetSimpleNonTypeMembers("this[]");
                if (simpleNonTypeMembers.IsEmpty)
                {
                    return ImmutableArray<PropertySymbol>.Empty;
                }
                ArrayBuilder<PropertySymbol> instance = ArrayBuilder<PropertySymbol>.GetInstance();
                ImmutableArray<Symbol>.Enumerator enumerator = simpleNonTypeMembers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    if (current.Kind == SymbolKind.Property)
                    {
                        instance.Add((PropertySymbol)current);
                    }
                }
                return instance.ToImmutableAndFree();
            }
        }

        public abstract bool MightContainExtensionMethods { get; }

        public override bool IsReferenceType
        {
            get
            {
                TypeKind typeKind = TypeKind;
                if (typeKind != TypeKind.Enum && typeKind != TypeKind.Struct)
                {
                    return typeKind != TypeKind.Error;
                }
                return false;
            }
        }

        public override bool IsValueType
        {
            get
            {
                TypeKind typeKind = TypeKind;
                if (typeKind != TypeKind.Struct)
                {
                    return typeKind == TypeKind.Enum;
                }
                return true;
            }
        }

        public virtual bool IsScriptClass => false;

        internal bool IsSubmissionClass => TypeKind == TypeKind.Submission;

        public virtual bool IsImplicitClass => false;

        public abstract override string Name { get; }

        public override string MetadataName
        {
            get
            {
                if (!MangleName)
                {
                    return Name;
                }
                return MetadataHelpers.ComposeAritySuffixedMetadataName(Name, Arity);
            }
        }

        internal abstract bool MangleName { get; }

        public abstract IEnumerable<string> MemberNames { get; }

        public abstract override Accessibility DeclaredAccessibility { get; }

        public override SymbolKind Kind => SymbolKind.NamedType;

        internal abstract bool HasCodeAnalysisEmbeddedAttribute { get; }

        public bool IsGenericType
        {
            get
            {
                NamedTypeSymbol namedTypeSymbol = this;
                while ((object)namedTypeSymbol != null)
                {
                    if (namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length != 0)
                    {
                        return true;
                    }
                    namedTypeSymbol = namedTypeSymbol.ContainingType;
                }
                return false;
            }
        }

        public virtual bool IsUnboundGenericType => false;

        public new virtual NamedTypeSymbol OriginalDefinition => this;

        protected sealed override TypeSymbol OriginalTypeSymbolDefinition => OriginalDefinition;

        internal virtual TypeMap TypeSubstitution => null;

        internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

        internal abstract bool HasSpecialName { get; }

        internal abstract bool IsComImport { get; }

        internal abstract bool IsWindowsRuntimeImport { get; }

        internal abstract bool ShouldAddWinRTMembers { get; }

        internal bool IsConditional
        {
            get
            {
                if (GetAppliedConditionalSymbols().Any())
                {
                    return true;
                }
                return BaseTypeNoUseSiteDiagnostics?.IsConditional ?? false;
            }
        }

        public abstract bool IsSerializable { get; }

        public abstract bool AreLocalsZeroed { get; }

        internal abstract TypeLayout Layout { get; }

        protected CharSet DefaultMarshallingCharSet => GetEffectiveDefaultMarshallingCharSet() ?? CharSet.Ansi;

        internal abstract CharSet MarshallingCharSet { get; }

        internal abstract bool HasDeclarativeSecurity { get; }

        internal virtual NamedTypeSymbol ComImportCoClass => null;

        internal virtual FieldSymbol FixedElementField => null;

        internal abstract bool IsInterface { get; }

        internal abstract NamedTypeSymbol NativeIntegerUnderlyingType { get; }

        INamedTypeSymbolInternal INamedTypeSymbolInternal.EnumUnderlyingType => EnumUnderlyingType;

        internal NamedTypeSymbol? TupleUnderlyingType
        {
            get
            {
                if (_lazyTupleData == null)
                {
                    if (!IsTupleType)
                    {
                        return null;
                    }
                    return this;
                }
                return TupleData!.TupleUnderlyingType;
            }
        }

        public sealed override bool IsTupleType
        {
            get
            {
                return IsTupleTypeOfCardinality(out int tupleCardinality);
            }
        }

        internal TupleExtraData? TupleData
        {
            get
            {
                if (!IsTupleType)
                {
                    return null;
                }
                if (_lazyTupleData == null)
                {
                    Interlocked.CompareExchange(ref _lazyTupleData, new TupleExtraData(this), null);
                }
                return _lazyTupleData;
            }
        }

        public sealed override ImmutableArray<string?> TupleElementNames
        {
            get
            {
                if (_lazyTupleData != null)
                {
                    return _lazyTupleData!.ElementNames;
                }
                return default(ImmutableArray<string>);
            }
        }

        private ImmutableArray<bool> TupleErrorPositions
        {
            get
            {
                if (_lazyTupleData != null)
                {
                    return _lazyTupleData!.ErrorPositions;
                }
                return default(ImmutableArray<bool>);
            }
        }

        private ImmutableArray<Location?> TupleElementLocations
        {
            get
            {
                if (_lazyTupleData != null)
                {
                    return _lazyTupleData!.ElementLocations;
                }
                return default(ImmutableArray<Location>);
            }
        }

        public sealed override ImmutableArray<TypeWithAnnotations> TupleElementTypesWithAnnotations
        {
            get
            {
                if (!IsTupleType)
                {
                    return default(ImmutableArray<TypeWithAnnotations>);
                }
                return TupleData!.TupleElementTypesWithAnnotations(this);
            }
        }

        public sealed override ImmutableArray<FieldSymbol> TupleElements
        {
            get
            {
                if (!IsTupleType)
                {
                    return default(ImmutableArray<FieldSymbol>);
                }
                return TupleData!.TupleElements(this);
            }
        }

        public SmallDictionary<FieldSymbol, int>? TupleFieldDefinitionsToIndexMap
        {
            get
            {
                if (!IsTupleType)
                {
                    return null;
                }
                if (!base.IsDefinition)
                {
                    return OriginalDefinition.TupleFieldDefinitionsToIndexMap;
                }
                return TupleData!.GetFieldDefinitionsToIndexMap(this);
            }
        }

        ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return AsTypeDefinitionImpl(moduleBeingBuilt);
        }

        INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            if ((object)AdaptedNamedTypeSymbol.ContainingType == null && AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.ContainingModule == pEModuleBuilder.SourceModule)
            {
                return this;
            }
            return null;
        }

        INestedTypeDefinition ITypeReference.AsNestedTypeDefinition(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return AsNestedTypeDefinitionImpl(moduleBeingBuilt);
        }

        private INestedTypeDefinition AsNestedTypeDefinitionImpl(PEModuleBuilder moduleBeingBuilt)
        {
            if ((object)AdaptedNamedTypeSymbol.ContainingType != null && AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.ContainingModule == moduleBeingBuilt.SourceModule)
            {
                return this;
            }
            return null;
        }

        ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return AsTypeDefinitionImpl(moduleBeingBuilt);
        }

        private ITypeDefinition AsTypeDefinitionImpl(PEModuleBuilder moduleBeingBuilt)
        {
            if (AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.ContainingModule == moduleBeingBuilt.SourceModule)
            {
                return this;
            }
            return null;
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return AsTypeDefinitionImpl(moduleBeingBuilt);
        }

        ITypeReference ITypeDefinition.GetBaseClass(EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            NamedTypeSymbol namedTypeSymbol = AdaptedNamedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
            if (AdaptedNamedTypeSymbol.IsScriptClass)
            {
                namedTypeSymbol = AdaptedNamedTypeSymbol.ContainingAssembly.GetSpecialType(SpecialType.System_Object);
            }
            if ((object)namedTypeSymbol == null)
            {
                return null;
            }
            return pEModuleBuilder.Translate(namedTypeSymbol, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        IEnumerable<IEventDefinition> ITypeDefinition.GetEvents(EmitContext context)
        {
            foreach (EventSymbol item in AdaptedNamedTypeSymbol.GetEventsToEmit())
            {
                IEventDefinition cciAdapter = item.GetCciAdapter();
                if (cciAdapter.ShouldInclude(context) || !cciAdapter.GetAccessors(context).IsEmpty())
                {
                    yield return cciAdapter;
                }
            }
        }

        IEnumerable<Microsoft.Cci.MethodImplementation> ITypeDefinition.GetExplicitImplementationOverrides(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            ImmutableArray<Symbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Method)
                {
                    continue;
                }
                MethodSymbol method = (MethodSymbol)current;
                if (method.ExplicitInterfaceImplementations.Length != 0)
                {
                    MethodSymbol adapter = method.GetCciAdapter();
                    ImmutableArray<MethodSymbol>.Enumerator enumerator2 = method.ExplicitInterfaceImplementations.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        MethodSymbol current2 = enumerator2.Current;
                        yield return new Microsoft.Cci.MethodImplementation(adapter, moduleBeingBuilt.TranslateOverriddenMethodReference(current2, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics));
                    }
                }
                if (AdaptedNamedTypeSymbol.IsInterface)
                {
                    continue;
                }
                if (method.RequiresExplicitOverride(out var _))
                {
                    yield return new Microsoft.Cci.MethodImplementation(method.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(method.OverriddenMethod, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics));
                }
                else
                {
                    if (method.MethodKind != MethodKind.Destructor || AdaptedNamedTypeSymbol.SpecialType == SpecialType.System_Object)
                    {
                        continue;
                    }
                    TypeSymbol specialType = AdaptedNamedTypeSymbol.DeclaringCompilation.GetSpecialType(SpecialType.System_Object);
                    ImmutableArray<Symbol>.Enumerator enumerator3 = specialType.GetMembers("Finalize").GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        if (enumerator3.Current is MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Destructor)
                        {
                            yield return new Microsoft.Cci.MethodImplementation(method.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(methodSymbol, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics));
                        }
                    }
                }
            }
            if (AdaptedNamedTypeSymbol.IsInterface)
            {
                yield break;
            }
            IEnumerable<IMethodDefinition> synthesizedMethods = moduleBeingBuilt.GetSynthesizedMethods(AdaptedNamedTypeSymbol);
            if (synthesizedMethods == null)
            {
                yield break;
            }
            foreach (IMethodDefinition i in synthesizedMethods)
            {
                if (i.GetInternalSymbol() is MethodSymbol methodSymbol2)
                {
                    ImmutableArray<MethodSymbol>.Enumerator enumerator2 = methodSymbol2.ExplicitInterfaceImplementations.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        MethodSymbol current3 = enumerator2.Current;
                        yield return new Microsoft.Cci.MethodImplementation(i, moduleBeingBuilt.TranslateOverriddenMethodReference(current3, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics));
                    }
                }
            }
        }

        IEnumerable<IFieldDefinition> ITypeDefinition.GetFields(EmitContext context)
        {
            bool isStruct = AdaptedNamedTypeSymbol.IsStructType();
            foreach (FieldSymbol item in AdaptedNamedTypeSymbol.GetFieldsToEmit())
            {
                if (isStruct || item.GetCciAdapter().ShouldInclude(context))
                {
                    yield return item.GetCciAdapter();
                }
            }
            IEnumerable<IFieldDefinition> synthesizedFields = ((PEModuleBuilder)context.Module).GetSynthesizedFields(AdaptedNamedTypeSymbol);
            if (synthesizedFields == null)
            {
                yield break;
            }
            foreach (IFieldDefinition item2 in synthesizedFields)
            {
                if (isStruct || item2.ShouldInclude(context))
                {
                    yield return item2;
                }
            }
        }

        IEnumerable<TypeReferenceWithAttributes> ITypeDefinition.Interfaces(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.GetInterfacesToEmit().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                INamedTypeReference typeRef = moduleBeingBuilt.Translate(current, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: true);
                TypeWithAnnotations type = TypeWithAnnotations.Create(current);
                yield return type.GetTypeRefWithAttributes(moduleBeingBuilt, AdaptedNamedTypeSymbol, typeRef);
            }
        }

        IEnumerable<IMethodDefinition> ITypeDefinition.GetMethods(EmitContext context)
        {
            bool alwaysIncludeConstructors = context.IncludePrivateMembers || AdaptedNamedTypeSymbol.DeclaringCompilation.IsAttributeType(AdaptedNamedTypeSymbol);
            foreach (MethodSymbol item in AdaptedNamedTypeSymbol.GetMethodsToEmit())
            {
                if ((alwaysIncludeConstructors && item.MethodKind == MethodKind.Constructor) || item.GetCciAdapter().ShouldInclude(context))
                {
                    yield return item.GetCciAdapter();
                }
            }
            IEnumerable<IMethodDefinition> synthesizedMethods = ((PEModuleBuilder)context.Module).GetSynthesizedMethods(AdaptedNamedTypeSymbol);
            if (synthesizedMethods == null)
            {
                yield break;
            }
            foreach (IMethodDefinition item2 in synthesizedMethods)
            {
                if ((alwaysIncludeConstructors && item2.IsConstructor) || item2.ShouldInclude(context))
                {
                    yield return item2;
                }
            }
        }

        IEnumerable<INestedTypeDefinition> ITypeDefinition.GetNestedTypes(EmitContext context)
        {
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.GetTypeMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                yield return current.GetCciAdapter();
            }
            IEnumerable<INestedTypeDefinition> synthesizedTypes = ((PEModuleBuilder)context.Module).GetSynthesizedTypes(AdaptedNamedTypeSymbol);
            if (synthesizedTypes == null)
            {
                yield break;
            }
            foreach (INestedTypeDefinition item in synthesizedTypes)
            {
                yield return item;
            }
        }

        IEnumerable<IPropertyDefinition> ITypeDefinition.GetProperties(EmitContext context)
        {
            foreach (PropertySymbol item in AdaptedNamedTypeSymbol.GetPropertiesToEmit())
            {
                IPropertyDefinition cciAdapter = item.GetCciAdapter();
                if (cciAdapter.ShouldInclude(context) || !cciAdapter.GetAccessors(context).IsEmpty())
                {
                    yield return cciAdapter;
                }
            }
            IEnumerable<IPropertyDefinition> synthesizedProperties = ((PEModuleBuilder)context.Module).GetSynthesizedProperties(AdaptedNamedTypeSymbol);
            if (synthesizedProperties == null)
            {
                yield break;
            }
            foreach (IPropertyDefinition item2 in synthesizedProperties)
            {
                if (item2.ShouldInclude(context) || !item2.GetAccessors(context).IsEmpty())
                {
                    yield return item2;
                }
            }
        }

        IUnitReference INamespaceTypeReference.GetUnit(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedNamedTypeSymbol.ContainingModule, context.Diagnostics);
        }

        ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedNamedTypeSymbol.ContainingType, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, AdaptedNamedTypeSymbol.IsDefinition);
        }

        ImmutableArray<ITypeReference> IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            ArrayBuilder<ITypeReference> instance = ArrayBuilder<ITypeReference>.GetInstance();
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = AdaptedNamedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
            for (int i = 0; i < typeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length; i++)
            {
                ITypeReference typeReference = pEModuleBuilder.Translate(typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i].Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
                ImmutableArray<CustomModifier> customModifiers = typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i].CustomModifiers;
                if (!customModifiers.IsDefaultOrEmpty)
                {
                    typeReference = new ModifiedTypeReference(typeReference, ImmutableArray<ICustomModifier>.CastUp(customModifiers));
                }
                instance.Add(typeReference);
            }
            return instance.ToImmutableAndFree();
        }

        INamedTypeReference IGenericTypeInstanceReference.GetGenericType(EmitContext context)
        {
            return GenericTypeImpl(context);
        }

        private INamedTypeReference GenericTypeImpl(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedNamedTypeSymbol.OriginalDefinition, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, needDeclaration: true);
        }

        INestedTypeReference ISpecializedNestedTypeReference.GetUnspecializedVersion(EmitContext context)
        {
            return GenericTypeImpl(context).AsNestedTypeReference;
        }

        internal new NamedTypeSymbol GetCciAdapter()
        {
            return this;
        }

        internal virtual IEnumerable<EventSymbol> GetEventsToEmit()
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Event)
                {
                    yield return (EventSymbol)current;
                }
            }
        }

        internal abstract IEnumerable<FieldSymbol> GetFieldsToEmit();

        internal abstract ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit();

        protected ImmutableArray<NamedTypeSymbol> CalculateInterfacesToEmit()
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            HashSet<NamedTypeSymbol> seen = null;
            InterfacesVisit(this, instance, ref seen);
            return instance.ToImmutableAndFree();
        }

        private static void InterfacesVisit(NamedTypeSymbol namedType, ArrayBuilder<NamedTypeSymbol> builder, ref HashSet<NamedTypeSymbol> seen)
        {
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = namedType.InterfacesNoUseSiteDiagnostics().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (seen == null)
                {
                    seen = new HashSet<NamedTypeSymbol>(SymbolEqualityComparer.CLRSignature);
                }
                if (seen.Add(current))
                {
                    builder.Add(current);
                    InterfacesVisit(current, builder, ref seen);
                }
            }
        }

        internal virtual IEnumerable<MethodSymbol> GetMethodsToEmit()
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Method)
                {
                    MethodSymbol methodSymbol = (MethodSymbol)current;
                    if (methodSymbol.ShouldEmit())
                    {
                        yield return methodSymbol;
                    }
                }
            }
        }

        internal virtual IEnumerable<PropertySymbol> GetPropertiesToEmit()
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind == SymbolKind.Property)
                {
                    yield return (PropertySymbol)current;
                }
            }
        }

        internal NamedTypeSymbol(TupleExtraData tupleData = null)
        {
            _lazyTupleData = tupleData;
        }

        internal ImmutableArray<TypeWithAnnotations> TypeArgumentsWithDefinitionUseSiteDiagnostics(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = typeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Type.OriginalDefinition.AddUseSiteInfo(ref useSiteInfo);
            }
            return typeArgumentsWithAnnotationsNoUseSiteDiagnostics;
        }

        internal TypeWithAnnotations TypeArgumentWithDefinitionUseSiteDiagnostics(int index, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            TypeWithAnnotations result = TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[index];
            result.Type.OriginalDefinition.AddUseSiteInfo(ref useSiteInfo);
            return result;
        }

        internal void SetKnownToHaveNoDeclaredBaseCycles()
        {
            _hasNoBaseCycles = true;
        }

        internal virtual bool GetGuidString(out string guidString)
        {
            return GetGuidStringDefaultImplementation(out guidString);
        }

        internal ImmutableArray<MethodSymbol> GetOperators(string name)
        {
            ImmutableArray<Symbol> simpleNonTypeMembers = GetSimpleNonTypeMembers(name);
            if (simpleNonTypeMembers.IsEmpty)
            {
                return ImmutableArray<MethodSymbol>.Empty;
            }
            ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
            foreach (MethodSymbol item in simpleNonTypeMembers.OfType<MethodSymbol>())
            {
                if (item.MethodKind == MethodKind.UserDefinedOperator || item.MethodKind == MethodKind.Conversion)
                {
                    instance.Add(item);
                }
            }
            return instance.ToImmutableAndFree();
        }

        private ImmutableArray<MethodSymbol> GetConstructors(bool includeInstance, bool includeStatic)
        {
            ImmutableArray<Symbol> immutableArray = (includeInstance ? GetMembers(".ctor") : ImmutableArray<Symbol>.Empty);
            ImmutableArray<Symbol> immutableArray2 = (includeStatic ? GetMembers(".cctor") : ImmutableArray<Symbol>.Empty);
            if (immutableArray.IsEmpty && immutableArray2.IsEmpty)
            {
                return ImmutableArray<MethodSymbol>.Empty;
            }
            ArrayBuilder<MethodSymbol> instance = ArrayBuilder<MethodSymbol>.GetInstance();
            ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is MethodSymbol item)
                {
                    instance.Add(item);
                }
            }
            enumerator = immutableArray2.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is MethodSymbol item2)
                {
                    instance.Add(item2);
                }
            }
            return instance.ToImmutableAndFree();
        }

        internal void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string nameOpt, int arity, LookupOptions options)
        {
            if (MightContainExtensionMethods)
            {
                DoGetExtensionMethods(methods, nameOpt, arity, options);
            }
        }

        internal void DoGetExtensionMethods(ArrayBuilder<MethodSymbol> methods, string nameOpt, int arity, LookupOptions options)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = ((nameOpt == null) ? GetMembersUnordered() : GetSimpleNonTypeMembers(nameOpt)).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Method)
                {
                    continue;
                }
                MethodSymbol methodSymbol = (MethodSymbol)current;
                if (methodSymbol.IsExtensionMethod && ((options & LookupOptions.AllMethodsOnArityZero) != 0 || arity == methodSymbol.Arity))
                {
                    ParameterSymbol parameterSymbol = methodSymbol.Parameters.First();
                    if ((parameterSymbol.RefKind != RefKind.Ref || parameterSymbol.Type.IsValueType) && (parameterSymbol.RefKind != RefKind.In || parameterSymbol.Type.TypeKind == TypeKind.Struct))
                    {
                        methods.Add(methodSymbol);
                    }
                }
            }
        }

        internal override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return BaseTypeAnalysis.GetManagedKind(this, ref useSiteInfo);
        }

        internal abstract AttributeUsageInfo GetAttributeUsageInfo();

        internal SynthesizedInstanceConstructor GetScriptConstructor()
        {
            return (SynthesizedInstanceConstructor)InstanceConstructors.Single();
        }

        internal SynthesizedInteractiveInitializerMethod GetScriptInitializer()
        {
            return (SynthesizedInteractiveInitializerMethod)GetMembers("<Initialize>").Single();
        }

        internal SynthesizedEntryPointSymbol GetScriptEntryPoint()
        {
            string name = ((TypeKind == TypeKind.Submission) ? "<Factory>" : "<Main>");
            return (SynthesizedEntryPointSymbol)GetMembers(name).Single();
        }

        public abstract override ImmutableArray<Symbol> GetMembers();

        public abstract override ImmutableArray<Symbol> GetMembers(string name);

        internal abstract bool HasPossibleWellKnownCloneMethod();

        internal virtual ImmutableArray<Symbol> GetSimpleNonTypeMembers(string name)
        {
            return GetMembers(name);
        }

        public abstract override ImmutableArray<NamedTypeSymbol> GetTypeMembers();

        public abstract override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name);

        public abstract override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity);

        internal virtual IEnumerable<Symbol> GetInstanceFieldsAndEvents()
        {
            return GetMembersUnordered().Where(IsInstanceFieldOrEvent);
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitNamedType(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitNamedType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitNamedType(this);
        }

        internal abstract ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers();

        internal abstract ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name);

        internal abstract NamedTypeSymbol GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved);

        internal abstract ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved);

        public override int GetHashCode()
        {
            if (SpecialType == SpecialType.System_Object)
            {
                return 1;
            }
            return RuntimeHelpers.GetHashCode(OriginalDefinition);
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind comparison)
        {
            if ((object)t2 == this)
            {
                return true;
            }
            if ((object)t2 == null)
            {
                return false;
            }
            if ((comparison & TypeCompareKind.IgnoreDynamic) != 0 && t2.TypeKind == TypeKind.Dynamic && SpecialType == SpecialType.System_Object)
            {
                return true;
            }
            if (!(t2 is NamedTypeSymbol namedTypeSymbol))
            {
                return false;
            }
            NamedTypeSymbol originalDefinition = OriginalDefinition;
            NamedTypeSymbol originalDefinition2 = namedTypeSymbol.OriginalDefinition;
            bool flag = (object)this == originalDefinition;
            bool flag2 = (object)namedTypeSymbol == originalDefinition2;
            if (flag && flag2)
            {
                return false;
            }
            if ((flag || flag2) && (comparison & (TypeCompareKind.AllNullableIgnoreOptions | TypeCompareKind.AllIgnoreOptionsForVB)) == 0)
            {
                return false;
            }
            if (!TypeSymbol.Equals(originalDefinition, originalDefinition2, comparison))
            {
                return false;
            }
            return EqualsComplicatedCases(namedTypeSymbol, comparison);
        }

        private bool EqualsComplicatedCases(NamedTypeSymbol other, TypeCompareKind comparison)
        {
            if ((object)ContainingType != null && !ContainingType.Equals(other.ContainingType, comparison))
            {
                return false;
            }
            bool flag = (object)ConstructedFrom == this;
            bool flag2 = (object)other.ConstructedFrom == other;
            if (flag && flag2)
            {
                return true;
            }
            if (IsUnboundGenericType != other.IsUnboundGenericType)
            {
                return false;
            }
            if ((flag || flag2) && (comparison & (TypeCompareKind.AllNullableIgnoreOptions | TypeCompareKind.AllIgnoreOptionsForVB)) == 0)
            {
                return false;
            }
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
            ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics2 = other.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
            int length = typeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length;
            for (int i = 0; i < length; i++)
            {
                TypeWithAnnotations typeWithAnnotations = typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i];
                TypeWithAnnotations other2 = typeArgumentsWithAnnotationsNoUseSiteDiagnostics2[i];
                if (!typeWithAnnotations.Equals(other2, comparison))
                {
                    return false;
                }
            }
            if (IsTupleType && !tupleNamesEquals(other, comparison))
            {
                return false;
            }
            return true;
            bool tupleNamesEquals(NamedTypeSymbol other, TypeCompareKind comparison)
            {
                if ((comparison & TypeCompareKind.IgnoreTupleNames) == 0)
                {
                    ImmutableArray<string> tupleElementNames = TupleElementNames;
                    ImmutableArray<string> tupleElementNames2 = other.TupleElementNames;
                    if (!tupleElementNames.IsDefault)
                    {
                        if (!tupleElementNames2.IsDefault)
                        {
                            return tupleElementNames.SequenceEqual(tupleElementNames2);
                        }
                        return false;
                    }
                    return tupleElementNames2.IsDefault;
                }
                return true;
            }
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            ContainingType?.AddNullableTransforms(transforms);
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.AddNullableTransforms(transforms);
            }
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            if (!IsGenericType)
            {
                result = this;
                return true;
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            GetAllTypeArgumentsNoUseSiteDiagnostics(instance);
            bool flag = false;
            for (int i = 0; i < instance.Count; i++)
            {
                TypeWithAnnotations typeWithAnnotations = instance[i];
                if (!typeWithAnnotations.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position, out var result2))
                {
                    instance.Free();
                    result = this;
                    return false;
                }
                if (!typeWithAnnotations.IsSameAs(result2))
                {
                    instance[i] = result2;
                    flag = true;
                }
            }
            result = (flag ? WithTypeArguments(instance.ToImmutable()) : this);
            instance.Free();
            return true;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            if (!IsGenericType)
            {
                return this;
            }
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            GetAllTypeArgumentsNoUseSiteDiagnostics(instance);
            bool flag = false;
            for (int i = 0; i < instance.Count; i++)
            {
                TypeWithAnnotations arg = instance[i];
                TypeWithAnnotations typeWithAnnotations = transform(arg);
                if (!arg.IsSameAs(typeWithAnnotations))
                {
                    instance[i] = typeWithAnnotations;
                    flag = true;
                }
            }
            NamedTypeSymbol result = (flag ? WithTypeArguments(instance.ToImmutable()) : this);
            instance.Free();
            return result;
        }

        internal NamedTypeSymbol WithTypeArguments(ImmutableArray<TypeWithAnnotations> allTypeArguments)
        {
            NamedTypeSymbol originalDefinition = OriginalDefinition;
            return new TypeMap(originalDefinition.GetAllTypeParameters(), allTypeArguments).SubstituteNamedType(originalDefinition).WithTupleDataFrom(this);
        }

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            if (!IsGenericType)
            {
                if (!other.IsDynamic())
                {
                    return this;
                }
                return other;
            }
            ArrayBuilder<TypeParameterSymbol> instance = ArrayBuilder<TypeParameterSymbol>.GetInstance();
            ArrayBuilder<TypeWithAnnotations> instance2 = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            NamedTypeSymbol namedTypeSymbol = ((!MergeEquivalentTypeArguments(this, (NamedTypeSymbol)other, variance, instance, instance2)) ? this : new TypeMap(instance.ToImmutable(), instance2.ToImmutable()).SubstituteNamedType(OriginalDefinition));
            instance2.Free();
            instance.Free();
            if (!IsTupleType)
            {
                return namedTypeSymbol;
            }
            return MergeTupleNames((NamedTypeSymbol)other, namedTypeSymbol);
        }

        private static bool MergeEquivalentTypeArguments(NamedTypeSymbol typeA, NamedTypeSymbol typeB, VarianceKind variance, ArrayBuilder<TypeParameterSymbol> allTypeParameters, ArrayBuilder<TypeWithAnnotations> allTypeArguments)
        {
            bool isTupleType = typeA.IsTupleType;
            NamedTypeSymbol namedTypeSymbol = typeA.OriginalDefinition;
            bool result = false;
            while (true)
            {
                ImmutableArray<TypeParameterSymbol> typeParameters = namedTypeSymbol.TypeParameters;
                if (typeParameters.Length > 0)
                {
                    ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics = typeA.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                    ImmutableArray<TypeWithAnnotations> typeArgumentsWithAnnotationsNoUseSiteDiagnostics2 = typeB.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                    allTypeParameters.AddRange(typeParameters);
                    for (int i = 0; i < typeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length; i++)
                    {
                        TypeWithAnnotations typeWithAnnotations = typeArgumentsWithAnnotationsNoUseSiteDiagnostics[i];
                        TypeWithAnnotations other = typeArgumentsWithAnnotationsNoUseSiteDiagnostics2[i];
                        VarianceKind typeArgumentVariance = GetTypeArgumentVariance(variance, isTupleType ? VarianceKind.Out : typeParameters[i].Variance);
                        TypeWithAnnotations typeWithAnnotations2 = typeWithAnnotations.MergeEquivalentTypes(other, typeArgumentVariance);
                        allTypeArguments.Add(typeWithAnnotations2);
                        if (!typeWithAnnotations.IsSameAs(typeWithAnnotations2))
                        {
                            result = true;
                        }
                    }
                }
                namedTypeSymbol = namedTypeSymbol.ContainingType;
                if ((object)namedTypeSymbol == null)
                {
                    break;
                }
                typeA = typeA.ContainingType;
                typeB = typeB.ContainingType;
                variance = VarianceKind.None;
            }
            return result;
        }

        private static VarianceKind GetTypeArgumentVariance(VarianceKind typeVariance, VarianceKind typeParameterVariance)
        {
            return typeVariance switch
            {
                VarianceKind.In => typeParameterVariance switch
                {
                    VarianceKind.In => VarianceKind.Out,
                    VarianceKind.Out => VarianceKind.In,
                    _ => VarianceKind.None,
                },
                VarianceKind.Out => typeParameterVariance,
                _ => VarianceKind.None,
            };
        }

        public NamedTypeSymbol Construct(params TypeSymbol[] typeArguments)
        {
            return ConstructWithoutModifiers(typeArguments.AsImmutableOrNull(), unbound: false);
        }

        public NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
        {
            return ConstructWithoutModifiers(typeArguments, unbound: false);
        }

        public NamedTypeSymbol Construct(IEnumerable<TypeSymbol> typeArguments)
        {
            return ConstructWithoutModifiers(typeArguments.AsImmutableOrNull(), unbound: false);
        }

        public NamedTypeSymbol ConstructUnboundGenericType()
        {
            return OriginalDefinition.AsUnboundGenericType();
        }

        internal NamedTypeSymbol GetUnboundGenericTypeOrSelf()
        {
            if (!IsGenericType)
            {
                return this;
            }
            return ConstructUnboundGenericType();
        }

        private NamedTypeSymbol ConstructWithoutModifiers(ImmutableArray<TypeSymbol> typeArguments, bool unbound)
        {
            ImmutableArray<TypeWithAnnotations> typeArguments2 = ((!typeArguments.IsDefault) ? typeArguments.SelectAsArray((TypeSymbol t) => TypeWithAnnotations.Create(t)) : default(ImmutableArray<TypeWithAnnotations>));
            return Construct(typeArguments2, unbound);
        }

        internal NamedTypeSymbol Construct(ImmutableArray<TypeWithAnnotations> typeArguments)
        {
            return Construct(typeArguments, unbound: false);
        }

        internal NamedTypeSymbol Construct(ImmutableArray<TypeWithAnnotations> typeArguments, bool unbound)
        {
            if ((object)this != ConstructedFrom)
            {
                throw new InvalidOperationException(CSharpResources.CannotCreateConstructedFromConstructed);
            }
            if (Arity == 0)
            {
                throw new InvalidOperationException(CSharpResources.CannotCreateConstructedFromNongeneric);
            }
            if (typeArguments.IsDefault)
            {
                throw new ArgumentNullException("typeArguments");
            }
            if (typeArguments.Any(TypeWithAnnotationsIsNullFunction))
            {
                throw new ArgumentException(CSharpResources.TypeArgumentCannotBeNull, "typeArguments");
            }
            if (typeArguments.Length != Arity)
            {
                throw new ArgumentException(CSharpResources.WrongNumberOfTypeArguments, "typeArguments");
            }
            if (ConstructedNamedTypeSymbol.TypeParametersMatchTypeArguments(TypeParameters, typeArguments))
            {
                return this;
            }
            return ConstructCore(typeArguments, unbound);
        }

        protected virtual NamedTypeSymbol ConstructCore(ImmutableArray<TypeWithAnnotations> typeArguments, bool unbound)
        {
            return new ConstructedNamedTypeSymbol(this, typeArguments, unbound);
        }

        internal void GetAllTypeArguments(ArrayBuilder<TypeSymbol> builder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ContainingType?.GetAllTypeArguments(builder, ref useSiteInfo);
            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo).GetEnumerator();
            while (enumerator.MoveNext())
            {
                builder.Add(enumerator.Current.Type);
            }
        }

        internal ImmutableArray<TypeWithAnnotations> GetAllTypeArguments(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ArrayBuilder<TypeWithAnnotations> instance = ArrayBuilder<TypeWithAnnotations>.GetInstance();
            GetAllTypeArguments(instance, ref useSiteInfo);
            return instance.ToImmutableAndFree();
        }

        internal void GetAllTypeArguments(ArrayBuilder<TypeWithAnnotations> builder, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ContainingType?.GetAllTypeArguments(builder, ref useSiteInfo);
            builder.AddRange(TypeArgumentsWithDefinitionUseSiteDiagnostics(ref useSiteInfo));
        }

        internal void GetAllTypeArgumentsNoUseSiteDiagnostics(ArrayBuilder<TypeWithAnnotations> builder)
        {
            ContainingType?.GetAllTypeArgumentsNoUseSiteDiagnostics(builder);
            builder.AddRange(TypeArgumentsWithAnnotationsNoUseSiteDiagnostics);
        }

        internal int AllTypeArgumentCount()
        {
            int num = TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.Length;
            NamedTypeSymbol containingType = ContainingType;
            if ((object)containingType != null)
            {
                num += containingType.AllTypeArgumentCount();
            }
            return num;
        }

        internal ImmutableArray<TypeWithAnnotations> GetTypeParametersAsTypeArguments()
        {
            return TypeMap.TypeParametersAsTypeSymbolsWithAnnotations(TypeParameters);
        }

        internal virtual NamedTypeSymbol AsMember(NamedTypeSymbol newOwner)
        {
            if (!newOwner.IsDefinition)
            {
                return new SubstitutedNestedTypeSymbol((SubstitutedNamedTypeSymbol)newOwner, this);
            }
            return this;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency);
            if (base.IsDefinition)
            {
                return result;
            }
            if (!DeriveUseSiteInfoFromType(ref result, OriginalDefinition))
            {
                DeriveUseSiteDiagnosticFromTypeArguments(ref result);
            }
            return result;
        }

        private bool DeriveUseSiteDiagnosticFromTypeArguments(ref UseSiteInfo<AssemblySymbol> result)
        {
            NamedTypeSymbol namedTypeSymbol = this;
            do
            {
                ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeWithAnnotations current = enumerator.Current;
                    if (DeriveUseSiteInfoFromType(ref result, current, AllowedRequiredModifierType.None))
                    {
                        return true;
                    }
                }
                namedTypeSymbol = namedTypeSymbol.ContainingType;
            }
            while ((object)namedTypeSymbol != null && !namedTypeSymbol.IsDefinition);
            return false;
        }

        internal DiagnosticInfo CalculateUseSiteDiagnostic()
        {
            DiagnosticInfo result = null;
            if (MergeUseSiteDiagnostics(ref result, DeriveUseSiteDiagnosticFromBase()))
            {
                return result;
            }
            if (ContainingModule.HasUnifiedReferences)
            {
                HashSet<TypeSymbol> checkedTypes = null;
                GetUnificationUseSiteDiagnosticRecursive(ref result, this, ref checkedTypes);
                return result;
            }
            return result;
        }

        private DiagnosticInfo DeriveUseSiteDiagnosticFromBase()
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            while ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                if (baseTypeNoUseSiteDiagnostics.IsErrorType() && baseTypeNoUseSiteDiagnostics is NoPiaIllegalGenericInstantiationSymbol)
                {
                    return baseTypeNoUseSiteDiagnostics.GetUseSiteInfo().DiagnosticInfo;
                }
                baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
            }
            return null;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            if (!this.MarkCheckedIfNecessary(ref checkedTypes))
            {
                return false;
            }
            if (owner.ContainingModule.GetUnificationUseSiteDiagnostic(ref result, this))
            {
                return true;
            }
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            if ((object)baseTypeNoUseSiteDiagnostics != null && baseTypeNoUseSiteDiagnostics.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes))
            {
                return true;
            }
            if (!Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result, InterfacesNoUseSiteDiagnostics(), owner, ref checkedTypes))
            {
                return Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result, TypeParameters, owner, ref checkedTypes);
            }
            return true;
        }

        internal abstract IEnumerable<SecurityAttribute> GetSecurityInformation();

        internal abstract ImmutableArray<string> GetAppliedConditionalSymbols();

        internal bool IsTupleTypeOfCardinality(out int tupleCardinality)
        {
            if (!IsUnboundGenericType)
            {
                Symbol containingSymbol = ContainingSymbol;
                if ((object)containingSymbol != null && containingSymbol.Kind == SymbolKind.Namespace)
                {
                    NamespaceSymbol containingNamespace = ContainingNamespace.ContainingNamespace;
                    if ((object)containingNamespace != null && containingNamespace.IsGlobalNamespace && Name == "ValueTuple" && ContainingNamespace.Name == "System")
                    {
                        int arity = Arity;
                        if (arity >= 0 && arity < 8)
                        {
                            tupleCardinality = arity;
                            return true;
                        }
                        if (arity == 8 && !base.IsDefinition)
                        {
                            TypeSymbol typeSymbol = this;
                            int num = 0;
                            do
                            {
                                num++;
                                typeSymbol = ((NamedTypeSymbol)typeSymbol).TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type;
                            }
                            while (TypeSymbol.Equals(typeSymbol.OriginalDefinition, OriginalDefinition, TypeCompareKind.ConsiderEverything) && !typeSymbol.IsDefinition);
                            arity = (typeSymbol as NamedTypeSymbol)?.Arity ?? 0;
                            if (arity > 0 && arity < 8 && ((NamedTypeSymbol)typeSymbol).IsTupleTypeOfCardinality(out tupleCardinality))
                            {
                                tupleCardinality += 7 * num;
                                return true;
                            }
                        }
                    }
                }
            }
            tupleCardinality = 0;
            return false;
        }

        internal abstract NamedTypeSymbol AsNativeInteger();

        protected override ISymbol CreateISymbol()
        {
            return new NonErrorNamedTypeSymbol(this, base.DefaultNullableAnnotation);
        }

        protected override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new NonErrorNamedTypeSymbol(this, nullableAnnotation);
        }

        internal static NamedTypeSymbol CreateTuple(Location? locationOpt, ImmutableArray<TypeWithAnnotations> elementTypesWithAnnotations, ImmutableArray<Location?> elementLocations, ImmutableArray<string?> elementNames, CSharpCompilation compilation, bool shouldCheckConstraints, bool includeNullability, ImmutableArray<bool> errorPositions, CSharpSyntaxNode? syntax = null, BindingDiagnosticBag? diagnostics = null)
        {
            int length = elementTypesWithAnnotations.Length;
            if (length <= 1)
            {
                throw ExceptionUtilities.Unreachable;
            }
            NamedTypeSymbol namedTypeSymbol = getTupleUnderlyingType(elementTypesWithAnnotations, syntax, compilation, diagnostics);
            if (length >= 8 && diagnostics != null && !namedTypeSymbol.IsErrorType())
            {
                WellKnownMember tupleTypeMember = GetTupleTypeMember(8, 8);
                GetWellKnownMemberInType(namedTypeSymbol.OriginalDefinition, tupleTypeMember, diagnostics, syntax);
            }
            if (diagnostics != null && diagnostics!.DiagnosticBag != null && ((SourceModuleSymbol)compilation.SourceModule).AnyReferencedAssembliesAreLinked)
            {
                EmbeddedTypesManager.IsValidEmbeddableType(namedTypeSymbol, syntax, diagnostics!.DiagnosticBag);
            }
            ImmutableArray<Location> locations = (((object)locationOpt == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(locationOpt));
            NamedTypeSymbol namedTypeSymbol2 = CreateTuple(namedTypeSymbol, elementNames, errorPositions, elementLocations, locations);
            if (shouldCheckConstraints && diagnostics != null)
            {
                ConstraintsHelper.CheckConstraintsArgs args = new ConstraintsHelper.CheckConstraintsArgs(compilation, compilation.Conversions, includeNullability, syntax!.Location, diagnostics);
                namedTypeSymbol2.CheckConstraints(in args, syntax, elementLocations, includeNullability ? diagnostics : null);
            }
            return namedTypeSymbol2;
            static NamedTypeSymbol getTupleUnderlyingType(ImmutableArray<TypeWithAnnotations> elementTypes, CSharpSyntaxNode? syntax, CSharpCompilation compilation, BindingDiagnosticBag? diagnostics)
            {
                int num = NumberOfValueTuples(elementTypes.Length, out int remainder);
                NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(GetTupleType(remainder));
                if (diagnostics != null && syntax != null)
                {
                    ReportUseSiteAndObsoleteDiagnostics(syntax, diagnostics, wellKnownType);
                }
                NamedTypeSymbol namedTypeSymbol3 = null;
                if (num > 1)
                {
                    namedTypeSymbol3 = compilation.GetWellKnownType(GetTupleType(8));
                    if (diagnostics != null && syntax != null)
                    {
                        ReportUseSiteAndObsoleteDiagnostics(syntax, diagnostics, namedTypeSymbol3);
                    }
                }
                return ConstructTupleUnderlyingType(wellKnownType, namedTypeSymbol3, elementTypes);
            }
        }

        public static NamedTypeSymbol CreateTuple(NamedTypeSymbol tupleCompatibleType, ImmutableArray<string?> elementNames = default(ImmutableArray<string?>), ImmutableArray<bool> errorPositions = default(ImmutableArray<bool>), ImmutableArray<Location?> elementLocations = default(ImmutableArray<Location?>), ImmutableArray<Location> locations = default(ImmutableArray<Location>))
        {
            return tupleCompatibleType.WithElementNames(elementNames, elementLocations, errorPositions, locations);
        }

        internal NamedTypeSymbol WithTupleDataFrom(NamedTypeSymbol original)
        {
            if (!IsTupleType || (original._lazyTupleData == null && _lazyTupleData == null) || TupleData!.EqualsIgnoringTupleUnderlyingType(original.TupleData))
            {
                return this;
            }
            return WithElementNames(original.TupleElementNames, original.TupleElementLocations, original.TupleErrorPositions, original.Locations);
        }

        internal NamedTypeSymbol WithElementTypes(ImmutableArray<TypeWithAnnotations> newElementTypes)
        {
            NamedTypeSymbol originalDefinition;
            NamedTypeSymbol chainedTupleTypeOpt;
            if (Arity < 8)
            {
                originalDefinition = OriginalDefinition;
                chainedTupleTypeOpt = null;
            }
            else
            {
                chainedTupleTypeOpt = OriginalDefinition;
                NamedTypeSymbol namedTypeSymbol = this;
                do
                {
                    namedTypeSymbol = (NamedTypeSymbol)namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type;
                }
                while (namedTypeSymbol.Arity >= 8);
                originalDefinition = namedTypeSymbol.OriginalDefinition;
            }
            return CreateTuple(ConstructTupleUnderlyingType(originalDefinition, chainedTupleTypeOpt, newElementTypes), TupleElementNames, elementLocations: TupleElementLocations, errorPositions: TupleErrorPositions, locations: Locations);
        }

        internal NamedTypeSymbol WithElementNames(ImmutableArray<string?> newElementNames, ImmutableArray<Location?> newElementLocations, ImmutableArray<bool> errorPositions, ImmutableArray<Location> locations)
        {
            return WithTupleData(new TupleExtraData(TupleUnderlyingType, newElementNames, newElementLocations, errorPositions, locations));
        }

        private NamedTypeSymbol WithTupleData(TupleExtraData newData)
        {
            if (newData.EqualsIgnoringTupleUnderlyingType(TupleData))
            {
                return this;
            }
            if (base.IsDefinition)
            {
                if (newData.ElementNames.IsDefault)
                {
                    return this;
                }
                return ConstructCore(GetTypeParametersAsTypeArguments(), unbound: false).WithTupleData(newData);
            }
            return WithTupleDataCore(newData);
        }

        protected abstract NamedTypeSymbol WithTupleDataCore(TupleExtraData newData);

        internal static void GetUnderlyingTypeChain(NamedTypeSymbol underlyingTupleType, ArrayBuilder<NamedTypeSymbol> underlyingTupleTypeChain)
        {
            NamedTypeSymbol namedTypeSymbol = underlyingTupleType;
            while (true)
            {
                underlyingTupleTypeChain.Add(namedTypeSymbol);
                if (namedTypeSymbol.Arity == 8)
                {
                    namedTypeSymbol = (NamedTypeSymbol)namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type;
                    continue;
                }
                break;
            }
        }

        private static int NumberOfValueTuples(int numElements, out int remainder)
        {
            remainder = (numElements - 1) % 7 + 1;
            return (numElements - 1) / 7 + 1;
        }

        private static NamedTypeSymbol ConstructTupleUnderlyingType(NamedTypeSymbol firstTupleType, NamedTypeSymbol? chainedTupleTypeOpt, ImmutableArray<TypeWithAnnotations> elementTypes)
        {
            int num = NumberOfValueTuples(elementTypes.Length, out int remainder);
            NamedTypeSymbol namedTypeSymbol = firstTupleType.Construct(ImmutableArray.Create(elementTypes, (num - 1) * 7, remainder));
            for (int num2 = num - 1; num2 > 0; num2--)
            {
                ImmutableArray<TypeWithAnnotations> typeArguments = ImmutableArray.Create(elementTypes, (num2 - 1) * 7, 7).Add(TypeWithAnnotations.Create(namedTypeSymbol));
                namedTypeSymbol = chainedTupleTypeOpt!.Construct(typeArguments);
            }
            return namedTypeSymbol;
        }

        private static void ReportUseSiteAndObsoleteDiagnostics(CSharpSyntaxNode? syntax, BindingDiagnosticBag diagnostics, NamedTypeSymbol firstTupleType)
        {
            Binder.ReportUseSite(firstTupleType, diagnostics, syntax);
            Binder.ReportDiagnosticsIfObsoleteInternal(diagnostics, firstTupleType, syntax, firstTupleType.ContainingType, BinderFlags.None);
        }

        internal static void VerifyTupleTypePresent(int cardinality, CSharpSyntaxNode? syntax, CSharpCompilation compilation, BindingDiagnosticBag diagnostics)
        {
            int num = NumberOfValueTuples(cardinality, out int remainder);
            NamedTypeSymbol wellKnownType = compilation.GetWellKnownType(GetTupleType(remainder));
            ReportUseSiteAndObsoleteDiagnostics(syntax, diagnostics, wellKnownType);
            if (num > 1)
            {
                NamedTypeSymbol wellKnownType2 = compilation.GetWellKnownType(GetTupleType(8));
                ReportUseSiteAndObsoleteDiagnostics(syntax, diagnostics, wellKnownType2);
            }
        }

        internal static void ReportTupleNamesMismatchesIfAny(TypeSymbol destination, BoundTupleLiteral literal, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<string> argumentNamesOpt = literal.ArgumentNamesOpt;
            if (argumentNamesOpt.IsDefault)
            {
                return;
            }
            ImmutableArray<bool> inferredNamesOpt = literal.InferredNamesOpt;
            bool isDefault = inferredNamesOpt.IsDefault;
            ImmutableArray<string> tupleElementNames = destination.TupleElementNames;
            int length = argumentNamesOpt.Length;
            bool isDefault2 = tupleElementNames.IsDefault;
            for (int i = 0; i < length; i++)
            {
                string text = argumentNamesOpt[i];
                bool flag = !isDefault && inferredNamesOpt[i];
                if (text != null && !flag && (isDefault2 || string.CompareOrdinal(tupleElementNames[i], text) != 0))
                {
                    diagnostics.Add(ErrorCode.WRN_TupleLiteralNameMismatch, literal.Arguments[i].Syntax.Parent!.Location, text, destination);
                }
            }
        }

        private static WellKnownType GetTupleType(int arity)
        {
            if (arity > 8)
            {
                throw ExceptionUtilities.Unreachable;
            }
            return tupleTypes[arity - 1];
        }

        internal static WellKnownMember GetTupleCtor(int arity)
        {
            if (arity > 8)
            {
                throw ExceptionUtilities.Unreachable;
            }
            return tupleCtors[arity - 1];
        }

        internal static WellKnownMember GetTupleTypeMember(int arity, int position)
        {
            return tupleMembers[arity - 1][position - 1];
        }

        internal static string TupleMemberName(int position)
        {
            return "Item" + position;
        }

        internal static int IsTupleElementNameReserved(string name)
        {
            if (isElementNameForbidden(name))
            {
                return 0;
            }
            return matchesCanonicalElementName(name);
            static bool isElementNameForbidden(string name)
            {
                switch (name)
                {
                    case "CompareTo":
                    case "Deconstruct":
                    case "Equals":
                    case "GetHashCode":
                    case "Rest":
                    case "ToString":
                        return true;
                    default:
                        return false;
                }
            }
            static int matchesCanonicalElementName(string name)
            {
                if (name.StartsWith("Item", StringComparison.Ordinal) && int.TryParse(name.Substring(4), out var result) && result > 0 && string.Equals(name, TupleMemberName(result), StringComparison.Ordinal))
                {
                    return result;
                }
                return -1;
            }
        }

        internal static Symbol? GetWellKnownMemberInType(NamedTypeSymbol type, WellKnownMember relativeMember, BindingDiagnosticBag diagnostics, SyntaxNode? syntax)
        {
            Symbol symbol = GetWellKnownMemberInType(type, relativeMember);
            if ((object)symbol == null)
            {
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(relativeMember);
                Binder.Error(diagnostics, ErrorCode.ERR_PredefinedTypeMemberNotFoundInAssembly, syntax, descriptor.Name, type, type.ContainingAssembly);
            }
            else
            {
                UseSiteInfo<AssemblySymbol> info = symbol.GetUseSiteInfo();
                DiagnosticInfo? diagnosticInfo = info.DiagnosticInfo;
                if (diagnosticInfo == null || diagnosticInfo!.Severity != DiagnosticSeverity.Error)
                {
                    info = info.AdjustDiagnosticInfo(null);
                }
                diagnostics.Add(info, syntax?.GetLocation() ?? Location.None);
            }
            return symbol;
            static Symbol? GetWellKnownMemberInType(NamedTypeSymbol type, WellKnownMember relativeMember)
            {
                MemberDescriptor descriptor2 = WellKnownMembers.GetDescriptor(relativeMember);
                return CSharpCompilation.GetRuntimeMember(type.GetMembers(descriptor2.Name), in descriptor2, CSharpCompilation.SpecialMembersSignatureComparer.Instance, null);
            }
        }

        public virtual void InitializeTupleFieldDefinitionsToIndexMap()
        {
            GetMembers();
        }

        public TMember? GetTupleMemberSymbolForUnderlyingMember<TMember>(TMember? underlyingMemberOpt) where TMember : Symbol
        {
            if (!IsTupleType)
            {
                return null;
            }
            return TupleData!.GetTupleMemberSymbolForUnderlyingMember(underlyingMemberOpt);
        }

        protected ArrayBuilder<Symbol> AddOrWrapTupleMembers(ImmutableArray<Symbol> currentMembers)
        {
            ImmutableArray<TypeWithAnnotations> tupleElementTypesWithAnnotations = TupleElementTypesWithAnnotations;
            ArrayBuilder<bool> instance = ArrayBuilder<bool>.GetInstance(tupleElementTypesWithAnnotations.Length, fillWithValue: false);
            ArrayBuilder<Symbol> instance2 = ArrayBuilder<Symbol>.GetInstance(currentMembers.Length);
            ArrayBuilder<Symbol> instance3 = ArrayBuilder<Symbol>.GetInstance();
            SmallDictionary<FieldSymbol, int> smallDictionary = (base.IsDefinition ? new SmallDictionary<FieldSymbol, int>(ReferenceEqualityComparer.Instance) : null);
            NamedTypeSymbol namedTypeSymbol = this;
            int num = 0;
            ArrayBuilder<FieldSymbol> instance4 = ArrayBuilder<FieldSymbol>.GetInstance(namedTypeSymbol.Arity);
            collectTargetTupleFields(namedTypeSymbol.Arity, getOriginalFields(currentMembers), instance4);
            ImmutableArray<string> tupleElementNames = TupleElementNames;
            ImmutableArray<Location> elementLocations2 = TupleData!.ElementLocations;
            while (true)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = currentMembers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    switch (current.Kind)
                    {
                        case SymbolKind.Field:
                            {
                                FieldSymbol fieldSymbol = (FieldSymbol)current;
                                if (fieldSymbol is TupleVirtualElementFieldSymbol)
                                {
                                    break;
                                }
                                FieldSymbol fieldSymbol2 = ((fieldSymbol is TupleElementFieldSymbol tupleElementFieldSymbol) ? tupleElementFieldSymbol.UnderlyingField.OriginalDefinition : fieldSymbol.OriginalDefinition);
                                int num2 = instance4.IndexOf(fieldSymbol2, ReferenceEqualityComparer.Instance);
                                if (fieldSymbol2 is TupleErrorFieldSymbol)
                                {
                                    break;
                                }
                                if (num2 >= 0)
                                {
                                    if (num != 0)
                                    {
                                        num2 += 7 * num;
                                    }
                                    string text = (tupleElementNames.IsDefault ? null : tupleElementNames[num2]);
                                    ImmutableArray<Location> locations = getElementLocations(in elementLocations2, num2);
                                    string text2 = TupleMemberName(num2 + 1);
                                    bool flag = text != text2;
                                    FieldSymbol underlyingField = fieldSymbol2.AsMember(namedTypeSymbol);
                                    FieldSymbol fieldSymbol3;
                                    if (num != 0)
                                    {
                                        fieldSymbol3 = new TupleVirtualElementFieldSymbol(this, underlyingField, text2, num2, locations, cannotUse: false, flag, null);
                                    }
                                    else if (base.IsDefinition)
                                    {
                                        fieldSymbol3 = fieldSymbol;
                                        smallDictionary.Add(fieldSymbol, num2);
                                    }
                                    else
                                    {
                                        fieldSymbol3 = new TupleElementFieldSymbol(this, underlyingField, num2, locations, flag);
                                    }
                                    instance2.Add(fieldSymbol3);
                                    if (flag && !string.IsNullOrEmpty(text))
                                    {
                                        ImmutableArray<bool> tupleErrorPositions = TupleErrorPositions;
                                        bool cannotUse = !tupleErrorPositions.IsDefault && tupleErrorPositions[num2];
                                        instance2.Add(new TupleVirtualElementFieldSymbol(this, underlyingField, text, num2, locations, cannotUse, isImplicitlyDeclared: false, fieldSymbol3));
                                    }
                                    instance[num2] = true;
                                }
                                else if (num == 0)
                                {
                                    instance2.Add(fieldSymbol);
                                }
                                break;
                            }
                        case SymbolKind.Event:
                        case SymbolKind.Method:
                        case SymbolKind.Property:
                            if (num == 0)
                            {
                                instance3.Add(current);
                            }
                            break;
                        default:
                            if (num == 0)
                            {
                                throw ExceptionUtilities.UnexpectedValue(current.Kind);
                            }
                            break;
                        case SymbolKind.NamedType:
                            break;
                    }
                }
                if (namedTypeSymbol.Arity != 8)
                {
                    break;
                }
                namedTypeSymbol = (NamedTypeSymbol)namedTypeSymbol.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type;
                num++;
                if (namedTypeSymbol.Arity != 8)
                {
                    currentMembers = namedTypeSymbol.GetMembers();
                    instance4.Clear();
                    collectTargetTupleFields(namedTypeSymbol.Arity, getOriginalFields(currentMembers), instance4);
                }
            }
            instance4.Free();
            for (int i = 0; i < instance.Count; i++)
            {
                if (!instance[i])
                {
                    int num3 = NumberOfValueTuples(i + 1, out int remainder);
                    NamedTypeSymbol originalDefinition = getNestedTupleUnderlyingType(this, num3 - 1).OriginalDefinition;
                    CSDiagnosticInfo useSiteDiagnosticInfo = (originalDefinition.IsErrorType() ? null : new CSDiagnosticInfo(ErrorCode.ERR_PredefinedTypeMemberNotFoundInAssembly, TupleMemberName(remainder), originalDefinition, originalDefinition.ContainingAssembly));
                    string text3 = (tupleElementNames.IsDefault ? null : tupleElementNames[i]);
                    Location location = (elementLocations2.IsDefault ? null : elementLocations2[i]);
                    string text4 = TupleMemberName(i + 1);
                    bool flag2 = text3 != text4;
                    TupleErrorFieldSymbol tupleErrorFieldSymbol = new TupleErrorFieldSymbol(this, text4, i, flag2 ? null : location, tupleElementTypesWithAnnotations[i], useSiteDiagnosticInfo, flag2, null);
                    instance2.Add(tupleErrorFieldSymbol);
                    if (flag2 && !string.IsNullOrEmpty(text3))
                    {
                        instance2.Add(new TupleErrorFieldSymbol(this, text3, i, location, tupleElementTypesWithAnnotations[i], useSiteDiagnosticInfo, isImplicitlyDeclared: false, tupleErrorFieldSymbol));
                    }
                }
            }
            instance.Free();
            instance2.AddRange(instance3);
            instance3.Free();
            if (smallDictionary != null)
            {
                TupleData!.SetFieldDefinitionsToIndexMap(smallDictionary);
            }
            return instance2;
            static void collectTargetTupleFields(int arity, ImmutableArray<Symbol> members, ArrayBuilder<FieldSymbol?> fieldsForElements)
            {
                int num4 = Math.Min(arity, 7);
                for (int k = 0; k < num4; k++)
                {
                    WellKnownMember tupleTypeMember = GetTupleTypeMember(arity, k + 1);
                    fieldsForElements.Add((FieldSymbol)GetWellKnownMemberInType(members, tupleTypeMember));
                }
            }
            static ImmutableArray<Location> getElementLocations(in ImmutableArray<Location?> elementLocations, int tupleFieldIndex)
            {
                if (elementLocations.IsDefault)
                {
                    return ImmutableArray<Location>.Empty;
                }
                Location location2 = elementLocations[tupleFieldIndex];
                if (!(location2 == null))
                {
                    return ImmutableArray.Create(location2);
                }
                return ImmutableArray<Location>.Empty;
            }
            static NamedTypeSymbol getNestedTupleUnderlyingType(NamedTypeSymbol topLevelUnderlyingType, int depth)
            {
                NamedTypeSymbol namedTypeSymbol2 = topLevelUnderlyingType;
                for (int j = 0; j < depth; j++)
                {
                    namedTypeSymbol2 = (NamedTypeSymbol)namedTypeSymbol2.TypeArgumentsWithAnnotationsNoUseSiteDiagnostics[7].Type;
                }
                return namedTypeSymbol2;
            }
            static ImmutableArray<Symbol> getOriginalFields(ImmutableArray<Symbol> members)
            {
                ArrayBuilder<Symbol> instance5 = ArrayBuilder<Symbol>.GetInstance();
                ImmutableArray<Symbol>.Enumerator enumerator2 = members.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (!(current2 is TupleVirtualElementFieldSymbol))
                    {
                        if (current2 is TupleElementFieldSymbol tupleElementFieldSymbol2)
                        {
                            instance5.Add(tupleElementFieldSymbol2.UnderlyingField.OriginalDefinition);
                        }
                        else if (current2 is FieldSymbol fieldSymbol4)
                        {
                            instance5.Add(fieldSymbol4.OriginalDefinition);
                        }
                    }
                }
                return instance5.ToImmutableAndFree();
            }
            static Symbol? GetWellKnownMemberInType(ImmutableArray<Symbol> members, WellKnownMember relativeMember)
            {
                MemberDescriptor descriptor = WellKnownMembers.GetDescriptor(relativeMember);
                return CSharpCompilation.GetRuntimeMember(members, in descriptor, CSharpCompilation.SpecialMembersSignatureComparer.Instance, null);
            }
        }

        private TypeSymbol MergeTupleNames(NamedTypeSymbol other, NamedTypeSymbol mergedType)
        {
            ImmutableArray<string> tupleElementNames = TupleElementNames;
            ImmutableArray<string> tupleElementNames2 = other.TupleElementNames;
            ImmutableArray<string> immutableArray;
            if (tupleElementNames.IsDefault || tupleElementNames2.IsDefault)
            {
                immutableArray = default(ImmutableArray<string>);
            }
            else
            {
                immutableArray = tupleElementNames.ZipAsArray(tupleElementNames2, (string n1, string n2) => (string.CompareOrdinal(n1, n2) != 0) ? null : n1);
                if (immutableArray.All((string n) => n == null))
                {
                    immutableArray = default(ImmutableArray<string>);
                }
            }
            if (!(immutableArray.IsDefault ? TupleElementNames.IsDefault : immutableArray.SequenceEqual(TupleElementNames)) || !Equals(mergedType, TypeCompareKind.ConsiderEverything))
            {
                return CreateTuple(mergedType, immutableArray, TupleErrorPositions, TupleElementLocations, Locations);
            }
            return this;
        }
    }
}
