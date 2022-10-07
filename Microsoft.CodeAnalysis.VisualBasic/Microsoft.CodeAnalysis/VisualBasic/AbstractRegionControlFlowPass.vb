Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class AbstractRegionControlFlowPass
		Inherits ControlFlowPass
		Friend Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region, False)
		End Sub

		Protected Overrides Sub Visit(ByVal node As BoundNode, ByVal dontLeaveRegion As Boolean)
			MyBase.VisitAlways(node, dontLeaveRegion)
		End Sub

		Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause) As BoundNode
			If (node.CapturedGroupOpt IsNot Nothing) Then
				MyBase.VisitRvalue(node.CapturedGroupOpt, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			End If
			MyBase.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Dim savedPending As AbstractFlowPass(Of ControlFlowPass.LocalState).SavedPending = MyBase.SavePending()
			Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.symbol
			Me.symbol = node.LambdaSymbol
			Dim state As ControlFlowPass.LocalState = Me.State
			MyBase.SetState(Me.ReachableState())
			Me.VisitBlock(node.Body)
			Me.symbol = symbol
			MyBase.RestorePending(savedPending, False)
			Me.IntersectWith(state, Me.State)
			MyBase.SetState(state)
			Return Nothing
		End Function

		Public Overrides Function VisitOrdering(ByVal node As BoundOrdering) As BoundNode
			MyBase.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			MyBase.VisitRvalue(node.Source, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
			MyBase.VisitRvalue(node.UnderlyingExpression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			MyBase.VisitRvalue(node.LastOperator, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			MyBase.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitQuerySource(ByVal node As BoundQuerySource) As BoundNode
			MyBase.VisitRvalue(node.Expression, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			MyBase.VisitRvalue(node.Value, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function

		Public Overrides Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion) As BoundNode
			MyBase.VisitRvalue(node.ConversionCall, AbstractFlowPass(Of LocalState).ReadWriteContext.None, False)
			Return Nothing
		End Function
	End Class
End Namespace