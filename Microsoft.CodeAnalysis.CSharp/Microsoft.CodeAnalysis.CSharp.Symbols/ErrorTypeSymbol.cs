using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using Microsoft.Cci;

using Roslyn.Utilities;

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal abstract class ErrorTypeSymbol : NamedTypeSymbol
    {
        protected sealed class ErrorTypeParameterSymbol : TypeParameterSymbol
        {
            private readonly ErrorTypeSymbol _container;

            private readonly string _name;

            private readonly int _ordinal;

            public override string Name => _name;

            public override TypeParameterKind TypeParameterKind => TypeParameterKind.Type;

            public override Symbol ContainingSymbol => _container;

            public override bool HasConstructorConstraint => false;

            public override bool HasReferenceTypeConstraint => false;

            public override bool IsReferenceTypeFromConstraintTypes => false;

            internal override bool? ReferenceTypeConstraintIsNullable => false;

            public override bool HasNotNullConstraint => false;

            internal override bool? IsNotNullable => null;

            public override bool HasValueTypeConstraint => false;

            public override bool IsValueTypeFromConstraintTypes => false;

            public override bool HasUnmanagedTypeConstraint => false;

            public override int Ordinal => _ordinal;

            public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

            public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

            public override VarianceKind Variance => VarianceKind.None;

            public override bool IsImplicitlyDeclared => true;

            public ErrorTypeParameterSymbol(ErrorTypeSymbol container, string name, int ordinal)
            {
                _container = container;
                _name = name;
                _ordinal = ordinal;
            }

            internal override void EnsureAllConstraintsAreResolved()
            {
            }

            internal override ImmutableArray<TypeWithAnnotations> GetConstraintTypes(ConsList<TypeParameterSymbol> inProgress)
            {
                return ImmutableArray<TypeWithAnnotations>.Empty;
            }

            internal override ImmutableArray<NamedTypeSymbol> GetInterfaces(ConsList<TypeParameterSymbol> inProgress)
            {
                return ImmutableArray<NamedTypeSymbol>.Empty;
            }

            internal override NamedTypeSymbol? GetEffectiveBaseClass(ConsList<TypeParameterSymbol> inProgress)
            {
                return null;
            }

            internal override TypeSymbol? GetDeducedBaseType(ConsList<TypeParameterSymbol> inProgress)
            {
                return null;
            }

            public override int GetHashCode()
            {
                return Hash.Combine(_container.GetHashCode(), _ordinal);
            }

            internal override bool Equals(TypeSymbol? t2, TypeCompareKind comparison)
            {
                if ((object)this == t2)
                {
                    return true;
                }
                if (t2 is ErrorTypeParameterSymbol errorTypeParameterSymbol && errorTypeParameterSymbol._ordinal == _ordinal)
                {
                    return errorTypeParameterSymbol.ContainingType.Equals(ContainingType, comparison);
                }
                return false;
            }
        }

        internal static readonly ErrorTypeSymbol UnknownResultType = new UnsupportedMetadataTypeSymbol();

        private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

        internal abstract DiagnosticInfo? ErrorInfo { get; }

        internal virtual LookupResultKind ResultKind => LookupResultKind.Empty;

        public virtual ImmutableArray<Symbol> CandidateSymbols => ImmutableArray<Symbol>.Empty;

        public CandidateReason CandidateReason
        {
            get
            {
                if (!CandidateSymbols.IsEmpty)
                {
                    return ResultKind.ToCandidateReason();
                }
                return CandidateReason.None;
            }
        }

        public override bool IsReferenceType => true;

        public sealed override bool IsValueType => false;

        public sealed override bool IsRefLikeType => false;

        public sealed override bool IsReadOnly => false;

        public override IEnumerable<string> MemberNames => SpecializedCollections.EmptyEnumerable<string>();

        public sealed override SymbolKind Kind => SymbolKind.ErrorType;

        public sealed override TypeKind TypeKind => TypeKind.Error;

        internal sealed override bool IsInterface => false;

        public override Symbol? ContainingSymbol => null;

        public override ImmutableArray<Location> Locations => ImmutableArray<Location>.Empty;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences => ImmutableArray<SyntaxReference>.Empty;

        public override int Arity => 0;

        public override string Name => string.Empty;

        internal override ImmutableArray<TypeWithAnnotations> TypeArgumentsWithAnnotationsNoUseSiteDiagnostics => GetTypeParametersAsTypeArguments();

        public override ImmutableArray<TypeParameterSymbol> TypeParameters
        {
            get
            {
                if (_lazyTypeParameters.IsDefault)
                {
                    ImmutableInterlocked.InterlockedCompareExchange(ref _lazyTypeParameters, GetTypeParameters(), default(ImmutableArray<TypeParameterSymbol>));
                }
                return _lazyTypeParameters;
            }
        }

        public override NamedTypeSymbol ConstructedFrom => this;

        public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public sealed override bool IsStatic => false;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsSealed => false;

        internal sealed override bool HasSpecialName => false;

        public sealed override bool MightContainExtensionMethods => false;

        internal override NamedTypeSymbol? BaseTypeNoUseSiteDiagnostics => null;

        internal override bool HasCodeAnalysisEmbeddedAttribute => false;

        internal sealed override bool ShouldAddWinRTMembers => false;

        internal sealed override bool IsWindowsRuntimeImport => false;

        internal sealed override TypeLayout Layout => default(TypeLayout);

        internal override CharSet MarshallingCharSet => base.DefaultMarshallingCharSet;

        public sealed override bool IsSerializable => false;

        internal sealed override bool HasDeclarativeSecurity => false;

        internal sealed override bool IsComImport => false;

        internal sealed override ObsoleteAttributeData? ObsoleteAttributeData => null;

        internal virtual bool Unreported => false;

        public sealed override bool AreLocalsZeroed
        {
            get
            {
                throw ExceptionUtilities.Unreachable;
            }
        }

        internal override NamedTypeSymbol? NativeIntegerUnderlyingType => null;

        internal sealed override bool IsRecord => false;

        internal override bool IsRecordStruct => false;

        internal TypeWithAnnotations Substitute(AbstractTypeMap typeMap)
        {
            return TypeWithAnnotations.Create(typeMap.SubstituteNamedType(this));
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            return new UseSiteInfo<AssemblySymbol>(ErrorInfo);
        }

        public override ImmutableArray<Symbol> GetMembers()
        {
            if (IsTupleType)
            {
                return AddOrWrapTupleMembers(ImmutableArray<Symbol>.Empty).ToImmutableAndFree();
            }
            return ImmutableArray<Symbol>.Empty;
        }

        public override ImmutableArray<Symbol> GetMembers(string name)
        {
            return GetMembers().WhereAsArray((Symbol m, string name) => m.Name == name, name);
        }

        internal sealed override IEnumerable<FieldSymbol> GetFieldsToEmit()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers()
        {
            return GetMembersUnordered();
        }

        internal override ImmutableArray<Symbol> GetEarlyAttributeDecodingMembers(string name)
        {
            return GetMembers(name);
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

        private ImmutableArray<TypeParameterSymbol> GetTypeParameters()
        {
            int arity = Arity;
            if (arity == 0)
            {
                return ImmutableArray<TypeParameterSymbol>.Empty;
            }
            TypeParameterSymbol[] array = new TypeParameterSymbol[arity];
            for (int i = 0; i < arity; i++)
            {
                array[i] = new ErrorTypeParameterSymbol(this, string.Empty, i);
            }
            return array.AsImmutableOrNull();
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitErrorType(this, argument);
        }

        internal ErrorTypeSymbol(TupleExtraData? tupleData = null)
            : base(tupleData)
        {
        }

        internal override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics(ConsList<TypeSymbol>? basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetInterfacesToEmit()
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        internal override NamedTypeSymbol? GetDeclaredBaseType(ConsList<TypeSymbol> basesBeingResolved)
        {
            return null;
        }

        internal override ImmutableArray<NamedTypeSymbol> GetDeclaredInterfaces(ConsList<TypeSymbol> basesBeingResolved)
        {
            return ImmutableArray<NamedTypeSymbol>.Empty;
        }

        protected override NamedTypeSymbol ConstructCore(ImmutableArray<TypeWithAnnotations> typeArguments, bool unbound)
        {
            return new ConstructedErrorTypeSymbol(this, typeArguments);
        }

        internal override NamedTypeSymbol AsMember(NamedTypeSymbol newOwner)
        {
            if (!newOwner.IsDefinition)
            {
                return new SubstitutedNestedErrorTypeSymbol(newOwner, this);
            }
            return this;
        }

        internal sealed override IEnumerable<SecurityAttribute> GetSecurityInformation()
        {
            throw ExceptionUtilities.Unreachable;
        }

        internal sealed override ImmutableArray<string> GetAppliedConditionalSymbols()
        {
            return ImmutableArray<string>.Empty;
        }

        internal override AttributeUsageInfo GetAttributeUsageInfo()
        {
            return AttributeUsageInfo.Null;
        }

        internal override NamedTypeSymbol AsNativeInteger()
        {
            throw ExceptionUtilities.Unreachable;
        }

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ErrorTypeSymbol(this, base.DefaultNullableAnnotation);
        }

        protected sealed override ITypeSymbol CreateITypeSymbol(Microsoft.CodeAnalysis.NullableAnnotation nullableAnnotation)
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ErrorTypeSymbol(this, nullableAnnotation);
        }

        internal sealed override bool HasPossibleWellKnownCloneMethod()
        {
            return false;
        }
    }
}
