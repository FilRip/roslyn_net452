Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSequencePointExpression
		Inherits BoundExpression
		Private ReadOnly _Expression As BoundExpression

		Public ReadOnly Property Expression As BoundExpression
			Get
				Return Me._Expression
			End Get
		End Property

		Public Overrides ReadOnly Property IsLValue As Boolean
			Get
				Return Me.Expression.IsLValue
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal expression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SequencePointExpression, syntax, type, If(hasErrors, True, expression.NonNullAndHasErrors()))
			Me._Expression = expression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSequencePointExpression(Me)
		End Function

		Public Shadows Function MakeRValue() As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression
			Dim boundSequencePointExpression As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression
			boundSequencePointExpression = If(Not Me.Expression.IsLValue, Me, Me.Update(Me.Expression.MakeRValue(), MyBase.Type))
			Return boundSequencePointExpression
		End Function

		Protected Overrides Function MakeRValueImpl() As BoundExpression
			Return Me.MakeRValue()
		End Function

		Public Function Update(ByVal expression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression
			Dim boundSequencePointExpression As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression
			If (expression <> Me.Expression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundSequencePointExpression1 As Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression = New Microsoft.CodeAnalysis.VisualBasic.BoundSequencePointExpression(MyBase.Syntax, expression, type, MyBase.HasErrors)
				boundSequencePointExpression1.CopyAttributes(Me)
				boundSequencePointExpression = boundSequencePointExpression1
			Else
				boundSequencePointExpression = Me
			End If
			Return boundSequencePointExpression
		End Function
	End Class
End Namespace