Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
	Friend NotInheritable Class BoundLambdaParameterSymbol
		Inherits LambdaParameterSymbol
		Private _lambdaSymbol As LambdaSymbol

		Private ReadOnly _syntaxNode As SyntaxNode

		Public Overrides ReadOnly Property ContainingSymbol As Symbol
			Get
				Return Me._lambdaSymbol
			End Get
		End Property

		Public ReadOnly Property Syntax As SyntaxNode
			Get
				Return Me._syntaxNode
			End Get
		End Property

		Public Sub New(ByVal name As String, ByVal ordinal As Integer, ByVal type As TypeSymbol, ByVal isByRef As Boolean, ByVal syntaxNode As Microsoft.CodeAnalysis.SyntaxNode, ByVal location As Microsoft.CodeAnalysis.Location)
			MyBase.New(name, ordinal, type, isByRef, location)
			Me._syntaxNode = syntaxNode
		End Sub

		Public Overrides Function Equals(ByVal obj As Object) As Boolean
			Dim flag As Boolean
			If (obj <> Me) Then
				Dim boundLambdaParameterSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.BoundLambdaParameterSymbol = TryCast(obj, Microsoft.CodeAnalysis.VisualBasic.Symbols.BoundLambdaParameterSymbol)
				flag = If(boundLambdaParameterSymbol Is Nothing OrElse Not [Object].Equals(boundLambdaParameterSymbol._lambdaSymbol, Me._lambdaSymbol), False, boundLambdaParameterSymbol.Ordinal = MyBase.Ordinal)
			Else
				flag = True
			End If
			Return flag
		End Function

		Public Overrides Function GetHashCode() As Integer
			Return Hash.Combine(Me._lambdaSymbol.GetHashCode(), MyBase.Ordinal)
		End Function

		Public Sub SetLambdaSymbol(ByVal lambda As LambdaSymbol)
			Me._lambdaSymbol = lambda
		End Sub
	End Class
End Namespace