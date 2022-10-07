Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.PooledObjects
Imports Microsoft.CodeAnalysis.VisualBasic.Symbols
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ExitPointsWalker
		Inherits AbstractRegionControlFlowPass
		Private _branchesOutOf As ArrayBuilder(Of StatementSyntax)

		Private _labelsInside As ArrayBuilder(Of LabelSymbol)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region)
			Me._branchesOutOf = ArrayBuilder(Of StatementSyntax).GetInstance()
			Me._labelsInside = ArrayBuilder(Of LabelSymbol).GetInstance()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo) As IEnumerable(Of StatementSyntax)
			Dim statementSyntaxes As IEnumerable(Of StatementSyntax)
			Dim exitPointsWalker As Microsoft.CodeAnalysis.VisualBasic.ExitPointsWalker = New Microsoft.CodeAnalysis.VisualBasic.ExitPointsWalker(info, region)
			Try
				statementSyntaxes = If(exitPointsWalker.Analyze(), DirectCast(exitPointsWalker._branchesOutOf.ToImmutable(), IEnumerable(Of StatementSyntax)), SpecializedCollections.EmptyEnumerable(Of StatementSyntax)())
			Finally
				exitPointsWalker.Free()
			End Try
			Return statementSyntaxes
		End Function

		Private Shadows Function Analyze() As Boolean
			Return Me.Scan()
		End Function

		Protected Overrides Sub Free()
			Me._branchesOutOf.Free()
			Me._branchesOutOf = Nothing
			Me._labelsInside.Free()
			Me._labelsInside = Nothing
			MyBase.Free()
		End Sub

		Protected Overrides Sub LeaveRegion()
			Dim current As AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch
			Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch).Enumerator = MyBase.PendingBranches.GetEnumerator()
			While True
				If (Not enumerator.MoveNext()) Then
					MyBase.LeaveRegion()
					Return
				End If
				current = enumerator.Current
				If (MyBase.IsInsideRegion(current.Branch.Syntax.Span)) Then
					Dim kind As BoundKind = current.Branch.Kind
					If (kind <= BoundKind.YieldStatement) Then
						If (kind <> BoundKind.ReturnStatement AndAlso kind <> BoundKind.YieldStatement) Then
							Exit While
						End If
					ElseIf (kind = BoundKind.ExitStatement) Then
						If (Not Me._labelsInside.Contains(TryCast(current.Branch, BoundExitStatement).Label)) Then
							GoTo Label0
						End If
						Continue While
					ElseIf (kind = BoundKind.ContinueStatement) Then
						If (Not Me._labelsInside.Contains(TryCast(current.Branch, BoundContinueStatement).Label)) Then
							GoTo Label0
						End If
						Continue While
					Else
						If (kind <> BoundKind.GotoStatement) Then
							Exit While
						End If
						If (Not Me._labelsInside.Contains(TryCast(current.Branch, BoundGotoStatement).Label)) Then
							GoTo Label0
						End If
						Continue While
					End If
				Label0:
					Me._branchesOutOf.Add(DirectCast(current.Branch.Syntax, StatementSyntax))
				End If
			End While
			Throw ExceptionUtilities.UnexpectedValue(current.Branch.Kind)
		End Sub

		Public Overrides Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement) As BoundNode
			If (MyBase.IsInside) Then
				Me._labelsInside.Add(node.ExitLabel)
				Me._labelsInside.Add(node.ContinueLabel)
			End If
			Return MyBase.VisitDoLoopStatement(node)
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
			If (MyBase.IsInside) Then
				Me._labelsInside.Add(node.ExitLabel)
				Me._labelsInside.Add(node.ContinueLabel)
			End If
			Return MyBase.VisitForEachStatement(node)
		End Function

		Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement) As BoundNode
			If (MyBase.IsInside) Then
				Me._labelsInside.Add(node.ExitLabel)
				Me._labelsInside.Add(node.ContinueLabel)
			End If
			Return MyBase.VisitForToStatement(node)
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			Dim syntax As SyntaxNode = node.Syntax
			If (MyBase.IsInside) Then
				Me._labelsInside.Add(node.Label)
			End If
			Return MyBase.VisitLabelStatement(node)
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
			If (MyBase.IsInside) Then
				Me._labelsInside.Add(node.ExitLabel)
			End If
			Return MyBase.VisitSelectStatement(node)
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As BoundWhileStatement) As BoundNode
			If (MyBase.IsInside) Then
				Me._labelsInside.Add(node.ExitLabel)
				Me._labelsInside.Add(node.ContinueLabel)
			End If
			Return MyBase.VisitWhileStatement(node)
		End Function
	End Class
End Namespace