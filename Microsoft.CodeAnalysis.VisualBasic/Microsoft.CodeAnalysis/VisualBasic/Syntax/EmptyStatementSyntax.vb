Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class EmptyStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Public ReadOnly Property Empty As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax)._empty, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal empty As PunctuationSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.EmptyStatementSyntax(kind, errors, annotations, empty), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitEmptyStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitEmptyStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Return Nothing
		End Function

		Public Function Update(ByVal empty As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax
			Dim emptyStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax
			If (empty = Me.Empty) Then
				emptyStatementSyntax = Me
			Else
				Dim emptyStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.EmptyStatement(empty)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				emptyStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, emptyStatementSyntax1, emptyStatementSyntax1.WithAnnotations(annotations))
			End If
			Return emptyStatementSyntax
		End Function

		Public Function WithEmpty(ByVal empty As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.EmptyStatementSyntax
			Return Me.Update(empty)
		End Function
	End Class
End Namespace