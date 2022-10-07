Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Emit
Imports Microsoft.CodeAnalysis.Emit.NoPia
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Emit
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Concurrent
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedTypesManager
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter)
		Private ReadOnly _assemblyGuidMap As ConcurrentDictionary(Of AssemblySymbol, String)

		Private ReadOnly _reportedSymbolsMap As ConcurrentDictionary(Of Symbol, Boolean)

		Private _lazySystemStringType As NamedTypeSymbol

		Private ReadOnly _lazyWellKnownTypeMethods As MethodSymbol()

		Public Sub New(ByVal moduleBeingBuilt As PEModuleBuilder)
			MyBase.New(moduleBeingBuilt)
			Me._assemblyGuidMap = New ConcurrentDictionary(Of AssemblySymbol, String)(ReferenceEqualityComparer.Instance)
			Me._reportedSymbolsMap = New ConcurrentDictionary(Of Symbol, Boolean)(ReferenceEqualityComparer.Instance)
			Me._lazySystemStringType = ErrorTypeSymbol.UnknownResultType
			ReDim Me._lazyWellKnownTypeMethods(417)
			Dim num As Integer = 0
			Do
				Me._lazyWellKnownTypeMethods(num) = ErrorMethodSymbol.UnknownMethod
				num = num + 1
			Loop While num <= 417
		End Sub

		Protected Overrides Function CreateCompilerGeneratedAttribute() As VisualBasicAttributeData
			Dim compilation As !0 = Me.ModuleBeingBuilt.Compilation
			Dim typedConstants As ImmutableArray(Of TypedConstant) = New ImmutableArray(Of TypedConstant)()
			Return compilation.TrySynthesizeAttribute(WellKnownMember.System_Runtime_CompilerServices_CompilerGeneratedAttribute__ctor, typedConstants, New ImmutableArray(Of KeyValuePair(Of WellKnownMember, TypedConstant))(), False)
		End Function

		Friend Overrides Function CreateSynthesizedAttribute(ByVal constructor As Microsoft.CodeAnalysis.WellKnownMember, ByVal attrData As VisualBasicAttributeData, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As VisualBasicAttributeData
			Dim synthesizedAttributeDatum As VisualBasicAttributeData
			Dim wellKnownMethod As MethodSymbol = Me.GetWellKnownMethod(constructor, syntaxNodeOpt, diagnostics)
			If (wellKnownMethod IsNot Nothing) Then
				Dim wellKnownMember As Microsoft.CodeAnalysis.WellKnownMember = constructor
				If (wellKnownMember = Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_CoClassAttribute__ctor) Then
					Dim parameters As ImmutableArray(Of ParameterSymbol) = wellKnownMethod.Parameters
					synthesizedAttributeDatum = New SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(Of TypedConstant)(New TypedConstant(parameters(0).Type, TypedConstantKind.Type, wellKnownMethod.ContainingAssembly.GetSpecialType(SpecialType.System_Object))), ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Empty)
				ElseIf (wellKnownMember <> Microsoft.CodeAnalysis.WellKnownMember.System_Runtime_InteropServices_ComEventInterfaceAttribute__ctor) Then
					synthesizedAttributeDatum = New SynthesizedAttributeData(wellKnownMethod, attrData.CommonConstructorArguments, attrData.CommonNamedArguments)
				Else
					Dim item As TypedConstant = attrData.CommonConstructorArguments(0)
					Dim commonConstructorArguments As ImmutableArray(Of TypedConstant) = attrData.CommonConstructorArguments
					synthesizedAttributeDatum = New SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(Of TypedConstant)(item, commonConstructorArguments(0)), ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Empty)
				End If
			Else
				synthesizedAttributeDatum = Nothing
			End If
			Return synthesizedAttributeDatum
		End Function

		Friend Overrides Function EmbedEvent(ByVal type As EmbeddedType, ByVal [event] As EventSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, ByVal isUsedForComAwareEventBinding As Boolean) As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedEvent
			Dim embeddedEvent As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedEvent
			Dim embeddedMethod As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim embeddedMethod1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim embeddedMethod2 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim addMethod As MethodSymbol = [event].AdaptedEventSymbol.AddMethod
			Dim removeMethod As MethodSymbol = [event].AdaptedEventSymbol.RemoveMethod
			Dim raiseMethod As MethodSymbol = [event].AdaptedEventSymbol.RaiseMethod
			If (addMethod IsNot Nothing) Then
				embeddedMethod = Me.EmbedMethod(type, addMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			Else
				embeddedMethod = Nothing
			End If
			Dim embeddedMethod3 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod = embeddedMethod
			If (removeMethod IsNot Nothing) Then
				embeddedMethod1 = Me.EmbedMethod(type, removeMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			Else
				embeddedMethod1 = Nothing
			End If
			Dim embeddedMethod4 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod = embeddedMethod1
			If (raiseMethod IsNot Nothing) Then
				embeddedMethod2 = Me.EmbedMethod(type, raiseMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			Else
				embeddedMethod2 = Nothing
			End If
			Dim embeddedEvent1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedEvent = New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedEvent([event], embeddedMethod3, embeddedMethod4, embeddedMethod2)
			Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedEvent = Me.EmbeddedEventsMap.GetOrAdd([event], embeddedEvent1)
			If (embeddedEvent1 = orAdd) Then
				Me.VerifyNotFrozen()
				MyBase.EmbedReferences(embeddedEvent1, syntaxNodeOpt, diagnostics)
				embeddedEvent1.EmbedCorrespondingComEventInterfaceMethod(syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding)
				embeddedEvent = embeddedEvent1
			Else
				If (isUsedForComAwareEventBinding) Then
					orAdd.EmbedCorrespondingComEventInterfaceMethod(syntaxNodeOpt, diagnostics, isUsedForComAwareEventBinding)
				End If
				embeddedEvent = orAdd
			End If
			Return embeddedEvent
		End Function

		Friend Overrides Function EmbedField(ByVal type As EmbeddedType, ByVal field As FieldSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedField
			Dim embeddedField As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedField
			Dim embeddedField1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedField = New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedField(type, field)
			Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedField = Me.EmbeddedFieldsMap.GetOrAdd(field, embeddedField1)
			If (embeddedField1 = orAdd) Then
				Me.VerifyNotFrozen()
				MyBase.EmbedReferences(embeddedField1, syntaxNodeOpt, diagnostics)
				Dim typeKind As Microsoft.CodeAnalysis.TypeKind = field.AdaptedFieldSymbol.ContainingType.TypeKind
				If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Interface] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.[Delegate] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.Struct AndAlso (field.AdaptedFieldSymbol.IsShared OrElse field.AdaptedFieldSymbol.DeclaredAccessibility <> Accessibility.[Public])) Then
					EmbeddedTypesManager.ReportNotEmbeddableSymbol(ERRID.ERR_InvalidStructMemberNoPIA1, type.UnderlyingNamedType.AdaptedNamedTypeSymbol, syntaxNodeOpt, diagnostics, Me)
				End If
				embeddedField = embeddedField1
			Else
				embeddedField = orAdd
			End If
			Return embeddedField
		End Function

		Friend Overrides Function EmbedMethod(ByVal type As EmbeddedType, ByVal method As MethodSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim embeddedMethod As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim embeddedMethod1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod = New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod(type, method)
			Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod = Me.EmbeddedMethodsMap.GetOrAdd(method, embeddedMethod1)
			If (embeddedMethod1 = orAdd) Then
				Me.VerifyNotFrozen()
				MyBase.EmbedReferences(embeddedMethod1, syntaxNodeOpt, diagnostics)
				Dim typeKind As Microsoft.CodeAnalysis.TypeKind = type.UnderlyingNamedType.AdaptedNamedTypeSymbol.TypeKind
				If (typeKind = Microsoft.CodeAnalysis.TypeKind.[Enum] OrElse typeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
					EmbeddedTypesManager.ReportNotEmbeddableSymbol(ERRID.ERR_InvalidStructMemberNoPIA1, type.UnderlyingNamedType.AdaptedNamedTypeSymbol, syntaxNodeOpt, diagnostics, Me)
				ElseIf (embeddedMethod1.HasBody()) Then
					EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_InteropMethodWithBody1, syntaxNodeOpt, New [Object]() { method.AdaptedMethodSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat) })
				End If
				Dim associatedSymbol As Symbol = method.AdaptedMethodSymbol.AssociatedSymbol
				If (associatedSymbol IsNot Nothing) Then
					Dim kind As SymbolKind = associatedSymbol.Kind
					If (kind = SymbolKind.[Event]) Then
						Me.EmbedEvent(type, DirectCast(associatedSymbol, EventSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics, False)
					Else
						If (kind <> SymbolKind.[Property]) Then
							Throw ExceptionUtilities.UnexpectedValue(associatedSymbol.Kind)
						End If
						Me.EmbedProperty(type, DirectCast(associatedSymbol, PropertySymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics)
					End If
				End If
				embeddedMethod = embeddedMethod1
			Else
				embeddedMethod = orAdd
			End If
			Return embeddedMethod
		End Function

		Friend Shared Function EmbedParameters(ByVal containingPropertyOrMethod As EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedMember, ByVal underlyingParameters As ImmutableArray(Of ParameterSymbol)) As ImmutableArray(Of Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter)
			Dim embeddedParameter As Func(Of ParameterSymbol, EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedMember, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter)
			Dim parameterSymbols As ImmutableArray(Of ParameterSymbol) = underlyingParameters
			If (EmbeddedTypesManager._Closure$__.$I27-0 Is Nothing) Then
				embeddedParameter = Function(parameter As ParameterSymbol, container As EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedMember) New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter(container, parameter.GetCciAdapter())
				EmbeddedTypesManager._Closure$__.$I27-0 = embeddedParameter
			Else
				embeddedParameter = EmbeddedTypesManager._Closure$__.$I27-0
			End If
			Return Microsoft.CodeAnalysis.ImmutableArrayExtensions.SelectAsArray(Of ParameterSymbol, EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedMember, Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedParameter)(parameterSymbols, embeddedParameter, containingPropertyOrMethod)
		End Function

		Friend Overrides Function EmbedProperty(ByVal type As EmbeddedType, ByVal [property] As PropertySymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedProperty
			Dim embeddedProperty As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedProperty
			Dim embeddedMethod As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim embeddedMethod1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod
			Dim getMethod As MethodSymbol = [property].AdaptedPropertySymbol.GetMethod
			Dim setMethod As MethodSymbol = [property].AdaptedPropertySymbol.SetMethod
			If (getMethod IsNot Nothing) Then
				embeddedMethod = Me.EmbedMethod(type, getMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			Else
				embeddedMethod = Nothing
			End If
			Dim embeddedMethod2 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedMethod = embeddedMethod
			If (setMethod IsNot Nothing) Then
				embeddedMethod1 = Me.EmbedMethod(type, setMethod.GetCciAdapter(), syntaxNodeOpt, diagnostics)
			Else
				embeddedMethod1 = Nothing
			End If
			Dim embeddedProperty1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedProperty = New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedProperty([property], embeddedMethod2, embeddedMethod1)
			Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedProperty = Me.EmbeddedPropertiesMap.GetOrAdd([property], embeddedProperty1)
			If (embeddedProperty1 = orAdd) Then
				Me.VerifyNotFrozen()
				MyBase.EmbedReferences(embeddedProperty1, syntaxNodeOpt, diagnostics)
				embeddedProperty = embeddedProperty1
			Else
				embeddedProperty = orAdd
			End If
			Return embeddedProperty
		End Function

		Private Function EmbedType(ByVal namedType As NamedTypeSymbol, ByVal fromImplements As Boolean, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType
			Dim embeddedType As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType
			Dim enumerator As IEnumerator(Of FieldSymbol) = Nothing
			Dim enumerator1 As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol) = Nothing
			Dim cciAdapter As NamedTypeSymbol = namedType.GetCciAdapter()
			Dim embeddedType1 As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType = New Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType(Me, cciAdapter)
			Dim orAdd As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType = Me.EmbeddedTypesMap.GetOrAdd(cciAdapter, embeddedType1)
			Dim isInterface As Boolean = namedType.IsInterface
			If (isInterface AndAlso fromImplements) Then
				orAdd.EmbedAllMembersOfImplementedInterface(syntaxNodeOpt, diagnostics)
			End If
			If (embeddedType1 = orAdd) Then
				Me.VerifyNotFrozen()
				Dim typeReferenceIndexer As Microsoft.Cci.TypeReferenceIndexer = New Microsoft.Cci.TypeReferenceIndexer(New EmitContext(Me.ModuleBeingBuilt, syntaxNodeOpt, diagnostics, False, True))
				typeReferenceIndexer.VisitTypeDefinitionNoMembers(embeddedType1)
				If (Not isInterface) Then
					If (namedType.TypeKind <> Microsoft.CodeAnalysis.TypeKind.Struct) Then
						Dim typeKind As Microsoft.CodeAnalysis.TypeKind = namedType.TypeKind
					End If
					Using enumerator
						enumerator = namedType.GetFieldsToEmit().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As FieldSymbol = enumerator.Current
							Me.EmbedField(embeddedType1, current.GetCciAdapter(), syntaxNodeOpt, diagnostics)
						End While
					End Using
					Using enumerator1
						enumerator1 = namedType.GetMethodsToEmit().GetEnumerator()
						While enumerator1.MoveNext()
							Dim methodSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MethodSymbol = enumerator1.Current
							Me.EmbedMethod(embeddedType1, methodSymbol.GetCciAdapter(), syntaxNodeOpt, diagnostics)
						End While
					End Using
				End If
				embeddedType = embeddedType1
			Else
				embeddedType = orAdd
			End If
			Return embeddedType
		End Function

		Friend Function EmbedTypeIfNeedTo(ByVal namedType As NamedTypeSymbol, ByVal fromImplements As Boolean, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As INamedTypeReference
			Dim namedTypeReference As INamedTypeReference
			If (Not EmbeddedTypesManager.IsValidEmbeddableType(namedType, syntaxNodeOpt, diagnostics, Me)) Then
				namedTypeReference = Nothing
			Else
				namedTypeReference = Me.EmbedType(namedType, fromImplements, syntaxNodeOpt, diagnostics)
			End If
			Return namedTypeReference
		End Function

		Friend Function GetAssemblyGuidString(ByVal assembly As AssemblySymbol) As String
			Dim orAdd As String
			Dim str As String = Nothing
			If (Not Me._assemblyGuidMap.TryGetValue(assembly, str)) Then
				assembly.GetGuidString(str)
				orAdd = Me._assemblyGuidMap.GetOrAdd(assembly, str)
			Else
				orAdd = str
			End If
			Return orAdd
		End Function

		Protected Overrides Function GetEmbeddedTypeForMember(ByVal member As Symbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType
			Dim embeddedType As Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia.EmbeddedType
			Dim containingType As NamedTypeSymbol = member.AdaptedSymbol.ContainingType
			If (Not EmbeddedTypesManager.IsValidEmbeddableType(containingType, syntaxNodeOpt, diagnostics, Me)) Then
				embeddedType = Nothing
			Else
				embeddedType = Me.EmbedType(containingType, False, syntaxNodeOpt, diagnostics)
			End If
			Return embeddedType
		End Function

		Public Function GetSystemStringType(ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As NamedTypeSymbol
			If (Me._lazySystemStringType = ErrorTypeSymbol.UnknownResultType) Then
				Dim specialType As NamedTypeSymbol = Me.ModuleBeingBuilt.Compilation.GetSpecialType(Microsoft.CodeAnalysis.SpecialType.System_String)
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = specialType.GetUseSiteInfo()
				If (specialType.IsErrorType()) Then
					specialType = Nothing
				End If
				If (TypeSymbol.Equals(Interlocked.CompareExchange(Of NamedTypeSymbol)(Me._lazySystemStringType, specialType, ErrorTypeSymbol.UnknownResultType), ErrorTypeSymbol.UnknownResultType, TypeCompareKind.ConsiderEverything) AndAlso useSiteInfo.DiagnosticInfo IsNot Nothing) Then
					EmbeddedTypesManager.ReportDiagnostic(diagnostics, syntaxNodeOpt, useSiteInfo.DiagnosticInfo)
				End If
			End If
			Return Me._lazySystemStringType
		End Function

		Friend Overrides Function GetTargetAttributeSignatureIndex(ByVal underlyingSymbol As Symbol, ByVal attrData As VisualBasicAttributeData, ByVal description As AttributeDescription) As Integer
			Return attrData.GetTargetAttributeSignatureIndex(underlyingSymbol.AdaptedSymbol, description)
		End Function

		Public Function GetWellKnownMethod(ByVal method As WellKnownMember, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As MethodSymbol
			Return Me.LazyGetWellKnownTypeMethod(Me._lazyWellKnownTypeMethods(CInt(method)), method, syntaxNodeOpt, diagnostics)
		End Function

		Friend Shared Function IsValidEmbeddableType(ByVal type As NamedTypeSymbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, Optional ByVal typeManagerOpt As EmbeddedTypesManager = Nothing) As Boolean
			Dim flag As Boolean
			If (type.SpecialType <> SpecialType.None OrElse type.IsErrorType() OrElse Not type.ContainingAssembly.IsLinked) Then
				flag = False
			Else
				Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None
				Dim typeKind As Microsoft.CodeAnalysis.TypeKind = type.TypeKind
				Select Case typeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					Label0:
						If (type.IsTupleType) Then
							type = type.TupleUnderlyingType
						End If
						If (type.ContainingType Is Nothing) Then
							If (Not type.IsGenericType) Then
								Exit Select
							End If
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CannotEmbedInterfaceWithGeneric
							Exit Select
						Else
							eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_NestedInteropType
							Exit Select
						End If
					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
					Label1:
						eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_CannotLinkClassWithNoPIA1
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
						Dim enumerator As ImmutableArray(Of Symbol).Enumerator = type.GetMembersUnordered().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Symbol = enumerator.Current
							If (current.Kind = SymbolKind.NamedType) Then
								Continue While
							End If
							If (current.IsMustOverride) Then
								If (Not current.IsNotOverridable) Then
									Continue While
								End If
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_ReAbstractionInNoPIAType
							Else
								eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_DefaultInterfaceImplementationInNoPIAType
							End If
						End While
						If (eRRID <> Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
							Exit Select
						Else
							GoTo Label0
						End If
					Case Else
						If (typeKind = Microsoft.CodeAnalysis.TypeKind.Struct) Then
							GoTo Label0
						End If
						GoTo Label1
				End Select
				If (eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_None) Then
					flag = True
				Else
					EmbeddedTypesManager.ReportNotEmbeddableSymbol(eRRID, type, syntaxNodeOpt, diagnostics, typeManagerOpt)
					flag = False
				End If
			End If
			Return flag
		End Function

		Private Function LazyGetWellKnownTypeMethod(ByRef lazyMethod As MethodSymbol, ByVal method As WellKnownMember, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As MethodSymbol
			If (lazyMethod = ErrorMethodSymbol.UnknownMethod) Then
				Dim useSiteInfo As UseSiteInfo(Of AssemblySymbol) = New UseSiteInfo(Of AssemblySymbol)()
				If (Interlocked.CompareExchange(Of MethodSymbol)(lazyMethod, DirectCast(Binder.GetWellKnownTypeMember(DirectCast(Me.ModuleBeingBuilt.Compilation, VisualBasicCompilation), method, useSiteInfo), MethodSymbol), ErrorMethodSymbol.UnknownMethod) = ErrorMethodSymbol.UnknownMethod AndAlso useSiteInfo.DiagnosticInfo IsNot Nothing) Then
					EmbeddedTypesManager.ReportDiagnostic(diagnostics, syntaxNodeOpt, useSiteInfo.DiagnosticInfo)
				End If
			End If
			Return lazyMethod
		End Function

		Protected Overrides Sub OnGetTypesCompleted(ByVal types As ImmutableArray(Of EmbeddedType), ByVal diagnostics As DiagnosticBag)
			Dim enumerator As IEnumerator(Of AssemblySymbol) = Nothing
			Dim enumerator1 As ImmutableArray(Of EmbeddedType).Enumerator = types.GetEnumerator()
			While enumerator1.MoveNext()
				Dim current As EmbeddedType = enumerator1.Current
				Me._assemblyGuidMap.TryAdd(current.UnderlyingNamedType.AdaptedNamedTypeSymbol.ContainingAssembly, Nothing)
			End While
			Using enumerator
				enumerator = Me.ModuleBeingBuilt.GetReferencedAssembliesUsedSoFar().GetEnumerator()
				While enumerator.MoveNext()
					Me.ReportIndirectReferencesToLinkedAssemblies(enumerator.Current, diagnostics)
				End While
			End Using
		End Sub

		Friend Shared Sub ReportDiagnostic(ByVal diagnostics As DiagnosticBag, ByVal id As ERRID, ByVal syntaxNodeOpt As SyntaxNode, ByVal ParamArray args As Object())
			EmbeddedTypesManager.ReportDiagnostic(diagnostics, syntaxNodeOpt, ErrorFactory.ErrorInfo(id, args))
		End Sub

		Private Shared Sub ReportDiagnostic(ByVal diagnostics As DiagnosticBag, ByVal syntaxNodeOpt As SyntaxNode, ByVal info As DiagnosticInfo)
			diagnostics.Add(New VBDiagnostic(info, If(syntaxNodeOpt Is Nothing, NoLocation.Singleton, syntaxNodeOpt.GetLocation()), False))
		End Sub

		Friend Overrides Sub ReportIndirectReferencesToLinkedAssemblies(ByVal assembly As AssemblySymbol, ByVal diagnostics As DiagnosticBag)
			Dim enumerator As ImmutableArray(Of ModuleSymbol).Enumerator = assembly.Modules.GetEnumerator()
			While enumerator.MoveNext()
				Dim enumerator1 As ImmutableArray(Of AssemblySymbol).Enumerator = enumerator.Current.GetReferencedAssemblySymbols().GetEnumerator()
				While enumerator1.MoveNext()
					Dim current As AssemblySymbol = enumerator1.Current
					If (current.IsMissing OrElse Not current.IsLinked OrElse Not Me._assemblyGuidMap.ContainsKey(current)) Then
						Continue While
					End If
					EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.WRN_IndirectRefToLinkedAssembly2, Nothing, New [Object]() { current, assembly })
				End While
			End While
		End Sub

		Protected Overrides Sub ReportNameCollisionBetweenEmbeddedTypes(ByVal typeA As EmbeddedType, ByVal typeB As EmbeddedType, ByVal diagnostics As DiagnosticBag)
			Dim adaptedNamedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = typeA.UnderlyingNamedType.AdaptedNamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = typeB.UnderlyingNamedType.AdaptedNamedTypeSymbol
			EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_DuplicateLocalTypes3, Nothing, New [Object]() { adaptedNamedTypeSymbol, adaptedNamedTypeSymbol.ContainingAssembly, namedTypeSymbol.ContainingAssembly })
		End Sub

		Protected Overrides Sub ReportNameCollisionWithAlreadyDeclaredType(ByVal type As EmbeddedType, ByVal diagnostics As DiagnosticBag)
			Dim adaptedNamedTypeSymbol As NamedTypeSymbol = type.UnderlyingNamedType.AdaptedNamedTypeSymbol
			EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_LocalTypeNameClash2, Nothing, New [Object]() { adaptedNamedTypeSymbol, adaptedNamedTypeSymbol.ContainingAssembly })
		End Sub

		Private Shared Sub ReportNotEmbeddableSymbol(ByVal id As ERRID, ByVal symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag, ByVal typeManagerOpt As EmbeddedTypesManager)
			If (typeManagerOpt Is Nothing OrElse typeManagerOpt._reportedSymbolsMap.TryAdd(symbol.OriginalDefinition, True)) Then
				EmbeddedTypesManager.ReportDiagnostic(diagnostics, id, syntaxNodeOpt, New [Object]() { symbol.OriginalDefinition })
			End If
		End Sub

		Private Sub VerifyNotFrozen()
			If (Me.IsFrozen) Then
				Throw ExceptionUtilities.UnexpectedValue(Me.IsFrozen)
			End If
		End Sub
	End Class
End Namespace