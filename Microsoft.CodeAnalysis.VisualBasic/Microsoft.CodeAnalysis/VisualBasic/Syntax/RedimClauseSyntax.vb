Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
	Public NotInheritable Class RedimClauseSyntax
		Inherits Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode
		Friend _expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax

		Friend _arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax

		Public ReadOnly Property ArrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax
			Get
				Return MyBase.GetRed(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)(Me._arrayBounds, 1)
			End Get
		End Property

		Public ReadOnly Property Expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax
			Get
				Return MyBase.GetRedAtZero(Of Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax)(Me._expression)
			End Get
		End Property

		Friend Sub New(ByVal green As GreenNode, ByVal parent As SyntaxNode, ByVal startLocation As Integer)
			MyBase.New(green, parent, startLocation)
		End Sub

		Friend Sub New(ByVal kind As SyntaxKind, ByVal errors As DiagnosticInfo(), ByVal annotations As SyntaxAnnotation(), ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax)
			MyClass.New(New Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.RedimClauseSyntax(kind, errors, annotations, DirectCast(expression.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ExpressionSyntax), DirectCast(arrayBounds.Green, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.ArgumentListSyntax)), Nothing, 0)
		End Sub

		Public Overrides Function Accept(Of TResult)(ByVal visitor As VisualBasicSyntaxVisitor(Of TResult)) As TResult
			Return visitor.VisitRedimClause(Me)
		End Function

		Public Overrides Sub Accept(ByVal visitor As Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxVisitor)
			visitor.VisitRedimClause(Me)
		End Sub

		Public Function AddArrayBoundsArguments(ByVal ParamArray items As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentSyntax()) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax
			Return Me.WithArrayBounds(If(Me.ArrayBounds IsNot Nothing, Me.ArrayBounds, Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.ArgumentList()).AddArguments(items))
		End Function

		Friend Overrides Function GetCachedSlot(ByVal i As Integer) As Microsoft.CodeAnalysis.SyntaxNode
			Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				syntaxNode = Me._expression
			ElseIf (num = 1) Then
				syntaxNode = Me._arrayBounds
			Else
				syntaxNode = Nothing
			End If
			Return syntaxNode
		End Function

		Friend Overrides Function GetNodeSlot(ByVal i As Integer) As SyntaxNode
			Dim expression As SyntaxNode
			Dim num As Integer = i
			If (num = 0) Then
				expression = Me.Expression
			ElseIf (num = 1) Then
				expression = Me.ArrayBounds
			Else
				expression = Nothing
			End If
			Return expression
		End Function

		Public Function Update(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax, ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax
			Dim redimClauseSyntax As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax
			If (expression <> Me.Expression OrElse arrayBounds <> Me.ArrayBounds) Then
				Dim redimClauseSyntax1 As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax = Microsoft.CodeAnalysis.VisualBasic.SyntaxFactory.RedimClause(expression, arrayBounds)
				Dim annotations As SyntaxAnnotation() = MyBase.GetAnnotations()
				redimClauseSyntax = If(annotations Is Nothing OrElse CInt(annotations.Length) <= 0, redimClauseSyntax1, redimClauseSyntax1.WithAnnotations(annotations))
			Else
				redimClauseSyntax = Me
			End If
			Return redimClauseSyntax
		End Function

		Public Function WithArrayBounds(ByVal arrayBounds As Microsoft.CodeAnalysis.VisualBasic.Syntax.ArgumentListSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax
			Return Me.Update(Me.Expression, arrayBounds)
		End Function

		Public Function WithExpression(ByVal expression As Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.Syntax.RedimClauseSyntax
			Return Me.Update(expression, Me.ArrayBounds)
		End Function
	End Class
End Namespace