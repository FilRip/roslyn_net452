Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable
Imports System.Globalization
Imports System.Threading

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols.Retargeting
	Friend NotInheritable Class RetargetingNamespaceSymbol
		Inherits NamespaceSymbol
		Private ReadOnly _retargetingModule As RetargetingModuleSymbol

		Private ReadOnly _underlyingNamespace As NamespaceSymbol

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
				Return Me.RetargetingTranslator.Retarget(Me._underlyingNamespace.ContainingSymbol)
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaredAccessibilityOfMostAccessibleDescendantType As Accessibility
			Get
				Return Me._underlyingNamespace.DeclaredAccessibilityOfMostAccessibleDescendantType
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaringCompilation As VisualBasicCompilation
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Me._underlyingNamespace.DeclaringSyntaxReferences
			End Get
		End Property

		Friend Overrides ReadOnly Property Extent As NamespaceExtent
			Get
				Return New NamespaceExtent(Me._retargetingModule)
			End Get
		End Property

		Public Overrides ReadOnly Property IsGlobalNamespace As Boolean
			Get
				Return Me._underlyingNamespace.IsGlobalNamespace
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._underlyingNamespace.Locations
			End Get
		End Property

		Public Overrides ReadOnly Property MetadataName As String
			Get
				Return Me._underlyingNamespace.MetadataName
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._underlyingNamespace.Name
			End Get
		End Property

		Private ReadOnly Property RetargetingTranslator As RetargetingModuleSymbol.RetargetingSymbolTranslator
			Get
				Return Me._retargetingModule.RetargetingTranslator
			End Get
		End Property

		Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
			Get
				Throw ExceptionUtilities.Unreachable
			End Get
		End Property

		Public ReadOnly Property UnderlyingNamespace As NamespaceSymbol
			Get
				Return Me._underlyingNamespace
			End Get
		End Property

		Public Sub New(ByVal retargetingModule As RetargetingModuleSymbol, ByVal underlyingNamespace As NamespaceSymbol)
			MyBase.New()
			If (TypeOf underlyingNamespace Is RetargetingNamespaceSymbol) Then
				Throw New ArgumentException()
			End If
			Me._retargetingModule = retargetingModule
			Me._underlyingNamespace = underlyingNamespace
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder)
			Me._underlyingNamespace.AddExtensionMethodLookupSymbolsInfo(nameSet, options, originalBinder, Me)
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamespaceSymbol)
			Throw ExceptionUtilities.Unreachable
		End Sub

		Friend Overrides Function AddExtensionMethodLookupSymbolsInfoViabilityCheck(ByVal method As MethodSymbol, ByVal options As LookupOptions, ByVal nameSet As LookupSymbolsInfo, ByVal originalBinder As Binder) As Boolean
			Return MyBase.AddExtensionMethodLookupSymbolsInfoViabilityCheck(Me.RetargetingTranslator.Retarget(method), options, nameSet, originalBinder)
		End Function

		Friend Overrides Sub AppendProbableExtensionMethods(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol))
			Dim count As Integer = methods.Count
			Me._underlyingNamespace.AppendProbableExtensionMethods(name, methods)
			Dim num As Integer = methods.Count - 1
			For i As Integer = count To num
				methods(i) = Me.RetargetingTranslator.Retarget(methods(i))
			Next

		End Sub

		Friend Overrides Sub BuildExtensionMethodsMap(ByVal map As Dictionary(Of String, ArrayBuilder(Of MethodSymbol)))
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = Me._underlyingNamespace.TypesToCheckForExtensionMethods.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.BuildExtensionMethodsMap(map, Me)
			End While
		End Sub

		Friend Overrides Sub BuildExtensionMethodsMapBucket(ByVal bucket As ArrayBuilder(Of MethodSymbol), ByVal method As MethodSymbol)
			bucket.Add(Me.RetargetingTranslator.Retarget(method))
		End Sub

		Protected Overrides Function GetDeclaredAccessibilityOfMostAccessibleDescendantType() As Accessibility
			Throw ExceptionUtilities.Unreachable
		End Function

		Public Overrides Function GetDocumentationCommentXml(Optional ByVal preferredCulture As CultureInfo = Nothing, Optional ByVal expandIncludes As Boolean = False, Optional ByVal cancellationToken As System.Threading.CancellationToken = Nothing) As String
			Return Me._underlyingNamespace.GetDocumentationCommentXml(preferredCulture, expandIncludes, cancellationToken)
		End Function

		Friend Overrides Sub GetExtensionMethods(ByVal methods As ArrayBuilder(Of MethodSymbol), ByVal name As String)
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = Me._underlyingNamespace.TypesToCheckForExtensionMethods.GetEnumerator()
			While enumerator.MoveNext()
				enumerator.Current.GetExtensionMethods(methods, Me, name)
			End While
		End Sub

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return Me.RetargetMembers(Me._underlyingNamespace.GetMembers())
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return Me.RetargetMembers(Me._underlyingNamespace.GetMembers(name))
		End Function

		Friend Overrides Function GetMembersUnordered() As ImmutableArray(Of Symbol)
			Return Me.RetargetMembers(Me._underlyingNamespace.GetMembersUnordered())
		End Function

		Public Overrides Function GetModuleMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingNamespace.GetModuleMembers())
		End Function

		Public Overrides Function GetModuleMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetingTranslator.Retarget(Me._underlyingNamespace.GetModuleMembers(name))
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetTypeMembers(Me._underlyingNamespace.GetTypeMembers())
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetTypeMembers(Me._underlyingNamespace.GetTypeMembers(name))
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetTypeMembers(Me._underlyingNamespace.GetTypeMembers(name, arity))
		End Function

		Friend Overrides Function GetTypeMembersUnordered() As ImmutableArray(Of NamedTypeSymbol)
			Return Me.RetargetTypeMembers(Me._underlyingNamespace.GetTypeMembersUnordered())
		End Function

		Friend Overrides Function LookupMetadataType(ByRef fullEmittedName As MetadataTypeName) As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim topLevel As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol
			Dim namedTypeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamedTypeSymbol = Me._underlyingNamespace.LookupMetadataType(fullEmittedName)
			If (namedTypeSymbol.IsErrorType() OrElse Not namedTypeSymbol.IsExplicitDefinitionOfNoPiaLocalType) Then
				topLevel = Me.RetargetingTranslator.Retarget(namedTypeSymbol, RetargetOptions.RetargetPrimitiveTypesByName)
			Else
				topLevel = New MissingMetadataTypeSymbol.TopLevel(Me._retargetingModule, fullEmittedName, SpecialType.System_Object Or SpecialType.System_Enum Or SpecialType.System_MulticastDelegate Or SpecialType.System_Delegate Or SpecialType.System_ValueType Or SpecialType.System_Void Or SpecialType.System_Boolean Or SpecialType.System_Char Or SpecialType.System_SByte Or SpecialType.System_Byte Or SpecialType.System_Int16 Or SpecialType.System_UInt16 Or SpecialType.System_Int32 Or SpecialType.System_UInt32 Or SpecialType.System_Int64 Or SpecialType.System_UInt64 Or SpecialType.System_Decimal Or SpecialType.System_Single Or SpecialType.System_Double Or SpecialType.System_String Or SpecialType.System_IntPtr Or SpecialType.System_UIntPtr Or SpecialType.System_Array Or SpecialType.System_Collections_IEnumerable Or SpecialType.System_Collections_Generic_IEnumerable_T Or SpecialType.System_Collections_Generic_IList_T Or SpecialType.System_Collections_Generic_ICollection_T Or SpecialType.System_Collections_IEnumerator Or SpecialType.System_Collections_Generic_IEnumerator_T Or SpecialType.System_Collections_Generic_IReadOnlyList_T Or SpecialType.System_Collections_Generic_IReadOnlyCollection_T Or SpecialType.System_Nullable_T Or SpecialType.System_DateTime Or SpecialType.System_Runtime_CompilerServices_IsVolatile Or SpecialType.System_IDisposable Or SpecialType.System_TypedReference Or SpecialType.System_ArgIterator Or SpecialType.System_RuntimeArgumentHandle Or SpecialType.System_RuntimeFieldHandle Or SpecialType.System_RuntimeMethodHandle Or SpecialType.System_RuntimeTypeHandle Or SpecialType.System_IAsyncResult Or SpecialType.System_AsyncCallback Or SpecialType.System_Runtime_CompilerServices_RuntimeFeature Or SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute Or SpecialType.Count)
			End If
			Return topLevel
		End Function

		Private Function RetargetMembers(ByVal underlyingMembers As ImmutableArray(Of Symbol)) As ImmutableArray(Of Symbol)
			Dim instance As ArrayBuilder(Of Symbol) = ArrayBuilder(Of Symbol).GetInstance()
			Dim enumerator As ImmutableArray(Of Symbol).Enumerator = underlyingMembers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Symbol = enumerator.Current
				If (current.Kind = SymbolKind.NamedType AndAlso DirectCast(current, NamedTypeSymbol).IsExplicitDefinitionOfNoPiaLocalType) Then
					Continue While
				End If
				instance.Add(Me.RetargetingTranslator.Retarget(current))
			End While
			Return instance.ToImmutableAndFree()
		End Function

		Private Function RetargetTypeMembers(ByVal underlyingMembers As ImmutableArray(Of NamedTypeSymbol)) As ImmutableArray(Of NamedTypeSymbol)
			Dim instance As ArrayBuilder(Of NamedTypeSymbol) = ArrayBuilder(Of NamedTypeSymbol).GetInstance()
			Dim enumerator As ImmutableArray(Of NamedTypeSymbol).Enumerator = underlyingMembers.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As NamedTypeSymbol = enumerator.Current
				If (current.IsExplicitDefinitionOfNoPiaLocalType) Then
					Continue While
				End If
				instance.Add(Me.RetargetingTranslator.Retarget(current, RetargetOptions.RetargetPrimitiveTypesByName))
			End While
			Return instance.ToImmutableAndFree()
		End Function
	End Class
End Namespace