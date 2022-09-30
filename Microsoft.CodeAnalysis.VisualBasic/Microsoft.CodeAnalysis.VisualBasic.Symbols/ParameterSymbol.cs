using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class ParameterSymbol : Symbol, IParameterTypeInformation, IParameterDefinition, IParameterSymbol, IParameterSymbolInternal
	{
		private ImmutableArray<ICustomModifier> IParameterTypeInformationCustomModifiers => AdaptedParameterSymbol.CustomModifiers.As<ICustomModifier>();

		private ImmutableArray<ICustomModifier> IParameterTypeInformationRefCustomModifiers => AdaptedParameterSymbol.RefCustomModifiers.As<ICustomModifier>();

		private bool IParameterTypeInformationIsByReference => AdaptedParameterSymbol.IsByRef;

		private ushort IParameterListEntryIndex => (ushort)AdaptedParameterSymbol.Ordinal;

		private bool IParameterDefinition_HasDefaultValue => AdaptedParameterSymbol.HasMetadataConstantValue;

		private bool IParameterDefinitionIsOptional => AdaptedParameterSymbol.IsMetadataOptional;

		private bool IParameterDefinitionIsIn => AdaptedParameterSymbol.IsMetadataIn;

		private bool IParameterDefinitionIsOut => AdaptedParameterSymbol.IsMetadataOut;

		private bool IParameterDefinitionIsMarshalledExplicitly => AdaptedParameterSymbol.IsMarshalledExplicitly;

		private IMarshallingInformation IParameterDefinitionMarshallingInformation => AdaptedParameterSymbol.MarshallingInformation;

		private ImmutableArray<byte> IParameterDefinitionMarshallingDescriptor => AdaptedParameterSymbol.MarshallingDescriptor;

		private string INamedEntityName => AdaptedParameterSymbol.MetadataName;

		internal ParameterSymbol AdaptedParameterSymbol => this;

		internal virtual bool HasMetadataConstantValue
		{
			get
			{
				if (HasExplicitDefaultValue)
				{
					ConstantValue explicitDefaultConstantValue = ExplicitDefaultConstantValue;
					return explicitDefaultConstantValue.Discriminator != ConstantValueTypeDiscriminator.DateTime && explicitDefaultConstantValue.Discriminator != ConstantValueTypeDiscriminator.Decimal;
				}
				return false;
			}
		}

		internal virtual bool IsMetadataOptional
		{
			get
			{
				if (!IsOptional)
				{
					return GetAttributes().Any((VisualBasicAttributeData a) => a.IsTargetAttribute(this, AttributeDescription.OptionalAttribute));
				}
				return true;
			}
		}

		internal virtual bool IsMarshalledExplicitly => MarshallingInformation != null;

		internal virtual ImmutableArray<byte> MarshallingDescriptor => default(ImmutableArray<byte>);

		public new virtual ParameterSymbol OriginalDefinition => this;

		protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

		public abstract bool IsByRef { get; }

		internal abstract bool IsExplicitByRef { get; }

		internal abstract bool IsMetadataOut { get; }

		internal abstract bool IsMetadataIn { get; }

		internal bool IsOut
		{
			get
			{
				if (IsByRef && IsMetadataOut)
				{
					return !IsMetadataIn;
				}
				return false;
			}
		}

		internal abstract MarshalPseudoCustomAttributeData MarshallingInformation { get; }

		internal virtual UnmanagedType MarshallingType => MarshallingInformation?.UnmanagedType ?? ((UnmanagedType)0);

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

		public abstract TypeSymbol Type { get; }

		public abstract ImmutableArray<CustomModifier> CustomModifiers { get; }

		public abstract ImmutableArray<CustomModifier> RefCustomModifiers { get; }

		public abstract int Ordinal { get; }

		public abstract bool IsParamArray { get; }

		public abstract bool IsOptional { get; }

		public abstract bool HasExplicitDefaultValue { get; }

		public object ExplicitDefaultValue
		{
			get
			{
				if (HasExplicitDefaultValue)
				{
					return ExplicitDefaultConstantValue.Value;
				}
				throw new InvalidOperationException();
			}
		}

		internal sealed override ObsoleteAttributeData ObsoleteAttributeData => null;

		internal abstract ConstantValue ExplicitDefaultConstantValue { get; }

		internal ConstantValue ExplicitDefaultConstantValue => this.get_ExplicitDefaultConstantValue(SymbolsInProgress<ParameterSymbol>.Empty);

		internal abstract bool HasOptionCompare { get; }

		public sealed override SymbolKind Kind => SymbolKind.Parameter;

		public sealed override Accessibility DeclaredAccessibility => Accessibility.NotApplicable;

		public sealed override bool IsMustOverride => false;

		public sealed override bool IsNotOverridable => false;

		public sealed override bool IsOverridable => false;

		public sealed override bool IsOverrides => false;

		public sealed override bool IsShared => false;

		public virtual bool IsMe => false;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => ContainingSymbol.EmbeddedSymbolKind;

		internal abstract bool IsIDispatchConstant { get; }

		internal abstract bool IsIUnknownConstant { get; }

		internal abstract bool IsCallerLineNumber { get; }

		internal abstract bool IsCallerMemberName { get; }

		internal abstract bool IsCallerFilePath { get; }

		protected override int HighestPriorityUseSiteError => 30649;

		public sealed override bool HasUnsupportedMetadata
		{
			get
			{
				DiagnosticInfo diagnosticInfo = DeriveUseSiteInfoFromParameter(this, HighestPriorityUseSiteError).DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					return diagnosticInfo.Code == 30649;
				}
				return false;
			}
		}

		private bool IParameterSymbol_IsDiscard => false;

		private RefKind IParameterSymbol_RefKind
		{
			get
			{
				if (!IsByRef)
				{
					return RefKind.None;
				}
				return RefKind.Ref;
			}
		}

		private ITypeSymbol IParameterSymbol_Type => Type;

		private NullableAnnotation IParameterSymbol_NullableAnnotation => NullableAnnotation.None;

		private bool IParameterSymbol_IsOptional => IsOptional;

		private bool IParameterSymbol_IsThis => IsMe;

		private ImmutableArray<CustomModifier> IParameterSymbol_RefCustomModifiers => RefCustomModifiers;

		private ImmutableArray<CustomModifier> IParameterSymbol_CustomModifiers => CustomModifiers;

		private int IParameterSymbol_Ordinal => Ordinal;

		private object IParameterSymbol_DefaultValue => ExplicitDefaultValue;

		private IParameterSymbol IParameterSymbol_OriginalDefinition => OriginalDefinition;

		private ITypeReference IParameterTypeInformationGetType(EmitContext context)
		{
			PEModuleBuilder obj = (PEModuleBuilder)context.Module;
			TypeSymbol type = AdaptedParameterSymbol.Type;
			return obj.Translate(type, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference IParameterTypeInformation.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IParameterTypeInformationGetType
			return this.IParameterTypeInformationGetType(context);
		}

		private MetadataConstant IParameterDefinition_GetDefaultValue(EmitContext context)
		{
			return GetMetadataConstantValue(context);
		}

		MetadataConstant IParameterDefinition.GetDefaultValue(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IParameterDefinition_GetDefaultValue
			return this.IParameterDefinition_GetDefaultValue(context);
		}

		internal MetadataConstant GetMetadataConstantValue(EmitContext context)
		{
			if (AdaptedParameterSymbol.HasMetadataConstantValue)
			{
				return ((PEModuleBuilder)context.Module).CreateConstant(AdaptedParameterSymbol.Type, RuntimeHelpers.GetObjectValue(AdaptedParameterSymbol.ExplicitDefaultConstantValue.Value), (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			}
			return null;
		}

		internal sealed override void IReferenceDispatch(MetadataVisitor visitor)
		{
			if (!AdaptedParameterSymbol.IsDefinition)
			{
				visitor.Visit((IParameterTypeInformation)this);
			}
			else if (AdaptedParameterSymbol.ContainingModule == ((PEModuleBuilder)visitor.Context.Module).SourceModule)
			{
				visitor.Visit(this);
			}
			else
			{
				visitor.Visit((IParameterTypeInformation)this);
			}
		}

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
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

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitParameter(this, arg);
		}

		internal ParameterSymbol()
		{
		}

		internal virtual ParameterSymbol ChangeOwner(Symbol newContainingSymbol)
		{
			throw ExceptionUtilities.Unreachable;
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitParameter(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitParameter(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitParameter(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitParameter(this);
		}
	}
}
