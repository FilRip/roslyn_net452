Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class ConditionalAccessBinder
		Inherits Binder
		Private ReadOnly _conditionalAccess As ConditionalAccessExpressionSyntax

		Private ReadOnly _placeholder As BoundValuePlaceholderBase

		Public Sub New(ByVal containingBinder As Binder, ByVal conditionalAccess As ConditionalAccessExpressionSyntax, ByVal placeholder As BoundValuePlaceholderBase)
			MyBase.New(containingBinder)
			Me._conditionalAccess = conditionalAccess
			Me._placeholder = placeholder
		End Sub

		Protected Overrides Function TryGetConditionalAccessReceiver(ByVal node As ConditionalAccessExpressionSyntax) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			If (node <> Me._conditionalAccess) Then
				boundExpression = MyBase.TryGetConditionalAccessReceiver(node)
			Else
				boundExpression = Me._placeholder
			End If
			Return boundExpression
		End Function
	End Class
End Namespace