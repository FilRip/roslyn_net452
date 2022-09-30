using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class PropertySymbol : Symbol, IPropertyDefinition, ISignature, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        private ParameterSignature _lazyParameterSignature;

        MetadataConstant IPropertyDefinition.DefaultValue => null;

        IMethodReference IPropertyDefinition.Getter
        {
            get
            {
                MethodSymbol getMethod = AdaptedPropertySymbol.GetMethod;
                if ((object)getMethod != null || !AdaptedPropertySymbol.IsSealed)
                {
                    return getMethod?.GetCciAdapter();
                }
                return GetSynthesizedSealedAccessor(MethodKind.PropertyGet);
            }
        }

        bool IPropertyDefinition.HasDefaultValue => false;

        bool IPropertyDefinition.IsRuntimeSpecial => AdaptedPropertySymbol.HasRuntimeSpecialName;

        bool IPropertyDefinition.IsSpecialName => AdaptedPropertySymbol.HasSpecialName;

        ImmutableArray<IParameterDefinition> IPropertyDefinition.Parameters => StaticCast<IParameterDefinition>.From(AdaptedPropertySymbol.Parameters);

        IMethodReference IPropertyDefinition.Setter
        {
            get
            {
                MethodSymbol setMethod = AdaptedPropertySymbol.SetMethod;
                if ((object)setMethod != null || !AdaptedPropertySymbol.IsSealed)
                {
                    return setMethod?.GetCciAdapter();
                }
                return GetSynthesizedSealedAccessor(MethodKind.PropertySet);
            }
        }

        CallingConvention ISignature.CallingConvention => AdaptedPropertySymbol.CallingConvention;

        ushort ISignature.ParameterCount => (ushort)AdaptedPropertySymbol.ParameterCount;

        ImmutableArray<ICustomModifier> ISignature.ReturnValueCustomModifiers => AdaptedPropertySymbol.TypeWithAnnotations.CustomModifiers.As<ICustomModifier>();

        ImmutableArray<ICustomModifier> ISignature.RefCustomModifiers => AdaptedPropertySymbol.RefCustomModifiers.As<ICustomModifier>();

        bool ISignature.ReturnValueIsByRef => AdaptedPropertySymbol.RefKind.IsManagedReference();

        ITypeDefinition ITypeDefinitionMember.ContainingTypeDefinition => AdaptedPropertySymbol.ContainingType.GetCciAdapter();

        TypeMemberVisibility ITypeDefinitionMember.Visibility => PEModuleBuilder.MemberVisibility(AdaptedPropertySymbol);

        string INamedEntity.Name => AdaptedPropertySymbol.MetadataName;

        internal PropertySymbol AdaptedPropertySymbol => this;

        internal virtual bool HasRuntimeSpecialName => false;

        public new virtual PropertySymbol OriginalDefinition => this;

        protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

        internal virtual ImmutableArray<string> NotNullMembers => ImmutableArray<string>.Empty;

        internal virtual ImmutableArray<string> NotNullWhenTrueMembers => ImmutableArray<string>.Empty;

        internal virtual ImmutableArray<string> NotNullWhenFalseMembers => ImmutableArray<string>.Empty;

        public bool ReturnsByRef => RefKind == RefKind.Ref;

        public bool ReturnsByRefReadonly => RefKind == RefKind.In;

        public abstract RefKind RefKind { get; }

        public abstract TypeWithAnnotations TypeWithAnnotations { get; }

        public TypeSymbol Type => TypeWithAnnotations.Type;

        public abstract ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

        internal int ParameterCount => Parameters.Length;

        internal ImmutableArray<TypeWithAnnotations> ParameterTypesWithAnnotations
        {
            get
            {
                ParameterSignature.PopulateParameterSignature(Parameters, ref _lazyParameterSignature);
                return _lazyParameterSignature.parameterTypesWithAnnotations;
            }
        }

        internal ImmutableArray<RefKind> ParameterRefKinds
        {
            get
            {
                ParameterSignature.PopulateParameterSignature(Parameters, ref _lazyParameterSignature);
                return _lazyParameterSignature.parameterRefKinds;
            }
        }

        public virtual bool RequiresInstanceReceiver => !IsStatic;

        public abstract bool IsIndexer { get; }

        public virtual bool IsIndexedProperty => false;

        public bool IsReadOnly => (object)((PropertySymbol)this.GetLeastOverriddenMember(ContainingType)).SetMethod == null;

        public bool IsWriteOnly => (object)((PropertySymbol)this.GetLeastOverriddenMember(ContainingType)).GetMethod == null;

        internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

        internal abstract bool HasSpecialName { get; }

        public abstract MethodSymbol GetMethod { get; }

        public abstract MethodSymbol SetMethod { get; }

        internal abstract CallingConvention CallingConvention { get; }

        internal abstract bool MustCallMethodsDirectly { get; }

        public PropertySymbol OverriddenProperty
        {
            get
            {
                if (IsOverride)
                {
                    if (base.IsDefinition)
                    {
                        return (PropertySymbol)OverriddenOrHiddenMembers.GetOverriddenMember();
                    }
                    return (PropertySymbol)OverriddenOrHiddenMembersResult.GetOverriddenMember(this, OriginalDefinition.OverriddenProperty);
                }
                return null;
            }
        }

        internal virtual OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers => this.MakeOverriddenOrHiddenMembers();

        internal bool HidesBasePropertiesByName => (GetMethod ?? SetMethod)?.HidesBaseMethodsByName ?? false;

        internal virtual bool IsExplicitInterfaceImplementation => ExplicitInterfaceImplementations.Any();

        public abstract ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations { get; }

        public sealed override SymbolKind Kind => SymbolKind.Property;

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

        IEnumerable<IMethodReference> IPropertyDefinition.GetAccessors(EmitContext context)
        {
            MethodSymbol methodSymbol = AdaptedPropertySymbol.GetMethod?.GetCciAdapter();
            if (methodSymbol != null && methodSymbol.ShouldInclude(context))
            {
                yield return methodSymbol;
            }
            MethodSymbol methodSymbol2 = AdaptedPropertySymbol.SetMethod?.GetCciAdapter();
            if (methodSymbol2 != null && methodSymbol2.ShouldInclude(context))
            {
                yield return methodSymbol2;
            }
            if (AdaptedPropertySymbol is SourcePropertySymbolBase sourcePropertySymbolBase && this.ShouldInclude(context))
            {
                SynthesizedSealedPropertyAccessor synthesizedSealedAccessorOpt = sourcePropertySymbolBase.SynthesizedSealedAccessorOpt;
                if ((object)synthesizedSealedAccessorOpt != null)
                {
                    yield return synthesizedSealedAccessorOpt.GetCciAdapter();
                }
            }
        }

        [Conditional("DEBUG")]
        private void CheckDefinitionInvariantAllowEmbedded()
        {
        }

        ImmutableArray<IParameterTypeInformation> ISignature.GetParameters(EmitContext context)
        {
            return StaticCast<IParameterTypeInformation>.From(AdaptedPropertySymbol.Parameters);
        }

        ITypeReference ISignature.GetType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedPropertySymbol.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
        {
            return AdaptedPropertySymbol.ContainingType.GetCciAdapter();
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return this;
        }

        private IMethodReference GetSynthesizedSealedAccessor(MethodKind targetMethodKind)
        {
            if (AdaptedPropertySymbol is SourcePropertySymbolBase sourcePropertySymbolBase)
            {
                SynthesizedSealedPropertyAccessor synthesizedSealedAccessorOpt = sourcePropertySymbolBase.SynthesizedSealedAccessorOpt;
                if ((object)synthesizedSealedAccessorOpt == null || synthesizedSealedAccessorOpt.MethodKind != targetMethodKind)
                {
                    return null;
                }
                return synthesizedSealedAccessorOpt.GetCciAdapter();
            }
            return null;
        }

        internal new PropertySymbol GetCciAdapter()
        {
            return this;
        }

        internal PropertySymbol()
        {
        }

        internal PropertySymbol GetLeastOverriddenProperty(NamedTypeSymbol accessingTypeOpt)
        {
            accessingTypeOpt = accessingTypeOpt?.OriginalDefinition;
            PropertySymbol propertySymbol = this;
            while (propertySymbol.IsOverride && !propertySymbol.HidesBasePropertiesByName)
            {
                PropertySymbol overriddenProperty = propertySymbol.OverriddenProperty;
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                if ((object)overriddenProperty == null || ((object)accessingTypeOpt != null && !AccessCheck.IsSymbolAccessible(overriddenProperty, accessingTypeOpt, ref useSiteInfo)))
                {
                    break;
                }
                propertySymbol = overriddenProperty;
            }
            return propertySymbol;
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitProperty(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitProperty(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitProperty(this);
        }

        internal PropertySymbol AsMember(NamedTypeSymbol newOwner)
        {
            if (!newOwner.IsDefinition)
            {
                return new SubstitutedPropertySymbol(newOwner as SubstitutedNamedTypeSymbol, this);
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
            if (DeriveUseSiteInfoFromType(ref result, TypeWithAnnotations, AllowedRequiredModifierType.None) || DeriveUseSiteInfoFromCustomModifiers(ref result, RefCustomModifiers, AllowedRequiredModifierType.System_Runtime_InteropServices_InAttribute) || DeriveUseSiteInfoFromParameters(ref result, Parameters))
            {
                return true;
            }
            if (ContainingModule.HasUnifiedReferences)
            {
                HashSet<TypeSymbol> checkedTypes = null;
                DiagnosticInfo result2 = result.DiagnosticInfo;
                if (TypeWithAnnotations.GetUnificationUseSiteDiagnosticRecursive(ref result2, this, ref checkedTypes) || Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result2, RefCustomModifiers, this, ref checkedTypes) || Symbol.GetUnificationUseSiteDiagnosticRecursive(ref result2, Parameters, this, ref checkedTypes))
                {
                    result = result.AdjustDiagnosticInfo(result2);
                    return true;
                }
                result = result.AdjustDiagnosticInfo(result2);
            }
            return false;
        }

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.PropertySymbol(this);
        }

        public override bool Equals(Symbol symbol, TypeCompareKind compareKind)
        {
            if (!(symbol is PropertySymbol propertySymbol))
            {
                return false;
            }
            if ((object)this == propertySymbol)
            {
                return true;
            }
            if (propertySymbol is NativeIntegerPropertySymbol nativeIntegerPropertySymbol)
            {
                return nativeIntegerPropertySymbol.Equals(this, compareKind);
            }
            if (TypeSymbol.Equals(ContainingType, propertySymbol.ContainingType, compareKind))
            {
                return (object)OriginalDefinition == propertySymbol.OriginalDefinition;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int currentKey = 1;
            currentKey = Hash.Combine(ContainingType, currentKey);
            currentKey = Hash.Combine(Name, currentKey);
            return Hash.Combine(currentKey, ParameterCount);
        }
    }
}
