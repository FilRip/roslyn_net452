Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class UsingStatementSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _variables As SyntaxNode

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property UsingKeyword As Microsoft.CodeAnalysis.SyntaxToken
			Get
				Return New Microsoft.CodeAnalysis.SyntaxToken(Me, DirectCast(MyBase.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax)._usingKeyword, MyBase.Position, 0)
			End Get
		End Property

		Public ReadOnly Property Variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
			Get
				Dim variableDeclaratorSyntaxes As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)
				Dim red As SyntaxNode = MyBase.GetRed(Me._variables, 2)
				variableDeclaratorSyntaxes = If(red Is Nothing, New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(), New SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)(red, MyBase.GetChildIndex(2)))
				Return variableDeclaratorSyntaxes
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal usingKeyword As Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.KeywordSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal variables As SyntaxNode)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.UsingStatementSyntax(kind, errors, annotations, usingKeyword, If(expression IsNot Nothing, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), Nothing), If(variables IsNot Nothing, variables.Green, Nothing)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitUsingStatement(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitUsingStatement(Me)
		End Sub

		Public Function AddVariables(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			Return Me.WithVariables(Me.Variables.AddRange(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				syntaxNode = Me._expression
			ElseIf (num = 2) Then
				syntaxNode = Me._variables
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			Dim num As Integer = i
			If (num = 1) Then
				expression = Me.Expression
			ElseIf (num = 2) Then
				expression = MyBase.GetRed(Me._variables, 2)
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Public Function Update(ByVal usingKeyword As Microsoft.CodeAnalysis.SyntaxToken, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			Dim usingStatementSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			If (usingKeyword <> Me.UsingKeyword OrElse expression <> Me.Expression OrElse variables <> Me.Variables) Then
				Dim usingStatementSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.UsingStatement(usingKeyword, expression, variables)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				usingStatementSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, usingStatementSyntax1, usingStatementSyntax1.WithAnnotations(annotations))
			Else
				usingStatementSyntax = Me
			End If
			Return usingStatementSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			Return Me.Update(Me.UsingKeyword, expression, Me.Variables)
		End Function

		Public Function WithUsingKeyword(ByVal usingKeyword As Microsoft.CodeAnalysis.SyntaxToken) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			Return Me.Update(usingKeyword, Me.Expression, Me.Variables)
		End Function

		Public Function WithVariables(ByVal variables As SeparatedSyntaxList(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableDeclaratorSyntax)) As Microsoft.CodeAnalysis.VisualBasic.Syntax.UsingStatementSyntax
			Return Me.Update(Me.UsingKeyword, Me.Expression, variables)
		End Function
	End Class
End Namespace