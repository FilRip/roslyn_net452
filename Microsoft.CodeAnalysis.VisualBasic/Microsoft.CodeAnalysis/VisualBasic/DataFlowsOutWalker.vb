Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class DataFlowsOutWalker
		Inherits AbstractRegionDataFlowPass
		Private ReadOnly _dataFlowsIn As ImmutableArray(Of ISymbol)

		Private ReadOnly _originalUnassigned As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private ReadOnly _dataFlowsOut As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByVal unassignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal originalUnassigned As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal dataFlowsIn As ImmutableArray(Of ISymbol))
			MyBase.New(info, region, unassignedVariables, True, True)
			Me._dataFlowsOut = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
			Me._dataFlowsIn = dataFlowsIn
			Me._originalUnassigned = originalUnassigned
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByVal unassignedVariables As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol), ByVal dataFlowsIn As ImmutableArray(Of ISymbol)) As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
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
			Dim dataFlowsOutWalker As Microsoft.CodeAnalysis.VisualBasic.DataFlowsOutWalker = New Microsoft.CodeAnalysis.VisualBasic.DataFlowsOutWalker(info, region, symbols1, unassignedVariables, dataFlowsIn)
			Try
				symbols = If(dataFlowsOutWalker.Analyze(), dataFlowsOutWalker._dataFlowsOut, New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)())
			Finally
				dataFlowsOutWalker.Free()
			End Try
			Return symbols
		End Function

		Protected Overrides Sub Assign(ByVal node As BoundNode, ByVal value As BoundExpression, Optional ByVal assigned As Boolean = True)
			If (MyBase.IsInside) Then
				If (assigned) Then
					MyBase.GetNodeSymbol(node)
				End If
				assigned = False
				If (Me.State.Reachable) Then
					Dim kind As BoundKind = node.Kind
					If (kind <> BoundKind.MeReference) Then
						If (kind = BoundKind.Local) Then
							Dim boundLocal As Microsoft.CodeAnalysis.VisualBasic.BoundLocal = DirectCast(node, Microsoft.CodeAnalysis.VisualBasic.BoundLocal)
							Dim localSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbols.LocalSymbol = boundLocal.LocalSymbol
							If (localSymbol.IsStatic AndAlso Me.WasUsedBeforeAssignment(localSymbol)) Then
								Dim slotCollection As DataFlowPass.SlotCollection = MyBase.MakeSlotsForExpression(boundLocal)
								If (slotCollection.Count > 0) Then
									Dim item As Integer = slotCollection(0)
									If (item >= 2) Then
										Me._dataFlowsOut.Add(Me.variableBySlot(item).Symbol)
										MyBase.Assign(node, value, assigned)
										Return
									Else
										MyBase.Assign(node, value, assigned)
										Return
									End If
								Else
									MyBase.Assign(node, value, assigned)
									Return
								End If
							Else
								MyBase.Assign(node, value, assigned)
								Return
							End If
						ElseIf (kind <> BoundKind.Parameter) Then
							MyBase.Assign(node, value, assigned)
							Return
						End If
					End If
					Dim slotCollection1 As DataFlowPass.SlotCollection = MyBase.MakeSlotsForExpression(DirectCast(node, BoundExpression))
					If (slotCollection1.Count > 0) Then
						Dim num As Integer = slotCollection1(0)
						If (num >= 2) Then
							Dim symbol As ParameterSymbol = DirectCast(Me.variableBySlot(num).Symbol, ParameterSymbol)
							If (symbol IsNot Nothing AndAlso symbol.IsByRef) Then
								Me._dataFlowsOut.Add(symbol)
							End If
						End If
					End If
				End If
			End If
			MyBase.Assign(node, value, assigned)
		End Sub

		Friend Overrides Sub AssignLocalOnDeclaration(ByVal local As LocalSymbol, ByVal node As BoundLocalDeclaration)
			If (Not local.IsStatic) Then
				MyBase.AssignLocalOnDeclaration(local, node)
				Return
			End If
			Me.Assign(node, node.InitializerOpt, True)
		End Sub

		Protected Overrides Sub EnterRegion()
			Dim enumerator As ImmutableArray(Of ISymbol).Enumerator = Me._dataFlowsIn.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As Microsoft.CodeAnalysis.VisualBasic.Symbol = DirectCast(enumerator.Current, Microsoft.CodeAnalysis.VisualBasic.Symbol)
				Dim orCreateSlot As Integer = MyBase.GetOrCreateSlot(current, 0)
				If (Me.State.IsAssigned(orCreateSlot) OrElse current.Kind = SymbolKind.RangeVariable OrElse current.Kind = SymbolKind.Local AndAlso DirectCast(current, LocalSymbol).IsStatic) Then
					Continue While
				End If
				Me._dataFlowsOut.Add(current)
			End While
			MyBase.EnterRegion()
		End Sub

		Protected Overrides Sub NoteWrite(ByVal variable As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal value As BoundExpression)
			If (Me.State.Reachable) Then
				Dim flag As Boolean = If(variable.Kind <> SymbolKind.Parameter, False, DirectCast(variable, ParameterSymbol).IsByRef)
				Dim flag1 As Boolean = If(variable.Kind <> SymbolKind.Local, False, DirectCast(variable, LocalSymbol).IsStatic)
				If (MyBase.IsInside AndAlso (flag OrElse flag1 AndAlso Me.WasUsedBeforeAssignment(variable))) Then
					Me._dataFlowsOut.Add(variable)
				End If
			End If
			MyBase.NoteWrite(variable, value)
		End Sub

		Protected Overrides Sub ReportUnassigned(ByVal local As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext, Optional ByVal slot As Integer = -1, Optional ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Nothing)
			If (Not Me._dataFlowsOut.Contains(local) AndAlso local.Kind <> SymbolKind.RangeVariable AndAlso Not MyBase.IsInside) Then
				If (local.Kind <> SymbolKind.Field) Then
					Me._dataFlowsOut.Add(local)
				Else
					Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = MyBase.GetNodeSymbol(boundFieldAccess)
					If (nodeSymbol IsNot Nothing) Then
						Me._dataFlowsOut.Add(nodeSymbol)
					End If
				End If
			End If
			MyBase.ReportUnassigned(local, node, rwContext, slot, boundFieldAccess)
		End Sub

		Protected Overrides Sub ReportUnassignedByRefParameter(ByVal parameter As ParameterSymbol)
			Me._dataFlowsOut.Add(parameter)
			MyBase.ReportUnassignedByRefParameter(parameter)
		End Sub

		Private Function WasUsedBeforeAssignment(ByVal sym As Microsoft.CodeAnalysis.VisualBasic.Symbol) As Boolean
			Return Me._originalUnassigned.Contains(sym)
		End Function
	End Class
End Namespace