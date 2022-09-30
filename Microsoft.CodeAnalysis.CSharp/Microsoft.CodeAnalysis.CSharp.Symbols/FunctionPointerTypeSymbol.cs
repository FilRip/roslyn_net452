using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection.Metadata;
using System.Threading;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public sealed class FunctionPointerTypeSymbol : TypeSymbol, IFunctionPointerTypeReference, ITypeReference, IReference
    {
        private sealed class FunctionPointerMethodSignature : ISignature
        {
            private readonly FunctionPointerMethodSymbol _underlying;

            internal ISignature Underlying => _underlying.GetCciAdapter();

            public CallingConvention CallingConvention => Underlying.CallingConvention;

            public ushort ParameterCount => Underlying.ParameterCount;

            public ImmutableArray<ICustomModifier> ReturnValueCustomModifiers => Underlying.ReturnValueCustomModifiers;

            public ImmutableArray<ICustomModifier> RefCustomModifiers => Underlying.RefCustomModifiers;

            public bool ReturnValueIsByRef => Underlying.ReturnValueIsByRef;

            internal FunctionPointerMethodSignature(FunctionPointerMethodSymbol underlying)
            {
                _underlying = underlying;
            }

            public ImmutableArray<IParameterTypeInformation> GetParameters(EmitContext context)
            {
                return Underlying.GetParameters(context);
            }

            public ITypeReference GetType(EmitContext context)
            {
                return Underlying.GetType(context);
            }

            public override bool Equals(object? obj)
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override int GetHashCode()
            {
                throw ExceptionUtilities.Unreachable;
            }

            public override string ToString()
            {
                return _underlying.ToDisplayString(SymbolDisplayFormat.ILVisualizationFormat);
            }
        }

        private FunctionPointerMethodSignature? _lazySignature;

        ISignature IFunctionPointerTypeReference.Signature
        {
            get
            {
                if (_lazySignature == null)
                {
                    Interlocked.CompareExchange(ref _lazySignature, new FunctionPointerMethodSignature(AdaptedFunctionPointerTypeSymbol.Signature), null);
                }
                return _lazySignature;
            }
        }

        bool ITypeReference.IsEnum => false;

        Microsoft.Cci.PrimitiveTypeCode ITypeReference.TypeCode => Microsoft.Cci.PrimitiveTypeCode.FunctionPointer;

        TypeDefinitionHandle ITypeReference.TypeDef => default;

        IGenericMethodParameterReference? ITypeReference.AsGenericMethodParameterReference => null;

        IGenericTypeInstanceReference? ITypeReference.AsGenericTypeInstanceReference => null;

        IGenericTypeParameterReference? ITypeReference.AsGenericTypeParameterReference => null;

        INamespaceTypeReference? ITypeReference.AsNamespaceTypeReference => null;

        INestedTypeReference? ITypeReference.AsNestedTypeReference => null;

        ISpecializedNestedTypeReference? ITypeReference.AsSpecializedNestedTypeReference => null;

        bool ITypeReference.IsValueType => AdaptedFunctionPointerTypeSymbol.IsValueType;

        internal FunctionPointerTypeSymbol AdaptedFunctionPointerTypeSymbol => this;

        public FunctionPointerMethodSymbol Signature { get; }

        public override bool IsReferenceType => false;

        public override bool IsValueType => true;

        public override TypeKind TypeKind => TypeKind.FunctionPointer;

        public override bool IsRefLikeType => false;

        public override bool IsReadOnly => false;

        public override SymbolKind Kind => SymbolKind.FunctionPointerType;

        public override Symbol? ContainingSymbol => null;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override bool IsStatic => false;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        internal override NamedTypeSymbol? BaseTypeNoUseSiteDiagnostics => null;

        internal override ObsoleteAttributeData? ObsoleteAttributeData => null;

        internal override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        INamespaceTypeDefinition? ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
        {
            return null;
        }

        INestedTypeDefinition? ITypeReference.AsNestedTypeDefinition(EmitContext context)
        {
            return null;
        }

        ITypeDefinition? ITypeReference.AsTypeDefinition(EmitContext context)
        {
            return null;
        }

        ITypeDefinition? ITypeReference.GetResolvedType(EmitContext context)
        {
            return null;
        }

        IEnumerable<ICustomAttribute> IReference.GetAttributes(EmitContext context)
        {
            return SpecializedCollections.EmptyEnumerable<ICustomAttribute>();
        }

        IDefinition? IReference.AsDefinition(EmitContext context)
        {
            return null;
        }

        internal new FunctionPointerTypeSymbol GetCciAdapter()
        {
            return this;
        }

        public static FunctionPointerTypeSymbol CreateFromSource(FunctionPointerTypeSyntax syntax, Binder typeBinder, BindingDiagnosticBag diagnostics, ConsList<TypeSymbol> basesBeingResolved, bool suppressUseSiteDiagnostics)
        {
            return new FunctionPointerTypeSymbol(FunctionPointerMethodSymbol.CreateFromSource(syntax, typeBinder, diagnostics, basesBeingResolved, suppressUseSiteDiagnostics));
        }

        public static FunctionPointerTypeSymbol CreateFromPartsForTests(CallingConvention callingConvention, TypeWithAnnotations returnType, ImmutableArray<CustomModifier> refCustomModifiers, RefKind returnRefKind, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<ImmutableArray<CustomModifier>> parameterRefCustomModifiers, ImmutableArray<RefKind> parameterRefKinds, CSharpCompilation compilation)
        {
            return new FunctionPointerTypeSymbol(FunctionPointerMethodSymbol.CreateFromPartsForTest(callingConvention, returnType, refCustomModifiers, returnRefKind, parameterTypes, parameterRefCustomModifiers, parameterRefKinds, compilation));
        }

        public static FunctionPointerTypeSymbol CreateFromParts(CallingConvention callingConvention, ImmutableArray<CustomModifier> callingConventionModifiers, TypeWithAnnotations returnType, RefKind returnRefKind, ImmutableArray<TypeWithAnnotations> parameterTypes, ImmutableArray<RefKind> parameterRefKinds, CSharpCompilation compilation)
        {
            return new FunctionPointerTypeSymbol(FunctionPointerMethodSymbol.CreateFromParts(callingConvention, callingConventionModifiers, returnType, returnRefKind, parameterTypes, parameterRefKinds, compilation));
        }

        public static FunctionPointerTypeSymbol CreateFromMetadata(CallingConvention callingConvention, ImmutableArray<ParamInfo<TypeSymbol>> retAndParamTypes)
        {
            return new FunctionPointerTypeSymbol(FunctionPointerMethodSymbol.CreateFromMetadata(callingConvention, retAndParamTypes));
        }

        public FunctionPointerTypeSymbol SubstituteTypeSymbol(TypeWithAnnotations substitutedReturnType, ImmutableArray<TypeWithAnnotations> substitutedParameterTypes, ImmutableArray<CustomModifier> refCustomModifiers, ImmutableArray<ImmutableArray<CustomModifier>> paramRefCustomModifiers)
        {
            return new FunctionPointerTypeSymbol(Signature.SubstituteParameterSymbols(substitutedReturnType, substitutedParameterTypes, refCustomModifiers, paramRefCustomModifiers));
        }

        private FunctionPointerTypeSymbol(FunctionPointerMethodSymbol signature)
        {
            Signature = signature;
        }

        internal override ManagedKind GetManagedKind(ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
        {
            return ManagedKind.Unmanaged;
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitFunctionPointerType(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitFunctionPointerType(this);
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

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument a)
        {
            return visitor.VisitFunctionPointerType(this, a);
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol>? basesBeingResolved = null)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override bool Equals(TypeSymbol t2, TypeCompareKind compareKind)
        {
            if ((object)this == t2)
            {
                return true;
            }
            if (t2 is FunctionPointerTypeSymbol functionPointerTypeSymbol)
            {
                return Signature.Equals(functionPointerTypeSymbol.Signature, compareKind);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Hash.Combine(1, Signature.GetHashCode());
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FunctionPointerTypeSymbol(this, base.DefaultNullableAnnotation);
        }

        protected override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FunctionPointerTypeSymbol(this, nullableAnnotation);
        }

        internal override void AddNullableTransforms(ArrayBuilder<byte> transforms)
        {
            Signature.AddNullableTransforms(transforms);
        }

        internal override bool ApplyNullableTransforms(byte defaultTransformFlag, ImmutableArray<byte> transforms, ref int position, out TypeSymbol result)
        {
            FunctionPointerMethodSymbol functionPointerMethodSymbol = Signature.ApplyNullableTransforms(defaultTransformFlag, transforms, ref position);
            bool flag = (object)Signature != functionPointerMethodSymbol;
            result = (flag ? new FunctionPointerTypeSymbol(functionPointerMethodSymbol) : this);
            return flag;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            UseSiteInfo<AssemblySymbol> fromSignature = Signature.GetUseSiteInfo();

            if (fromSignature.DiagnosticInfo?.Code == (int)ErrorCode.ERR_BindToBogus && fromSignature.DiagnosticInfo.Arguments.AsSingleton() == (object)Signature)
            {
                return new UseSiteInfo<AssemblySymbol>(new CSDiagnosticInfo(ErrorCode.ERR_BogusType, this));
            }

            return fromSignature;
        }

        internal override bool GetUnificationUseSiteDiagnosticRecursive(ref DiagnosticInfo? result, Symbol owner, ref HashSet<TypeSymbol> checkedTypes)
        {
            return Signature.GetUnificationUseSiteDiagnosticRecursive(ref result, owner, ref checkedTypes);
        }

        internal override TypeSymbol MergeEquivalentTypes(TypeSymbol other, VarianceKind variance)
        {
            FunctionPointerMethodSymbol functionPointerMethodSymbol = Signature.MergeEquivalentTypes(((FunctionPointerTypeSymbol)other).Signature, variance);
            if ((object)functionPointerMethodSymbol != Signature)
            {
                return new FunctionPointerTypeSymbol(functionPointerMethodSymbol);
            }
            return this;
        }

        internal override TypeSymbol SetNullabilityForReferenceTypes(Func<TypeWithAnnotations, TypeWithAnnotations> transform)
        {
            FunctionPointerMethodSymbol functionPointerMethodSymbol = Signature.SetNullabilityForReferenceTypes(transform);
            if ((object)Signature != functionPointerMethodSymbol)
            {
                return new FunctionPointerTypeSymbol(functionPointerMethodSymbol);
            }
            return this;
        }

        internal static bool RefKindEquals(TypeCompareKind compareKind, RefKind refKind1, RefKind refKind2)
        {
            if ((compareKind & TypeCompareKind.FunctionPointerRefMatchesOutInRefReadonly) == 0)
            {
                return refKind1 == refKind2;
            }
            return refKind1 == RefKind.None == (refKind2 == RefKind.None);
        }

        internal static RefKind GetRefKindForHashCode(RefKind refKind)
        {
            if (refKind != 0)
            {
                return RefKind.Ref;
            }
            return RefKind.None;
        }

        internal static bool IsCallingConventionModifier(NamedTypeSymbol modifierType)
        {
            if ((object)modifierType.ContainingAssembly == modifierType.ContainingAssembly?.CorLibrary && modifierType.Arity == 0 && modifierType.Name != "CallConv" && modifierType.Name.StartsWith("CallConv", StringComparison.Ordinal))
            {
                return modifierType.IsCompilerServicesTopLevelType();
            }
            return false;
        }
    }
}
