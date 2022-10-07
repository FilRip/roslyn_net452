Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLateMemberAccess
		Inherits BoundExpression
		Private ReadOnly _NameOpt As String

		Private ReadOnly _ContainerTypeOpt As TypeSymbol

		Private ReadOnly _ReceiverOpt As BoundExpression

		Private ReadOnly _TypeArgumentsOpt As BoundTypeArguments

		Private ReadOnly _AccessKind As LateBoundAccessKind

		Public ReadOnly Property AccessKind As LateBoundAccessKind
			Get
				Return Me._AccessKind
			End Get
		End Property

		Public ReadOnly Property ContainerTypeOpt As TypeSymbol
			Get
				Return Me._ContainerTypeOpt
			End Get
		End Property

		Public ReadOnly Property NameOpt As String
			Get
				Return Me._NameOpt
			End Get
		End Property

		Public ReadOnly Property ReceiverOpt As BoundExpression
			Get
				Return Me._ReceiverOpt
			End Get
		End Property

		Public ReadOnly Property TypeArgumentsOpt As BoundTypeArguments
			Get
				Return Me._TypeArgumentsOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal nameOpt As String, ByVal containerTypeOpt As TypeSymbol, ByVal receiverOpt As BoundExpression, ByVal typeArgumentsOpt As BoundTypeArguments, ByVal accessKind As LateBoundAccessKind, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LateMemberAccess, syntax, type, If(hasErrors OrElse receiverOpt.NonNullAndHasErrors(), True, typeArgumentsOpt.NonNullAndHasErrors()))
			Me._NameOpt = nameOpt
			Me._ContainerTypeOpt = containerTypeOpt
			Me._ReceiverOpt = receiverOpt
			Me._TypeArgumentsOpt = typeArgumentsOpt
			Me._AccessKind = accessKind
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLateMemberAccess(Me)
		End Function

		Public Function SetAccessKind(ByVal newAccessKind As LateBoundAccessKind) As BoundLateMemberAccess
			Return Me.Update(Me.NameOpt, Me.ContainerTypeOpt, Me.ReceiverOpt, Me.TypeArgumentsOpt, newAccessKind, MyBase.Type)
		End Function

		Public Function Update(ByVal nameOpt As String, ByVal containerTypeOpt As TypeSymbol, ByVal receiverOpt As BoundExpression, ByVal typeArgumentsOpt As BoundTypeArguments, ByVal accessKind As LateBoundAccessKind, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess
			Dim boundLateMemberAccess As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess
			If (CObj(nameOpt) <> CObj(Me.NameOpt) OrElse CObj(containerTypeOpt) <> CObj(Me.ContainerTypeOpt) OrElse receiverOpt <> Me.ReceiverOpt OrElse typeArgumentsOpt <> Me.TypeArgumentsOpt OrElse accessKind <> Me.AccessKind OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLateMemberAccess1 As Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess = New Microsoft.CodeAnalysis.VisualBasic.BoundLateMemberAccess(MyBase.Syntax, nameOpt, containerTypeOpt, receiverOpt, typeArgumentsOpt, accessKind, type, MyBase.HasErrors)
				boundLateMemberAccess1.CopyAttributes(Me)
				boundLateMemberAccess = boundLateMemberAccess1
			Else
				boundLateMemberAccess = Me
			End If
			Return boundLateMemberAccess
		End Function
	End Class
End Namespace