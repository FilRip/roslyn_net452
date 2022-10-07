Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend Class MissingNamespaceSymbol
		Inherits NamespaceSymbol
		Private ReadOnly _name As String

		Private ReadOnly _containingSymbol As Symbol

		Public Overrides ReadOnly Property ContainingAssembly As AssemblySymbol
			Get
				Return Me._containingSymbol.ContainingAssembly
			End Get
		End Property

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._containingSymbol
			End Get
		End Property

		Friend Overrides ReadOnly Property DeclaredAccessibilityOfMostAccessibleDescendantType As Accessibility
			Get
				Return Accessibility.[Private]
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return ImmutableArray(Of SyntaxReference).Empty
			End Get
		End Property

		Friend Overrides ReadOnly Property Extent As NamespaceExtent
			Get
				Dim namespaceExtent As Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceExtent
				namespaceExtent = If(Me._containingSymbol.Kind <> SymbolKind.NetModule, DirectCast(Me._containingSymbol, NamespaceSymbol).Extent, New Microsoft.CodeAnalysis.VisualBasic.Symbols.NamespaceExtent(DirectCast(Me._containingSymbol, ModuleSymbol)))
				Return namespaceExtent
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return ImmutableArray(Of Location).Empty
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._name
			End Get
		End Property

		Friend Overrides ReadOnly Property TypesToCheckForExtensionMethods As ImmutableArray(Of NamedTypeSymbol)
			Get
				Return ImmutableArray(Of NamedTypeSymbol).Empty
			End Get
		End Property

		Public Sub New(ByVal containingModule As MissingModuleSymbol)
			MyBase.New()
			Me._containingSymbol = containingModule
			Me._name = [String].Empty
		End Sub

		Public Sub New(ByVal containingNamespace As NamespaceSymbol, ByVal name As String)
			MyBase.New()
			Me._containingSymbol = containingNamespace
			Me._name = name
		End Sub

		Friend Overrides Sub AddExtensionMethodLookupSymbolsInfo(ByVal nameSet As LookupSymbolsInfo, ByVal options As LookupOptions, ByVal originalBinder As Binder, ByVal appendThrough As NamespaceSymbol)
		End Sub

		Friend Overrides Sub AppendProbableExtensionMethods(ByVal name As String, ByVal methods As ArrayBuilder(Of MethodSymbol))
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (Me <> obj) Then
				Dim missingNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingNamespaceSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.MissingNamespaceSymbol)
				flag = If(missingNamespaceSymbol Is Nothing OrElse Not [String].Equals(Me._name, missingNamespaceSymbol._name, StringComparison.Ordinal), False, Me._containingSymbol.Equals(missingNamespaceSymbol._containingSymbol))
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me._containingSymbol.GetHashCode(), Me._name.GetHashCode())
		End Function

		Public Overrides Function GetMembers() As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public Overrides Function GetMembers(ByVal name As String) As ImmutableArray(Of Symbol)
			Return ImmutableArray(Of Symbol).Empty
		End Function

		Public Overrides Function GetModuleMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetModuleMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers() As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function

		Public Overrides Function GetTypeMembers(ByVal name As String, ByVal arity As Integer) As ImmutableArray(Of NamedTypeSymbol)
			Return ImmutableArray(Of NamedTypeSymbol).Empty
		End Function
	End Class
End Namespace