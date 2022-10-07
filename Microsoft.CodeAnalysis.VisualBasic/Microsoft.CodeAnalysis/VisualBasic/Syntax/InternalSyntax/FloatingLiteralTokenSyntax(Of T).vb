Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class FloatingLiteralTokenSyntax(Of T)
		Inherits FloatingLiteralTokenSyntax
		Friend ReadOnly _value As T

		Friend Overrides ReadOnly Property ObjectValue As Object
			Get
				Return Me.Value
			End Get
		End Property

		Friend ReadOnly Property Value As T
			Get
				Return Me._value
			End Get
		End Property

		Friend Overrides ReadOnly Property ValueText As String
			Get
				Return Me._value.ToString()
			End Get
		End Property

		Shared Sub New()
			ObjectBinder.RegisterTypeReader(GetType(FloatingLiteralTokenSyntax(Of T)), Function(r As ObjectReader) New FloatingLiteralTokenSyntax(Of T)(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal typeSuffix As TypeCharacter, ByVal value As T)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia, typeSuffix)
			Me._value = value
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal typeSuffix As TypeCharacter, ByVal value As T)
			MyBase.New(kind, errors, annotations, text, leadingTrivia, trailingTrivia, typeSuffix)
			Me._value = value
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
			Me._value = Microsoft.VisualBasic.CompilerServices.Conversions.ToGenericParameter(Of T)(reader.ReadValue())
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New FloatingLiteralTokenSyntax(Of T)(MyBase.Kind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me._typeSuffix, Me._value)
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New FloatingLiteralTokenSyntax(Of T)(MyBase.Kind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia(), Me._typeSuffix, Me._value)
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New FloatingLiteralTokenSyntax(Of T)(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia(), Me._typeSuffix, Me._value)
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New FloatingLiteralTokenSyntax(Of T)(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia, Me._typeSuffix, Me._value)
		End Function

		Friend Overrides Sub WriteTo(ByVal writer As ObjectWriter)
			MyBase.WriteTo(writer)
			writer.WriteValue(Me._value)
		End Sub
	End Class
End Namespace