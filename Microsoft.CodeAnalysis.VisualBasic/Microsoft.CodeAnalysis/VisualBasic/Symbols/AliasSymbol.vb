Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable
Imports System.Linq

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class AliasSymbol
		Inherits Symbol
		Implements IAliasSymbol
		Private ReadOnly _aliasTarget As NamespaceOrTypeSymbol

		Private ReadOnly _aliasName As String

		Private ReadOnly _aliasLocations As ImmutableArray(Of Location)

		Private ReadOnly _aliasContainer As Symbol

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._aliasContainer
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Of SimpleImportsClauseSyntax)(Me.Locations)
			End Get
		End Property

		ReadOnly Property IAliasSymbol_Target As INamespaceOrTypeSymbol Implements IAliasSymbol.Target
			Get
				Return Me.Target
			End Get
		End Property

		Public Overrides ReadOnly Property IsMustOverride As Boolean
			Get
				Return False
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
				Return False
			End Get
		End Property

		Public Overrides ReadOnly Property Kind As SymbolKind
			Get
				Return SymbolKind.[Alias]
			End Get
		End Property

		Public Overrides ReadOnly Property Locations As ImmutableArray(Of Location)
			Get
				Return Me._aliasLocations
			End Get
		End Property

		Public Overrides ReadOnly Property Name As String
			Get
				Return Me._aliasName
			End Get
		End Property

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Public ReadOnly Property Target As NamespaceOrTypeSymbol
			Get
				Return Me._aliasTarget
			End Get
		End Property

		Friend Sub New(ByVal compilation As VisualBasicCompilation, ByVal aliasContainer As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal aliasName As String, ByVal aliasTarget As NamespaceOrTypeSymbol, ByVal aliasLocation As Location)
			MyBase.New()
			Dim mergedNamespaceSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceSymbol = TryCast(aliasContainer, Microsoft.CodeAnalysis.VisualBasic.Symbols.MergedNamespaceSymbol)
			Dim constituentForCompilation As NamespaceSymbol = Nothing
			If (mergedNamespaceSymbol IsNot Nothing) Then
				constituentForCompilation = mergedNamespaceSymbol.GetConstituentForCompilation(compilation)
			End If
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = constituentForCompilation
			If (symbol Is Nothing) Then
				symbol = aliasContainer
			End If
			Me._aliasContainer = symbol
			Me._aliasTarget = aliasTarget
			Me._aliasName = aliasName
			Me._aliasLocations = ImmutableArray.Create(Of Location)(aliasLocation)
		End Sub

		Friend Overrides Function Accept(Of TArg, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArg, TResult), ByVal a As TArg) As TResult
			Return visitor.VisitAlias(Me, a)
		End Function

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			visitor.VisitAlias(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitAlias(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			visitor.VisitAlias(Me)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Return visitor.VisitAlias(Me)
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj = Me) Then
				flag = True
			ElseIf (obj IsNot Nothing) Then
				Dim aliasSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.AliasSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.AliasSymbol)
				flag = If(aliasSymbol Is Nothing OrElse Not [Object].Equals(System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Location)(Me.Locations), System.Linq.ImmutableArrayExtensions.FirstOrDefault(Of Location)(aliasSymbol.Locations)), False, CObj(Me.ContainingAssembly) = CObj(aliasSymbol.ContainingAssembly))
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			If (Me.Locations.Length <= 0) Then
				Return Me.Name.GetHashCode()
			End If
			Return Me.Locations(0).GetHashCode()
		End Function
	End Class
End Namespace