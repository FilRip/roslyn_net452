Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend Class ControlFlowPass
		Inherits AbstractFlowPass(Of ControlFlowPass.LocalState)
		Protected _convertInsufficientExecutionStackExceptionToCancelledByStackGuardException As Boolean

		Friend Sub New(ByVal info As FlowAnalysisInfo, ByVal suppressConstExpressionsSupport As Boolean)
			MyBase.New(info, suppressConstExpressionsSupport)
			Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = False
		End Sub

		Friend Sub New(ByVal info As FlowAnalysisInfo, ByVal region As FlowAnalysisRegionInfo, ByVal suppressConstantExpressionsSupport As Boolean)
			MyBase.New(info, region, suppressConstantExpressionsSupport, False)
			Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = False
		End Sub

		Public Shared Function Analyze(ByVal info As FlowAnalysisInfo, ByVal diagnostics As DiagnosticBag, ByVal suppressConstantExpressionsSupport As Boolean) As Boolean
			Dim alive As Boolean
			Dim controlFlowPass As Microsoft.CodeAnalysis.VisualBasic.ControlFlowPass = New Microsoft.CodeAnalysis.VisualBasic.ControlFlowPass(info, suppressConstantExpressionsSupport)
			If (diagnostics IsNot Nothing) Then
				controlFlowPass._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException = True
			End If
			Try
				Try
					controlFlowPass.Analyze()
					If (diagnostics IsNot Nothing) Then
						diagnostics.AddRange(controlFlowPass.diagnostics)
					End If
					alive = controlFlowPass.State.Alive
				Catch cancelledByStackGuardException As BoundTreeVisitor.CancelledByStackGuardException When diagnostics IsNot Nothing
					cancelledByStackGuardException.AddAnError(diagnostics)
					alive = True
					ProjectData.ClearProjectError()
				End Try
			Finally
				controlFlowPass.Free()
			End Try
			Return alive
		End Function

		Protected Overrides Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
			Return Me._convertInsufficientExecutionStackExceptionToCancelledByStackGuardException
		End Function

		Protected Overrides Function Dump(ByVal state As ControlFlowPass.LocalState) As String
			Return [String].Concat(New [String]() { "[alive: ", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(state.Alive), "; reported: ", Microsoft.VisualBasic.CompilerServices.Conversions.ToString(state.Reported), "]" })
		End Function

		Protected Overrides Function IntersectWith(ByRef self As ControlFlowPass.LocalState, ByRef other As ControlFlowPass.LocalState) As Boolean
			Dim localState As ControlFlowPass.LocalState = self
			self.Alive = self.Alive Or other.Alive
			self.Reported = self.Reported And other.Reported
			Return Not self.Equals(localState)
		End Function

		Protected Overrides Function ReachableState() As ControlFlowPass.LocalState
			Return New ControlFlowPass.LocalState(True, False)
		End Function

		Protected Overrides Sub UnionWith(ByRef self As ControlFlowPass.LocalState, ByRef other As ControlFlowPass.LocalState)
			self.Alive = self.Alive And other.Alive
			self.Reported = self.Reported And other.Reported
		End Sub

		Protected Overrides Function UnreachableState() As ControlFlowPass.LocalState
			Return New ControlFlowPass.LocalState(False, Me.State.Reported)
		End Function

		Protected Overrides Sub Visit(ByVal node As BoundNode, ByVal dontLeaveRegion As Boolean)
			If (Not TypeOf node Is BoundExpression) Then
				MyBase.Visit(node, dontLeaveRegion)
			End If
		End Sub

		Protected Overrides Sub VisitCatchBlock(ByVal node As BoundCatchBlock, ByRef finallyState As ControlFlowPass.LocalState)
			Dim savedPending As AbstractFlowPass(Of ControlFlowPass.LocalState).SavedPending = MyBase.SavePending()
			MyBase.VisitCatchBlock(node, finallyState)
			Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch).Enumerator = MyBase.PendingBranches.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch = enumerator.Current
				If (current.Branch.Kind <> BoundKind.YieldStatement) Then
					Continue While
				End If
				Me.diagnostics.Add(ERRID.ERR_BadYieldInTryHandler, current.Branch.Syntax.GetLocation())
			End While
			MyBase.RestorePending(savedPending, False)
		End Sub

		Protected Overrides Sub VisitFinallyBlock(ByVal finallyBlock As BoundStatement, ByRef endState As ControlFlowPass.LocalState)
			Dim syntaxNodeOrToken As Microsoft.CodeAnalysis.SyntaxNodeOrToken
			Dim eRRID As Microsoft.CodeAnalysis.VisualBasic.ERRID
			Dim savedPending As AbstractFlowPass(Of ControlFlowPass.LocalState).SavedPending = MyBase.SavePending()
			Dim savedPending1 As AbstractFlowPass(Of ControlFlowPass.LocalState).SavedPending = MyBase.SavePending()
			MyBase.VisitFinallyBlock(finallyBlock, endState)
			MyBase.RestorePending(savedPending1, False)
			Dim enumerator As ImmutableArray(Of AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch).Enumerator = MyBase.PendingBranches.GetEnumerator()
			While enumerator.MoveNext()
				Dim current As AbstractFlowPass(Of ControlFlowPass.LocalState).PendingBranch = enumerator.Current
				Dim syntax As SyntaxNode = current.Branch.Syntax
				If (current.Branch.Kind <> BoundKind.YieldStatement) Then
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BranchOutOfFinally
					syntaxNodeOrToken = If(syntax.Kind() <> SyntaxKind.GoToStatement, syntax, DirectCast(syntax, GoToStatementSyntax).Label)
				Else
					eRRID = Microsoft.CodeAnalysis.VisualBasic.ERRID.ERR_BadYieldInTryHandler
					syntaxNodeOrToken = syntax
				End If
				Me.diagnostics.Add(eRRID, syntaxNodeOrToken.GetLocation())
			End While
			MyBase.RestorePending(savedPending, False)
		End Sub

		Protected Overrides Sub VisitStatement(ByVal statement As BoundStatement)
			Dim kind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = statement.Kind
			If (kind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.NoOpStatement AndAlso kind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.Block AndAlso kind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.LabelStatement AndAlso Not Me.State.Alive AndAlso Not Me.State.Reported) Then
				Dim boundKind As Microsoft.CodeAnalysis.VisualBasic.BoundKind = statement.Kind
				If (boundKind = Microsoft.CodeAnalysis.VisualBasic.BoundKind.ReturnStatement) Then
					If (Not TryCast(statement, BoundReturnStatement).IsEndOfMethodReturn()) Then
						Me.State.Reported = True
					End If
				ElseIf (boundKind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.LocalDeclaration) Then
					If (boundKind <> Microsoft.CodeAnalysis.VisualBasic.BoundKind.DimStatement) Then
						Me.State.Reported = True
					End If
				ElseIf (TryCast(statement, BoundLocalDeclaration).InitializerOpt IsNot Nothing) Then
					Me.State.Reported = True
				End If
			End If
			MyBase.VisitStatement(statement)
		End Sub

		Protected Overrides Sub VisitTryBlock(ByVal tryBlock As BoundStatement, ByVal node As BoundTryStatement, ByRef tryState As ControlFlowPass.LocalState)
			If (node.CatchBlocks.IsEmpty) Then
				MyBase.VisitTryBlock(tryBlock, node, tryState)
				Return
			End If
			Dim savedPending As AbstractFlowPass(Of ControlFlowPass.LocalState).SavedPending = MyBase.SavePending()
			MyBase.VisitTryBlock(tryBlock, node, tryState)
			MyBase.RestorePending(savedPending, True)
		End Sub

		Friend Structure LocalState
			Implements AbstractFlowPass(Of ControlFlowPass.LocalState).AbstractLocalState
			Friend Alive As Boolean

			Friend Reported As Boolean

			Public Sub New(ByVal live As Boolean, ByVal reported As Boolean)
				Me = New ControlFlowPass.LocalState() With
				{
					.Alive = live,
					.Reported = reported
				}
			End Sub

			Public Function Clone() As ControlFlowPass.LocalState Implements AbstractFlowPass(Of ControlFlowPass.LocalState).AbstractLocalState.Clone
				Return Me
			End Function
		End Structure
	End Class
End Namespace