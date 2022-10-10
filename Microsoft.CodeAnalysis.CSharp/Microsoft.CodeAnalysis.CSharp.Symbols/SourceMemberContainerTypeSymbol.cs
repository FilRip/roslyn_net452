using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceMemberContainerTypeSymbol : NamedTypeSymbol
    {
        private struct Flags
        {
            private int _flags;

            private const int SpecialTypeOffset = 0;

            private const int SpecialTypeSize = 6;

            private const int ManagedKindOffset = 6;

            private const int ManagedKindSize = 2;

            private const int FieldDefinitionsNotedOffset = 8;

            private const int FieldDefinitionsNotedSize = 1;

            private const int FlattenedMembersIsSortedOffset = 9;

            private const int FlattenedMembersIsSortedSize = 1;

            private const int TypeKindOffset = 10;

            private const int TypeKindSize = 4;

            private const int NullableContextOffset = 14;

            private const int NullableContextSize = 3;

            private const int SpecialTypeMask = 63;

            private const int ManagedKindMask = 3;

            private const int TypeKindMask = 15;

            private const int NullableContextMask = 7;

            private const int FieldDefinitionsNotedBit = 256;

            private const int FlattenedMembersIsSortedBit = 512;

            public SpecialType SpecialType => (SpecialType)(_flags & 0x3F);

            public ManagedKind ManagedKind => (ManagedKind)((uint)(_flags >> 6) & 3u);

            public bool FieldDefinitionsNoted => (_flags & 0x100) != 0;

            public bool FlattenedMembersIsSorted => (_flags & 0x200) != 0;

            public TypeKind TypeKind => (TypeKind)((uint)(_flags >> 10) & 0xFu);

            public Flags(SpecialType specialType, TypeKind typeKind)
            {
                int num = (int)(specialType & (SpecialType)63);
                int num2 = (int)((uint)(typeKind & (TypeKind)15) << 10);
                _flags = num | num2;
            }

            public void SetFieldDefinitionsNoted()
            {
                ThreadSafeFlagOperations.Set(ref _flags, 256);
            }

            public void SetFlattenedMembersIsSorted()
            {
                ThreadSafeFlagOperations.Set(ref _flags, 512);
            }

            private static bool BitsAreUnsetOrSame(int bits, int mask)
            {
                if ((bits & mask) != 0)
                {
                    return (bits & mask) == mask;
                }
                return true;
            }

            public void SetManagedKind(ManagedKind managedKind)
            {
                int toSet = (int)((uint)(managedKind & ManagedKind.Managed) << 6);
                ThreadSafeFlagOperations.Set(ref _flags, toSet);
            }

            public bool TryGetNullableContext(out byte? value)
            {
                return ((NullableContextKind)((uint)(_flags >> 14) & 7u)).TryGetByte(out value);
            }

            public bool SetNullableContext(byte? value)
            {
                return ThreadSafeFlagOperations.Set(ref _flags, (int)((uint)(value.ToNullableContextFlags() & (NullableContextKind)7) << 14));
            }
        }

        protected sealed class MembersAndInitializers
        {
            internal readonly ImmutableArray<Symbol> NonTypeMembers;

            internal readonly ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> StaticInitializers;

            internal readonly ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> InstanceInitializers;

            internal readonly bool HaveIndexers;

            internal readonly bool IsNullableEnabledForInstanceConstructorsAndFields;

            internal readonly bool IsNullableEnabledForStaticConstructorsAndFields;

            public MembersAndInitializers(ImmutableArray<Symbol> nonTypeMembers, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> staticInitializers, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> instanceInitializers, bool haveIndexers, bool isNullableEnabledForInstanceConstructorsAndFields, bool isNullableEnabledForStaticConstructorsAndFields)
            {
                NonTypeMembers = nonTypeMembers;
                StaticInitializers = staticInitializers;
                InstanceInitializers = instanceInitializers;
                HaveIndexers = haveIndexers;
                IsNullableEnabledForInstanceConstructorsAndFields = isNullableEnabledForInstanceConstructorsAndFields;
                IsNullableEnabledForStaticConstructorsAndFields = isNullableEnabledForStaticConstructorsAndFields;
            }
        }

        private sealed class DeclaredMembersAndInitializersBuilder
        {
            public ArrayBuilder<Symbol> NonTypeMembers = ArrayBuilder<Symbol>.GetInstance();

            public readonly ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>> StaticInitializers = ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>>.GetInstance();

            public readonly ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>> InstanceInitializers = ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>>.GetInstance();

            public bool HaveIndexers;

            public RecordDeclarationSyntax? RecordDeclarationWithParameters;

            public SynthesizedRecordConstructor? RecordPrimaryConstructor;

            public bool IsNullableEnabledForInstanceConstructorsAndFields;

            public bool IsNullableEnabledForStaticConstructorsAndFields;

            public DeclaredMembersAndInitializers ToReadOnlyAndFree(CSharpCompilation compilation)
            {
                return new DeclaredMembersAndInitializers(NonTypeMembers.ToImmutableAndFree(), MembersAndInitializersBuilder.ToReadOnlyAndFree(StaticInitializers), MembersAndInitializersBuilder.ToReadOnlyAndFree(InstanceInitializers), HaveIndexers, RecordDeclarationWithParameters, RecordPrimaryConstructor, IsNullableEnabledForInstanceConstructorsAndFields, IsNullableEnabledForStaticConstructorsAndFields, compilation);
            }

            public void UpdateIsNullableEnabledForConstructorsAndFields(bool useStatic, CSharpCompilation compilation, CSharpSyntaxNode syntax)
            {
                ref bool isNullableEnabledForConstructorsAndFields = ref GetIsNullableEnabledForConstructorsAndFields(useStatic);
                isNullableEnabledForConstructorsAndFields = isNullableEnabledForConstructorsAndFields || compilation.IsNullableAnalysisEnabledIn(syntax);
            }

            public void UpdateIsNullableEnabledForConstructorsAndFields(bool useStatic, bool value)
            {
                GetIsNullableEnabledForConstructorsAndFields(useStatic) |= value;
            }

            private ref bool GetIsNullableEnabledForConstructorsAndFields(bool useStatic)
            {
                if (!useStatic)
                {
                    return ref IsNullableEnabledForInstanceConstructorsAndFields;
                }
                return ref IsNullableEnabledForStaticConstructorsAndFields;
            }

            public void Free()
            {
                NonTypeMembers.Free();
                ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>>.Enumerator enumerator = StaticInitializers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Free();
                }
                StaticInitializers.Free();
                enumerator = InstanceInitializers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Free();
                }
                InstanceInitializers.Free();
            }

            internal void AddOrWrapTupleMembers(SourceMemberContainerTypeSymbol type)
            {
                NonTypeMembers = type.AddOrWrapTupleMembers(NonTypeMembers.ToImmutableAndFree());
            }
        }

        protected sealed class DeclaredMembersAndInitializers
        {
            public readonly ImmutableArray<Symbol> NonTypeMembers;

            public readonly ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> StaticInitializers;

            public readonly ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> InstanceInitializers;

            public readonly bool HaveIndexers;

            public readonly RecordDeclarationSyntax? RecordDeclarationWithParameters;

            public readonly SynthesizedRecordConstructor? RecordPrimaryConstructor;

            public readonly bool IsNullableEnabledForInstanceConstructorsAndFields;

            public readonly bool IsNullableEnabledForStaticConstructorsAndFields;

            public static readonly DeclaredMembersAndInitializers UninitializedSentinel = new DeclaredMembersAndInitializers();

            private DeclaredMembersAndInitializers()
            {
            }

            public DeclaredMembersAndInitializers(ImmutableArray<Symbol> nonTypeMembers, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> staticInitializers, ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> instanceInitializers, bool haveIndexers, RecordDeclarationSyntax? recordDeclarationWithParameters, SynthesizedRecordConstructor? recordPrimaryConstructor, bool isNullableEnabledForInstanceConstructorsAndFields, bool isNullableEnabledForStaticConstructorsAndFields, CSharpCompilation compilation)
            {
                NonTypeMembers = nonTypeMembers;
                StaticInitializers = staticInitializers;
                InstanceInitializers = instanceInitializers;
                HaveIndexers = haveIndexers;
                RecordDeclarationWithParameters = recordDeclarationWithParameters;
                RecordPrimaryConstructor = recordPrimaryConstructor;
                IsNullableEnabledForInstanceConstructorsAndFields = isNullableEnabledForInstanceConstructorsAndFields;
                IsNullableEnabledForStaticConstructorsAndFields = isNullableEnabledForStaticConstructorsAndFields;
            }

            [Conditional("DEBUG")]
            public static void AssertInitializers(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers, CSharpCompilation compilation)
            {
                if (!initializers.IsEmpty)
                {
                    ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator = initializers.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        _ = enumerator.Current;
                    }
                    for (int i = 0; i < initializers.Length; i++)
                    {
                        _ = 0;
                        _ = i + 1;
                        _ = initializers.Length;
                        _ = initializers[i].Length;
                        _ = 1;
                    }
                }
            }
        }

        private sealed class MembersAndInitializersBuilder
        {
            public ArrayBuilder<Symbol>? NonTypeMembers;

            private ArrayBuilder<FieldOrPropertyInitializer>? InstanceInitializersForPositionalMembers;

            private bool IsNullableEnabledForInstanceConstructorsAndFields;

            private bool IsNullableEnabledForStaticConstructorsAndFields;

            public MembersAndInitializersBuilder(DeclaredMembersAndInitializers declaredMembersAndInitializers)
            {
                IsNullableEnabledForInstanceConstructorsAndFields = declaredMembersAndInitializers.IsNullableEnabledForInstanceConstructorsAndFields;
                IsNullableEnabledForStaticConstructorsAndFields = declaredMembersAndInitializers.IsNullableEnabledForStaticConstructorsAndFields;
            }

            public MembersAndInitializers ToReadOnlyAndFree(DeclaredMembersAndInitializers declaredMembers)
            {
                DeclaredMembersAndInitializers declaredMembers2 = declaredMembers;
                return new MembersAndInitializers(NonTypeMembers?.ToImmutableAndFree() ?? declaredMembers2.NonTypeMembers, instanceInitializers: (InstanceInitializersForPositionalMembers == null) ? declaredMembers2.InstanceInitializers : mergeInitializers(), staticInitializers: declaredMembers2.StaticInitializers, haveIndexers: declaredMembers2.HaveIndexers, isNullableEnabledForInstanceConstructorsAndFields: IsNullableEnabledForInstanceConstructorsAndFields, isNullableEnabledForStaticConstructorsAndFields: IsNullableEnabledForStaticConstructorsAndFields);
                ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> mergeInitializers()
                {
                    int length = declaredMembers2.InstanceInitializers.Length;
                    if (length == 0)
                    {
                        return ImmutableArray.Create(InstanceInitializersForPositionalMembers!.ToImmutableAndFree());
                    }
                    CSharpCompilation declaringCompilation = declaredMembers2.RecordPrimaryConstructor!.DeclaringCompilation;
                    LexicalSortKey xSortKey = new LexicalSortKey(InstanceInitializersForPositionalMembers!.First().Syntax, declaringCompilation);
                    int i;
                    for (i = 0; i < length && LexicalSortKey.Compare(xSortKey, new LexicalSortKey(declaredMembers2.InstanceInitializers[i][0].Syntax, declaringCompilation)) >= 0; i++)
                    {
                    }
                    ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> instance;
                    if (i != length && declaredMembers2.RecordDeclarationWithParameters!.SyntaxTree == declaredMembers2.InstanceInitializers[i][0].Syntax.SyntaxTree && declaredMembers2.RecordDeclarationWithParameters!.Span.Contains(declaredMembers2.InstanceInitializers[i][0].Syntax.Span.Start))
                    {
                        ImmutableArray<FieldOrPropertyInitializer> items = declaredMembers2.InstanceInitializers[i];
                        ArrayBuilder<FieldOrPropertyInitializer> instanceInitializersForPositionalMembers = InstanceInitializersForPositionalMembers;
                        instanceInitializersForPositionalMembers.AddRange(items);
                        instance = ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>>.GetInstance(length);
                        instance.AddRange(declaredMembers2.InstanceInitializers, i);
                        instance.Add(instanceInitializersForPositionalMembers.ToImmutableAndFree());
                        instance.AddRange(declaredMembers2.InstanceInitializers, i + 1, length - (i + 1));
                    }
                    else
                    {
                        instance = ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>>.GetInstance(length + 1);
                        instance.AddRange(declaredMembers2.InstanceInitializers, i);
                        instance.Add(InstanceInitializersForPositionalMembers!.ToImmutableAndFree());
                        instance.AddRange(declaredMembers2.InstanceInitializers, i, length - i);
                    }
                    return instance.ToImmutableAndFree();
                }
            }

            public void AddInstanceInitializerForPositionalMembers(FieldOrPropertyInitializer initializer)
            {
                if (InstanceInitializersForPositionalMembers == null)
                {
                    InstanceInitializersForPositionalMembers = ArrayBuilder<FieldOrPropertyInitializer>.GetInstance();
                }
                InstanceInitializersForPositionalMembers!.Add(initializer);
            }

            public IReadOnlyCollection<Symbol> GetNonTypeMembers(DeclaredMembersAndInitializers declaredMembers)
            {
                IReadOnlyCollection<Symbol> nonTypeMembers = NonTypeMembers;
                return (IReadOnlyCollection<Symbol>)(nonTypeMembers ?? ((object)declaredMembers.NonTypeMembers));
            }

            public void AddNonTypeMember(Symbol member, DeclaredMembersAndInitializers declaredMembers)
            {
                if (NonTypeMembers == null)
                {
                    NonTypeMembers = ArrayBuilder<Symbol>.GetInstance(declaredMembers.NonTypeMembers.Length + 1);
                    NonTypeMembers!.AddRange(declaredMembers.NonTypeMembers);
                }
                NonTypeMembers!.Add(member);
            }

            public void UpdateIsNullableEnabledForConstructorsAndFields(bool useStatic, CSharpCompilation compilation, CSharpSyntaxNode syntax)
            {
                ref bool isNullableEnabledForConstructorsAndFields = ref GetIsNullableEnabledForConstructorsAndFields(useStatic);
                isNullableEnabledForConstructorsAndFields = isNullableEnabledForConstructorsAndFields || compilation.IsNullableAnalysisEnabledIn(syntax);
            }

            private ref bool GetIsNullableEnabledForConstructorsAndFields(bool useStatic)
            {
                if (!useStatic)
                {
                    return ref IsNullableEnabledForInstanceConstructorsAndFields;
                }
                return ref IsNullableEnabledForStaticConstructorsAndFields;
            }

            internal static ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> ToReadOnlyAndFree(ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>> initializers)
            {
                if (initializers.Count == 0)
                {
                    initializers.Free();
                    return ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Empty;
                }
                ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>> instance = ArrayBuilder<ImmutableArray<FieldOrPropertyInitializer>>.GetInstance(initializers.Count);
                ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>>.Enumerator enumerator = initializers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ArrayBuilder<FieldOrPropertyInitializer> current = enumerator.Current;
                    instance.Add(current.ToImmutableAndFree());
                }
                initializers.Free();
                return instance.ToImmutableAndFree();
            }

            public void Free()
            {
                NonTypeMembers?.Free();
                InstanceInitializersForPositionalMembers?.Free();
            }
        }

        private enum HasBaseTypeDeclaringInterfaceResult
        {
            NoMatch,
            IgnoringNullableMatch,
            ExactMatch
        }

        private static readonly ObjectPool<PooledDictionary<Symbol, Symbol>> s_duplicateRecordMemberSignatureDictionary = PooledDictionary<Symbol, Symbol>.CreatePool(MemberSignatureComparer.RecordAPISignatureComparer);

        protected SymbolCompletionState state;

        private Flags _flags;

        private ImmutableArray<DiagnosticInfo> _managedKindUseSiteDiagnostics;

        private ImmutableArray<AssemblySymbol> _managedKindUseSiteDependencies;

        private readonly DeclarationModifiers _declModifiers;

        private readonly NamespaceOrTypeSymbol _containingSymbol;

        protected readonly MergedTypeDeclaration declaration;

        private DeclaredMembersAndInitializers? _lazyDeclaredMembersAndInitializers = DeclaredMembersAndInitializers.UninitializedSentinel;

        private MembersAndInitializers? _lazyMembersAndInitializers;

        private Dictionary<string, ImmutableArray<Symbol>>? _lazyMembersDictionary;

        private Dictionary<string, ImmutableArray<Symbol>>? _lazyEarlyAttributeDecodingMembersDictionary;

        private static readonly Dictionary<string, ImmutableArray<NamedTypeSymbol>> s_emptyTypeMembers = new Dictionary<string, ImmutableArray<NamedTypeSymbol>>(EmptyComparer.Instance);

        private Dictionary<string, ImmutableArray<NamedTypeSymbol>>? _lazyTypeMembers;

        private ImmutableArray<Symbol> _lazyMembersFlattened;

        private ImmutableArray<SynthesizedExplicitImplementationForwardingMethod> _lazySynthesizedExplicitImplementations;

        private int _lazyKnownCircularStruct;

        private LexicalSortKey _lazyLexicalSortKey = LexicalSortKey.NotInitialized;

        private ThreeState _lazyContainsExtensionMethods;

        private ThreeState _lazyAnyMemberHasAttributes;

        private static readonly ReportMismatchInReturnType<Location> ReportBadReturn = delegate (BindingDiagnosticBag diagnostics, MethodSymbol overriddenMethod, MethodSymbol overridingMethod, bool topLevel, Location location)
        {
            diagnostics.Add(topLevel ? ErrorCode.WRN_TopLevelNullabilityMismatchInReturnTypeOnOverride : ErrorCode.WRN_NullabilityMismatchInReturnTypeOnOverride, location);
        };

        private static readonly ReportMismatchInParameterType<Location> ReportBadParameter = delegate (BindingDiagnosticBag diagnostics, MethodSymbol overriddenMethod, MethodSymbol overridingMethod, ParameterSymbol overridingParameter, bool topLevel, Location location)
        {
            diagnostics.Add(topLevel ? ErrorCode.WRN_TopLevelNullabilityMismatchInParameterTypeOnOverride : ErrorCode.WRN_NullabilityMismatchInParameterTypeOnOverride, location, new FormattedSymbol(overridingParameter, SymbolDisplayFormat.ShortFormat));
        };

        internal sealed override bool RequiresCompletion => true;

        public sealed override NamedTypeSymbol? ContainingType => _containingSymbol as NamedTypeSymbol;

        public sealed override Symbol ContainingSymbol => _containingSymbol;

        public override SpecialType SpecialType => _flags.SpecialType;

        public override TypeKind TypeKind => _flags.TypeKind;

        internal MergedTypeDeclaration MergedDeclaration => declaration;

        internal sealed override bool IsInterface => TypeKind == TypeKind.Interface;

        public override bool IsStatic => HasFlag(DeclarationModifiers.Static);

        public sealed override bool IsRefLikeType => HasFlag(DeclarationModifiers.Ref);

        public override bool IsReadOnly => HasFlag(DeclarationModifiers.ReadOnly);

        public override bool IsSealed => HasFlag(DeclarationModifiers.Sealed);

        public override bool IsAbstract => HasFlag(DeclarationModifiers.Abstract);

        internal bool IsPartial => HasFlag(DeclarationModifiers.Partial);

        internal bool IsNew => HasFlag(DeclarationModifiers.New);

        public override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(_declModifiers);

        public override bool IsScriptClass
        {
            get
            {
                DeclarationKind kind = declaration.Declarations[0].Kind;
                if (kind != DeclarationKind.Script)
                {
                    return kind == DeclarationKind.Submission;
                }
                return true;
            }
        }

        public override bool IsImplicitClass => declaration.Declarations[0].Kind == DeclarationKind.ImplicitClass;

        internal override bool IsRecord => declaration.Declarations[0].Kind == DeclarationKind.Record;

        internal override bool IsRecordStruct => declaration.Declarations[0].Kind == DeclarationKind.RecordStruct;

        public override bool IsImplicitlyDeclared
        {
            get
            {
                if (!IsImplicitClass)
                {
                    return IsScriptClass;
                }
                return true;
            }
        }

        public override int Arity => declaration.Arity;

        public override string Name => declaration.Name;

        internal override bool MangleName => Arity > 0;

        public override ImmutableArray<Location> Locations => declaration.NameLocations.Cast<SourceLocation, Location>();

        public ImmutableArray<SyntaxReference> SyntaxReferences => declaration.SyntaxReferences;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => SyntaxReferences;

        internal ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> StaticInitializers => GetMembersAndInitializers().StaticInitializers;

        internal ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> InstanceInitializers => GetMembersAndInitializers().InstanceInitializers;

        public override IEnumerable<string> MemberNames
        {
            get
            {
                if (!IsTupleType && !IsRecord && !IsRecordStruct)
                {
                    return declaration.MemberNames;
                }
                return from m in GetMembers()
                       select m.Name;
            }
        }

        internal override bool KnownCircularStruct
        {
            get
            {
                if (_lazyKnownCircularStruct == 0)
                {
                    if (TypeKind != TypeKind.Struct)
                    {
                        Interlocked.CompareExchange(ref _lazyKnownCircularStruct, 1, 0);
                    }
                    else
                    {
                        BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                        int value = (int)CheckStructCircularity(instance).ToThreeState();
                        if (Interlocked.CompareExchange(ref _lazyKnownCircularStruct, value, 0) == 0)
                        {
                            AddDeclarationDiagnostics(instance);
                        }
                        instance.Free();
                    }
                }
                return _lazyKnownCircularStruct == 2;
            }
        }

        internal bool ContainsExtensionMethods
        {
            get
            {
                if (!_lazyContainsExtensionMethods.HasValue())
                {
                    bool value = ((IsStatic && !base.IsGenericType) || IsScriptClass) && declaration.ContainsExtensionMethods;
                    _lazyContainsExtensionMethods = value.ToThreeState();
                }
                return _lazyContainsExtensionMethods.Value();
            }
        }

        internal bool AnyMemberHasAttributes
        {
            get
            {
                if (!_lazyAnyMemberHasAttributes.HasValue())
                {
                    bool anyMemberHasAttributes = declaration.AnyMemberHasAttributes;
                    _lazyAnyMemberHasAttributes = anyMemberHasAttributes.ToThreeState();
                }
                return _lazyAnyMemberHasAttributes.Value();
            }
        }

        public override bool MightContainExtensionMethods => ContainsExtensionMethods;

        public sealed override NamedTypeSymbol ConstructedFrom => this;

        public SourceMemberContainerTypeSymbol(NamespaceOrTypeSymbol containingSymbol, MergedTypeDeclaration declaration, BindingDiagnosticBag diagnostics, TupleExtraData? tupleData = null)
            : base(tupleData)
        {
            _containingSymbol = containingSymbol;
            this.declaration = declaration;
            TypeKind typeKind = declaration.Kind.ToTypeKind();
            DeclarationModifiers declarationModifiers = MakeModifiers(typeKind, diagnostics);
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                diagnostics.AddRange(current.Diagnostics);
            }
            int num = (int)(declarationModifiers & DeclarationModifiers.AccessibilityMask);
            if ((num & (num - 1)) != 0)
            {
                if ((declarationModifiers & DeclarationModifiers.Partial) != 0)
                {
                    diagnostics.Add(ErrorCode.ERR_PartialModifierConflict, Locations[0], this);
                }
                num &= ~(num - 1);
                declarationModifiers &= ~DeclarationModifiers.AccessibilityMask;
                declarationModifiers = (DeclarationModifiers)((uint)declarationModifiers | (uint)num);
            }
            _declModifiers = declarationModifiers;
            SpecialType specialType = ((num == 16) ? MakeSpecialType() : SpecialType.None);
            _flags = new Flags(specialType, typeKind);
            NamedTypeSymbol? containingType = ContainingType;
            if ((object)containingType != null && containingType!.IsSealed && DeclaredAccessibility.HasProtected())
            {
                diagnostics.Add(AccessCheck.GetProtectedMemberInSealedTypeError(ContainingType), Locations[0], this);
            }
            state.NotePartComplete(CompletionPart.TypeArguments);
        }

        private SpecialType MakeSpecialType()
        {
            if (ContainingSymbol.Kind == SymbolKind.Namespace && ContainingSymbol.ContainingAssembly.KeepLookingForDeclaredSpecialTypes)
            {
                return SpecialTypes.GetTypeFromMetadataName(MetadataHelpers.BuildQualifiedName(ContainingSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat), MetadataName));
            }
            return SpecialType.None;
        }

        private DeclarationModifiers MakeModifiers(TypeKind typeKind, BindingDiagnosticBag diagnostics)
        {
            Symbol containingSymbol = ContainingSymbol;
            DeclarationModifiers declarationModifiers = DeclarationModifiers.AccessibilityMask;
            DeclarationModifiers defaultAccess;
            if (containingSymbol.Kind == SymbolKind.Namespace)
            {
                defaultAccess = DeclarationModifiers.Internal;
            }
            else
            {
                declarationModifiers |= DeclarationModifiers.New;
                defaultAccess = ((!((NamedTypeSymbol)containingSymbol).IsInterface) ? DeclarationModifiers.Private : DeclarationModifiers.Public);
            }
            switch (typeKind)
            {
                case TypeKind.Class:
                case TypeKind.Submission:
                    declarationModifiers |= DeclarationModifiers.Abstract | DeclarationModifiers.Sealed | DeclarationModifiers.Partial | DeclarationModifiers.Unsafe;
                    if (!IsRecord)
                    {
                        declarationModifiers |= DeclarationModifiers.Static;
                    }
                    break;
                case TypeKind.Struct:
                    declarationModifiers |= DeclarationModifiers.ReadOnly | DeclarationModifiers.Partial | DeclarationModifiers.Unsafe;
                    if (!IsRecordStruct)
                    {
                        declarationModifiers |= DeclarationModifiers.Ref;
                    }
                    break;
                case TypeKind.Interface:
                    declarationModifiers |= DeclarationModifiers.Partial | DeclarationModifiers.Unsafe;
                    break;
                case TypeKind.Delegate:
                    declarationModifiers |= DeclarationModifiers.Unsafe;
                    break;
            }
            DeclarationModifiers declarationModifiers2 = MakeAndCheckTypeModifiers(defaultAccess, declarationModifiers, diagnostics, out bool modifierErrors);
            this.CheckUnsafeModifier(declarationModifiers2, diagnostics);
            if (!modifierErrors && (declarationModifiers2 & DeclarationModifiers.Abstract) != 0 && (declarationModifiers2 & (DeclarationModifiers.Sealed | DeclarationModifiers.Static)) != 0)
            {
                diagnostics.Add(ErrorCode.ERR_AbstractSealedStatic, Locations[0], this);
            }
            if (!modifierErrors && (declarationModifiers2 & (DeclarationModifiers.Sealed | DeclarationModifiers.Static)) == (DeclarationModifiers.Sealed | DeclarationModifiers.Static))
            {
                diagnostics.Add(ErrorCode.ERR_SealedStaticClass, Locations[0], this);
            }
            switch (typeKind)
            {
                case TypeKind.Interface:
                    declarationModifiers2 |= DeclarationModifiers.Abstract;
                    break;
                case TypeKind.Enum:
                case TypeKind.Struct:
                    declarationModifiers2 |= DeclarationModifiers.Sealed;
                    break;
                case TypeKind.Delegate:
                    declarationModifiers2 |= DeclarationModifiers.Sealed;
                    break;
            }
            return declarationModifiers2;
        }

        private DeclarationModifiers MakeAndCheckTypeModifiers(DeclarationModifiers defaultAccess, DeclarationModifiers allowedModifiers, BindingDiagnosticBag diagnostics, out bool modifierErrors)
        {
            modifierErrors = false;
            DeclarationModifiers declarationModifiers = DeclarationModifiers.Unset;
            int length = declaration.Declarations.Length;
            bool flag = false;
            for (int i = 0; i < length; i++)
            {
                DeclarationModifiers declarationModifiers2 = declaration.Declarations[i].Modifiers;
                if (length > 1 && (declarationModifiers2 & DeclarationModifiers.Partial) == 0)
                {
                    flag = true;
                }
                if (!modifierErrors)
                {
                    declarationModifiers2 = ModifierUtils.CheckModifiers(declarationModifiers2, allowedModifiers, declaration.Declarations[i].NameLocation, diagnostics, null, out modifierErrors);
                    if (!modifierErrors)
                    {
                        CSDiagnosticInfo cSDiagnosticInfo = ModifierUtils.CheckAccessibility(declarationModifiers2, this, isExplicitInterfaceImplementation: false);
                        if (cSDiagnosticInfo != null)
                        {
                            diagnostics.Add(cSDiagnosticInfo, Locations[0]);
                            modifierErrors = true;
                        }
                    }
                }
                declarationModifiers = ((declarationModifiers != DeclarationModifiers.Unset) ? (declarationModifiers | declarationModifiers2) : declarationModifiers2);
            }
            if ((declarationModifiers & DeclarationModifiers.AccessibilityMask) == 0)
            {
                declarationModifiers |= defaultAccess;
            }
            if (flag)
            {
                if ((declarationModifiers & DeclarationModifiers.Partial) == 0)
                {
                    switch (ContainingSymbol.Kind)
                    {
                        case SymbolKind.Namespace:
                            {
                                for (int k = 1; k < length; k++)
                                {
                                    diagnostics.Add(ErrorCode.ERR_DuplicateNameInNS, declaration.Declarations[k].NameLocation, Name, ContainingSymbol);
                                    modifierErrors = true;
                                }
                                break;
                            }
                        case SymbolKind.NamedType:
                            {
                                for (int j = 1; j < length; j++)
                                {
                                    if (ContainingType!.Locations.Length == 1 || ContainingType.IsPartial())
                                    {
                                        diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, declaration.Declarations[j].NameLocation, ContainingSymbol, Name);
                                    }
                                    modifierErrors = true;
                                }
                                break;
                            }
                    }
                }
                else
                {
                    for (int l = 0; l < length; l++)
                    {
                        SingleTypeDeclaration singleTypeDeclaration = declaration.Declarations[l];
                        if ((singleTypeDeclaration.Modifiers & DeclarationModifiers.Partial) == 0)
                        {
                            diagnostics.Add(ErrorCode.ERR_MissingPartial, singleTypeDeclaration.NameLocation, Name);
                            modifierErrors = true;
                        }
                    }
                }
            }
            if (Name == SyntaxFacts.GetText(SyntaxKind.RecordKeyword))
            {
                ImmutableArray<SyntaxReference>.Enumerator enumerator = SyntaxReferences.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxNode syntax = enumerator.Current.GetSyntax();
                    SyntaxToken? syntaxToken = ((syntax is BaseTypeDeclarationSyntax baseTypeDeclarationSyntax) ? new SyntaxToken?(baseTypeDeclarationSyntax.Identifier) : ((!(syntax is DelegateDeclarationSyntax delegateDeclarationSyntax)) ? null : new SyntaxToken?(delegateDeclarationSyntax.Identifier)));
                    SyntaxToken? syntaxToken2 = syntaxToken;
                    ReportTypeNamedRecord(syntaxToken2?.Text, DeclaringCompilation, diagnostics.DiagnosticBag, syntaxToken2?.GetLocation() ?? Location.None);
                }
            }
            return declarationModifiers;
        }

        internal static void ReportTypeNamedRecord(string? name, CSharpCompilation compilation, DiagnosticBag? diagnostics, Location location)
        {
            if (diagnostics != null && name == SyntaxFacts.GetText(SyntaxKind.RecordKeyword) && compilation.LanguageVersion >= MessageID.IDS_FeatureRecords.RequiredVersion())
            {
                diagnostics.Add(ErrorCode.WRN_RecordNamedDisallowed, location, name);
            }
        }

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return state.HasComplete(part);
        }

        protected abstract void CheckBase(BindingDiagnosticBag diagnostics);

        protected abstract void CheckInterfaces(BindingDiagnosticBag diagnostics);

        internal override void ForceComplete(SourceLocation? locationOpt, CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                CompletionPart nextIncompletePart = state.NextIncompletePart;
                switch (nextIncompletePart)
                {
                    case CompletionPart.Attributes:
                        GetAttributes();
                        break;
                    case CompletionPart.StartBaseType:
                    case CompletionPart.FinishBaseType:
                        if (state.NotePartComplete(CompletionPart.StartBaseType))
                        {
                            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                            CheckBase(instance);
                            AddDeclarationDiagnostics(instance);
                            state.NotePartComplete(CompletionPart.FinishBaseType);
                            instance.Free();
                        }
                        break;
                    case CompletionPart.StartInterfaces:
                    case CompletionPart.FinishInterfaces:
                        if (state.NotePartComplete(CompletionPart.StartInterfaces))
                        {
                            BindingDiagnosticBag instance3 = BindingDiagnosticBag.GetInstance();
                            CheckInterfaces(instance3);
                            AddDeclarationDiagnostics(instance3);
                            state.NotePartComplete(CompletionPart.FinishInterfaces);
                            instance3.Free();
                        }
                        break;
                    case CompletionPart.EnumUnderlyingType:
                        _ = EnumUnderlyingType;
                        break;
                    case CompletionPart.TypeArguments:
                        _ = TypeArgumentsWithAnnotationsNoUseSiteDiagnostics;
                        break;
                    case CompletionPart.TypeParameters:
                        {
                            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator2 = TypeParameters.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                enumerator2.Current.ForceComplete(locationOpt, cancellationToken);
                            }
                            state.NotePartComplete(CompletionPart.TypeParameters);
                            break;
                        }
                    case CompletionPart.Members:
                        GetMembersByName();
                        break;
                    case CompletionPart.TypeMembers:
                        GetTypeMembersUnordered();
                        break;
                    case CompletionPart.SynthesizedExplicitImplementations:
                        GetSynthesizedExplicitImplementations(cancellationToken);
                        break;
                    case CompletionPart.StartMemberChecks:
                    case CompletionPart.FinishMemberChecks:
                        if (state.NotePartComplete(CompletionPart.StartMemberChecks))
                        {
                            BindingDiagnosticBag instance2 = BindingDiagnosticBag.GetInstance();
                            AfterMembersChecks(instance2);
                            AddDeclarationDiagnostics(instance2);
                            DeclaringCompilation.SymbolDeclaredEvent(this);
                            state.NotePartComplete(CompletionPart.FinishMemberChecks);
                            instance2.Free();
                        }
                        break;
                    case CompletionPart.MembersCompleted:
                        {
                            ImmutableArray<Symbol> membersUnordered = GetMembersUnordered();
                            bool flag = true;
                            if (locationOpt == null)
                            {
                                ImmutableArray<Symbol>.Enumerator enumerator = membersUnordered.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    Symbol current = enumerator.Current;
                                    cancellationToken.ThrowIfCancellationRequested();
                                    current.ForceComplete(locationOpt, cancellationToken);
                                }
                            }
                            else
                            {
                                ImmutableArray<Symbol>.Enumerator enumerator = membersUnordered.GetEnumerator();
                                while (enumerator.MoveNext())
                                {
                                    Symbol current2 = enumerator.Current;
                                    Symbol.ForceCompleteMemberByLocation(locationOpt, current2, cancellationToken);
                                    flag = flag && current2.HasComplete(CompletionPart.All);
                                }
                            }
                            if (!flag)
                            {
                                CompletionPart part = CompletionPart.NamedTypeSymbolWithLocationAll;
                                state.SpinWaitComplete(part, cancellationToken);
                                return;
                            }
                            EnsureFieldDefinitionsNoted();
                            state.NotePartComplete(CompletionPart.MembersCompleted);
                            break;
                        }
                    case CompletionPart.None:
                        return;
                    default:
                        state.NotePartComplete(CompletionPart.ReturnTypeAttributes | CompletionPart.Parameters | CompletionPart.Type);
                        break;
                }
                state.SpinWaitComplete(nextIncompletePart, cancellationToken);
            }
        }

        internal void EnsureFieldDefinitionsNoted()
        {
            if (!_flags.FieldDefinitionsNoted)
            {
                NoteFieldDefinitions();
            }
        }

        private void NoteFieldDefinitions()
        {
            MembersAndInitializers membersAndInitializers = GetMembersAndInitializers();
            lock (membersAndInitializers)
            {
                if (_flags.FieldDefinitionsNoted)
                {
                    return;
                }
                SourceAssemblySymbol sourceAssemblySymbol = (SourceAssemblySymbol)ContainingAssembly;
                Accessibility accessibility = EffectiveAccessibility();
                ImmutableArray<Symbol>.Enumerator enumerator = membersAndInitializers.NonTypeMembers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.IsFieldOrFieldLikeEvent(out var field) && !field.IsConst && !field.IsFixedSizeBuffer)
                    {
                        Accessibility declaredAccessibility = field.DeclaredAccessibility;
                        if (declaredAccessibility == Accessibility.Private)
                        {
                            sourceAssemblySymbol.NoteFieldDefinition(field, isInternal: false, isUnread: true);
                        }
                        else if (accessibility == Accessibility.Private)
                        {
                            sourceAssemblySymbol.NoteFieldDefinition(field, isInternal: false, isUnread: false);
                        }
                        else if (declaredAccessibility == Accessibility.Internal || accessibility == Accessibility.Internal)
                        {
                            sourceAssemblySymbol.NoteFieldDefinition(field, isInternal: true, isUnread: false);
                        }
                    }
                }
                _flags.SetFieldDefinitionsNoted();
            }
        }

        internal override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            ManagedKind managedKind = _flags.ManagedKind;
            if (managedKind == ManagedKind.Unknown)
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = new CompoundUseSiteInfo<AssemblySymbol>(ContainingAssembly);
                managedKind = base.GetManagedKind(ref useSiteInfo2);
                ImmutableInterlocked.InterlockedInitialize(ref _managedKindUseSiteDiagnostics, useSiteInfo2.Diagnostics?.ToImmutableArray() ?? ImmutableArray<DiagnosticInfo>.Empty);
                ImmutableInterlocked.InterlockedInitialize(ref _managedKindUseSiteDependencies, useSiteInfo2.Dependencies?.ToImmutableArray() ?? ImmutableArray<AssemblySymbol>.Empty);
                _flags.SetManagedKind(managedKind);
            }
            if (useSiteInfo.AccumulatesDiagnostics)
            {
                ImmutableArray<DiagnosticInfo> managedKindUseSiteDiagnostics = _managedKindUseSiteDiagnostics;
                managedKindUseSiteDiagnostics = ImmutableInterlocked.InterlockedCompareExchange(ref _managedKindUseSiteDiagnostics, managedKindUseSiteDiagnostics, managedKindUseSiteDiagnostics);
                useSiteInfo.AddDiagnostics(managedKindUseSiteDiagnostics);
            }
            if (useSiteInfo.AccumulatesDependencies)
            {
                ImmutableArray<AssemblySymbol> managedKindUseSiteDependencies = _managedKindUseSiteDependencies;
                managedKindUseSiteDependencies = ImmutableInterlocked.InterlockedCompareExchange(ref _managedKindUseSiteDependencies, managedKindUseSiteDependencies, managedKindUseSiteDependencies);
                useSiteInfo.AddDependencies(managedKindUseSiteDependencies);
            }
            return managedKind;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool HasFlag(DeclarationModifiers flag)
        {
            return (_declModifiers & flag) != 0;
        }

        private Accessibility EffectiveAccessibility()
        {
            Accessibility accessibility = DeclaredAccessibility;
            if (accessibility == Accessibility.Private)
            {
                return Accessibility.Private;
            }
            Symbol containingType = ContainingType;
            while ((object)containingType != null)
            {
                switch (containingType.DeclaredAccessibility)
                {
                    case Accessibility.Private:
                        return Accessibility.Private;
                    case Accessibility.Internal:
                        accessibility = Accessibility.Internal;
                        break;
                }
                containingType = containingType.ContainingType;
            }
            return accessibility;
        }

        internal override LexicalSortKey GetLexicalSortKey()
        {
            if (!_lazyLexicalSortKey.IsInitialized)
            {
                _lazyLexicalSortKey.SetFrom(declaration.GetLexicalSortKey(DeclaringCompilation));
            }
            return _lazyLexicalSortKey;
        }

        internal override bool IsDefinedInSourceTree(SyntaxTree tree, TextSpan? definedWithinSpan, CancellationToken cancellationToken)
        {
            ImmutableArray<SingleTypeDeclaration> declarations = declaration.Declarations;
            if (IsImplicitlyDeclared && declarations.IsEmpty)
            {
                return ContainingSymbol.IsDefinedInSourceTree(tree, definedWithinSpan, cancellationToken);
            }
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                SyntaxReference syntaxReference = current.SyntaxReference;
                if (syntaxReference.SyntaxTree == tree && (!definedWithinSpan.HasValue || syntaxReference.Span.IntersectsWith(definedWithinSpan.Value)))
                {
                    return true;
                }
            }
            return false;
        }

        internal int CalculateSyntaxOffsetInSynthesizedConstructor(int position, SyntaxTree tree, bool isStatic)
        {
            if (IsScriptClass && !isStatic)
            {
                int num = 0;
                ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SyntaxReference syntaxReference = enumerator.Current.SyntaxReference;
                    if (tree == syntaxReference.SyntaxTree)
                    {
                        return num + position;
                    }
                    num += syntaxReference.Span.Length;
                }
                throw ExceptionUtilities.Unreachable;
            }
            if (TryCalculateSyntaxOffsetOfPositionInInitializer(position, tree, isStatic, 0, out var syntaxOffset))
            {
                return syntaxOffset;
            }
            if (declaration.Declarations.Length >= 1 && position == declaration.Declarations[0].Location.SourceSpan.Start)
            {
                return 0;
            }
            throw ExceptionUtilities.Unreachable;
        }

        internal bool TryCalculateSyntaxOffsetOfPositionInInitializer(int position, SyntaxTree tree, bool isStatic, int ctorInitializerLength, out int syntaxOffset)
        {
            MembersAndInitializers membersAndInitializers = GetMembersAndInitializers();
            ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers2 = (isStatic ? membersAndInitializers.StaticInitializers : membersAndInitializers.InstanceInitializers);
            if (!findInitializer(initializers2, position, tree, out var found2, out var precedingLength2))
            {
                syntaxOffset = 0;
                return false;
            }
            int num = getInitializersLength(initializers2);
            int num2 = position - found2.Syntax.Span.Start;
            int num3 = num + ctorInitializerLength - (precedingLength2 + num2);
            syntaxOffset = -num3;
            return true;
            static bool findInitializer(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers, int position, SyntaxTree tree, out FieldOrPropertyInitializer found, out int precedingLength)
            {
                precedingLength = 0;
                ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator3 = initializers.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    ImmutableArray<FieldOrPropertyInitializer> current3 = enumerator3.Current;
                    if (!current3.IsEmpty && current3[0].Syntax.SyntaxTree == tree && position < current3.Last().Syntax.Span.End)
                    {
                        int num7 = IndexOfInitializerContainingPosition(current3, position);
                        if (num7 < 0)
                        {
                            break;
                        }
                        precedingLength += getPrecedingInitializersLength(current3, num7);
                        found = current3[num7];
                        return true;
                    }
                    precedingLength += getGroupLength(current3);
                }
                found = default(FieldOrPropertyInitializer);
                return false;
            }
            static int getGroupLength(ImmutableArray<FieldOrPropertyInitializer> initializers)
            {
                int num5 = 0;
                ImmutableArray<FieldOrPropertyInitializer>.Enumerator enumerator2 = initializers.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    FieldOrPropertyInitializer current2 = enumerator2.Current;
                    num5 += getInitializerLength(current2);
                }
                return num5;
            }
            static int getInitializerLength(FieldOrPropertyInitializer initializer)
            {
                if (initializer.FieldOpt == null || !initializer.FieldOpt.IsMetadataConstant)
                {
                    return initializer.Syntax.Span.Length;
                }
                return 0;
            }
            static int getInitializersLength(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers)
            {
                int num4 = 0;
                ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>>.Enumerator enumerator = initializers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    ImmutableArray<FieldOrPropertyInitializer> current = enumerator.Current;
                    num4 += getGroupLength(current);
                }
                return num4;
            }
            static int getPrecedingInitializersLength(ImmutableArray<FieldOrPropertyInitializer> initializers, int index)
            {
                int num6 = 0;
                for (int i = 0; i < index; i++)
                {
                    num6 += getInitializerLength(initializers[i]);
                }
                return num6;
            }
        }

        private static int IndexOfInitializerContainingPosition(ImmutableArray<FieldOrPropertyInitializer> initializers, int position)
        {
            int num = initializers.BinarySearch(position, (FieldOrPropertyInitializer initializer, int pos) => initializer.Syntax.Span.Start.CompareTo(pos));
            if (num >= 0)
            {
                return num;
            }
            int num2 = ~num - 1;
            if (num2 >= 0 && initializers[num2].Syntax.Span.Contains(position))
            {
                return num2;
            }
            return -1;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetTypeMembersUnordered()
        {
            return GetTypeMembersDictionary().Flatten();
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers()
        {
            return GetTypeMembersDictionary().Flatten(LexicalOrderSymbolComparer.Instance);
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name)
        {
            if (GetTypeMembersDictionary().TryGetValue(name, out var value))
            {
                return value;
            }
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        public override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity)
        {
            return GetTypeMembers(name).WhereAsArray((NamedTypeSymbol t, int arity) => t.Arity == arity, arity);
        }

        private Dictionary<string, ImmutableArray<NamedTypeSymbol>> GetTypeMembersDictionary()
        {
            if (_lazyTypeMembers == null)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                if (Interlocked.CompareExchange(ref _lazyTypeMembers, MakeTypeMembers(instance), null) == null)
                {
                    AddDeclarationDiagnostics(instance);
                    state.NotePartComplete(CompletionPart.TypeMembers);
                }
                instance.Free();
            }
            return _lazyTypeMembers;
        }

        private Dictionary<string, ImmutableArray<NamedTypeSymbol>> MakeTypeMembers(BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<NamedTypeSymbol> instance = ArrayBuilder<NamedTypeSymbol>.GetInstance();
            Dictionary<(string, int), SourceNamedTypeSymbol> dictionary = new Dictionary<(string, int), SourceNamedTypeSymbol>();
            try
            {
                ImmutableArray<MergedTypeDeclaration>.Enumerator enumerator = declaration.Children.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    MergedTypeDeclaration current = enumerator.Current;
                    SourceNamedTypeSymbol sourceNamedTypeSymbol = new SourceNamedTypeSymbol(this, current, diagnostics);
                    CheckMemberNameDistinctFromType(sourceNamedTypeSymbol, diagnostics);
                    (string, int) key = (sourceNamedTypeSymbol.Name, sourceNamedTypeSymbol.Arity);
                    if (dictionary.TryGetValue(key, out var value))
                    {
                        if (Locations.Length == 1 || IsPartial)
                        {
                            if (sourceNamedTypeSymbol.IsPartial && value.IsPartial)
                            {
                                diagnostics.Add(ErrorCode.ERR_PartialTypeKindConflict, sourceNamedTypeSymbol.Locations[0], sourceNamedTypeSymbol);
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, sourceNamedTypeSymbol.Locations[0], this, sourceNamedTypeSymbol.Name);
                            }
                        }
                    }
                    else
                    {
                        dictionary.Add(key, sourceNamedTypeSymbol);
                    }
                    instance.Add(sourceNamedTypeSymbol);
                }
                if (IsInterface)
                {
                    ArrayBuilder<NamedTypeSymbol>.Enumerator enumerator2 = instance.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        NamedTypeSymbol current2 = enumerator2.Current;
                        Binder.CheckFeatureAvailability(current2.DeclaringSyntaxReferences[0].GetSyntax(), MessageID.IDS_DefaultInterfaceImplementation, diagnostics, current2.Locations[0]);
                    }
                }
                return (instance.Count > 0) ? instance.ToDictionary((NamedTypeSymbol s) => s.Name, StringOrdinalComparer.Instance) : s_emptyTypeMembers;
            }
            finally
            {
                instance.Free();
            }
        }

        private void CheckMemberNameDistinctFromType(Symbol member, BindingDiagnosticBag diagnostics)
        {
            TypeKind typeKind = TypeKind;
            if (typeKind != TypeKind.Class)
            {
                if (typeKind != TypeKind.Interface)
                {
                    if (typeKind != TypeKind.Struct)
                    {
                        return;
                    }
                }
                else if (!member.IsStatic)
                {
                    return;
                }
            }
            if (member.Name == Name)
            {
                diagnostics.Add(ErrorCode.ERR_MemberNameSameAsType, member.Locations[0], Name);
            }
        }

        internal override ImmutableArray<Symbol> GetMembersUnordered()
        {
            ImmutableArray<Symbol> lazyMembersFlattened = _lazyMembersFlattened;
            if (lazyMembersFlattened.IsDefault)
            {
                lazyMembersFlattened = GetMembersByName().Flatten();
                ImmutableInterlocked.InterlockedInitialize(ref _lazyMembersFlattened, lazyMembersFlattened);
                lazyMembersFlattened = _lazyMembersFlattened;
            }
            return lazyMembersFlattened.ConditionallyDeOrder();
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            if (_flags.FlattenedMembersIsSorted)
            {
                return _lazyMembersFlattened;
            }
            ImmutableArray<Symbol> immutableArray = GetMembersUnordered();
            if (immutableArray.Length > 1)
            {
                immutableArray = immutableArray.Sort(LexicalOrderSymbolComparer.Instance);
                ImmutableInterlocked.InterlockedExchange(ref _lazyMembersFlattened, immutableArray);
            }
            _flags.SetFlattenedMembersIsSorted();
            return immutableArray;
        }

        public sealed override ImmutableArray<Symbol> GetMembers(string name)
        {
            if (GetMembersByName().TryGetValue(name, out var value))
            {
                return value;
            }
            return ImmutableArray<Symbol>.Empty;
        }

        internal override bool HasPossibleWellKnownCloneMethod()
        {
            return IsRecord;
        }

        internal override ImmutableArray<Symbol> GetSimpleNonTypeMembers(string name)
        {
            if (_lazyMembersDictionary == null && !declaration.MemberNames.Contains(name))
            {
                DeclarationKind kind = declaration.Kind;
                if (kind != DeclarationKind.Record && kind != DeclarationKind.RecordStruct)
                {
                    return ImmutableArray<Symbol>.Empty;
                }
            }
            return GetMembers(name);
        }

        internal override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            if (TypeKind == TypeKind.Enum)
            {
                yield return ((SourceNamedTypeSymbol)this).EnumValueField;
            }
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                switch (current.Kind)
                {
                    case SymbolKind.Field:
                        if (!(current is TupleErrorFieldSymbol))
                        {
                            yield return (FieldSymbol)current;
                        }
                        break;
                    case SymbolKind.Event:
                        {
                            FieldSymbol associatedField = ((EventSymbol)current).AssociatedField;
                            if ((object)associatedField != null)
                            {
                                yield return associatedField;
                            }
                            break;
                        }
                }
            }
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
        {
            return GetEarlyAttributeDecodingMembersDictionary().Flatten();
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
        {
            if (!GetEarlyAttributeDecodingMembersDictionary().TryGetValue(name, out var value))
            {
                return ImmutableArray<Symbol>.Empty;
            }
            return value;
        }

        private Dictionary<string, ImmutableArray<Symbol>> GetEarlyAttributeDecodingMembersDictionary()
        {
            if (_lazyEarlyAttributeDecodingMembersDictionary == null)
            {
                Dictionary<string, ImmutableArray<Symbol>> dictionary = Volatile.Read(ref _lazyMembersDictionary);
                if (dictionary != null)
                {
                    return dictionary;
                }
                MembersAndInitializers membersAndInitializers = GetMembersAndInitializers();
                Dictionary<string, ImmutableArray<Symbol>> dictionary2 = (membersAndInitializers.HaveIndexers ? membersAndInitializers.NonTypeMembers.WhereAsArray(delegate (Symbol s)
                {
                    if (!s.IsIndexer())
                    {
                        if (s.IsAccessor())
                        {
                            Symbol associatedSymbol = ((MethodSymbol)s).AssociatedSymbol;
                            if ((object)associatedSymbol == null)
                            {
                                return true;
                            }
                            return !associatedSymbol.IsIndexer();
                        }
                        return true;
                    }
                    return false;
                }).ToDictionary((Symbol s) => s.Name) : membersAndInitializers.NonTypeMembers.ToDictionary((Symbol s) => s.Name));
                AddNestedTypesToDictionary(dictionary2, GetTypeMembersDictionary());
                Interlocked.CompareExchange(ref _lazyEarlyAttributeDecodingMembersDictionary, dictionary2, null);
            }
            return _lazyEarlyAttributeDecodingMembersDictionary;
        }

        protected MembersAndInitializers GetMembersAndInitializers()
        {
            MembersAndInitializers lazyMembersAndInitializers = _lazyMembersAndInitializers;
            if (lazyMembersAndInitializers != null)
            {
                return lazyMembersAndInitializers;
            }
            BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
            lazyMembersAndInitializers = BuildMembersAndInitializers(instance);
            MembersAndInitializers membersAndInitializers = Interlocked.CompareExchange(ref _lazyMembersAndInitializers, lazyMembersAndInitializers, null);
            if (membersAndInitializers != null)
            {
                instance.Free();
                return membersAndInitializers;
            }
            AddDeclarationDiagnostics(instance);
            instance.Free();
            _lazyDeclaredMembersAndInitializers = null;
            return lazyMembersAndInitializers;
        }

        [Conditional("DEBUG")]
        internal void AssertMemberExposure(Symbol member, bool forDiagnostics = false)
        {
            Symbol member2 = member;
            if ((member2 is FieldSymbol && forDiagnostics && IsTupleType) || member2 is NamedTypeSymbol || member2 is TypeParameterSymbol || member2 is SynthesizedMethodBaseSymbol)
            {
                return;
            }
            if (member2 is FieldSymbol fieldSymbol && fieldSymbol.AssociatedSymbol is EventSymbol eventSymbol)
            {
                member2 = eventSymbol;
            }
            DeclaredMembersAndInitializers declaredMembersAndInitializers = Volatile.Read(ref _lazyDeclaredMembersAndInitializers);
            if (declaredMembersAndInitializers == null || (!declaredMembersAndInitializers.NonTypeMembers.Contains((Symbol m) => (object)m == member2) && (object)declaredMembersAndInitializers.RecordPrimaryConstructor != member2))
            {
                MembersAndInitializers? membersAndInitializers = Volatile.Read(ref _lazyMembersAndInitializers);
                if (membersAndInitializers != null)
                {
                    membersAndInitializers!.NonTypeMembers.Contains((Symbol m) => (object)m == member2);
                }
                else
                    _ = 0;
            }
        }

        protected Dictionary<string, ImmutableArray<Symbol>> GetMembersByName()
        {
            if (state.HasComplete(CompletionPart.Members))
            {
                return _lazyMembersDictionary;
            }
            return GetMembersByNameSlow();
        }

        private Dictionary<string, ImmutableArray<Symbol>> GetMembersByNameSlow()
        {
            if (_lazyMembersDictionary == null)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                Dictionary<string, ImmutableArray<Symbol>> value = MakeAllMembers(instance);
                if (Interlocked.CompareExchange(ref _lazyMembersDictionary, value, null) == null)
                {
                    AddDeclarationDiagnostics(instance);
                    state.NotePartComplete(CompletionPart.Members);
                }
                instance.Free();
            }
            state.SpinWaitComplete(CompletionPart.Members, default(CancellationToken));
            return _lazyMembersDictionary;
        }

        internal override IEnumerable<Symbol> GetInstanceFieldsAndEvents()
        {
            return GetMembersAndInitializers().NonTypeMembers.Where(NamedTypeSymbol.IsInstanceFieldOrEvent);
        }

        protected void AfterMembersChecks(BindingDiagnosticBag diagnostics)
        {
            if (IsInterface)
            {
                CheckInterfaceMembers(GetMembersAndInitializers().NonTypeMembers, diagnostics);
            }
            CheckMemberNamesDistinctFromType(diagnostics);
            CheckMemberNameConflicts(diagnostics);
            CheckRecordMemberNames(diagnostics);
            CheckSpecialMemberErrors(diagnostics);
            CheckTypeParameterNameConflicts(diagnostics);
            CheckAccessorNameConflicts(diagnostics);
            _ = KnownCircularStruct;
            CheckSequentialOnPartialType(diagnostics);
            CheckForProtectedInStaticClass(diagnostics);
            CheckForUnmatchedOperators(diagnostics);
            Location location = Locations[0];
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (IsRefLikeType)
            {
                declaringCompilation.EnsureIsByRefLikeAttributeExists(diagnostics, location, modifyCompilation: true);
            }
            if (IsReadOnly)
            {
                declaringCompilation.EnsureIsReadOnlyAttributeExists(diagnostics, location, modifyCompilation: true);
            }
            NamedTypeSymbol baseType = BaseTypeNoUseSiteDiagnostics;
            ImmutableArray<NamedTypeSymbol> interfaces = GetInterfacesToEmit();
            if (hasBaseTypeOrInterface((NamedTypeSymbol t) => t.ContainsNativeInteger()))
            {
                declaringCompilation.EnsureNativeIntegerAttributeExists(diagnostics, location, modifyCompilation: true);
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                if (ShouldEmitNullableContextValue(out var _))
                {
                    declaringCompilation.EnsureNullableContextAttributeExists(diagnostics, location, modifyCompilation: true);
                }
                if (hasBaseTypeOrInterface((NamedTypeSymbol t) => t.NeedsNullableAttribute()))
                {
                    declaringCompilation.EnsureNullableAttributeExists(diagnostics, location, modifyCompilation: true);
                }
            }
            if (interfaces.Any((NamedTypeSymbol t) => needsTupleElementNamesAttribute(t)))
            {
                Binder.ReportMissingTupleElementNamesAttributesIfNeeded(declaringCompilation, location, diagnostics);
            }
            bool hasBaseTypeOrInterface(Func<NamedTypeSymbol, bool> predicate)
            {
                if ((object)baseType == null || !predicate(baseType))
                {
                    return interfaces.Any(predicate);
                }
                return true;
            }
            static bool needsTupleElementNamesAttribute(TypeSymbol type)
            {
                if ((object)type == null)
                {
                    return false;
                }
                return (object)type.VisitType((TypeSymbol t, object a, bool b) => !t.TupleElementNames.IsDefaultOrEmpty && !t.IsErrorType(), null) != null;
            }
        }

        private void CheckMemberNamesDistinctFromType(BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembersAndInitializers().NonTypeMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                CheckMemberNameDistinctFromType(current, diagnostics);
            }
        }

        private void CheckRecordMemberNames(BindingDiagnosticBag diagnostics)
        {
            if (declaration.Kind == DeclarationKind.Record || declaration.Kind == DeclarationKind.RecordStruct)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = GetMembers("Clone").GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    diagnostics.Add(ErrorCode.ERR_CloneDisallowedInRecord, current.Locations[0]);
                }
            }
        }

        private void CheckMemberNameConflicts(BindingDiagnosticBag diagnostics)
        {
            Dictionary<string, ImmutableArray<Symbol>> membersByName = GetMembersByName();
            CheckIndexerNameConflicts(diagnostics, membersByName);
            Dictionary<SourceMemberMethodSymbol, SourceMemberMethodSymbol> dictionary = new Dictionary<SourceMemberMethodSymbol, SourceMemberMethodSymbol>(MemberSignatureComparer.DuplicateSourceComparer);
            Dictionary<SourceMemberMethodSymbol, SourceMemberMethodSymbol> dictionary2 = new Dictionary<SourceMemberMethodSymbol, SourceMemberMethodSymbol>(MemberSignatureComparer.DuplicateSourceComparer);
            HashSet<SourceUserDefinedConversionSymbol> hashSet = new HashSet<SourceUserDefinedConversionSymbol>(ConversionSignatureComparer.Comparer);
            foreach (KeyValuePair<string, ImmutableArray<Symbol>> item in membersByName)
            {
                string key = item.Key;
                Symbol symbol = GetTypeMembers(key).FirstOrDefault();
                dictionary.Clear();
                ImmutableArray<Symbol>.Enumerator enumerator2 = item.Value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    if (current2.Kind == SymbolKind.NamedType || current2.IsAccessor() || current2.IsIndexer())
                    {
                        continue;
                    }
                    if ((object)symbol != null)
                    {
                        if (current2.Kind != SymbolKind.Method || symbol.Kind != SymbolKind.Method)
                        {
                            if ((current2.Kind != SymbolKind.Field || !current2.IsImplicitlyDeclared) && (Locations.Length == 1 || IsPartial))
                            {
                                diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, current2.Locations[0], this, current2.Name);
                            }
                            if (symbol.Kind == SymbolKind.Method)
                            {
                                symbol = current2;
                            }
                        }
                    }
                    else
                    {
                        symbol = current2;
                    }
                    SourceUserDefinedConversionSymbol sourceUserDefinedConversionSymbol = current2 as SourceUserDefinedConversionSymbol;
                    SourceMemberMethodSymbol sourceMemberMethodSymbol = current2 as SourceMemberMethodSymbol;
                    if ((object)sourceUserDefinedConversionSymbol != null)
                    {
                        if (!hashSet.Add(sourceUserDefinedConversionSymbol))
                        {
                            diagnostics.Add(ErrorCode.ERR_DuplicateConversionInClass, sourceUserDefinedConversionSymbol.Locations[0], this);
                        }
                        else if (!dictionary2.ContainsKey(sourceUserDefinedConversionSymbol))
                        {
                            dictionary2.Add(sourceUserDefinedConversionSymbol, sourceUserDefinedConversionSymbol);
                        }
                        if (dictionary.TryGetValue(sourceUserDefinedConversionSymbol, out var value))
                        {
                            ReportMethodSignatureCollision(diagnostics, sourceUserDefinedConversionSymbol, value);
                        }
                    }
                    else if ((object)sourceMemberMethodSymbol != null)
                    {
                        if (dictionary2.TryGetValue(sourceMemberMethodSymbol, out var value2))
                        {
                            ReportMethodSignatureCollision(diagnostics, sourceMemberMethodSymbol, value2);
                        }
                        if (dictionary.TryGetValue(sourceMemberMethodSymbol, out var value3))
                        {
                            ReportMethodSignatureCollision(diagnostics, sourceMemberMethodSymbol, value3);
                        }
                        else
                        {
                            dictionary.Add(sourceMemberMethodSymbol, sourceMemberMethodSymbol);
                        }
                    }
                }
            }
        }

        // Report a name conflict; the error is reported on the location of method1.
        // UNDONE: Consider adding a secondary location pointing to the second method.
        private void ReportMethodSignatureCollision(BindingDiagnosticBag diagnostics, SourceMemberMethodSymbol method1, SourceMemberMethodSymbol method2)
        {
            switch (method1, method2)
            {
                case (SourceOrdinaryMethodSymbol { IsPartialDefinition: true }, SourceOrdinaryMethodSymbol { IsPartialImplementation: true }):
                case (SourceOrdinaryMethodSymbol { IsPartialImplementation: true }, SourceOrdinaryMethodSymbol { IsPartialDefinition: true }):
                    // these could be 2 parts of the same partial method.
                    // Partial methods are allowed to collide by signature.
                    return;
                case (SynthesizedSimpleProgramEntryPointSymbol { }, SynthesizedSimpleProgramEntryPointSymbol { }):
                    return;
            }

            // If method1 is a constructor only because its return type is missing, then
            // we've already produced a diagnostic for the missing return type and we suppress the
            // diagnostic about duplicate signature.
            if (method1.MethodKind == MethodKind.Constructor &&
                ((ConstructorDeclarationSyntax)method1.SyntaxRef.GetSyntax()).Identifier.ValueText != this.Name)
            {
                return;
            }

            for (int i = 0; i < method1.ParameterCount; i++)
            {
                var refKind1 = method1.Parameters[i].RefKind;
                var refKind2 = method2.Parameters[i].RefKind;

                if (refKind1 != refKind2)
                {
                    // '{0}' cannot define an overloaded {1} that differs only on parameter modifiers '{2}' and '{3}'
                    var methodKind = method1.MethodKind == MethodKind.Constructor ? MessageID.IDS_SK_CONSTRUCTOR : MessageID.IDS_SK_METHOD;
                    diagnostics.Add(ErrorCode.ERR_OverloadRefKind, method1.Locations[0], this, methodKind.Localize(), refKind1.ToParameterDisplayString(), refKind2.ToParameterDisplayString());

                    return;
                }
            }

            // Special case: if there are two destructors, use the destructor syntax instead of "Finalize"
            var methodName = (method1.MethodKind == MethodKind.Destructor && method2.MethodKind == MethodKind.Destructor) ?
                "~" + this.Name :
                (method1.IsConstructor() ? this.Name : method1.Name);

            // Type '{1}' already defines a member called '{0}' with the same parameter types
            diagnostics.Add(ErrorCode.ERR_MemberAlreadyExists, method1.Locations[0], methodName, this);
        }

        private void CheckIndexerNameConflicts(BindingDiagnosticBag diagnostics, Dictionary<string, ImmutableArray<Symbol>> membersByName)
        {
            PooledHashSet<string> pooledHashSet = null;
            if (Arity > 0)
            {
                pooledHashSet = PooledHashSet<string>.GetInstance();
                ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = TypeParameters.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    TypeParameterSymbol current = enumerator.Current;
                    pooledHashSet.Add(current.Name);
                }
            }
            Dictionary<PropertySymbol, PropertySymbol> dictionary = new Dictionary<PropertySymbol, PropertySymbol>(MemberSignatureComparer.DuplicateSourceComparer);
            foreach (ImmutableArray<Symbol> value in membersByName.Values)
            {
                string lastIndexerName = null;
                dictionary.Clear();
                ImmutableArray<Symbol>.Enumerator enumerator3 = value.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    Symbol current3 = enumerator3.Current;
                    if (!current3.IsIndexer())
                    {
                        continue;
                    }
                    PropertySymbol propertySymbol = (PropertySymbol)current3;
                    CheckIndexerSignatureCollisions(propertySymbol, diagnostics, membersByName, dictionary, ref lastIndexerName);
                    if (pooledHashSet != null)
                    {
                        string metadataName = propertySymbol.MetadataName;
                        if (pooledHashSet.Contains(metadataName))
                        {
                            diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, propertySymbol.Locations[0], this, metadataName);
                        }
                    }
                }
            }
            pooledHashSet?.Free();
        }

        private void CheckIndexerSignatureCollisions(PropertySymbol indexer, BindingDiagnosticBag diagnostics, Dictionary<string, ImmutableArray<Symbol>> membersByName, Dictionary<PropertySymbol, PropertySymbol> indexersBySignature, ref string? lastIndexerName)
        {
            if (!indexer.IsExplicitInterfaceImplementation)
            {
                string metadataName = indexer.MetadataName;
                if (lastIndexerName != null && lastIndexerName != metadataName)
                {
                    diagnostics.Add(ErrorCode.ERR_InconsistentIndexerNames, indexer.Locations[0]);
                }
                lastIndexerName = metadataName;
                if ((Locations.Length == 1 || IsPartial) && membersByName.ContainsKey(metadataName))
                {
                    diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, indexer.Locations[0], this, metadataName);
                }
            }
            if (indexersBySignature.TryGetValue(indexer, out var _))
            {
                diagnostics.Add(ErrorCode.ERR_MemberAlreadyExists, indexer.Locations[0], SyntaxFacts.GetText(SyntaxKind.ThisKeyword), this);
            }
            else
            {
                indexersBySignature[indexer] = indexer;
            }
        }

        private void CheckSpecialMemberErrors(BindingDiagnosticBag diagnostics)
        {
            TypeConversions conversions = new TypeConversions(ContainingAssembly.CorLibrary);
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.AfterAddingTypeMembersChecks(conversions, diagnostics);
            }
        }

        private void CheckTypeParameterNameConflicts(BindingDiagnosticBag diagnostics)
        {
            if (TypeKind == TypeKind.Delegate || (Locations.Length != 1 && !IsPartial))
            {
                return;
            }
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = TypeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TypeParameterSymbol current = enumerator.Current;
                ImmutableArray<Symbol>.Enumerator enumerator2 = GetMembers(current.Name).GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, current2.Locations[0], this, current.Name);
                }
            }
        }

        private void CheckAccessorNameConflicts(BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (!current.IsExplicitInterfaceImplementation())
                {
                    switch (current.Kind)
                    {
                        case SymbolKind.Property:
                            {
                                PropertySymbol propertySymbol = (PropertySymbol)current;
                                CheckForMemberConflictWithPropertyAccessor(propertySymbol, getNotSet: true, diagnostics);
                                CheckForMemberConflictWithPropertyAccessor(propertySymbol, getNotSet: false, diagnostics);
                                break;
                            }
                        case SymbolKind.Event:
                            {
                                EventSymbol eventSymbol = (EventSymbol)current;
                                CheckForMemberConflictWithEventAccessor(eventSymbol, isAdder: true, diagnostics);
                                CheckForMemberConflictWithEventAccessor(eventSymbol, isAdder: false, diagnostics);
                                break;
                            }
                    }
                }
            }
        }

        private bool CheckStructCircularity(BindingDiagnosticBag diagnostics)
        {
            CheckFiniteFlatteningGraph(diagnostics);
            return HasStructCircularity(diagnostics);
        }

        private bool HasStructCircularity(BindingDiagnosticBag diagnostics)
        {
            foreach (ImmutableArray<Symbol> value in GetMembersByName().Values)
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    if (current.Kind != SymbolKind.Field)
                    {
                        continue;
                    }
                    FieldSymbol fieldSymbol = (FieldSymbol)current;
                    if (fieldSymbol.IsStatic)
                    {
                        continue;
                    }
                    TypeSymbol typeSymbol = fieldSymbol.NonPointerType();
                    if ((object)typeSymbol != null && typeSymbol.TypeKind == TypeKind.Struct && BaseTypeAnalysis.StructDependsOn((NamedTypeSymbol)typeSymbol, this) && !typeSymbol.IsPrimitiveRecursiveStruct())
                    {
                        Symbol symbol = fieldSymbol.AssociatedSymbol ?? fieldSymbol;
                        if (symbol.Kind == SymbolKind.Parameter)
                        {
                            symbol = fieldSymbol;
                        }
                        diagnostics.Add(ErrorCode.ERR_StructLayoutCycle, symbol.Locations[0], symbol, typeSymbol);
                        return true;
                    }
                }
            }
            return false;
        }

        private void CheckForProtectedInStaticClass(BindingDiagnosticBag diagnostics)
        {
            if (!IsStatic)
            {
                return;
            }
            foreach (ImmutableArray<Symbol> value in GetMembersByName().Values)
            {
                ImmutableArray<Symbol>.Enumerator enumerator2 = value.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current = enumerator2.Current;
                    if (!(current is TypeSymbol) && current.DeclaredAccessibility.HasProtected() && (current.Kind != SymbolKind.Method || ((MethodSymbol)current).MethodKind != MethodKind.Destructor))
                    {
                        diagnostics.Add(ErrorCode.ERR_ProtectedInStatic, current.Locations[0], current);
                    }
                }
            }
        }

        private void CheckForUnmatchedOperators(BindingDiagnosticBag diagnostics)
        {
            CheckForUnmatchedOperator(diagnostics, "op_True", "op_False");
            CheckForUnmatchedOperator(diagnostics, "op_Equality", "op_Inequality");
            CheckForUnmatchedOperator(diagnostics, "op_LessThan", "op_GreaterThan");
            CheckForUnmatchedOperator(diagnostics, "op_LessThanOrEqual", "op_GreaterThanOrEqual");
            CheckForEqualityAndGetHashCode(diagnostics);
        }

        private void CheckForUnmatchedOperator(BindingDiagnosticBag diagnostics, string operatorName1, string operatorName2)
        {
            ImmutableArray<MethodSymbol> operators = GetOperators(operatorName1);
            ImmutableArray<MethodSymbol> operators2 = GetOperators(operatorName2);
            CheckForUnmatchedOperator(diagnostics, operators, operators2, operatorName2);
            CheckForUnmatchedOperator(diagnostics, operators2, operators, operatorName1);
        }

        private static void CheckForUnmatchedOperator(BindingDiagnosticBag diagnostics, ImmutableArray<MethodSymbol> ops1, ImmutableArray<MethodSymbol> ops2, string operatorName2)
        {
            ImmutableArray<MethodSymbol>.Enumerator enumerator = ops1.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MethodSymbol current = enumerator.Current;
                bool flag = false;
                ImmutableArray<MethodSymbol>.Enumerator enumerator2 = ops2.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    MethodSymbol current2 = enumerator2.Current;
                    flag = DoOperatorsPair(current, current2);
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    diagnostics.Add(ErrorCode.ERR_OperatorNeedsMatch, current.Locations[0], current, SyntaxFacts.GetText(SyntaxFacts.GetOperatorKind(operatorName2)));
                }
            }
        }

        private static bool DoOperatorsPair(MethodSymbol op1, MethodSymbol op2)
        {
            if (op1.ParameterCount != op2.ParameterCount)
            {
                return false;
            }
            for (int i = 0; i < op1.ParameterCount; i++)
            {
                if (!op1.ParameterTypesWithAnnotations[i].Equals(op2.ParameterTypesWithAnnotations[i], TypeCompareKind.AllIgnoreOptions))
                {
                    return false;
                }
            }
            if (!op1.ReturnType.Equals(op2.ReturnType, TypeCompareKind.AllIgnoreOptions))
            {
                return false;
            }
            return true;
        }

        private void CheckForEqualityAndGetHashCode(BindingDiagnosticBag diagnostics)
        {
            if (this.IsInterfaceType() || IsRecord || IsRecordStruct)
            {
                return;
            }
            bool flag = GetOperators("op_Equality").Any() || GetOperators("op_Inequality").Any();
            bool flag2 = TypeOverridesObjectMethod("Equals");
            if (flag || flag2)
            {
                bool flag3 = TypeOverridesObjectMethod("GetHashCode");
                if (flag2 && !flag3)
                {
                    diagnostics.Add(ErrorCode.WRN_EqualsWithoutGetHashCode, Locations[0], this);
                }
                if (flag && !flag2)
                {
                    diagnostics.Add(ErrorCode.WRN_EqualityOpWithoutEquals, Locations[0], this);
                }
                if (flag && !flag3)
                {
                    diagnostics.Add(ErrorCode.WRN_EqualityOpWithoutGetHashCode, Locations[0], this);
                }
            }
        }

        private bool TypeOverridesObjectMethod(string name)
        {
            foreach (MethodSymbol item in GetMembers(name).OfType<MethodSymbol>())
            {
                if (item.IsOverride && item.GetConstructedLeastOverriddenMethod(this, requireSameReturnType: false).ContainingType.SpecialType == SpecialType.System_Object)
                {
                    return true;
                }
            }
            return false;
        }

        private void CheckFiniteFlatteningGraph(BindingDiagnosticBag diagnostics)
        {
            if (AllTypeArgumentCount() == 0)
            {
                return;
            }
            Dictionary<NamedTypeSymbol, NamedTypeSymbol> dictionary = new Dictionary<NamedTypeSymbol, NamedTypeSymbol>(ReferenceEqualityComparer.Instance);
            dictionary.Add(this, this);
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembersUnordered().GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is FieldSymbol fieldSymbol && fieldSymbol.IsStatic && fieldSymbol.Type.TypeKind == TypeKind.Struct)
                {
                    NamedTypeSymbol namedTypeSymbol = (NamedTypeSymbol)fieldSymbol.Type;
                    if (InfiniteFlatteningGraph(this, namedTypeSymbol, dictionary))
                    {
                        diagnostics.Add(ErrorCode.ERR_StructLayoutCycle, fieldSymbol.Locations[0], fieldSymbol, namedTypeSymbol);
                        break;
                    }
                }
            }
        }

        private static bool InfiniteFlatteningGraph(SourceMemberContainerTypeSymbol top, NamedTypeSymbol t, Dictionary<NamedTypeSymbol, NamedTypeSymbol> instanceMap)
        {
            if (!t.ContainsTypeParameter())
            {
                return false;
            }
            NamedTypeSymbol originalDefinition = t.OriginalDefinition;
            if (instanceMap.TryGetValue(originalDefinition, out var value))
            {
                if (!TypeSymbol.Equals(value, t, TypeCompareKind.AllNullableIgnoreOptions))
                {
                    return (object)originalDefinition == top;
                }
                return false;
            }
            instanceMap.Add(originalDefinition, t);
            try
            {
                ImmutableArray<Symbol>.Enumerator enumerator = t.GetMembersUnordered().GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current is FieldSymbol fieldSymbol && fieldSymbol.IsStatic && fieldSymbol.Type.TypeKind == TypeKind.Struct)
                    {
                        NamedTypeSymbol t2 = (NamedTypeSymbol)fieldSymbol.Type;
                        if (InfiniteFlatteningGraph(top, t2, instanceMap))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            finally
            {
                instanceMap.Remove(originalDefinition);
            }
        }

        private void CheckSequentialOnPartialType(BindingDiagnosticBag diagnostics)
        {
            if (!IsPartial || Layout.Kind != 0)
            {
                return;
            }
            SyntaxReference syntaxReference = null;
            if (SyntaxReferences.Length <= 1)
            {
                return;
            }
            ImmutableArray<SyntaxReference>.Enumerator enumerator = SyntaxReferences.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SyntaxReference current = enumerator.Current;
                if (!(current.GetSyntax() is TypeDeclarationSyntax typeDeclarationSyntax))
                {
                    continue;
                }
                SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator2 = typeDeclarationSyntax.Members.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    if (HasInstanceData(enumerator2.Current))
                    {
                        if (syntaxReference != null && syntaxReference != current)
                        {
                            diagnostics.Add(ErrorCode.WRN_SequentialOnPartialClass, Locations[0], this);
                            return;
                        }
                        syntaxReference = current;
                    }
                }
            }
        }

        private static bool HasInstanceData(MemberDeclarationSyntax m)
        {
            switch (m.Kind())
            {
                case SyntaxKind.FieldDeclaration:
                    {
                        FieldDeclarationSyntax fieldDeclarationSyntax = (FieldDeclarationSyntax)m;
                        if (!ContainsModifier(fieldDeclarationSyntax.Modifiers, SyntaxKind.StaticKeyword))
                        {
                            return !ContainsModifier(fieldDeclarationSyntax.Modifiers, SyntaxKind.ConstKeyword);
                        }
                        return false;
                    }
                case SyntaxKind.PropertyDeclaration:
                    {
                        PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)m;
                        if (!ContainsModifier(propertyDeclarationSyntax.Modifiers, SyntaxKind.StaticKeyword) && !ContainsModifier(propertyDeclarationSyntax.Modifiers, SyntaxKind.AbstractKeyword) && !ContainsModifier(propertyDeclarationSyntax.Modifiers, SyntaxKind.ExternKeyword) && propertyDeclarationSyntax.AccessorList != null)
                        {
                            return All(propertyDeclarationSyntax.AccessorList!.Accessors, (AccessorDeclarationSyntax a) => a.Body == null && a.ExpressionBody == null);
                        }
                        return false;
                    }
                case SyntaxKind.EventFieldDeclaration:
                    {
                        EventFieldDeclarationSyntax eventFieldDeclarationSyntax = (EventFieldDeclarationSyntax)m;
                        if (!ContainsModifier(eventFieldDeclarationSyntax.Modifiers, SyntaxKind.StaticKeyword) && !ContainsModifier(eventFieldDeclarationSyntax.Modifiers, SyntaxKind.AbstractKeyword))
                        {
                            return !ContainsModifier(eventFieldDeclarationSyntax.Modifiers, SyntaxKind.ExternKeyword);
                        }
                        return false;
                    }
                default:
                    return false;
            }
        }

        private static bool All<T>(SyntaxList<T> list, Func<T, bool> predicate) where T : CSharpSyntaxNode
        {
            SyntaxList<T>.Enumerator enumerator = list.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T current = enumerator.Current;
                if (predicate(current))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool ContainsModifier(SyntaxTokenList modifiers, SyntaxKind modifier)
        {
            SyntaxTokenList.Enumerator enumerator = modifiers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.IsKind(modifier))
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<string, ImmutableArray<Symbol>> MakeAllMembers(BindingDiagnosticBag diagnostics)
        {
            MembersAndInitializers membersAndInitializers = GetMembersAndInitializers();
            Dictionary<string, ImmutableArray<Symbol>> membersByName;
            if (!membersAndInitializers.HaveIndexers && !IsTupleType && _lazyEarlyAttributeDecodingMembersDictionary != null)
            {
                membersByName = _lazyEarlyAttributeDecodingMembersDictionary;
            }
            else
            {
                membersByName = membersAndInitializers.NonTypeMembers.ToDictionary((Symbol s) => s.Name, StringOrdinalComparer.Instance);
                AddNestedTypesToDictionary(membersByName, GetTypeMembersDictionary());
            }
            MergePartialMembers(ref membersByName, diagnostics);
            return membersByName;
        }

        private static void AddNestedTypesToDictionary(Dictionary<string, ImmutableArray<Symbol>> membersByName, Dictionary<string, ImmutableArray<NamedTypeSymbol>> typesByName)
        {
            foreach (KeyValuePair<string, ImmutableArray<NamedTypeSymbol>> item in typesByName)
            {
                string key = item.Key;
                ImmutableArray<Symbol> immutableArray = StaticCast<Symbol>.From(item.Value);
                if (membersByName.TryGetValue(key, out var value))
                {
                    membersByName[key] = value.Concat(immutableArray);
                }
                else
                {
                    membersByName.Add(key, immutableArray);
                }
            }
        }

        protected virtual MembersAndInitializers? BuildMembersAndInitializers(BindingDiagnosticBag diagnostics)
        {
            DeclaredMembersAndInitializers declaredMembersAndInitializers = getDeclaredMembersAndInitializers();
            if (declaredMembersAndInitializers == null)
            {
                return null;
            }
            MembersAndInitializersBuilder membersAndInitializersBuilder = new MembersAndInitializersBuilder(declaredMembersAndInitializers);
            AddSynthesizedMembers(membersAndInitializersBuilder, declaredMembersAndInitializers, diagnostics);
            if (Volatile.Read(ref _lazyMembersAndInitializers) != null)
            {
                membersAndInitializersBuilder.Free();
                return null;
            }
            return membersAndInitializersBuilder.ToReadOnlyAndFree(declaredMembersAndInitializers);
            DeclaredMembersAndInitializers? buildDeclaredMembersAndInitializers(BindingDiagnosticBag diagnostics)
            {
                DeclaredMembersAndInitializersBuilder declaredMembersAndInitializersBuilder = new DeclaredMembersAndInitializersBuilder();
                AddDeclaredNontypeMembers(declaredMembersAndInitializersBuilder, diagnostics);
                switch (TypeKind)
                {
                    case TypeKind.Struct:
                        CheckForStructBadInitializers(declaredMembersAndInitializersBuilder, diagnostics);
                        CheckForStructDefaultConstructors(declaredMembersAndInitializersBuilder.NonTypeMembers, isEnum: false, diagnostics);
                        break;
                    case TypeKind.Enum:
                        CheckForStructDefaultConstructors(declaredMembersAndInitializersBuilder.NonTypeMembers, isEnum: true, diagnostics);
                        break;
                }
                if (IsTupleType)
                {
                    declaredMembersAndInitializersBuilder.AddOrWrapTupleMembers(this);
                }
                if (Volatile.Read(ref _lazyDeclaredMembersAndInitializers) != DeclaredMembersAndInitializers.UninitializedSentinel)
                {
                    declaredMembersAndInitializersBuilder.Free();
                    return null;
                }
                return declaredMembersAndInitializersBuilder.ToReadOnlyAndFree(DeclaringCompilation);
            }
            DeclaredMembersAndInitializers? getDeclaredMembersAndInitializers()
            {
                DeclaredMembersAndInitializers lazyDeclaredMembersAndInitializers = _lazyDeclaredMembersAndInitializers;
                if (lazyDeclaredMembersAndInitializers != DeclaredMembersAndInitializers.UninitializedSentinel)
                {
                    return lazyDeclaredMembersAndInitializers;
                }
                if (Volatile.Read(ref _lazyMembersAndInitializers) != null)
                {
                    return null;
                }
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                lazyDeclaredMembersAndInitializers = buildDeclaredMembersAndInitializers(instance);
                DeclaredMembersAndInitializers declaredMembersAndInitializers2 = Interlocked.CompareExchange(ref _lazyDeclaredMembersAndInitializers, lazyDeclaredMembersAndInitializers, DeclaredMembersAndInitializers.UninitializedSentinel);
                if (declaredMembersAndInitializers2 != DeclaredMembersAndInitializers.UninitializedSentinel)
                {
                    instance.Free();
                    return declaredMembersAndInitializers2;
                }
                AddDeclarationDiagnostics(instance);
                instance.Free();
                return lazyDeclaredMembersAndInitializers;
            }
        }

        private void AddSynthesizedMembers(MembersAndInitializersBuilder builder, DeclaredMembersAndInitializers declaredMembersAndInitializers, BindingDiagnosticBag diagnostics)
        {
            switch (TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Struct:
                case TypeKind.Submission:
                    AddSynthesizedRecordMembersIfNecessary(builder, declaredMembersAndInitializers, diagnostics);
                    AddSynthesizedConstructorsIfNecessary(builder, declaredMembersAndInitializers, diagnostics);
                    break;
            }
        }

        private void AddDeclaredNontypeMembers(DeclaredMembersAndInitializersBuilder builder, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<SingleTypeDeclaration>.Enumerator enumerator = declaration.Declarations.GetEnumerator();
            while (enumerator.MoveNext())
            {
                SingleTypeDeclaration current = enumerator.Current;
                if (!current.HasAnyNontypeMembers)
                {
                    continue;
                }
                if (_lazyMembersAndInitializers != null)
                {
                    break;
                }
                SyntaxNode syntax2 = current.SyntaxReference.GetSyntax();
                switch (syntax2.Kind())
                {
                    case SyntaxKind.EnumDeclaration:
                        AddEnumMembers(builder, (EnumDeclarationSyntax)syntax2, diagnostics);
                        break;
                    case SyntaxKind.DelegateDeclaration:
                        SourceDelegateMethodSymbol.AddDelegateMembers(this, builder.NonTypeMembers, (DelegateDeclarationSyntax)syntax2, diagnostics);
                        break;
                    case SyntaxKind.NamespaceDeclaration:
                        AddNonTypeMembers(builder, ((NamespaceDeclarationSyntax)syntax2).Members, diagnostics);
                        break;
                    case SyntaxKind.CompilationUnit:
                        AddNonTypeMembers(builder, ((CompilationUnitSyntax)syntax2).Members, diagnostics);
                        break;
                    case SyntaxKind.ClassDeclaration:
                    case SyntaxKind.StructDeclaration:
                    case SyntaxKind.InterfaceDeclaration:
                        {
                            TypeDeclarationSyntax typeDeclarationSyntax = (TypeDeclarationSyntax)syntax2;
                            AddNonTypeMembers(builder, typeDeclarationSyntax.Members, diagnostics);
                            break;
                        }
                    case SyntaxKind.RecordDeclaration:
                    case SyntaxKind.RecordStructDeclaration:
                        {
                            RecordDeclarationSyntax recordDeclarationSyntax = (RecordDeclarationSyntax)syntax2;
                            ParameterListSyntax parameterList2 = recordDeclarationSyntax.ParameterList;
                            noteRecordParameters(recordDeclarationSyntax, parameterList2, builder, diagnostics);
                            AddNonTypeMembers(builder, recordDeclarationSyntax.Members, diagnostics);
                            if (syntax2.Kind() == SyntaxKind.RecordStructDeclaration && parameterList2 != null && parameterList2.ParameterCount == 0)
                            {
                                diagnostics.Add(ErrorCode.ERR_StructsCantContainDefaultConstructor, parameterList2.Location);
                            }
                            break;
                        }
                    default:
                        throw ExceptionUtilities.UnexpectedValue(syntax2.Kind());
                }
            }
            void noteRecordParameters(RecordDeclarationSyntax syntax, ParameterListSyntax? parameterList, DeclaredMembersAndInitializersBuilder builder, BindingDiagnosticBag diagnostics)
            {
                if (parameterList != null)
                {
                    if (builder.RecordDeclarationWithParameters == null)
                    {
                        builder.RecordDeclarationWithParameters = syntax;
                        SynthesizedRecordConstructor synthesizedRecordConstructor = (builder.RecordPrimaryConstructor = new SynthesizedRecordConstructor(this, syntax));
                        CSharpCompilation declaringCompilation = DeclaringCompilation;
                        builder.UpdateIsNullableEnabledForConstructorsAndFields(synthesizedRecordConstructor.IsStatic, declaringCompilation, parameterList);
                        if (syntax != null)
                        {
                            PrimaryConstructorBaseTypeSyntax primaryConstructorBaseTypeIfClass = syntax.PrimaryConstructorBaseTypeIfClass;
                            if (primaryConstructorBaseTypeIfClass != null)
                            {
                                ArgumentListSyntax argumentList = primaryConstructorBaseTypeIfClass.ArgumentList;
                                if (argumentList != null)
                                {
                                    builder.UpdateIsNullableEnabledForConstructorsAndFields(synthesizedRecordConstructor.IsStatic, declaringCompilation, argumentList);
                                }
                            }
                        }
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_MultipleRecordParameterLists, parameterList!.Location);
                    }
                }
            }
        }

        internal Binder GetBinder(CSharpSyntaxNode syntaxNode)
        {
            return DeclaringCompilation.GetBinder(syntaxNode);
        }

        private void MergePartialMembers(
            ref Dictionary<string, ImmutableArray<Symbol>> membersByName,
            BindingDiagnosticBag diagnostics)
        {
            var memberNames = ArrayBuilder<string>.GetInstance(membersByName.Count);
            memberNames.AddRange(membersByName.Keys);

            //key and value will be the same object
            var methodsBySignature = new Dictionary<MethodSymbol, SourceMemberMethodSymbol>(MemberSignatureComparer.PartialMethodsComparer);

            foreach (var name in memberNames)
            {
                methodsBySignature.Clear();
                foreach (var symbol in membersByName[name])
                {
                    var method = symbol as SourceMemberMethodSymbol;
                    if (method is null || !method.IsPartial)
                    {
                        continue; // only partial methods need to be merged
                    }

                    if (methodsBySignature.TryGetValue(method, out var prev))
                    {
                        var prevPart = (SourceOrdinaryMethodSymbol)prev;
                        var methodPart = (SourceOrdinaryMethodSymbol)method;

                        if (methodPart.IsPartialImplementation &&
                            (prevPart.IsPartialImplementation || (prevPart.OtherPartOfPartial is MethodSymbol otherImplementation && (object)otherImplementation != methodPart)))
                        {
                            // A partial method may not have multiple implementing declarations
                            diagnostics.Add(ErrorCode.ERR_PartialMethodOnlyOneActual, methodPart.Locations[0]);
                        }
                        else if (methodPart.IsPartialDefinition &&
                                 (prevPart.IsPartialDefinition || (prevPart.OtherPartOfPartial is MethodSymbol otherDefinition && (object)otherDefinition != methodPart)))
                        {
                            // A partial method may not have multiple defining declarations
                            diagnostics.Add(ErrorCode.ERR_PartialMethodOnlyOneLatent, methodPart.Locations[0]);
                        }
                        else
                        {
                            if ((object)membersByName == _lazyEarlyAttributeDecodingMembersDictionary)
                            {
                                // Avoid mutating the cached dictionary and especially avoid doing this possibly on multiple threads in parallel.
                                membersByName = new Dictionary<string, ImmutableArray<Symbol>>(membersByName);
                            }

                            membersByName[name] = FixPartialMember(membersByName[name], prevPart, methodPart);
                        }
                    }
                    else
                    {
                        methodsBySignature.Add(method, method);
                    }
                }

                foreach (SourceOrdinaryMethodSymbol method in methodsBySignature.Values)
                {
                    // partial implementations not paired with a definition
                    if (method.IsPartialImplementation && method.OtherPartOfPartial is null)
                    {
                        diagnostics.Add(ErrorCode.ERR_PartialMethodMustHaveLatent, method.Locations[0], method);
                    }
                    else if (method.OtherPartOfPartial is MethodSymbol otherPart && MemberSignatureComparer.ConsideringTupleNamesCreatesDifference(method, otherPart))
                    {
                        diagnostics.Add(ErrorCode.ERR_PartialMethodInconsistentTupleNames, method.Locations[0], method, method.OtherPartOfPartial);
                    }
                    else if (method is { IsPartialDefinition: true, OtherPartOfPartial: null, HasExplicitAccessModifier: true })
                    {
                        diagnostics.Add(ErrorCode.ERR_PartialMethodWithAccessibilityModsMustHaveImplementation, method.Locations[0], method);
                    }
                }
            }

            memberNames.Free();
        }

        private static ImmutableArray<Symbol> FixPartialMember(ImmutableArray<Symbol> symbols, SourceOrdinaryMethodSymbol part1, SourceOrdinaryMethodSymbol part2)
        {
            SourceOrdinaryMethodSymbol definition;
            SourceOrdinaryMethodSymbol sourceOrdinaryMethodSymbol;
            if (part1.IsPartialDefinition)
            {
                definition = part1;
                sourceOrdinaryMethodSymbol = part2;
            }
            else
            {
                definition = part2;
                sourceOrdinaryMethodSymbol = part1;
            }
            SourceOrdinaryMethodSymbol.InitializePartialMethodParts(definition, sourceOrdinaryMethodSymbol);
            return Remove(symbols, sourceOrdinaryMethodSymbol);
        }

        private static ImmutableArray<Symbol> Remove(ImmutableArray<Symbol> symbols, Symbol symbol)
        {
            ArrayBuilder<Symbol> instance = ArrayBuilder<Symbol>.GetInstance();
            ImmutableArray<Symbol>.Enumerator enumerator = symbols.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if ((object)current != symbol)
                {
                    instance.Add(current);
                }
            }
            return instance.ToImmutableAndFree();
        }

        private void CheckForMemberConflictWithPropertyAccessor(PropertySymbol propertySymbol, bool getNotSet, BindingDiagnosticBag diagnostics)
        {
            MethodSymbol methodSymbol = (getNotSet ? propertySymbol.GetMethod : propertySymbol.SetMethod);
            string text = (((object)methodSymbol == null) ? SourcePropertyAccessorSymbol.GetAccessorName(propertySymbol.IsIndexer ? propertySymbol.MetadataName : propertySymbol.Name, getNotSet, propertySymbol.IsCompilationOutputWinMdObj()) : methodSymbol.Name);
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers(text).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Method)
                {
                    if (Locations.Length == 1 || IsPartial)
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, GetAccessorOrPropertyLocation(propertySymbol, getNotSet), this, text);
                    }
                    break;
                }
                MethodSymbol methodSymbol2 = (MethodSymbol)current;
                if (methodSymbol2.MethodKind == MethodKind.Ordinary && ParametersMatchPropertyAccessor(propertySymbol, getNotSet, methodSymbol2.Parameters))
                {
                    diagnostics.Add(ErrorCode.ERR_MemberReserved, GetAccessorOrPropertyLocation(propertySymbol, getNotSet), text, this);
                    break;
                }
            }
        }

        private void CheckForMemberConflictWithEventAccessor(EventSymbol eventSymbol, bool isAdder, BindingDiagnosticBag diagnostics)
        {
            string accessorName = SourceEventSymbol.GetAccessorName(eventSymbol.Name, isAdder);
            ImmutableArray<Symbol>.Enumerator enumerator = GetMembers(accessorName).GetEnumerator();
            while (enumerator.MoveNext())
            {
                Symbol current = enumerator.Current;
                if (current.Kind != SymbolKind.Method)
                {
                    if (Locations.Length == 1 || IsPartial)
                    {
                        diagnostics.Add(ErrorCode.ERR_DuplicateNameInClass, GetAccessorOrEventLocation(eventSymbol, isAdder), this, accessorName);
                    }
                    break;
                }
                MethodSymbol methodSymbol = (MethodSymbol)current;
                if (methodSymbol.MethodKind == MethodKind.Ordinary && ParametersMatchEventAccessor(eventSymbol, methodSymbol.Parameters))
                {
                    diagnostics.Add(ErrorCode.ERR_MemberReserved, GetAccessorOrEventLocation(eventSymbol, isAdder), accessorName, this);
                    break;
                }
            }
        }

        private static Location GetAccessorOrPropertyLocation(PropertySymbol propertySymbol, bool getNotSet)
        {
            return ((Symbol)((getNotSet ? propertySymbol.GetMethod : propertySymbol.SetMethod) ?? ((object)propertySymbol))).Locations[0];
        }

        private static Location GetAccessorOrEventLocation(EventSymbol propertySymbol, bool isAdder)
        {
            return ((Symbol)((isAdder ? propertySymbol.AddMethod : propertySymbol.RemoveMethod) ?? ((object)propertySymbol))).Locations[0];
        }

        private static bool ParametersMatchPropertyAccessor(PropertySymbol propertySymbol, bool getNotSet, ImmutableArray<ParameterSymbol> methodParams)
        {
            ImmutableArray<ParameterSymbol> parameters = propertySymbol.Parameters;
            int num = parameters.Length + ((!getNotSet) ? 1 : 0);
            if (num != methodParams.Length)
            {
                return false;
            }
            for (int i = 0; i < num; i++)
            {
                ParameterSymbol parameterSymbol = methodParams[i];
                if (parameterSymbol.RefKind != 0)
                {
                    return false;
                }
                if (!((i == num - 1 && !getNotSet) ? propertySymbol.TypeWithAnnotations : parameters[i].TypeWithAnnotations).Type.Equals(parameterSymbol.Type, TypeCompareKind.AllIgnoreOptions))
                {
                    return false;
                }
            }
            return true;
        }

        private static bool ParametersMatchEventAccessor(EventSymbol eventSymbol, ImmutableArray<ParameterSymbol> methodParams)
        {
            if (methodParams.Length == 1 && methodParams[0].RefKind == RefKind.None)
            {
                return eventSymbol.Type.Equals(methodParams[0].Type, TypeCompareKind.AllIgnoreOptions);
            }
            return false;
        }

        private void AddEnumMembers(DeclaredMembersAndInitializersBuilder result, EnumDeclarationSyntax syntax, BindingDiagnosticBag diagnostics)
        {
            SourceEnumConstantSymbol sourceEnumConstantSymbol = null;
            int num = 0;
            SeparatedSyntaxList<EnumMemberDeclarationSyntax>.Enumerator enumerator = syntax.Members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EnumMemberDeclarationSyntax current = enumerator.Current;
                EqualsValueClauseSyntax? equalsValue = current.EqualsValue;
                SourceEnumConstantSymbol sourceEnumConstantSymbol2 = ((equalsValue == null) ? SourceEnumConstantSymbol.CreateImplicitValuedConstant(this, current, sourceEnumConstantSymbol, num, diagnostics) : SourceEnumConstantSymbol.CreateExplicitValuedConstant(this, current, diagnostics));
                result.NonTypeMembers.Add(sourceEnumConstantSymbol2);
                if (equalsValue != null || (object)sourceEnumConstantSymbol == null)
                {
                    sourceEnumConstantSymbol = sourceEnumConstantSymbol2;
                    num = 1;
                }
                else
                {
                    num++;
                }
            }
        }

        private static void AddInitializer(ref ArrayBuilder<FieldOrPropertyInitializer>? initializers, FieldSymbol? fieldOpt, CSharpSyntaxNode node)
        {
            if (initializers == null)
            {
                initializers = ArrayBuilder<FieldOrPropertyInitializer>.GetInstance();
            }
            else
            {
                _ = initializers!.Count;
            }
            initializers!.Add(new FieldOrPropertyInitializer(fieldOpt, node));
        }

        private static void AddInitializers(ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>> allInitializers, ArrayBuilder<FieldOrPropertyInitializer>? siblingsOpt)
        {
            if (siblingsOpt != null)
            {
                allInitializers.Add(siblingsOpt);
            }
        }

        private static void CheckInterfaceMembers(ImmutableArray<Symbol> nonTypeMembers, BindingDiagnosticBag diagnostics)
        {
            ImmutableArray<Symbol>.Enumerator enumerator = nonTypeMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                CheckInterfaceMember(enumerator.Current, diagnostics);
            }
        }

        private static void CheckInterfaceMember(Symbol member, BindingDiagnosticBag diagnostics)
        {
            switch (member.Kind)
            {
                case SymbolKind.Method:
                    {
                        MethodSymbol methodSymbol = (MethodSymbol)member;
                        switch (methodSymbol.MethodKind)
                        {
                            case MethodKind.Constructor:
                                diagnostics.Add(ErrorCode.ERR_InterfacesCantContainConstructors, member.Locations[0]);
                                break;
                            case MethodKind.Conversion:
                                diagnostics.Add(ErrorCode.ERR_InterfacesCantContainConversionOrEqualityOperators, member.Locations[0]);
                                break;
                            case MethodKind.UserDefinedOperator:
                                if (methodSymbol.Name == "op_Equality" || methodSymbol.Name == "op_Inequality")
                                {
                                    diagnostics.Add(ErrorCode.ERR_InterfacesCantContainConversionOrEqualityOperators, member.Locations[0]);
                                }
                                break;
                            case MethodKind.Destructor:
                                diagnostics.Add(ErrorCode.ERR_OnlyClassesCanContainDestructors, member.Locations[0]);
                                break;
                            default:
                                throw ExceptionUtilities.UnexpectedValue(methodSymbol.MethodKind);
                            case MethodKind.EventAdd:
                            case MethodKind.EventRemove:
                            case MethodKind.ExplicitInterfaceImplementation:
                            case MethodKind.Ordinary:
                            case MethodKind.PropertyGet:
                            case MethodKind.PropertySet:
                            case MethodKind.StaticConstructor:
                            case MethodKind.LocalFunction:
                                break;
                        }
                        break;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(member.Kind);
                case SymbolKind.Event:
                case SymbolKind.Field:
                case SymbolKind.Property:
                    break;
            }
        }

        private static void CheckForStructDefaultConstructors(ArrayBuilder<Symbol> members, bool isEnum, BindingDiagnosticBag diagnostics)
        {
            ArrayBuilder<Symbol>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current is MethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Constructor && methodSymbol.ParameterCount == 0)
                {
                    if (isEnum)
                    {
                        diagnostics.Add(ErrorCode.ERR_EnumsCantContainDefaultConstructor, methodSymbol.Locations[0]);
                    }
                    else
                    {
                        diagnostics.Add(ErrorCode.ERR_StructsCantContainDefaultConstructor, methodSymbol.Locations[0]);
                    }
                }
            }
        }

        private void CheckForStructBadInitializers(DeclaredMembersAndInitializersBuilder builder, BindingDiagnosticBag diagnostics)
        {
            if (builder.RecordDeclarationWithParameters != null)
            {
                return;
            }
            ArrayBuilder<ArrayBuilder<FieldOrPropertyInitializer>>.Enumerator enumerator = builder.InstanceInitializers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ArrayBuilder<FieldOrPropertyInitializer>.Enumerator enumerator2 = enumerator.Current.GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    FieldOrPropertyInitializer current = enumerator2.Current;
                    diagnostics.Add(ErrorCode.ERR_FieldInitializerInStruct, (current.FieldOpt.AssociatedSymbol ?? current.FieldOpt).Locations[0], this);
                }
            }
        }

        private void AddSynthesizedRecordMembersIfNecessary(MembersAndInitializersBuilder builder, DeclaredMembersAndInitializers declaredMembersAndInitializers, BindingDiagnosticBag diagnostics)
        {
            BindingDiagnosticBag diagnostics2 = diagnostics;
            MembersAndInitializersBuilder builder2 = builder;
            DeclarationKind kind = declaration.Kind;
            if (kind != DeclarationKind.Record && kind != DeclarationKind.RecordStruct)
            {
                return;
            }
            ParameterListSyntax paramList = declaredMembersAndInitializers.RecordDeclarationWithParameters?.ParameterList;
            PooledDictionary<Symbol, Symbol> memberSignatures = s_duplicateRecordMemberSignatureDictionary.Allocate();
            PooledDictionary<string, Symbol> fieldsByName = PooledDictionary<string, Symbol>.GetInstance();
            IReadOnlyCollection<Symbol> nonTypeMembers = builder2.GetNonTypeMembers(declaredMembersAndInitializers);
            ArrayBuilder<Symbol> members = ArrayBuilder<Symbol>.GetInstance(nonTypeMembers.Count + 1);
            PooledHashSet<string> memberNames = PooledHashSet<string>.GetInstance();
            foreach (Symbol item2 in nonTypeMembers)
            {
                memberNames.Add(item2.Name);
                if (item2 is EventSymbol)
                {
                    continue;
                }
                if (item2 is MethodSymbol methodSymbol)
                {
                    MethodKind methodKind = methodSymbol.MethodKind;
                    if (methodKind != MethodKind.Constructor && methodKind != MethodKind.Ordinary)
                    {
                        continue;
                    }
                }
                else if (item2 is FieldSymbol)
                {
                    string name = item2.Name;
                    if (!fieldsByName.ContainsKey(name))
                    {
                        fieldsByName.Add(name, item2);
                    }
                    continue;
                }
                if (!memberSignatures.ContainsKey(item2))
                {
                    memberSignatures.Add(item2, item2);
                }
            }
            CSharpCompilation compilation = DeclaringCompilation;
            bool isRecordClass = declaration.Kind == DeclarationKind.Record;
            bool primaryAndCopyCtorAmbiguity2 = false;
            if (paramList != null)
            {
                SynthesizedRecordConstructor recordPrimaryConstructor = declaredMembersAndInitializers.RecordPrimaryConstructor;
                members.Add(recordPrimaryConstructor);
                if (recordPrimaryConstructor.ParameterCount != 0)
                {
                    ImmutableArray<Symbol> positionalMembers2 = addProperties(recordPrimaryConstructor.Parameters);
                    addDeconstruct(recordPrimaryConstructor, positionalMembers2);
                }
                if (isRecordClass)
                {
                    primaryAndCopyCtorAmbiguity2 = recordPrimaryConstructor.ParameterCount == 1 && recordPrimaryConstructor.Parameters[0].Type.Equals(this, TypeCompareKind.AllIgnoreOptions);
                }
            }
            if (isRecordClass)
            {
                addCopyCtor(primaryAndCopyCtorAmbiguity2);
                addCloneMethod();
            }
            PropertySymbol equalityContract2 = (isRecordClass ? addEqualityContract() : null);
            MethodSymbol methodSymbol2 = addThisEquals(equalityContract2);
            if (isRecordClass)
            {
                addBaseEquals();
            }
            addObjectEquals(methodSymbol2);
            MethodSymbol methodSymbol3 = addGetHashCode(equalityContract2);
            addEqualityOperators();
            if (!(methodSymbol2 is SynthesizedRecordEquals) && methodSymbol3 is SynthesizedRecordGetHashCode)
            {
                diagnostics2.Add(ErrorCode.WRN_RecordEqualsWithoutGetHashCode, methodSymbol2.Locations[0], declaration.Name);
            }
            MethodSymbol printMethod2 = addPrintMembersMethod();
            addToStringMethod(printMethod2);
            memberSignatures.Free();
            fieldsByName.Free();
            memberNames.Free();
            members.AddRange(nonTypeMembers);
            builder2.NonTypeMembers?.Free();
            builder2.NonTypeMembers = members;
            void addBaseEquals()
            {
                if (!BaseTypeNoUseSiteDiagnostics.IsObjectType())
                {
                    members.Add(new SynthesizedRecordBaseEquals(this, members.Count, diagnostics2));
                }
            }
            void addCloneMethod()
            {
                members.Add(new SynthesizedRecordClone(this, members.Count, diagnostics2));
            }
            void addCopyCtor(bool primaryAndCopyCtorAmbiguity)
            {
                SignatureOnlyMethodSymbol key3 = new SignatureOnlyMethodSymbol(".ctor", this, MethodKind.Constructor, CallingConvention.HasThis, ImmutableArray<TypeParameterSymbol>.Empty, ImmutableArray.Create((ParameterSymbol)new SignatureOnlyParameterSymbol(TypeWithAnnotations.Create(this), ImmutableArray<CustomModifier>.Empty, isParams: false, RefKind.None)), RefKind.None, isInitOnly: false, TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Void)), ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty);
                if (!memberSignatures.TryGetValue(key3, out var value6))
                {
                    SynthesizedRecordCopyCtor synthesizedRecordCopyCtor = new SynthesizedRecordCopyCtor(this, members.Count);
                    members.Add(synthesizedRecordCopyCtor);
                    if (primaryAndCopyCtorAmbiguity)
                    {
                        diagnostics2.Add(ErrorCode.ERR_RecordAmbigCtor, synthesizedRecordCopyCtor.Locations[0]);
                    }
                }
                else
                {
                    MethodSymbol methodSymbol9 = (MethodSymbol)value6;
                    if (!IsSealed && methodSymbol9.DeclaredAccessibility != Accessibility.Public && methodSymbol9.DeclaredAccessibility != Accessibility.Protected)
                    {
                        diagnostics2.Add(ErrorCode.ERR_CopyConstructorWrongAccessibility, methodSymbol9.Locations[0], methodSymbol9);
                    }
                }
            }
            void addDeconstruct(SynthesizedRecordConstructor ctor, ImmutableArray<Symbol> positionalMembers)
            {
                SignatureOnlyMethodSymbol signatureOnlyMethodSymbol3 = new SignatureOnlyMethodSymbol("Deconstruct", this, MethodKind.Ordinary, CallingConvention.HasThis, ImmutableArray<TypeParameterSymbol>.Empty, ctor.Parameters.SelectAsArray((Func<ParameterSymbol, ParameterSymbol>)((ParameterSymbol param) => new SignatureOnlyParameterSymbol(param.TypeWithAnnotations, ImmutableArray<CustomModifier>.Empty, isParams: false, RefKind.Out))), RefKind.None, isInitOnly: false, TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Void)), ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty);
                if (!memberSignatures.TryGetValue(signatureOnlyMethodSymbol3, out var value7))
                {
                    members.Add(new SynthesizedRecordDeconstruct(this, ctor, positionalMembers, members.Count, diagnostics2));
                }
                else
                {
                    MethodSymbol methodSymbol10 = (MethodSymbol)value7;
                    if (methodSymbol10.DeclaredAccessibility != Accessibility.Public)
                    {
                        diagnostics2.Add(ErrorCode.ERR_NonPublicAPIInRecord, methodSymbol10.Locations[0], methodSymbol10);
                    }
                    if (methodSymbol10.ReturnType.SpecialType != SpecialType.System_Void && !methodSymbol10.ReturnType.IsErrorType())
                    {
                        diagnostics2.Add(ErrorCode.ERR_SignatureMismatchInRecord, methodSymbol10.Locations[0], methodSymbol10, signatureOnlyMethodSymbol3.ReturnType);
                    }
                    if (methodSymbol10.IsStatic)
                    {
                        diagnostics2.Add(ErrorCode.ERR_StaticAPIInRecord, methodSymbol10.Locations[0], methodSymbol10);
                    }
                }
            }
            PropertySymbol addEqualityContract()
            {
                SignatureOnlyPropertySymbol signatureOnlyPropertySymbol = new SignatureOnlyPropertySymbol("EqualityContract", this, ImmutableArray<ParameterSymbol>.Empty, RefKind.None, TypeWithAnnotations.Create(compilation.GetWellKnownType(WellKnownType.System_Type)), ImmutableArray<CustomModifier>.Empty, isStatic: false, ImmutableArray<PropertySymbol>.Empty);
                PropertySymbol propertySymbol;
                if (!memberSignatures.TryGetValue(signatureOnlyPropertySymbol, out var value5))
                {
                    propertySymbol = new SynthesizedRecordEqualityContractProperty(this, diagnostics2);
                    members.Add(propertySymbol);
                    members.Add(propertySymbol.GetMethod);
                }
                else
                {
                    propertySymbol = (PropertySymbol)value5;
                    if (IsSealed && BaseTypeNoUseSiteDiagnostics.IsObjectType())
                    {
                        if (propertySymbol.DeclaredAccessibility != Accessibility.Private)
                        {
                            diagnostics2.Add(ErrorCode.ERR_NonPrivateAPIInRecord, propertySymbol.Locations[0], propertySymbol);
                        }
                    }
                    else if (propertySymbol.DeclaredAccessibility != Accessibility.Protected)
                    {
                        diagnostics2.Add(ErrorCode.ERR_NonProtectedAPIInRecord, propertySymbol.Locations[0], propertySymbol);
                    }
                    if (!propertySymbol.Type.Equals(signatureOnlyPropertySymbol.Type, TypeCompareKind.AllIgnoreOptions))
                    {
                        if (!propertySymbol.Type.IsErrorType())
                        {
                            diagnostics2.Add(ErrorCode.ERR_SignatureMismatchInRecord, propertySymbol.Locations[0], propertySymbol, signatureOnlyPropertySymbol.Type);
                        }
                    }
                    else
                    {
                        SynthesizedRecordEqualityContractProperty.VerifyOverridesEqualityContractFromBase(propertySymbol, diagnostics2);
                    }
                    if ((object)propertySymbol.GetMethod == null)
                    {
                        diagnostics2.Add(ErrorCode.ERR_EqualityContractRequiresGetter, propertySymbol.Locations[0], propertySymbol);
                    }
                    reportStaticOrNotOverridableAPIInRecord(propertySymbol, diagnostics2);
                }
                return propertySymbol;
            }
            void addEqualityOperators()
            {
                members.Add(new SynthesizedRecordEqualityOperator(this, members.Count, diagnostics2));
                members.Add(new SynthesizedRecordInequalityOperator(this, members.Count, diagnostics2));
            }
            MethodSymbol addGetHashCode(PropertySymbol? equalityContract)
            {
                SignatureOnlyMethodSymbol key2 = new SignatureOnlyMethodSymbol("GetHashCode", this, MethodKind.Ordinary, CallingConvention.HasThis, ImmutableArray<TypeParameterSymbol>.Empty, ImmutableArray<ParameterSymbol>.Empty, RefKind.None, isInitOnly: false, TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Int32)), ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty);
                MethodSymbol methodSymbol7;
                if (!memberSignatures.TryGetValue(key2, out var value3))
                {
                    methodSymbol7 = new SynthesizedRecordGetHashCode(this, equalityContract, members.Count, diagnostics2);
                    members.Add(methodSymbol7);
                }
                else
                {
                    methodSymbol7 = (MethodSymbol)value3;
                    if (!SynthesizedRecordObjectMethod.VerifyOverridesMethodFromObject(methodSymbol7, SpecialMember.System_Object__GetHashCode, diagnostics2) && methodSymbol7.IsSealed && !IsSealed)
                    {
                        diagnostics2.Add(ErrorCode.ERR_SealedAPIInRecord, methodSymbol7.Locations[0], methodSymbol7);
                    }
                }
                return methodSymbol7;
            }
            void addObjectEquals(MethodSymbol thisEquals)
            {
                members.Add(new SynthesizedRecordObjEquals(this, thisEquals, members.Count, diagnostics2));
            }
            MethodSymbol addPrintMembersMethod()
            {
                SignatureOnlyMethodSymbol signatureOnlyMethodSymbol = new SignatureOnlyMethodSymbol("PrintMembers", this, MethodKind.Ordinary, CallingConvention.HasThis, ImmutableArray<TypeParameterSymbol>.Empty, ImmutableArray.Create((ParameterSymbol)new SignatureOnlyParameterSymbol(TypeWithAnnotations.Create(compilation.GetWellKnownType(WellKnownType.System_Text_StringBuilder)), ImmutableArray<CustomModifier>.Empty, isParams: false, RefKind.None)), RefKind.None, isInitOnly: false, TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Boolean)), ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty);
                MethodSymbol methodSymbol6;
                if (!memberSignatures.TryGetValue(signatureOnlyMethodSymbol, out var value2))
                {
                    methodSymbol6 = new SynthesizedRecordPrintMembers(this, members.Count, diagnostics2);
                    members.Add(methodSymbol6);
                }
                else
                {
                    methodSymbol6 = (MethodSymbol)value2;
                    if (!isRecordClass || (IsSealed && BaseTypeNoUseSiteDiagnostics.IsObjectType()))
                    {
                        if (methodSymbol6.DeclaredAccessibility != Accessibility.Private)
                        {
                            diagnostics2.Add(ErrorCode.ERR_NonPrivateAPIInRecord, methodSymbol6.Locations[0], methodSymbol6);
                        }
                    }
                    else if (methodSymbol6.DeclaredAccessibility != Accessibility.Protected)
                    {
                        diagnostics2.Add(ErrorCode.ERR_NonProtectedAPIInRecord, methodSymbol6.Locations[0], methodSymbol6);
                    }
                    if (!methodSymbol6.ReturnType.Equals(signatureOnlyMethodSymbol.ReturnType, TypeCompareKind.AllIgnoreOptions))
                    {
                        if (!methodSymbol6.ReturnType.IsErrorType())
                        {
                            diagnostics2.Add(ErrorCode.ERR_SignatureMismatchInRecord, methodSymbol6.Locations[0], methodSymbol6, signatureOnlyMethodSymbol.ReturnType);
                        }
                    }
                    else if (isRecordClass)
                    {
                        SynthesizedRecordPrintMembers.VerifyOverridesPrintMembersFromBase(methodSymbol6, diagnostics2);
                    }
                    reportStaticOrNotOverridableAPIInRecord(methodSymbol6, diagnostics2);
                }
                return methodSymbol6;
            }
            ImmutableArray<Symbol> addProperties(ImmutableArray<ParameterSymbol> recordParameters)
            {
                ArrayBuilder<Symbol> existingOrAddedMembers = ArrayBuilder<Symbol>.GetInstance(recordParameters.Length);
                int addedCount = 0;
                ImmutableArray<ParameterSymbol>.Enumerator enumerator3 = recordParameters.GetEnumerator();
                while (enumerator3.MoveNext())
                {
                    ParameterSymbol param2 = enumerator3.Current;
                    bool flag = false;
                    CSharpSyntaxNode nonNullSyntaxNode = param2.GetNonNullSyntaxNode();
                    SignatureOnlyPropertySymbol signatureOnlyPropertySymbol2 = new SignatureOnlyPropertySymbol(param2.Name, this, ImmutableArray<ParameterSymbol>.Empty, RefKind.None, param2.TypeWithAnnotations, ImmutableArray<CustomModifier>.Empty, isStatic: false, ImmutableArray<PropertySymbol>.Empty);
                    if (!memberSignatures.TryGetValue(signatureOnlyPropertySymbol2, out var value8) && !fieldsByName.TryGetValue(param2.Name, out value8))
                    {
                        value8 = OverriddenOrHiddenMembersHelpers.FindFirstHiddenMemberIfAny(signatureOnlyPropertySymbol2, memberIsFromSomeCompilation: true);
                        flag = true;
                    }
                    if ((object)value8 == null)
                    {
                        addProperty(new SynthesizedRecordPropertySymbol(this, nonNullSyntaxNode, param2, isOverride: false, diagnostics2));
                    }
                    else if (value8 is FieldSymbol fieldSymbol && !value8.IsStatic && fieldSymbol.TypeWithAnnotations.Equals(param2.TypeWithAnnotations, TypeCompareKind.AllIgnoreOptions))
                    {
                        Binder.CheckFeatureAvailability(nonNullSyntaxNode, MessageID.IDS_FeaturePositionalFieldsInRecords, diagnostics2);
                        if (!flag || checkMemberNotHidden(fieldSymbol, param2))
                        {
                            existingOrAddedMembers.Add(fieldSymbol);
                        }
                    }
                    else if (value8 is PropertySymbol propertySymbol2 && !value8.IsStatic && (object)propertySymbol2.GetMethod != null && propertySymbol2.TypeWithAnnotations.Equals(param2.TypeWithAnnotations, TypeCompareKind.AllIgnoreOptions))
                    {
                        if (flag && propertySymbol2.IsAbstract)
                        {
                            addProperty(new SynthesizedRecordPropertySymbol(this, nonNullSyntaxNode, param2, isOverride: true, diagnostics2));
                        }
                        else if (!flag || checkMemberNotHidden(propertySymbol2, param2))
                        {
                            existingOrAddedMembers.Add(propertySymbol2);
                        }
                    }
                    else
                    {
                        diagnostics2.Add(ErrorCode.ERR_BadRecordMemberForPositionalParameter, param2.Locations[0], new FormattedSymbol(value8, SymbolDisplayFormat.CSharpErrorMessageFormat.WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType)), param2.TypeWithAnnotations, param2.Name);
                    }
                    void addProperty(SynthesizedRecordPropertySymbol property)
                    {
                        existingOrAddedMembers.Add(property);
                        members.Add(property);
                        members.Add(property.GetMethod);
                        members.Add(property.SetMethod);
                        members.Add(property.BackingField);
                        builder2.AddInstanceInitializerForPositionalMembers(new FieldOrPropertyInitializer(property.BackingField, paramList.Parameters[param2.Ordinal]));
                        addedCount++;
                    }
                }
                return existingOrAddedMembers.ToImmutableAndFree();
            }
            MethodSymbol addThisEquals(PropertySymbol? equalityContract)
            {
                SignatureOnlyMethodSymbol signatureOnlyMethodSymbol2 = new SignatureOnlyMethodSymbol("Equals", this, MethodKind.Ordinary, CallingConvention.HasThis, ImmutableArray<TypeParameterSymbol>.Empty, ImmutableArray.Create((ParameterSymbol)new SignatureOnlyParameterSymbol(TypeWithAnnotations.Create(this), ImmutableArray<CustomModifier>.Empty, isParams: false, RefKind.None)), RefKind.None, isInitOnly: false, TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_Boolean)), ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty);
                MethodSymbol methodSymbol8;
                if (!memberSignatures.TryGetValue(signatureOnlyMethodSymbol2, out var value4))
                {
                    methodSymbol8 = new SynthesizedRecordEquals(this, equalityContract, members.Count, diagnostics2);
                    members.Add(methodSymbol8);
                }
                else
                {
                    methodSymbol8 = (MethodSymbol)value4;
                    if (methodSymbol8.DeclaredAccessibility != Accessibility.Public)
                    {
                        diagnostics2.Add(ErrorCode.ERR_NonPublicAPIInRecord, methodSymbol8.Locations[0], methodSymbol8);
                    }
                    if (methodSymbol8.ReturnType.SpecialType != SpecialType.System_Boolean && !methodSymbol8.ReturnType.IsErrorType())
                    {
                        diagnostics2.Add(ErrorCode.ERR_SignatureMismatchInRecord, methodSymbol8.Locations[0], methodSymbol8, signatureOnlyMethodSymbol2.ReturnType);
                    }
                    reportStaticOrNotOverridableAPIInRecord(methodSymbol8, diagnostics2);
                }
                return methodSymbol8;
            }
            void addToStringMethod(MethodSymbol printMethod)
            {
                SignatureOnlyMethodSymbol key = new SignatureOnlyMethodSymbol("ToString", this, MethodKind.Ordinary, CallingConvention.HasThis, ImmutableArray<TypeParameterSymbol>.Empty, ImmutableArray<ParameterSymbol>.Empty, RefKind.None, isInitOnly: false, TypeWithAnnotations.Create(compilation.GetSpecialType(SpecialType.System_String)), ImmutableArray<CustomModifier>.Empty, ImmutableArray<MethodSymbol>.Empty);
                MethodSymbol baseToStringMethod = getBaseToStringMethod();
                if ((object)baseToStringMethod != null && baseToStringMethod.IsSealed)
                {
                    if (baseToStringMethod.ContainingModule != ContainingModule && !DeclaringCompilation.IsFeatureEnabled(MessageID.IDS_FeatureSealedToStringInRecord))
                    {
                        LanguageVersion languageVersion = ((CSharpParseOptions)Locations[0].SourceTree!.Options).LanguageVersion;
                        LanguageVersion version = MessageID.IDS_FeatureSealedToStringInRecord.RequiredVersion();
                        diagnostics2.Add(ErrorCode.ERR_InheritingFromRecordWithSealedToString, Locations[0], languageVersion.ToDisplayString(), new CSharpRequiredLanguageVersion(version));
                    }
                }
                else if (!memberSignatures.TryGetValue(key, out Symbol value))
                {
                    SynthesizedRecordToString item = new SynthesizedRecordToString(this, printMethod, members.Count, diagnostics2);
                    members.Add(item);
                }
                else
                {
                    MethodSymbol methodSymbol5 = (MethodSymbol)value;
                    if (!SynthesizedRecordObjectMethod.VerifyOverridesMethodFromObject(methodSymbol5, SpecialMember.System_Object__ToString, diagnostics2) && methodSymbol5.IsSealed && !IsSealed)
                    {
                        MessageID.IDS_FeatureSealedToStringInRecord.CheckFeatureAvailability(diagnostics2, DeclaringCompilation, methodSymbol5.Locations[0]);
                    }
                }
            }
            bool checkMemberNotHidden(Symbol symbol, ParameterSymbol param)
            {
                if (memberNames.Contains(symbol.Name) || GetTypeMembersDictionary().ContainsKey(symbol.Name))
                {
                    diagnostics2.Add(ErrorCode.ERR_HiddenPositionalMember, param.Locations[0], symbol);
                    return false;
                }
                return true;
            }
            MethodSymbol? getBaseToStringMethod()
            {
                Symbol specialTypeMember = DeclaringCompilation.GetSpecialTypeMember(SpecialMember.System_Object__ToString);
                NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
                while ((object)baseTypeNoUseSiteDiagnostics != null)
                {
                    ImmutableArray<Symbol>.Enumerator enumerator2 = baseTypeNoUseSiteDiagnostics.GetSimpleNonTypeMembers("ToString").GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        if (enumerator2.Current is MethodSymbol methodSymbol4 && methodSymbol4.GetLeastOverriddenMethod(null) == specialTypeMember)
                        {
                            return methodSymbol4;
                        }
                    }
                    baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
                }
                return null;
            }
            void reportStaticOrNotOverridableAPIInRecord(Symbol symbol, BindingDiagnosticBag diagnostics)
            {
                if (isRecordClass && !IsSealed && ((!symbol.IsAbstract && !symbol.IsVirtual && !symbol.IsOverride) || symbol.IsSealed))
                {
                    diagnostics.Add(ErrorCode.ERR_NotOverridableAPIInRecord, symbol.Locations[0], symbol);
                }
                else if (symbol.IsStatic)
                {
                    diagnostics.Add(ErrorCode.ERR_StaticAPIInRecord, symbol.Locations[0], symbol);
                }
            }
        }

        private void AddSynthesizedConstructorsIfNecessary(MembersAndInitializersBuilder builder, DeclaredMembersAndInitializers declaredMembersAndInitializers, BindingDiagnosticBag diagnostics)
        {
            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;
            foreach (Symbol nonTypeMember in builder.GetNonTypeMembers(declaredMembersAndInitializers))
            {
                if (nonTypeMember.Kind == SymbolKind.Method)
                {
                    MethodSymbol methodSymbol = (MethodSymbol)nonTypeMember;
                    switch (methodSymbol.MethodKind)
                    {
                        case MethodKind.Constructor:
                            if (!IsRecord || !SynthesizedRecordCopyCtor.HasCopyConstructorSignature(methodSymbol) || methodSymbol is SynthesizedRecordConstructor)
                            {
                                flag = true;
                                flag2 = flag2 || methodSymbol.ParameterCount == 0;
                            }
                            break;
                        case MethodKind.StaticConstructor:
                            flag3 = true;
                            break;
                    }
                }
                if (flag && flag3)
                {
                    break;
                }
            }
            if ((!flag2 && this.IsStructType()) || (!flag && !IsStatic && !IsInterface))
            {
                builder.AddNonTypeMember((TypeKind == TypeKind.Submission) ? new SynthesizedSubmissionConstructor(this, diagnostics) : new SynthesizedInstanceConstructor(this), declaredMembersAndInitializers);
            }
            if (!flag3 && hasNonConstantInitializer(declaredMembersAndInitializers.StaticInitializers))
            {
                builder.AddNonTypeMember(new SynthesizedStaticConstructor(this), declaredMembersAndInitializers);
            }
            if (IsScriptClass)
            {
                SynthesizedInteractiveInitializerMethod synthesizedInteractiveInitializerMethod = new SynthesizedInteractiveInitializerMethod(this, diagnostics);
                builder.AddNonTypeMember(synthesizedInteractiveInitializerMethod, declaredMembersAndInitializers);
                SynthesizedEntryPointSymbol member = SynthesizedEntryPointSymbol.Create(synthesizedInteractiveInitializerMethod, diagnostics);
                builder.AddNonTypeMember(member, declaredMembersAndInitializers);
            }
            static bool hasNonConstantInitializer(ImmutableArray<ImmutableArray<FieldOrPropertyInitializer>> initializers)
            {
                return initializers.Any((ImmutableArray<FieldOrPropertyInitializer> siblings) => siblings.Any((FieldOrPropertyInitializer initializer) => !initializer.FieldOpt.IsConst));
            }
        }

        private void AddNonTypeMembers(DeclaredMembersAndInitializersBuilder builder, SyntaxList<MemberDeclarationSyntax> members, BindingDiagnosticBag diagnostics)
        {
            if (members.Count == 0)
            {
                return;
            }
            MemberDeclarationSyntax syntaxNode = members[0];
            Binder binder = GetBinder(syntaxNode);
            ArrayBuilder<FieldOrPropertyInitializer> initializers = null;
            ArrayBuilder<FieldOrPropertyInitializer> initializers2 = null;
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            SyntaxList<MemberDeclarationSyntax>.Enumerator enumerator = members.GetEnumerator();
            while (enumerator.MoveNext())
            {
                MemberDeclarationSyntax current = enumerator.Current;
                if (_lazyMembersAndInitializers != null)
                {
                    return;
                }
                bool flag = !current.HasErrors;
                switch (current.Kind())
                {
                    case SyntaxKind.FieldDeclaration:
                        {
                            FieldDeclarationSyntax fieldDeclarationSyntax = (FieldDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = fieldDeclarationSyntax.Declaration.Variables.First().Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            DeclarationModifiers declarationModifiers = SourceMemberFieldSymbol.MakeModifiers(this, fieldDeclarationSyntax.Declaration.Variables[0].Identifier, fieldDeclarationSyntax.Modifiers, diagnostics, out bool modifierErrors);
                            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator2 = fieldDeclarationSyntax.Declaration.Variables.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                VariableDeclaratorSyntax current4 = enumerator2.Current;
                                SourceMemberFieldSymbolFromDeclarator sourceMemberFieldSymbolFromDeclarator = (((declarationModifiers & DeclarationModifiers.Fixed) == 0) ? new SourceMemberFieldSymbolFromDeclarator(this, current4, declarationModifiers, modifierErrors, diagnostics) : new SourceFixedFieldSymbol(this, current4, declarationModifiers, modifierErrors, diagnostics));
                                builder.NonTypeMembers.Add(sourceMemberFieldSymbolFromDeclarator);
                                builder.UpdateIsNullableEnabledForConstructorsAndFields(sourceMemberFieldSymbolFromDeclarator.IsStatic, declaringCompilation, current4);
                                if (IsScriptClass)
                                {
                                    ExpressionFieldFinder.FindExpressionVariables(builder.NonTypeMembers, current4, this, DeclarationModifiers.Private | (declarationModifiers & DeclarationModifiers.Static), sourceMemberFieldSymbolFromDeclarator);
                                }
                                if (current4.Initializer != null)
                                {
                                    if (sourceMemberFieldSymbolFromDeclarator.IsStatic)
                                    {
                                        AddInitializer(ref initializers, sourceMemberFieldSymbolFromDeclarator, current4.Initializer);
                                    }
                                    else
                                    {
                                        AddInitializer(ref initializers2, sourceMemberFieldSymbolFromDeclarator, current4.Initializer);
                                    }
                                }
                            }
                            break;
                        }
                    case SyntaxKind.MethodDeclaration:
                        {
                            MethodDeclarationSyntax methodDeclarationSyntax = (MethodDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = methodDeclarationSyntax.Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourceOrdinaryMethodSymbol item2 = SourceOrdinaryMethodSymbol.CreateMethodSymbol(this, binder, methodDeclarationSyntax, declaringCompilation.IsNullableAnalysisEnabledIn(methodDeclarationSyntax), diagnostics);
                            builder.NonTypeMembers.Add(item2);
                            break;
                        }
                    case SyntaxKind.ConstructorDeclaration:
                        {
                            ConstructorDeclarationSyntax constructorDeclarationSyntax = (ConstructorDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = constructorDeclarationSyntax.Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            bool flag2 = declaringCompilation.IsNullableAnalysisEnabledIn(constructorDeclarationSyntax);
                            SourceConstructorSymbol sourceConstructorSymbol = SourceConstructorSymbol.CreateConstructorSymbol(this, constructorDeclarationSyntax, flag2, diagnostics);
                            builder.NonTypeMembers.Add(sourceConstructorSymbol);
                            ConstructorInitializerSyntax? initializer = constructorDeclarationSyntax.Initializer;
                            if (initializer == null || initializer!.Kind() != SyntaxKind.ThisConstructorInitializer)
                            {
                                builder.UpdateIsNullableEnabledForConstructorsAndFields(sourceConstructorSymbol.IsStatic, flag2);
                            }
                            break;
                        }
                    case SyntaxKind.DestructorDeclaration:
                        {
                            DestructorDeclarationSyntax destructorDeclarationSyntax = (DestructorDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = destructorDeclarationSyntax.Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourceDestructorSymbol item4 = new SourceDestructorSymbol(this, destructorDeclarationSyntax, declaringCompilation.IsNullableAnalysisEnabledIn(destructorDeclarationSyntax), diagnostics);
                            builder.NonTypeMembers.Add(item4);
                            break;
                        }
                    case SyntaxKind.PropertyDeclaration:
                        {
                            PropertyDeclarationSyntax propertyDeclarationSyntax = (PropertyDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = propertyDeclarationSyntax.Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourcePropertySymbol sourcePropertySymbol2 = SourcePropertySymbol.Create(this, binder, propertyDeclarationSyntax, diagnostics);
                            builder.NonTypeMembers.Add(sourcePropertySymbol2);
                            AddAccessorIfAvailable(builder.NonTypeMembers, sourcePropertySymbol2.GetMethod);
                            AddAccessorIfAvailable(builder.NonTypeMembers, sourcePropertySymbol2.SetMethod);
                            FieldSymbol backingField = sourcePropertySymbol2.BackingField;
                            if ((object)backingField == null)
                            {
                                break;
                            }
                            builder.NonTypeMembers.Add(backingField);
                            builder.UpdateIsNullableEnabledForConstructorsAndFields(backingField.IsStatic, declaringCompilation, propertyDeclarationSyntax);
                            EqualsValueClauseSyntax initializer2 = propertyDeclarationSyntax.Initializer;
                            if (initializer2 != null)
                            {
                                if (IsScriptClass)
                                {
                                    ExpressionFieldFinder.FindExpressionVariables(builder.NonTypeMembers, initializer2, this, DeclarationModifiers.Private | (sourcePropertySymbol2.IsStatic ? DeclarationModifiers.Static : DeclarationModifiers.None), backingField);
                                }
                                if (sourcePropertySymbol2.IsStatic)
                                {
                                    AddInitializer(ref initializers, backingField, initializer2);
                                }
                                else
                                {
                                    AddInitializer(ref initializers2, backingField, initializer2);
                                }
                            }
                            break;
                        }
                    case SyntaxKind.EventFieldDeclaration:
                        {
                            EventFieldDeclarationSyntax eventFieldDeclarationSyntax = (EventFieldDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = eventFieldDeclarationSyntax.Declaration.Variables.First().Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator2 = eventFieldDeclarationSyntax.Declaration.Variables.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                VariableDeclaratorSyntax current3 = enumerator2.Current;
                                SourceFieldLikeEventSymbol sourceFieldLikeEventSymbol = new SourceFieldLikeEventSymbol(this, binder, eventFieldDeclarationSyntax.Modifiers, current3, diagnostics);
                                builder.NonTypeMembers.Add(sourceFieldLikeEventSymbol);
                                FieldSymbol associatedField = sourceFieldLikeEventSymbol.AssociatedField;
                                if (IsScriptClass)
                                {
                                    ExpressionFieldFinder.FindExpressionVariables(builder.NonTypeMembers, current3, this, DeclarationModifiers.Private | (sourceFieldLikeEventSymbol.IsStatic ? DeclarationModifiers.Static : DeclarationModifiers.None), associatedField);
                                }
                                if ((object)associatedField != null)
                                {
                                    builder.UpdateIsNullableEnabledForConstructorsAndFields(associatedField.IsStatic, declaringCompilation, current3);
                                    if (current3.Initializer != null)
                                    {
                                        if (associatedField.IsStatic)
                                        {
                                            AddInitializer(ref initializers, associatedField, current3.Initializer);
                                        }
                                        else
                                        {
                                            AddInitializer(ref initializers2, associatedField, current3.Initializer);
                                        }
                                    }
                                }
                                AddAccessorIfAvailable(builder.NonTypeMembers, sourceFieldLikeEventSymbol.AddMethod);
                                AddAccessorIfAvailable(builder.NonTypeMembers, sourceFieldLikeEventSymbol.RemoveMethod);
                            }
                            break;
                        }
                    case SyntaxKind.EventDeclaration:
                        {
                            EventDeclarationSyntax eventDeclarationSyntax = (EventDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = eventDeclarationSyntax.Identifier;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourceCustomEventSymbol sourceCustomEventSymbol = new SourceCustomEventSymbol(this, binder, eventDeclarationSyntax, diagnostics);
                            builder.NonTypeMembers.Add(sourceCustomEventSymbol);
                            AddAccessorIfAvailable(builder.NonTypeMembers, sourceCustomEventSymbol.AddMethod);
                            AddAccessorIfAvailable(builder.NonTypeMembers, sourceCustomEventSymbol.RemoveMethod);
                            break;
                        }
                    case SyntaxKind.IndexerDeclaration:
                        {
                            IndexerDeclarationSyntax indexerDeclarationSyntax = (IndexerDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = indexerDeclarationSyntax.ThisKeyword;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourcePropertySymbol sourcePropertySymbol = SourcePropertySymbol.Create(this, binder, indexerDeclarationSyntax, diagnostics);
                            builder.HaveIndexers = true;
                            builder.NonTypeMembers.Add(sourcePropertySymbol);
                            AddAccessorIfAvailable(builder.NonTypeMembers, sourcePropertySymbol.GetMethod);
                            AddAccessorIfAvailable(builder.NonTypeMembers, sourcePropertySymbol.SetMethod);
                            break;
                        }
                    case SyntaxKind.ConversionOperatorDeclaration:
                        {
                            ConversionOperatorDeclarationSyntax conversionOperatorDeclarationSyntax = (ConversionOperatorDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = conversionOperatorDeclarationSyntax.OperatorKeyword;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourceUserDefinedConversionSymbol item = SourceUserDefinedConversionSymbol.CreateUserDefinedConversionSymbol(this, conversionOperatorDeclarationSyntax, declaringCompilation.IsNullableAnalysisEnabledIn(conversionOperatorDeclarationSyntax), diagnostics);
                            builder.NonTypeMembers.Add(item);
                            break;
                        }
                    case SyntaxKind.OperatorDeclaration:
                        {
                            OperatorDeclarationSyntax operatorDeclarationSyntax = (OperatorDeclarationSyntax)current;
                            if (IsImplicitClass && flag)
                            {
                                SyntaxToken token = operatorDeclarationSyntax.OperatorKeyword;
                                diagnostics.Add(ErrorCode.ERR_NamespaceUnexpected, new SourceLocation(in token));
                            }
                            SourceUserDefinedOperatorSymbol item3 = SourceUserDefinedOperatorSymbol.CreateUserDefinedOperatorSymbol(this, operatorDeclarationSyntax, declaringCompilation.IsNullableAnalysisEnabledIn(operatorDeclarationSyntax), diagnostics);
                            builder.NonTypeMembers.Add(item3);
                            break;
                        }
                    case SyntaxKind.GlobalStatement:
                        {
                            StatementSyntax statement = ((GlobalStatementSyntax)current).Statement;
                            if (IsScriptClass)
                            {
                                StatementSyntax statementSyntax = statement;
                                while (statementSyntax.Kind() == SyntaxKind.LabeledStatement)
                                {
                                    statementSyntax = ((LabeledStatementSyntax)statementSyntax).Statement;
                                }
                                switch (statementSyntax.Kind())
                                {
                                    case SyntaxKind.LocalDeclarationStatement:
                                        {
                                            SeparatedSyntaxList<VariableDeclaratorSyntax>.Enumerator enumerator2 = ((LocalDeclarationStatementSyntax)statementSyntax).Declaration.Variables.GetEnumerator();
                                            while (enumerator2.MoveNext())
                                            {
                                                VariableDeclaratorSyntax current2 = enumerator2.Current;
                                                ExpressionFieldFinder.FindExpressionVariables(builder.NonTypeMembers, current2, this, DeclarationModifiers.Private, null);
                                            }
                                            break;
                                        }
                                    case SyntaxKind.ExpressionStatement:
                                    case SyntaxKind.ReturnStatement:
                                    case SyntaxKind.YieldReturnStatement:
                                    case SyntaxKind.ThrowStatement:
                                    case SyntaxKind.LockStatement:
                                    case SyntaxKind.IfStatement:
                                    case SyntaxKind.SwitchStatement:
                                        ExpressionFieldFinder.FindExpressionVariables(builder.NonTypeMembers, statementSyntax, this, DeclarationModifiers.Private, null);
                                        break;
                                }
                                AddInitializer(ref initializers2, null, statement);
                            }
                            else if (flag && !SyntaxFacts.IsSimpleProgramTopLevelStatement((GlobalStatementSyntax)current))
                            {
                                diagnostics.Add(ErrorCode.ERR_GlobalStatement, new SourceLocation(statement));
                            }
                            break;
                        }
                }
            }
            AddInitializers(builder.InstanceInitializers, initializers2);
            AddInitializers(builder.StaticInitializers, initializers);
        }

        private void AddAccessorIfAvailable(ArrayBuilder<Symbol> symbols, MethodSymbol? accessorOpt)
        {
            if ((object)accessorOpt != null)
            {
                symbols.Add(accessorOpt);
            }
        }

        internal override byte? GetLocalNullableContextValue()
        {
            if (!_flags.TryGetNullableContext(out var value))
            {
                value = ComputeNullableContextValue();
                _flags.SetNullableContext(value);
            }
            return value;
        }

        private byte? ComputeNullableContextValue()
        {
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (!declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                return null;
            }
            MostCommonNullableValueBuilder builder = default(MostCommonNullableValueBuilder);
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            if ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                builder.AddValue(TypeWithAnnotations.Create(baseTypeNoUseSiteDiagnostics));
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = GetInterfacesToEmit().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                builder.AddValue(TypeWithAnnotations.Create(current));
            }
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator2 = TypeParameters.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                enumerator2.Current.GetCommonNullableValues(declaringCompilation, ref builder);
            }
            ImmutableArray<Symbol>.Enumerator enumerator3 = GetMembersUnordered().GetEnumerator();
            while (enumerator3.MoveNext())
            {
                enumerator3.Current.GetCommonNullableValues(declaringCompilation, ref builder);
            }
            return builder.MostCommonValue;
        }

        internal bool IsNullableEnabledForConstructorsAndInitializers(bool useStatic)
        {
            MembersAndInitializers membersAndInitializers = GetMembersAndInitializers();
            if (!useStatic)
            {
                return membersAndInitializers.IsNullableEnabledForInstanceConstructorsAndFields;
            }
            return membersAndInitializers.IsNullableEnabledForStaticConstructorsAndFields;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            if ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                if (baseTypeNoUseSiteDiagnostics.ContainsDynamic())
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDynamicAttribute(baseTypeNoUseSiteDiagnostics, 0));
                }
                if (baseTypeNoUseSiteDiagnostics.ContainsNativeInteger())
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNativeIntegerAttribute(this, baseTypeNoUseSiteDiagnostics));
                }
                if (baseTypeNoUseSiteDiagnostics.ContainsTupleNames())
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeTupleNamesAttribute(baseTypeNoUseSiteDiagnostics));
                }
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this))
            {
                if (ShouldEmitNullableContextValue(out var value))
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableContextAttribute(this, value));
                }
                if ((object)baseTypeNoUseSiteDiagnostics != null)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableAttributeIfNecessary(this, value, TypeWithAnnotations.Create(baseTypeNoUseSiteDiagnostics)));
                }
            }
        }

        internal ImmutableArray<SynthesizedExplicitImplementationForwardingMethod> GetSynthesizedExplicitImplementations(CancellationToken cancellationToken)
        {
            if (_lazySynthesizedExplicitImplementations.IsDefault)
            {
                BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    CheckMembersAgainstBaseType(instance, cancellationToken);
                    cancellationToken.ThrowIfCancellationRequested();
                    CheckAbstractClassImplementations(instance);
                    cancellationToken.ThrowIfCancellationRequested();
                    CheckInterfaceUnification(instance);
                    if (IsInterface)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        this.CheckInterfaceVarianceSafety(instance);
                    }
                    if (ImmutableInterlocked.InterlockedCompareExchange(ref _lazySynthesizedExplicitImplementations, ComputeInterfaceImplementations(instance, cancellationToken), default(ImmutableArray<SynthesizedExplicitImplementationForwardingMethod>)).IsDefault)
                    {
                        AddDeclarationDiagnostics(instance);
                        state.NotePartComplete(CompletionPart.SynthesizedExplicitImplementations);
                    }
                }
                finally
                {
                    instance.Free();
                }
            }
            return _lazySynthesizedExplicitImplementations;
        }

        private void CheckAbstractClassImplementations(BindingDiagnosticBag diagnostics)
        {
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            if (IsAbstract || (object)baseTypeNoUseSiteDiagnostics == null || !baseTypeNoUseSiteDiagnostics.IsAbstract)
            {
                return;
            }
            foreach (Symbol abstractMember in base.AbstractMembers)
            {
                if (abstractMember.Kind == SymbolKind.Method && !(abstractMember is SynthesizedRecordOrdinaryMethod))
                {
                    diagnostics.Add(ErrorCode.ERR_UnimplementedAbstractMethod, Locations[0], this, abstractMember);
                }
            }
        }

        private ImmutableArray<SynthesizedExplicitImplementationForwardingMethod> ComputeInterfaceImplementations(BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            ArrayBuilder<SynthesizedExplicitImplementationForwardingMethod> instance = ArrayBuilder<SynthesizedExplicitImplementationForwardingMethod>.GetInstance();
            MultiDictionary<NamedTypeSymbol, NamedTypeSymbol> interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics = base.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = base.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                cancellationToken.ThrowIfCancellationRequested();
                if (!interfacesAndTheirBaseInterfacesNoUseSiteDiagnostics[current].Contains(current))
                {
                    continue;
                }
                HasBaseTypeDeclaringInterfaceResult? hasBaseTypeDeclaringInterfaceResult = null;
                ImmutableArray<Symbol>.Enumerator enumerator2 = current.GetMembersUnordered().GetEnumerator();
                while (enumerator2.MoveNext())
                {
                    Symbol current2 = enumerator2.Current;
                    cancellationToken.ThrowIfCancellationRequested();
                    SymbolKind kind = current2.Kind;
                    if ((kind != SymbolKind.Event && kind != SymbolKind.Method && kind != SymbolKind.Property) || !current2.IsImplementableInterfaceMember())
                    {
                        continue;
                    }
                    SymbolAndDiagnostics symbolAndDiagnostics;
                    if (IsInterface)
                    {
                        MultiDictionary<Symbol, Symbol>.ValueSet explicitImplementationForInterfaceMember = GetExplicitImplementationForInterfaceMember(current2);
                        int count = explicitImplementationForInterfaceMember.Count;
                        if (count == 0)
                        {
                            continue;
                        }
                        if (count == 1)
                        {
                            symbolAndDiagnostics = new SymbolAndDiagnostics(explicitImplementationForInterfaceMember.Single(), ImmutableBindingDiagnostic<AssemblySymbol>.Empty);
                        }
                        else
                        {
                            Diagnostic item = new CSDiagnostic(new CSDiagnosticInfo(ErrorCode.ERR_DuplicateExplicitImpl, current2), Locations[0]);
                            symbolAndDiagnostics = new SymbolAndDiagnostics(null, new ImmutableBindingDiagnostic<AssemblySymbol>(ImmutableArray.Create(item), default(ImmutableArray<AssemblySymbol>)));
                        }
                    }
                    else
                    {
                        symbolAndDiagnostics = FindImplementationForInterfaceMemberInNonInterfaceWithDiagnostics(current2);
                    }
                    Symbol symbol = symbolAndDiagnostics.Symbol;
                    SynthesizedExplicitImplementationForwardingMethod synthesizedExplicitImplementationForwardingMethod = SynthesizeInterfaceMemberImplementation(symbolAndDiagnostics, current2);
                    bool flag = (object)symbol != null;
                    if ((object)synthesizedExplicitImplementationForwardingMethod != null)
                    {
                        if (synthesizedExplicitImplementationForwardingMethod.IsVararg)
                        {
                            diagnostics.Add(ErrorCode.ERR_InterfaceImplementedImplicitlyByVariadic, TypeSymbol.GetImplicitImplementationDiagnosticLocation(current2, this, symbol), symbol, current2, this);
                        }
                        else
                        {
                            instance.Add(synthesizedExplicitImplementationForwardingMethod);
                        }
                    }
                    if (flag && kind == SymbolKind.Event)
                    {
                        EventSymbol eventSymbol = (EventSymbol)current2;
                        EventSymbol eventSymbol2 = (EventSymbol)symbol;
                        EventSymbol eventSymbol3;
                        EventSymbol eventSymbol4;
                        if (eventSymbol.IsWindowsRuntimeEvent)
                        {
                            eventSymbol3 = eventSymbol;
                            eventSymbol4 = eventSymbol2;
                        }
                        else
                        {
                            eventSymbol3 = eventSymbol2;
                            eventSymbol4 = eventSymbol;
                        }
                        if (eventSymbol.IsWindowsRuntimeEvent != eventSymbol2.IsWindowsRuntimeEvent)
                        {
                            object[] args = new object[4] { eventSymbol2, eventSymbol, eventSymbol3, eventSymbol4 };
                            CSDiagnosticInfo info = new CSDiagnosticInfo(ErrorCode.ERR_MixingWinRTEventWithRegular, args, ImmutableArray<Symbol>.Empty, ImmutableArray.Create(Locations[0]));
                            diagnostics.Add(info, eventSymbol2.Locations[0]);
                        }
                    }
                    Symbol symbol2 = ((kind == SymbolKind.Method) ? ((MethodSymbol)current2).AssociatedSymbol : null);
                    if ((object)symbol2 != null && !ReportAccessorOfInterfacePropertyOrEvent(symbol2) && (!flag || symbol.IsAccessor()))
                    {
                        continue;
                    }
                    bool flag2 = false;
                    if (symbolAndDiagnostics.Diagnostics.Diagnostics.Any())
                    {
                        diagnostics.AddRange(symbolAndDiagnostics.Diagnostics);
                        flag2 = symbolAndDiagnostics.Diagnostics.Diagnostics.Any((Diagnostic d) => d.Severity == DiagnosticSeverity.Error);
                    }
                    if (flag2)
                    {
                        continue;
                    }
                    if (!flag || (!symbol.ContainingType.Equals(this, TypeCompareKind.ConsiderEverything) && symbol.GetExplicitInterfaceImplementations().Contains(current2, ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance)))
                    {
                        hasBaseTypeDeclaringInterfaceResult = hasBaseTypeDeclaringInterfaceResult ?? HasBaseClassDeclaringInterface(current);
                        HasBaseTypeDeclaringInterfaceResult matchResult = hasBaseTypeDeclaringInterfaceResult.GetValueOrDefault();
                        if (matchResult != HasBaseTypeDeclaringInterfaceResult.ExactMatch && flag && symbol.ContainingType.IsInterface)
                        {
                            HasBaseInterfaceDeclaringInterface(symbol.ContainingType, current, ref matchResult);
                        }
                        switch (matchResult)
                        {
                            case HasBaseTypeDeclaringInterfaceResult.NoMatch:
                                if (!current2.MustCallMethodsDirectly() && !current2.IsIndexedProperty())
                                {
                                    DiagnosticInfo diagnosticInfo = current2.GetUseSiteInfo().DiagnosticInfo;
                                    if (diagnosticInfo != null && diagnosticInfo.DefaultSeverity == DiagnosticSeverity.Error)
                                    {
                                        diagnostics.Add(diagnosticInfo, GetImplementsLocationOrFallback(current));
                                        break;
                                    }
                                    diagnostics.Add(ErrorCode.ERR_UnimplementedInterfaceMember, GetImplementsLocationOrFallback(current), this, current2);
                                }
                                break;
                            case HasBaseTypeDeclaringInterfaceResult.IgnoringNullableMatch:
                                diagnostics.Add(ErrorCode.WRN_NullabilityMismatchInInterfaceImplementedByBase, GetImplementsLocationOrFallback(current), this, current2);
                                break;
                            default:
                                throw ExceptionUtilities.UnexpectedValue(matchResult);
                            case HasBaseTypeDeclaringInterfaceResult.ExactMatch:
                                break;
                        }
                    }
                    if (flag && kind == SymbolKind.Method && ((object)synthesizedExplicitImplementationForwardingMethod != null || TypeSymbol.Equals(symbol.ContainingType, this, TypeCompareKind.ConsiderEverything)))
                    {
                        UseSiteInfo<AssemblySymbol> useSiteInfo = current2.GetUseSiteInfo();
                        Location location = (symbol.IsFromCompilation(DeclaringCompilation) ? symbol.Locations[0] : Locations[0]);
                        diagnostics.Add(useSiteInfo, location);
                    }
                }
            }
            return instance.ToImmutableAndFree();
        }

        protected abstract Location GetCorrespondingBaseListLocation(NamedTypeSymbol @base);

        private Location GetImplementsLocationOrFallback(NamedTypeSymbol implementedInterface)
        {
            return GetImplementsLocation(implementedInterface) ?? Locations[0];
        }

        internal Location? GetImplementsLocation(NamedTypeSymbol implementedInterface)
        {
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
            NamedTypeSymbol namedTypeSymbol = null;
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = InterfacesNoUseSiteDiagnostics().GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if (TypeSymbol.Equals(current, implementedInterface, TypeCompareKind.ConsiderEverything))
                {
                    namedTypeSymbol = current;
                    break;
                }
                if ((object)namedTypeSymbol == null && current.ImplementsInterface(implementedInterface, ref useSiteInfo))
                {
                    namedTypeSymbol = current;
                }
            }
            return GetCorrespondingBaseListLocation(namedTypeSymbol);
        }

        private bool ReportAccessorOfInterfacePropertyOrEvent(Symbol interfacePropertyOrEvent)
        {
            if (interfacePropertyOrEvent.IsIndexedProperty())
            {
                return true;
            }
            Symbol symbol;
            if (IsInterface)
            {
                MultiDictionary<Symbol, Symbol>.ValueSet explicitImplementationForInterfaceMember = GetExplicitImplementationForInterfaceMember(interfacePropertyOrEvent);
                switch (explicitImplementationForInterfaceMember.Count)
                {
                    case 0:
                        return true;
                    case 1:
                        symbol = explicitImplementationForInterfaceMember.Single();
                        break;
                    default:
                        symbol = null;
                        break;
                }
            }
            else
            {
                symbol = FindImplementationForInterfaceMemberInNonInterface(interfacePropertyOrEvent);
            }
            if ((object)symbol == null)
            {
                return false;
            }
            if (interfacePropertyOrEvent.Kind == SymbolKind.Event && symbol.Kind == SymbolKind.Event && ((EventSymbol)interfacePropertyOrEvent).IsWindowsRuntimeEvent != ((EventSymbol)symbol).IsWindowsRuntimeEvent)
            {
                return false;
            }
            return true;
        }

        private HasBaseTypeDeclaringInterfaceResult HasBaseClassDeclaringInterface(NamedTypeSymbol @interface)
        {
            HasBaseTypeDeclaringInterfaceResult result = HasBaseTypeDeclaringInterfaceResult.NoMatch;
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
            while ((object)baseTypeNoUseSiteDiagnostics != null && !DeclaresBaseInterface(baseTypeNoUseSiteDiagnostics, @interface, ref result))
            {
                baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
            }
            return result;
        }

        private static bool DeclaresBaseInterface(NamedTypeSymbol currType, NamedTypeSymbol @interface, ref HasBaseTypeDeclaringInterfaceResult result)
        {
            MultiDictionary<NamedTypeSymbol, NamedTypeSymbol>.ValueSet valueSet = currType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics[@interface];
            if (valueSet.Count != 0)
            {
                if (valueSet.Contains(@interface))
                {
                    result = HasBaseTypeDeclaringInterfaceResult.ExactMatch;
                    return true;
                }
                if (result == HasBaseTypeDeclaringInterfaceResult.NoMatch && valueSet.Contains(@interface, SymbolEqualityComparer.IgnoringNullable))
                {
                    result = HasBaseTypeDeclaringInterfaceResult.IgnoringNullableMatch;
                }
            }
            return false;
        }

        private void HasBaseInterfaceDeclaringInterface(NamedTypeSymbol baseInterface, NamedTypeSymbol @interface, ref HasBaseTypeDeclaringInterfaceResult matchResult)
        {
            if (DeclaresBaseInterface(baseInterface, @interface, ref matchResult))
            {
                return;
            }
            ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = base.AllInterfacesNoUseSiteDiagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                NamedTypeSymbol current = enumerator.Current;
                if ((object)current != baseInterface && current.Equals(baseInterface, TypeCompareKind.CLRSignatureCompareOptions) && DeclaresBaseInterface(current, @interface, ref matchResult))
                {
                    break;
                }
            }
        }

        private void CheckMembersAgainstBaseType(BindingDiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            switch (TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Enum:
                    break;
                default:
                    throw ExceptionUtilities.UnexpectedValue(TypeKind);
                case TypeKind.Class:
                case TypeKind.Interface:
                case TypeKind.Struct:
                case TypeKind.Submission:
                    {
                        ImmutableArray<Symbol>.Enumerator enumerator = GetMembersUnordered().GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            Symbol current = enumerator.Current;
                            cancellationToken.ThrowIfCancellationRequested();
                            bool suppressAccessors;
                            switch (current.Kind)
                            {
                                case SymbolKind.Method:
                                    {
                                        MethodSymbol methodSymbol = (MethodSymbol)current;
                                        if (MethodSymbol.CanOverrideOrHide(methodSymbol.MethodKind) && !methodSymbol.IsAccessor())
                                        {
                                            if (current.IsOverride)
                                            {
                                                CheckOverrideMember(methodSymbol, methodSymbol.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            }
                                            else if (methodSymbol is SourceMemberMethodSymbol sourceMemberMethodSymbol)
                                            {
                                                bool isNew3 = sourceMemberMethodSymbol.IsNew;
                                                CheckNonOverrideMember(methodSymbol, isNew3, methodSymbol.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            }
                                        }
                                        else if (methodSymbol.MethodKind == MethodKind.Destructor)
                                        {
                                            MethodSymbol firstRuntimeOverriddenMethodIgnoringNewSlot = methodSymbol.GetFirstRuntimeOverriddenMethodIgnoringNewSlot(out bool wasAmbiguous);
                                            if ((object)firstRuntimeOverriddenMethodIgnoringNewSlot != null && firstRuntimeOverriddenMethodIgnoringNewSlot.IsMetadataFinal)
                                            {
                                                diagnostics.Add(ErrorCode.ERR_CantOverrideSealed, methodSymbol.Locations[0], methodSymbol, firstRuntimeOverriddenMethodIgnoringNewSlot);
                                            }
                                        }
                                        break;
                                    }
                                case SymbolKind.Property:
                                    {
                                        PropertySymbol propertySymbol = (PropertySymbol)current;
                                        MethodSymbol getMethod = propertySymbol.GetMethod;
                                        MethodSymbol setMethod = propertySymbol.SetMethod;
                                        if (current.IsOverride)
                                        {
                                            CheckOverrideMember(propertySymbol, propertySymbol.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            if (!suppressAccessors)
                                            {
                                                if ((object)getMethod != null)
                                                {
                                                    CheckOverrideMember(getMethod, getMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                                }
                                                if ((object)setMethod != null)
                                                {
                                                    CheckOverrideMember(setMethod, setMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (!(propertySymbol is SourcePropertySymbolBase sourcePropertySymbolBase))
                                            {
                                                break;
                                            }
                                            bool isNew2 = sourcePropertySymbolBase.IsNew;
                                            CheckNonOverrideMember(propertySymbol, isNew2, propertySymbol.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            if (!suppressAccessors)
                                            {
                                                if ((object)getMethod != null)
                                                {
                                                    CheckNonOverrideMember(getMethod, isNew2, getMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                                }
                                                if ((object)setMethod != null)
                                                {
                                                    CheckNonOverrideMember(setMethod, isNew2, setMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                                }
                                            }
                                        }
                                        break;
                                    }
                                case SymbolKind.Event:
                                    {
                                        EventSymbol eventSymbol = (EventSymbol)current;
                                        MethodSymbol addMethod = eventSymbol.AddMethod;
                                        MethodSymbol removeMethod = eventSymbol.RemoveMethod;
                                        if (current.IsOverride)
                                        {
                                            CheckOverrideMember(eventSymbol, eventSymbol.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            if (!suppressAccessors)
                                            {
                                                if ((object)addMethod != null)
                                                {
                                                    CheckOverrideMember(addMethod, addMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                                }
                                                if ((object)removeMethod != null)
                                                {
                                                    CheckOverrideMember(removeMethod, removeMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                                }
                                            }
                                            break;
                                        }
                                        bool isNew4 = ((SourceEventSymbol)eventSymbol).IsNew;
                                        CheckNonOverrideMember(eventSymbol, isNew4, eventSymbol.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                        if (!suppressAccessors)
                                        {
                                            if ((object)addMethod != null)
                                            {
                                                CheckNonOverrideMember(addMethod, isNew4, addMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            }
                                            if ((object)removeMethod != null)
                                            {
                                                CheckNonOverrideMember(removeMethod, isNew4, removeMethod.OverriddenOrHiddenMembers, diagnostics, out suppressAccessors);
                                            }
                                        }
                                        break;
                                    }
                                case SymbolKind.Field:
                                    {
                                        bool isNew = current is SourceFieldSymbol sourceFieldSymbol && sourceFieldSymbol.IsNew;
                                        CheckNewModifier(current, isNew, diagnostics);
                                        break;
                                    }
                                case SymbolKind.NamedType:
                                    CheckNewModifier(current, ((SourceMemberContainerTypeSymbol)current).IsNew, diagnostics);
                                    break;
                            }
                        }
                        break;
                    }
            }
        }

        private void CheckNewModifier(Symbol symbol, bool isNew, BindingDiagnosticBag diagnostics)
        {
            if (symbol.IsImplicitlyDeclared)
            {
                return;
            }
            if (symbol.ContainingType.IsInterface)
            {
                CheckNonOverrideMember(symbol, isNew, OverriddenOrHiddenMembersHelpers.MakeInterfaceOverriddenOrHiddenMembers(symbol, memberIsFromSomeCompilation: true), diagnostics, out var _);
            }
            else
            {
                if ((object)BaseTypeNoUseSiteDiagnostics == null)
                {
                    return;
                }
                int memberArity = symbol.GetMemberArity();
                Location location = symbol.Locations.FirstOrDefault();
                bool suppressAccessors2 = false;
                NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
                while ((object)baseTypeNoUseSiteDiagnostics != null)
                {
                    ImmutableArray<Symbol>.Enumerator enumerator = baseTypeNoUseSiteDiagnostics.GetMembers(symbol.Name).GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Symbol current = enumerator.Current;
                        if (current.Kind == SymbolKind.Method && !((MethodSymbol)current).CanBeHiddenByMemberKind(symbol.Kind))
                        {
                            continue;
                        }
                        CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
                        bool num = AccessCheck.IsSymbolAccessible(current, this, ref useSiteInfo);
                        diagnostics.Add(location, useSiteInfo);
                        if (num && current.GetMemberArity() == memberArity)
                        {
                            if (!isNew)
                            {
                                diagnostics.Add(ErrorCode.WRN_NewRequired, location, symbol, current);
                            }
                            AddHidingAbstractDiagnostic(symbol, location, current, diagnostics, ref suppressAccessors2);
                            return;
                        }
                    }
                    baseTypeNoUseSiteDiagnostics = baseTypeNoUseSiteDiagnostics.BaseTypeNoUseSiteDiagnostics;
                }
                if (isNew)
                {
                    diagnostics.Add(ErrorCode.WRN_NewNotRequired, location, symbol);
                }
            }
        }

        private void CheckOverrideMember(Symbol overridingMember, OverriddenOrHiddenMembersResult overriddenOrHiddenMembers, BindingDiagnosticBag diagnostics, out bool suppressAccessors)
        {
            suppressAccessors = false;
            bool flag = overridingMember.Kind == SymbolKind.Method;
            bool flag2 = overridingMember.Kind == SymbolKind.Property;
            _ = overridingMember.Kind;
            Location location2 = overridingMember.Locations[0];
            ImmutableArray<Symbol> overriddenMembers = overriddenOrHiddenMembers.OverriddenMembers;
            if (overriddenMembers.Length == 0)
            {
                ImmutableArray<Symbol> hiddenMembers = overriddenOrHiddenMembers.HiddenMembers;
                if (hiddenMembers.Any())
                {
                    ErrorCode code = (flag ? ErrorCode.ERR_CantOverrideNonFunction : (flag2 ? ErrorCode.ERR_CantOverrideNonProperty : ErrorCode.ERR_CantOverrideNonEvent));
                    diagnostics.Add(code, location2, overridingMember, hiddenMembers[0]);
                }
                else
                {
                    Symbol symbol = null;
                    if (flag)
                    {
                        symbol = ((MethodSymbol)overridingMember).AssociatedSymbol;
                    }
                    if ((object)symbol == null)
                    {
                        bool flag3 = false;
                        if (flag || overridingMember.IsIndexer())
                        {
                            ImmutableArray<TypeWithAnnotations>.Enumerator enumerator = (flag ? ((MethodSymbol)overridingMember).ParameterTypesWithAnnotations : ((PropertySymbol)overridingMember).ParameterTypesWithAnnotations).GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                if (IsOrContainsErrorType(enumerator.Current.Type))
                                {
                                    flag3 = true;
                                    break;
                                }
                            }
                        }
                        if (!flag3)
                        {
                            diagnostics.Add(ErrorCode.ERR_OverrideNotExpected, location2, overridingMember);
                        }
                    }
                    else if (symbol.Kind == SymbolKind.Property)
                    {
                        PropertySymbol propertySymbol = (PropertySymbol)symbol;
                        PropertySymbol overriddenProperty2 = propertySymbol.OverriddenProperty;
                        if ((object)overriddenProperty2 != null)
                        {
                            if (propertySymbol.GetMethod == overridingMember && (object)overriddenProperty2.GetMethod == null)
                            {
                                diagnostics.Add(ErrorCode.ERR_NoGetToOverride, location2, overridingMember, overriddenProperty2);
                            }
                            else if (propertySymbol.SetMethod == overridingMember && (object)overriddenProperty2.SetMethod == null)
                            {
                                diagnostics.Add(ErrorCode.ERR_NoSetToOverride, location2, overridingMember, overriddenProperty2);
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_OverrideNotExpected, location2, overridingMember);
                            }
                        }
                    }
                }
            }
            else
            {
                NamedTypeSymbol containingType = overridingMember.ContainingType;
                if (overriddenMembers.Length > 1)
                {
                    diagnostics.Add(ErrorCode.ERR_AmbigOverride, location2, overriddenMembers[0].OriginalDefinition, overriddenMembers[1].OriginalDefinition, containingType);
                    suppressAccessors = true;
                }
                else
                {
                    checkSingleOverriddenMember(overridingMember, overriddenMembers[0], diagnostics, ref suppressAccessors);
                }
            }
            if (!ContainingAssembly.RuntimeSupportsCovariantReturnsOfClasses && overridingMember is MethodSymbol methodSymbol)
            {
                methodSymbol.RequiresExplicitOverride(out var warnAmbiguous);
                if (warnAmbiguous)
                {
                    MethodSymbol overriddenMethod2 = methodSymbol.OverriddenMethod;
                    diagnostics.Add(ErrorCode.WRN_MultipleRuntimeOverrideMatches, overriddenMethod2.Locations[0], overriddenMethod2, overridingMember);
                    suppressAccessors = true;
                }
            }
            void checkSingleOverriddenMember(Symbol overridingMember, Symbol overriddenMember, BindingDiagnosticBag diagnostics, ref bool suppressAccessors)
            {
                Location location3 = overridingMember.Locations[0];
                bool flag4 = overridingMember.Kind == SymbolKind.Method;
                bool flag5 = overridingMember.Kind == SymbolKind.Property;
                bool flag6 = overridingMember.Kind == SymbolKind.Event;
                _ = overridingMember.ContainingType;
                if (overriddenMember.MustCallMethodsDirectly())
                {
                    diagnostics.Add(ErrorCode.ERR_CantOverrideBogusMethod, location3, overridingMember, overriddenMember);
                    suppressAccessors = true;
                }
                else if (!overriddenMember.IsVirtual && !overriddenMember.IsAbstract && !overriddenMember.IsOverride && (!flag4 || ((MethodSymbol)overriddenMember).MethodKind != MethodKind.Destructor))
                {
                    diagnostics.Add(ErrorCode.ERR_CantOverrideNonVirtual, location3, overridingMember, overriddenMember);
                    suppressAccessors = true;
                }
                else if (overriddenMember.IsSealed)
                {
                    diagnostics.Add(ErrorCode.ERR_CantOverrideSealed, location3, overridingMember, overriddenMember);
                    suppressAccessors = true;
                }
                else if (!OverrideHasCorrectAccessibility(overriddenMember, overridingMember))
                {
                    string text = SyntaxFacts.GetText(overriddenMember.DeclaredAccessibility);
                    diagnostics.Add(ErrorCode.ERR_CantChangeAccessOnOverride, location3, overridingMember, text, overriddenMember);
                    suppressAccessors = true;
                }
                else if (overridingMember.ContainsTupleNames() && MemberSignatureComparer.ConsideringTupleNamesCreatesDifference(overridingMember, overriddenMember))
                {
                    diagnostics.Add(ErrorCode.ERR_CantChangeTupleNamesOnOverride, location3, overridingMember, overriddenMember);
                }
                else
                {
                    Symbol leastOverriddenMember = overriddenMember.GetLeastOverriddenMember(overriddenMember.ContainingType);
                    overridingMember.ForceCompleteObsoleteAttribute();
                    leastOverriddenMember.ForceCompleteObsoleteAttribute();
                    bool flag7 = overridingMember.ObsoleteState == ThreeState.True;
                    bool flag8 = leastOverriddenMember.ObsoleteState == ThreeState.True;
                    if (flag7 != flag8)
                    {
                        ErrorCode code2 = (flag7 ? ErrorCode.WRN_ObsoleteOverridingNonObsolete : ErrorCode.WRN_NonObsoleteOverridingObsolete);
                        diagnostics.Add(code2, location3, overridingMember, leastOverriddenMember);
                    }
                    if (flag5)
                    {
                        checkOverriddenProperty((PropertySymbol)overridingMember, (PropertySymbol)overriddenMember, diagnostics, ref suppressAccessors);
                    }
                    else if (flag6)
                    {
                        EventSymbol eventSymbol = (EventSymbol)overridingMember;
                        EventSymbol eventSymbol2 = (EventSymbol)overriddenMember;
                        TypeWithAnnotations typeWithAnnotations = eventSymbol.TypeWithAnnotations;
                        TypeWithAnnotations typeWithAnnotations2 = eventSymbol2.TypeWithAnnotations;
                        if (!typeWithAnnotations.Equals(typeWithAnnotations2, TypeCompareKind.AllIgnoreOptions))
                        {
                            if (!IsOrContainsErrorType(typeWithAnnotations.Type))
                            {
                                diagnostics.Add(ErrorCode.ERR_CantChangeTypeOnOverride, location3, overridingMember, overriddenMember, typeWithAnnotations2.Type);
                            }
                            suppressAccessors = true;
                        }
                        else
                        {
                            CheckValidNullableEventOverride(eventSymbol.DeclaringCompilation, eventSymbol2, eventSymbol, diagnostics, delegate (BindingDiagnosticBag diagnostics, EventSymbol overriddenEvent, EventSymbol overridingEvent, Location location)
                            {
                                diagnostics.Add(ErrorCode.WRN_NullabilityMismatchInTypeOnOverride, location);
                            }, location3);
                        }
                    }
                    else
                    {
                        MethodSymbol methodSymbol2 = (MethodSymbol)overridingMember;
                        MethodSymbol methodSymbol3 = (MethodSymbol)overriddenMember;
                        if (methodSymbol2.IsGenericMethod)
                        {
                            methodSymbol3 = methodSymbol3.Construct(TypeMap.TypeParametersAsTypeSymbolsWithIgnoredAnnotations(methodSymbol2.TypeParameters));
                        }
                        if (methodSymbol2.RefKind != methodSymbol3.RefKind)
                        {
                            diagnostics.Add(ErrorCode.ERR_CantChangeRefReturnOnOverride, location3, overridingMember, overriddenMember);
                        }
                        else if (!IsValidOverrideReturnType(methodSymbol2, methodSymbol2.ReturnTypeWithAnnotations, methodSymbol3.ReturnTypeWithAnnotations, diagnostics))
                        {
                            if (!IsOrContainsErrorType(methodSymbol2.ReturnType))
                            {
                                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                                if (DeclaringCompilation.Conversions.HasIdentityOrImplicitReferenceConversion(methodSymbol2.ReturnTypeWithAnnotations.Type, methodSymbol3.ReturnTypeWithAnnotations.Type, ref useSiteInfo))
                                {
                                    if (!methodSymbol2.ContainingAssembly.RuntimeSupportsCovariantReturnsOfClasses)
                                    {
                                        diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportCovariantReturnsOfClasses, location3, overridingMember, overriddenMember, methodSymbol3.ReturnType);
                                    }
                                    else
                                    {
                                        CSDiagnosticInfo featureAvailabilityDiagnosticInfo = MessageID.IDS_FeatureCovariantReturnsForOverrides.GetFeatureAvailabilityDiagnosticInfo(DeclaringCompilation);
                                        if (featureAvailabilityDiagnosticInfo == null)
                                        {
                                            throw ExceptionUtilities.Unreachable;
                                        }
                                        diagnostics.Add(featureAvailabilityDiagnosticInfo, location3);
                                    }
                                }
                                else
                                {
                                    diagnostics.Add(ErrorCode.ERR_CantChangeReturnTypeOnOverride, location3, overridingMember, overriddenMember, methodSymbol3.ReturnType);
                                }
                            }
                        }
                        else if (methodSymbol3.IsRuntimeFinalizer())
                        {
                            diagnostics.Add(ErrorCode.ERR_OverrideFinalizeDeprecated, location3);
                        }
                        else if (!methodSymbol2.IsAccessor())
                        {
                            checkValidNullableMethodOverride(location3, methodSymbol3, methodSymbol2, diagnostics, checkReturnType: true, checkParameters: true);
                        }
                    }
                    if (Binder.ReportUseSite(overriddenMember, diagnostics, overridingMember.Locations[0]))
                    {
                        suppressAccessors = true;
                    }
                }
                void checkOverriddenProperty(PropertySymbol overridingProperty, PropertySymbol overriddenProperty, BindingDiagnosticBag diagnostics, ref bool suppressAccessors)
                {
                    Location location4 = overridingProperty.Locations[0];
                    NamedTypeSymbol containingType2 = overridingProperty.ContainingType;
                    TypeWithAnnotations typeWithAnnotations3 = overridingProperty.TypeWithAnnotations;
                    TypeWithAnnotations typeWithAnnotations4 = overriddenProperty.TypeWithAnnotations;
                    if (overridingProperty.RefKind != overriddenProperty.RefKind)
                    {
                        diagnostics.Add(ErrorCode.ERR_CantChangeRefReturnOnOverride, location4, overridingProperty, overriddenProperty);
                        suppressAccessors = true;
                    }
                    else if (((object)overridingProperty.SetMethod == null) ? (!IsValidOverrideReturnType(overridingProperty, typeWithAnnotations3, typeWithAnnotations4, diagnostics)) : (!typeWithAnnotations3.Equals(typeWithAnnotations4, TypeCompareKind.AllIgnoreOptions)))
                    {
                        if (!IsOrContainsErrorType(typeWithAnnotations3.Type))
                        {
                            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo2 = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                            if ((object)overridingProperty.SetMethod == null && DeclaringCompilation.Conversions.HasIdentityOrImplicitReferenceConversion(typeWithAnnotations3.Type, typeWithAnnotations4.Type, ref useSiteInfo2))
                            {
                                if (!overridingProperty.ContainingAssembly.RuntimeSupportsCovariantReturnsOfClasses)
                                {
                                    diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportCovariantPropertiesOfClasses, location4, overridingMember, overriddenMember, typeWithAnnotations4.Type);
                                }
                                else
                                {
                                    CSDiagnosticInfo featureAvailabilityDiagnosticInfo2 = MessageID.IDS_FeatureCovariantReturnsForOverrides.GetFeatureAvailabilityDiagnosticInfo(DeclaringCompilation);
                                    diagnostics.Add(featureAvailabilityDiagnosticInfo2, location4);
                                }
                            }
                            else
                            {
                                diagnostics.Add(ErrorCode.ERR_CantChangeTypeOnOverride, location4, overridingMember, overriddenMember, typeWithAnnotations4.Type);
                            }
                        }
                        suppressAccessors = true;
                    }
                    else
                    {
                        if ((object)overridingProperty.GetMethod != null)
                        {
                            MethodSymbol ownOrInheritedGetMethod = overriddenProperty.GetOwnOrInheritedGetMethod();
                            checkValidNullableMethodOverride(overridingProperty.GetMethod.Locations[0], ownOrInheritedGetMethod, overridingProperty.GetMethod, diagnostics, checkReturnType: true, (object)overridingProperty.SetMethod == null || ownOrInheritedGetMethod?.AssociatedSymbol != overriddenProperty || overriddenProperty.GetOwnOrInheritedSetMethod()?.AssociatedSymbol != overriddenProperty);
                        }
                        if ((object)overridingProperty.SetMethod != null)
                        {
                            MethodSymbol ownOrInheritedSetMethod = overriddenProperty.GetOwnOrInheritedSetMethod();
                            checkValidNullableMethodOverride(overridingProperty.SetMethod.Locations[0], ownOrInheritedSetMethod, overridingProperty.SetMethod, diagnostics, checkReturnType: false, checkParameters: true);
                            if ((object)ownOrInheritedSetMethod != null && overridingProperty.SetMethod.IsInitOnly != ownOrInheritedSetMethod.IsInitOnly)
                            {
                                diagnostics.Add(ErrorCode.ERR_CantChangeInitOnlyOnOverride, location4, overridingProperty, overriddenProperty);
                            }
                        }
                    }
                    if (overridingProperty.IsSealed)
                    {
                        MethodSymbol ownOrInheritedGetMethod2 = overridingProperty.GetOwnOrInheritedGetMethod();
                        CompoundUseSiteInfo<AssemblySymbol> useSiteInfo3 = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, overridingProperty.ContainingAssembly);
                        if (overridingProperty.GetMethod != ownOrInheritedGetMethod2 && !AccessCheck.IsSymbolAccessible(ownOrInheritedGetMethod2, containingType2, ref useSiteInfo3))
                        {
                            diagnostics.Add(ErrorCode.ERR_NoGetToOverride, location4, overridingProperty, overriddenProperty);
                        }
                        MethodSymbol ownOrInheritedSetMethod2 = overridingProperty.GetOwnOrInheritedSetMethod();
                        if (overridingProperty.SetMethod != ownOrInheritedSetMethod2 && !AccessCheck.IsSymbolAccessible(ownOrInheritedSetMethod2, containingType2, ref useSiteInfo3))
                        {
                            diagnostics.Add(ErrorCode.ERR_NoSetToOverride, location4, overridingProperty, overriddenProperty);
                        }
                        diagnostics.Add(location4, useSiteInfo3);
                    }
                }
            }
            static void checkValidNullableMethodOverride(Location overridingMemberLocation, MethodSymbol overriddenMethod, MethodSymbol overridingMethod, BindingDiagnosticBag diagnostics, bool checkReturnType, bool checkParameters)
            {
                CheckValidNullableMethodOverride(overridingMethod.DeclaringCompilation, overriddenMethod, overridingMethod, diagnostics, checkReturnType ? ReportBadReturn : null, checkParameters ? ReportBadParameter : null, overridingMemberLocation);
            }
        }

        internal static bool IsOrContainsErrorType(TypeSymbol typeSymbol)
        {
            return (object)typeSymbol.VisitType((TypeSymbol currentTypeSymbol, object unused1, bool unused2) => currentTypeSymbol.IsErrorType(), null) != null;
        }

        private bool IsValidOverrideReturnType(Symbol overridingSymbol, TypeWithAnnotations overridingReturnType, TypeWithAnnotations overriddenReturnType, BindingDiagnosticBag diagnostics)
        {
            if (overridingSymbol.ContainingAssembly.RuntimeSupportsCovariantReturnsOfClasses && DeclaringCompilation.LanguageVersion >= MessageID.IDS_FeatureCovariantReturnsForOverrides.RequiredVersion())
            {
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
                bool result = DeclaringCompilation.Conversions.HasIdentityOrImplicitReferenceConversion(overridingReturnType.Type, overriddenReturnType.Type, ref useSiteInfo);
                Location location = overridingSymbol.Locations.FirstOrDefault();
                diagnostics.Add(location, useSiteInfo);
                return result;
            }
            return overridingReturnType.Equals(overriddenReturnType, TypeCompareKind.AllIgnoreOptions);
        }

        internal static void CheckValidNullableMethodOverride<TArg>(CSharpCompilation compilation, MethodSymbol baseMethod, MethodSymbol overrideMethod, BindingDiagnosticBag diagnostics, ReportMismatchInReturnType<TArg> reportMismatchInReturnType, ReportMismatchInParameterType<TArg> reportMismatchInParameterType, TArg extraArgument, bool invokedAsExtensionMethod = false)
        {
            if (!PerformValidNullableOverrideCheck(compilation, baseMethod, overrideMethod))
            {
                return;
            }
            if ((baseMethod.FlowAnalysisAnnotations & FlowAnalysisAnnotations.DoesNotReturn) == FlowAnalysisAnnotations.DoesNotReturn && (overrideMethod.FlowAnalysisAnnotations & FlowAnalysisAnnotations.DoesNotReturn) != FlowAnalysisAnnotations.DoesNotReturn)
            {
                diagnostics.Add(ErrorCode.WRN_DoesNotReturnMismatch, overrideMethod.Locations[0], new FormattedSymbol(overrideMethod, SymbolDisplayFormat.MinimallyQualifiedFormat));
            }
            ConversionsBase conversions2 = compilation.Conversions.WithNullability(includeNullability: true);
            ImmutableArray<ParameterSymbol> baseParameters = baseMethod.Parameters;
            ImmutableArray<ParameterSymbol> overrideParameters = overrideMethod.Parameters;
            int overrideParameterOffset = (invokedAsExtensionMethod ? 1 : 0);
            if (reportMismatchInReturnType != null)
            {
                TypeWithAnnotations overridingType = getNotNullIfNotNullOutputType(overrideMethod.ReturnTypeWithAnnotations, overrideMethod.ReturnNotNullIfParameterNotNull);
                if (!isValidNullableConversion(conversions2, overrideMethod.RefKind, overridingType.Type, baseMethod.ReturnTypeWithAnnotations.Type))
                {
                    reportMismatchInReturnType(diagnostics, baseMethod, overrideMethod, topLevel: false, extraArgument);
                    return;
                }
                if (!NullableWalker.AreParameterAnnotationsCompatible((overrideMethod.RefKind == RefKind.Ref) ? RefKind.Ref : RefKind.Out, baseMethod.ReturnTypeWithAnnotations, baseMethod.ReturnTypeFlowAnalysisAnnotations, overridingType, overrideMethod.ReturnTypeFlowAnalysisAnnotations))
                {
                    reportMismatchInReturnType(diagnostics, baseMethod, overrideMethod, topLevel: true, extraArgument);
                    return;
                }
            }
            if (reportMismatchInParameterType == null)
            {
                return;
            }
            for (int i = 0; i < baseParameters.Length; i++)
            {
                ParameterSymbol parameterSymbol = baseParameters[i];
                TypeWithAnnotations typeWithAnnotations = parameterSymbol.TypeWithAnnotations;
                ParameterSymbol parameterSymbol2 = overrideParameters[i + overrideParameterOffset];
                TypeWithAnnotations overridingType2 = getNotNullIfNotNullOutputType(parameterSymbol2.TypeWithAnnotations, parameterSymbol2.NotNullIfParameterNotNull);
                if (!isValidNullableConversion(conversions2, parameterSymbol2.RefKind, typeWithAnnotations.Type, overridingType2.Type))
                {
                    reportMismatchInParameterType(diagnostics, baseMethod, overrideMethod, parameterSymbol2, topLevel: false, extraArgument);
                }
                else if (!NullableWalker.AreParameterAnnotationsCompatible(parameterSymbol2.RefKind, typeWithAnnotations, parameterSymbol.FlowAnalysisAnnotations, overridingType2, parameterSymbol2.FlowAnalysisAnnotations))
                {
                    reportMismatchInParameterType(diagnostics, baseMethod, overrideMethod, parameterSymbol2, topLevel: true, extraArgument);
                }
            }
            TypeWithAnnotations getNotNullIfNotNullOutputType(TypeWithAnnotations outputType, ImmutableHashSet<string> notNullIfParameterNotNull)
            {
                if (!notNullIfParameterNotNull.IsEmpty)
                {
                    for (int j = 0; j < baseParameters.Length; j++)
                    {
                        ParameterSymbol parameterSymbol3 = overrideParameters[j + overrideParameterOffset];
                        if (notNullIfParameterNotNull.Contains(parameterSymbol3.Name) && !baseParameters[j].TypeWithAnnotations.NullableAnnotation.IsAnnotated())
                        {
                            return outputType.AsNotAnnotated();
                        }
                    }
                }
                return outputType;
            }
            static bool isValidNullableConversion(ConversionsBase conversions, RefKind refKind, TypeSymbol sourceType, TypeSymbol targetType)
            {
                switch (refKind)
                {
                    case RefKind.Ref:
                        return sourceType.Equals(targetType, TypeCompareKind.IgnoreDynamicAndTupleNames | TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds | TypeCompareKind.ObliviousNullableModifierMatchesAny | TypeCompareKind.IgnoreNativeIntegers);
                    case RefKind.Out:
                        {
                            TypeSymbol typeSymbol = targetType;
                            TypeSymbol typeSymbol2 = sourceType;
                            sourceType = typeSymbol;
                            targetType = typeSymbol2;
                            break;
                        }
                }
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                return conversions.ClassifyImplicitConversionFromType(sourceType, targetType, ref useSiteInfo).Kind != ConversionKind.NoConversion;
            }
        }

        private static bool PerformValidNullableOverrideCheck(CSharpCompilation compilation, Symbol overriddenMember, Symbol overridingMember)
        {
            if ((object)overriddenMember != null && (object)overridingMember != null && compilation != null)
            {
                return compilation.IsFeatureEnabled(MessageID.IDS_FeatureNullableReferenceTypes);
            }
            return false;
        }

        internal static void CheckValidNullableEventOverride<TArg>(CSharpCompilation compilation, EventSymbol overriddenEvent, EventSymbol overridingEvent, BindingDiagnosticBag diagnostics, Action<BindingDiagnosticBag, EventSymbol, EventSymbol, TArg> reportMismatch, TArg extraArgument)
        {
            if (PerformValidNullableOverrideCheck(compilation, overriddenEvent, overridingEvent) && !compilation.Conversions.WithNullability(includeNullability: true).HasAnyNullabilityImplicitConversion(overriddenEvent.TypeWithAnnotations, overridingEvent.TypeWithAnnotations))
            {
                reportMismatch(diagnostics, overriddenEvent, overridingEvent, extraArgument);
            }
        }

        private static void CheckNonOverrideMember(Symbol hidingMember, bool hidingMemberIsNew, OverriddenOrHiddenMembersResult overriddenOrHiddenMembers, BindingDiagnosticBag diagnostics, out bool suppressAccessors)
        {
            suppressAccessors = false;
            Location location = hidingMember.Locations[0];
            ImmutableArray<Symbol> hiddenMembers = overriddenOrHiddenMembers.HiddenMembers;
            if (hiddenMembers.Length == 0)
            {
                if (hidingMemberIsNew && !hidingMember.IsAccessor())
                {
                    diagnostics.Add(ErrorCode.WRN_NewNotRequired, location, hidingMember);
                }
                return;
            }
            bool flag = false;
            if (!hidingMember.ContainingType.IsInterface)
            {
                ImmutableArray<Symbol>.Enumerator enumerator = hiddenMembers.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    Symbol current = enumerator.Current;
                    flag |= AddHidingAbstractDiagnostic(hidingMember, location, current, diagnostics, ref suppressAccessors);
                    if (!hidingMemberIsNew && current.Kind == hidingMember.Kind && !hidingMember.IsAccessor() && (current.IsAbstract || current.IsVirtual || current.IsOverride) && !IsShadowingSynthesizedRecordMember(hidingMember))
                    {
                        diagnostics.Add(ErrorCode.WRN_NewOrOverrideExpected, location, hidingMember, current);
                        flag = true;
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            if (!hidingMemberIsNew && !IsShadowingSynthesizedRecordMember(hidingMember) && !flag && !hidingMember.IsAccessor() && !hidingMember.IsOperator())
            {
                diagnostics.Add(ErrorCode.WRN_NewRequired, location, hidingMember, hiddenMembers[0]);
            }
        }

        private static bool IsShadowingSynthesizedRecordMember(Symbol hidingMember)
        {
            if (!(hidingMember is SynthesizedRecordEquals) && !(hidingMember is SynthesizedRecordDeconstruct))
            {
                return hidingMember is SynthesizedRecordClone;
            }
            return true;
        }

        /// <summary>
        /// If necessary, report a diagnostic for a hidden abstract member.
        /// </summary>
        /// <returns>True if a diagnostic was reported.</returns>
        private static bool AddHidingAbstractDiagnostic(Symbol hidingMember, Location hidingMemberLocation, Symbol hiddenMember, BindingDiagnosticBag diagnostics, ref bool suppressAccessors)
        {
            switch (hiddenMember.Kind)
            {
                case SymbolKind.Method:
                case SymbolKind.Property:
                case SymbolKind.Event:
                    break; // Can result in diagnostic
                default:
                    return false; // Cannot result in diagnostic
            }

            // If the hidden member isn't abstract, the diagnostic doesn't apply.
            // If the hiding member is in a non-abstract type, then suppress this cascading error.
            if (!hiddenMember.IsAbstract || !hidingMember.ContainingType.IsAbstract)
            {
                return false;
            }

            switch (hidingMember.DeclaredAccessibility)
            {
                case Accessibility.Internal:
                case Accessibility.Private:
                case Accessibility.ProtectedAndInternal:
                    break;
                case Accessibility.Public:
                case Accessibility.ProtectedOrInternal:
                case Accessibility.Protected:
                    {
                        // At this point we know we're going to report ERR_HidingAbstractMethod, we just have to
                        // figure out the substitutions.

                        switch (hidingMember.Kind)
                        {
                            case SymbolKind.Method:
                                var associatedPropertyOrEvent = ((MethodSymbol)hidingMember).AssociatedSymbol;
                                if ((object)associatedPropertyOrEvent != null)
                                {
                                    //Dev10 reports that the property/event is doing the hiding, rather than the method
                                    diagnostics.Add(ErrorCode.ERR_HidingAbstractMethod, associatedPropertyOrEvent.Locations[0], associatedPropertyOrEvent, hiddenMember);
                                    break;
                                }

                                goto default;
                            case SymbolKind.Property:
                            case SymbolKind.Event:
                                // NOTE: We used to let the accessors take care of this case, but then we weren't handling the case
                                // where a hiding and hidden properties did not have any accessors in common.

                                // CONSIDER: Dev10 actually reports an error for each accessor of a hidden property/event, but that seems unnecessary.
                                suppressAccessors = true;

                                goto default;
                            default:
                                diagnostics.Add(ErrorCode.ERR_HidingAbstractMethod, hidingMemberLocation, hidingMember, hiddenMember);
                                break;
                        }

                        return true;
                    }
                default:
                    throw ExceptionUtilities.UnexpectedValue(hidingMember.DeclaredAccessibility);
            }
            return false;
        }

        private static bool OverrideHasCorrectAccessibility(Symbol overridden, Symbol overriding)
        {
            if (!overriding.ContainingAssembly.HasInternalAccessTo(overridden.ContainingAssembly) && overridden.DeclaredAccessibility == Accessibility.ProtectedOrInternal)
            {
                return overriding.DeclaredAccessibility == Accessibility.Protected;
            }
            return overridden.DeclaredAccessibility == overriding.DeclaredAccessibility;
        }

        private void CheckInterfaceUnification(BindingDiagnosticBag diagnostics)
        {
            if (!base.IsGenericType)
            {
                return;
            }
            int count = base.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Count;
            if (count < 2)
            {
                return;
            }
            NamedTypeSymbol[] array = base.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.Keys.ToArray();
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    NamedTypeSymbol namedTypeSymbol = array[i];
                    NamedTypeSymbol namedTypeSymbol2 = array[j];
                    if (namedTypeSymbol.IsGenericType && namedTypeSymbol2.IsGenericType && TypeSymbol.Equals(namedTypeSymbol.OriginalDefinition, namedTypeSymbol2.OriginalDefinition, TypeCompareKind.ConsiderEverything) && namedTypeSymbol.CanUnifyWith(namedTypeSymbol2))
                    {
                        if (GetImplementsLocationOrFallback(namedTypeSymbol).SourceSpan.Start > GetImplementsLocationOrFallback(namedTypeSymbol2).SourceSpan.Start)
                        {
                            NamedTypeSymbol namedTypeSymbol3 = namedTypeSymbol;
                            namedTypeSymbol = namedTypeSymbol2;
                            namedTypeSymbol2 = namedTypeSymbol3;
                        }
                        diagnostics.Add(ErrorCode.ERR_UnifyingInterfaceInstantiations, Locations[0], this, namedTypeSymbol, namedTypeSymbol2);
                    }
                }
            }
        }

        private SynthesizedExplicitImplementationForwardingMethod SynthesizeInterfaceMemberImplementation(SymbolAndDiagnostics implementingMemberAndDiagnostics, Symbol interfaceMember)
        {
            if (interfaceMember.DeclaredAccessibility != Accessibility.Public)
            {
                return null;
            }
            ImmutableArray<Diagnostic>.Enumerator enumerator = implementingMemberAndDiagnostics.Diagnostics.Diagnostics.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Severity == DiagnosticSeverity.Error)
                {
                    return null;
                }
            }
            Symbol symbol = implementingMemberAndDiagnostics.Symbol;
            if ((object)symbol == null || symbol.Kind != SymbolKind.Method)
            {
                return null;
            }
            MethodSymbol methodSymbol = (MethodSymbol)interfaceMember;
            MethodSymbol methodSymbol2 = (MethodSymbol)symbol;
            Symbol associatedSymbol = methodSymbol.AssociatedSymbol;
            if ((object)associatedSymbol != null && associatedSymbol.IsEventOrPropertyWithImplementableNonPublicAccessor())
            {
                return null;
            }
            if (methodSymbol2.ExplicitInterfaceImplementations.Contains(methodSymbol, ExplicitInterfaceImplementationTargetMemberEqualityComparer.Instance))
            {
                return null;
            }
            MethodSymbol originalDefinition = methodSymbol2.OriginalDefinition;
            bool flag = true;
            if (MemberSignatureComparer.RuntimeImplicitImplementationComparer.Equals(methodSymbol2, methodSymbol) && IsOverrideOfPossibleImplementationUnderRuntimeRules(methodSymbol2, methodSymbol.ContainingType))
            {
                if ((object)ContainingModule == originalDefinition.ContainingModule)
                {
                    if (originalDefinition is SourceMemberMethodSymbol sourceMemberMethodSymbol)
                    {
                        sourceMemberMethodSymbol.EnsureMetadataVirtual();
                        flag = false;
                    }
                }
                else if (methodSymbol2.IsMetadataVirtual(ignoreInterfaceImplementationChanges: true))
                {
                    flag = false;
                }
            }
            if (!flag)
            {
                return null;
            }
            return new SynthesizedExplicitImplementationForwardingMethod(methodSymbol, methodSymbol2, this);
        }

        private static bool IsPossibleImplementationUnderRuntimeRules(MethodSymbol implementingMethod, NamedTypeSymbol @interface)
        {
            NamedTypeSymbol containingType = implementingMethod.ContainingType;
            if (containingType.InterfacesAndTheirBaseInterfacesNoUseSiteDiagnostics.ContainsKey(@interface))
            {
                return true;
            }
            NamedTypeSymbol baseTypeNoUseSiteDiagnostics = containingType.BaseTypeNoUseSiteDiagnostics;
            if ((object)baseTypeNoUseSiteDiagnostics != null)
            {
                return !baseTypeNoUseSiteDiagnostics.AllInterfacesNoUseSiteDiagnostics.Contains(@interface);
            }
            return true;
        }

        private static bool IsOverrideOfPossibleImplementationUnderRuntimeRules(MethodSymbol implementingMethod, NamedTypeSymbol @interface)
        {
            MethodSymbol methodSymbol = implementingMethod;
            while ((object)methodSymbol != null)
            {
                if (IsPossibleImplementationUnderRuntimeRules(methodSymbol, @interface))
                {
                    return true;
                }
                methodSymbol = methodSymbol.OverriddenMethod;
            }
            return false;
        }

        internal sealed override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            return CalculateInterfacesToEmit();
        }
    }
}
