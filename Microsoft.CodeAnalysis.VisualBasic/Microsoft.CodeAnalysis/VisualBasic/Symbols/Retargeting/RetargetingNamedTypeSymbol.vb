Imports Microsoft.Cci
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Linq
Imports System.Runtime.CompilerServices
Imports System.Runtime.InteropServices
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingNamedTypeSymbol
		Inherits InstanceTypeSymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingType As NamedTypeSymbol

		Private _lazyTypeParameters As ImmutableArray(Of TypeParameterSymbol)

		Private _lazyCoClass As TypeSymbol

		Private _lazyCustomAttributes As ImmutableArray(Of VisualBasicAttributeData)

		Private _lazyCachedUseSiteInfo As CachedUseSiteInfo(Of AssemblySymbol)

		Public Overrides ReadOnly Property Arity As Integer
			Get
				Return Me._underlyingType.Arity
			End Get
		End Property

		Friend Overrides ReadOnly Property CoClassType As TypeSymbol
			Get
				If (Me._lazyCoClass = ErrorTypeSymbol.UnknownResultType) Then
					Dim typeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol = Me._underlyingType.CoClassType
					If (typeSymbol IsNot Nothing) Then
						typeSymbol = Me.RetargetingTranslator.Retarget(typeSymbol, RetargetOptions.RetargetPrimitiveTypesByName)
					End If
					Interlocked.CompareExchange(Of Microsoft.CodeAnalysis.VisualBasic.Symbols.TypeSymbol)(Me._lazyCoClass, typeSymbol, ErrorTypeSymbol.UnknownResultType)
				End If
				Return Me._lazyCoClass
			End Get
		End Property

		Public Overrides ReadOnly Property ConstructedFrom As NamedTypeSymbol
			Get
				Return Me
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._retargetingModule.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingModule As ModuleSymbol
			Get
				Return Me._retargetingModule
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me.RetargetingTranslator.Retarget(Me._underlyingType.ContainingSymbol)
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Me._underlyingType.DeclaredAccessibility
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingType.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property DefaultPropertyName As String
			Get
				Return Me._underlyingType.DefaultPropertyName
			End Get
		End Property

		Public Overrides ReadOnly Property EnumUnderlyingType As NamedTypeSymbol
			Get
				Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._underlyingType.EnumUnderlyingType
				If (namedTypeSymbol Is Nothing) Then
					Return Nothing
				End If
				Return Me.RetargetingTranslator.Retarget(namedTypeSymbol, RetargetOptions.RetargetPrimitiveTypesByTypeCode)
			End Get
		End Property

		Friend Overrides ReadOnly Property HasCodeAnalysisEmbeddedAttribute As Boolean
			Get
				Return Me._underlyingType.HasCodeAnalysisEmbeddedAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property HasDeclarativeSecurity As Boolean
			Get
				Return Me._underlyingType.HasDeclarativeSecurity
			End Get
		End Property

		Friend Overrides ReadOnly Property HasSpecialName As Boolean
			Get
				Return Me._underlyingType.HasSpecialName
			End Get
		End Property

		Friend Overrides ReadOnly Property HasVisualBasicEmbeddedAttribute As Boolean
			Get
				Return Me._underlyingType.HasVisualBasicEmbeddedAttribute
			End Get
		End Property

		Friend Overrides ReadOnly Property IsComImport As Boolean
			Get
				Return Me._underlyingType.IsComImport
			End Get
		End Property

		Friend Overrides ReadOnly Property IsExtensibleInterfaceNoUseSiteDiagnostics As Boolean
			Get
				Return Me._underlyingType.IsExtensibleInterfaceNoUseSiteDiagnostics
			End Get
		End Property

		Public Overrides ReadOnly Property IsImplicitlyDeclared As Boolean
			Get
				Return Me._underlyingType.IsImplicitlyDeclared
			End Get
		End Property

		Friend Overrides ReadOnly Property IsInterface As Boolean
			Get
				Return Me._underlyingType.IsInterface
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataAbstract As Boolean
			Get
				Return Me._underlyingType.IsMetadataAbstract
			End Get
		End Property

		Friend Overrides ReadOnly Property IsMetadataSealed As Boolean
			Get
				Return Me._underlyingType.IsMetadataSealed
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustInherit As Boolean
			Get
				Return Me._underlyingType.IsMustInherit
			End Get
		End Property

		Public Overrides ReadOnly Property IsNotInheritable As Boolean
			Get
				Return Me._underlyingType.IsNotInheritable
			End Get
		End Property

		Public Overrides ReadOnly Property IsSerializable As Boolean
			Get
				Return Me._underlyingType.IsSerializable
			End Get
		End Property

		Friend Overrides ReadOnly Property IsWindowsRuntimeImport As Boolean
			Get
				Return Me._underlyingType.IsWindowsRuntimeImport
			End Get
		End Property

		Friend Overrides ReadOnly Property Layout As TypeLayout
			Get
				Return Me._underlyingType.Layout
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingType.Locations
			End Get
		End Property

		Friend Overrides ReadOnly Property MangleName As Boolean
			Get
				Return Me._underlyingType.MangleName
			End Get
		End Property

		Friend Overrides ReadOnly Property MarshallingCharSet As CharSet
			Get
				Return Me._underlyingType.MarshallingCharSet
			End Get
		End Property

		Public Overrides ReadOnly Property MemberNames As IEnumerable(Of String)
			Get
				Return Me._underlyingType.MemberNames
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingType.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property MightContainExtensionMethods As Boolean
			Get
				Return Me._underlyingType.MightContainExtensionMethods
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingType.Name
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Me._underlyingType.ObsoleteAttributeData
			End Get
		End Property

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me._retargetingModule.RetargetingTranslator
			End Get
		End Property

		Friend Overrides ReadOnly Property ShouldAddWinRTMembers As Boolean
			Get
				Return Me._underlyingType.ShouldAddWinRTMembers
			End Get
		End Property

		Public Overrides ReadOnly Property TypeKind As Microsoft.CodeAnalysis.TypeKind
			Get
				Return Me._underlyingType.TypeKind
			End Get
		End Property

		Public Overrides ReadOnly Property TypeParameters As ImmutableArray(Of TypeParameterSymbol)
			Get
				If (Me._lazyTypeParameters.IsDefault) Then
					If (Me.Arity <> 0) Then
						Dim typeParameterSymbols As ImmutableArray(Of TypeParameterSymbol) = Me.RetargetingTranslator.Retarget(Me._underlyingType.TypeParameters)
						Dim typeParameterSymbols1 As ImmutableArray(Of TypeParameterSymbol) = New ImmutableArray(Of TypeParameterSymbol)()
						ImmutableInterlocked.InterlockedCompareExchange(Of TypeParameterSymbol)(Me._lazyTypeParameters, typeParameterSymbols, typeParameterSymbols1)
					Else
						Me._lazyTypeParameters = ImmutableArray(Of TypeParameterSymbol).Empty
					End If
				End If
				Return Me._lazyTypeParameters
			End Get
		End Property

		Public ReadOnly Property UnderlyingNamedType As NamedTypeSymbol
			Get
				Return Me._underlyingType
			End Get
		End Property

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingType As NamedTypeSymbol)
			MyBase.New()
			Me._lazyCoClass = ErrorTypeSymbol.UnknownResultType
			Me._lazyCachedUseSiteInfo = CachedUseSiteInfo(Of AssemblySymbol).Uninitialized
			If (TypeOf underlyingType Is RetargetingNamedTypeSymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingType = underlyingType
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Me._underlyingType.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me)
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamedTypeSymbol)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Function AddExtensionMethodLookupSymbolsInfoViabilityCheck(ByVal method As MethodSymbol, ByVal options As LookupOptions, ByVal nameSet As LookupSymbolsInfo, ByVal originalBinder As Binder) As Boolean
			Return MyBase.AddExtensionMethodLookupSymbolsInfoViabilityCheck(Me.RetargetingTranslator.Retarget(method), options, nameSet, originalBinder)
		End Function

		Friend Overrides Sub AppendProbableExtensionMethods(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol))
			Dim count As Integer = methods.Count
			Me._underlyingType.AppendProbableExtensionMethods(name, methods)
			Dim num As Integer = methods.Count - 1
			For i As Integer = count To num
				methods(i) = Me.RetargetingTranslator.Retarget(methods(i))
			Next

		End Sub

		Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)), ByVal appendThrough As NamespaceSymbol)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Private Shared Function CyclicInheritanceError(ByVal diag As DiagnosticInfo) As ErrorTypeSymbol
			Return New ExtendedErrorTypeSymbol(diag, True, Nothing)
		End Function

		Friend Overrides Sub GenerateDeclarationErrors(ByVal cancellationToken As System.Threading.CancellationToken)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Function GetAppliedConditionalSymbols() As ImmutableArray(Of String)
			Return Me._underlyingType.GetAppliedConditionalSymbols()
		End Function

		Public Overrides Function GetAttributes() As ImmutableArray(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.GetRetargetedAttributes(Me._underlyingType, Me._lazyCustomAttributes, False)
		End Function

		Friend Overrides Function GetAttributeUsageInfo() As AttributeUsageInfo
			Return Me._underlyingType.GetAttributeUsageInfo()
		End Function

		Friend Overrides Function GetCustomAttributesToEmit(ByVal compilationState As ModuleCompilationState) As IEnumerable(Of VisualBasicAttributeData)
			Return Me.RetargetingTranslator.RetargetAttributes(Me._underlyingType.GetCustomAttributesToEmit(compilationState))
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingType.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Function GetEmittedNamespaceName() As String
			Return Me._underlyingType.GetEmittedNamespaceName()
		End Function

		Friend Overrides Function GetEventsToEmit() As IEnumerable(Of EventSymbol)
			Return New RetargetingNamedTypeSymbol.VB$StateMachine_71_GetEventsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overrides Sub GetExtensionMethods(ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal appendThrough As NamespaceSymbol, ByVal Name As String)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Function GetFieldsToEmit() As IEnumerable(Of FieldSymbol)
			Return New RetargetingNamedTypeSymbol.VB$StateMachine_68_GetFieldsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overrides Function GetGuidString(ByRef guidString As String) As Boolean
			Return Me._underlyingType.GetGuidString(guidString)
		End Function

		Friend Overrides Function GetInterfacesToEmit() As IEnumerable(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetInterfacesToEmit())
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetMembers())
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetMembers(name))
		End Function

		Friend Overrides Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetMembersUnordered())
		End Function

		Friend Overrides Function GetMethodsToEmit() As IEnumerable(Of MethodSymbol)
			Return New RetargetingNamedTypeSymbol.VB$StateMachine_69_GetMethodsToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overrides Function GetPropertiesToEmit() As IEnumerable(Of PropertySymbol)
			Return New RetargetingNamedTypeSymbol.VB$StateMachine_70_GetPropertiesToEmit(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Friend Overrides Function GetSecurityInformation() As IEnumerable(Of SecurityAttribute)
			Return Me._underlyingType.GetSecurityInformation()
		End Function

		Friend Overrides Function GetSynthesizedWithEventsOverrides() As IEnumerable(Of PropertySymbol)
			Return New RetargetingNamedTypeSymbol.VB$StateMachine_119_GetSynthesizedWithEventsOverrides(-2) With
			{
				.$VB$Me = Me
			}
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetTypeMembers())
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetTypeMembers(name))
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetTypeMembers(name, arity))
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.GetTypeMembersUnordered())
		End Function

		Friend Overrides Function GetUseSiteInfo() As UseSiteInfo(Of AssemblySymbol)
			Dim primaryDependency As AssemblySymbol = MyBase.PrimaryDependency
			If (Not Me._lazyCachedUseSiteInfo.IsInitialized) Then
				Me._lazyCachedUseSiteInfo.Initialize(primaryDependency, MyBase.CalculateUseSiteInfo())
			End If
			Return Me._lazyCachedUseSiteInfo.ToUseSiteInfo(primaryDependency)
		End Function

		Friend Overrides Function LookupMetadataType(ByRef emittedTypeName As MetadataTypeName) As NamedTypeSymbol
			Return Me.RetargetingTranslator.Retarget(Me._underlyingType.LookupMetadataType(emittedTypeName), RetargetOptions.RetargetPrimitiveTypesByName)
		End Function

		Friend Overrides Function MakeAcyclicBaseType(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim dependencyDiagnosticsForImportedClass As DiagnosticInfo = BaseTypeAnalysis.GetDependencyDiagnosticsForImportedClass(Me)
			If (dependencyDiagnosticsForImportedClass Is Nothing) Then
				Dim declaredBase As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me.GetDeclaredBase(New BasesBeingResolved())
				If (declaredBase Is Nothing) Then
					Dim baseTypeNoUseSiteDiagnostics As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._underlyingType.BaseTypeNoUseSiteDiagnostics
					If (baseTypeNoUseSiteDiagnostics IsNot Nothing) Then
						declaredBase = Me.RetargetingTranslator.Retarget(baseTypeNoUseSiteDiagnostics, RetargetOptions.RetargetPrimitiveTypesByName)
					End If
				End If
				namedTypeSymbol = declaredBase
			Else
				namedTypeSymbol = RetargetingNamedTypeSymbol.CyclicInheritanceError(dependencyDiagnosticsForImportedClass)
			End If
			Return namedTypeSymbol
		End Function

		Friend Overrides Function MakeAcyclicInterfaces(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Dim namedTypeSymbols As ImmutableArray(Of NamedTypeSymbol)
			Dim func
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me.GetDeclaredInterfacesNoUseSiteDiagnostics(New BasesBeingResolved())
			If (Me.IsInterface) Then
				Dim collection = declaredInterfacesNoUseSiteDiagnostics.[Select](Function(t As NamedTypeSymbol) New With { Key .t = t, Key .diag = BaseTypeAnalysis.GetDependencyDiagnosticsForImportedBaseInterface(Me, t) })
				If (RetargetingNamedTypeSymbol._Closure$__.$I83-1 Is Nothing) Then
					func = Function(argument0)
						If (argument0.diag Is Nothing) Then
							Return argument0.t
						End If
						Return RetargetingNamedTypeSymbol.CyclicInheritanceError(argument0.diag)
					End Function
					RetargetingNamedTypeSymbol._Closure$__.$I83-1 = func
				Else
					func = RetargetingNamedTypeSymbol._Closure$__.$I83-1
				End If
				namedTypeSymbols = collection.[Select](func).AsImmutable()
			Else
				namedTypeSymbols = declaredInterfacesNoUseSiteDiagnostics
			End If
			Return namedTypeSymbols
		End Function

		Friend Overrides Function MakeDeclaredBase(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As NamedTypeSymbol
			Dim declaredBase As NamedTypeSymbol = Me._underlyingType.GetDeclaredBase(basesBeingResolved)
			If (declaredBase Is Nothing) Then
				Return Nothing
			End If
			Return Me.RetargetingTranslator.Retarget(declaredBase, RetargetOptions.RetargetPrimitiveTypesByName)
		End Function

		Friend Overrides Function MakeDeclaredInterfaces(ByVal basesBeingResolved As Microsoft.CodeAnalysis.VisualBasic.BasesBeingResolved, ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag) As ImmutableArray(Of NamedTypeSymbol)
			Dim declaredInterfacesNoUseSiteDiagnostics As ImmutableArray(Of NamedTypeSymbol) = Me._underlyingType.GetDeclaredInterfacesNoUseSiteDiagnostics(basesBeingResolved)
			Return Me.RetargetingTranslator.Retarget(declaredInterfacesNoUseSiteDiagnostics)
		End Function
	End Class
End Namespace