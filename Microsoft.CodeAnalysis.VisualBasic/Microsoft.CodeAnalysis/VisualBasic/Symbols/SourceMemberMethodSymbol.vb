Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Linq
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class SourceMemberMethodSymbol
		Inherits SourceNonPropertyAccessorMethodSymbol
		Private ReadOnly _name As String

		Private ReadOnly _arity As Integer

		Private ReadOnly _quickAttributes As QuickAttributes

		Private _lazyMetadataName As String

		Private _lazyImplementedMethods As ImmutableArray(Of MethodSymbol)

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyHandles As ImmutableArray(Of HandledEvent)

		Private _otherPartOfPartial As SourceMemberMethodSymbol

		Private ReadOnly _asyncStateMachineType As NamedTypeSymbol

		Private _lazyState As Integer

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._arity
			End Get
		End Property

		Protected Overrides ReadOnly Property BoundAttributesSource As SourceMethodSymbol
			Get
				Return Me.SourcePartialDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of MethodSymbol)
			Get
				Dim methodSymbols As ImmutableArray(Of MethodSymbol)
				Dim empty As ImmutableArray(Of MethodSymbol)
				If (Me._lazyImplementedMethods.IsDefault) Then
					Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					If (Not MyBase.IsPartial) Then
						methodSymbols = Me.GetExplicitInterfaceImplementations(containingModule, instance)
					Else
						Dim declarationSyntax As MethodStatementSyntax = TryCast(MyBase.DeclarationSyntax, MethodStatementSyntax)
						If (declarationSyntax IsNot Nothing AndAlso declarationSyntax.ImplementsClause IsNot Nothing) Then
							instance.Add(ERRID.ERR_PartialDeclarationImplements1, declarationSyntax.Identifier.GetLocation(), New [Object]() { declarationSyntax.Identifier.ToString() })
						End If
						Dim partialImplementationPart As MethodSymbol = Me.PartialImplementationPart
						If (partialImplementationPart Is Nothing) Then
							empty = ImmutableArray(Of MethodSymbol).Empty
						Else
							empty = partialImplementationPart.ExplicitInterfaceImplementations
						End If
						methodSymbols = empty
					End If
					containingModule.AtomicStoreArrayAndDiagnostics(Of MethodSymbol)(Me._lazyImplementedMethods, methodSymbols, instance)
					instance.Free()
				End If
				Return Me._lazyImplementedMethods
			End Get
		End Property

		Friend Overrides ReadOnly Property GenerateDebugInfoImpl As Boolean
			Get
				If (Not MyBase.GenerateDebugInfoImpl) Then
					Return False
				End If
				Return Not MyBase.IsAsync
			End Get
		End Property

		Public Overrides ReadOnly Property HandledEvents As ImmutableArray(Of HandledEvent)
			Get
				If (Me._lazyHandles.IsDefault) Then
					Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim [handles] As ImmutableArray(Of HandledEvent) = Me.GetHandles(containingModule, instance)
					containingModule.AtomicStoreArrayAndDiagnostics(Of HandledEvent)(Me._lazyHandles, [handles], instance)
					instance.Free()
				End If
				Return Me._lazyHandles
			End Get
		End Property

		Friend Overrides ReadOnly Property HasEmptyBody As Boolean
			Get
				Dim flag As Boolean
				If (MyBase.HasEmptyBody) Then
					Dim sourcePartialImplementation As SourceMemberMethodSymbol = Me.SourcePartialImplementation
					flag = If(sourcePartialImplementation Is Nothing, True, sourcePartialImplementation.HasEmptyBody)
				Else
					flag = False
				End If
				Return flag
			End Get
		End Property

		Public Overrides ReadOnly Property IsExtensionMethod As Boolean
			Get
				Return If(Not Me.MayBeReducibleExtensionMethod, False, MyBase.IsExtensionMethod)
			End Get
		End Property

		Friend ReadOnly Property IsPartialDefinition As Boolean
			Get
				Return MyBase.IsPartial
			End Get
		End Property

		Friend ReadOnly Property IsPartialImplementation As Boolean
			Get
				If (Me.IsPartialDefinition) Then
					Return False
				End If
				Return CObj(Me.OtherPartOfPartial) <> CObj(Nothing)
			End Get
		End Property

		Friend Overrides ReadOnly Property MayBeReducibleExtensionMethod As Boolean
			Get
				Return (Me.GetQuickAttributes() And QuickAttributes.Extension) <> QuickAttributes.None
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				If (Me._lazyMetadataName Is Nothing) Then
					If (Me.MethodKind <> Microsoft.CodeAnalysis.MethodKind.Ordinary) Then
						Me.SetMetadataName(Me._name)
					Else
						OverloadingHelper.SetMetadataNameForAllOverloads(Me._name, SymbolKind.Method, Me.m_containingType)
					End If
				End If
				Return Me._lazyMetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				' 
				' Current member / type: Microsoft.CodeAnalysis.ObsoleteAttributeData Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol::get_ObsoleteAttributeData()
				' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
				' 
				' Product version: 2019.1.118.0
				' Exception in: Microsoft.CodeAnalysis.ObsoleteAttributeData get_ObsoleteAttributeData()
				' 
				' La référence d'objet n'est pas définie à une instance d'un objet.
				'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
				'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
				'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
				'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
				'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
				'    à ..(MethodBody , & ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\PropertyDecompiler.cs:ligne 345
				' 
				' mailto: JustDecompilePublicFeedback@telerik.com

			End Get
		End Property

		Friend Property OtherPartOfPartial As SourceMemberMethodSymbol
			Get
				Return Me._otherPartOfPartial
			End Get
			Private Set(ByVal value As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol)
				Dim sourceMemberMethodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol = Me._otherPartOfPartial
				Me._otherPartOfPartial = value
			End Set
		End Property

		Public Overrides ReadOnly Property PartialDefinitionPart As MethodSymbol
			Get
				Return Me.SourcePartialDefinition
			End Get
		End Property

		Public Overrides ReadOnly Property PartialImplementationPart As MethodSymbol
			Get
				Return Me.SourcePartialImplementation
			End Get
		End Property

		Public ReadOnly Property SourcePartialDefinition As SourceMemberMethodSymbol
			Get
				If (Me.IsPartialDefinition) Then
					Return Nothing
				End If
				Return Me.OtherPartOfPartial
			End Get
		End Property

		Public ReadOnly Property SourcePartialImplementation As SourceMemberMethodSymbol
			Get
				If (Not Me.IsPartialDefinition) Then
					Return Nothing
				End If
				Return Me.OtherPartOfPartial
			End Get
		End Property

		Friend Property SuppressDuplicateProcDefDiagnostics As Boolean
			Get
				Return (Me._lazyState And 1) <> 0
			End Get
			Set(ByVal value As Boolean)
				ThreadSafeFlagOperations.[Set](Me._lazyState, 1)
			End Set
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = Me._lazyTypeParameters
				If (typeParameterSymbols.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim containingModule As SourceModuleSymbol = DirectCast(Me.ContainingModule, SourceModuleSymbol)
					typeParameterSymbols = Me.GetTypeParameters(containingModule, instance)
					containingModule.AtomicStoreArrayAndDiagnostics(Of TypeParameterSymbol)(Me._lazyTypeParameters, typeParameterSymbols, instance)
					instance.Free()
					typeParameterSymbols = Me._lazyTypeParameters
				End If
				Return typeParameterSymbols
			End Get
		End Property

		Friend Sub New(ByVal containingType As SourceMemberContainerTypeSymbol, ByVal name As String, ByVal flags As SourceMemberFlags, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As MethodBaseSyntax, ByVal arity As Integer, Optional ByVal handledEvents As ImmutableArray(Of HandledEvent) = Nothing)
			MyBase.New(containingType, flags, binder.GetSyntaxReference(syntax), New ImmutableArray(Of Location)())
			Me._asyncStateMachineType = Nothing
			Me._lazyHandles = handledEvents
			Me._name = name
			Me._arity = arity
			Me._quickAttributes = binder.QuickAttributeChecker.CheckAttributes(syntax.AttributeLists)
			If (Not containingType.AllowsExtensionMethods()) Then
				Me._quickAttributes = Me._quickAttributes And (QuickAttributes.Obsolete Or QuickAttributes.MyGroupCollection Or QuickAttributes.TypeIdentifier)
			End If
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (MyBase.IsAsync OrElse MyBase.IsIterator) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeStateMachineAttribute(Me, compilationState))
				If (MyBase.IsAsync) Then
					Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeOptionalDebuggerStepThroughAttribute())
				End If
			End If
		End Sub

		Friend Function BindSingleHandlesClause(ByVal singleHandleClause As HandlesClauseItemSyntax, ByVal typeBinder As Binder, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByVal candidateEventSymbols As ArrayBuilder(Of Symbol) = Nothing, Optional ByVal candidateWithEventsSymbols As ArrayBuilder(Of Symbol) = Nothing, Optional ByVal candidateWithEventsPropertySymbols As ArrayBuilder(Of Symbol) = Nothing, Optional ByRef resultKind As LookupResultKind = 0) As Microsoft.CodeAnalysis.VisualBasic.HandledEvent
			Dim handledEvent As Microsoft.CodeAnalysis.VisualBasic.HandledEvent
			Dim handledEventKind As Microsoft.CodeAnalysis.VisualBasic.HandledEventKind = 0
			Dim qualificationKind As Microsoft.CodeAnalysis.VisualBasic.QualificationKind
			Dim identifier As SyntaxToken
			Dim containingType As TypeSymbol = Nothing
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			Dim orAddWithEventsOverride As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			If (Not Me.ContainingType.IsModuleType() OrElse singleHandleClause.EventContainer.Kind() = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer) Then
				Dim syntaxKind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = singleHandleClause.EventContainer.Kind()
				Dim newCompoundUseSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol) = typeBinder.GetNewCompoundUseSiteInfo(diagBag)
				If (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.KeywordEventContainer) Then
					Dim syntaxKind1 As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind = DirectCast(singleHandleClause.EventContainer, KeywordEventContainerSyntax).Keyword.Kind()
					If (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MeKeyword) Then
						handledEventKind = Microsoft.CodeAnalysis.VisualBasic.HandledEventKind.[Me]
						containingType = Me.ContainingType
					ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyBaseKeyword) Then
						handledEventKind = Microsoft.CodeAnalysis.VisualBasic.HandledEventKind.[MyBase]
						containingType = Me.ContainingType.BaseTypeNoUseSiteDiagnostics
					ElseIf (syntaxKind1 = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.MyClassKeyword) Then
						handledEventKind = Microsoft.CodeAnalysis.VisualBasic.HandledEventKind.[MyClass]
						containingType = Me.ContainingType
					End If
				ElseIf (syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsEventContainer OrElse syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer) Then
					handledEventKind = Microsoft.CodeAnalysis.VisualBasic.HandledEventKind.[WithEvents]
					Dim valueText As String = If(syntaxKind = Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer, DirectCast(singleHandleClause.EventContainer, WithEventsPropertyEventContainerSyntax).WithEventsContainer, DirectCast(singleHandleClause.EventContainer, WithEventsEventContainerSyntax)).Identifier.ValueText
					propertySymbol1 = SourceMemberMethodSymbol.FindWithEventsProperty(Me.m_containingType, typeBinder, valueText, newCompoundUseSiteInfo, candidateWithEventsSymbols, resultKind)
					diagBag.Add(singleHandleClause.EventContainer, newCompoundUseSiteInfo)
					newCompoundUseSiteInfo = New CompoundUseSiteInfo(Of AssemblySymbol)(newCompoundUseSiteInfo)
					If (propertySymbol1 IsNot Nothing) Then
						Dim flag As Boolean = Not TypeSymbol.Equals(propertySymbol1.ContainingType, Me.ContainingType, TypeCompareKind.ConsiderEverything)
						If (propertySymbol1.IsShared) Then
							If (Not MyBase.IsShared) Then
								Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_SharedEventNeedsSharedHandler)
							End If
							If (flag) Then
								Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_SharedEventNeedsHandlerInTheSameType)
							End If
						End If
						If (syntaxKind <> Microsoft.CodeAnalysis.VisualBasic.SyntaxKind.WithEventsPropertyEventContainer) Then
							containingType = propertySymbol1.Type
						Else
							identifier = DirectCast(singleHandleClause.EventContainer, WithEventsPropertyEventContainerSyntax).[Property].Identifier
							Dim str As String = identifier.ValueText
							propertySymbol = SourceMemberMethodSymbol.FindProperty(propertySymbol1.Type, typeBinder, str, newCompoundUseSiteInfo, candidateWithEventsPropertySymbols, resultKind)
							If (propertySymbol Is Nothing) Then
								Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_HandlesSyntaxInClass)
								handledEvent = Nothing
								Return handledEvent
							End If
							containingType = propertySymbol.Type
						End If
						If (Not flag) Then
							orAddWithEventsOverride = propertySymbol1
						Else
							orAddWithEventsOverride = DirectCast(Me.ContainingType, SourceNamedTypeSymbol).GetOrAddWithEventsOverride(propertySymbol1)
						End If
						typeBinder.ReportDiagnosticsIfObsoleteOrNotSupportedByRuntime(diagBag, orAddWithEventsOverride, singleHandleClause.EventContainer)
					Else
						Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_NoWithEventsVarOnHandlesList)
						handledEvent = Nothing
						Return handledEvent
					End If
				Else
					Binder.ReportDiagnostic(diagBag, singleHandleClause.EventContainer, ERRID.ERR_HandlesSyntaxInClass)
					handledEvent = Nothing
					Return handledEvent
				End If
				identifier = singleHandleClause.EventMember.Identifier
				Dim valueText1 As String = identifier.ValueText
				Dim eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol = Nothing
				If (containingType IsNot Nothing) Then
					Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, containingType)
					eventSymbol = SourceMemberMethodSymbol.FindEvent(containingType, typeBinder, valueText1, handledEventKind = Microsoft.CodeAnalysis.VisualBasic.HandledEventKind.[MyBase], newCompoundUseSiteInfo, candidateEventSymbols, resultKind)
				End If
				diagBag.Add(singleHandleClause.EventMember, newCompoundUseSiteInfo)
				If (eventSymbol IsNot Nothing) Then
					typeBinder.ReportDiagnosticsIfObsoleteOrNotSupportedByRuntime(diagBag, eventSymbol, singleHandleClause.EventMember)
					Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, eventSymbol)
					If (eventSymbol.AddMethod IsNot Nothing) Then
						Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, eventSymbol.AddMethod)
					End If
					If (eventSymbol.RemoveMethod IsNot Nothing) Then
						Binder.ReportUseSite(diagBag, singleHandleClause.EventMember, eventSymbol.RemoveMethod)
					End If
					If (eventSymbol.IsWindowsRuntimeEvent) Then
						typeBinder.GetWellKnownTypeMember(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__AddEventHandler, singleHandleClause.EventMember, diagBag)
						typeBinder.GetWellKnownTypeMember(WellKnownMember.System_Runtime_InteropServices_WindowsRuntime_EventRegistrationTokenTable_T__RemoveEventHandler, singleHandleClause.EventMember, diagBag)
					End If
					Select Case Me.ContainingType.TypeKind
						Case TypeKind.[Class]
						Case TypeKind.[Module]
							Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = Nothing
							Dim setMethod As MethodSymbol = Nothing
							If (handledEventKind <> Microsoft.CodeAnalysis.VisualBasic.HandledEventKind.[WithEvents]) Then
								setMethod = If(Not eventSymbol.IsShared OrElse Not MyBase.IsShared, Me.ContainingType.InstanceConstructors(0), Me.ContainingType.SharedConstructors(0))
							Else
								setMethod = orAddWithEventsOverride.SetMethod
							End If
							If (Not setMethod.IsShared) Then
								boundExpression = (New BoundMeReference(singleHandleClause, Me.ContainingType)).MakeCompilerGenerated()
							End If
							Dim partialDefinitionPart As MethodSymbol = Me
							If (Me.PartialDefinitionPart IsNot Nothing) Then
								partialDefinitionPart = Me.PartialDefinitionPart
							End If
							qualificationKind = If(Not setMethod.IsShared, Microsoft.CodeAnalysis.VisualBasic.QualificationKind.QualifiedViaValue, Microsoft.CodeAnalysis.VisualBasic.QualificationKind.QualifiedViaTypeName)
							Dim boundMethodGroup As Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup = New Microsoft.CodeAnalysis.VisualBasic.BoundMethodGroup(singleHandleClause, Nothing, ImmutableArray.Create(Of MethodSymbol)(partialDefinitionPart), LookupResultKind.Good, boundExpression, qualificationKind, False)
							Dim boundAddressOfOperator As Microsoft.CodeAnalysis.VisualBasic.BoundAddressOfOperator = (New Microsoft.CodeAnalysis.VisualBasic.BoundAddressOfOperator(singleHandleClause, typeBinder, diagBag.AccumulatesDependencies, boundMethodGroup, False)).MakeCompilerGenerated()
							Dim delegateResolutionResult As Binder.DelegateResolutionResult = Binder.InterpretDelegateBinding(boundAddressOfOperator, eventSymbol.Type, True)
							If (Microsoft.CodeAnalysis.VisualBasic.Conversions.ConversionExists(delegateResolutionResult.DelegateConversions)) Then
								diagBag.AddDependencies(delegateResolutionResult.Diagnostics.Dependencies)
								Dim boundExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundExpression = typeBinder.ReclassifyAddressOf(boundAddressOfOperator, delegateResolutionResult, eventSymbol.Type, diagBag, True, True)
								handledEvent = New Microsoft.CodeAnalysis.VisualBasic.HandledEvent(handledEventKind, eventSymbol, propertySymbol1, propertySymbol, boundExpression1, setMethod)
								Exit Select
							Else
								Binder.ReportDiagnostic(diagBag, singleHandleClause.EventMember, ERRID.ERR_EventHandlerSignatureIncompatible2, New [Object]() { Me.Name, valueText1 })
								handledEvent = Nothing
								Exit Select
							End If
						Case TypeKind.[Delegate]
						Case TypeKind.[Enum]
						Case TypeKind.[Interface]
						Case TypeKind.Struct
							handledEvent = Nothing
							Exit Select
						Case TypeKind.Dynamic
						Case TypeKind.[Error]
						Case TypeKind.Pointer
							Throw ExceptionUtilities.UnexpectedValue(Me.ContainingType.TypeKind)
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(Me.ContainingType.TypeKind)
					End Select
				Else
					Binder.ReportDiagnostic(diagBag, singleHandleClause.EventMember, ERRID.ERR_EventNotFound1, New [Object]() { valueText1 })
					handledEvent = Nothing
				End If
			Else
				Binder.ReportDiagnostic(diagBag, singleHandleClause, ERRID.ERR_HandlesSyntaxInModule)
				handledEvent = Nothing
			End If
			Return handledEvent
		End Function

		Friend Shared Function FindEvent(ByVal containingType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal name As String, ByVal isThroughMyBase As Boolean, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal candidateEventSymbols As ArrayBuilder(Of Symbol) = Nothing, Optional ByRef resultKind As LookupResultKind = 0) As EventSymbol
			Dim lookupOption As LookupOptions = LookupOptions.IgnoreExtensionMethods Or LookupOptions.EventsOnly
			If (isThroughMyBase) Then
				lookupOption = lookupOption Or LookupOptions.UseBaseReferenceAccessibility
			End If
			Dim instance As LookupResult = LookupResult.GetInstance()
			binder.LookupMember(instance, containingType, name, 0, lookupOption, useSiteInfo)
			If (candidateEventSymbols IsNot Nothing) Then
				candidateEventSymbols.AddRange(instance.Symbols)
				resultKind = instance.Kind
			End If
			Dim singleSymbol As EventSymbol = Nothing
			If (instance.IsGood) Then
				If (Not instance.HasSingleSymbol) Then
					resultKind = LookupResultKind.Ambiguous
				Else
					singleSymbol = TryCast(instance.SingleSymbol, EventSymbol)
					If (singleSymbol Is Nothing) Then
						resultKind = LookupResultKind.NotAnEvent
					End If
				End If
			End If
			instance.Free()
			Return singleSymbol
		End Function

		Private Shared Function FindProperty(ByVal containingType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal name As String, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal candidatePropertySymbols As ArrayBuilder(Of Symbol) = Nothing, Optional ByRef resultKind As LookupResultKind = 0) As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Dim lookupOption As LookupOptions = LookupOptions.NoBaseClassLookup Or LookupOptions.IgnoreExtensionMethods
			Dim instance As LookupResult = LookupResult.GetInstance()
			binder.LookupMember(instance, containingType, name, 0, lookupOption, useSiteInfo)
			If (candidatePropertySymbols IsNot Nothing) Then
				candidatePropertySymbols.AddRange(instance.Symbols)
				resultKind = instance.Kind
			End If
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			If (instance.IsGood) Then
				Dim enumerator As ArrayBuilder(Of Symbol).Enumerator = instance.Symbols.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Symbol = enumerator.Current
					If (current.Kind <> SymbolKind.[Property]) Then
						Continue While
					End If
					Dim propertySymbol1 As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = DirectCast(current, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					If (System.Linq.ImmutableArrayExtensions.Any(Of ParameterSymbol)(propertySymbol1.Parameters) OrElse propertySymbol1.GetMethod Is Nothing OrElse Not propertySymbol1.GetMethod.ReturnType.IsClassOrInterfaceType() OrElse Not SourceMemberMethodSymbol.ReturnsEventSource(propertySymbol1, binder.Compilation)) Then
						Continue While
					End If
					propertySymbol = propertySymbol1
					Exit While
				End While
				If (propertySymbol Is Nothing) Then
					resultKind = LookupResultKind.Empty
				End If
			End If
			instance.Free()
			Return propertySymbol
		End Function

		Friend Shared Function FindWithEventsProperty(ByVal containingType As TypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal name As String, <InAttribute> <Out> ByRef useSiteInfo As CompoundUseSiteInfo(Of AssemblySymbol), Optional ByVal candidateEventSymbols As ArrayBuilder(Of Symbol) = Nothing, Optional ByRef resultKind As LookupResultKind = 0) As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol
			Dim instance As LookupResult = LookupResult.GetInstance()
			binder.LookupMember(instance, containingType, name, 0, LookupOptions.IgnoreExtensionMethods Or LookupOptions.UseBaseReferenceAccessibility, useSiteInfo)
			If (candidateEventSymbols IsNot Nothing) Then
				candidateEventSymbols.AddRange(instance.Symbols)
				resultKind = instance.Kind
			End If
			Dim propertySymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = Nothing
			If (instance.IsGood) Then
				If (Not instance.HasSingleSymbol) Then
					resultKind = LookupResultKind.Ambiguous
				Else
					Dim singleSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol = TryCast(instance.SingleSymbol, Microsoft.CodeAnalysis.VisualBasic.Symbols.PropertySymbol)
					If (singleSymbol Is Nothing OrElse Not singleSymbol.IsWithEvents) Then
						resultKind = LookupResultKind.NotAWithEventsMember
					Else
						propertySymbol = singleSymbol
					End If
				End If
			End If
			instance.Free()
			Return propertySymbol
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			' 
			' Current member / type: System.Void Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceMemberMethodSymbol::GenerateDeclarationErrors(System.Threading.CancellationToken)
			' File path: C:\Code\Libs\Compilateurs\Work\Compilateur.NET\Microsoft.CodeAnalysis.VisualBasic.dll
			' 
			' Product version: 2019.1.118.0
			' Exception in: System.Void GenerateDeclarationErrors(System.Threading.CancellationToken)
			' 
			' La référence d'objet n'est pas définie à une instance d'un objet.
			'    à ..(Expression , Instruction ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 291
			'    à ..(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:ligne 48
			'    à Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:ligne 91
			'    à ..(MethodBody ,  , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 88
			'    à ..(MethodBody , ILanguage ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:ligne 70
			'    à Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 95
			'    à Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:ligne 58
			'    à ..(ILanguage , MethodDefinition ,  ) dans C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:ligne 117
			' 
			' mailto: JustDecompilePublicFeedback@telerik.com

		End Sub

		Protected Overrides Function GetAttributeDeclarations() As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			Dim oneOrMany As OneOrMany(Of SyntaxList(Of AttributeListSyntax))
			oneOrMany = If(Me.SourcePartialImplementation Is Nothing, Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(MyBase.AttributeDeclarationSyntaxList), Roslyn.Utilities.OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(ImmutableArray.Create(Of SyntaxList(Of AttributeListSyntax))(MyBase.AttributeDeclarationSyntaxList, Me.SourcePartialImplementation.AttributeDeclarationSyntaxList)))
			Return oneOrMany
		End Function

		Friend Overrides Function GetBoundMethodBody(ByVal compilationState As TypeCompilationState, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, Optional ByRef methodBodyBinder As Binder = Nothing) As BoundBlock
			If (MyBase.IsPartial) Then
				Throw ExceptionUtilities.Unreachable
			End If
			Return MyBase.GetBoundMethodBody(compilationState, diagnostics, methodBodyBinder)
		End Function

		Private Function GetExplicitInterfaceImplementations(ByVal sourceModule As SourceModuleSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of MethodSymbol)
			Dim empty As ImmutableArray(Of MethodSymbol)
			Dim declarationSyntax As MethodStatementSyntax = TryCast(MyBase.DeclarationSyntax, MethodStatementSyntax)
			If (declarationSyntax IsNot Nothing AndAlso declarationSyntax.ImplementsClause IsNot Nothing) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(sourceModule, MyBase.SyntaxTree, Me.ContainingType)
				If (MyBase.IsShared And Not Me.ContainingType.IsModuleType()) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagBag, declarationSyntax.Modifiers.First(SyntaxKind.SharedKeyword), ERRID.ERR_SharedOnProcThatImpl, New [Object]() { declarationSyntax.Identifier.ToString() })
					empty = ImmutableArray(Of MethodSymbol).Empty
					Return empty
				End If
				empty = ImplementsHelper.ProcessImplementsClause(Of MethodSymbol)(declarationSyntax.ImplementsClause, Me, DirectCast(Me.ContainingType, SourceMemberContainerTypeSymbol), binder, diagBag)
				Return empty
			End If
			empty = ImmutableArray(Of MethodSymbol).Empty
			Return empty
		End Function

		Private Function GetHandles(ByVal sourceModule As SourceModuleSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent)
			Dim empty As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent)
			Dim declarationSyntax As MethodStatementSyntax = TryCast(MyBase.DeclarationSyntax, MethodStatementSyntax)
			If (declarationSyntax Is Nothing OrElse declarationSyntax.HandlesClause Is Nothing) Then
				empty = ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent).Empty
			Else
				Dim locationSpecificBinder As Binder = BinderBuilder.CreateBinderForType(sourceModule, MyBase.SyntaxTree, Me.m_containingType)
				locationSpecificBinder = New Microsoft.CodeAnalysis.VisualBasic.LocationSpecificBinder(BindingLocation.HandlesClause, Me, locationSpecificBinder)
				Dim instance As ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent) = ArrayBuilder(Of Microsoft.CodeAnalysis.VisualBasic.HandledEvent).GetInstance()
				Dim enumerator As SeparatedSyntaxList(Of HandlesClauseItemSyntax).Enumerator = declarationSyntax.HandlesClause.Events.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As HandlesClauseItemSyntax = enumerator.Current
					Dim lookupResultKind As Microsoft.CodeAnalysis.VisualBasic.LookupResultKind = Microsoft.CodeAnalysis.VisualBasic.LookupResultKind.Empty
					Dim handledEvent As Microsoft.CodeAnalysis.VisualBasic.HandledEvent = Me.BindSingleHandlesClause(current, locationSpecificBinder, diagBag, Nothing, Nothing, Nothing, lookupResultKind)
					If (handledEvent Is Nothing) Then
						Continue While
					End If
					instance.Add(handledEvent)
				End While
				empty = instance.ToImmutableAndFree()
			End If
			Return empty
		End Function

		Private Function GetQuickAttributes() As QuickAttributes
			Dim quickAttribute As QuickAttributes
			Dim quickAttribute1 As QuickAttributes = Me._quickAttributes
			If (MyBase.IsPartial) Then
				Dim otherPartOfPartial As SourceMemberMethodSymbol = Me.OtherPartOfPartial
				If (otherPartOfPartial Is Nothing) Then
					quickAttribute = quickAttribute1
					Return quickAttribute
				End If
				quickAttribute = quickAttribute1 Or otherPartOfPartial._quickAttributes
				Return quickAttribute
			End If
			quickAttribute = quickAttribute1
			Return quickAttribute
		End Function

		Private Function GetTypeParameters(ByVal sourceModule As SourceModuleSymbol, ByVal diagBag As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of TypeParameterSymbol)
			Dim empty As ImmutableArray(Of TypeParameterSymbol)
			Dim typeParameterListSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TypeParameterListSyntax = SourceMethodSymbol.GetTypeParameterListSyntax(MyBase.DeclarationSyntax)
			If (typeParameterListSyntax IsNot Nothing) Then
				Dim methodTypeParametersBinder As Binder = BinderBuilder.CreateBinderForType(sourceModule, MyBase.SyntaxTree, Me.m_containingType)
				Dim parameters As SeparatedSyntaxList(Of TypeParameterSyntax) = typeParameterListSyntax.Parameters
				Dim count As Integer = parameters.Count
				Dim sourceTypeParameterOnMethodSymbol(count - 1 + 1 - 1) As TypeParameterSymbol
				Dim num As Integer = count - 1
				Dim num1 As Integer = 0
				Do
					Dim item As TypeParameterSyntax = parameters(num1)
					Dim identifier As SyntaxToken = item.Identifier
					Binder.DisallowTypeCharacter(identifier, diagBag, ERRID.ERR_TypeCharOnGenericParam)
					sourceTypeParameterOnMethodSymbol(num1) = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SourceTypeParameterOnMethodSymbol(Me, num1, identifier.ValueText, methodTypeParametersBinder.GetSyntaxReference(item))
					If (MyBase.DeclarationSyntax.Kind() = SyntaxKind.FunctionStatement AndAlso CaseInsensitiveComparison.Equals(Me.Name, identifier.ValueText)) Then
						Binder.ReportDiagnostic(diagBag, item, ERRID.ERR_TypeParamNameFunctionNameCollision)
					End If
					num1 = num1 + 1
				Loop While num1 <= num
				methodTypeParametersBinder = New Microsoft.CodeAnalysis.VisualBasic.MethodTypeParametersBinder(methodTypeParametersBinder, Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(sourceTypeParameterOnMethodSymbol))
				Dim containingType As SourceNamedTypeSymbol = TryCast(Me.ContainingType, SourceNamedTypeSymbol)
				If (containingType IsNot Nothing) Then
					containingType.CheckForDuplicateTypeParameters(Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(sourceTypeParameterOnMethodSymbol), diagBag)
				End If
				empty = Microsoft.CodeAnalysis.ImmutableArrayExtensions.AsImmutableOrNull(Of TypeParameterSymbol)(sourceTypeParameterOnMethodSymbol)
			Else
				empty = ImmutableArray(Of TypeParameterSymbol).Empty
			End If
			Return empty
		End Function

		Friend Function HasExplicitInterfaceImplementations() As Boolean
			Dim declarationSyntax As MethodStatementSyntax = TryCast(MyBase.DeclarationSyntax, MethodStatementSyntax)
			If (declarationSyntax Is Nothing) Then
				Return False
			End If
			Return declarationSyntax.ImplementsClause IsNot Nothing
		End Function

		Friend Shared Sub InitializePartialMethodParts(ByVal definition As SourceMemberMethodSymbol, ByVal implementation As SourceMemberMethodSymbol)
			definition.OtherPartOfPartial = implementation
			If (implementation IsNot Nothing) Then
				implementation.OtherPartOfPartial = definition
			End If
		End Sub

		Private Shared Function ReturnsEventSource(ByVal prop As PropertySymbol, ByVal compilation As VisualBasicCompilation) As Boolean
			Dim flag As Boolean
			Dim enumerator As ImmutableArray(Of VisualBasicAttributeData).Enumerator = prop.GetAttributes().GetEnumerator()
			While True
				If (enumerator.MoveNext()) Then
					Dim current As VisualBasicAttributeData = enumerator.Current
					If (CObj(current.AttributeClass) = CObj(compilation.GetWellKnownType(WellKnownType.System_ComponentModel_DesignerSerializationVisibilityAttribute))) Then
						Dim commonConstructorArguments As ImmutableArray(Of TypedConstant) = current.CommonConstructorArguments
						If (commonConstructorArguments.Length = 1) Then
							Dim item As TypedConstant = commonConstructorArguments(0)
							If (item.Kind <> TypedConstantKind.Array AndAlso Microsoft.VisualBasic.CompilerServices.Conversions.ToInteger(item.ValueInternal) = 2) Then
								flag = True
								Exit While
							End If
						End If
					End If
				Else
					flag = False
					Exit While
				End If
			End While
			Return flag
		End Function

		Friend Overrides Sub SetMetadataName(ByVal metadataName As String)
			Interlocked.CompareExchange(Of String)(Me._lazyMetadataName, metadataName, Nothing)
			If (MyBase.IsPartial) Then
				Dim otherPartOfPartial As SourceMemberMethodSymbol = Me.OtherPartOfPartial
				If (otherPartOfPartial IsNot Nothing) Then
					otherPartOfPartial.SetMetadataName(metadataName)
				End If
			End If
		End Sub

		Friend Sub ValidateImplementedMethodConstraints(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
			If (MyBase.IsPartial AndAlso Me.OtherPartOfPartial IsNot Nothing) Then
				Me.OtherPartOfPartial.ValidateImplementedMethodConstraints(diagnostics)
				Return
			End If
			Dim explicitInterfaceImplementations As ImmutableArray(Of MethodSymbol) = Me.ExplicitInterfaceImplementations
			If (Not explicitInterfaceImplementations.IsEmpty) Then
				Dim enumerator As ImmutableArray(Of MethodSymbol).Enumerator = explicitInterfaceImplementations.GetEnumerator()
				While enumerator.MoveNext()
					ImplementsHelper.ValidateImplementedMethodConstraints(Me, enumerator.Current, diagnostics)
				End While
			End If
		End Sub

		<Flags>
		Private Enum StateFlags
			SuppressDuplicateProcDefDiagnostics = 1
			AllDiagnosticsReported = 2
		End Enum
	End Class
End Namespace