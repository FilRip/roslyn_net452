Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundLateInvocation
		Inherits BoundExpression
		Private ReadOnly _Member As BoundExpression

		Private ReadOnly _ArgumentsOpt As ImmutableArray(Of BoundExpression)

		Private ReadOnly _ArgumentNamesOpt As ImmutableArray(Of String)

		Private ReadOnly _AccessKind As LateBoundAccessKind

		Private ReadOnly _MethodOrPropertyGroupOpt As BoundMethodOrPropertyGroup

		Public ReadOnly Property AccessKind As LateBoundAccessKind
			Get
				Return Me._AccessKind
			End Get
		End Property

		Public ReadOnly Property ArgumentNamesOpt As ImmutableArray(Of String)
			Get
				Return Me._ArgumentNamesOpt
			End Get
		End Property

		Public ReadOnly Property ArgumentsOpt As ImmutableArray(Of BoundExpression)
			Get
				Return Me._ArgumentsOpt
			End Get
		End Property

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Dim argumentsOpt As ImmutableArray(Of BoundExpression) = Me.ArgumentsOpt
				Return StaticCast(Of BoundNode).From(Of BoundExpression)(argumentsOpt.Insert(0, Me.Member))
			End Get
		End Property

		Public ReadOnly Property Member As BoundExpression
			Get
				Return Me._Member
			End Get
		End Property

		Public ReadOnly Property MethodOrPropertyGroupOpt As BoundMethodOrPropertyGroup
			Get
				Return Me._MethodOrPropertyGroupOpt
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal member As BoundExpression, ByVal argumentsOpt As ImmutableArray(Of BoundExpression), ByVal argumentNamesOpt As ImmutableArray(Of String), ByVal accessKind As LateBoundAccessKind, ByVal methodOrPropertyGroupOpt As BoundMethodOrPropertyGroup, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.LateInvocation, syntax, type, If(hasErrors OrElse member.NonNullAndHasErrors() OrElse argumentsOpt.NonNullAndHasErrors(), True, methodOrPropertyGroupOpt.NonNullAndHasErrors()))
			Me._Member = member
			Me._ArgumentsOpt = argumentsOpt
			Me._ArgumentNamesOpt = argumentNamesOpt
			Me._AccessKind = accessKind
			Me._MethodOrPropertyGroupOpt = methodOrPropertyGroupOpt
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitLateInvocation(Me)
		End Function

		Public Function SetAccessKind(ByVal newAccessKind As LateBoundAccessKind) As BoundLateInvocation
			Dim member As BoundExpression = Me.Member
			If (member.Kind = BoundKind.LateMemberAccess) Then
				member = DirectCast(member, BoundLateMemberAccess).SetAccessKind(newAccessKind)
			End If
			Return Me.Update(member, Me.ArgumentsOpt, Me.ArgumentNamesOpt, newAccessKind, Me.MethodOrPropertyGroupOpt, MyBase.Type)
		End Function

		Public Function Update(ByVal member As BoundExpression, ByVal argumentsOpt As ImmutableArray(Of BoundExpression), ByVal argumentNamesOpt As ImmutableArray(Of String), ByVal accessKind As LateBoundAccessKind, ByVal methodOrPropertyGroupOpt As BoundMethodOrPropertyGroup, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation
			Dim boundLateInvocation As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation
			If (member <> Me.Member OrElse argumentsOpt <> Me.ArgumentsOpt OrElse argumentNamesOpt <> Me.ArgumentNamesOpt OrElse accessKind <> Me.AccessKind OrElse methodOrPropertyGroupOpt <> Me.MethodOrPropertyGroupOpt OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundLateInvocation1 As Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation = New Microsoft.CodeAnalysis.VisualBasic.BoundLateInvocation(MyBase.Syntax, member, argumentsOpt, argumentNamesOpt, accessKind, methodOrPropertyGroupOpt, type, MyBase.HasErrors)
				boundLateInvocation1.CopyAttributes(Me)
				boundLateInvocation = boundLateInvocation1
			Else
				boundLateInvocation = Me
			End If
			Return boundLateInvocation
		End Function
	End Class
End Namespace