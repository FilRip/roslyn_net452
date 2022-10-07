Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class IfStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._condition, 1)
			End Get
		End Property

		Public ReadOnly Property IfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)._ifKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property ThenKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Dim syntaxToken As Microsoft.CodeAnalysis.SyntaxToken
				Dim green As KeywordSyntax = DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax)._thenKeyword
				syntaxToken = If(green Is Nothing, New Microsoft.CodeAnalysis.SyntaxToken(), New Microsoft.CodeAnalysis.SyntaxToken(Me, green, Me.GetChildPosition(2), MyBase.GetChildIndex(2)))
				Return syntaxToken
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifKeyword As KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal thenKeyword As KeywordSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.IfStatementSyntax(kind, errors, annotations, ifKeyword, DirectCast(condition.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), thenKeyword), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitIfStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitIfStatement(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			If (i <> 1) Then
				syntaxNode = Nothing
			Else
				syntaxNode = Me._condition
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim condition As SyntaxNode
			If (i <> 1) Then
				condition = Nothing
			Else
				condition = Me.Condition
			End If
			Return condition
		End Function

		Public Function Update(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal thenKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax
			Dim ifStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax
			If (ifKeyword <> Me.IfKeyword OrElse condition <> Me.Condition OrElse thenKeyword <> Me.ThenKeyword) Then
				Dim ifStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.IfStatement(ifKeyword, condition, thenKeyword)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				ifStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, ifStatementSyntax1, ifStatementSyntax1.WithAnnotations(annotations))
			Else
				ifStatementSyntax = Me
			End If
			Return ifStatementSyntax
		End Function

		Public Function WithCondition(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax
			Return Me.Update(Me.IfKeyword, condition, Me.ThenKeyword)
		End Function

		Public Function WithIfKeyword(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax
			Return Me.Update(ifKeyword, Me.Condition, Me.ThenKeyword)
		End Function

		Public Function WithThenKeyword(ByVal thenKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.IfStatementSyntax
			Return Me.Update(Me.IfKeyword, Me.Condition, thenKeyword)
		End Function
	End Class
End Namespace