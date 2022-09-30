using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class MethodSymbol : Symbol, ITypeMemberReference, IMethodReference, IGenericMethodInstanceReference, ISpecializedMethodReference, ITypeDefinitionMember, IMethodDefinition, IMethodSymbol, IMethodSymbolInternal
	{
		public const MethodImplAttributes DisableJITOptimizationFlags = (MethodImplAttributes)72;

		private IGenericMethodInstanceReference IMethodReferenceAsGenericMethodInstanceReference
		{
			get
			{
				if (!AdaptedMethodSymbol.IsDefinition && AdaptedMethodSymbol.IsGenericMethod && (object)AdaptedMethodSymbol != AdaptedMethodSymbol.ConstructedFrom)
				{
					return this;
				}
				return null;
			}
		}

		private ISpecializedMethodReference IMethodReferenceAsSpecializedMethodReference
		{
			get
			{
				if (!AdaptedMethodSymbol.IsDefinition && (!AdaptedMethodSymbol.IsGenericMethod || (object)AdaptedMethodSymbol == AdaptedMethodSymbol.ConstructedFrom))
				{
					return this;
				}
				return null;
			}
		}

		private string INamedEntityName => AdaptedMethodSymbol.MetadataName;

		private bool IMethodReferenceAcceptsExtraArguments => AdaptedMethodSymbol.IsVararg;

		private ushort IMethodReferenceGenericParameterCount => (ushort)AdaptedMethodSymbol.Arity;

		private bool IMethodReferenceIsGeneric => AdaptedMethodSymbol.IsGenericMethod;

		private ushort IMethodReferenceParameterCount => (ushort)AdaptedMethodSymbol.ParameterCount;

		private ImmutableArray<IParameterTypeInformation> IMethodReferenceExtraParameters => ImmutableArray<IParameterTypeInformation>.Empty;

		private CallingConvention ISignatureCallingConvention => AdaptedMethodSymbol.CallingConvention;

		private ImmutableArray<ICustomModifier> ISignatureReturnValueCustomModifiers => AdaptedMethodSymbol.ReturnTypeCustomModifiers.As<ICustomModifier>();

		private ImmutableArray<ICustomModifier> ISignatureRefCustomModifiers => AdaptedMethodSymbol.RefCustomModifiers.As<ICustomModifier>();

		private bool ISignatureReturnValueIsByRef => AdaptedMethodSymbol.ReturnsByRef;

		private IMethodReference ISpecializedMethodReferenceUnspecializedVersion => AdaptedMethodSymbol.OriginalDefinition.GetCciAdapter();

		private ITypeDefinition ITypeDefinitionMemberContainingTypeDefinition
		{
			get
			{
				if (AdaptedMethodSymbol is SynthesizedGlobalMethodBase synthesizedGlobalMethodBase)
				{
					return synthesizedGlobalMethodBase.ContainingPrivateImplementationDetailsType;
				}
				return AdaptedMethodSymbol.ContainingType.GetCciAdapter();
			}
		}

		private TypeMemberVisibility ITypeDefinitionMemberVisibility => PEModuleBuilder.MemberVisibility(AdaptedMethodSymbol);

		private IEnumerable<IGenericMethodParameter> IMethodDefinitionGenericParameters => AdaptedMethodSymbol.TypeParameters;

		private bool IMethodDefinitionHasDeclarativeSecurity => AdaptedMethodSymbol.HasDeclarativeSecurity;

		private bool IMethodDefinitionIsAbstract => AdaptedMethodSymbol.IsMustOverride;

		private bool IMethodDefinitionIsAccessCheckedOnOverride => AdaptedMethodSymbol.IsAccessCheckedOnOverride;

		private bool IMethodDefinitionIsConstructor => AdaptedMethodSymbol.MethodKind == MethodKind.Constructor;

		private bool IMethodDefinitionIsExternal => AdaptedMethodSymbol.IsExternal;

		private bool IMethodDefinitionIsHiddenBySignature => AdaptedMethodSymbol.IsHiddenBySignature;

		private bool IMethodDefinitionIsNewSlot => AdaptedMethodSymbol.IsMetadataNewSlot();

		private bool IMethodDefinitionIsPlatformInvoke => AdaptedMethodSymbol.GetDllImportData() != null;

		private IPlatformInvokeInformation IMethodDefinitionPlatformInvokeData => AdaptedMethodSymbol.GetDllImportData();

		private bool IMethodDefinitionIsRuntimeSpecial => AdaptedMethodSymbol.HasRuntimeSpecialName;

		private bool IMethodDefinitionIsSealed => AdaptedMethodSymbol.IsMetadataFinal;

		private bool IMethodDefinitionIsSpecialName => AdaptedMethodSymbol.HasSpecialName;

		private bool IMethodDefinitionIsStatic => AdaptedMethodSymbol.IsShared;

		private bool IMethodDefinitionIsVirtual => SymbolExtensions.IsMetadataVirtual(AdaptedMethodSymbol);

		private ImmutableArray<IParameterDefinition> IMethodDefinitionParameters => StaticCast<IParameterDefinition>.From(AdaptedMethodSymbol.Parameters);

		private bool IMethodDefinitionRequiresSecurityObject => false;

		private bool IMethodDefinitionReturnValueIsMarshalledExplicitly => AdaptedMethodSymbol.ReturnValueIsMarshalledExplicitly;

		private IMarshallingInformation IMethodDefinitionReturnValueMarshallingInformation => AdaptedMethodSymbol.ReturnTypeMarshallingInformation;

		private ImmutableArray<byte> IMethodDefinitionReturnValueMarshallingDescriptor => AdaptedMethodSymbol.ReturnValueMarshallingDescriptor;

		private IEnumerable<SecurityAttribute> IMethodDefinitionSecurityAttributes => AdaptedMethodSymbol.GetSecurityInformation();

		private INamespace IMethodDefinition_ContainingNamespace => AdaptedMethodSymbol.ContainingNamespace.GetCciAdapter();

		internal MethodSymbol AdaptedMethodSymbol => this;

		internal virtual bool IsAccessCheckedOnOverride => SymbolExtensions.IsMetadataVirtual(this);

		internal virtual bool IsExternal => IsExternalMethod;

		internal virtual bool IsHiddenBySignature => IsOverloads;

		internal virtual bool HasRuntimeSpecialName
		{
			get
			{
				if (MethodKind != MethodKind.Constructor)
				{
					return MethodKind == MethodKind.StaticConstructor;
				}
				return true;
			}
		}

		internal virtual bool IsMetadataFinal
		{
			get
			{
				if (!IsNotOverridable)
				{
					if (SymbolExtensions.IsMetadataVirtual(this))
					{
						if (!IsOverridable && !IsMustOverride)
						{
							return !IsOverrides;
						}
						return false;
					}
					return false;
				}
				return true;
			}
		}

		internal virtual bool ReturnValueIsMarshalledExplicitly => ReturnTypeMarshallingInformation != null;

		internal virtual ImmutableArray<byte> ReturnValueMarshallingDescriptor => default(ImmutableArray<byte>);

		public abstract MethodKind MethodKind { get; }

		internal abstract bool IsMethodKindBasedOnSyntax { get; }

		public abstract bool IsVararg { get; }

		public virtual bool IsCheckedBuiltin => false;

		public virtual bool IsGenericMethod => Arity != 0;

		public abstract int Arity { get; }

		public abstract ImmutableArray<TypeParameterSymbol> TypeParameters { get; }

		public abstract ImmutableArray<TypeSymbol> TypeArguments { get; }

		public new virtual MethodSymbol OriginalDefinition => this;

		protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

		public virtual MethodSymbol ConstructedFrom => this;

		private bool IMethodSymbol_IsReadOnly => false;

		private bool IMethodSymbol_IsInitOnly => IsInitOnly;

		public abstract bool IsSub { get; }

		public abstract bool IsAsync { get; }

		public abstract bool IsIterator { get; }

		public abstract bool IsInitOnly { get; }

		public abstract bool ReturnsByRef { get; }

		public abstract TypeSymbol ReturnType { get; }

		public abstract ImmutableArray<CustomModifier> ReturnTypeCustomModifiers { get; }

		public abstract ImmutableArray<CustomModifier> RefCustomModifiers { get; }

		internal virtual int ParameterCount => Parameters.Length;

		public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

		internal abstract SyntaxNode Syntax { get; }

		public bool IsConditional => GetAppliedConditionalSymbols().Any();

		internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

		internal abstract bool HasSpecialName { get; }

		public abstract Symbol AssociatedSymbol { get; }

		public virtual NamedTypeSymbol AssociatedAnonymousDelegate => null;

		public virtual MethodSymbol OverriddenMethod
		{
			get
			{
				if (SymbolExtensions.IsAccessor(this) && AssociatedSymbol.Kind == SymbolKind.Property)
				{
					return ((PropertySymbol)AssociatedSymbol).GetAccessorOverride(MethodKind == MethodKind.PropertyGet);
				}
				if (IsOverrides && (object)ConstructedFrom == this)
				{
					if (base.IsDefinition)
					{
						return OverriddenMembers.OverriddenMember;
					}
					return OverriddenMembersResult<MethodSymbol>.GetOverriddenMember(this, OriginalDefinition.OverriddenMethod);
				}
				return null;
			}
		}

		internal virtual OverriddenMembersResult<MethodSymbol> OverriddenMembers => OverrideHidingHelper<MethodSymbol>.MakeOverriddenMembers(this);

		public virtual ImmutableArray<HandledEvent> HandledEvents => ImmutableArray<HandledEvent>.Empty;

		public abstract ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations { get; }

		public abstract bool IsExternalMethod { get; }

		internal abstract MarshalPseudoCustomAttributeData ReturnTypeMarshallingInformation { get; }

		internal abstract MethodImplAttributes ImplementationAttributes { get; }

		internal abstract bool HasDeclarativeSecurity { get; }

		public abstract bool IsExtensionMethod { get; }

		internal virtual bool MayBeReducibleExtensionMethod
		{
			get
			{
				if (IsExtensionMethod)
				{
					return MethodKind != MethodKind.ReducedExtension;
				}
				return false;
			}
		}

		public abstract bool IsOverloads { get; }

		internal bool IsRuntimeImplemented => (ImplementationAttributes & MethodImplAttributes.CodeTypeMask) != 0;

		internal override Symbol ImplicitlyDefinedBy => AssociatedSymbol;

		public sealed override SymbolKind Kind => SymbolKind.Method;

		internal bool IsScriptConstructor
		{
			get
			{
				if (MethodKind == MethodKind.Constructor)
				{
					return ContainingType.IsScriptClass;
				}
				return false;
			}
		}

		internal virtual bool IsScriptInitializer => false;

		internal bool IsSubmissionConstructor
		{
			get
			{
				if (IsScriptConstructor)
				{
					return ContainingAssembly.IsInteractive;
				}
				return false;
			}
		}

		internal bool IsEntryPointCandidate
		{
			get
			{
				if (ContainingType.IsEmbedded)
				{
					return false;
				}
				if (IsSubmissionConstructor)
				{
					return false;
				}
				if (IsImplicitlyDeclared)
				{
					return false;
				}
				return string.Equals(Name, "Main", StringComparison.OrdinalIgnoreCase);
			}
		}

		internal bool IsViableMainMethod
		{
			get
			{
				if (IsShared && IsAccessibleEntryPoint())
				{
					return HasEntryPointSignature();
				}
				return false;
			}
		}

		internal virtual bool CanConstruct
		{
			get
			{
				if (base.IsDefinition)
				{
					return Arity > 0;
				}
				return false;
			}
		}

		internal abstract CallingConvention CallingConvention { get; }

		internal ParameterSymbol MeParameter
		{
			get
			{
				ParameterSymbol meParameter = null;
				if (!TryGetMeParameter(out meParameter))
				{
					throw ExceptionUtilities.Unreachable;
				}
				return meParameter;
			}
		}

		protected override int HighestPriorityUseSiteError => 30657;

		public sealed override bool HasUnsupportedMetadata
		{
			get
			{
				DiagnosticInfo diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					return diagnosticInfo.Code == 30657;
				}
				return false;
			}
		}

		public virtual MethodSymbol ReducedFrom => null;

		internal bool IsReducedExtensionMethod => (object)ReducedFrom != null;

		internal virtual MethodSymbol CallsiteReducedFromMethod => null;

		public virtual TypeSymbol ReceiverType => ContainingType;

		internal virtual ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>> FixedTypeParameters => ImmutableArray<KeyValuePair<TypeParameterSymbol, TypeSymbol>>.Empty;

		internal virtual int Proximity => 0;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind
		{
			get
			{
				if ((object)ContainingSymbol != null)
				{
					return ContainingSymbol.EmbeddedSymbolKind;
				}
				return EmbeddedSymbolKind.None;
			}
		}

		internal abstract bool GenerateDebugInfoImpl { get; }

		internal bool GenerateDebugInfo
		{
			get
			{
				if (GenerateDebugInfoImpl)
				{
					return !base.IsEmbedded;
				}
				return false;
			}
		}

		internal virtual bool PreserveOriginalLocals => false;

		public virtual bool IsTupleMethod => false;

		public virtual MethodSymbol TupleUnderlyingMethod => null;

		private int IMethodSymbol_Arity => Arity;

		private IMethodSymbol IMethodSymbol_ConstructedFrom => ConstructedFrom;

		private ImmutableArray<IMethodSymbol> IMethodSymbol_ExplicitInterfaceImplementations => ExplicitInterfaceImplementations.Cast<MethodSymbol, IMethodSymbol>();

		private MethodImplAttributes IMethodSymbol_MethodImplementationFlags => ImplementationAttributes;

		private bool IMethodSymbol_IsExtensionMethod => IsExtensionMethod;

		private MethodKind IMethodSymbol_MethodKind => MethodKind;

		private IMethodSymbol IMethodSymbol_OriginalDefinition => OriginalDefinition;

		private IMethodSymbol IMethodSymbol_OverriddenMethod => OverriddenMethod;

		private ITypeSymbol IMethodSymbol_ReceiverType => ReceiverType;

		private NullableAnnotation IMethodSymbol_ReceiverNullableAnnotation => NullableAnnotation.None;

		private IMethodSymbol IMethodSymbol_ReducedFrom => ReducedFrom;

		private ImmutableArray<IParameterSymbol> IMethodSymbol_Parameters => ImmutableArray<IParameterSymbol>.CastUp(Parameters);

		private bool ISymbol_IsExtern => IsExternalMethod;

		public virtual MethodSymbol PartialImplementationPart => null;

		public virtual MethodSymbol PartialDefinitionPart => null;

		private IMethodSymbol IMethodSymbol_PartialDefinitionPart => PartialDefinitionPart;

		private IMethodSymbol IMethodSymbol_PartialImplementationPart => PartialImplementationPart;

		private bool IMethodSymbol_IsPartialDefinition => (this as SourceMemberMethodSymbol)?.IsPartialDefinition ?? false;

		private bool IMethodSymbol_ReturnsVoid => IsSub;

		private bool IMethodSymbol_ReturnsByRef => ReturnsByRef;

		private bool IMethodSymbol_ReturnsByReadonlyRef => false;

		private RefKind IMethodSymbol_RefKind
		{
			get
			{
				if (!ReturnsByRef)
				{
					return RefKind.None;
				}
				return RefKind.Ref;
			}
		}

		private ITypeSymbol IMethodSymbol_ReturnType => ReturnType;

		private NullableAnnotation IMethodSymbol_ReturnNullableAnnotation => NullableAnnotation.None;

		private SignatureCallingConvention IMethodSymbol_CallingConvention => CallingConvention.ToSignatureConvention();

		private ImmutableArray<INamedTypeSymbol> IMethodSymbol_UnmanagedCallingConventionTypes => ImmutableArray<INamedTypeSymbol>.Empty;

		private ImmutableArray<ITypeSymbol> IMethodSymbol_TypeArguments => StaticCast<ITypeSymbol>.From(TypeArguments);

		private ImmutableArray<NullableAnnotation> IMethodSymbol_TypeArgumentsNullableAnnotation => TypeArguments.SelectAsArray((TypeSymbol t) => NullableAnnotation.None);

		private ImmutableArray<ITypeParameterSymbol> IMethodSymbol_TypeParameters => StaticCast<ITypeParameterSymbol>.From(TypeParameters);

		private ISymbol IMethodSymbol_AssociatedSymbol => AssociatedSymbol;

		private bool IMethodSymbol_IsGenericMethod => IsGenericMethod;

		private bool IMethodSymbol_IsAsync => IsAsync;

		private bool IMethodSymbol_HidesBaseMethodsByName => true;

		private ImmutableArray<CustomModifier> IMethodSymbol_RefCustomModifiers => RefCustomModifiers;

		private ImmutableArray<CustomModifier> IMethodSymbol_ReturnTypeCustomModifiers => ReturnTypeCustomModifiers;

		private INamedTypeSymbol IMethodSymbol_AssociatedAnonymousDelegate => AssociatedAnonymousDelegate;

		private bool IMethodSymbolInternal_IsIterator => IsIterator;

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			return ResolvedMethodImpl((PEModuleBuilder)context.Module);
		}

		private ITypeReference ITypeMemberReferenceGetContainingType(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			if (!AdaptedMethodSymbol.IsDefinition)
			{
				return pEModuleBuilder.Translate(AdaptedMethodSymbol.ContainingType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			}
			if (AdaptedMethodSymbol is SynthesizedGlobalMethodBase)
			{
				return pEModuleBuilder.GetPrivateImplClass((VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			}
			return pEModuleBuilder.Translate(AdaptedMethodSymbol.ContainingType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, needDeclaration: true);
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeMemberReferenceGetContainingType
			return this.ITypeMemberReferenceGetContainingType(context);
		}

		internal sealed override void IReferenceDispatch(MetadataVisitor visitor)
		{
			if (!AdaptedMethodSymbol.IsDefinition)
			{
				if (AdaptedMethodSymbol.IsGenericMethod && (object)AdaptedMethodSymbol != AdaptedMethodSymbol.ConstructedFrom)
				{
					visitor.Visit((IGenericMethodInstanceReference)this);
				}
				else
				{
					visitor.Visit((IMethodReference)this);
				}
				return;
			}
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)visitor.Context.Module;
			if (AdaptedMethodSymbol.ContainingModule == pEModuleBuilder.SourceModule)
			{
				visitor.Visit((IMethodDefinition)this);
			}
			else
			{
				visitor.Visit((IMethodReference)this);
			}
		}

		private IMethodDefinition IMethodReferenceGetResolvedMethod(EmitContext context)
		{
			return ResolvedMethodImpl((PEModuleBuilder)context.Module);
		}

		IMethodDefinition IMethodReference.GetResolvedMethod(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodReferenceGetResolvedMethod
			return this.IMethodReferenceGetResolvedMethod(context);
		}

		private IMethodDefinition ResolvedMethodImpl(PEModuleBuilder moduleBeingBuilt)
		{
			if (AdaptedMethodSymbol.IsDefinition && AdaptedMethodSymbol.ContainingModule == moduleBeingBuilt.SourceModule)
			{
				return this;
			}
			return null;
		}

		private ImmutableArray<IParameterTypeInformation> ISignatureGetParameters(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			if (AdaptedMethodSymbol.IsDefinition && AdaptedMethodSymbol.ContainingModule == pEModuleBuilder.SourceModule)
			{
				return EnumerateDefinitionParameters();
			}
			return pEModuleBuilder.Translate(AdaptedMethodSymbol.Parameters);
		}

		ImmutableArray<IParameterTypeInformation> ISignature.GetParameters(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISignatureGetParameters
			return this.ISignatureGetParameters(context);
		}

		private ImmutableArray<IParameterTypeInformation> EnumerateDefinitionParameters()
		{
			return StaticCast<IParameterTypeInformation>.From(AdaptedMethodSymbol.Parameters);
		}

		private ITypeReference ISignatureGetType(EmitContext context)
		{
			PEModuleBuilder obj = (PEModuleBuilder)context.Module;
			TypeSymbol returnType = AdaptedMethodSymbol.ReturnType;
			return obj.Translate(returnType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ISignature.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISignatureGetType
			return this.ISignatureGetType(context);
		}

		private IEnumerable<ITypeReference> IGenericMethodInstanceReferenceGetGenericArguments(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			return AdaptedMethodSymbol.TypeArguments.Select((TypeSymbol arg) => pEModuleBuilder.Translate(arg, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics));
		}

		IEnumerable<ITypeReference> IGenericMethodInstanceReference.GetGenericArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericMethodInstanceReferenceGetGenericArguments
			return this.IGenericMethodInstanceReferenceGetGenericArguments(context);
		}

		private IMethodReference IGenericMethodInstanceReferenceGetGenericMethod(EmitContext context)
		{
			if (!NamedTypeSymbolExtensions.IsOrInGenericType(AdaptedMethodSymbol.ContainingType))
			{
				return ((PEModuleBuilder)context.Module).Translate(AdaptedMethodSymbol.OriginalDefinition, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, needDeclaration: true);
			}
			return new SpecializedMethodReference(AdaptedMethodSymbol.ConstructedFrom);
		}

		IMethodReference IGenericMethodInstanceReference.GetGenericMethod(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericMethodInstanceReferenceGetGenericMethod
			return this.IGenericMethodInstanceReferenceGetGenericMethod(context);
		}

		private IMethodBody IMethodDefinitionGetBody(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).GetMethodBody(AdaptedMethodSymbol);
		}

		IMethodBody IMethodDefinition.GetBody(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodDefinitionGetBody
			return this.IMethodDefinitionGetBody(context);
		}

		private MethodImplAttributes IMethodDefinitionGetImplementationOptions(EmitContext context)
		{
			return AdaptedMethodSymbol.ImplementationAttributes | (((PEModuleBuilder)context.Module).JITOptimizationIsDisabled(AdaptedMethodSymbol) ? ((MethodImplAttributes)72) : MethodImplAttributes.IL);
		}

		MethodImplAttributes IMethodDefinition.GetImplementationAttributes(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodDefinitionGetImplementationOptions
			return this.IMethodDefinitionGetImplementationOptions(context);
		}

		private IEnumerable<ICustomAttribute> IMethodDefinitionGetReturnValueAttributes(EmitContext context)
		{
			ArrayBuilder<SynthesizedAttributeData> attributes = null;
			ImmutableArray<VisualBasicAttributeData> returnTypeAttributes = AdaptedMethodSymbol.GetReturnTypeAttributes();
			AdaptedMethodSymbol.AddSynthesizedReturnTypeAttributes(ref attributes);
			return AdaptedMethodSymbol.GetCustomAttributesToEmit(returnTypeAttributes, attributes, isReturnType: true, emittingAssemblyAttributesInNetModule: false);
		}

		IEnumerable<ICustomAttribute> IMethodDefinition.GetReturnValueAttributes(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodDefinitionGetReturnValueAttributes
			return this.IMethodDefinitionGetReturnValueAttributes(context);
		}

		internal new MethodSymbol GetCciAdapter()
		{
			return this;
		}

		internal virtual bool IsMetadataNewSlot(bool ignoreInterfaceImplementationChanges = false)
		{
			if (IsOverrides)
			{
				return OverrideHidingHelper.RequiresExplicitOverride(this);
			}
			return SymbolExtensions.IsMetadataVirtual(this);
		}

		protected bool ValidateGenericConstraintsOnExtensionMethodDefinition()
		{
			if (Arity == 0)
			{
				return true;
			}
			ParameterSymbol parameterSymbol = Parameters[0];
			HashSet<TypeParameterSymbol> hashSet = new HashSet<TypeParameterSymbol>();
			TypeSymbolExtensions.CollectReferencedTypeParameters(parameterSymbol.Type, hashSet);
			if (hashSet.Count > 0)
			{
				foreach (TypeParameterSymbol item in hashSet)
				{
					ImmutableArray<TypeSymbol>.Enumerator enumerator2 = item.ConstraintTypesNoUseSiteDiagnostics.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						if (TypeSymbolExtensions.ReferencesTypeParameterNotInTheSet(enumerator2.Current, hashSet))
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal virtual bool IsParameterlessConstructor()
		{
			if (ParameterCount == 0)
			{
				return MethodKind == MethodKind.Constructor;
			}
			return false;
		}

		public virtual ImmutableArray<VisualBasicAttributeData> GetReturnTypeAttributes()
		{
			return ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal virtual void AddSynthesizedReturnTypeAttributes(ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
		}

		internal virtual bool CallsAreOmitted(SyntaxNodeOrToken atNode, SyntaxTree syntaxTree)
		{
			if (!MethodSymbolExtensions.IsPartialWithoutImplementation(this))
			{
				if (syntaxTree != null)
				{
					return CallsAreConditionallyOmitted(atNode, syntaxTree);
				}
				return false;
			}
			return true;
		}

		private bool CallsAreConditionallyOmitted(SyntaxNodeOrToken atNode, SyntaxTree syntaxTree)
		{
			NamedTypeSymbol containingType = ContainingType;
			if (IsConditional && IsSub && MethodKind != MethodKind.PropertySet && ((object)containingType == null || !TypeSymbolExtensions.IsInterfaceType(containingType)))
			{
				IEnumerable<string> conditionalSymbolNames = GetAppliedConditionalSymbols();
				if (VisualBasicExtensions.IsAnyPreprocessorSymbolDefined(syntaxTree, conditionalSymbolNames, atNode))
				{
					return false;
				}
				return true;
			}
			return false;
		}

		internal abstract ImmutableArray<string> GetAppliedConditionalSymbols();

		public abstract DllImportData GetDllImportData();

		DllImportData IMethodSymbol.GetDllImportData()
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetDllImportData
			return this.GetDllImportData();
		}

		internal abstract IEnumerable<SecurityAttribute> GetSecurityInformation();

		private bool IsAccessibleEntryPoint()
		{
			if (DeclaredAccessibility == Accessibility.Private)
			{
				return false;
			}
			NamedTypeSymbol containingType = ContainingType;
			while ((object)containingType != null)
			{
				if (containingType.DeclaredAccessibility == Accessibility.Private)
				{
					return false;
				}
				containingType = containingType.ContainingType;
			}
			return true;
		}

		internal bool HasEntryPointSignature()
		{
			TypeSymbol returnType = ReturnType;
			if (returnType.SpecialType != SpecialType.System_Int32 && returnType.SpecialType != SpecialType.System_Void)
			{
				return false;
			}
			if (Parameters.Length == 0)
			{
				return true;
			}
			if (Parameters.Length > 1)
			{
				return false;
			}
			if (Parameters[0].IsByRef)
			{
				return false;
			}
			TypeSymbol type = Parameters[0].Type;
			if (type.TypeKind != TypeKind.Array)
			{
				return false;
			}
			ArrayTypeSymbol arrayTypeSymbol = (ArrayTypeSymbol)type;
			return arrayTypeSymbol.IsSZArray && arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_String;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitMethod(this, arg);
		}

		internal MethodSymbol()
		{
		}

		protected void CheckCanConstructAndTypeArguments(ImmutableArray<TypeSymbol> typeArguments)
		{
			if (!CanConstruct || (object)this != ConstructedFrom)
			{
				throw new InvalidOperationException();
			}
			TypeSymbolExtensions.CheckTypeArguments(typeArguments, Arity);
		}

		public virtual MethodSymbol Construct(ImmutableArray<TypeSymbol> typeArguments)
		{
			CheckCanConstructAndTypeArguments(typeArguments);
			TypeSubstitution typeSubstitution = TypeSubstitution.Create(this, TypeParameters, typeArguments, allowAlphaRenamedTypeParametersAsArguments: true);
			if (typeSubstitution == null)
			{
				return this;
			}
			return new SubstitutedMethodSymbol.ConstructedNotSpecializedGenericMethod(typeSubstitution, typeArguments);
		}

		public MethodSymbol Construct(params TypeSymbol[] typeArguments)
		{
			return Construct(ImmutableArray.Create(typeArguments));
		}

		internal virtual bool TryGetMeParameter(out ParameterSymbol meParameter)
		{
			meParameter = null;
			return false;
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
			UseSiteInfo<AssemblySymbol> result = MergeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency), DeriveUseSiteInfoFromType(ReturnType));
			DiagnosticInfo? diagnosticInfo = result.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == 30657)
			{
				return result;
			}
			UseSiteInfo<AssemblySymbol> result2 = DeriveUseSiteInfoFromCustomModifiers(RefCustomModifiers);
			DiagnosticInfo? diagnosticInfo2 = result2.DiagnosticInfo;
			if (diagnosticInfo2 != null && diagnosticInfo2!.Code == 30657)
			{
				return result2;
			}
			UseSiteInfo<AssemblySymbol> result3 = DeriveUseSiteInfoFromCustomModifiers(ReturnTypeCustomModifiers, IsInitOnly);
			DiagnosticInfo? diagnosticInfo3 = result3.DiagnosticInfo;
			if (diagnosticInfo3 != null && diagnosticInfo3!.Code == 30657)
			{
				return result3;
			}
			UseSiteInfo<AssemblySymbol> result4 = DeriveUseSiteInfoFromParameters(Parameters);
			DiagnosticInfo? diagnosticInfo4 = result4.DiagnosticInfo;
			if (diagnosticInfo4 != null && diagnosticInfo4!.Code == 30657)
			{
				return result4;
			}
			DiagnosticInfo diagnosticInfo5 = result.DiagnosticInfo ?? result2.DiagnosticInfo ?? result3.DiagnosticInfo ?? result4.DiagnosticInfo;
			if (diagnosticInfo5 == null && ContainingModule.HasUnifiedReferences)
			{
				HashSet<TypeSymbol> checkedTypes = null;
				diagnosticInfo5 = ReturnType.GetUnificationUseSiteDiagnosticRecursive(this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(RefCustomModifiers, this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(ReturnTypeCustomModifiers, this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(Parameters, this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(TypeParameters, this, ref checkedTypes);
			}
			UseSiteInfo<AssemblySymbol> result5;
			if (diagnosticInfo5 != null)
			{
				result5 = new UseSiteInfo<AssemblySymbol>(diagnosticInfo5);
			}
			else
			{
				AssemblySymbol primaryDependency = result.PrimaryDependency;
				ImmutableHashSet<AssemblySymbol> secondaryDependencies = result.SecondaryDependencies;
				result2.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
				result3.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
				result4.MergeDependencies(ref primaryDependency, ref secondaryDependencies);
				result5 = new UseSiteInfo<AssemblySymbol>(null, primaryDependency, secondaryDependencies);
			}
			return result5;
		}

		public virtual TypeSymbol GetTypeInferredDuringReduction(TypeParameterSymbol reducedFromTypeParameter)
		{
			throw new InvalidOperationException();
		}

		internal MethodSymbol ReduceExtensionMethod(TypeSymbol instanceType, int proximity, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ReducedExtensionMethodSymbol.Create(instanceType, this, proximity, ref useSiteInfo);
		}

		public MethodSymbol ReduceExtensionMethod(TypeSymbol instanceType, ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			return ReduceExtensionMethod(instanceType, 0, ref useSiteInfo);
		}

		internal virtual BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, out Binder methodBodyBinder = null)
		{
			throw ExceptionUtilities.Unreachable;
		}

		internal abstract int CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree);

		private ITypeSymbol IMethodSymbol_GetTypeInferredDuringReduction(ITypeParameterSymbol reducedFromTypeParameter)
		{
			return GetTypeInferredDuringReduction(SymbolExtensions.EnsureVbSymbolOrNothing<ITypeParameterSymbol, TypeParameterSymbol>(reducedFromTypeParameter, "reducedFromTypeParameter"));
		}

		ITypeSymbol IMethodSymbol.GetTypeInferredDuringReduction(ITypeParameterSymbol reducedFromTypeParameter)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbol_GetTypeInferredDuringReduction
			return this.IMethodSymbol_GetTypeInferredDuringReduction(reducedFromTypeParameter);
		}

		private IMethodSymbol IMethodSymbol_ReduceExtensionMethod(ITypeSymbol receiverType)
		{
			if (receiverType == null)
			{
				throw new ArgumentNullException("receiverType");
			}
			TypeSymbol instanceType = SymbolExtensions.EnsureVbSymbolOrNothing<ITypeSymbol, TypeSymbol>(receiverType, "receiverType");
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
			return ReduceExtensionMethod(instanceType, ref useSiteInfo);
		}

		IMethodSymbol IMethodSymbol.ReduceExtensionMethod(ITypeSymbol receiverType)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbol_ReduceExtensionMethod
			return this.IMethodSymbol_ReduceExtensionMethod(receiverType);
		}

		private ImmutableArray<AttributeData> IMethodSymbol_GetReturnTypeAttributes()
		{
			return GetReturnTypeAttributes().Cast<VisualBasicAttributeData, AttributeData>();
		}

		ImmutableArray<AttributeData> IMethodSymbol.GetReturnTypeAttributes()
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbol_GetReturnTypeAttributes
			return this.IMethodSymbol_GetReturnTypeAttributes();
		}

		private IMethodSymbol IMethodSymbol_Construct(params ITypeSymbol[] typeArguments)
		{
			return Construct(Symbol.ConstructTypeArguments(typeArguments));
		}

		IMethodSymbol IMethodSymbol.Construct(params ITypeSymbol[] typeArguments)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbol_Construct
			return this.IMethodSymbol_Construct(typeArguments);
		}

		private IMethodSymbolInternal IMethodSymbolInternal_Construct(params ITypeSymbolInternal[] typeArguments)
		{
			return Construct((TypeSymbol[])typeArguments);
		}

		IMethodSymbolInternal IMethodSymbolInternal.Construct(params ITypeSymbolInternal[] typeArguments)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbolInternal_Construct
			return this.IMethodSymbolInternal_Construct(typeArguments);
		}

		private IMethodSymbol IMethodSymbol_Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<NullableAnnotation> typeArgumentNullableAnnotations)
		{
			return Construct(Symbol.ConstructTypeArguments(typeArguments, typeArgumentNullableAnnotations));
		}

		IMethodSymbol IMethodSymbol.Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<NullableAnnotation> typeArgumentNullableAnnotations)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbol_Construct
			return this.IMethodSymbol_Construct(typeArguments, typeArgumentNullableAnnotations);
		}

		private int IMethodSymbolInternal_CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			return CalculateLocalSyntaxOffset(localPosition, localTree);
		}

		int IMethodSymbolInternal.CalculateLocalSyntaxOffset(int localPosition, SyntaxTree localTree)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IMethodSymbolInternal_CalculateLocalSyntaxOffset
			return this.IMethodSymbolInternal_CalculateLocalSyntaxOffset(localPosition, localTree);
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitMethod(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitMethod(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitMethod(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitMethod(this);
		}
	}
}
