Imports Microsoft.CodeAnalysis
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundSyncLockStatement
		Inherits BoundStatement
		Private ReadOnly _LockExpression As BoundExpression

		Private ReadOnly _Body As BoundBlock

		Public ReadOnly Property Body As BoundBlock
			Get
				Return Me._Body
			End Get
		End Property

		Public ReadOnly Property LockExpression As BoundExpression
			Get
				Return Me._LockExpression
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal lockExpression As BoundExpression, ByVal body As BoundBlock, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.SyncLockStatement, syntax, If(hasErrors OrElse lockExpression.NonNullAndHasErrors(), True, body.NonNullAndHasErrors()))
			Me._LockExpression = lockExpression
			Me._Body = body
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitSyncLockStatement(Me)
		End Function

		Public Function Update(ByVal lockExpression As BoundExpression, ByVal body As BoundBlock) As Microsoft.CodeAnalysis.VisualBasic.BoundSyncLockStatement
			Dim boundSyncLockStatement As Microsoft.CodeAnalysis.VisualBasic.BoundSyncLockStatement
			If (lockExpression <> Me.LockExpression OrElse body <> Me.Body) Then
				Dim boundSyncLockStatement1 As Microsoft.CodeAnalysis.VisualBasic.BoundSyncLockStatement = New Microsoft.CodeAnalysis.VisualBasic.BoundSyncLockStatement(MyBase.Syntax, lockExpression, body, MyBase.HasErrors)
				boundSyncLockStatement1.CopyAttributes(Me)
				boundSyncLockStatement = boundSyncLockStatement1
			Else
				boundSyncLockStatement = Me
			End If
			Return boundSyncLockStatement
		End Function
	End Class
End Namespace