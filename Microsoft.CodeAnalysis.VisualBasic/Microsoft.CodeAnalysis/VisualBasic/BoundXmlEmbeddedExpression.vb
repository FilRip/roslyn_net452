Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundXmlEmbeddedExpression
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundExpression

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.XmlEmbeddedExpression, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitXmlEmbeddedExpression(Me)
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundXmlEmbeddedExpression
			Dim boundXmlEmbeddedExpression As Microsoft.CodeAnalysis.VisualBasic.BoundXmlEmbeddedExpression
			If (expression <> Me.Expression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundXmlEmbeddedExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundXmlEmbeddedExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundXmlEmbeddedExpression(MyBase.Syntax, expression, type, MyBase.HasErrors)
				boundXmlEmbeddedExpression1.CopyAttributes(Me)
				boundXmlEmbeddedExpression = boundXmlEmbeddedExpression1
			Else
				boundXmlEmbeddedExpression = Me
			End If
			Return boundXmlEmbeddedExpression
		End Function
	End Class
End Namespace