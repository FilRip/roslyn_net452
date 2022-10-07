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
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Emit.NoPia
	Friend NotInheritable Class EmbeddedType
		Inherits EmbeddedTypesManager(Of PEModuleBuilder, ModuleCompilationState, EmbeddedTypesManager, SyntaxNode, VisualBasicAttributeData, Symbol, AssemblySymbol, NamedTypeSymbol, FieldSymbol, MethodSymbol, EventSymbol, PropertySymbol, ParameterSymbol, TypeParameterSymbol, EmbeddedType, EmbeddedField, EmbeddedMethod, EmbeddedEvent, EmbeddedProperty, EmbeddedParameter, EmbeddedTypeParameter).CommonEmbeddedType
		Private _embeddedAllMembersOfImplementedInterface As Boolean

		Protected Overrides ReadOnly Property IsAbstract As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsMetadataAbstract
			End Get
		End Property

		Protected Overrides ReadOnly Property IsBeforeFieldInit As Boolean
			Get
				Dim flag As Boolean
				Select Case Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.TypeKind
					Case Microsoft.CodeAnalysis.TypeKind.[Delegate]
					Case Microsoft.CodeAnalysis.TypeKind.[Enum]
					Case Microsoft.CodeAnalysis.TypeKind.[Interface]
						flag = False
						Exit Select
					Case Microsoft.CodeAnalysis.TypeKind.Dynamic
					Case Microsoft.CodeAnalysis.TypeKind.[Error]
					Label0:
						flag = True
						Exit Select
					Case Else
						GoTo Label0
				End Select
				Return flag
			End Get
		End Property

		Protected Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsComImport
			End Get
		End Property

		Protected Overrides ReadOnly Property IsDelegate As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsDelegateType()
			End Get
		End Property

		Protected Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsInterfaceType()
			End Get
		End Property

		Protected Overrides ReadOnly Property IsPublic As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.DeclaredAccessibility = Accessibility.[Public]
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSealed As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsMetadataSealed
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsSerializable
			End Get
		End Property

		Protected Overrides ReadOnly Property IsSpecialName As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.HasSpecialName
			End Get
		End Property

		Protected Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsWindowsRuntimeImport
			End Get
		End Property

		Protected Overrides ReadOnly Property StringFormat As CharSet
			Get
				Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.MarshallingCharSet
			End Get
		End Property

		Public Sub New(ByVal typeManager As EmbeddedTypesManager, ByVal underlyingNamedType As NamedTypeSymbol)
			MyBase.New(typeManager, underlyingNamedType)
		End Sub

		Protected Overrides Function CreateTypeIdentifierAttribute(ByVal hasGuid As Boolean, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As VisualBasicAttributeData
			Dim synthesizedAttributeDatum As VisualBasicAttributeData
			Dim wellKnownMethod As MethodSymbol = Me.TypeManager.GetWellKnownMethod(If(hasGuid, WellKnownMember.System_Runtime_InteropServices_TypeIdentifierAttribute__ctor, WellKnownMember.System_Runtime_InteropServices_TypeIdentifierAttribute__ctorStringString), syntaxNodeOpt, diagnostics)
			If (wellKnownMethod Is Nothing) Then
				synthesizedAttributeDatum = Nothing
			ElseIf (Not hasGuid) Then
				Dim systemStringType As NamedTypeSymbol = Me.TypeManager.GetSystemStringType(syntaxNodeOpt, diagnostics)
				If (systemStringType Is Nothing) Then
					synthesizedAttributeDatum = Nothing
				Else
					Dim assemblyGuidString As String = Me.TypeManager.GetAssemblyGuidString(Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.ContainingAssembly)
					synthesizedAttributeDatum = New SynthesizedAttributeData(wellKnownMethod, ImmutableArray.Create(Of TypedConstant)(New TypedConstant(systemStringType, TypedConstantKind.Primitive, assemblyGuidString), New TypedConstant(systemStringType, TypedConstantKind.Primitive, Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.ToDisplayString(SymbolDisplayFormat.QualifiedNameOnlyFormat))), ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Empty)
				End If
			Else
				synthesizedAttributeDatum = New SynthesizedAttributeData(wellKnownMethod, ImmutableArray(Of TypedConstant).Empty, ImmutableArray(Of KeyValuePair(Of String, TypedConstant)).Empty)
			End If
			Return synthesizedAttributeDatum
		End Function

		Public Sub EmbedAllMembersOfImplementedInterface(ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag)
			Dim enumerator As IEnumerator(Of MethodSymbol) = Nothing
			Dim enumerator1 As IEnumerator(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol) = Nothing
			If (Not Me._embeddedAllMembersOfImplementedInterface) Then
				Me._embeddedAllMembersOfImplementedInterface = True
				Using Me._embeddedAllMembersOfImplementedInterface = True
					enumerator = Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetMethodsToEmit().GetEnumerator()
					While enumerator.MoveNext()
						Dim current As MethodSymbol = enumerator.Current
						If (current Is Nothing) Then
							Continue While
						End If
						Me.TypeManager.EmbedMethod(Me, current.GetCciAdapter(), syntaxNodeOpt, diagnostics)
					End While
				End Using
				Using enumerator1
					enumerator1 = Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetInterfacesToEmit().GetEnumerator()
					While enumerator1.MoveNext()
						Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = enumerator1.Current
						Me.TypeManager.ModuleBeingBuilt.Translate(namedTypeSymbol, syntaxNodeOpt, diagnostics, True, False)
					End While
				End Using
			End If
		End Sub

		Protected Overrides Sub EmbedDefaultMembers(ByVal defaultMember As String, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag)
			Dim members As ImmutableArray(Of Symbol) = Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetMembers(defaultMember)
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = members.GetEnumerator()
		Label1:
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				Dim kind As SymbolKind = current.Kind
				Select Case kind
					Case SymbolKind.[Event]
						Me.TypeManager.EmbedEvent(Me, DirectCast(current, EventSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics, False)
						Continue While
					Case SymbolKind.Field
						Me.TypeManager.EmbedField(Me, DirectCast(current, FieldSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics)
						Continue While
					Case SymbolKind.Label
					Case SymbolKind.Local
						Continue While
					Case SymbolKind.Method
						Me.TypeManager.EmbedMethod(Me, DirectCast(current, MethodSymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics)
						Continue While
					Case Else
						If (kind = SymbolKind.[Property]) Then
							Exit Select
						Else
							GoTo Label1
						End If
				End Select
				Me.TypeManager.EmbedProperty(Me, DirectCast(current, PropertySymbol).GetCciAdapter(), syntaxNodeOpt, diagnostics)
			End While
		End Sub

		Protected Overrides Function GetAssemblyRefIndex() As Integer
			Return Me.TypeManager.ModuleBeingBuilt.SourceModule.GetReferencedAssemblySymbols().IndexOf(Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.ContainingAssembly, ReferenceEqualityComparer.Instance)
		End Function

		Protected Overrides Function GetBaseClass(ByVal moduleBuilder As PEModuleBuilder, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag) As ITypeReference
			Dim baseTypeNoUseSiteDiagnostics As NamedTypeSymbol = Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.BaseTypeNoUseSiteDiagnostics
			If (baseTypeNoUseSiteDiagnostics Is Nothing) Then
				Return Nothing
			End If
			Return moduleBuilder.Translate(baseTypeNoUseSiteDiagnostics, syntaxNodeOpt, diagnostics, False, False)
		End Function

		Protected Overrides Function GetCustomAttributesToEmit(ByVal moduleBuilder As PEModuleBuilder) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetCustomAttributesToEmit(moduleBuilder.CompilationState)
		End Function

		Protected Overrides Function GetEventsToEmit() As IEnumerable(Of EventSymbol)
			Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetEventsToEmit()
		End Function

		Protected Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetFieldsToEmit()
		End Function

		Protected Overrides Function GetInterfaces(ByVal context As EmitContext) As IEnumerable(Of TypeReferenceWithAttributes)
			Return New EmbeddedType.VB$StateMachine_11_GetInterfaces(-2) With
			{
				.$VB$Me = Me,
				.$P_context = context
			}
		End Function

		Protected Overrides Function GetMethodsToEmit() As IEnumerable(Of MethodSymbol)
			Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetMethodsToEmit()
		End Function

		Protected Overrides Function GetPropertiesToEmit() As IEnumerable(Of PropertySymbol)
			Return Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.GetPropertiesToEmit()
		End Function

		Protected Overrides Function GetTypeLayoutIfStruct() As Nullable(Of TypeLayout)
			Return New Nullable(Of TypeLayout)(If(Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.IsStructureType(), Me.UnderlyingNamedType.AdaptedNamedTypeSymbol.Layout, New TypeLayout()))
		End Function

		Protected Overrides Sub ReportMissingAttribute(ByVal description As AttributeDescription, ByVal syntaxNodeOpt As SyntaxNode, ByVal diagnostics As DiagnosticBag)
			EmbeddedTypesManager.ReportDiagnostic(diagnostics, ERRID.ERR_NoPIAAttributeMissing2, syntaxNodeOpt, New [Object]() { Me.UnderlyingNamedType.AdaptedNamedTypeSymbol, description.FullName })
		End Sub
	End Class
End Namespace