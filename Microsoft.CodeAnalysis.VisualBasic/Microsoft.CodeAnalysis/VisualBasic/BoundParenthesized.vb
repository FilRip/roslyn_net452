Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundParenthesized
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundExpression

		Public Overrides ReadOnly Property ConstantValueOpt As ConstantValue
			Get
				Return Me.Expression.ConstantValueOpt
			End Get
		End Property

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.Parenthesized, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitParenthesized(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundParenthesized
			Dim boundParenthesized As Microsoft.CodeAnalysis.VisualBasic.BoundParenthesized
			If (expression <> Me.Expression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundParenthesized1 As Microsoft.CodeAnalysis.VisualBasic.BoundParenthesized = New Microsoft.CodeAnalysis.VisualBasic.BoundParenthesized(MyBase.Syntax, expression, type, MyBase.HasErrors)
				boundParenthesized1.CopyAttributes(Me)
				boundParenthesized = boundParenthesized1
			Else
				boundParenthesized = Me
			End If
			Return boundParenthesized
		End Function
	End Class
End Namespace