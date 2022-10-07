Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundEventAccess
		Inherits BoundExpression
		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _EventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol

		Public ReadOnly Property EventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol
			Get
				Return Me._EventSymbol
			End Get
		End Property

		Public Overrides ReadOnly Property ExpressionSymbol As Symbol
			Get
				Return Me.EventSymbol
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal receiverOpt As BoundExpression, ByVal eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.EventAccess, syntax, type, If(hasErrors, True, receiverOpt.NonNullAndHasErrors()))
			Me._ReceiverOpt = receiverOpt
			Me._EventSymbol = eventSymbol
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitEventAccess(Me)
		End Function

		Public Function Update(ByVal receiverOpt As BoundExpression, ByVal eventSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.EventSymbol, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess
			Dim boundEventAccess As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess
			If (receiverOpt <> Me.ReceiverOpt OrElse CObj(eventSymbol) <> CObj(Me.EventSymbol) OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundEventAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundEventAccess(MyBase.Syntax, receiverOpt, eventSymbol, type, MyBase.HasErrors)
				boundEventAccess1.CopyAttributes(Me)
				boundEventAccess = boundEventAccess1
			Else
				boundEventAccess = Me
			End If
			Return boundEventAccess
		End Function
	End Class
End Namespace