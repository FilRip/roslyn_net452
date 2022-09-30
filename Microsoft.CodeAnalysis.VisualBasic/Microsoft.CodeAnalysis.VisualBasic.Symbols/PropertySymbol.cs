using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class PropertySymbol : Symbol, IPropertyDefinition, IPropertySymbol
	{
		private MetadataConstant IPropertyDefinitionDefaultValue => null;

		private IMethodReference IPropertyDefinitionGetter => AdaptedPropertySymbol.GetMethod?.GetCciAdapter();

		private bool IPropertyDefinitionHasDefaultValue => false;

		private bool IPropertyDefinitionIsRuntimeSpecial => AdaptedPropertySymbol.HasRuntimeSpecialName;

		private bool IPropertyDefinitionIsSpecialName => AdaptedPropertySymbol.HasSpecialName;

		private ImmutableArray<IParameterDefinition> IPropertyDefinitionParameters => StaticCast<IParameterDefinition>.From(AdaptedPropertySymbol.Parameters);

		private IMethodReference IPropertyDefinitionSetter => AdaptedPropertySymbol.SetMethod?.GetCciAdapter();

		private CallingConvention ISignatureCallingConvention => AdaptedPropertySymbol.CallingConvention;

		private ushort ISignatureParameterCount => (ushort)AdaptedPropertySymbol.ParameterCount;

		private ImmutableArray<ICustomModifier> ISignatureReturnValueCustomModifiers => AdaptedPropertySymbol.TypeCustomModifiers.As<ICustomModifier>();

		private ImmutableArray<ICustomModifier> ISignatureRefCustomModifiers => AdaptedPropertySymbol.RefCustomModifiers.As<ICustomModifier>();

		private bool ISignatureReturnValueIsByRef => AdaptedPropertySymbol.ReturnsByRef;

		private ITypeDefinition ITypeDefinitionMemberContainingTypeDefinition => AdaptedPropertySymbol.ContainingType.GetCciAdapter();

		private TypeMemberVisibility ITypeDefinitionMemberVisibility => PEModuleBuilder.MemberVisibility(AdaptedPropertySymbol);

		private string INamedEntityName => AdaptedPropertySymbol.MetadataName;

		internal PropertySymbol AdaptedPropertySymbol => this;

		internal virtual bool HasRuntimeSpecialName => false;

		public new virtual PropertySymbol OriginalDefinition => this;

		protected sealed override Symbol OriginalSymbolDefinition => OriginalDefinition;

		public abstract bool ReturnsByRef { get; }

		public abstract TypeSymbol Type { get; }

		public abstract ImmutableArray<CustomModifier> TypeCustomModifiers { get; }

		public abstract ImmutableArray<CustomModifier> RefCustomModifiers { get; }

		public abstract ImmutableArray<ParameterSymbol> Parameters { get; }

		public virtual int ParameterCount => Parameters.Length;

		internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

		internal abstract bool HasSpecialName { get; }

		public abstract bool IsDefault { get; }

		public virtual bool IsReadOnly => (object)SetMethod == null;

		internal bool IsReadable => (object)GetMostDerivedGetMethod() != null;

		public virtual bool IsWriteOnly => (object)GetMethod == null;

		internal bool HasSet => (object)GetMostDerivedSetMethod() != null;

		public abstract MethodSymbol GetMethod { get; }

		public abstract MethodSymbol SetMethod { get; }

		internal abstract FieldSymbol AssociatedField { get; }

		public abstract bool IsOverloads { get; }

		public PropertySymbol OverriddenProperty
		{
			get
			{
				if (IsOverrides)
				{
					if (base.IsDefinition)
					{
						return OverriddenMembers.OverriddenMember;
					}
					return OverriddenMembersResult<PropertySymbol>.GetOverriddenMember(this, OriginalDefinition.OverriddenProperty);
				}
				return null;
			}
		}

		internal virtual OverriddenMembersResult<PropertySymbol> OverriddenMembers => OverrideHidingHelper<PropertySymbol>.MakeOverriddenMembers(this);

		public abstract ImmutableArray<PropertySymbol> ExplicitInterfaceImplementations { get; }

		public sealed override SymbolKind Kind => SymbolKind.Property;

		internal abstract CallingConvention CallingConvention { get; }

		internal virtual ParameterSymbol MeParameter
		{
			get
			{
				throw ExceptionUtilities.Unreachable;
			}
		}

		internal virtual PropertySymbol ReducedFrom => null;

		internal virtual PropertySymbol ReducedFromDefinition => null;

		internal virtual TypeSymbol ReceiverType => ContainingType;

		protected override int HighestPriorityUseSiteError => 30643;

		public sealed override bool HasUnsupportedMetadata
		{
			get
			{
				DiagnosticInfo diagnosticInfo = GetUseSiteInfo().DiagnosticInfo;
				if (diagnosticInfo != null)
				{
					return diagnosticInfo.Code == 30643;
				}
				return false;
			}
		}

		public virtual bool IsWithEvents => OverriddenProperty?.IsWithEvents ?? false;

		internal override EmbeddedSymbolKind EmbeddedSymbolKind => ContainingSymbol.EmbeddedSymbolKind;

		public virtual bool IsTupleProperty => false;

		public virtual PropertySymbol TupleUnderlyingProperty => null;

		internal abstract override bool IsMyGroupCollectionProperty { get; }

		private ImmutableArray<IPropertySymbol> IPropertySymbol_ExplicitInterfaceImplementations => ExplicitInterfaceImplementations.Cast<PropertySymbol, IPropertySymbol>();

		private IMethodSymbol IPropertySymbol_GetMethod => GetMethod;

		private IPropertySymbol IPropertySymbol_OriginalDefinition => OriginalDefinition;

		private IPropertySymbol IPropertySymbol_OverriddenProperty => OverriddenProperty;

		private ImmutableArray<IParameterSymbol> IPropertySymbol_Parameters => StaticCast<IParameterSymbol>.From(Parameters);

		private IMethodSymbol IPropertySymbol_SetMethod => SetMethod;

		private bool IPropertySymbol_ReturnsByRef => ReturnsByRef;

		private bool IPropertySymbol_ByRefReturnIsReadonly => false;

		private RefKind IPropertySymbol_RefKind
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

		private ITypeSymbol IPropertySymbol_Type => Type;

		private NullableAnnotation IPropertySymbol_NullableAnnotation => NullableAnnotation.None;

		private ImmutableArray<CustomModifier> IPropertySymbol_RefCustomModifiers => RefCustomModifiers;

		private ImmutableArray<CustomModifier> IPropertySymbol_TypeCustomModifiers => TypeCustomModifiers;

		[IteratorStateMachine(typeof(VB_0024StateMachine_0_IPropertyDefinitionAccessors))]
		private IEnumerable<IMethodReference> IPropertyDefinitionAccessors(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_0_IPropertyDefinitionAccessors(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<IMethodReference> IPropertyDefinition.GetAccessors(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IPropertyDefinitionAccessors
			return this.IPropertyDefinitionAccessors(context);
		}

		[Conditional("DEBUG")]
		protected internal void CheckDefinitionInvariantAllowEmbedded()
		{
		}

		private ImmutableArray<IParameterTypeInformation> ISignatureGetParameters(EmitContext context)
		{
			return StaticCast<IParameterTypeInformation>.From(AdaptedPropertySymbol.Parameters);
		}

		ImmutableArray<IParameterTypeInformation> ISignature.GetParameters(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISignatureGetParameters
			return this.ISignatureGetParameters(context);
		}

		private ITypeReference ISignatureGetType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(AdaptedPropertySymbol.Type, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
		}

		ITypeReference ISignature.GetType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISignatureGetType
			return this.ISignatureGetType(context);
		}

		private ITypeReference ITypeMemberReferenceGetContainingType(EmitContext context)
		{
			return AdaptedPropertySymbol.ContainingType.GetCciAdapter();
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeMemberReferenceGetContainingType
			return this.ITypeMemberReferenceGetContainingType(context);
		}

		internal sealed override void IReferenceDispatch(MetadataVisitor visitor)
		{
			visitor.Visit(this);
		}

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			return this;
		}

		internal new PropertySymbol GetCciAdapter()
		{
			return this;
		}

		internal PropertySymbol()
		{
		}

		internal bool IsWritable(BoundExpression receiverOpt, Binder containingBinder, bool isKnownTargetOfObjectMemberInitializer)
		{
			MethodSymbol mostDerivedSetMethod = GetMostDerivedSetMethod();
			if ((object)mostDerivedSetMethod != null)
			{
				if (!mostDerivedSetMethod.IsInitOnly)
				{
					return true;
				}
				if (receiverOpt == null)
				{
					return false;
				}
				if (isKnownTargetOfObjectMemberInitializer)
				{
					return true;
				}
				Symbol containingMember = containingBinder.ContainingMember;
				MethodSymbol obj = containingMember as MethodSymbol;
				if ((object)obj == null || obj.MethodKind != MethodKind.Constructor)
				{
					return false;
				}
				if (receiverOpt.Kind == BoundKind.WithLValueExpressionPlaceholder || receiverOpt.Kind == BoundKind.WithRValueExpressionPlaceholder)
				{
					Binder binder = containingBinder;
					while (binder != null && (object)binder.ContainingMember == containingMember)
					{
						if (binder is WithBlockBinder withBlockBinder)
						{
							if (withBlockBinder.Info?.ExpressionPlaceholder == receiverOpt)
							{
								receiverOpt = withBlockBinder.Info.OriginalExpression;
							}
							break;
						}
						binder = binder.ContainingBinder;
					}
				}
				while (true)
				{
					switch (receiverOpt.Kind)
					{
					case BoundKind.MeReference:
					case BoundKind.MyBaseReference:
					case BoundKind.MyClassReference:
						return true;
					case BoundKind.Parenthesized:
						break;
					default:
						return false;
					}
					receiverOpt = ((BoundParenthesized)receiverOpt).Expression;
				}
			}
			SourcePropertySymbol sourcePropertySymbol = this as SourcePropertySymbol;
			bool isShared = IsShared;
			Symbol containingMember2 = containingBinder.ContainingMember;
			return (object)sourcePropertySymbol != null && (object)containingMember2 != null && sourcePropertySymbol.IsAutoProperty && TypeSymbol.Equals(sourcePropertySymbol.ContainingType, containingMember2.ContainingType, TypeCompareKind.ConsiderEverything) && isShared == containingMember2.IsShared && (isShared || (receiverOpt != null && receiverOpt.Kind == BoundKind.MeReference)) && ((containingMember2.Kind == SymbolKind.Method && SymbolExtensions.IsAnyConstructor((MethodSymbol)containingMember2)) || containingBinder is DeclarationInitializerBinder);
		}

		internal MethodSymbol GetMostDerivedGetMethod()
		{
			PropertySymbol propertySymbol = this;
			do
			{
				MethodSymbol getMethod = propertySymbol.GetMethod;
				if ((object)getMethod != null)
				{
					return getMethod;
				}
				propertySymbol = propertySymbol.OverriddenProperty;
			}
			while ((object)propertySymbol != null);
			return null;
		}

		internal MethodSymbol GetMostDerivedSetMethod()
		{
			PropertySymbol propertySymbol = this;
			do
			{
				MethodSymbol setMethod = propertySymbol.SetMethod;
				if ((object)setMethod != null)
				{
					return setMethod;
				}
				propertySymbol = propertySymbol.OverriddenProperty;
			}
			while ((object)propertySymbol != null);
			return null;
		}

		public ImmutableArray<VisualBasicAttributeData> GetFieldAttributes()
		{
			return AssociatedField?.GetAttributes() ?? ImmutableArray<VisualBasicAttributeData>.Empty;
		}

		internal MethodSymbol GetAccessorOverride(bool getter)
		{
			PropertySymbol overriddenProperty = OverriddenProperty;
			if ((object)overriddenProperty != null)
			{
				return getter ? overriddenProperty.GetMethod : overriddenProperty.SetMethod;
			}
			return null;
		}

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitProperty(this, arg);
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
			UseSiteInfo<AssemblySymbol> result = MergeUseSiteInfo(new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency), DeriveUseSiteInfoFromType(Type));
			DiagnosticInfo? diagnosticInfo = result.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == 30643)
			{
				return result;
			}
			UseSiteInfo<AssemblySymbol> result2 = DeriveUseSiteInfoFromCustomModifiers(RefCustomModifiers);
			DiagnosticInfo? diagnosticInfo2 = result2.DiagnosticInfo;
			if (diagnosticInfo2 != null && diagnosticInfo2!.Code == 30643)
			{
				return result2;
			}
			UseSiteInfo<AssemblySymbol> result3 = DeriveUseSiteInfoFromCustomModifiers(TypeCustomModifiers);
			DiagnosticInfo? diagnosticInfo3 = result3.DiagnosticInfo;
			if (diagnosticInfo3 != null && diagnosticInfo3!.Code == 30643)
			{
				return result3;
			}
			UseSiteInfo<AssemblySymbol> result4 = DeriveUseSiteInfoFromParameters(Parameters);
			DiagnosticInfo? diagnosticInfo4 = result4.DiagnosticInfo;
			if (diagnosticInfo4 != null && diagnosticInfo4!.Code == 30643)
			{
				return result4;
			}
			DiagnosticInfo diagnosticInfo5 = result.DiagnosticInfo ?? result2.DiagnosticInfo ?? result3.DiagnosticInfo ?? result4.DiagnosticInfo;
			if (diagnosticInfo5 == null && ContainingModule.HasUnifiedReferences)
			{
				HashSet<TypeSymbol> checkedTypes = null;
				diagnosticInfo5 = Type.GetUnificationUseSiteDiagnosticRecursive(this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(RefCustomModifiers, this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(TypeCustomModifiers, this, ref checkedTypes) ?? Symbol.GetUnificationUseSiteDiagnosticRecursive(Parameters, this, ref checkedTypes);
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

		internal void CloneParameters(MethodSymbol method, ArrayBuilder<ParameterSymbol> parameters)
		{
			ImmutableArray<ParameterSymbol> parameters2 = Parameters;
			int num = parameters2.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ParameterSymbol parameterSymbol = parameters2[i];
				parameters.Add(parameterSymbol.ChangeOwner(method));
			}
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitProperty(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitProperty(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitProperty(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitProperty(this);
		}
	}
}
