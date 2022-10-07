Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundArrayLength
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundExpression

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ArrayLength, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitArrayLength(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength
			Dim boundArrayLength As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength
			If (expression <> Me.Expression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundArrayLength1 As Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength = New Microsoft.CodeAnalysis.VisualBasic.BoundArrayLength(MyBase.Syntax, expression, type, MyBase.HasErrors)
				boundArrayLength1.CopyAttributes(Me)
				boundArrayLength = boundArrayLength1
			Else
				boundArrayLength = Me
			End If
			Return boundArrayLength
		End Function
	End Class
End Namespace