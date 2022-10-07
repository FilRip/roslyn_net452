Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class ExpressionRangeVariableSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax

		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression, 1)
			End Get
		End Property

		Public ReadOnly Property NameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax)(Me._nameEquals)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As Microsoft.CodeAnalysis.VisualBasic.SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionRangeVariableSyntax(kind, errors, annotations, If(nameEquals IsNot Nothing, DirectCast(nameEquals.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VariableNameEqualsSyntax), Nothing), DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitExpressionRangeVariable(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitExpressionRangeVariable(Me)
		End Sub

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._nameEquals
			ElseIf (num = 1) Then
				syntaxNode = Me._expression
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim nameEquals As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				nameEquals = Me.NameEquals
			ElseIf (num = 1) Then
				nameEquals = Me.Expression
			Else
				nameEquals = Nothing
			End If
			Return nameEquals
		End Function

		Public Function Update(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax, ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax
			Dim expressionRangeVariableSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax
			If (nameEquals <> Me.NameEquals OrElse expression <> Me.Expression) Then
				Dim expressionRangeVariableSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ExpressionRangeVariable(nameEquals, expression)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				expressionRangeVariableSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, expressionRangeVariableSyntax1, expressionRangeVariableSyntax1.WithAnnotations(annotations))
			Else
				expressionRangeVariableSyntax = Me
			End If
			Return expressionRangeVariableSyntax
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax
			Return Me.Update(Me.NameEquals, expression)
		End Function

		Public Function WithNameEquals(ByVal nameEquals As Microsoft.CodeAnalysis.VisualBasic.Syntax.VariableNameEqualsSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax
			Return Me.Update(nameEquals, Me.Expression)
		End Function
	End Class
End Namespace