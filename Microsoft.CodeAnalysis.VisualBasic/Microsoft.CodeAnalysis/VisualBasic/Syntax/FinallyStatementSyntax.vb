Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class FinallyStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Public ReadOnly Property FinallyKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax)._finallyKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal finallyKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.FinallyStatementSyntax(kind, errors, annotations, finallyKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitFinallyStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitFinallyStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal finallyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax
			Dim finallyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax
			If (finallyKeyword = Me.FinallyKeyword) Then
				finallyStatementSyntax = Me
			Else
				Dim finallyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.FinallyStatement(finallyKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				finallyStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, finallyStatementSyntax1, finallyStatementSyntax1.WithAnnotations(annotations))
			End If
			Return finallyStatementSyntax
		End Function

		Public Function WithFinallyKeyword(ByVal finallyKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.FinallyStatementSyntax
			Return Me.Update(finallyKeyword)
		End Function
	End Class
End Namespace