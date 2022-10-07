Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Roslyn.Utilities
Imports System
Imports System.Collections.Generic

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class EntryPointsWalker
		Inherits AbstractRegionControlFlowPass
		Private ReadOnly _entryPoints As HashSet(Of LabelStatementSyntax)

		Private Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo)
			MyBase.New(info, region)
			Me._entryPoints = New HashSet(Of LabelStatementSyntax)()
		End Sub

		Friend Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByRef succeeded As Nullable(Of Boolean)) As IEnumerable(Of LabelStatementSyntax)
			Dim labelStatementSyntaxes As IEnumerable(Of LabelStatementSyntax)
			Dim labelStatementSyntaxes1 As IEnumerable(Of LabelStatementSyntax)
			Dim entryPointsWalker As Microsoft.CodeAnalysis.VisualBasic.EntryPointsWalker = New Microsoft.CodeAnalysis.VisualBasic.EntryPointsWalker(info, region)
			Try
				succeeded = New Nullable(Of Boolean)(entryPointsWalker.Analyze())
				If (succeeded.GetValueOrDefault()) Then
					labelStatementSyntaxes1 = entryPointsWalker._entryPoints
				Else
					labelStatementSyntaxes1 = SpecializedCollections.EmptyEnumerable(Of LabelStatementSyntax)()
				End If
				labelStatementSyntaxes = labelStatementSyntaxes1
			Finally
				entryPointsWalker.Free()
			End Try
			Return labelStatementSyntaxes
		End Function

		Private Shadows Function Analyze() As Boolean
			Return Me.Scan()
		End Function

		Protected Overrides Sub Free()
			MyBase.Free()
		End Sub

		Protected Overrides Sub NoteBranch(ByVal pending As AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch, ByVal stmt As BoundStatement, ByVal labelStmt As BoundLabelStatement)
			If (stmt.Syntax IsNot Nothing AndAlso labelStmt.Syntax IsNot Nothing AndAlso MyBase.IsInsideRegion(labelStmt.Syntax.Span) AndAlso Not MyBase.IsInsideRegion(stmt.Syntax.Span)) Then
				Dim kind As BoundKind = stmt.Kind
				If (kind <> BoundKind.ReturnStatement) Then
					If (kind <> BoundKind.GotoStatement) Then
						Throw ExceptionUtilities.UnexpectedValue(stmt.Kind)
					End If
					Me._entryPoints.Add(DirectCast(labelStmt.Syntax, LabelStatementSyntax))
					Return
				End If
			End If
		End Sub
	End Class
End Namespace