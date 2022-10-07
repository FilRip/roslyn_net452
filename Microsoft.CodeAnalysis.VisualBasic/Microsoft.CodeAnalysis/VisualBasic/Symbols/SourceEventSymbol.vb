Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class SourceEventSymbol
		Inherits EventSymbol
		Implements IAttributeTargetSymbol
		Private ReadOnly _containingType As SourceMemberContainerTypeSymbol

		Private ReadOnly _name As String

		Private ReadOnly _syntaxRef As Microsoft.CodeAnalysis.SyntaxReference

		Private ReadOnly _location As Location

		Private ReadOnly _memberFlags As SourceMemberFlags

		Private ReadOnly _addMethod As MethodSymbol

		Private ReadOnly _removeMethod As MethodSymbol

		Private ReadOnly _raiseMethod As MethodSymbol

		Private ReadOnly _backingField As FieldSymbol

		Private _lazyState As Integer

		Private _lazyType As TypeSymbol

		Private _lazyImplementedEvents As ImmutableArray(Of EventSymbol)

		Private _lazyDelegateParameters As ImmutableArray(Of ParameterSymbol)

		Private _lazyDocComment As String

		Private _lazyExpandedDocComment As String

		Private _lazyCustomAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData)

		Public Overrides ReadOnly Property AddMethod As MethodSymbol
			Get
				Return Me._addMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property AssociatedField As FieldSymbol
			Get
				Return Me._backingField
			End Get
		End Property

		Friend ReadOnly Property AttributeDeclarationSyntaxList As SyntaxList(Of AttributeListSyntax)
			Get
				Return DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), EventStatementSyntax).AttributeLists
			End Get
		End Property

		Public ReadOnly Property ContainingSourceModule As SourceModuleSymbol
			Get
				Return Me._containingType.ContainingSourceModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingType As NamedTypeSymbol
			Get
				Return Me._containingType
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return DirectCast((Me._memberFlags And SourceMemberFlags.AccessibilityMask), Accessibility)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of Microsoft.CodeAnalysis.SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Me._syntaxRef)
			End Get
		End Property

		Public ReadOnly Property DefaultAttributeLocation As AttributeLocation Implements IAttributeTargetSymbol.DefaultAttributeLocation
			Get
				Return AttributeLocation.[Event]
			End Get
		End Property

		Friend Overrides ReadOnly Property DelegateParameters As ImmutableArray(Of ParameterSymbol)
			Get
				If (Me._lazyDelegateParameters.IsDefault) Then
					Dim syntax As EventStatementSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), EventStatementSyntax)
					If (syntax.AsClause Is Nothing) Then
						Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
						Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
						Me.ContainingSourceModule.AtomicStoreArrayAndDiagnostics(Of ParameterSymbol)(Me._lazyDelegateParameters, binder.DecodeParameterListOfDelegateDeclaration(Me, syntax.ParameterList, instance), instance)
						instance.Free()
					Else
						Me._lazyDelegateParameters = MyBase.DelegateParameters
					End If
				End If
				Return Me._lazyDelegateParameters
			End Get
		End Property

		Public Overrides ReadOnly Property ExplicitInterfaceImplementations As ImmutableArray(Of EventSymbol)
			Get
				If (Me._lazyImplementedEvents.IsDefault) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Me.ContainingSourceModule.AtomicStoreArrayAndDiagnostics(Of EventSymbol)(Me._lazyImplementedEvents, Me.ComputeImplementedEvents(instance), instance)
					instance.Free()
				End If
				Return Me._lazyImplementedEvents
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Dim decodedWellKnownAttributeData As EventWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasSpecialNameAttribute
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property IsDirectlyExcludedFromCodeCoverage As Boolean
			Get
				Dim decodedWellKnownAttributeData As EventWellKnownAttributeData = Me.GetDecodedWellKnownAttributeData()
				If (decodedWellKnownAttributeData Is Nothing) Then
					Return False
				End If
				Return decodedWellKnownAttributeData.HasExcludeFromCodeCoverageAttribute
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return (Me._memberFlags And SourceMemberFlags.[MustOverride]) <> SourceMemberFlags.None
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverridable As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsOverrides As Boolean
			Get
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property IsShared As Boolean
			Get
				Return (Me._memberFlags And SourceMemberFlags.[Shared]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend ReadOnly Property IsTypeInferred As Boolean
			Get
				Dim type As TypeSymbol = Me.Type
				Return (Me._lazyState And 1) <> 0
			End Get
		End Property

		Public Overrides ReadOnly Property IsWindowsRuntimeEvent As Boolean
			Get
				Dim explicitInterfaceImplementations As ImmutableArray(Of EventSymbol) = Me.ExplicitInterfaceImplementations
				If (Not System.Linq.ImmutableArrayExtensions.Any(Of EventSymbol)(explicitInterfaceImplementations)) Then
					Return Me.IsCompilationOutputWinMdObj()
				End If
				Return explicitInterfaceImplementations(0).IsWindowsRuntimeEvent
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray.Create(Of Location)(Me._location)
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Dim uninitialized As Microsoft.CodeAnalysis.ObsoleteAttributeData
				Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData
				If (Me._containingType.AnyMemberHasAttributes) Then
					Dim customAttributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
					If (customAttributesBag Is Nothing OrElse Not customAttributesBag.IsEarlyDecodedWellKnownAttributeDataComputed) Then
						uninitialized = Microsoft.CodeAnalysis.ObsoleteAttributeData.Uninitialized
					Else
						Dim earlyDecodedWellKnownAttributeData As CommonEventEarlyWellKnownAttributeData = DirectCast(Me._lazyCustomAttributesBag.EarlyDecodedWellKnownAttributeData, CommonEventEarlyWellKnownAttributeData)
						If (earlyDecodedWellKnownAttributeData IsNot Nothing) Then
							obsoleteAttributeDatum = earlyDecodedWellKnownAttributeData.ObsoleteAttributeData
						Else
							obsoleteAttributeDatum = Nothing
						End If
						uninitialized = obsoleteAttributeDatum
					End If
				Else
					uninitialized = Nothing
				End If
				Return uninitialized
			End Get
		End Property

		Public Overrides ReadOnly Property RaiseMethod As MethodSymbol
			Get
				Return Me._raiseMethod
			End Get
		End Property

		Public Overrides ReadOnly Property RemoveMethod As MethodSymbol
			Get
				Return Me._removeMethod
			End Get
		End Property

		Friend Overrides ReadOnly Property ShadowsExplicitly As Boolean
			Get
				Return (Me._memberFlags And SourceMemberFlags.[Shadows]) <> SourceMemberFlags.None
			End Get
		End Property

		Friend ReadOnly Property SyntaxReference As Microsoft.CodeAnalysis.SyntaxReference
			Get
				Return Me._syntaxRef
			End Get
		End Property

		Public Overrides ReadOnly Property Type As TypeSymbol
			Get
				If (Me._lazyType Is Nothing) Then
					Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					Dim flag As Boolean = False
					Dim flag1 As Boolean = False
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me.ComputeType(instance, flag, flag1)
					Dim num As Integer = If(flag, 1, 0) Or If(flag1, 2, 0)
					ThreadSafeFlagOperations.[Set](Me._lazyState, num)
					Me.ContainingSourceModule.AtomicStoreReferenceAndDiagnostics(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyType, typeSymbol, instance, Nothing)
					instance.Free()
				End If
				Return Me._lazyType
			End Get
		End Property

		Friend Sub New(ByVal containingType As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal syntax As EventStatementSyntax, ByVal blockSyntaxOpt As EventBlockSyntax, ByVal diagnostics As DiagnosticBag)
			MyBase.New()
			Dim locations As ImmutableArray(Of Microsoft.CodeAnalysis.Location)
			Me._containingType = containingType
			Dim memberModifier As MemberModifiers = SourceEventSymbol.DecodeModifiers(syntax.Modifiers, containingType, binder, diagnostics)
			Me._memberFlags = memberModifier.AllFlags
			Dim identifier As SyntaxToken = syntax.Identifier
			Me._name = identifier.ValueText
			If (identifier.GetTypeCharacter() <> TypeCharacter.None) Then
				Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, identifier, ERRID.ERR_TypecharNotallowed)
			End If
			Dim location As Microsoft.CodeAnalysis.Location = identifier.GetLocation()
			Me._location = location
			Me._syntaxRef = binder.GetSyntaxReference(syntax)
			binder = New LocationSpecificBinder(BindingLocation.EventSignature, Me, binder)
			If (blockSyntaxOpt Is Nothing) Then
				Me._addMethod = New SynthesizedAddAccessorSymbol(containingType, Me)
				Me._removeMethod = New SynthesizedRemoveAccessorSymbol(containingType, Me)
				If (Not containingType.IsInterfaceType()) Then
					Me._backingField = New SynthesizedEventBackingFieldSymbol(Me, [String].Concat(Me.Name, "Event"), Me.IsShared)
				End If
			Else
				Dim enumerator As SyntaxList(Of AccessorBlockSyntax).Enumerator = blockSyntaxOpt.Accessors.GetEnumerator()
				While enumerator.MoveNext()
					Dim customEventAccessorSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.CustomEventAccessorSymbol = Me.BindEventAccessor(enumerator.Current, binder)
					Select Case customEventAccessorSymbol.MethodKind
						Case MethodKind.EventAdd
							If (Me._addMethod IsNot Nothing) Then
								locations = customEventAccessorSymbol.Locations
								diagnostics.Add(ERRID.ERR_DuplicateAddHandlerDef, locations(0))
								Continue While
							Else
								Me._addMethod = customEventAccessorSymbol
								Continue While
							End If
						Case MethodKind.EventRaise
							If (Me._raiseMethod IsNot Nothing) Then
								locations = customEventAccessorSymbol.Locations
								diagnostics.Add(ERRID.ERR_DuplicateRaiseEventDef, locations(0))
								Continue While
							Else
								Me._raiseMethod = customEventAccessorSymbol
								Continue While
							End If
						Case MethodKind.EventRemove
							If (Me._removeMethod IsNot Nothing) Then
								locations = customEventAccessorSymbol.Locations
								diagnostics.Add(ERRID.ERR_DuplicateRemoveHandlerDef, locations(0))
								Continue While
							Else
								Me._removeMethod = customEventAccessorSymbol
								Continue While
							End If
					End Select
					Throw ExceptionUtilities.UnexpectedValue(customEventAccessorSymbol.MethodKind)
				End While
				If (Me._addMethod Is Nothing) Then
					diagnostics.Add(ERRID.ERR_MissingAddHandlerDef1, location, New [Object]() { Me })
				End If
				If (Me._removeMethod Is Nothing) Then
					diagnostics.Add(ERRID.ERR_MissingRemoveHandlerDef1, location, New [Object]() { Me })
				End If
				If (Me._raiseMethod Is Nothing) Then
					diagnostics.Add(ERRID.ERR_MissingRaiseEventDef1, location, New [Object]() { Me })
					Return
				End If
			End If
		End Sub

		Friend Overrides Sub AddSynthesizedAttributes(ByVal compilationState As ModuleCompilationState, ByRef attributes As ArrayBuilder(Of SynthesizedAttributeData))
			MyBase.AddSynthesizedAttributes(compilationState, attributes)
			If (Me.Type.ContainsTupleNames()) Then
				Symbol.AddSynthesizedAttribute(attributes, Me.DeclaringCompilation.SynthesizeTupleNamesAttribute(Me.Type))
			End If
		End Sub

		Private Function BindEventAccessor(ByVal blockSyntax As AccessorBlockSyntax, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder) As CustomEventAccessorSymbol
			Dim blockStatement As MethodBaseSyntax = blockSyntax.BlockStatement
			Dim sourceMemberFlag As SourceMemberFlags = Me._memberFlags
			If (Me.IsImplementing()) Then
				sourceMemberFlag = sourceMemberFlag Or SourceMemberFlags.[Overrides] Or SourceMemberFlags.[NotOverridable]
			End If
			Select Case blockSyntax.Kind()
				Case SyntaxKind.AddHandlerAccessorBlock
					sourceMemberFlag = sourceMemberFlag Or SourceMemberFlags.MethodKindEventAdd Or SourceMemberFlags.[Dim]
					Exit Select
				Case SyntaxKind.RemoveHandlerAccessorBlock
					sourceMemberFlag = sourceMemberFlag Or SourceMemberFlags.MethodKindEventRemove Or SourceMemberFlags.[Dim]
					Exit Select
				Case SyntaxKind.RaiseEventAccessorBlock
					sourceMemberFlag = sourceMemberFlag Or SourceMemberFlags.MethodKindEventRaise Or SourceMemberFlags.[Dim]
					Exit Select
				Case Else
					Throw ExceptionUtilities.UnexpectedValue(blockSyntax.Kind())
			End Select
			Dim location As Microsoft.CodeAnalysis.Location = blockStatement.GetLocation()
			Return New CustomEventAccessorSymbol(Me._containingType, Me, Microsoft.CodeAnalysis.VisualBasic.Binder.GetAccessorName(Me.Name, sourceMemberFlag.ToMethodKind(), False), sourceMemberFlag, binder.GetSyntaxReference(blockStatement), location)
		End Function

		Private Sub CheckExplicitImplementationTypes()
			If ((Me._lazyState And 7) = 0) Then
				Dim instance As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag = Nothing
				Dim type As TypeSymbol = Me.Type
				Dim enumerator As ImmutableArray(Of EventSymbol).Enumerator = Me.ExplicitInterfaceImplementations.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As EventSymbol = enumerator.Current
					If (current.Type.IsSameType(type, TypeCompareKind.IgnoreTupleNames)) Then
						Continue While
					End If
					If (instance Is Nothing) Then
						instance = Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag.GetInstance()
					End If
					Dim implementingLocation As Location = Me.GetImplementingLocation(current)
					instance.Add(ERRID.ERR_EventImplMismatch5, implementingLocation, New [Object]() { Me, current, current.ContainingType, type, current.Type })
				End While
				If (instance IsNot Nothing) Then
					Me.ContainingSourceModule.AtomicSetFlagAndStoreDiagnostics(Me._lazyState, 4, 0, instance)
					instance.Free()
				End If
			End If
		End Sub

		Private Function ComputeImplementedEvents(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of EventSymbol)
			Dim empty As ImmutableArray(Of EventSymbol)
			Dim syntax As EventStatementSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), EventStatementSyntax)
			Dim implementsClause As ImplementsClauseSyntax = syntax.ImplementsClause
			If (implementsClause IsNot Nothing) Then
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
				If (Not Me._containingType.IsInterfaceType()) Then
					If (Me.IsShared AndAlso Not Me._containingType.IsModuleType()) Then
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.Modifiers.First(SyntaxKind.SharedKeyword), ERRID.ERR_SharedOnProcThatImpl)
						empty = ImmutableArray(Of EventSymbol).Empty
						Return empty
					End If
					empty = ImplementsHelper.ProcessImplementsClause(Of EventSymbol)(implementsClause, Me, Me._containingType, binder, diagnostics)
					Return empty
				Else
					Dim implementsKeyword As SyntaxToken = implementsClause.ImplementsKeyword
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, implementsKeyword, ERRID.ERR_InterfaceEventCantUse1, New [Object]() { implementsKeyword.ValueText })
				End If
			End If
			empty = ImmutableArray(Of EventSymbol).Empty
			Return empty
		End Function

		Private Function ComputeType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag, <Out> ByRef isTypeInferred As Boolean, <Out> ByRef isDelegateFromImplements As Boolean) As TypeSymbol
			Dim synthesizedEventDelegateSymbol As TypeSymbol
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
			Dim syntax As EventStatementSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), EventStatementSyntax)
			isTypeInferred = False
			isDelegateFromImplements = False
			Dim flag As Boolean = If(syntax.ImplementsClause IsNot Nothing, False, Me.IsWindowsRuntimeEvent)
			If (syntax.AsClause Is Nothing) Then
				If (flag) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.Identifier, ERRID.ERR_WinRTEventWithoutDelegate)
				End If
				Dim explicitInterfaceImplementations As ImmutableArray(Of EventSymbol) = Me.ExplicitInterfaceImplementations
				If (explicitInterfaceImplementations.IsEmpty) Then
					Dim typeMembers As ImmutableArray(Of NamedTypeSymbol) = Me._containingType.GetTypeMembers([String].Concat(Me.Name, "EventHandler"))
					synthesizedEventDelegateSymbol = Nothing
					Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = typeMembers.GetEnumerator()
					While enumerator.MoveNext()
						Dim current As NamedTypeSymbol = enumerator.Current
						If (current.AssociatedSymbol <> Me) Then
							Continue While
						End If
						synthesizedEventDelegateSymbol = current
						Exit While
					End While
					If (synthesizedEventDelegateSymbol Is Nothing) Then
						synthesizedEventDelegateSymbol = New Microsoft.CodeAnalysis.VisualBasic.Symbols.SynthesizedEventDelegateSymbol(Me._syntaxRef, Me._containingType)
					End If
					isTypeInferred = True
				Else
					Dim type As TypeSymbol = explicitInterfaceImplementations(0).Type
					Dim length As Integer = explicitInterfaceImplementations.Length - 1
					Dim num As Integer = 1
					Do
						Dim item As EventSymbol = explicitInterfaceImplementations(num)
						If (Not item.Type.IsSameType(type, TypeCompareKind.IgnoreTupleNames)) Then
							Dim implementingLocation As Location = Me.GetImplementingLocation(item)
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, implementingLocation, ERRID.ERR_MultipleEventImplMismatch3, New [Object]() { Me, item, item.ContainingType })
						End If
						num = num + 1
					Loop While num <= length
					synthesizedEventDelegateSymbol = type
					isDelegateFromImplements = True
				End If
			Else
				synthesizedEventDelegateSymbol = binder.DecodeIdentifierType(syntax.Identifier, syntax.AsClause, Nothing, diagnostics)
				If (Not syntax.AsClause.AsKeyword.IsMissing) Then
					If (synthesizedEventDelegateSymbol.IsDelegateType()) Then
						Dim delegateInvokeMethod As MethodSymbol = DirectCast(synthesizedEventDelegateSymbol, NamedTypeSymbol).DelegateInvokeMethod
						If (delegateInvokeMethod Is Nothing) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.AsClause.Type, ERRID.ERR_UnsupportedType1, New [Object]() { synthesizedEventDelegateSymbol.Name })
						ElseIf (Not delegateInvokeMethod.IsSub) Then
							Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.AsClause.Type, ERRID.ERR_EventDelegatesCantBeFunctions)
						End If
					Else
						Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.AsClause.Type, ERRID.ERR_EventTypeNotDelegate)
					End If
				ElseIf (flag) Then
					Microsoft.CodeAnalysis.VisualBasic.Binder.ReportDiagnostic(diagnostics, syntax.Identifier, ERRID.ERR_WinRTEventWithoutDelegate)
				End If
			End If
			If (Not synthesizedEventDelegateSymbol.IsErrorType()) Then
				AccessCheck.VerifyAccessExposureForMemberType(Me, syntax.Identifier, synthesizedEventDelegateSymbol, diagnostics, isDelegateFromImplements)
			End If
			Return synthesizedEventDelegateSymbol
		End Function

		Private Function CreateBinderForTypeDeclaration() As Microsoft.CodeAnalysis.VisualBasic.Binder
			Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = BinderBuilder.CreateBinderForType(Me.ContainingSourceModule, Me._syntaxRef.SyntaxTree, Me._containingType)
			Return New LocationSpecificBinder(BindingLocation.EventSignature, Me, binder)
		End Function

		Friend Shared Function DecodeModifiers(ByVal modifiers As SyntaxTokenList, ByVal container As SourceMemberContainerTypeSymbol, ByVal binder As Microsoft.CodeAnalysis.VisualBasic.Binder, ByVal diagBag As DiagnosticBag) As MemberModifiers
			Dim memberModifier As MemberModifiers = binder.DecodeModifiers(modifiers, SourceMemberFlags.[Private] Or SourceMemberFlags.[Protected] Or SourceMemberFlags.[Friend] Or SourceMemberFlags.[Public] Or SourceMemberFlags.AllAccessibilityModifiers Or SourceMemberFlags.[Shared] Or SourceMemberFlags.[Shadows] Or SourceMemberFlags.InvalidIfDefault, ERRID.ERR_BadEventFlags1, Accessibility.[Public], diagBag)
			Return binder.ValidateEventModifiers(modifiers, memberModifier, container, diagBag)
		End Function

		Friend Overrides Sub DecodeWellKnownAttribute(ByRef arguments As DecodeWellKnownAttributeArguments(Of AttributeSyntax, VisualBasicAttributeData, AttributeLocation))
			Dim attribute As VisualBasicAttributeData = arguments.Attribute
			If (attribute.IsTargetAttribute(Me, AttributeDescription.TupleElementNamesAttribute)) Then
				DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_ExplicitTupleElementNamesAttribute, arguments.AttributeSyntaxOpt.Location)
			End If
			If (attribute.IsTargetAttribute(Me, AttributeDescription.NonSerializedAttribute)) Then
				If (Not Me.ContainingType.IsSerializable) Then
					DirectCast(arguments.Diagnostics, Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag).Add(ERRID.ERR_InvalidNonSerializedUsage, arguments.AttributeSyntaxOpt.GetLocation())
				Else
					arguments.GetOrCreateData(Of EventWellKnownAttributeData)().HasNonSerializedAttribute = True
				End If
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.SpecialNameAttribute)) Then
				arguments.GetOrCreateData(Of EventWellKnownAttributeData)().HasSpecialNameAttribute = True
			ElseIf (attribute.IsTargetAttribute(Me, AttributeDescription.ExcludeFromCodeCoverageAttribute)) Then
				arguments.GetOrCreateData(Of EventWellKnownAttributeData)().HasExcludeFromCodeCoverageAttribute = True
			End If
			MyBase.DecodeWellKnownAttribute(arguments)
		End Sub

		Friend Overrides Function EarlyDecodeWellKnownAttribute(ByRef arguments As EarlyDecodeWellKnownAttributeArguments(Of EarlyWellKnownAttributeBinder, NamedTypeSymbol, AttributeSyntax, AttributeLocation)) As VisualBasicAttributeData
			Dim visualBasicAttributeDatum As VisualBasicAttributeData
			Dim visualBasicAttributeDatum1 As VisualBasicAttributeData = Nothing
			Dim obsoleteAttributeDatum As Microsoft.CodeAnalysis.ObsoleteAttributeData = Nothing
			If (Not MyBase.EarlyDecodeDeprecatedOrExperimentalOrObsoleteAttribute(arguments, visualBasicAttributeDatum1, obsoleteAttributeDatum)) Then
				visualBasicAttributeDatum = MyBase.EarlyDecodeWellKnownAttribute(arguments)
			Else
				If (obsoleteAttributeDatum IsNot Nothing) Then
					arguments.GetOrCreateData(Of CommonEventEarlyWellKnownAttributeData)().ObsoleteAttributeData = obsoleteAttributeDatum
				End If
				visualBasicAttributeDatum = visualBasicAttributeDatum1
			End If
			Return visualBasicAttributeDatum
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			MyBase.GenerateDeclarationErrors(cancellationToken)
			Dim type As TypeSymbol = Me.Type
			Dim explicitInterfaceImplementations As ImmutableArray(Of EventSymbol) = Me.ExplicitInterfaceImplementations
			Me.CheckExplicitImplementationTypes()
			If (Me.DeclaringCompilation.EventQueue IsNot Nothing) Then
				Me.ContainingSourceModule.AtomicSetFlagAndRaiseSymbolDeclaredEvent(Me._lazyState, 8, 0, Me)
			End If
		End Sub

		Friend Function GetAccessorImplementations(ByVal kind As MethodKind) As ImmutableArray(Of MethodSymbol)
			Dim immutableAndFree As ImmutableArray(Of MethodSymbol)
			Dim addMethod As MethodSymbol
			Dim explicitInterfaceImplementations As ImmutableArray(Of EventSymbol) = Me.ExplicitInterfaceImplementations
			If (Not explicitInterfaceImplementations.IsEmpty) Then
				Dim instance As ArrayBuilder(Of MethodSymbol) = ArrayBuilder(Of MethodSymbol).GetInstance()
				Dim enumerator As ImmutableArray(Of EventSymbol).Enumerator = explicitInterfaceImplementations.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As EventSymbol = enumerator.Current
					Select Case kind
						Case MethodKind.EventAdd
							addMethod = current.AddMethod
							Exit Select
						Case MethodKind.EventRaise
							addMethod = current.RaiseMethod
							Exit Select
						Case MethodKind.EventRemove
							addMethod = current.RemoveMethod
							Exit Select
						Case Else
							Throw ExceptionUtilities.UnexpectedValue(kind)
					End Select
					If (addMethod Is Nothing) Then
						Continue While
					End If
					instance.Add(addMethod)
				End While
				immutableAndFree = instance.ToImmutableAndFree()
			Else
				immutableAndFree = ImmutableArray(Of MethodSymbol).Empty
			End If
			Return immutableAndFree
		End Function

		Public NotOverridable Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.GetAttributesBag().Attributes
		End Function

		Private Function GetAttributesBag() As CustomAttributesBag(Of VisualBasicAttributeData)
			If (Me._lazyCustomAttributesBag Is Nothing OrElse Not Me._lazyCustomAttributesBag.IsSealed) Then
				MyBase.LoadAndValidateAttributes(OneOrMany.Create(Of SyntaxList(Of AttributeListSyntax))(Me.AttributeDeclarationSyntaxList), Me._lazyCustomAttributesBag, AttributeLocation.None)
			End If
			Return Me._lazyCustomAttributesBag
		End Function

		Friend Function GetDecodedWellKnownAttributeData() As EventWellKnownAttributeData
			Dim attributesBag As CustomAttributesBag(Of VisualBasicAttributeData) = Me._lazyCustomAttributesBag
			If (attributesBag Is Nothing OrElse Not attributesBag.IsDecodedWellKnownAttributeDataComputed) Then
				attributesBag = Me.GetAttributesBag()
			End If
			Return DirectCast(attributesBag.DecodedWellKnownAttributeData, EventWellKnownAttributeData)
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Dim str As String
			str = If(Not expandIncludes, SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyDocComment, cancellationToken), SourceDocumentationCommentUtils.GetAndCacheDocumentationComment(Me, preferredCulture, expandIncludes, Me._lazyExpandedDocComment, cancellationToken))
			Return str
		End Function

		Friend Function GetImplementingLocation(ByVal implementedEvent As EventSymbol) As Microsoft.CodeAnalysis.Location
			Dim location As Microsoft.CodeAnalysis.Location
			Dim syntax As EventStatementSyntax = DirectCast(Me._syntaxRef.GetSyntax(New CancellationToken()), EventStatementSyntax)
			Dim syntaxTree As Microsoft.CodeAnalysis.SyntaxTree = Me._syntaxRef.SyntaxTree
			If (syntax.ImplementsClause Is Nothing) Then
				location = If(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Microsoft.CodeAnalysis.Location)(Me.Locations), NoLocation.Singleton)
			Else
				Dim binder As Microsoft.CodeAnalysis.VisualBasic.Binder = Me.CreateBinderForTypeDeclaration()
				location = ImplementsHelper.FindImplementingSyntax(Of EventSymbol)(syntax.ImplementsClause, Me, implementedEvent, Me._containingType, binder).GetLocation()
			End If
			Return location
		End Function

		Friend Overrides Function GetLexicalSortKey() As LexicalSortKey
			Return New LexicalSortKey(Me._location, Me.DeclaringCompilation)
		End Function

		Friend NotOverridable Overrides Function IsDefinedInSourceTree(ByVal tree As SyntaxTree, ByVal definedWithinSpan As Nullable(Of TextSpan), Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As Boolean
			Return Symbol.IsDefinedInSourceTree(Me._syntaxRef.GetSyntax(cancellationToken).Parent, tree, definedWithinSpan, cancellationToken)
		End Function

		Private Function IsImplementing() As Boolean
			Return Not Me.ExplicitInterfaceImplementations.IsEmpty
		End Function

		<Flags>
		Private Enum StateFlags
			IsTypeInferred = 1
			IsDelegateFromImplements = 2
			ReportedExplicitImplementationDiagnostics = 4
			SymbolDeclaredEvent = 8
		End Enum
	End Class
End Namespace