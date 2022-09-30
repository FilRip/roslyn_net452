using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Emit;
using Microsoft.CodeAnalysis.VisualBasic.Symbols.Metadata.PE;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class NamedTypeSymbol : TypeSymbol, ITypeReference, ITypeDefinition, INamedTypeReference, INamedTypeDefinition, INamespaceTypeReference, INamespaceTypeDefinition, INestedTypeReference, INestedTypeDefinition, IGenericTypeInstanceReference, ISpecializedNestedTypeReference, INamedTypeSymbol, INamedTypeSymbolInternal
	{
		private NamedTypeSymbol _lazyDeclaredBase;

		private ImmutableArray<NamedTypeSymbol> _lazyDeclaredInterfaces;

		private NamedTypeSymbol _lazyBaseType;

		private ImmutableArray<NamedTypeSymbol> _lazyInterfaces;

		private bool ITypeReferenceIsEnum => AdaptedNamedTypeSymbol.TypeKind == TypeKind.Enum;

		private bool ITypeReferenceIsValueType => AdaptedNamedTypeSymbol.IsValueType;

		private Microsoft.Cci.PrimitiveTypeCode ITypeReferenceTypeCode
		{
			get
			{
				if (AdaptedNamedTypeSymbol.IsDefinition)
				{
					return AdaptedNamedTypeSymbol.PrimitiveTypeCode;
				}
				return Microsoft.Cci.PrimitiveTypeCode.NotPrimitive;
			}
		}

		private TypeDefinitionHandle ITypeReferenceTypeDef
		{
			get
			{
				if (AdaptedNamedTypeSymbol is PENamedTypeSymbol pENamedTypeSymbol)
				{
					return pENamedTypeSymbol.Handle;
				}
				return default(TypeDefinitionHandle);
			}
		}

		private IGenericMethodParameterReference ITypeReferenceAsGenericMethodParameterReference => null;

		private IGenericTypeInstanceReference ITypeReferenceAsGenericTypeInstanceReference
		{
			get
			{
				if (!AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.Arity > 0 && (object)AdaptedNamedTypeSymbol.ConstructedFrom != AdaptedNamedTypeSymbol)
				{
					return this;
				}
				return null;
			}
		}

		private IGenericTypeParameterReference ITypeReferenceAsGenericTypeParameterReference => null;

		private INamespaceTypeReference ITypeReferenceAsNamespaceTypeReference
		{
			get
			{
				if (AdaptedNamedTypeSymbol.IsDefinition && (object)AdaptedNamedTypeSymbol.ContainingType == null)
				{
					return this;
				}
				return null;
			}
		}

		private INestedTypeReference ITypeReferenceAsNestedTypeReference
		{
			get
			{
				if ((object)AdaptedNamedTypeSymbol.ContainingType != null)
				{
					return this;
				}
				return null;
			}
		}

		private ISpecializedNestedTypeReference ITypeReferenceAsSpecializedNestedTypeReference
		{
			get
			{
				if (!AdaptedNamedTypeSymbol.IsDefinition && (AdaptedNamedTypeSymbol.Arity == 0 || (object)AdaptedNamedTypeSymbol.ConstructedFrom == AdaptedNamedTypeSymbol))
				{
					return this;
				}
				return null;
			}
		}

		private ushort ITypeDefinitionAlignment => (ushort)AdaptedNamedTypeSymbol.Layout.Alignment;

		private IEnumerable<IGenericTypeParameter> ITypeDefinitionGenericParameters => AdaptedNamedTypeSymbol.TypeParameters;

		private ushort ITypeDefinitionGenericParameterCount => GenericParameterCountImpl;

		private ushort GenericParameterCountImpl => (ushort)AdaptedNamedTypeSymbol.Arity;

		private bool ITypeDefinitionHasDeclarativeSecurity => AdaptedNamedTypeSymbol.HasDeclarativeSecurity;

		private bool ITypeDefinitionIsAbstract => AdaptedNamedTypeSymbol.IsMetadataAbstract;

		private bool ITypeDefinitionIsBeforeFieldInit
		{
			get
			{
				switch (AdaptedNamedTypeSymbol.TypeKind)
				{
				case TypeKind.Delegate:
				case TypeKind.Enum:
				case TypeKind.Interface:
					return false;
				default:
				{
					MethodSymbol methodSymbol = AdaptedNamedTypeSymbol.SharedConstructors.FirstOrDefault();
					if ((object)methodSymbol != null)
					{
						if (!methodSymbol.IsImplicitlyDeclared)
						{
							return false;
						}
						ImmutableArray<Symbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.GetMembers().GetEnumerator();
						while (enumerator.MoveNext())
						{
							Symbol current = enumerator.Current;
							if (current.Kind != SymbolKind.Method)
							{
								continue;
							}
							ImmutableArray<HandledEvent> handledEvents = ((MethodSymbol)current).HandledEvents;
							if (handledEvents.IsEmpty)
							{
								continue;
							}
							ImmutableArray<HandledEvent>.Enumerator enumerator2 = handledEvents.GetEnumerator();
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.hookupMethod.MethodKind == MethodKind.StaticConstructor)
								{
									return false;
								}
							}
						}
						return true;
					}
					if (AdaptedNamedTypeSymbol is SourceMemberContainerTypeSymbol sourceMemberContainerTypeSymbol && !sourceMemberContainerTypeSymbol.StaticInitializers.IsDefaultOrEmpty)
					{
						return sourceMemberContainerTypeSymbol.AnyInitializerToBeInjectedIntoConstructor(sourceMemberContainerTypeSymbol.StaticInitializers, includingNonMetadataConstants: true);
					}
					return false;
				}
				}
			}
		}

		private bool ITypeDefinitionIsComObject => AdaptedNamedTypeSymbol.IsComImport;

		private bool ITypeDefinitionIsGeneric => AdaptedNamedTypeSymbol.Arity != 0;

		private bool ITypeDefinitionIsInterface => AdaptedNamedTypeSymbol.IsInterface;

		private bool ITypeDefinitionIsDelegate => TypeSymbolExtensions.IsDelegateType(AdaptedNamedTypeSymbol);

		private bool ITypeDefinitionIsRuntimeSpecial => false;

		private bool ITypeDefinitionIsSerializable => AdaptedNamedTypeSymbol.IsSerializable;

		private bool ITypeDefinitionIsSpecialName => AdaptedNamedTypeSymbol.HasSpecialName;

		private bool ITypeDefinitionIsWindowsRuntimeImport => AdaptedNamedTypeSymbol.IsWindowsRuntimeImport;

		private bool ITypeDefinitionIsSealed => AdaptedNamedTypeSymbol.IsMetadataSealed;

		private LayoutKind ITypeDefinitionLayout => AdaptedNamedTypeSymbol.Layout.Kind;

		private IEnumerable<SecurityAttribute> ITypeDefinitionSecurityAttributes => AdaptedNamedTypeSymbol.GetSecurityInformation();

		private uint ITypeDefinitionSizeOf => (uint)AdaptedNamedTypeSymbol.Layout.Size;

		private CharSet ITypeDefinitionStringFormat => AdaptedNamedTypeSymbol.MarshallingCharSet;

		private ushort INamedTypeReferenceGenericParameterCount => GenericParameterCountImpl;

		private bool INamedTypeReferenceMangleName => AdaptedNamedTypeSymbol.MangleName;

		private string INamedEntityName => AdaptedNamedTypeSymbol.Name;

		private string INamespaceTypeReferenceNamespaceName => AdaptedNamedTypeSymbol.GetEmittedNamespaceName() ?? AdaptedNamedTypeSymbol.ContainingNamespace.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat);

		private bool INamespaceTypeDefinitionIsPublic => PEModuleBuilder.MemberVisibility(AdaptedNamedTypeSymbol) == TypeMemberVisibility.Public;

		private ITypeDefinition ITypeDefinitionMemberContainingTypeDefinition => AdaptedNamedTypeSymbol.ContainingType.GetCciAdapter();

		private TypeMemberVisibility ITypeDefinitionMemberVisibility => PEModuleBuilder.MemberVisibility(AdaptedNamedTypeSymbol);

		private INamedTypeReference GenericTypeImpl => ((PEModuleBuilder)context.Module).Translate(AdaptedNamedTypeSymbol.OriginalDefinition, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, needDeclaration: true);

		internal NamedTypeSymbol AdaptedNamedTypeSymbol => this;

		internal virtual bool IsMetadataAbstract
		{
			get
			{
				if (!IsMustInherit)
				{
					return IsInterface;
				}
				return true;
			}
		}

		internal virtual bool IsMetadataSealed
		{
			get
			{
				if (IsNotInheritable)
				{
					return true;
				}
				TypeKind typeKind = TypeKind;
				if (typeKind == TypeKind.Enum || typeKind == TypeKind.Module || typeKind == TypeKind.Struct)
				{
					return true;
				}
				return false;
			}
		}

		public abstract int Arity { get; }

		public abstract ImmutableArray<TypeParameterSymbol> TypeParameters { get; }

		internal abstract bool HasTypeArgumentsCustomModifiers { get; }

		internal abstract ImmutableArray<TypeSymbol> TypeArgumentsNoUseSiteDiagnostics { get; }

		public abstract NamedTypeSymbol ConstructedFrom { get; }

		public virtual NamedTypeSymbol EnumUnderlyingType => null;

		public override NamedTypeSymbol ContainingType => ContainingSymbol as NamedTypeSymbol;

		public virtual Symbol AssociatedSymbol => null;

		internal virtual bool KnownCircularStruct => false;

		internal virtual bool IsExplicitDefinitionOfNoPiaLocalType => false;

		public override string MetadataName
		{
			get
			{
				if (!MangleName)
				{
					return Name;
				}
				return MetadataHelpers.ComposeAritySuffixedMetadataName(Name, Arity);
			}
		}

		internal virtual bool IsDirectlyExcludedFromCodeCoverage => false;

		internal abstract bool MangleName { get; }

		internal abstract bool HasSpecialName { get; }

		public abstract bool IsSerializable { get; }

		internal abstract TypeLayout Layout { get; }

		protected CharSet DefaultMarshallingCharSet
		{
			get
			{
				CharSet? effectiveDefaultMarshallingCharSet;
				CharSet? charSet = (effectiveDefaultMarshallingCharSet = base.EffectiveDefaultMarshallingCharSet);
				if (!charSet.HasValue)
				{
					return CharSet.Ansi;
				}
				return effectiveDefaultMarshallingCharSet.GetValueOrDefault();
			}
		}

		internal abstract CharSet MarshallingCharSet { get; }

		public virtual MethodSymbol DelegateInvokeMethod
		{
			get
			{
				if (TypeKind != TypeKind.Delegate)
				{
					return null;
				}
				ImmutableArray<Symbol> members = GetMembers("Invoke");
				if (members.Length != 1)
				{
					return null;
				}
				return members[0] as MethodSymbol;
			}
		}

		public abstract bool IsMustInherit { get; }

		public abstract bool IsNotInheritable { get; }

		public abstract bool MightContainExtensionMethods { get; }

		internal abstract bool HasCodeAnalysisEmbeddedAttribute { get; }

		internal abstract bool HasVisualBasicEmbeddedAttribute { get; }

		internal abstract bool IsExtensibleInterfaceNoUseSiteDiagnostics { get; }

		public ImmutableArray<MethodSymbol> InstanceConstructors => GetConstructors<MethodSymbol>(includeInstance: true, includeShared: false);

		public ImmutableArray<MethodSymbol> SharedConstructors => GetConstructors<MethodSymbol>(includeInstance: false, includeShared: true);

		public ImmutableArray<MethodSymbol> Constructors => GetConstructors<MethodSymbol>(includeInstance: true, includeShared: true);

		public override bool IsReferenceType
		{
			get
			{
				if (TypeKind != TypeKind.Enum && TypeKind != TypeKind.Struct)
				{
					return TypeKind != TypeKind.Error;
				}
				return false;
			}
		}

		public override bool IsValueType
		{
			get
			{
				if (TypeKind != TypeKind.Enum)
				{
					return TypeKind == TypeKind.Struct;
				}
				return true;
			}
		}

		internal abstract bool CanConstruct { get; }

		internal abstract string DefaultPropertyName { get; }

		internal abstract TypeSubstitution TypeSubstitution { get; }

		public abstract override string Name { get; }

		public abstract IEnumerable<string> MemberNames { get; }

		public virtual bool IsScriptClass => false;

		public bool IsSubmissionClass => TypeKind == TypeKind.Submission;

		public virtual bool IsImplicitClass => false;

		public abstract override Accessibility DeclaredAccessibility { get; }

		public override SymbolKind Kind => SymbolKind.NamedType;

		internal abstract bool IsComImport { get; }

		internal abstract TypeSymbol CoClassType { get; }

		internal bool IsConditional => GetAppliedConditionalSymbols().Any();

		internal virtual bool AreMembersImplicitlyDeclared => false;

		internal abstract bool HasDeclarativeSecurity { get; }

		internal sealed override NamedTypeSymbol BaseTypeNoUseSiteDiagnostics
		{
			get
			{
				if ((object)_lazyBaseType == ErrorTypeSymbol.UnknownResultType)
				{
					if ((object)ContainingType != null)
					{
						_ = ContainingType.BaseTypeNoUseSiteDiagnostics;
					}
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					NamedTypeSymbol value = MakeAcyclicBaseType(instance);
					AtomicStoreReferenceAndDiagnostics(ref _lazyBaseType, value, instance, ErrorTypeSymbol.UnknownResultType);
					instance.Free();
				}
				return _lazyBaseType;
			}
		}

		internal sealed override ImmutableArray<NamedTypeSymbol> InterfacesNoUseSiteDiagnostics
		{
			get
			{
				if (_lazyInterfaces.IsDefault)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					ImmutableArray<NamedTypeSymbol> value = MakeAcyclicInterfaces(instance);
					AtomicStoreArrayAndDiagnostics(ref _lazyInterfaces, value, instance);
					instance.Free();
				}
				return _lazyInterfaces;
			}
		}

		public bool IsGenericType
		{
			get
			{
				NamedTypeSymbol namedTypeSymbol = this;
				while ((object)namedTypeSymbol != null)
				{
					if (namedTypeSymbol.Arity != 0)
					{
						return true;
					}
					namedTypeSymbol = namedTypeSymbol.ContainingType;
				}
				return false;
			}
		}

		public new virtual NamedTypeSymbol OriginalDefinition => this;

		protected sealed override TypeSymbol OriginalTypeSymbolDefinition => OriginalDefinition;

		public virtual bool IsUnboundGenericType => false;

		internal abstract bool IsWindowsRuntimeImport { get; }

		internal abstract bool ShouldAddWinRTMembers { get; }

		internal abstract bool IsInterface { get; }

		private int INamedTypeSymbol_Arity => Arity;

		private INamedTypeSymbol INamedTypeSymbol_ConstructedFrom => ConstructedFrom;

		private IMethodSymbol INamedTypeSymbol_DelegateInvokeMethod => DelegateInvokeMethod;

		private INamedTypeSymbol INamedTypeSymbol_EnumUnderlyingType => EnumUnderlyingType;

		private INamedTypeSymbolInternal INamedTypeSymbolInternal_EnumUnderlyingType => EnumUnderlyingType;

		private IEnumerable<string> INamedTypeSymbol_MemberNames => MemberNames;

		private bool INamedTypeSymbol_IsUnboundGenericType => IsUnboundGenericType;

		private INamedTypeSymbol INamedTypeSymbol_OriginalDefinition => OriginalDefinition;

		private ImmutableArray<ITypeSymbol> INamedTypeSymbol_TypeArguments => StaticCast<ITypeSymbol>.From(TypeArgumentsNoUseSiteDiagnostics);

		private ImmutableArray<NullableAnnotation> TypeArgumentNullableAnnotations => TypeArgumentsNoUseSiteDiagnostics.SelectAsArray((TypeSymbol t) => NullableAnnotation.None);

		private ImmutableArray<ITypeParameterSymbol> INamedTypeSymbol_TypeParameters => StaticCast<ITypeParameterSymbol>.From(TypeParameters);

		private bool INamedTypeSymbol_IsScriptClass => IsScriptClass;

		private bool INamedTypeSymbol_IsImplicitClass => IsImplicitClass;

		private ImmutableArray<IMethodSymbol> INamedTypeSymbol_InstanceConstructors => GetConstructors<IMethodSymbol>(includeInstance: true, includeShared: false);

		private ImmutableArray<IMethodSymbol> INamedTypeSymbol_StaticConstructors => GetConstructors<IMethodSymbol>(includeInstance: false, includeShared: true);

		private ImmutableArray<IMethodSymbol> INamedTypeSymbol_Constructors => GetConstructors<IMethodSymbol>(includeInstance: true, includeShared: true);

		private ISymbol INamedTypeSymbol_AssociatedSymbol => AssociatedSymbol;

		private bool INamedTypeSymbol_IsComImport => IsComImport;

		private INamedTypeSymbol INamedTypeSymbol_NativeIntegerUnderlyingType => null;

		protected override bool ISymbol_IsAbstract => IsMustInherit;

		protected override bool ISymbol_IsSealed => IsNotInheritable;

		private ImmutableArray<IFieldSymbol> INamedTypeSymbol_TupleElements => StaticCast<IFieldSymbol>.From(TupleElements);

		private INamedTypeSymbol INamedTypeSymbol_TupleUnderlyingType => TupleUnderlyingType;

		private ITypeDefinition ITypeReferenceGetResolvedType(EmitContext context)
		{
			PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
			return AsTypeDefinitionImpl(moduleBeingBuilt);
		}

		ITypeDefinition ITypeReference.GetResolvedType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceGetResolvedType
			return this.ITypeReferenceGetResolvedType(context);
		}

		private INamespaceTypeDefinition ITypeReferenceAsNamespaceTypeDefinition(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			if ((object)AdaptedNamedTypeSymbol.ContainingType == null && AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.ContainingModule.Equals(pEModuleBuilder.SourceModule))
			{
				return this;
			}
			return null;
		}

		INamespaceTypeDefinition ITypeReference.AsNamespaceTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsNamespaceTypeDefinition
			return this.ITypeReferenceAsNamespaceTypeDefinition(context);
		}

		private INestedTypeDefinition ITypeReferenceAsNestedTypeDefinition(EmitContext context)
		{
			PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
			return AsNestedTypeDefinitionImpl(moduleBeingBuilt);
		}

		INestedTypeDefinition ITypeReference.AsNestedTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsNestedTypeDefinition
			return this.ITypeReferenceAsNestedTypeDefinition(context);
		}

		private INestedTypeDefinition AsNestedTypeDefinitionImpl(PEModuleBuilder moduleBeingBuilt)
		{
			if ((object)AdaptedNamedTypeSymbol.ContainingType != null && AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.ContainingModule.Equals(moduleBeingBuilt.SourceModule))
			{
				return this;
			}
			return null;
		}

		private ITypeDefinition ITypeReferenceAsTypeDefinition(EmitContext context)
		{
			PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
			return AsTypeDefinitionImpl(moduleBeingBuilt);
		}

		ITypeDefinition ITypeReference.AsTypeDefinition(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeReferenceAsTypeDefinition
			return this.ITypeReferenceAsTypeDefinition(context);
		}

		private ITypeDefinition AsTypeDefinitionImpl(PEModuleBuilder moduleBeingBuilt)
		{
			if (AdaptedNamedTypeSymbol.IsDefinition && AdaptedNamedTypeSymbol.ContainingModule.Equals(moduleBeingBuilt.SourceModule))
			{
				return this;
			}
			return null;
		}

		internal sealed override void IReferenceDispatch(MetadataVisitor visitor)
		{
			if (!AdaptedNamedTypeSymbol.IsDefinition)
			{
				if (AdaptedNamedTypeSymbol.Arity > 0 && (object)AdaptedNamedTypeSymbol.ConstructedFrom != AdaptedNamedTypeSymbol)
				{
					visitor.Visit((IGenericTypeInstanceReference)this);
				}
				else
				{
					visitor.Visit((INestedTypeReference)this);
				}
				return;
			}
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)visitor.Context.Module;
			bool flag = AdaptedNamedTypeSymbol.ContainingModule.Equals(pEModuleBuilder.SourceModule);
			if ((object)AdaptedNamedTypeSymbol.ContainingType == null)
			{
				if (flag)
				{
					visitor.Visit((INamespaceTypeDefinition)this);
				}
				else
				{
					visitor.Visit((INamespaceTypeReference)this);
				}
			}
			else if (flag)
			{
				visitor.Visit((INestedTypeDefinition)this);
			}
			else
			{
				visitor.Visit((INestedTypeReference)this);
			}
		}

		internal sealed override IDefinition IReferenceAsDefinition(EmitContext context)
		{
			PEModuleBuilder moduleBeingBuilt = (PEModuleBuilder)context.Module;
			return AsTypeDefinitionImpl(moduleBeingBuilt);
		}

		private ITypeReference ITypeDefinitionGetBaseClass(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			NamedTypeSymbol namedTypeSymbol = AdaptedNamedTypeSymbol.BaseTypeNoUseSiteDiagnostics;
			if (AdaptedNamedTypeSymbol.TypeKind == TypeKind.Submission)
			{
				namedTypeSymbol = AdaptedNamedTypeSymbol.ContainingAssembly.GetSpecialType(SpecialType.System_Object);
			}
			if ((object)namedTypeSymbol != null)
			{
				return pEModuleBuilder.Translate(namedTypeSymbol, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
			}
			return null;
		}

		ITypeReference ITypeDefinition.GetBaseClass(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionGetBaseClass
			return this.ITypeDefinitionGetBaseClass(context);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_31_ITypeDefinitionEvents))]
		private IEnumerable<IEventDefinition> ITypeDefinitionEvents(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_31_ITypeDefinitionEvents(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<IEventDefinition> ITypeDefinition.GetEvents(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionEvents
			return this.ITypeDefinitionEvents(context);
		}

		private IEnumerable<Microsoft.Cci.MethodImplementation> ITypeDefinitionGetExplicitImplementationOverrides(EmitContext context)
		{
			if (AdaptedNamedTypeSymbol.IsInterface)
			{
				return SpecializedCollections.EmptyEnumerable<Microsoft.Cci.MethodImplementation>();
			}
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			SourceNamedTypeSymbol sourceNamedType = AdaptedNamedTypeSymbol as SourceNamedTypeSymbol;
			ArrayBuilder<Microsoft.Cci.MethodImplementation> instance = ArrayBuilder<Microsoft.Cci.MethodImplementation>.GetInstance();
			ImmutableArray<Symbol>.Enumerator enumerator = AdaptedNamedTypeSymbol.GetMembersForCci().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Method)
				{
					AddExplicitImplementations(context, (MethodSymbol)current, instance, sourceNamedType, pEModuleBuilder);
				}
			}
			IEnumerable<IMethodDefinition> synthesizedMethods = pEModuleBuilder.GetSynthesizedMethods(AdaptedNamedTypeSymbol);
			if (synthesizedMethods != null)
			{
				foreach (IMethodDefinition item in synthesizedMethods)
				{
					if (item.GetInternalSymbol() is MethodSymbol implementingMethod)
					{
						AddExplicitImplementations(context, implementingMethod, instance, sourceNamedType, pEModuleBuilder);
					}
				}
			}
			return instance.ToImmutableAndFree();
		}

		IEnumerable<Microsoft.Cci.MethodImplementation> ITypeDefinition.GetExplicitImplementationOverrides(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionGetExplicitImplementationOverrides
			return this.ITypeDefinitionGetExplicitImplementationOverrides(context);
		}

		private void AddExplicitImplementations(EmitContext context, MethodSymbol implementingMethod, ArrayBuilder<Microsoft.Cci.MethodImplementation> explicitImplements, SourceNamedTypeSymbol sourceNamedType, PEModuleBuilder moduleBeingBuilt)
		{
			ImmutableArray<MethodSymbol>.Enumerator enumerator = implementingMethod.ExplicitInterfaceImplementations.GetEnumerator();
			while (enumerator.MoveNext())
			{
				MethodSymbol current = enumerator.Current;
				if (MethodSignatureComparer.CustomModifiersAndParametersAndReturnTypeSignatureComparer.Equals(implementingMethod, current))
				{
					explicitImplements.Add(new Microsoft.Cci.MethodImplementation(implementingMethod.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(current, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics)));
				}
			}
			if (OverrideHidingHelper.RequiresExplicitOverride(implementingMethod))
			{
				explicitImplements.Add(new Microsoft.Cci.MethodImplementation(implementingMethod.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(implementingMethod.OverriddenMethod, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics)));
			}
			if ((object)sourceNamedType != null)
			{
				MethodSymbol correspondingComClassInterfaceMethod = sourceNamedType.GetCorrespondingComClassInterfaceMethod(implementingMethod);
				if ((object)correspondingComClassInterfaceMethod != null)
				{
					explicitImplements.Add(new Microsoft.Cci.MethodImplementation(implementingMethod.GetCciAdapter(), moduleBeingBuilt.TranslateOverriddenMethodReference(correspondingComClassInterfaceMethod, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics)));
				}
			}
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_34_ITypeDefinitionGetFields))]
		private IEnumerable<IFieldDefinition> ITypeDefinitionGetFields(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_34_ITypeDefinitionGetFields(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<IFieldDefinition> ITypeDefinition.GetFields(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionGetFields
			return this.ITypeDefinitionGetFields(context);
		}

		private bool IsWithEventsField(FieldSymbol field)
		{
			return field is SourceWithEventsBackingFieldSymbol;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_44_ITypeDefinitionInterfaces))]
		private IEnumerable<TypeReferenceWithAttributes> ITypeDefinitionInterfaces(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_44_ITypeDefinitionInterfaces(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<TypeReferenceWithAttributes> ITypeDefinition.Interfaces(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionInterfaces
			return this.ITypeDefinitionInterfaces(context);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_69_ITypeDefinitionGetMethods))]
		private IEnumerable<IMethodDefinition> ITypeDefinitionGetMethods(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_69_ITypeDefinitionGetMethods(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<IMethodDefinition> ITypeDefinition.GetMethods(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionGetMethods
			return this.ITypeDefinitionGetMethods(context);
		}

		private IEnumerable<INestedTypeDefinition> ITypeDefinitionGetNestedTypes(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			ImmutableArray<NamedTypeSymbol> typeMembers = AdaptedNamedTypeSymbol.GetTypeMembers();
			IEnumerable<INestedTypeDefinition> enumerable;
			if (typeMembers.Length == 0)
			{
				enumerable = SpecializedCollections.EmptyEnumerable<INestedTypeDefinition>();
			}
			else
			{
				IEnumerable<NamedTypeSymbol> enumerable2 = ((!AdaptedNamedTypeSymbol.IsEmbedded) ? ((IEnumerable<NamedTypeSymbol>)typeMembers) : System.Linq.ImmutableArrayExtensions.Where(typeMembers, pEModuleBuilder.SourceModule.ContainingSourceAssembly.DeclaringCompilation.EmbeddedSymbolManager.IsReferencedPredicate));
				enumerable = enumerable2;
			}
			IEnumerable<INestedTypeDefinition> synthesizedTypes = pEModuleBuilder.GetSynthesizedTypes(AdaptedNamedTypeSymbol);
			if (synthesizedTypes != null)
			{
				enumerable = enumerable.Concat(synthesizedTypes);
			}
			return enumerable;
		}

		IEnumerable<INestedTypeDefinition> ITypeDefinition.GetNestedTypes(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionGetNestedTypes
			return this.ITypeDefinitionGetNestedTypes(context);
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_71_ITypeDefinitionGetProperties))]
		private IEnumerable<IPropertyDefinition> ITypeDefinitionGetProperties(EmitContext context)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_71_ITypeDefinitionGetProperties(-2)
			{
				_0024VB_0024Me = this,
				_0024P_context = context
			};
		}

		IEnumerable<IPropertyDefinition> ITypeDefinition.GetProperties(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeDefinitionGetProperties
			return this.ITypeDefinitionGetProperties(context);
		}

		private IUnitReference INamespaceTypeReferenceGetUnit(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(AdaptedNamedTypeSymbol.ContainingModule, context.Diagnostics);
		}

		IUnitReference INamespaceTypeReference.GetUnit(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamespaceTypeReferenceGetUnit
			return this.INamespaceTypeReferenceGetUnit(context);
		}

		private ITypeReference ITypeMemberReferenceGetContainingType(EmitContext context)
		{
			return ((PEModuleBuilder)context.Module).Translate(AdaptedNamedTypeSymbol.ContainingType, (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics, fromImplements: false, AdaptedNamedTypeSymbol.IsDefinition);
		}

		ITypeReference ITypeMemberReference.GetContainingType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ITypeMemberReferenceGetContainingType
			return this.ITypeMemberReferenceGetContainingType(context);
		}

		private ImmutableArray<ITypeReference> IGenericTypeInstanceReferenceGetGenericArguments(EmitContext context)
		{
			PEModuleBuilder pEModuleBuilder = (PEModuleBuilder)context.Module;
			bool hasTypeArgumentsCustomModifiers = AdaptedNamedTypeSymbol.HasTypeArgumentsCustomModifiers;
			ArrayBuilder<ITypeReference> instance = ArrayBuilder<ITypeReference>.GetInstance();
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = AdaptedNamedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics;
			int num = typeArgumentsNoUseSiteDiagnostics.Length - 1;
			for (int i = 0; i <= num; i++)
			{
				ITypeReference typeReference = pEModuleBuilder.Translate(typeArgumentsNoUseSiteDiagnostics[i], (VisualBasicSyntaxNode)context.SyntaxNode, context.Diagnostics);
				if (hasTypeArgumentsCustomModifiers)
				{
					ImmutableArray<CustomModifier> typeArgumentCustomModifiers = AdaptedNamedTypeSymbol.GetTypeArgumentCustomModifiers(i);
					if (!typeArgumentCustomModifiers.IsDefaultOrEmpty)
					{
						typeReference = new ModifiedTypeReference(typeReference, typeArgumentCustomModifiers.As<ICustomModifier>());
					}
				}
				instance.Add(typeReference);
			}
			return instance.ToImmutableAndFree();
		}

		ImmutableArray<ITypeReference> IGenericTypeInstanceReference.GetGenericArguments(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericTypeInstanceReferenceGetGenericArguments
			return this.IGenericTypeInstanceReferenceGetGenericArguments(context);
		}

		private INamedTypeReference IGenericTypeInstanceReferenceGetGenericType(EmitContext context)
		{
			return this.get_GenericTypeImpl(context);
		}

		INamedTypeReference IGenericTypeInstanceReference.GetGenericType(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in IGenericTypeInstanceReferenceGetGenericType
			return this.IGenericTypeInstanceReferenceGetGenericType(context);
		}

		private INestedTypeReference ISpecializedNestedTypeReferenceGetUnspecializedVersion(EmitContext context)
		{
			return this.get_GenericTypeImpl(context).AsNestedTypeReference;
		}

		INestedTypeReference ISpecializedNestedTypeReference.GetUnspecializedVersion(EmitContext context)
		{
			//ILSpy generated this explicit interface implementation from .override directive in ISpecializedNestedTypeReferenceGetUnspecializedVersion
			return this.ISpecializedNestedTypeReferenceGetUnspecializedVersion(context);
		}

		internal new NamedTypeSymbol GetCciAdapter()
		{
			return this;
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_102_GetEventsToEmit))]
		internal virtual IEnumerable<EventSymbol> GetEventsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_102_GetEventsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		internal abstract IEnumerable<FieldSymbol> GetFieldsToEmit();

		internal virtual IEnumerable<NamedTypeSymbol> GetSynthesizedImplements()
		{
			return null;
		}

		internal virtual IEnumerable<NamedTypeSymbol> GetInterfacesToEmit()
		{
			IEnumerable<NamedTypeSymbol> synthesizedImplements = GetSynthesizedImplements();
			ImmutableArray<NamedTypeSymbol> interfacesNoUseSiteDiagnostics = InterfacesNoUseSiteDiagnostics;
			if (interfacesNoUseSiteDiagnostics.IsEmpty)
			{
				return synthesizedImplements ?? SpecializedCollections.EmptyEnumerable<NamedTypeSymbol>();
			}
			NamedTypeSymbol baseTypeNoUseSiteDiagnostics = BaseTypeNoUseSiteDiagnostics;
			IEnumerable<NamedTypeSymbol> enumerable = interfacesNoUseSiteDiagnostics.Where(delegate(NamedTypeSymbol sym)
			{
				if ((object)baseTypeNoUseSiteDiagnostics != null)
				{
					NamedTypeSymbol subType = baseTypeNoUseSiteDiagnostics;
					EqualsIgnoringComparer instanceCLRSignatureCompare = EqualsIgnoringComparer.InstanceCLRSignatureCompare;
					CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = CompoundUseSiteInfo<AssemblySymbol>.Discarded;
					if (TypeSymbolExtensions.ImplementsInterface(subType, sym, instanceCLRSignatureCompare, ref useSiteInfo))
					{
						return ImplementsAllMembersOfInterface(sym);
					}
				}
				return true;
			});
			return (synthesizedImplements == null) ? enumerable : synthesizedImplements.Concat(enumerable);
		}

		internal virtual ImmutableArray<Symbol> GetMembersForCci()
		{
			return GetMembers();
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_111_GetMethodsToEmit))]
		internal virtual IEnumerable<MethodSymbol> GetMethodsToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_111_GetMethodsToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_112_GetPropertiesToEmit))]
		internal virtual IEnumerable<PropertySymbol> GetPropertiesToEmit()
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_112_GetPropertiesToEmit(-2)
			{
				_0024VB_0024Me = this
			};
		}

		public abstract ImmutableArray<CustomModifier> GetTypeArgumentCustomModifiers(int ordinal);

		internal ImmutableArray<CustomModifier> GetEmptyTypeArgumentCustomModifiers(int ordinal)
		{
			if (ordinal < 0 || ordinal >= Arity)
			{
				throw new IndexOutOfRangeException();
			}
			return ImmutableArray<CustomModifier>.Empty;
		}

		internal ImmutableArray<TypeSymbol> TypeArgumentsWithDefinitionUseSiteDiagnostics([In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<TypeSymbol> typeArgumentsNoUseSiteDiagnostics = TypeArgumentsNoUseSiteDiagnostics;
			ImmutableArray<TypeSymbol>.Enumerator enumerator = typeArgumentsNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbolExtensions.AddUseSiteInfo(enumerator.Current.OriginalDefinition, ref useSiteInfo);
			}
			return typeArgumentsNoUseSiteDiagnostics;
		}

		internal TypeSymbol TypeArgumentWithDefinitionUseSiteDiagnostics(int index, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			TypeSymbol typeSymbol = TypeArgumentsNoUseSiteDiagnostics[index];
			TypeSymbolExtensions.AddUseSiteInfo(typeSymbol.OriginalDefinition, ref useSiteInfo);
			return typeSymbol;
		}

		internal virtual bool GetGuidString(ref string guidString)
		{
			return GetGuidStringDefaultImplementation(out guidString);
		}

		internal virtual void AppendProbableExtensionMethods(string name, ArrayBuilder<MethodSymbol> methods)
		{
			if (!MightContainExtensionMethods)
			{
				return;
			}
			ImmutableArray<Symbol>.Enumerator enumerator = GetMembers(name).GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Method)
				{
					MethodSymbol methodSymbol = (MethodSymbol)current;
					if (methodSymbol.MayBeReducibleExtensionMethod)
					{
						methods.Add(methodSymbol);
					}
				}
			}
		}

		internal virtual void BuildExtensionMethodsMap(Dictionary<string, ArrayBuilder<MethodSymbol>> map, NamespaceSymbol appendThrough)
		{
			if (MightContainExtensionMethods)
			{
				appendThrough.BuildExtensionMethodsMap(map, MemberNames.Select((string name) => new KeyValuePair<string, ImmutableArray<Symbol>>(name, GetMembers(name))));
			}
		}

		internal virtual void GetExtensionMethods(ArrayBuilder<MethodSymbol> methods, NamespaceSymbol appendThrough, string Name)
		{
			if (MightContainExtensionMethods)
			{
				ImmutableArray<Symbol>.Enumerator enumerator = GetSimpleNonTypeMembers(Name).GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					appendThrough.AddMemberIfExtension(methods, current);
				}
			}
		}

		internal virtual void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder)
		{
			AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, this);
		}

		internal virtual void AddExtensionMethodLookupSymbolsInfo(LookupSymbolsInfo nameSet, LookupOptions options, Binder originalBinder, NamedTypeSymbol appendThrough)
		{
			if (MightContainExtensionMethods)
			{
				appendThrough.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, MemberNames.Select((string name) => new KeyValuePair<string, ImmutableArray<Symbol>>(name, GetMembers(name))));
			}
		}

		private ImmutableArray<TMethodSymbol> GetConstructors<TMethodSymbol>(bool includeInstance, bool includeShared) where TMethodSymbol : class, IMethodSymbol
		{
			ImmutableArray<Symbol> immutableArray = (includeInstance ? GetMembers(".ctor") : ImmutableArray<Symbol>.Empty);
			ImmutableArray<Symbol> immutableArray2 = (includeShared ? GetMembers(".cctor") : ImmutableArray<Symbol>.Empty);
			if (immutableArray.IsEmpty && immutableArray2.IsEmpty)
			{
				return ImmutableArray<TMethodSymbol>.Empty;
			}
			ArrayBuilder<TMethodSymbol> instance = ArrayBuilder<TMethodSymbol>.GetInstance();
			ImmutableArray<Symbol>.Enumerator enumerator = immutableArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (current.Kind == SymbolKind.Method)
				{
					TMethodSymbol item = current as TMethodSymbol;
					instance.Add(item);
				}
			}
			ImmutableArray<Symbol>.Enumerator enumerator2 = immutableArray2.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				Symbol current2 = enumerator2.Current;
				if (current2.Kind == SymbolKind.Method)
				{
					TMethodSymbol item2 = current2 as TMethodSymbol;
					instance.Add(item2);
				}
			}
			return instance.ToImmutableAndFree();
		}

		public NamedTypeSymbol Construct(params TypeSymbol[] typeArguments)
		{
			return Construct(typeArguments.AsImmutableOrNull());
		}

		public NamedTypeSymbol Construct(IEnumerable<TypeSymbol> typeArguments)
		{
			return Construct(typeArguments.AsImmutableOrNull());
		}

		public abstract NamedTypeSymbol Construct(ImmutableArray<TypeSymbol> typeArguments);

		protected void CheckCanConstructAndTypeArguments(ImmutableArray<TypeSymbol> typeArguments)
		{
			if (!CanConstruct || (object)this != ConstructedFrom)
			{
				throw new InvalidOperationException();
			}
			TypeSymbolExtensions.CheckTypeArguments(typeArguments, Arity);
		}

		internal NamedTypeSymbol Construct(TypeSubstitution substitution)
		{
			if (substitution == null)
			{
				return this;
			}
			substitution.ThrowIfSubstitutingToAlphaRenamedTypeParameter();
			return (NamedTypeSymbol)InternalSubstituteTypeParameters(substitution).AsTypeSymbolOnly();
		}

		public NamedTypeSymbol ConstructUnboundGenericType()
		{
			return NamedTypeSymbolExtensions.AsUnboundGenericType(this);
		}

		internal SynthesizedConstructorBase GetScriptConstructor()
		{
			return (SynthesizedConstructorBase)InstanceConstructors.Single();
		}

		internal SynthesizedInteractiveInitializerMethod GetScriptInitializer()
		{
			return (SynthesizedInteractiveInitializerMethod)GetMembers("<Initialize>").Single();
		}

		internal SynthesizedEntryPointSymbol GetScriptEntryPoint()
		{
			string name = ((TypeKind == TypeKind.Submission) ? "<Factory>" : "<Main>");
			return (SynthesizedEntryPointSymbol)GetMembers(name).Single();
		}

		public abstract override ImmutableArray<Symbol> GetMembers();

		public abstract override ImmutableArray<Symbol> GetMembers(string name);

		public abstract override ImmutableArray<NamedTypeSymbol> GetTypeMembers();

		public abstract override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name);

		public abstract override ImmutableArray<NamedTypeSymbol> GetTypeMembers(string name, int arity);

		internal override TResult Accept<TArgument, TResult>(VisualBasicSymbolVisitor<TArgument, TResult> visitor, TArgument arg)
		{
			return visitor.VisitNamedType(this, arg);
		}

		internal NamedTypeSymbol()
		{
			_lazyDeclaredBase = ErrorTypeSymbol.UnknownResultType;
			_lazyDeclaredInterfaces = default(ImmutableArray<NamedTypeSymbol>);
			_lazyBaseType = ErrorTypeSymbol.UnknownResultType;
		}

		internal abstract ImmutableArray<string> GetAppliedConditionalSymbols();

		internal abstract AttributeUsageInfo GetAttributeUsageInfo();

		internal abstract IEnumerable<SecurityAttribute> GetSecurityInformation();

		internal abstract NamedTypeSymbol MakeDeclaredBase(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics);

		internal abstract ImmutableArray<NamedTypeSymbol> MakeDeclaredInterfaces(BasesBeingResolved basesBeingResolved, BindingDiagnosticBag diagnostics);

		internal virtual NamedTypeSymbol GetDeclaredBase(BasesBeingResolved basesBeingResolved)
		{
			if ((object)_lazyDeclaredBase == ErrorTypeSymbol.UnknownResultType)
			{
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
				AtomicStoreReferenceAndDiagnostics(ref _lazyDeclaredBase, MakeDeclaredBase(basesBeingResolved, instance), instance, ErrorTypeSymbol.UnknownResultType);
				instance.Free();
			}
			return _lazyDeclaredBase;
		}

		internal virtual ImmutableArray<Symbol> GetSimpleNonTypeMembers(string name)
		{
			return GetMembers(name);
		}

		private void AtomicStoreReferenceAndDiagnostics<T>(ref T variable, T value, BindingDiagnosticBag diagBag, T comparand = null) where T : class
		{
			if (diagBag == null || diagBag.IsEmpty)
			{
				Interlocked.CompareExchange(ref variable, value, comparand);
			}
			else if (ContainingModule is SourceModuleSymbol sourceModuleSymbol)
			{
				sourceModuleSymbol.AtomicStoreReferenceAndDiagnostics(ref variable, value, diagBag, comparand);
			}
		}

		internal void AtomicStoreArrayAndDiagnostics<T>(ref ImmutableArray<T> variable, ImmutableArray<T> value, BindingDiagnosticBag diagBag)
		{
			if (diagBag == null || diagBag.IsEmpty)
			{
				ImmutableInterlocked.InterlockedCompareExchange(ref variable, value, default(ImmutableArray<T>));
			}
			else if (ContainingModule is SourceModuleSymbol sourceModuleSymbol)
			{
				sourceModuleSymbol.AtomicStoreArrayAndDiagnostics(ref variable, value, diagBag);
			}
		}

		internal virtual ImmutableArray<NamedTypeSymbol> GetDeclaredInterfacesNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			if (_lazyDeclaredInterfaces.IsDefault)
			{
				BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
				AtomicStoreArrayAndDiagnostics(ref _lazyDeclaredInterfaces, MakeDeclaredInterfaces(basesBeingResolved, instance), instance);
				instance.Free();
			}
			return _lazyDeclaredInterfaces;
		}

		internal ImmutableArray<NamedTypeSymbol> GetDeclaredInterfacesWithDefinitionUseSiteDiagnostics(BasesBeingResolved basesBeingResolved, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo)
		{
			ImmutableArray<NamedTypeSymbol> declaredInterfacesNoUseSiteDiagnostics = GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved);
			ImmutableArray<NamedTypeSymbol>.Enumerator enumerator = declaredInterfacesNoUseSiteDiagnostics.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TypeSymbolExtensions.AddUseSiteInfo(enumerator.Current.OriginalDefinition, ref useSiteInfo);
			}
			return declaredInterfacesNoUseSiteDiagnostics;
		}

		internal ImmutableArray<NamedTypeSymbol> GetDirectBaseInterfacesNoUseSiteDiagnostics(BasesBeingResolved basesBeingResolved)
		{
			if (TypeKind == TypeKind.Interface)
			{
				if (basesBeingResolved.InheritsBeingResolvedOpt == null)
				{
					return InterfacesNoUseSiteDiagnostics;
				}
				return GetDeclaredBaseInterfacesSafe(basesBeingResolved);
			}
			return ImmutableArray<NamedTypeSymbol>.Empty;
		}

		internal virtual ImmutableArray<NamedTypeSymbol> GetDeclaredBaseInterfacesSafe(BasesBeingResolved basesBeingResolved)
		{
			if (basesBeingResolved.InheritsBeingResolvedOpt.Contains(this))
			{
				return default(ImmutableArray<NamedTypeSymbol>);
			}
			return GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved.PrependInheritsBeingResolved(this));
		}

		internal abstract NamedTypeSymbol MakeAcyclicBaseType(BindingDiagnosticBag diagnostics);

		internal abstract ImmutableArray<NamedTypeSymbol> MakeAcyclicInterfaces(BindingDiagnosticBag diagnostics);

		internal NamedTypeSymbol GetBestKnownBaseType()
		{
			NamedTypeSymbol lazyBaseType = _lazyBaseType;
			if ((object)lazyBaseType != ErrorTypeSymbol.UnknownResultType)
			{
				return lazyBaseType;
			}
			return GetDeclaredBase(default(BasesBeingResolved));
		}

		internal ImmutableArray<NamedTypeSymbol> GetBestKnownInterfacesNoUseSiteDiagnostics()
		{
			ImmutableArray<NamedTypeSymbol> lazyInterfaces = _lazyInterfaces;
			if (!lazyInterfaces.IsDefault)
			{
				return lazyInterfaces;
			}
			return GetDeclaredInterfacesNoUseSiteDiagnostics(default(BasesBeingResolved));
		}

		internal virtual string GetEmittedNamespaceName()
		{
			return null;
		}

		internal bool ImplementsAllMembersOfInterface(NamedTypeSymbol iface)
		{
			MultiDictionary<Symbol, Symbol> explicitInterfaceImplementationMap = ExplicitInterfaceImplementationMap;
			ImmutableArray<Symbol>.Enumerator enumerator = iface.GetMembersUnordered().GetEnumerator();
			while (enumerator.MoveNext())
			{
				Symbol current = enumerator.Current;
				if (SymbolExtensions.RequiresImplementation(current) && !explicitInterfaceImplementationMap.ContainsKey(current))
				{
					return false;
				}
			}
			return true;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			if (base.IsDefinition)
			{
				return new UseSiteInfo<AssemblySymbol>(base.PrimaryDependency);
			}
			UseSiteInfo<AssemblySymbol> useSiteInfo = DeriveUseSiteInfoFromType(OriginalDefinition);
			DiagnosticInfo? diagnosticInfo = useSiteInfo.DiagnosticInfo;
			if (diagnosticInfo != null && diagnosticInfo!.Code == 30649)
			{
				return useSiteInfo;
			}
			UseSiteInfo<AssemblySymbol> second = DeriveUseSiteInfoFromTypeArguments();
			return MergeUseSiteInfo(useSiteInfo, second);
		}

		private UseSiteInfo<AssemblySymbol> DeriveUseSiteInfoFromTypeArguments()
		{
			UseSiteInfo<AssemblySymbol> result = default(UseSiteInfo<AssemblySymbol>);
			NamedTypeSymbol namedTypeSymbol = this;
			do
			{
				ImmutableArray<TypeSymbol>.Enumerator enumerator = namedTypeSymbol.TypeArgumentsNoUseSiteDiagnostics.GetEnumerator();
				while (enumerator.MoveNext())
				{
					TypeSymbol current = enumerator.Current;
					if (Symbol.MergeUseSiteInfo(ref result, DeriveUseSiteInfoFromType(current), 30649))
					{
						return result;
					}
				}
				if (namedTypeSymbol.HasTypeArgumentsCustomModifiers)
				{
					int num = Arity - 1;
					for (int i = 0; i <= num; i++)
					{
						if (Symbol.MergeUseSiteInfo(ref result, DeriveUseSiteInfoFromCustomModifiers(GetTypeArgumentCustomModifiers(i)), 30649))
						{
							return result;
						}
					}
				}
				namedTypeSymbol = namedTypeSymbol.ContainingType;
			}
			while ((object)namedTypeSymbol != null && !namedTypeSymbol.IsDefinition);
			return result;
		}

		internal abstract override void GenerateDeclarationErrors(CancellationToken cancellationToken);

		internal virtual IEnumerable<INestedTypeDefinition> GetSynthesizedNestedTypes()
		{
			return null;
		}

		internal abstract IEnumerable<PropertySymbol> GetSynthesizedWithEventsOverrides();

		private ImmutableArray<CustomModifier> INamedTypeSymbol_GetTypeArgumentCustomModifiers(int ordinal)
		{
			return GetTypeArgumentCustomModifiers(ordinal);
		}

		ImmutableArray<CustomModifier> INamedTypeSymbol.GetTypeArgumentCustomModifiers(int ordinal)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamedTypeSymbol_GetTypeArgumentCustomModifiers
			return this.INamedTypeSymbol_GetTypeArgumentCustomModifiers(ordinal);
		}

		private INamedTypeSymbol INamedTypeSymbol_Construct(params ITypeSymbol[] typeArguments)
		{
			return Construct(Symbol.ConstructTypeArguments(typeArguments));
		}

		INamedTypeSymbol INamedTypeSymbol.Construct(params ITypeSymbol[] typeArguments)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamedTypeSymbol_Construct
			return this.INamedTypeSymbol_Construct(typeArguments);
		}

		private INamedTypeSymbol INamedTypeSymbol_Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<NullableAnnotation> typeArgumentNullableAnnotations)
		{
			return Construct(Symbol.ConstructTypeArguments(typeArguments, typeArgumentNullableAnnotations));
		}

		INamedTypeSymbol INamedTypeSymbol.Construct(ImmutableArray<ITypeSymbol> typeArguments, ImmutableArray<NullableAnnotation> typeArgumentNullableAnnotations)
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamedTypeSymbol_Construct
			return this.INamedTypeSymbol_Construct(typeArguments, typeArgumentNullableAnnotations);
		}

		private INamedTypeSymbol INamedTypeSymbol_ConstructUnboundGenericType()
		{
			return ConstructUnboundGenericType();
		}

		INamedTypeSymbol INamedTypeSymbol.ConstructUnboundGenericType()
		{
			//ILSpy generated this explicit interface implementation from .override directive in INamedTypeSymbol_ConstructUnboundGenericType
			return this.INamedTypeSymbol_ConstructUnboundGenericType();
		}

		public override void Accept(SymbolVisitor visitor)
		{
			visitor.VisitNamedType(this);
		}

		public override TResult Accept<TResult>(SymbolVisitor<TResult> visitor)
		{
			return visitor.VisitNamedType(this);
		}

		public override void Accept(VisualBasicSymbolVisitor visitor)
		{
			visitor.VisitNamedType(this);
		}

		public override TResult Accept<TResult>(VisualBasicSymbolVisitor<TResult> visitor)
		{
			return visitor.VisitNamedType(this);
		}

		public sealed override bool IsTupleCompatible(out int tupleCardinality)
		{
			if (IsTupleType)
			{
				tupleCardinality = 0;
				return false;
			}
			if (!IsUnboundGenericType)
			{
				Symbol containingSymbol = ContainingSymbol;
				if ((object)containingSymbol != null && containingSymbol.Kind == SymbolKind.Namespace)
				{
					NamespaceSymbol containingNamespace = base.ContainingNamespace.ContainingNamespace;
					if ((object)containingNamespace != null && containingNamespace.IsGlobalNamespace && EmbeddedOperators.CompareString(Name, "ValueTuple", TextCompare: false) == 0 && EmbeddedOperators.CompareString(base.ContainingNamespace.Name, "System", TextCompare: false) == 0)
					{
						int arity = Arity;
						if (arity > 0 && arity < 8)
						{
							tupleCardinality = arity;
							return true;
						}
						if (arity == 8 && !base.IsDefinition)
						{
							TypeSymbol typeSymbol = this;
							int num = 0;
							do
							{
								num++;
								typeSymbol = ((NamedTypeSymbol)typeSymbol).TypeArgumentsNoUseSiteDiagnostics[7];
							}
							while (TypeSymbol.Equals(typeSymbol.OriginalDefinition, OriginalDefinition, TypeCompareKind.ConsiderEverything) && !typeSymbol.IsDefinition);
							if (typeSymbol.IsTupleType)
							{
								NamedTypeSymbol tupleUnderlyingType = typeSymbol.TupleUnderlyingType;
								if (tupleUnderlyingType.Arity == 8 && !TypeSymbol.Equals(tupleUnderlyingType.OriginalDefinition, OriginalDefinition, TypeCompareKind.ConsiderEverything))
								{
									tupleCardinality = 0;
									return false;
								}
								tupleCardinality = 7 * num + typeSymbol.TupleElementTypes.Length;
								return true;
							}
							arity = (typeSymbol as NamedTypeSymbol)?.Arity ?? 0;
							if (arity > 0 && arity < 8 && typeSymbol.IsTupleCompatible(out tupleCardinality))
							{
								tupleCardinality += 7 * num;
								return true;
							}
						}
					}
				}
			}
			tupleCardinality = 0;
			return false;
		}
	}
}
