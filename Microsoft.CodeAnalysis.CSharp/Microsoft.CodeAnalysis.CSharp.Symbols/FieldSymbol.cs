using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.InteropServices;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class FieldSymbol : Symbol, IFieldReference, ITypeMemberReference, IReference, INamedEntity, IFieldDefinition, ITypeDefinitionMember, IDefinition, ISpecializedFieldReference, IFieldSymbolInternal, ISymbolInternal
    {
        ISpecializedFieldReference IFieldReference.AsSpecializedFieldReference
        {
            get
            {
                if (!AdaptedFieldSymbol.IsDefinition)
                {
                    return this;
                }
                return null;
            }
        }

        string INamedEntity.Name => AdaptedFieldSymbol.MetadataName;

        bool IFieldReference.IsContextualNamedEntity => false;

        ImmutableArray<byte> IFieldDefinition.MappedData => default(ImmutableArray<byte>);

        bool IFieldDefinition.IsCompileTimeConstant => AdaptedFieldSymbol.IsMetadataConstant;

        bool IFieldDefinition.IsNotSerialized => AdaptedFieldSymbol.IsNotSerialized;

        bool IFieldDefinition.IsReadOnly
        {
            get
            {
                if (!AdaptedFieldSymbol.IsReadOnly)
                {
                    if (AdaptedFieldSymbol.IsConst)
                    {
                        return !AdaptedFieldSymbol.IsMetadataConstant;
                    }
                    return false;
                }
                return true;
            }
        }

        bool IFieldDefinition.IsRuntimeSpecial => AdaptedFieldSymbol.HasRuntimeSpecialName;

        bool IFieldDefinition.IsSpecialName => AdaptedFieldSymbol.HasSpecialName;

        bool IFieldDefinition.IsStatic => AdaptedFieldSymbol.IsStatic;

        bool IFieldDefinition.IsMarshalledExplicitly => AdaptedFieldSymbol.IsMarshalledExplicitly;

        IMarshallingInformation IFieldDefinition.MarshallingInformation => AdaptedFieldSymbol.MarshallingInformation;

        ImmutableArray<byte> IFieldDefinition.MarshallingDescriptor => AdaptedFieldSymbol.MarshallingDescriptor;

        int IFieldDefinition.Offset => AdaptedFieldSymbol.TypeLayoutOffset.GetValueOrDefault();

        ITypeDefinition ITypeDefinitionMember.ContainingTypeDefinition => AdaptedFieldSymbol.ContainingType.GetCciAdapter();

        TypeMemberVisibility ITypeDefinitionMember.Visibility => PEModuleBuilder.MemberVisibility(AdaptedFieldSymbol);

        IFieldReference ISpecializedFieldReference.UnspecializedVersion => AdaptedFieldSymbol.OriginalDefinition.GetCciAdapter();

        internal FieldSymbol AdaptedFieldSymbol => this;

        internal virtual bool IsMarshalledExplicitly => MarshallingInformation != null;

        internal virtual ImmutableArray<byte> MarshallingDescriptor => default(ImmutableArray<byte>);

        public new virtual FieldSymbol OriginalDefinition => this;

        protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

        public TypeWithAnnotations TypeWithAnnotations => GetFieldType(ConsList<FieldSymbol>.Empty);

        public abstract FlowAnalysisAnnotations FlowAnalysisAnnotations { get; }

        public TypeSymbol Type => TypeWithAnnotations.Type;

        public abstract Symbol AssociatedSymbol { get; }

        public abstract bool IsReadOnly { get; }

        public abstract bool IsVolatile { get; }

        public virtual bool RequiresInstanceReceiver => !IsStatic;

        public virtual bool IsFixedSizeBuffer => false;

        public virtual int FixedSize => 0;

        internal virtual bool IsCapturedFrame => false;

        public abstract bool IsConst { get; }

        public bool IsMetadataConstant
        {
            get
            {
                if (IsConst)
                {
                    return Type.SpecialType != SpecialType.System_Decimal;
                }
                return false;
            }
        }

        public virtual bool HasConstantValue
        {
            get
            {
                if (!IsConst)
                {
                    return false;
                }
                ConstantValue constantValue = GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false);
                if (constantValue != null)
                {
                    return !constantValue.IsBad;
                }
                return false;
            }
        }

        public virtual object ConstantValue
        {
            get
            {
                if (!IsConst)
                {
                    return null;
                }
                ConstantValue constantValue = GetConstantValue(ConstantFieldsInProgress.Empty, earlyDecodingWellKnownAttributes: false);
                if (!(constantValue == null))
                {
                    return constantValue.Value;
                }
                return null;
            }
        }

        public sealed override SymbolKind Kind => SymbolKind.Field;

        public sealed override bool IsAbstract => false;

        public sealed override bool IsExtern => false;

        public sealed override bool IsOverride => false;

        public sealed override bool IsSealed => false;

        public sealed override bool IsVirtual => false;

        internal abstract bool HasSpecialName { get; }

        internal abstract bool HasRuntimeSpecialName { get; }

        internal abstract bool IsNotSerialized { get; }

        internal virtual bool HasPointerType => Type.IsPointerOrFunctionPointer();

        internal abstract MarshalPseudoCustomAttributeData MarshallingInformation { get; }

        internal virtual UnmanagedType MarshallingType => MarshallingInformation?.UnmanagedType ?? 0;

        internal abstract int? TypeLayoutOffset { get; }

        protected override int HighestPriorityUseSiteError => 570;

        public sealed override bool HasUnsupportedMetadata
        {
            get
            {
                DiagnosticInfo diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
                if (diagnosticInfo != null)
                {
                    return diagnosticInfo.Code == 570;
                }
                return false;
            }
        }

        public virtual bool IsVirtualTupleField => false;

        public virtual bool IsDefaultTupleElement => TupleElementIndex >= 0;

        public virtual bool IsExplicitlyNamedTupleElement => false;

        public virtual FieldSymbol TupleUnderlyingField
        {
            get
            {
                if (!ContainingType.IsTupleType)
                {
                    return null;
                }
                return this;
            }
        }

        public virtual FieldSymbol CorrespondingTupleField
        {
            get
            {
                if (TupleElementIndex < 0)
                {
                    return null;
                }
                return this;
            }
        }

        public virtual int TupleElementIndex
        {
            get
            {
                SmallDictionary<FieldSymbol, int> tupleFieldDefinitionsToIndexMap = ContainingType.TupleFieldDefinitionsToIndexMap;
                if (tupleFieldDefinitionsToIndexMap != null && tupleFieldDefinitionsToIndexMap.TryGetValue(OriginalDefinition, out var value))
                {
                    return value;
                }
                return -1;
            }
        }

        bool IFieldSymbolInternal.IsVolatile => IsVolatile;

        ITypeReference IFieldReference.GetType(EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            TypeWithAnnotations typeWithAnnotations = AdaptedFieldSymbol.TypeWithAnnotations;
            ImmutableArray<CustomModifier> customModifiers = typeWithAnnotations.CustomModifiers;
            bool isFixedSizeBuffer = AdaptedFieldSymbol.IsFixedSizeBuffer;
            TypeSymbol symbol = (isFixedSizeBuffer ? AdaptedFieldSymbol.FixedImplementationType(pEModuleBuilder) : typeWithAnnotations.Type);
            ITypeReference typeReference = pEModuleBuilder.Translate(symbol, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
            if (isFixedSizeBuffer || customModifiers.Length == 0)
            {
                return typeReference;
            }
            return new ModifiedTypeReference(typeReference, ImmutableArray<ICustomModifier>.CastUp(customModifiers));
        }

        IFieldDefinition IFieldReference.GetResolvedField(EmitContext context)
        {
            return ResolvedFieldImpl((PEModuleBuilder)context.Module);
        }

        private IFieldDefinition ResolvedFieldImpl(PEModuleBuilder moduleBeingBuilt)
        {
            if (AdaptedFieldSymbol.IsDefinition && AdaptedFieldSymbol.ContainingModule == moduleBeingBuilt.SourceModule)
            {
                return this;
            }
            return null;
        }

        ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedFieldSymbol.ContainingType, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, AdaptedFieldSymbol.IsDefinition);
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            if (!AdaptedFieldSymbol.IsDefinition)
            {
                visitor.Visit((IFieldReference)this);
            }
            else if (AdaptedFieldSymbol.ContainingModule == ((PEModuleBuilder)visitor.Context.Module).SourceModule)
            {
                visitor.Visit(this);
            }
            else
            {
                visitor.Visit((IFieldReference)this);
            }
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
            return ResolvedFieldImpl(moduleBeingBuilt);
        }

        MetadataConstant IFieldDefinition.GetCompileTimeValue(EmitContext context)
        {
            return GetMetadataConstantValue(context);
        }

        internal MetadataConstant GetMetadataConstantValue(EmitContext context)
        {
            if (AdaptedFieldSymbol.IsMetadataConstant)
            {
                return ((PEModuleBuilder)context.Module).CreateConstant(AdaptedFieldSymbol.Type, AdaptedFieldSymbol.ConstantValue, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
            }
            return null;
        }

        internal new FieldSymbol GetCciAdapter()
        {
            return this;
        }

        internal FieldSymbol()
        {
        }

        internal abstract TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound);

        internal virtual NamedTypeSymbol FixedImplementationType(PEModuleBuilder emitModule)
        {
            return null;
        }

        internal abstract ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress, bool earlyDecodingWellKnownAttributes);

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitField(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitField(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitField(this);
        }

        internal virtual FieldSymbol AsMember(NamedTypeSymbol newOwner)
        {
            if (!newOwner.IsDefinition)
            {
                return new SubstitutedFieldSymbol(newOwner as SubstitutedNamedTypeSymbol, this);
            }
            return this;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            if (base.IsDefinition)
            {
                return new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency);
            }
            return OriginalDefinition.GetUseSiteInfo();
        }

        internal bool CalculateUseSiteDiagnostic(ref UseSiteInfo<AssemblySymbol> result)
        {
            if (DeriveUseSiteInfoFromType(ref result, TypeWithAnnotations, AllowedRequiredModifierType.System_Runtime_CompilerServices_Volatile))
            {
                return true;
            }
            if (ContainingModule.HasUnifiedReferences)
            {
                HashSet<TypeSymbol> checkedTypes = null;
                DiagnosticInfo result2 = result.DiagnosticInfo;
                if (TypeWithAnnotations.GetUnificationUseSiteDiagnosticRecursive(ref result2, this, ref checkedTypes))
                {
                    result = result.AdjustDiagnosticInfo(result2);
                    return true;
                }
                result = result.AdjustDiagnosticInfo(result2);
            }
            return false;
        }

        internal bool IsTupleElement()
        {
            return (object)CorrespondingTupleField != null;
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.FieldSymbol(this);
        }

        public override bool Equals(Symbol other, TypeCompareKind compareKind)
        {
            if (other is SubstitutedFieldSymbol substitutedFieldSymbol)
            {
                return substitutedFieldSymbol.Equals(this, compareKind);
            }
            return base.Equals(other, compareKind);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
