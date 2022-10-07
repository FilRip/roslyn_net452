Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend NotInheritable Class UnassignedVariablesWalker
		Inherits DataFlowPass
		Private ReadOnly _result As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Protected Overrides ReadOnly Property EnableBreakingFlowAnalysisFeatures As Boolean
			Get
				Return True
			End Get
		End Property

		Protected Overrides ReadOnly Property IgnoreOutSemantics As Boolean
			Get
				Return False
			End Get
		End Property

		Protected Overrides ReadOnly Property SuppressRedimOperandRvalueOnPreserve As Boolean
			Get
				Return False
			End Get
		End Property

		Private Sub New(ByVal info As FlowAnalysisInfo)
			MyBase.New(info, False, True)
			Me._result = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo) As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim symbols As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim unassignedVariablesWalker As Microsoft.CodeAnalysis.VisualBasic.UnassignedVariablesWalker = New Microsoft.CodeAnalysis.VisualBasic.UnassignedVariablesWalker(info)
			Try
				symbols = If(unassignedVariablesWalker.Analyze(), unassignedVariablesWalker._result, New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)())
			Finally
				unassignedVariablesWalker.Free()
			End Try
			Return symbols
		End Function

		Friend Overrides Sub AssignLocalOnDeclaration(ByVal local As LocalSymbol, ByVal node As BoundLocalDeclaration)
			If (Not local.IsStatic) Then
				MyBase.AssignLocalOnDeclaration(local, node)
			End If
		End Sub

		Protected Overrides Sub ReportUnassigned(ByVal local As Microsoft.CodeAnalysis.VisualBasic.Symbol, ByVal node As SyntaxNode, ByVal rwContext As AbstractFlowPass(Of DataFlowPass.LocalState).ReadWriteContext, Optional ByVal slot As Integer = -1, Optional ByVal boundFieldAccess As Microsoft.CodeAnalysis.VisualBasic.BoundFieldAccess = Nothing)
			If (local.Kind <> SymbolKind.Field) Then
				Me._result.Add(local)
			Else
				Dim nodeSymbol As Microsoft.CodeAnalysis.VisualBasic.Symbol = MyBase.GetNodeSymbol(boundFieldAccess)
				If (nodeSymbol IsNot Nothing) Then
					Me._result.Add(nodeSymbol)
				End If
			End If
			MyBase.ReportUnassigned(local, node, rwContext, slot, boundFieldAccess)
		End Sub
	End Class
End Namespace