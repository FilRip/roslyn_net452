Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class PreprocessingSymbol
		Inherits Symbol
		Implements IPreprocessingSymbol
		Private ReadOnly _name As String

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Nothing
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaredAccessibility As Accessibility
			Get
				Return Accessibility.NotApplicable
			End Get
		End Property

		Public Overrides ReadOnly Property DeclaringSyntaxReferences As ImmutableArray(Of SyntaxReference)
			Get
				Return Symbol.GetDeclaringSyntaxReferenceHelper(Of VisualBasicSyntaxNode)(Me.Locations)
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
				Return SymbolKind.Preprocessing
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

		Friend Overrides ReadOnly Property ObsoleteAttributeData As Microsoft.CodeAnalysis.ObsoleteAttributeData
			Get
				Return Nothing
			End Get
		End Property

		Friend Sub New(ByVal name As String)
			MyBase.New()
			Me._name = name
		End Sub

		Public Overrides Sub Accept(ByVal visitor As SymbolVisitor)
			Throw New NotSupportedException()
		End Sub

		Public Overrides Sub Accept(ByVal visitor As VisualBasicSymbolVisitor)
			Throw New NotSupportedException()
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As SymbolVisitor(Of TResult)) As TResult
			Throw New NotSupportedException()
		End Function

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TResult)) As TResult
			Throw New NotSupportedException()
		End Function

		Friend Overrides Function Accept(Of TArgument, TResult)(ByVal visitor As VisualBasicSymbolVisitor(Of TArgument, TResult), ByVal arg As TArgument) As TResult
			Throw New NotSupportedException()
		End Function

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj = Me) Then
				flag = True
			ElseIf (obj IsNot Nothing) Then
				Dim preprocessingSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.PreprocessingSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.PreprocessingSymbol)
				flag = If(preprocessingSymbol Is Nothing, False, CaseInsensitiveComparison.Equals(Me.Name, preprocessingSymbol.Name))
			Else
				flag = False
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Me.Name.GetHashCode()
		End Function
	End Class
End Namespace