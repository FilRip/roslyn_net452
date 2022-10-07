Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DataFlowsInWalker
		Inherits AbstractRegionDataFlowPass
		Private ReadOnly _dataFlowsIn As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByVal unassignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			MyBase.New(info, region, unassignedVariables, False, True)
			Me._dataFlowsIn = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByVal unassignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByRef succeeded As Nullable(Of Boolean), ByRef invalidRegionDetected As Boolean) As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim symbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim enumerator As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol).Enumerator()
			Dim symbols1 As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol) = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Try
				enumerator = unassignedVariables.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = enumerator.Current
					If (current.Kind = SymbolKind.Local AndAlso DirectCast(current, LocalSymbol).IsStatic) Then
						Continue While
					End If
					symbols1.Add(current)
				End While
			Finally
				DirectCast(enumerator, IDisposable).Dispose()
			End Try
			Dim dataFlowsInWalker As Microsoft.CodeAnalysis.VisualBasic.DataFlowsInWalker = New Microsoft.CodeAnalysis.VisualBasic.DataFlowsInWalker(info, region, symbols1)
			Try
				succeeded = New Nullable(Of Boolean)(If(Not dataFlowsInWalker.Analyze(), False, Not dataFlowsInWalker.InvalidRegionDetected))
				invalidRegionDetected = dataFlowsInWalker.InvalidRegionDetected
				symbols = If(succeeded.GetValueOrDefault(), dataFlowsInWalker._dataFlowsIn, New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)())
			Finally
				dataFlowsInWalker.Free()
			End Try
			Return symbols
		End Function

		Friend Overrides Sub AssignLocalOnDeclaration(ByVal local As LocalSymbol, ByVal node As BoundLocalDeclaration)
			If (Not local.IsStatic) Then
				MyBase.AssignLocalOnDeclaration(local, node)
			End If
		End Sub

		Protected Overrides Sub EnterRegion()
			MyBase.SetState(Me.ResetState(Me.State))
			Me._dataFlowsIn.Clear()
			MyBase.EnterRegion()
		End Sub

		Protected Overrides Sub NoteBranch(ByVal pending As AbstractFlowPass(Of DataFlowPass.LocalState).PendingBranch, ByVal stmt As BoundStatement, ByVal labelStmt As BoundLabelStatement)
			If (stmt.Syntax IsNot Nothing AndAlso labelStmt.Syntax IsNot Nothing AndAlso Not MyBase.IsInsideRegion(stmt.Syntax.Span) AndAlso MyBase.IsInsideRegion(labelStmt.Syntax.Span)) Then
				pending.State = Me.ResetState(pending.State)
			End If
			MyBase.NoteBranch(pending, stmt, labelStmt)
		End Sub

		Protected Overrides Sub ReportUnassigned(ByVal local As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext, Optional ByVal slot As Integer = -1, Optional ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Nothing)
			If (MyBase.IsInsideRegion(node.Span)) Then
				If (local.Kind <> SymbolKind.Field) Then
					Me._dataFlowsIn.Add(local)
				Else
					Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = MyBase.GetNodeSymbol(boundFieldAccess)
					If (nodeSymbol IsNot Nothing) Then
						Me._dataFlowsIn.Add(nodeSymbol)
					End If
				End If
			End If
			MyBase.ReportUnassigned(local, node, rwContext, slot, boundFieldAccess)
		End Sub

		Private Function ResetState(ByVal state As DataFlowPass.LocalState) As DataFlowPass.LocalState
			Dim reachable As Boolean = Not state.Reachable
			state = Me.ReachableState()
			If (reachable) Then
				state.Assign(0)
			End If
			Return state
		End Function

		Protected Overrides Sub VisitAmbiguousLocalSymbol(ByVal ambiguous As DataFlowPass.AmbiguousLocalsPseudoSymbol)
			MyBase.VisitAmbiguousLocalSymbol(ambiguous)
			If (MyBase.IsInside) Then
				Dim item As LocalSymbol = ambiguous.Locals(0)
				If (Not Me.State.IsAssigned(MyBase.VariableSlot(item, 0))) Then
					MyBase.SetInvalidRegion()
				End If
			End If
		End Sub

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			If (Not node.WasCompilerGenerated AndAlso MyBase.IsInside AndAlso Not MyBase.IsInsideRegion(node.RangeVariable.Syntax.Span)) Then
				Me._dataFlowsIn.Add(node.RangeVariable)
			End If
			Return Nothing
		End Function
	End Class
End Namespace