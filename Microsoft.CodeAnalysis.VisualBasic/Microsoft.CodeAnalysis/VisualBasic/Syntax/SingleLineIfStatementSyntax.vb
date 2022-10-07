Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class SingleLineIfStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _statements As SyntaxNode

		Friend _elseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax

		Public ReadOnly Property Condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._condition, 1)
			End Get
		End Property

		Public ReadOnly Property ElseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax)(Me._elseClause, 4)
			End Get
		End Property

		Public ReadOnly Property IfKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax)._ifKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)
			Get
				Return New SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)(MyBase.GetRed(Me._statements, 3))
			End Get
		End Property

		Public ReadOnly Property ThenKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax)._thenKeyword, Me.GetChildPosition(2), MyBase.GetChildIndex(2))
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal ifKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal thenKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal statements As SyntaxNode, ByVal elseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineIfStatementSyntax(kind, errors, annotations, ifKeyword, DirectCast(condition.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), thenKeyword, If(statements IsNot Nothing, statements.Green, Nothing), If(elseClause IsNot Nothing, DirectCast(elseClause.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SingleLineElseClauseSyntax), Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitSingleLineIfStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitSingleLineIfStatement(Me)
		End Sub

		Public Function AddElseClauseStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.WithElseClause(If(Me.ElseClause IsNot Nothing, Me.ElseClause, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SingleLineElseClause()).AddStatements(items))
		End Function

		Public Function AddStatements(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.WithStatements(Me.Statements.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Select Case i
				Case 1
					syntaxNode = Me._condition
					Exit Select
				Case 2
				Label0:
					syntaxNode = Nothing
					Exit Select
				Case 3
					syntaxNode = Me._statements
					Exit Select
				Case 4
					syntaxNode = Me._elseClause
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim condition As SyntaxNode
			Select Case i
				Case 1
					condition = Me.Condition
					Exit Select
				Case 2
				Label0:
					condition = Nothing
					Exit Select
				Case 3
					condition = MyBase.GetRed(Me._statements, 3)
					Exit Select
				Case 4
					condition = Me.ElseClause
					Exit Select
				Case Else
					GoTo Label0
			End Select
			Return condition
		End Function

		Public Function Update(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal thenKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax), ByVal elseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Dim singleLineIfStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			If (ifKeyword <> Me.IfKeyword OrElse condition <> Me.Condition OrElse thenKeyword <> Me.ThenKeyword OrElse statements <> Me.Statements OrElse elseClause <> Me.ElseClause) Then
				Dim singleLineIfStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.SingleLineIfStatement(ifKeyword, condition, thenKeyword, statements, elseClause)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				singleLineIfStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, singleLineIfStatementSyntax1, singleLineIfStatementSyntax1.WithAnnotations(annotations))
			Else
				singleLineIfStatementSyntax = Me
			End If
			Return singleLineIfStatementSyntax
		End Function

		Public Function WithCondition(ByVal condition As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.Update(Me.IfKeyword, condition, Me.ThenKeyword, Me.Statements, Me.ElseClause)
		End Function

		Public Function WithElseClause(ByVal elseClause As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineElseClauseSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.Update(Me.IfKeyword, Me.Condition, Me.ThenKeyword, Me.Statements, elseClause)
		End Function

		Public Function WithIfKeyword(ByVal ifKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.Update(ifKeyword, Me.Condition, Me.ThenKeyword, Me.Statements, Me.ElseClause)
		End Function

		Public Function WithStatements(ByVal statements As SyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.Update(Me.IfKeyword, Me.Condition, Me.ThenKeyword, statements, Me.ElseClause)
		End Function

		Public Function WithThenKeyword(ByVal thenKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.SingleLineIfStatementSyntax
			Return Me.Update(Me.IfKeyword, Me.Condition, thenKeyword, Me.Statements, Me.ElseClause)
		End Function
	End Class
End Namespace