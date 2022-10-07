Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SyncLockStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property SyncLockKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax)._syncLockKeyword, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal syncLockKeyword As KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyncLockStatementSyntax(kind, errors, annotations, syncLockKeyword, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSyncLockStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSyncLockStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._expression
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			If (i <> 1) Then
				expression = Nothing
			Else
				expression = Me.Expression
			End If
			Return expression
		End Function

		Public Function Update(ByVal syncLockKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax
			Dim syncLockStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax
			If (syncLockKeyword <> Me.SyncLockKeyword OrElse expression <> Me.Expression) Then
				Dim syncLockStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SyncLockStatement(syncLockKeyword, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				syncLockStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, syncLockStatementSyntax1, syncLockStatementSyntax1.WithAnnotations(annotations))
			Else
				syncLockStatementSyntax = Me
			End If
			Return syncLockStatementSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax
			Return Me.Update(Me.SyncLockKeyword, expression)
		End Function

		Public Function WithSyncLockKeyword(ByVal syncLockKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SyncLockStatementSyntax
			Return Me.Update(syncLockKeyword, Me.Expression)
		End Function
	End Class
End Namespace