Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Roslyn.Utilities
Imports System
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
	Friend NotInheritable Class KeywordSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken
		Friend Shared CreateInstance As Func(Of ObjectReader, Object)

		Friend NotOverridable Overrides ReadOnly Property IsKeyword As Boolean
			Get
				Return True
			End Get
		End Property

		Friend NotOverridable Overrides ReadOnly Property ObjectValue As Object
			Get
				Dim boxedFalse As Object
				Dim kind As SyntaxKind = MyBase.Kind
				If (kind = SyntaxKind.FalseKeyword) Then
					boxedFalse = Boxes.BoxedFalse
				ElseIf (kind = SyntaxKind.NothingKeyword) Then
					boxedFalse = Nothing
				ElseIf (kind = SyntaxKind.TrueKeyword) Then
					boxedFalse = Boxes.BoxedTrue
				Else
					boxedFalse = MyBase.Text
				End If
				Return boxedFalse
			End Get
		End Property

		Shared Sub New()
			KeywordSyntax.CreateInstance = Function(o As ObjectReader) New KeywordSyntax(o)
			ObjectBinder.RegisterTypeReader(GetType(KeywordSyntax), Function(r As ObjectReader) New KeywordSyntax(r))
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode, ByVal context As ISyntaxFactoryContext)
			MyBase.New(kind, text, leadingTrivia, trailingTrivia)
			MyBase.SetFactoryContext(context)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal text As String, ByVal leadingTrivia As GreenNode, ByVal trailingTrivia As GreenNode)
			MyBase.New(kind, errors, annotations, text, leadingTrivia, trailingTrivia)
		End Sub

		Friend Sub New(ByVal reader As ObjectReader)
			MyBase.New(reader)
		End Sub

		Friend Overrides Function SetAnnotations(ByVal annotations As SyntaxAnnotation()) As GreenNode
			Return New KeywordSyntax(MyBase.Kind, MyBase.GetDiagnostics(), annotations, MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia())
		End Function

		Friend Overrides Function SetDiagnostics(ByVal newErrors As DiagnosticInfo()) As GreenNode
			Return New KeywordSyntax(MyBase.Kind, newErrors, MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), MyBase.GetTrailingTrivia())
		End Function

		Public Overrides Function WithLeadingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New KeywordSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, trivia, MyBase.GetTrailingTrivia())
		End Function

		Public Overrides Function WithTrailingTrivia(ByVal trivia As GreenNode) As GreenNode
			Return New KeywordSyntax(MyBase.Kind, MyBase.GetDiagnostics(), MyBase.GetAnnotations(), MyBase.Text, MyBase.GetLeadingTrivia(), trivia)
		End Function
	End Class
End Namespace