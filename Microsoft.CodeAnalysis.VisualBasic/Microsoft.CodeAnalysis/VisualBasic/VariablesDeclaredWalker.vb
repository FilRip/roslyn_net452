Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class VariablesDeclaredWalker
		Inherits AbstractRegionControlFlowPass
		Private ReadOnly _variablesDeclared As HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region)
			Me._variablesDeclared = New HashSet(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo) As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim symbols As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim symbols1 As IEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)
			Dim variablesDeclaredWalker As Microsoft.CodeAnalysis.VisualBasic.VariablesDeclaredWalker = New Microsoft.CodeAnalysis.VisualBasic.VariablesDeclaredWalker(info, region)
			Try
				If (variablesDeclaredWalker.Analyze()) Then
					symbols1 = variablesDeclaredWalker._variablesDeclared
				Else
					symbols1 = SpecializedCollections.EmptyEnumerable(Of Microsoft.CodeAnalysis.VisualBasic.Symbol)()
				End If
				symbols = symbols1
			Finally
				variablesDeclaredWalker.Free()
			End Try
			Return symbols
		End Function

		Private Shadows Function Analyze() As Boolean
			Return Me.Scan()
		End Function

		Protected Overrides Sub VisitCatchBlock(ByVal catchBlock As BoundCatchBlock, ByRef finallyState As ControlFlowPass.LocalState)
			If (MyBase.IsInsideRegion(catchBlock.Syntax.Span) AndAlso catchBlock.LocalOpt IsNot Nothing) Then
				Me._variablesDeclared.Add(catchBlock.LocalOpt)
			End If
			MyBase.VisitCatchBlock(catchBlock, finallyState)
		End Sub

		Protected Overrides Sub VisitForStatementVariableDeclaration(ByVal node As BoundForStatement)
			If (MyBase.IsInside AndAlso node.DeclaredOrInferredLocalOpt IsNot Nothing) Then
				Me._variablesDeclared.Add(node.DeclaredOrInferredLocalOpt)
			End If
			MyBase.VisitForStatementVariableDeclaration(node)
		End Sub

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			If (MyBase.IsInside) Then
				Dim enumerator As ImmutableArray(Of ParameterSymbol).Enumerator = node.LambdaSymbol.Parameters.GetEnumerator()
				While enumerator.MoveNext()
					Dim current As ParameterSymbol = enumerator.Current
					Me._variablesDeclared.Add(current)
				End While
			End If
			Return MyBase.VisitLambda(node)
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			If (MyBase.IsInside) Then
				Me._variablesDeclared.Add(node.LocalSymbol)
			End If
			Return MyBase.VisitLocalDeclaration(node)
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			MyBase.VisitQueryableSource(node)
			If (Not node.WasCompilerGenerated AndAlso node.RangeVariables.Length > 0 AndAlso MyBase.IsInside) Then
				Me._variablesDeclared.Add(node.RangeVariables(0))
			End If
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			If (Not node.WasCompilerGenerated AndAlso MyBase.IsInside) Then
				Me._variablesDeclared.Add(node.RangeVariable)
			End If
			MyBase.VisitRangeVariableAssignment(node)
			Return Nothing
		End Function
	End Class
End Namespace