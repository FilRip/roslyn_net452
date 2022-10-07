Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class AlwaysAssignedWalker
		Inherits AbstractRegionDataFlowPass
		Private _endOfRegionState As DataFlowPass.LocalState

		Private ReadOnly _labelsInside As HashSet(Of LabelSymbol)

		Private ReadOnly Property AlwaysAssigned As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Get
				Dim enumerator As IEnumerator(Of Integer) = Nothing
				Dim symbols As List(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = New List(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
				If (Me._endOfRegionState.Reachable) Then
					Try
						enumerator = Me._endOfRegionState.Assigned.TrueBits().GetEnumerator()
						While enumerator.MoveNext()
							Dim current As Integer = enumerator.Current
							If (current >= CInt(Me.variableBySlot.Length)) Then
								Continue While
							End If
							Dim variableIdentifier As DataFlowPass.VariableIdentifier = Me.variableBySlot(current)
							If (Not variableIdentifier.Exists OrElse variableIdentifier.Symbol.Kind = SymbolKind.Field) Then
								Continue While
							End If
							symbols.Add(variableIdentifier.Symbol)
						End While
					Finally
						If (enumerator IsNot Nothing) Then
							enumerator.Dispose()
						End If
					End Try
				End If
				Return symbols
			End Get
		End Property

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region, Nothing, False, False)
			Me._labelsInside = New HashSet(Of LabelSymbol)()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo) As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim symbols As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim alwaysAssignedWalker As Microsoft.CodeAnalysis.VisualBasic.AlwaysAssignedWalker = New Microsoft.CodeAnalysis.VisualBasic.AlwaysAssignedWalker(info, region)
			Try
				symbols = If(alwaysAssignedWalker.Analyze(), alwaysAssignedWalker.AlwaysAssigned, SpecializedCollections.EmptyEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)())
			Finally
				alwaysAssignedWalker.Free()
			End Try
			Return symbols
		End Function

		Protected Overrides Sub EnterRegion()
			MyBase.SetState(Me.ReachableState())
			MyBase.EnterRegion()
		End Sub

		Protected Overrides Sub LeaveRegion()
			If (Not Me.IsConditionalState) Then
				Me._endOfRegionState = Me.State.Clone()
			Else
				Me._endOfRegionState = Me.StateWhenTrue.Clone()
				Me.IntersectWith(Me._endOfRegionState, Me.StateWhenFalse)
			End If
			Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of DataFlowPass.LocalState).PendingBranch).Enumerator = MyBase.PendingBranches.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AbstractFlowPass(Of DataFlowPass.LocalState).PendingBranch = enumerator.Current
				If (Not MyBase.IsInsideRegion(current.Branch.Syntax.Span) OrElse Me._labelsInside.Contains(current.Label)) Then
					Continue While
				End If
				Me.IntersectWith(Me._endOfRegionState, current.State)
			End While
			MyBase.LeaveRegion()
		End Sub

		Protected Overrides Sub ResolveBranch(ByVal pending As AbstractFlowPass(Of DataFlowPass.LocalState).PendingBranch, ByVal label As LabelSymbol, ByVal target As BoundLabelStatement, ByRef labelStateChanged As Boolean)
			If (MyBase.IsInside AndAlso pending.Branch IsNot Nothing AndAlso Not MyBase.IsInsideRegion(pending.Branch.Syntax.Span)) Then
				pending.State = If(pending.State.Reachable, Me.ReachableState(), Me.UnreachableState())
			End If
			MyBase.ResolveBranch(pending, label, target, labelStateChanged)
		End Sub

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			If (node.Syntax IsNot Nothing AndAlso MyBase.IsInsideRegion(node.Syntax.Span)) Then
				Me._labelsInside.Add(node.Label)
			End If
			Return MyBase.VisitLabelStatement(node)
		End Function

		Protected Overrides Sub WriteArgument(ByVal arg As BoundExpression, ByVal isOut As Boolean)
			If (isOut) Then
				MyBase.WriteArgument(arg, isOut)
			End If
		End Sub
	End Class
End Namespace