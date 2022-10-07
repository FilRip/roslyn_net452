Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Immutable
Imports System.Diagnostics

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class BoundGroupAggregation
		Inherits BoundQueryPart
		Private ReadOnly _Group As BoundExpression

		Protected Overrides ReadOnly Property Children As ImmutableArray(Of BoundNode)
			Get
				Return ImmutableArray.Create(Of BoundNode)(Me.Group)
			End Get
		End Property

		Public ReadOnly Property Group As BoundExpression
			Get
				Return Me._Group
			End Get
		End Property

		Public Sub New(ByVal syntax As SyntaxNode, ByVal group As BoundExpression, ByVal type As TypeSymbol, Optional ByVal hasErrors As Boolean = False)
			MyBase.New(BoundKind.GroupAggregation, syntax, type, If(hasErrors, True, group.NonNullAndHasErrors()))
			Me._Group = group
		End Sub

		<DebuggerStepThrough>
		Public Overrides Function Accept(ByVal visitor As BoundTreeVisitor) As BoundNode
			Return visitor.VisitGroupAggregation(Me)
		End Function

		Public Function Update(ByVal group As BoundExpression, ByVal type As TypeSymbol) As Microsoft.CodeAnalysis.VisualBasic.BoundGroupAggregation
			Dim boundGroupAggregation As Microsoft.CodeAnalysis.VisualBasic.BoundGroupAggregation
			If (group <> Me.Group OrElse CObj(type) <> CObj(MyBase.Type)) Then
				Dim boundGroupAggregation1 As Microsoft.CodeAnalysis.VisualBasic.BoundGroupAggregation = New Microsoft.CodeAnalysis.VisualBasic.BoundGroupAggregation(MyBase.Syntax, group, type, MyBase.HasErrors)
				boundGroupAggregation1.CopyAttributes(Me)
				boundGroupAggregation = boundGroupAggregation1
			Else
				boundGroupAggregation = Me
			End If
			Return boundGroupAggregation
		End Function
	End Class
End Namespace