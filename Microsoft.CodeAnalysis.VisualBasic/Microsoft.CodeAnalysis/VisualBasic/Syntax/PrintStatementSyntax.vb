Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class PrintStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.ExecutableStatementSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property QuestionToken As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax)._questionToken, MyBase.Position, 0)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal questionToken As PunctuationSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.PrintStatementSyntax(kind, errors, annotations, questionToken, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitPrintStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitPrintStatement(Me)
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

		Public Function Update(ByVal questionToken As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax
			Dim printStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax
			If (questionToken <> Me.QuestionToken OrElse expression <> Me.Expression) Then
				Dim printStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.PrintStatement(questionToken, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				printStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, printStatementSyntax1, printStatementSyntax1.WithAnnotations(annotations))
			Else
				printStatementSyntax = Me
			End If
			Return printStatementSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax
			Return Me.Update(Me.QuestionToken, expression)
		End Function

		Public Function WithQuestionToken(ByVal questionToken As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.PrintStatementSyntax
			Return Me.Update(questionToken, Me.Expression)
		End Function
	End Class
End Namespace