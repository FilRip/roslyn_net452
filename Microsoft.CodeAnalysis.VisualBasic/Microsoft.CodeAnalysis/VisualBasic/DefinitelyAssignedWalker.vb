Imports Microsoft.CodeAnalysis
Imports System
Imports System.Collections
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DefinitelyAssignedWalker
		Inherits AbstractRegionDataFlowPass
		Private ReadOnly _definitelyAssignedOnEntry As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _definitelyAssignedOnExit As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region, Nothing, False, False)
			Me._definitelyAssignedOnEntry = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._definitelyAssignedOnExit = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo) As <TupleElementNames(New String() { "entry", "ex" })> ValueTuple(Of HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			Dim valueTuple As ValueTuple(Of HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			Dim definitelyAssignedWalker As Microsoft.CodeAnalysis.VisualBasic.DefinitelyAssignedWalker = New Microsoft.CodeAnalysis.VisualBasic.DefinitelyAssignedWalker(info, region)
			Try
				valueTuple = If(definitelyAssignedWalker.Analyze(), New ValueTuple(Of HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))(definitelyAssignedWalker._definitelyAssignedOnEntry, definitelyAssignedWalker._definitelyAssignedOnExit), New ValueTuple(Of HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))(New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)(), New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()))
			Finally
				definitelyAssignedWalker.Free()
			End Try
			Return valueTuple
		End Function

		Protected Overrides Sub EnterRegion()
			Me.ProcessRegion(Me._definitelyAssignedOnEntry)
			MyBase.EnterRegion()
		End Sub

		Protected Overrides Sub LeaveRegion()
			Me.ProcessRegion(Me._definitelyAssignedOnExit)
			MyBase.LeaveRegion()
		End Sub

		Private Sub ProcessRegion(ByVal definitelyAssigned As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol))
			definitelyAssigned.Clear()
			If (Me.IsConditionalState) Then
				Me.ProcessState(definitelyAssigned, Me.StateWhenTrue, New Nullable(Of DataFlowPass.LocalState)(Me.StateWhenFalse))
				Return
			End If
			Me.ProcessState(definitelyAssigned, Me.State, Nothing)
		End Sub

		Private Sub ProcessState(ByVal definitelyAssigned As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal state1 As DataFlowPass.LocalState, ByVal state2opt As Nullable(Of DataFlowPass.LocalState))
			Dim enumerator As IEnumerator(Of Integer) = Nothing
			Try
				enumerator = state1.Assigned.TrueBits().GetEnumerator()
				While enumerator.MoveNext()
					Dim current As Integer = enumerator.Current
					If (current >= CInt(Me.variableBySlot.Length) OrElse state2opt.HasValue AndAlso Not state2opt.Value.IsAssigned(current)) Then
						Continue While
					End If
					Dim symbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = Me.variableBySlot(current).Symbol
					If (symbol Is Nothing OrElse symbol.Kind = SymbolKind.Field) Then
						Continue While
					End If
					definitelyAssigned.Add(symbol)
				End While
			Finally
				If (enumerator IsNot Nothing) Then
					enumerator.Dispose()
				End If
			End Try
		End Sub
	End Class
End Namespace