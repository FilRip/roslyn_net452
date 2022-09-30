using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class EventSymbol : Symbol, IEventDefinition, ITypeDefinitionMember, ITypeMemberReference, IReference, INamedEntity, IDefinition
    {
        IMethodReference IEventDefinition.Adder => AdaptedEventSymbol.AddMethod?.GetCciAdapter();

        IMethodReference IEventDefinition.Remover => AdaptedEventSymbol.RemoveMethod?.GetCciAdapter();

        bool IEventDefinition.IsRuntimeSpecial => AdaptedEventSymbol.HasRuntimeSpecialName;

        bool IEventDefinition.IsSpecialName => AdaptedEventSymbol.HasSpecialName;

        IMethodReference? IEventDefinition.Caller => null;

        ITypeDefinition ITypeDefinitionMember.ContainingTypeDefinition => AdaptedEventSymbol.ContainingType.GetCciAdapter();

        TypeMemberVisibility ITypeDefinitionMember.Visibility => PEModuleBuilder.MemberVisibility(AdaptedEventSymbol);

        string INamedEntity.Name => AdaptedEventSymbol.MetadataName;

        internal EventSymbol AdaptedEventSymbol => this;

        internal virtual bool HasRuntimeSpecialName => false;

        public new virtual EventSymbol OriginalDefinition => this;

        protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

        public abstract TypeWithAnnotations TypeWithAnnotations { get; }

        public TypeSymbol Type => TypeWithAnnotations.Type;

        public abstract MethodSymbol? AddMethod { get; }

        public abstract MethodSymbol? RemoveMethod { get; }

        internal bool HasAssociatedField => (object)AssociatedField != null;

        public virtual bool RequiresInstanceReceiver => !IsStatic;

        public abstract bool IsWindowsRuntimeEvent { get; }

        internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

        internal abstract bool HasSpecialName { get; }

        internal virtual FieldSymbol? AssociatedField => null;

        public EventSymbol? OverriddenEvent
        {
            get
            {
                if (IsOverride)
                {
                    if (base.IsDefinition)
                    {
                        return (EventSymbol)OverriddenOrHiddenMembers.GetOverriddenMember();
                    }
                    return (EventSymbol)OverriddenOrHiddenMembersResult.GetOverriddenMember(this, OriginalDefinition.OverriddenEvent);
                }
                return null;
            }
        }

        internal virtual OverriddenOrHiddenMembersResult OverriddenOrHiddenMembers => this.MakeOverriddenOrHiddenMembers();

        internal bool HidesBaseEventsByName => (AddMethod ?? RemoveMethod)?.HidesBaseMethodsByName ?? false;

        internal virtual bool IsExplicitInterfaceImplementation => ExplicitInterfaceImplementations.Any();

        public abstract ImmutableArray<EventSymbol> ExplicitInterfaceImplementations { get; }

        public sealed override SymbolKind Kind => SymbolKind.Event;

        internal abstract bool MustCallMethodsDirectly { get; }

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

        IEnumerable<IMethodReference> IEventDefinition.GetAccessors(EmitContext context)
        {
            MethodSymbol methodSymbol = AdaptedEventSymbol.AddMethod?.GetCciAdapter();
            if (methodSymbol.ShouldInclude(context))
            {
                yield return methodSymbol;
            }
            MethodSymbol methodSymbol2 = AdaptedEventSymbol.RemoveMethod?.GetCciAdapter();
            if (methodSymbol2.ShouldInclude(context))
            {
                yield return methodSymbol2;
            }
        }

        ITypeReference IEventDefinition.GetType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedEventSymbol.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
        {
            return AdaptedEventSymbol.ContainingType.GetCciAdapter();
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            visitor.Visit(this);
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            return this;
        }

        internal new EventSymbol GetCciAdapter()
        {
            return this;
        }

        internal EventSymbol()
        {
        }

        public ImmutableArray<CSharpAttributeData> GetFieldAttributes()
        {
            if ((object)AssociatedField != null)
            {
                return AssociatedField!.GetAttributes();
            }
            return ImmutableArray<CSharpAttributeData>.Empty;
        }

        internal EventSymbol GetLeastOverriddenEvent(NamedTypeSymbol? accessingTypeOpt)
        {
            accessingTypeOpt = accessingTypeOpt?.OriginalDefinition;
            EventSymbol eventSymbol = this;
            while (eventSymbol.IsOverride && !eventSymbol.HidesBaseEventsByName)
            {
                EventSymbol overriddenEvent = eventSymbol.OverriddenEvent;
                CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
                if ((object)overriddenEvent == null || ((object)accessingTypeOpt != null && !AccessCheck.IsSymbolAccessible(overriddenEvent, accessingTypeOpt, ref useSiteInfo)))
                {
                    break;
                }
                eventSymbol = overriddenEvent;
            }
            return eventSymbol;
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitEvent(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitEvent(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitEvent(this);
        }

        internal EventSymbol AsMember(NamedTypeSymbol newOwner)
        {
            if (!newOwner.IsDefinition)
            {
                return new SubstitutedEventSymbol(newOwner as SubstitutedNamedTypeSymbol, this);
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
            if (DeriveUseSiteInfoFromType(ref result, TypeWithAnnotations, AllowedRequiredModifierType.None))
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

        protected sealed override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.EventSymbol(this);
        }

        public override bool Equals(Symbol? obj, TypeCompareKind compareKind)
        {
            if (!(obj is EventSymbol eventSymbol))
            {
                return false;
            }
            if ((object)this == eventSymbol)
            {
                return true;
            }
            if (TypeSymbol.Equals(ContainingType, eventSymbol.ContainingType, compareKind))
            {
                return (object)OriginalDefinition == eventSymbol.OriginalDefinition;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int currentKey = 1;
            currentKey = Hash.Combine(ContainingType, currentKey);
            return Hash.Combine(Name, currentKey);
        }
    }
}
