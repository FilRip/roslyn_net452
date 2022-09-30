using System.Collections.Generic;
using System.Collections.Immutable;
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
	internal abstract class FieldSymbol : Symbol, IFieldReference, IFieldDefinition, ITypeMemberReference, ITypeDefinitionMember, ISpecializedFieldReference, IFieldSymbol, IFieldSymbolInternal
	{
		private ISpecializedFieldReference IFieldReferenceAsSpecializedFieldReference
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

		private string INamedEntityName => AdaptedFieldSymbol.MetadataName;

		private bool IFieldReference_IsContextualNamedEntity => AdaptedFieldSymbol.IsContextualNamedEntity;

		private ImmutableArray<byte> IFieldDefinitionFieldMapping => default(ImmutableArray<byte>);

		private bool IFieldDefinitionIsCompileTimeConstant
		{
			get
			{
				if (AdaptedFieldSymbol.IsMetadataConstant)
				{
					return true;
				}
				return false;
			}
		}

		private bool IFieldDefinitionIsNotSerialized => AdaptedFieldSymbol.IsNotSerialized;

		private bool IFieldDefinitionIsReadOnly
		{
			get
			{
				if (!AdaptedFieldSymbol.IsReadOnly)
				{
					return AdaptedFieldSymbol.IsConstButNotMetadataConstant;
				}
				return true;
			}
		}

		private bool IFieldDefinitionIsRuntimeSpecial => AdaptedFieldSymbol.HasRuntimeSpecialName;

		private bool IFieldDefinitionIsSpecialName => AdaptedFieldSymbol.HasSpecialName;

		private bool IFieldDefinitionIsStatic => AdaptedFieldSymbol.IsShared;

		private bool IFieldDefinitionIsMarshalledExplicitly => AdaptedFieldSymbol.IsMarshalledExplicitly;

		private IMarshallingInformation IFieldDefinitionMarshallingInformation => AdaptedFieldSymbol.MarshallingInformation;

		private ImmutableArray<byte> IFieldDefinitionMarshallingDescriptor => AdaptedFieldSymbol.MarshallingDescriptor;

		private int IFieldDefinitionOffset => AdaptedFieldSymbol.TypeLayoutOffset.GetValueOrDefault();

		private ITypeDefinition ITypeDefinitionMemberContainingTypeDefinition => AdaptedFieldSymbol.ContainingType.GetCciAdapter();

		private TypeMemberVisibility ITypeDefinitionMemberVisibility => PEModuleBuilder.MemberVisibility(AdaptedFieldSymbol);

		private IFieldReference ISpecializedFieldReferenceUnspecializedVersion => AdaptedFieldSymbol.OriginalDefinition.GetCciAdapter();

		internal FieldSymbol AdaptedFieldSymbol => this;

		internal virtual bool IsContextualNamedEntity => false;

		internal virtual bool IsMarshalledExplicitly => MarshallingInformation != null;

		internal virtual ImmutableArray<byte> MarshallingDescriptor => default(ImmutableArray<byte>);

		public new virtual FieldSymbol OriginalDefinition => this;

		protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

		public abstract TypeSymbol Type { get; }

		internal virtual bool HasDeclaredType => true;

		public abstract ImmutableArray<CustomModifier> CustomModifiers { get; }

		public abstract Symbol AssociatedSymbol { get; }

		public abstract bool IsReadOnly { get; }

		public abstract bool IsConst { get; }

		internal bool IsMetadataConstant
		{
			get
			{
				if (IsConst)
				{
					SpecialType specialType = Type.SpecialType;
					return specialType != SpecialType.System_DateTime && specialType != SpecialType.System_Decimal;
				}
				return false;
			}
		}

		internal bool IsConstButNotMetadataConstant
		{
			get
			{
				if (IsConst)
				{
					SpecialType specialType = Type.SpecialType;
					return specialType == SpecialType.System_DateTime || specialType == SpecialType.System_Decimal;
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
				ConstantValue constantValue = GetConstantValue(ConstantFieldsInProgress.Empty);
				return (object)constantValue != null && !constantValue.IsBad;
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
				return GetConstantValue(ConstantFieldsInProgress.Empty)?.Value;
			}
		}

		public sealed override SymbolKind Kind => SymbolKind.Field;

		public sealed override bool IsMustOverride => false;

		public sealed override bool IsNotOverridable => false;

		public sealed override bool IsOverridable => false;

		public sealed override bool IsOverrides => false;

		internal abstract bool HasSpecialName { get; }

		internal abstract bool HasRuntimeSpecialName { get; }

		internal abstract bool IsNotSerialized { get; }

		internal abstract MarshalPseudoCustomAttributeData MarshallingInformation { get; }

		internal virtual UnmanagedType MarshallingType => MarshallingInformation?.UnmanagedType ?? ((UnmanagedType)0);

		internal abstract int? TypeLayoutOffset { get; }

		internal virtual ParameterSymbol MeParameter
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal virtual bool IsCapturedFrame => false;

		protected override int HighestPriorityUseSiteError => 30656;

		public sealed override bool HasUnsupportedMetadata
		{
			get
			{
				DiagnosticInfo diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					return diagnosticInfo.Code == 30656;
				}
				return false;
			}
		}

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => ContainingSymbol.EmbeddedSymbolKind;

		public virtual bool IsTupleField => false;

		public virtual bool IsVirtualTupleField => false;

		public virtual bool IsDefaultTupleElement => false;

		public virtual FieldSymbol TupleUnderlyingField => null;

		public virtual FieldSymbol CorrespondingTupleField => null;

		public virtual int TupleElementIndex => -1;

		private ISymbol IFieldSymbol_AssociatedSymbol => AssociatedSymbol;

		private bool IFieldSymbol_IsConst => IsConst;

		private bool IFieldSymbol_IsVolatile => false;

		private bool IFieldSymbol_IsFixedSizeBuffer => false;

		private ITypeSymbol IFieldSymbol_Type => Type;

		private NullableAnnotation IFieldSymbol_NullableAnnotation => NullableAnnotation.None;

		private bool IFieldSymbol_HasConstantValue => HasConstantValue;

		private object IFieldSymbol_ConstantValue => ConstantValue;

		private ImmutableArray<CustomModifier> IFieldSymbol_CustomModifiers => CustomModifiers;

		private IFieldSymbol IFieldSymbol_OriginalDefinition => OriginalDefinition;

		private IFieldSymbol IFieldSymbol_CorrespondingTupleField => CorrespondingTupleField;

		private bool IFieldSymbol_HasExplicitTupleElementName => !IsImplicitlyDeclared;

		private ITypeReference IFieldReferenceGetType(EmitContext context)
		{
			PEModuleBuilder obj = (PEModuleBuilder)context.Module;
			ImmutableArray<CustomModifier> customModifiers = AdaptedFieldSymbol.CustomModifiers;
			ITypeReference typeReference = obj.Translate(AdaptedFieldSymbol.Type, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			if (customModifiers.Length == 0)
			{
				return typeReference;
			}
			return new ModifiedTypeReference(typeReference, customModifiers.As<ICustomModifier>());
		}

		ITypeReference IFieldReference.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFieldReferenceGetType
			return this.IFieldReferenceGetType(context);
		}

		private IFieldDefinition IFieldReferenceGetResolvedField(EmitContext context)
		{
			return ResolvedFieldImpl((PEModuleBuilder)context.Module);
		}

		IFieldDefinition IFieldReference.GetResolvedField(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFieldReferenceGetResolvedField
			return this.IFieldReferenceGetResolvedField(context);
		}

		private IFieldDefinition ResolvedFieldImpl(PEModuleBuilder moduleBeingBuilt)
		{
			if (AdaptedFieldSymbol.IsDefinition && AdaptedFieldSymbol.ContainingModule == moduleBeingBuilt.SourceModule)
			{
				return this;
			}
			return null;
		}

		private ITypeReference ITypeMemberReferenceGetContainingType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(AdaptedFieldSymbol.ContainingType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, AdaptedFieldSymbol.IsDefinition);
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeMemberReferenceGetContainingType
			return this.ITypeMemberReferenceGetContainingType(context);
		}

		internal sealed override void IReferenceDispatch(MetadataVisitor visitor)
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

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
			return ResolvedFieldImpl(moduleBeingBuilt);
		}

		private MetadataConstant IFieldDefinition_GetCompileTimeValue(EmitContext context)
		{
			return GetMetadataConstantValue(context);
		}

		MetadataConstant IFieldDefinition.GetCompileTimeValue(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IFieldDefinition_GetCompileTimeValue
			return this.IFieldDefinition_GetCompileTimeValue(context);
		}

		internal MetadataConstant GetMetadataConstantValue(EmitContext context)
		{
			if (AdaptedFieldSymbol.IsMetadataConstant)
			{
				return ((PEModuleBuilder)context.Module).CreateConstant(AdaptedFieldSymbol.Type, RuntimeHelpers.GetObjectValue(AdaptedFieldSymbol.ConstantValue), (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
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

		internal abstract ConstantValue GetConstantValue(ConstantFieldsInProgress inProgress);

		internal virtual TypeSymbol GetInferredType(ConstantFieldsInProgress inProgress)
		{
			return Type;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitField(this, arg);
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			if (base.IsDefinition)
			{
				return new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency);
			}
			return OriginalDefinition.GetUseSiteInfo();
		}

		internal UseSiteInfo<AssemblySymbol> CalculateUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = MergeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency), DeriveUseSiteInfoFromType(Type));
			DiagnosticInfo? diagnosticInfo = useSiteInfo.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == 30656)
			{
				return useSiteInfo;
			}
			UseSiteInfo<AssemblySymbol> useSiteInfo2 = DeriveUseSiteInfoFromCustomModifiers(CustomModifiers);
			DiagnosticInfo? diagnosticInfo2 = useSiteInfo2.DiagnosticInfo;
			if (diagnosticInfo2 != null && diagnosticInfo2!.Code == 30656)
			{
				return useSiteInfo2;
			}
			useSiteInfo = MergeUseSiteInfo(useSiteInfo, useSiteInfo2);
			if (useSiteInfo.DiagnosticInfo == null && ContainingModule.HasUnifiedReferences)
			{
				HashSet<TypeSymbol> checkedTypes = null;
				DiagnosticInfo diagnosticInfo3 = Type.GetUnificationUseSiteDiagnosticRecursive(this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(CustomModifiers, this, ref checkedTypes);
				if (diagnosticInfo3 != null)
				{
					useSiteInfo = new UseSiteInfo<AssemblySymbol>(diagnosticInfo3);
				}
			}
			return useSiteInfo;
		}

		internal FieldSymbol AsMember(NamedTypeSymbol newOwner)
		{
			if (!TypeSymbol.Equals(newOwner, ContainingType, TypeCompareKind.ConsiderEverything))
			{
				return (FieldSymbol)((SubstitutedNamedType)newOwner).GetMemberForDefinition(this);
			}
			return this;
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitField(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitField(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitField(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitField(this);
		}
	}
}
