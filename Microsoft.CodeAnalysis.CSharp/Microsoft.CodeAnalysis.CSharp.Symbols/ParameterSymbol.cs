using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.CSharp.Emit;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;

using Roslyn.Utilities;

#nullable enable

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    public abstract class ParameterSymbol : Symbol, IParameterTypeInformation, IParameterListEntry, IParameterDefinition, IDefinition, IReference, INamedEntity, IParameterSymbolInternal, ISymbolInternal
    {
        internal const string ValueParameterName = "value";

        ImmutableArray<ICustomModifier> IParameterTypeInformation.CustomModifiers => ImmutableArray<ICustomModifier>.CastUp(AdaptedParameterSymbol.TypeWithAnnotations.CustomModifiers);

        bool IParameterTypeInformation.IsByReference => AdaptedParameterSymbol.RefKind != RefKind.None;

        ImmutableArray<ICustomModifier> IParameterTypeInformation.RefCustomModifiers => ImmutableArray<ICustomModifier>.CastUp(AdaptedParameterSymbol.RefCustomModifiers);

        ushort IParameterListEntry.Index => (ushort)AdaptedParameterSymbol.Ordinal;

        bool IParameterDefinition.HasDefaultValue => AdaptedParameterSymbol.HasMetadataConstantValue;

        bool IParameterDefinition.IsOptional => AdaptedParameterSymbol.IsMetadataOptional;

        bool IParameterDefinition.IsIn => AdaptedParameterSymbol.IsMetadataIn;

        bool IParameterDefinition.IsMarshalledExplicitly => AdaptedParameterSymbol.IsMarshalledExplicitly;

        bool IParameterDefinition.IsOut => AdaptedParameterSymbol.IsMetadataOut;

        IMarshallingInformation IParameterDefinition.MarshallingInformation => AdaptedParameterSymbol.MarshallingInformation;

        ImmutableArray<byte> IParameterDefinition.MarshallingDescriptor => AdaptedParameterSymbol.MarshallingDescriptor;

        string INamedEntity.Name => AdaptedParameterSymbol.MetadataName;

        internal ParameterSymbol AdaptedParameterSymbol => this;

        internal virtual bool HasMetadataConstantValue
        {
            get
            {
                if (ExplicitDefaultConstantValue != null && ExplicitDefaultConstantValue!.SpecialType != SpecialType.System_Decimal)
                {
                    return ExplicitDefaultConstantValue!.SpecialType != SpecialType.System_DateTime;
                }
                return false;
            }
        }

        internal virtual bool IsMarshalledExplicitly => MarshallingInformation != null;

        internal virtual ImmutableArray<byte> MarshallingDescriptor => default(ImmutableArray<byte>);

        public new virtual ParameterSymbol OriginalDefinition => this;

        protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

        public abstract TypeWithAnnotations TypeWithAnnotations { get; }

        public TypeSymbol Type => TypeWithAnnotations.Type;

        public abstract RefKind RefKind { get; }

        public abstract bool IsDiscard { get; }

        public abstract ImmutableArray<CustomModifier> RefCustomModifiers { get; }

        internal abstract MarshalPseudoCustomAttributeData MarshallingInformation { get; }

        internal virtual UnmanagedType MarshallingType => MarshallingInformation?.UnmanagedType ?? 0;

        internal bool IsMarshalAsObject
        {
            get
            {
                UnmanagedType marshallingType = MarshallingType;
                if ((uint)(marshallingType - 25) <= 1u || marshallingType == UnmanagedType.Interface)
                {
                    return true;
                }
                return false;
            }
        }

        public abstract int Ordinal { get; }

        public abstract bool IsParams { get; }

        public bool IsOptional
        {
            get
            {
                if (!IsParams && IsMetadataOptional)
                {
                    RefKind refKind;
                    if ((refKind = RefKind) != 0)
                    {
                        switch (refKind)
                        {
                            case RefKind.Ref:
                                return ContainingSymbol.ContainingType.IsComImport;
                            default:
                                return false;
                            case RefKind.In:
                                break;
                        }
                    }
                    return true;
                }
                return false;
            }
        }

        internal abstract bool IsMetadataOptional { get; }

        internal abstract bool IsMetadataIn { get; }

        internal abstract bool IsMetadataOut { get; }

        public bool HasExplicitDefaultValue
        {
            get
            {
                if (IsOptional)
                {
                    return ExplicitDefaultConstantValue != null;
                }
                return false;
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public object ExplicitDefaultValue
        {
            get
            {
                if (HasExplicitDefaultValue)
                {
                    return ExplicitDefaultConstantValue!.Value;
                }
                throw new InvalidOperationException();
            }
        }

        internal abstract ConstantValue? ExplicitDefaultConstantValue { get; }

        public sealed override SymbolKind Kind => SymbolKind.Parameter;

        public override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

        public override bool IsAbstract => false;

        public override bool IsSealed => false;

        public override bool IsVirtual => false;

        public override bool IsOverride => false;

        public override bool IsStatic => false;

        public override bool IsExtern => false;

        public virtual bool IsThis => false;

        internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

        internal abstract bool IsIDispatchConstant { get; }

        internal abstract bool IsIUnknownConstant { get; }

        internal abstract bool IsCallerFilePath { get; }

        internal abstract bool IsCallerLineNumber { get; }

        internal abstract bool IsCallerMemberName { get; }

        internal abstract FlowAnalysisAnnotations FlowAnalysisAnnotations { get; }

        internal abstract ImmutableHashSet<string> NotNullIfParameterNotNull { get; }

        protected sealed override int HighestPriorityUseSiteError => 648;

        public sealed override bool HasUnsupportedMetadata
        {
            get
            {
                UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
                DeriveUseSiteInfoFromParameter(ref result, this);
                DiagnosticInfo? diagnosticInfo = result.DiagnosticInfo;
                if (diagnosticInfo == null)
                {
                    return false;
                }
                return diagnosticInfo!.Code == 648;
            }
        }

        ITypeReference IParameterTypeInformation.GetType(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate(AdaptedParameterSymbol.Type, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        MetadataConstant IParameterDefinition.GetDefaultValue(EmitContext context)
        {
            return GetMetadataConstantValue(context);
        }

        internal MetadataConstant GetMetadataConstantValue(EmitContext context)
        {
            if (!AdaptedParameterSymbol.HasMetadataConstantValue)
            {
                return null;
            }
            ConstantValue explicitDefaultConstantValue = AdaptedParameterSymbol.ExplicitDefaultConstantValue;
            TypeSymbol type = ((explicitDefaultConstantValue.SpecialType == SpecialType.None) ? AdaptedParameterSymbol.Type : AdaptedParameterSymbol.ContainingAssembly.GetSpecialType(explicitDefaultConstantValue.SpecialType));
            return ((PEModuleBuilder)context.Module).CreateConstant(type, explicitDefaultConstantValue.Value, (CSharpSyntaxNode)context.SyntaxNode, context.Diagnostics);
        }

        void IReference.Dispatch(MetadataVisitor visitor)
        {
            throw ExceptionUtilities.Unreachable;
        }

        IDefinition IReference.AsDefinition(EmitContext context)
        {
            PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
            if (AdaptedParameterSymbol.IsDefinition && AdaptedParameterSymbol.ContainingModule == pEModuleBuilder.SourceModule)
            {
                return this;
            }
            return null;
        }

        internal new ParameterSymbol GetCciAdapter()
        {
            return this;
        }

        internal ParameterSymbol()
        {
        }

        internal override TResult Accept<TArgument, TResult>(CSharpSymbolVisitor<TArgument, TResult> visitor, TArgument argument)
        {
            return visitor.VisitParameter(this, argument);
        }

        public override void Accept(CSharpSymbolVisitor visitor)
        {
            visitor.VisitParameter(this);
        }

        public override TResult Accept<TResult>(CSharpSymbolVisitor<TResult> visitor)
        {
            return visitor.VisitParameter(this);
        }

        protected override ISymbol CreateISymbol()
        {
            return new Microsoft.CodeAnalysis.CSharp.Symbols.PublicModel.ParameterSymbol(this);
        }
    }
}
