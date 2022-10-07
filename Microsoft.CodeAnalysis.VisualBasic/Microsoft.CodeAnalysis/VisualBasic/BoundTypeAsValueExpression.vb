Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundTypeAsValueExpression
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundTypeExpression

		Public ReadOnly Property Expression As BoundTypeExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundTypeExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.TypeAsValueExpression, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitTypeAsValueExpression(Me)
		End Function

		Public Function Update(ByVal expression As BoundTypeExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundTypeAsValueExpression
			Dim boundTypeAsValueExpression As Microsoft.CodeAnalysis.VisualBasic.BoundTypeAsValueExpression
			If (expression <> Me.Expression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundTypeAsValueExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundTypeAsValueExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundTypeAsValueExpression(MyBase.Syntax, expression, type, MyBase.HasErrors)
				boundTypeAsValueExpression1.CopyAttributes(Me)
				boundTypeAsValueExpression = boundTypeAsValueExpression1
			Else
				boundTypeAsValueExpression = Me
			End If
			Return boundTypeAsValueExpression
		End Function
	End Class
End Namespace