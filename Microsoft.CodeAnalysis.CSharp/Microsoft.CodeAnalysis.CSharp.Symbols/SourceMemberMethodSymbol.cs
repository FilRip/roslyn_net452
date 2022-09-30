using System.Collections.Immutable;
using System.Globalization;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class SourceMemberMethodSymbol : SourceMethodSymbolWithAttributes, IAttributeTargetSymbol
    {
        protected struct Flags
        {
            private int _flags;

            private const int MethodKindOffset = 0;

            private const int MethodKindSize = 5;

            private const int IsExtensionMethodOffset = 5;

            private const int IsExtensionMethodSize = 1;

            private const int IsMetadataVirtualIgnoringInterfaceChangesOffset = 6;

            private const int IsMetadataVirtualIgnoringInterfaceChangesSize = 1;

            private const int IsMetadataVirtualOffset = 7;

            private const int IsMetadataVirtualSize = 1;

            private const int IsMetadataVirtualLockedOffset = 8;

            private const int IsMetadataVirtualLockedSize = 1;

            private const int ReturnsVoidOffset = 9;

            private const int ReturnsVoidSize = 2;

            private const int NullableContextOffset = 11;

            private const int NullableContextSize = 3;

            private const int IsNullableAnalysisEnabledOffset = 14;

            private const int IsNullableAnalysisEnabledSize = 1;

            private const int MethodKindMask = 31;

            private const int IsExtensionMethodBit = 32;

            private const int IsMetadataVirtualIgnoringInterfaceChangesBit = 64;

            private const int IsMetadataVirtualBit = 64;

            private const int IsMetadataVirtualLockedBit = 256;

            private const int ReturnsVoidBit = 512;

            private const int ReturnsVoidIsSetBit = 1024;

            private const int NullableContextMask = 7;

            private const int IsNullableAnalysisEnabledBit = 16384;

            public MethodKind MethodKind => (MethodKind)(_flags & 0x1F);

            public bool IsExtensionMethod => (_flags & 0x20) != 0;

            public bool IsNullableAnalysisEnabled => (_flags & 0x4000) != 0;

            public bool IsMetadataVirtualLocked => (_flags & 0x100) != 0;

            public bool TryGetReturnsVoid(out bool value)
            {
                int flags = _flags;
                value = (flags & 0x200) != 0;
                return (flags & 0x400) != 0;
            }

            public void SetReturnsVoid(bool value)
            {
                ThreadSafeFlagOperations.Set(ref _flags, 0x400 | (value ? 512 : 0));
            }

            private static bool ModifiersRequireMetadataVirtual(DeclarationModifiers modifiers)
            {
                return (modifiers & (DeclarationModifiers.Abstract | DeclarationModifiers.Virtual | DeclarationModifiers.Override)) != 0;
            }

            public Flags(MethodKind methodKind, DeclarationModifiers declarationModifiers, bool returnsVoid, bool isExtensionMethod, bool isNullableAnalysisEnabled, bool isMetadataVirtualIgnoringModifiers = false)
            {
                bool num = isMetadataVirtualIgnoringModifiers || ModifiersRequireMetadataVirtual(declarationModifiers);
                int num2 = (int)(methodKind & (MethodKind)31);
                int num3 = (isExtensionMethod ? 32 : 0);
                int num4 = (isNullableAnalysisEnabled ? 16384 : 0);
                int num5 = (num ? 64 : 0);
                int num6 = (num ? 64 : 0);
                _flags = num2 | num3 | num4 | num5 | num6 | (returnsVoid ? 512 : 0) | 0x400;
            }

            public bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
            {
                if (ignoreInterfaceImplementationChanges)
                {
                    return (_flags & 0x40) != 0;
                }
                if (!IsMetadataVirtualLocked)
                {
                    ThreadSafeFlagOperations.Set(ref _flags, 256);
                }
                return (_flags & 0x40) != 0;
            }

            public void EnsureMetadataVirtual()
            {
                if ((_flags & 0x40) == 0)
                {
                    ThreadSafeFlagOperations.Set(ref _flags, 64);
                }
            }

            public bool TryGetNullableContext(out byte? value)
            {
                return ((NullableContextKind)((uint)(_flags >> 11) & 7u)).TryGetByte(out value);
            }

            public bool SetNullableContext(byte? value)
            {
                return ThreadSafeFlagOperations.Set(ref _flags, (int)((uint)(value.ToNullableContextFlags() & (NullableContextKind)7) << 11));
            }
        }

        protected SymbolCompletionState state;

        protected DeclarationModifiers DeclarationModifiers;

        protected Flags flags;

        private readonly NamedTypeSymbol _containingType;

        private ParameterSymbol _lazyThisParameter;

        private TypeWithAnnotations.Boxed _lazyIteratorElementType;

        private OverriddenOrHiddenMembersResult _lazyOverriddenOrHiddenMembers;

        protected ImmutableArray<Location> locations;

        protected string lazyDocComment;

        protected string lazyExpandedDocComment;

        private ImmutableArray<Diagnostic> _cachedDiagnostics;

        internal ImmutableArray<Diagnostic> Diagnostics => _cachedDiagnostics;

        protected virtual object MethodChecksLockObject => syntaxReferenceOpt;

        public sealed override Symbol ContainingSymbol => _containingType;

        public override NamedTypeSymbol ContainingType => _containingType;

        public override Symbol AssociatedSymbol => null;

        public override bool ReturnsVoid
        {
            get
            {
                flags.TryGetReturnsVoid(out var value);
                return value;
            }
        }

        public sealed override MethodKind MethodKind => flags.MethodKind;

        public override bool IsExtensionMethod => flags.IsExtensionMethod;

        public override Accessibility DeclaredAccessibility => ModifierUtils.EffectiveAccessibility(DeclarationModifiers);

        internal bool HasExternModifier => (DeclarationModifiers & DeclarationModifiers.Extern) != 0;

        public override bool IsExtern => HasExternModifier;

        public sealed override bool IsSealed => (DeclarationModifiers & DeclarationModifiers.Sealed) != 0;

        public sealed override bool IsAbstract => (DeclarationModifiers & DeclarationModifiers.Abstract) != 0;

        public sealed override bool IsOverride => (DeclarationModifiers & DeclarationModifiers.Override) != 0;

        internal bool IsPartial => (DeclarationModifiers & DeclarationModifiers.Partial) != 0;

        public sealed override bool IsVirtual => (DeclarationModifiers & DeclarationModifiers.Virtual) != 0;

        internal bool IsNew => (DeclarationModifiers & DeclarationModifiers.New) != 0;

        public sealed override bool IsStatic => (DeclarationModifiers & DeclarationModifiers.Static) != 0;

        internal bool IsUnsafe => (DeclarationModifiers & DeclarationModifiers.Unsafe) != 0;

        public sealed override bool IsAsync => (DeclarationModifiers & DeclarationModifiers.Async) != 0;

        internal override bool IsDeclaredReadOnly => (DeclarationModifiers & DeclarationModifiers.ReadOnly) != 0;

        internal override bool IsInitOnly => false;

        internal sealed override CallingConvention CallingConvention
        {
            get
            {
                CallingConvention callingConvention = (IsVararg ? CallingConvention.ExtraArguments : CallingConvention.Default);
                if (IsGenericMethod)
                {
                    callingConvention |= CallingConvention.Generic;
                }
                if (!IsStatic)
                {
                    callingConvention |= CallingConvention.HasThis;
                }
                return callingConvention;
            }
        }

        internal (BlockSyntax blockBody, ArrowExpressionClauseSyntax arrowBody) Bodies
        {
            get
            {
                CSharpSyntaxNode syntaxNode = SyntaxNode;
                if (!(syntaxNode is BaseMethodDeclarationSyntax baseMethodDeclarationSyntax))
                {
                    if (!(syntaxNode is AccessorDeclarationSyntax accessorDeclarationSyntax))
                    {
                        if (!(syntaxNode is ArrowExpressionClauseSyntax item))
                        {
                            if (syntaxNode is BlockSyntax item2)
                            {
                                return (item2, null);
                            }
                            return (null, null);
                        }
                        return (null, item);
                    }
                    return (accessorDeclarationSyntax.Body, accessorDeclarationSyntax.ExpressionBody);
                }
                return (baseMethodDeclarationSyntax.Body, baseMethodDeclarationSyntax.ExpressionBody);
            }
        }

        public override ImmutableArray<Location> Locations => locations;

        public override ImmutableArray<CustomModifier> RefCustomModifiers => ImmutableArray<CustomModifier>.Empty;

        public sealed override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotations => GetTypeParametersAsTypeArguments();

        public sealed override int Arity => TypeParameters.Length;

        internal override TypeWithAnnotations IteratorElementTypeWithAnnotations
        {
            get
            {
                return _lazyIteratorElementType?.Value ?? default(TypeWithAnnotations);
            }
            set
            {
                Interlocked.CompareExchange(ref _lazyIteratorElementType, new TypeWithAnnotations.Boxed(value), TypeWithAnnotations.Boxed.Sentinel);
            }
        }

        internal override bool IsIterator => _lazyIteratorElementType != null;

        public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations => ImmutableArray<MethodSymbol>.Empty;

        internal sealed override OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers
        {
            get
            {
                LazyMethodChecks();
                if (_lazyOverriddenOrHiddenMembers == null)
                {
                    Interlocked.CompareExchange(ref _lazyOverriddenOrHiddenMembers, this.MakeOverriddenOrHiddenMembers(), null);
                }
                return _lazyOverriddenOrHiddenMembers;
            }
        }

        internal sealed override bool RequiresCompletion => true;

        internal abstract bool IsExpressionBodied { get; }

        internal ImmutableArray<Diagnostic> SetDiagnostics(ImmutableArray<Diagnostic> newSet, out bool diagsWritten)
        {
            diagsWritten = ImmutableInterlocked.InterlockedInitialize(ref _cachedDiagnostics, newSet);
            return _cachedDiagnostics;
        }

        protected SourceMemberMethodSymbol(NamedTypeSymbol containingType, SyntaxReference syntaxReferenceOpt, Location location, bool isIterator)
            : this(containingType, syntaxReferenceOpt, ImmutableArray.Create(location), isIterator)
        {
        }

        protected SourceMemberMethodSymbol(NamedTypeSymbol containingType, SyntaxReference syntaxReferenceOpt, ImmutableArray<Location> locations, bool isIterator)
            : base(syntaxReferenceOpt)
        {
            _containingType = containingType;
            this.locations = locations;
            if (isIterator)
            {
                _lazyIteratorElementType = TypeWithAnnotations.Boxed.Sentinel;
            }
        }

        protected void CheckEffectiveAccessibility(TypeWithAnnotations returnType, ImmutableArray<ParameterSymbol> parameters, BindingDiagnosticBag diagnostics)
        {
            if (DeclaredAccessibility <= Accessibility.Private || MethodKind == MethodKind.ExplicitInterfaceImplementation)
            {
                return;
            }
            ErrorCode code = ((MethodKind == MethodKind.Conversion || MethodKind == MethodKind.UserDefinedOperator) ? ErrorCode.ERR_BadVisOpReturn : ErrorCode.ERR_BadVisReturnType);
            CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(diagnostics, ContainingAssembly);
            if (!this.IsNoMoreVisibleThan(returnType, ref useSiteInfo))
            {
                diagnostics.Add(code, Locations[0], this, returnType.Type);
            }
            code = ((MethodKind == MethodKind.Conversion || MethodKind == MethodKind.UserDefinedOperator) ? ErrorCode.ERR_BadVisOpParam : ErrorCode.ERR_BadVisParamType);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator = parameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ParameterSymbol current = enumerator.Current;
                if (!current.TypeWithAnnotations.IsAtLeastAsVisibleAs(this, ref useSiteInfo))
                {
                    diagnostics.Add(code, Locations[0], this, current.Type);
                }
            }
            diagnostics.Add(Locations[0], useSiteInfo);
        }

        protected void MakeFlags(MethodKind methodKind, DeclarationModifiers declarationModifiers, bool returnsVoid, bool isExtensionMethod, bool isNullableAnalysisEnabled, bool isMetadataVirtualIgnoringModifiers = false)
        {
            DeclarationModifiers = declarationModifiers;
            flags = new Flags(methodKind, declarationModifiers, returnsVoid, isExtensionMethod, isNullableAnalysisEnabled, isMetadataVirtualIgnoringModifiers);
        }

        protected void SetReturnsVoid(bool returnsVoid)
        {
            flags.SetReturnsVoid(returnsVoid);
        }

        protected abstract void MethodChecks(BindingDiagnosticBag diagnostics);

        protected void LazyMethodChecks()
        {
            if (state.HasComplete(CompletionPart.StartMemberChecks))
            {
                return;
            }
            lock (MethodChecksLockObject)
            {
                if (state.NotePartComplete(CompletionPart.SynthesizedExplicitImplementations))
                {
                    BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
                    try
                    {
                        MethodChecks(instance);
                        AddDeclarationDiagnostics(instance);
                        return;
                    }
                    finally
                    {
                        state.NotePartComplete(CompletionPart.StartMemberChecks);
                        instance.Free();
                    }
                }
            }
        }

        protected virtual void LazyAsyncMethodChecks(CancellationToken cancellationToken)
        {
            state.NotePartComplete(CompletionPart.Members);
            state.NotePartComplete(CompletionPart.TypeMembers);
        }

        internal override bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
        {
            if (IsExplicitInterfaceImplementation && _containingType.IsInterface)
            {
                return false;
            }
            if (!IsOverride)
            {
                return IsMetadataVirtual(ignoreInterfaceImplementationChanges);
            }
            return this.RequiresExplicitOverride(out bool warnAmbiguous);
        }

        internal override bool IsMetadataVirtual(bool ignoreInterfaceImplementationChanges = false)
        {
            return flags.IsMetadataVirtual(ignoreInterfaceImplementationChanges);
        }

        internal void EnsureMetadataVirtual()
        {
            flags.EnsureMetadataVirtual();
        }

        private Binder TryGetInMethodBinder(BinderFactory binderFactoryOpt = null)
        {
            CSharpSyntaxNode inMethodSyntaxNode = GetInMethodSyntaxNode();
            if (inMethodSyntaxNode == null)
            {
                return null;
            }
            return (binderFactoryOpt ?? DeclaringCompilation.GetBinderFactory(inMethodSyntaxNode.SyntaxTree)).GetBinder(inMethodSyntaxNode);
        }

        internal virtual ExecutableCodeBinder TryGetBodyBinder(BinderFactory binderFactoryOpt = null, bool ignoreAccessibility = false)
        {
            Binder binder = TryGetInMethodBinder(binderFactoryOpt);
            if (binder != null)
            {
                return new ExecutableCodeBinder(SyntaxNode, this, binder.WithAdditionalFlags(ignoreAccessibility ? BinderFlags.IgnoreAccessibility : BinderFlags.None));
            }
            return null;
        }

        public override string GetDocumentationCommentXml(CultureInfo preferredCulture = null, bool expandIncludes = false, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(this, expandIncludes, ref expandIncludes ? ref lazyExpandedDocComment : ref lazyDocComment);
        }

        internal sealed override bool TryGetThisParameter(out ParameterSymbol thisParameter)
        {
            thisParameter = _lazyThisParameter;
            if ((object)thisParameter != null || IsStatic)
            {
                return true;
            }
            Interlocked.CompareExchange(ref _lazyThisParameter, new ThisParameterSymbol(this), null);
            thisParameter = _lazyThisParameter;
            return true;
        }

        internal sealed override bool HasComplete(CompletionPart part)
        {
            return state.HasComplete(part);
        }

        internal override void ForceComplete(SourceLocation locationOpt, CancellationToken cancellationToken)
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
                    case CompletionPart.ReturnTypeAttributes:
                        GetReturnTypeAttributes();
                        break;
                    case CompletionPart.Type:
                        _ = ReturnTypeWithAnnotations;
                        state.NotePartComplete(CompletionPart.Type);
                        break;
                    case CompletionPart.Parameters:
                        {
                            ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = Parameters.GetEnumerator();
                            while (enumerator2.MoveNext())
                            {
                                enumerator2.Current.ForceComplete(locationOpt, cancellationToken);
                            }
                            state.NotePartComplete(CompletionPart.Parameters);
                            break;
                        }
                    case CompletionPart.TypeParameters:
                        {
                            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = TypeParameters.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                enumerator.Current.ForceComplete(locationOpt, cancellationToken);
                            }
                            state.NotePartComplete(CompletionPart.TypeParameters);
                            break;
                        }
                    case CompletionPart.Members:
                    case CompletionPart.TypeMembers:
                        LazyAsyncMethodChecks(cancellationToken);
                        break;
                    case CompletionPart.SynthesizedExplicitImplementations:
                    case CompletionPart.StartMemberChecks:
                        {
                            LazyMethodChecks();
                            CompletionPart part = CompletionPart.MethodSymbolAll;
                            state.SpinWaitComplete(part, cancellationToken);
                            return;
                        }
                    case CompletionPart.None:
                        return;
                    default:
                        state.NotePartComplete(CompletionPart.ImportsAll | CompletionPart.StartInterfaces | CompletionPart.FinishInterfaces | CompletionPart.EnumUnderlyingType | CompletionPart.TypeArguments | CompletionPart.FinishMemberChecks | CompletionPart.MembersCompleted);
                        break;
                }
                state.SpinWaitComplete(nextIncompletePart, cancellationToken);
            }
        }

        protected sealed override void NoteAttributesComplete(bool forReturnType)
        {
            CompletionPart part = ((!forReturnType) ? CompletionPart.Attributes : CompletionPart.ReturnTypeAttributes);
            state.NotePartComplete(part);
        }

        internal override void AfterAddingTypeMembersChecks(ConversionsBase conversions, BindingDiagnosticBag diagnostics)
        {
            base.AfterAddingTypeMembersChecks(conversions, diagnostics);
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            Location location = locations[0];
            if (IsDeclaredReadOnly && !ContainingType.IsReadOnly)
            {
                declaringCompilation.EnsureIsReadOnlyAttributeExists(diagnostics, location, modifyCompilation: true);
            }
            if (declaringCompilation.ShouldEmitNullableAttributes(this) && ShouldEmitNullableContextValue(out var _))
            {
                declaringCompilation.EnsureNullableContextAttributeExists(diagnostics, location, modifyCompilation: true);
            }
        }

        internal override byte? GetLocalNullableContextValue()
        {
            if (!flags.TryGetNullableContext(out var value))
            {
                value = ComputeNullableContextValue();
                flags.SetNullableContext(value);
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
            ImmutableArray<TypeParameterSymbol>.Enumerator enumerator = TypeParameters.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.GetCommonNullableValues(declaringCompilation, ref builder);
            }
            builder.AddValue(ReturnTypeWithAnnotations);
            ImmutableArray<ParameterSymbol>.Enumerator enumerator2 = Parameters.GetEnumerator();
            while (enumerator2.MoveNext())
            {
                enumerator2.Current.GetCommonNullableValues(declaringCompilation, ref builder);
            }
            return builder.MostCommonValue;
        }

        internal override bool IsNullableAnalysisEnabled()
        {
            return flags.IsNullableAnalysisEnabled;
        }

        internal override void AddSynthesizedAttributes(PEModuleBuilder moduleBuilder, ref ArrayBuilder<SynthesizedAttributeData> attributes)
        {
            base.AddSynthesizedAttributes(moduleBuilder, ref attributes);
            if (IsDeclaredReadOnly && !ContainingType.IsReadOnly)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeIsReadOnlyAttribute(this));
            }
            CSharpCompilation declaringCompilation = DeclaringCompilation;
            if (declaringCompilation.ShouldEmitNullableAttributes(this) && ShouldEmitNullableContextValue(out var value))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizeNullableContextAttribute(this, value));
            }
            if (this.RequiresExplicitOverride(out var _))
            {
                Symbol.AddSynthesizedAttribute(ref attributes, moduleBuilder.SynthesizePreserveBaseOverridesAttribute());
            }
            bool isAsync = IsAsync;
            bool isIterator = IsIterator;
            if (!isAsync && !isIterator)
            {
                return;
            }
            if (moduleBuilder.CompilationState.TryGetStateMachineType(this, out var stateMachineType))
            {
                TypedConstant item = new TypedConstant(declaringCompilation.GetWellKnownType(WellKnownType.System_Type), TypedConstantKind.Type, stateMachineType.GetUnboundGenericTypeOrSelf());
                if (isAsync && isIterator)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_AsyncIteratorStateMachineAttribute__ctor, ImmutableArray.Create(item)));
                }
                else if (isAsync)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_AsyncStateMachineAttribute__ctor, ImmutableArray.Create(item)));
                }
                else if (isIterator)
                {
                    Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_IteratorStateMachineAttribute__ctor, ImmutableArray.Create(item)));
                }
            }
            if (isAsync && !isIterator)
            {
                Symbol.AddSynthesizedAttribute(ref attributes, declaringCompilation.SynthesizeDebuggerStepThroughAttribute());
            }
        }

        protected void CheckModifiersForBody(Location location, BindingDiagnosticBag diagnostics)
        {
            if (IsExtern && !IsAbstract)
            {
                diagnostics.Add(ErrorCode.ERR_ExternHasBody, location, this);
            }
            else if (IsAbstract && !IsExtern)
            {
                diagnostics.Add(ErrorCode.ERR_AbstractHasBody, location, this);
            }
        }

        protected void CheckFeatureAvailabilityAndRuntimeSupport(SyntaxNode declarationSyntax, Location location, bool hasBody, BindingDiagnosticBag diagnostics)
        {
            if (_containingType.IsInterface)
            {
                if (hasBody || IsExplicitInterfaceImplementation)
                {
                    Binder.CheckFeatureAvailability(declarationSyntax, MessageID.IDS_DefaultInterfaceImplementation, diagnostics, location);
                }
                if ((hasBody || IsExplicitInterfaceImplementation || IsExtern) && !ContainingAssembly.RuntimeSupportsDefaultInterfaceImplementation)
                {
                    diagnostics.Add(ErrorCode.ERR_RuntimeDoesNotSupportDefaultInterfaceImplementation, location);
                }
            }
        }

        internal override int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
        {
            (BlockSyntax blockBody, ArrowExpressionClauseSyntax arrowBody) bodies = Bodies;
            BlockSyntax item = bodies.blockBody;
            ArrowExpressionClauseSyntax item2 = bodies.arrowBody;
            CSharpSyntaxNode cSharpSyntaxNode = null;
            if (item != null && item.Span.Contains(localPosition))
            {
                cSharpSyntaxNode = item;
            }
            else
            {
                if (item2 == null || !item2.Span.Contains(localPosition))
                {
                    return -1;
                }
                cSharpSyntaxNode = item2;
            }
            return localPosition - cSharpSyntaxNode.SpanStart;
        }
    }
}
