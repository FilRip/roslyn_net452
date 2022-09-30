using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SourceMemberMethodSymbol : SourceNonPropertyAccessorMethodSymbol
	{
		[Flags]
		private enum StateFlags
		{
			SuppressDuplicateProcDefDiagnostics = 1,
			AllDiagnosticsReported = 2
		}

		private readonly string _name;

		private readonly int _arity;

		private readonly QuickAttributes _quickAttributes;

		private string _lazyMetadataName;

		private ImmutableArray<MethodSymbol> _lazyImplementedMethods;

		private ImmutableArray<TypeParameterSymbol> _lazyTypeParameters;

		private ImmutableArray<HandledEvent> _lazyHandles;

		private SourceMemberMethodSymbol _otherPartOfPartial;

		private readonly NamedTypeSymbol _asyncStateMachineType;

		private int _lazyState;

		public override string Name => _name;

		protected override SourceMethodSymbol BoundAttributesSource => SourcePartialDefinition;

		public override string MetadataName
		{
			get
			{
				if (_lazyMetadataName == null)
				{
					if (MethodKind == MethodKind.Ordinary)
					{
						OverloadingHelper.SetMetadataNameForAllOverloads(_name, SymbolKind.Method, m_containingType);
					}
					else
					{
						SetMetadataName(_name);
					}
				}
				return _lazyMetadataName;
			}
		}

		internal override bool GenerateDebugInfoImpl
		{
			get
			{
				if (base.GenerateDebugInfoImpl)
				{
					return !base.IsAsync;
				}
				return false;
			}
		}

		internal override bool MayBeReducibleExtensionMethod => (GetQuickAttributes() & QuickAttributes.Extension) != 0;

		public override bool IsExtensionMethod
		{
			get
			{
				if (MayBeReducibleExtensionMethod)
				{
					return base.IsExtensionMethod;
				}
				return false;
			}
		}

		internal override ObsoleteAttributeData ObsoleteAttributeData
		{
			get
			{
				if ((GetQuickAttributes() & QuickAttributes.Obsolete) != 0)
				{
					return base.ObsoleteAttributeData;
				}
				return null;
			}
		}

		public override int Arity => _arity;

		public override ImmutableArray<TypeParameterSymbol> TypeParameters
		{
			get
			{
				ImmutableArray<TypeParameterSymbol> lazyTypeParameters = _lazyTypeParameters;
				if (lazyTypeParameters.IsDefault)
				{
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					SourceModuleSymbol sourceModuleSymbol = (SourceModuleSymbol)ContainingModule;
					lazyTypeParameters = GetTypeParameters(sourceModuleSymbol, instance);
					sourceModuleSymbol.AtomicStoreArrayAndDiagnostics(ref _lazyTypeParameters, lazyTypeParameters, instance);
					instance.Free();
					lazyTypeParameters = _lazyTypeParameters;
				}
				return lazyTypeParameters;
			}
		}

		public override ImmutableArray<MethodSymbol> ExplicitInterfaceImplementations
		{
			get
			{
				if (_lazyImplementedMethods.IsDefault)
				{
					SourceModuleSymbol sourceModuleSymbol = (SourceModuleSymbol)ContainingModule;
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					ImmutableArray<MethodSymbol> value;
					if (base.IsPartial)
					{
						if (base.DeclarationSyntax is MethodStatementSyntax methodStatementSyntax && methodStatementSyntax.ImplementsClause != null)
						{
							instance.Add(ERRID.ERR_PartialDeclarationImplements1, methodStatementSyntax.Identifier.GetLocation(), methodStatementSyntax.Identifier.ToString());
						}
						value = PartialImplementationPart?.ExplicitInterfaceImplementations ?? ImmutableArray<MethodSymbol>.Empty;
					}
					else
					{
						value = GetExplicitInterfaceImplementations(sourceModuleSymbol, instance);
					}
					sourceModuleSymbol.AtomicStoreArrayAndDiagnostics(ref _lazyImplementedMethods, value, instance);
					instance.Free();
				}
				return _lazyImplementedMethods;
			}
		}

		internal override bool HasEmptyBody
		{
			get
			{
				if (!base.HasEmptyBody)
				{
					return false;
				}
				return SourcePartialImplementation?.HasEmptyBody ?? true;
			}
		}

		internal bool IsPartialDefinition => base.IsPartial;

		internal bool IsPartialImplementation
		{
			get
			{
				if (!IsPartialDefinition)
				{
					return (object)OtherPartOfPartial != null;
				}
				return false;
			}
		}

		public SourceMemberMethodSymbol SourcePartialDefinition
		{
			get
			{
				if (!IsPartialDefinition)
				{
					return OtherPartOfPartial;
				}
				return null;
			}
		}

		public SourceMemberMethodSymbol SourcePartialImplementation
		{
			get
			{
				if (!IsPartialDefinition)
				{
					return null;
				}
				return OtherPartOfPartial;
			}
		}

		public override MethodSymbol PartialDefinitionPart => SourcePartialDefinition;

		public override MethodSymbol PartialImplementationPart => SourcePartialImplementation;

		internal SourceMemberMethodSymbol OtherPartOfPartial
		{
			get
			{
				return _otherPartOfPartial;
			}
			private set
			{
				_ = _otherPartOfPartial;
				_otherPartOfPartial = value;
			}
		}

		internal bool SuppressDuplicateProcDefDiagnostics
		{
			get
			{
				return (_lazyState & 1) != 0;
			}
			set
			{
				ThreadSafeFlagOperations.Set(ref _lazyState, 1);
			}
		}

		public override ImmutableArray<HandledEvent> HandledEvents
		{
			get
			{
				if (_lazyHandles.IsDefault)
				{
					SourceModuleSymbol sourceModuleSymbol = (SourceModuleSymbol)ContainingModule;
					BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
					ImmutableArray<HandledEvent> handles = GetHandles(sourceModuleSymbol, instance);
					sourceModuleSymbol.AtomicStoreArrayAndDiagnostics(ref _lazyHandles, handles, instance);
					instance.Free();
				}
				return _lazyHandles;
			}
		}

		internal SourceMemberMethodSymbol(SourceMemberContainerTypeSymbol containingType, string name, SourceMemberFlags flags, Binder binder, MethodBaseSyntax syntax, int arity, ImmutableArray<HandledEvent> handledEvents = default(ImmutableArray<HandledEvent>))
			: base(containingType, flags, binder.GetSyntaxReference(syntax))
		{
			_asyncStateMachineType = null;
			_lazyHandles = handledEvents;
			_name = name;
			_arity = arity;
			_quickAttributes = binder.QuickAttributeChecker.CheckAttributes(syntax.AttributeLists);
			if (!NamedTypeSymbolExtensions.AllowsExtensionMethods(containingType))
			{
				_quickAttributes &= ~QuickAttributes.Extension;
			}
		}

		internal override void SetMetadataName(string metadataName)
		{
			Interlocked.CompareExchange(ref _lazyMetadataName, metadataName, null);
			if (base.IsPartial)
			{
				OtherPartOfPartial?.SetMetadataName(metadataName);
			}
		}

		protected override OneOrMany<SyntaxList<AttributeListSyntax>> GetAttributeDeclarations()
		{
			if ((object)SourcePartialImplementation != null)
			{
				return OneOrMany.Create(ImmutableArray.Create(base.AttributeDeclarationSyntaxList, SourcePartialImplementation.AttributeDeclarationSyntaxList));
			}
			return OneOrMany.Create(base.AttributeDeclarationSyntaxList);
		}

		private QuickAttributes GetQuickAttributes()
		{
			QuickAttributes quickAttributes = _quickAttributes;
			if (base.IsPartial)
			{
				SourceMemberMethodSymbol otherPartOfPartial = OtherPartOfPartial;
				if ((object)otherPartOfPartial != null)
				{
					return quickAttributes | otherPartOfPartial._quickAttributes;
				}
			}
			return quickAttributes;
		}

		internal override void AddSynthesizedAttributes(ModuleCompilationState compilationState, ref ArrayBuilder<SynthesizedAttributeData> attributes)
		{
			base.AddSynthesizedAttributes(compilationState, ref attributes);
			if (base.IsAsync || base.IsIterator)
			{
				Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeStateMachineAttribute(this, compilationState));
				if (base.IsAsync)
				{
					Symbol.AddSynthesizedAttribute(ref attributes, DeclaringCompilation.SynthesizeOptionalDebuggerStepThroughAttribute());
				}
			}
		}

		internal override void GenerateDeclarationErrors(CancellationToken cancellationToken)
		{
			if (((uint)_lazyState & 2u) != 0)
			{
				return;
			}
			base.GenerateDeclarationErrors(cancellationToken);
			BindingDiagnosticBag instance = BindingDiagnosticBag.GetInstance();
			if (!ExplicitInterfaceImplementations.IsEmpty)
			{
				ValidateImplementedMethodConstraints(instance);
			}
			SourceMemberMethodSymbol sourceMemberMethodSymbol = (base.IsPartial ? SourcePartialImplementation : this);
			if ((object)sourceMemberMethodSymbol != null && (sourceMemberMethodSymbol.IsAsync || sourceMemberMethodSymbol.IsIterator) && !TypeSymbolExtensions.IsInterfaceType(sourceMemberMethodSymbol.ContainingType))
			{
				NamedTypeSymbol containingType = sourceMemberMethodSymbol.ContainingType;
				do
				{
					if (containingType is SourceNamedTypeSymbol sourceNamedTypeSymbol && sourceNamedTypeSymbol.HasSecurityCriticalAttributes)
					{
						Location nonMergedLocation = sourceMemberMethodSymbol.NonMergedLocation;
						if ((object)nonMergedLocation != null)
						{
							Binder.ReportDiagnostic(instance, nonMergedLocation, ERRID.ERR_SecurityCriticalAsyncInClassOrStruct);
						}
						break;
					}
					containingType = containingType.ContainingType;
				}
				while ((object)containingType != null);
				if (sourceMemberMethodSymbol.IsAsync && (sourceMemberMethodSymbol.ImplementationAttributes & MethodImplAttributes.Synchronized) != 0)
				{
					Location nonMergedLocation2 = sourceMemberMethodSymbol.NonMergedLocation;
					if ((object)nonMergedLocation2 != null)
					{
						Binder.ReportDiagnostic(instance, nonMergedLocation2, ERRID.ERR_SynchronizedAsyncMethod);
					}
				}
			}
			if ((object)sourceMemberMethodSymbol != null && (object)sourceMemberMethodSymbol != this)
			{
				SymbolComparisonResults symbolComparisonResults = MethodSignatureComparer.DetailedCompare(this, sourceMemberMethodSymbol, (SymbolComparisonResults)5120);
				if (symbolComparisonResults != 0)
				{
					Location nonMergedLocation3 = sourceMemberMethodSymbol.NonMergedLocation;
					if ((object)nonMergedLocation3 != null)
					{
						if ((symbolComparisonResults & SymbolComparisonResults.ParamArrayMismatch) != 0)
						{
							Binder.ReportDiagnostic(instance, nonMergedLocation3, ERRID.ERR_PartialMethodParamArrayMismatch2, sourceMemberMethodSymbol, this);
						}
						else if ((symbolComparisonResults & SymbolComparisonResults.OptionalParameterValueMismatch) != 0)
						{
							Binder.ReportDiagnostic(instance, nonMergedLocation3, ERRID.ERR_PartialMethodDefaultParameterValueMismatch2, sourceMemberMethodSymbol, this);
						}
					}
				}
			}
			base.ContainingSourceModule.AtomicSetFlagAndStoreDiagnostics(ref _lazyState, 2, 0, instance);
			instance.Free();
		}

		private ImmutableArray<TypeParameterSymbol> GetTypeParameters(SourceModuleSymbol sourceModule, BindingDiagnosticBag diagBag)
		{
			TypeParameterListSyntax typeParameterListSyntax = SourceMethodSymbol.GetTypeParameterListSyntax(base.DeclarationSyntax);
			if (typeParameterListSyntax == null)
			{
				return ImmutableArray<TypeParameterSymbol>.Empty;
			}
			Binder binder = BinderBuilder.CreateBinderForType(sourceModule, base.SyntaxTree, m_containingType);
			SeparatedSyntaxList<TypeParameterSyntax> parameters = typeParameterListSyntax.Parameters;
			int count = parameters.Count;
			TypeParameterSymbol[] array = new TypeParameterSymbol[count - 1 + 1];
			int num = count - 1;
			for (int i = 0; i <= num; i++)
			{
				TypeParameterSyntax typeParameterSyntax = parameters[i];
				SyntaxToken identifier = typeParameterSyntax.Identifier;
				Binder.DisallowTypeCharacter(identifier, diagBag, ERRID.ERR_TypeCharOnGenericParam);
				array[i] = new SourceTypeParameterOnMethodSymbol(this, i, identifier.ValueText, binder.GetSyntaxReference(typeParameterSyntax));
				if (base.DeclarationSyntax.Kind() == SyntaxKind.FunctionStatement && CaseInsensitiveComparison.Equals(Name, identifier.ValueText))
				{
					Binder.ReportDiagnostic(diagBag, typeParameterSyntax, ERRID.ERR_TypeParamNameFunctionNameCollision);
				}
			}
			binder = new MethodTypeParametersBinder(binder, array.AsImmutableOrNull());
			if (ContainingType is SourceNamedTypeSymbol sourceNamedTypeSymbol)
			{
				sourceNamedTypeSymbol.CheckForDuplicateTypeParameters(array.AsImmutableOrNull(), diagBag);
			}
			return array.AsImmutableOrNull();
		}

		internal bool HasExplicitInterfaceImplementations()
		{
			if (base.DeclarationSyntax is MethodStatementSyntax methodStatementSyntax)
			{
				return methodStatementSyntax.ImplementsClause != null;
			}
			return false;
		}

		private ImmutableArray<MethodSymbol> GetExplicitInterfaceImplementations(SourceModuleSymbol sourceModule, BindingDiagnosticBag diagBag)
		{
			if (base.DeclarationSyntax is MethodStatementSyntax methodStatementSyntax && methodStatementSyntax.ImplementsClause != null)
			{
				Binder binder = BinderBuilder.CreateBinderForType(sourceModule, base.SyntaxTree, ContainingType);
				if (!(base.IsShared & !TypeSymbolExtensions.IsModuleType(ContainingType)))
				{
					return ImplementsHelper.ProcessImplementsClause(methodStatementSyntax.ImplementsClause, (MethodSymbol)this, (SourceMemberContainerTypeSymbol)ContainingType, binder, diagBag);
				}
				Binder.ReportDiagnostic(diagBag, Microsoft.CodeAnalysis.VisualBasicExtensions.First(methodStatementSyntax.Modifiers, SyntaxKind.SharedKeyword), ERRID.ERR_SharedOnProcThatImpl, methodStatementSyntax.Identifier.ToString());
			}
			return ImmutableArray<MethodSymbol>.Empty;
		}

		internal void ValidateImplementedMethodConstraints(BindingDiagnosticBag diagnostics)
		{
			if (base.IsPartial && (object)OtherPartOfPartial != null)
			{
				OtherPartOfPartial.ValidateImplementedMethodConstraints(diagnostics);
				return;
			}
			ImmutableArray<MethodSymbol> explicitInterfaceImplementations = ExplicitInterfaceImplementations;
			if (!explicitInterfaceImplementations.IsEmpty)
			{
				ImmutableArray<MethodSymbol>.Enumerator enumerator = explicitInterfaceImplementations.GetEnumerator();
				while (enumerator.MoveNext())
				{
					MethodSymbol current = enumerator.Current;
					ImplementsHelper.ValidateImplementedMethodConstraints(this, current, diagnostics);
				}
			}
		}

		internal static void InitializePartialMethodParts(SourceMemberMethodSymbol definition, SourceMemberMethodSymbol implementation)
		{
			definition.OtherPartOfPartial = implementation;
			if ((object)implementation != null)
			{
				implementation.OtherPartOfPartial = definition;
			}
		}

		internal override BoundBlock GetBoundMethodBody(TypeCompilationState compilationState, BindingDiagnosticBag diagnostics, ref Binder methodBodyBinder = null)
		{
			if (base.IsPartial)
			{
				throw ExceptionUtilities.Unreachable;
			}
			return base.GetBoundMethodBody(compilationState, diagnostics, ref methodBodyBinder);
		}

		private ImmutableArray<HandledEvent> GetHandles(SourceModuleSymbol sourceModule, BindingDiagnosticBag diagBag)
		{
			if (!(base.DeclarationSyntax is MethodStatementSyntax methodStatementSyntax) || methodStatementSyntax.HandlesClause == null)
			{
				return ImmutableArray<HandledEvent>.Empty;
			}
			Binder containingBinder = BinderBuilder.CreateBinderForType(sourceModule, base.SyntaxTree, m_containingType);
			containingBinder = new LocationSpecificBinder(BindingLocation.HandlesClause, this, containingBinder);
			ArrayBuilder<HandledEvent> instance = ArrayBuilder<HandledEvent>.GetInstance();
			SeparatedSyntaxList<HandlesClauseItemSyntax>.Enumerator enumerator = methodStatementSyntax.HandlesClause.Events.GetEnumerator();
			while (enumerator.MoveNext())
			{
				HandlesClauseItemSyntax current = enumerator.Current;
				Binder typeBinder = containingBinder;
				LookupResultKind resultKind = LookupResultKind.Empty;
				HandledEvent handledEvent = BindSingleHandlesClause(current, typeBinder, diagBag, null, null, null, ref resultKind);
				if (handledEvent != null)
				{
					instance.Add(handledEvent);
				}
			}
			return instance.ToImmutableAndFree();
		}

		internal HandledEvent BindSingleHandlesClause(HandlesClauseItemSyntax singleHandleClause, Binder typeBinder, BindingDiagnosticBag diagBag, ArrayBuilder<Symbol> candidateEventSymbols = null, ArrayBuilder<Symbol> candidateWithEventsSymbols = null, ArrayBuilder<Symbol> candidateWithEventsPropertySymbols = null, ref LookupResultKind resultKind = LookupResultKind.Empty)
		{
			TypeSymbol typeSymbol = null;
			PropertySymbol propertySymbol = null;
			PropertySymbol propertySymbol2 = null;
			PropertySymbol propertySymbol3 = null;
			if (TypeSymbolExtensions.IsModuleType(ContainingType) && singleHandleClause.EventContainer.Kind() != SyntaxKind.WithEventsEventContainer)
			{
				Binder.ReportDiagnostic(diagBag, singleHandleClause, ERRID.ERR_HandlesSyntaxInModule);
				return null;
			}
			SyntaxKind syntaxKind = singleHandleClause.EventContainer.Kind();
			CompoundUseSiteInfo<AssemblySymbol> useSiteInfo = typeBinder.GetNewCompoundUseSiteInfo(diagBag);
			HandledEventKind handledEventKind = default(HandledEventKind);
			switch (syntaxKind)
			{
			case SyntaxKind.KeywordEventContainer:
				switch (VisualBasicExtensions.Kind(((KeywordEventContainerSyntax)singleHandleClause.EventContainer).Keyword))
				{
				case SyntaxKind.MeKeyword:
					handledEventKind = HandledEventKind.Me;
					typeSymbol = ContainingType;
					break;
				case SyntaxKind.MyClassKeyword:
					handledEventKind = HandledEventKind.MyClass;
					typeSymbol = ContainingType;
					break;
				case SyntaxKind.MyBaseKeyword:
					handledEventKind = HandledEventKind.MyBase;
					typeSymbol = ContainingType.BaseTypeNoUseSiteDiagnostics;
					break;
				}
				break;
			case SyntaxKind.WithEventsEventContainer:
			case SyntaxKind.WithEventsPropertyEventContainer:
			{
				handledEventKind = HandledEventKind.WithEvents;
				string valueText = ((syntaxKind == SyntaxKind.WithEventsPropertyEventContainer) ? ((WithEventsPropertyEventContainerSyntax)singleHandleClause.EventContainer).WithEventsContainer : ((WithEventsEventContainerSyntax)singleHandleClause.EventContainer)).Identifier.ValueText;
				propertySymbol2 = FindWithEventsProperty(m_containingType, typeBinder, valueText, ref useSiteInfo, candidateWithEventsSymbols, ref resultKind);
				((BindingDiagnosticBag<AssemblySymbol>)diagBag).Add((SyntaxNode)singleHandleClause.EventContainer, useSiteInfo);
				useSiteInfo = new CompoundUseSiteInfo<AssemblySymbol>(useSiteInfo);
				if ((object)propertySymbol2 == null)
				{
					Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_NoWithEventsVarOnHandlesList);
					return null;
				}
				bool flag = !TypeSymbol.Equals(propertySymbol2.ContainingType, ContainingType, TypeCompareKind.ConsiderEverything);
				if (propertySymbol2.IsShared)
				{
					if (!base.IsShared)
					{
						Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_SharedEventNeedsSharedHandler);
					}
					if (flag)
					{
						Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_SharedEventNeedsHandlerInTheSameType);
					}
				}
				if (syntaxKind == SyntaxKind.WithEventsPropertyEventContainer)
				{
					string valueText2 = ((WithEventsPropertyEventContainerSyntax)singleHandleClause.EventContainer).Property.Identifier.ValueText;
					propertySymbol = FindProperty(propertySymbol2.Type, typeBinder, valueText2, ref useSiteInfo, candidateWithEventsPropertySymbols, ref resultKind);
					if ((object)propertySymbol == null)
					{
						Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_HandlesSyntaxInClass);
						return null;
					}
					typeSymbol = propertySymbol.Type;
				}
				else
				{
					typeSymbol = propertySymbol2.Type;
				}
				propertySymbol3 = ((!flag) ? propertySymbol2 : ((SourceNamedTypeSymbol)ContainingType).GetOrAddWithEventsOverride(propertySymbol2));
				typeBinder.ReportDiagnosticsIfObsoleteOrNotSupportedByRuntime(diagBag, propertySymbol3, singleHandleClause.EventContainer);
				break;
			}
			default:
				Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_HandlesSyntaxInClass);
				return null;
			}
			string valueText3 = singleHandleClause.EventMember.Identifier.ValueText;
			EventSymbol eventSymbol = null;
			if ((object)typeSymbol != null)
			{
				Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, typeSymbol);
				eventSymbol = FindEvent(typeSymbol, typeBinder, valueText3, handledEventKind == HandledEventKind.MyBase, ref useSiteInfo, candidateEventSymbols, ref resultKind);
			}
			((BindingDiagnosticBag<AssemblySymbol>)diagBag).Add((SyntaxNode)singleHandleClause.EventMember, useSiteInfo);
			if ((object)eventSymbol == null)
			{
				Binder.ReportDiagnostic(diagBag, singleHandleClause.EventMember, ERRID.ERR_EventNotFound1, valueText3);
				return null;
			}
			typeBinder.ReportDiagnosticsIfObsoleteOrNotSupportedByRuntime(diagBag, eventSymbol, singleHandleClause.EventMember);
			Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, eventSymbol);
			if ((object)eventSymbol.AddMethod != null)
			{
				Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, eventSymbol.AddMethod);
			}
			if ((object)eventSymbol.RemoveMethod != null)
			{
				Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, eventSymbol.RemoveMethod);
			}
			if (eventSymbol.IsWindowsRuntimeEvent)
			{
				typeBinder.GetWellKnownTypeMember(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler, singleHandleClause.EventMember, diagBag);
				typeBinder.GetWellKnownTypeMember(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler, singleHandleClause.EventMember, diagBag);
			}
			switch (ContainingType.TypeKind)
			{
			case TypeKind.Delegate:
			case TypeKind.Enum:
			case TypeKind.Interface:
			case TypeKind.Struct:
				return null;
			default:
				throw ExceptionUtilities.UnexpectedValue(ContainingType.TypeKind);
			case TypeKind.Class:
			case TypeKind.Module:
			{
				BoundExpression receiverOpt = null;
				MethodSymbol methodSymbol = null;
				methodSymbol = ((handledEventKind == HandledEventKind.WithEvents) ? propertySymbol3.SetMethod : ((!eventSymbol.IsShared || !base.IsShared) ? ContainingType.InstanceConstructors[0] : ContainingType.SharedConstructors[0]));
				if (!methodSymbol.IsShared)
				{
					receiverOpt = BoundNodeExtensions.MakeCompilerGenerated(new BoundMeReference(singleHandleClause, ContainingType));
				}
				MethodSymbol item = this;
				if ((object)PartialDefinitionPart != null)
				{
					item = PartialDefinitionPart;
				}
				BoundMethodGroup methodGroup = new BoundMethodGroup(qualificationKind: (!methodSymbol.IsShared) ? QualificationKind.QualifiedViaValue : QualificationKind.QualifiedViaTypeName, syntax: singleHandleClause, typeArgumentsOpt: null, methods: ImmutableArray.Create(item), resultKind: LookupResultKind.Good, receiverOpt: receiverOpt);
				BoundAddressOfOperator addressOfExpression = BoundNodeExtensions.MakeCompilerGenerated(new BoundAddressOfOperator(singleHandleClause, typeBinder, diagBag.AccumulatesDependencies, methodGroup));
				Binder.DelegateResolutionResult delegateResolutionResult = Binder.InterpretDelegateBinding(addressOfExpression, eventSymbol.Type, isForHandles: true);
				if (!Conversions.ConversionExists(delegateResolutionResult.DelegateConversions))
				{
					Binder.ReportDiagnostic(diagBag, singleHandleClause.EventMember, ERRID.ERR_EventHandlerSignatureIncompatible2, Name, valueText3);
					return null;
				}
				ImmutableBindingDiagnostic<AssemblySymbol> diagnostics = delegateResolutionResult.Diagnostics;
				diagBag.AddDependencies(diagnostics.Dependencies);
				BoundExpression delegateCreation = typeBinder.ReclassifyAddressOf(addressOfExpression, ref delegateResolutionResult, eventSymbol.Type, diagBag, isForHandles: true, warnIfResultOfAsyncMethodIsDroppedDueToRelaxation: true);
				return new HandledEvent(handledEventKind, eventSymbol, propertySymbol2, propertySymbol, delegateCreation, methodSymbol);
			}
			}
		}

		internal static PropertySymbol FindWithEventsProperty(TypeSymbol containingType, Binder binder, string name, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ArrayBuilder<Symbol> candidateEventSymbols = null, ref LookupResultKind resultKind = LookupResultKind.Empty)
		{
			LookupResult instance = LookupResult.GetInstance();
			LookupOptions options = LookupOptions.IgnoreExtensionMethods | LookupOptions.UseBaseReferenceAccessibility;
			binder.LookupMember(instance, containingType, name, 0, options, ref useSiteInfo);
			if (candidateEventSymbols != null)
			{
				candidateEventSymbols.AddRange(instance.Symbols);
				resultKind = instance.Kind;
			}
			PropertySymbol result = null;
			if (instance.IsGood)
			{
				if (instance.HasSingleSymbol)
				{
					if (instance.SingleSymbol is PropertySymbol propertySymbol && propertySymbol.IsWithEvents)
					{
						result = propertySymbol;
					}
					else
					{
						resultKind = LookupResultKind.NotAWithEventsMember;
					}
				}
				else
				{
					resultKind = LookupResultKind.Ambiguous;
				}
			}
			instance.Free();
			return result;
		}

		internal static EventSymbol FindEvent(TypeSymbol containingType, Binder binder, string name, bool isThroughMyBase, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ArrayBuilder<Symbol> candidateEventSymbols = null, ref LookupResultKind resultKind = LookupResultKind.Empty)
		{
			LookupOptions lookupOptions = LookupOptions.IgnoreExtensionMethods | LookupOptions.EventsOnly;
			if (isThroughMyBase)
			{
				lookupOptions |= LookupOptions.UseBaseReferenceAccessibility;
			}
			LookupResult instance = LookupResult.GetInstance();
			binder.LookupMember(instance, containingType, name, 0, lookupOptions, ref useSiteInfo);
			if (candidateEventSymbols != null)
			{
				candidateEventSymbols.AddRange(instance.Symbols);
				resultKind = instance.Kind;
			}
			EventSymbol eventSymbol = null;
			if (instance.IsGood)
			{
				if (instance.HasSingleSymbol)
				{
					eventSymbol = instance.SingleSymbol as EventSymbol;
					if ((object)eventSymbol == null)
					{
						resultKind = LookupResultKind.NotAnEvent;
					}
				}
				else
				{
					resultKind = LookupResultKind.Ambiguous;
				}
			}
			instance.Free();
			return eventSymbol;
		}

		private static PropertySymbol FindProperty(TypeSymbol containingType, Binder binder, string name, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, ArrayBuilder<Symbol> candidatePropertySymbols = null, ref LookupResultKind resultKind = LookupResultKind.Empty)
		{
			LookupOptions options = LookupOptions.NoBaseClassLookup | LookupOptions.IgnoreExtensionMethods;
			LookupResult instance = LookupResult.GetInstance();
			binder.LookupMember(instance, containingType, name, 0, options, ref useSiteInfo);
			if (candidatePropertySymbols != null)
			{
				candidatePropertySymbols.AddRange(instance.Symbols);
				resultKind = instance.Kind;
			}
			PropertySymbol propertySymbol = null;
			if (instance.IsGood)
			{
				ArrayBuilder<Symbol>.Enumerator enumerator = instance.Symbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					Symbol current = enumerator.Current;
					if (current.Kind == SymbolKind.Property)
					{
						PropertySymbol propertySymbol2 = (PropertySymbol)current;
						if (!propertySymbol2.Parameters.Any() && (object)propertySymbol2.GetMethod != null && TypeSymbolExtensions.IsClassOrInterfaceType(propertySymbol2.GetMethod.ReturnType) && ReturnsEventSource(propertySymbol2, binder.Compilation))
						{
							propertySymbol = propertySymbol2;
							break;
						}
					}
				}
				if ((object)propertySymbol == null)
				{
					resultKind = LookupResultKind.Empty;
				}
			}
			instance.Free();
			return propertySymbol;
		}

		private static bool ReturnsEventSource(PropertySymbol prop, VisualBasicCompilation compilation)
		{
			ImmutableArray<VisualBasicAttributeData>.Enumerator enumerator = prop.GetAttributes().GetEnumerator();
			while (enumerator.MoveNext())
			{
				VisualBasicAttributeData current = enumerator.Current;
				if ((object)current.AttributeClass != compilation.GetWellKnownType(WellKnownType.System_ComponentModel_DesignerSerializationVisibilityAttribute))
				{
					continue;
				}
				ImmutableArray<TypedConstant> commonConstructorArguments = current.CommonConstructorArguments;
				if (commonConstructorArguments.Length == 1)
				{
					TypedConstant typedConstant = commonConstructorArguments[0];
					if (typedConstant.Kind != TypedConstantKind.Array && Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(typedConstant.ValueInternal) == 2)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
