Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class TryStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Public ReadOnly Property TryKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax)._tryKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal tryKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.TryStatementSyntax(kind, errors, annotations, tryKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitTryStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitTryStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal tryKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax
			Dim tryStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax
			If (tryKeyword = Me.TryKeyword) Then
				tryStatementSyntax = Me
			Else
				Dim tryStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.TryStatement(tryKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				tryStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, tryStatementSyntax1, tryStatementSyntax1.WithAnnotations(annotations))
			End If
			Return tryStatementSyntax
		End Function

		Public Function WithTryKeyword(ByVal tryKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.TryStatementSyntax
			Return Me.Update(tryKeyword)
		End Function
	End Class
End Namespace