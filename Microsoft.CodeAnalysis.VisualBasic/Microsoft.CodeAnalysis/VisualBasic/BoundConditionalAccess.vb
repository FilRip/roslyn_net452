Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundConditionalAccess
		Inherits BoundExpression
		Private ReadOnly _Receiver As BoundExpression

		Private ReadOnly _Placeholder As BoundRValuePlaceholder

		Private ReadOnly _AccessExpression As BoundExpression

		Public ReadOnly Property AccessExpression As BoundExpression
			Get
				Return Me._AccessExpression
			End Get
		End Property

		Public ReadOnly Property Placeholder As BoundRValuePlaceholder
			Get
				Return Me._Placeholder
			End Get
		End Property

		Public ReadOnly Property Receiver As BoundExpression
			Get
				Return Me._Receiver
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal receiver As BoundExpression, ByVal placeholder As BoundRValuePlaceholder, ByVal accessExpression As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.ConditionalAccess, syntax, type, If(hasErrors OrElse receiver.NonNullAndHasErrors() OrElse placeholder.NonNullAndHasErrors(), True, accessExpression.NonNullAndHasErrors()))
			Me._Receiver = receiver
			Me._Placeholder = placeholder
			Me._AccessExpression = accessExpression
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitConditionalAccess(Me)
		End Function

		Public Function Update(ByVal receiver As BoundExpression, ByVal placeholder As BoundRValuePlaceholder, ByVal accessExpression As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccess
			Dim boundConditionalAccess As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccess
			If (receiver <> Me.Receiver OrElse placeholder <> Me.Placeholder OrElse accessExpression <> Me.AccessExpression OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundConditionalAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundConditionalAccess(MyBase.Syntax, receiver, placeholder, accessExpression, type, MyBase.HasErrors)
				boundConditionalAccess1.CopyAttributes(Me)
				boundConditionalAccess = boundConditionalAccess1
			Else
				boundConditionalAccess = Me
			End If
			Return boundConditionalAccess
		End Function
	End Class
End Namespace