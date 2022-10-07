Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.VisualBasic.CompilerServices
Imports System
Imports System.Diagnostics
Imports System.Linq
Imports System.Runtime.CompilerServices

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Protected Overridable Function ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException() As Boolean
			Return True
		End Function

		<DebuggerHidden>
		Public Overridable Function DefaultVisit(ByVal node As BoundNode) As BoundNode
			Return Nothing
		End Function

		<DebuggerHidden>
		Public Overridable Function Visit(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundNode) As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			Dim boundNode As Microsoft.CodeAnalysis.VisualBasic.BoundNode
			If (node Is Nothing) Then
				boundNode = Nothing
			Else
				boundNode = node.Accept(Me)
			End If
			Return boundNode
		End Function

		Public Overridable Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAggregateClause(ByVal node As BoundAggregateClause) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayAccess(ByVal node As BoundArrayAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayCreation(ByVal node As BoundArrayCreation) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayInitialization(ByVal node As BoundArrayInitialization) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayLength(ByVal node As BoundArrayLength) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayLiteral(ByVal node As BoundArrayLiteral) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAttribute(ByVal node As BoundAttribute) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBadStatement(ByVal node As BoundBadStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBadVariable(ByVal node As BoundBadVariable) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBlock(ByVal node As BoundBlock) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCall(ByVal node As BoundCall) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCaseBlock(ByVal node As BoundCaseBlock) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCaseStatement(ByVal node As BoundCaseStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConditionalAccess(ByVal node As BoundConditionalAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConditionalGoto(ByVal node As BoundConditionalGoto) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitContinueStatement(ByVal node As BoundContinueStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConversion(ByVal node As BoundConversion) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConvertedTupleElements(ByVal node As BoundConvertedTupleElements) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDimStatement(ByVal node As BoundDimStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDirectCast(ByVal node As BoundDirectCast) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDup(ByVal node As BoundDup) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEndStatement(ByVal node As BoundEndStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEraseStatement(ByVal node As BoundEraseStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEventAccess(ByVal node As BoundEventAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExitStatement(ByVal node As BoundExitStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExpressionStatement(ByVal node As BoundExpressionStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Protected MustOverride Function VisitExpressionWithoutStackGuard(ByVal node As BoundExpression) As BoundExpression

		<DebuggerStepThrough>
		Protected Function VisitExpressionWithStackGuard(ByRef recursionDepth As Integer, ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			recursionDepth = recursionDepth + 1
			If (recursionDepth > 1 OrElse Not Me.ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()) Then
				StackGuard.EnsureSufficientExecutionStack(recursionDepth)
				boundExpression = Me.VisitExpressionWithoutStackGuard(node)
			Else
				boundExpression = Me.VisitExpressionWithStackGuard(node)
			End If
			recursionDepth = recursionDepth - 1
			Return boundExpression
		End Function

		<DebuggerStepThrough>
		Private Function VisitExpressionWithStackGuard(ByVal node As Microsoft.CodeAnalysis.VisualBasic.BoundExpression) As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Dim boundExpression As Microsoft.CodeAnalysis.VisualBasic.BoundExpression
			Try
				boundExpression = Me.VisitExpressionWithoutStackGuard(node)
			Catch insufficientExecutionStackException As System.InsufficientExecutionStackException
				ProjectData.SetProjectError(insufficientExecutionStackException)
				Throw New BoundTreeVisitor.CancelledByStackGuardException(insufficientExecutionStackException, node)
			End Try
			Return boundExpression
		End Function

		Public Overridable Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFieldInfo(ByVal node As BoundFieldInfo) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFieldInitializer(ByVal node As BoundFieldInitializer) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForToStatement(ByVal node As BoundForToStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForToUserDefinedOperators(ByVal node As BoundForToUserDefinedOperators) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGetType(ByVal node As BoundGetType) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGlobalStatementInitializer(ByVal node As BoundGlobalStatementInitializer) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGotoStatement(ByVal node As BoundGotoStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGroupAggregation(ByVal node As BoundGroupAggregation) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGroupTypeInferenceLambda(ByVal node As GroupTypeInferenceLambda) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitHostObjectMemberReference(ByVal node As BoundHostObjectMemberReference) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitIfStatement(ByVal node As BoundIfStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInitializer(ByVal node As BoundInitializer) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInstrumentationPayloadRoot(ByVal node As BoundInstrumentationPayloadRoot) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolation(ByVal node As BoundInterpolation) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLabel(ByVal node As BoundLabel) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLateBoundArgumentSupportingAssignmentWithCapture(ByVal node As BoundLateBoundArgumentSupportingAssignmentWithCapture) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLateInvocation(ByVal node As BoundLateInvocation) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLiteral(ByVal node As BoundLiteral) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLocal(ByVal node As BoundLocal) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMaximumMethodDefIndex(ByVal node As BoundMaximumMethodDefIndex) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMethodDefIndex(ByVal node As BoundMethodDefIndex) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMethodInfo(ByVal node As BoundMethodInfo) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMidResult(ByVal node As BoundMidResult) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitModuleVersionId(ByVal node As BoundModuleVersionId) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitModuleVersionIdString(ByVal node As BoundModuleVersionIdString) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNameOfOperator(ByVal node As BoundNameOfOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNamespaceExpression(ByVal node As BoundNamespaceExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNewT(ByVal node As BoundNewT) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNoOpStatement(ByVal node As BoundNoOpStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOmittedArgument(ByVal node As BoundOmittedArgument) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOrdering(ByVal node As BoundOrdering) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitParameter(ByVal node As BoundParameter) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitParameterEqualsValue(ByVal node As BoundParameterEqualsValue) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitParenthesized(ByVal node As BoundParenthesized) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPreviousSubmissionReference(ByVal node As BoundPreviousSubmissionReference) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPropertyGroup(ByVal node As BoundPropertyGroup) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPseudoVariable(ByVal node As BoundPseudoVariable) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQuerySource(ByVal node As BoundQuerySource) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRangeCaseClause(ByVal node As BoundRangeCaseClause) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRedimClause(ByVal node As BoundRedimClause) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRedimStatement(ByVal node As BoundRedimStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRelationalCaseClause(ByVal node As BoundRelationalCaseClause) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRelaxationLambda(ByVal node As BoundRelaxationLambda) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitResumeStatement(ByVal node As BoundResumeStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSequence(ByVal node As BoundSequence) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSequencePoint(ByVal node As BoundSequencePoint) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSequencePointExpression(ByVal node As BoundSequencePointExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSimpleCaseClause(ByVal node As BoundSimpleCaseClause) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSpillSequence(ByVal node As BoundSpillSequence) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitStateMachineScope(ByVal node As BoundStateMachineScope) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitStatementList(ByVal node As BoundStatementList) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitStopStatement(ByVal node As BoundStopStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitThrowStatement(ByVal node As BoundThrowStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTryCast(ByVal node As BoundTryCast) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTupleLiteral(ByVal node As BoundTupleLiteral) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeArguments(ByVal node As BoundTypeArguments) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeAsValueExpression(ByVal node As BoundTypeAsValueExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeExpression(ByVal node As BoundTypeExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeOf(ByVal node As BoundTypeOf) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeOrValueExpression(ByVal node As BoundTypeOrValueExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnstructuredExceptionHandlingCatchFilter(ByVal node As BoundUnstructuredExceptionHandlingCatchFilter) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUsingStatement(ByVal node As BoundUsingStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitValueTypeMeReference(ByVal node As BoundValueTypeMeReference) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWhileStatement(ByVal node As BoundWhileStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithStatement(ByVal node As BoundWithStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlAttribute(ByVal node As BoundXmlAttribute) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlCData(ByVal node As BoundXmlCData) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlComment(ByVal node As BoundXmlComment) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlDocument(ByVal node As BoundXmlDocument) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlElement(ByVal node As BoundXmlElement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlName(ByVal node As BoundXmlName) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlNamespace(ByVal node As BoundXmlNamespace) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitYieldStatement(ByVal node As BoundYieldStatement) As BoundNode
			Return Me.DefaultVisit(node)
		End Function

		Public Class CancelledByStackGuardException
			Inherits Exception
			Public ReadOnly Node As BoundNode

			Public Sub New(ByVal inner As Exception, ByVal node As BoundNode)
				MyBase.New(inner.Message, inner)
				Me.Node = node
			End Sub

			Public Sub AddAnError(ByVal diagnostics As DiagnosticBag)
				diagnostics.Add(ERRID.ERR_TooLongOrComplexExpression, BoundTreeVisitor.CancelledByStackGuardException.GetTooLongOrComplexExpressionErrorLocation(Me.Node))
			End Sub

			Public Sub AddAnError(ByVal diagnostics As Microsoft.CodeAnalysis.VisualBasic.BindingDiagnosticBag)
				diagnostics.Add(ERRID.ERR_TooLongOrComplexExpression, BoundTreeVisitor.CancelledByStackGuardException.GetTooLongOrComplexExpressionErrorLocation(Me.Node))
			End Sub

			Public Shared Function GetTooLongOrComplexExpressionErrorLocation(ByVal node As BoundNode) As Location
				Dim func As Func(Of Microsoft.CodeAnalysis.SyntaxNode, Boolean)
				Dim syntax As Microsoft.CodeAnalysis.SyntaxNode = node.Syntax
				If (Not TypeOf syntax Is ExpressionSyntax) Then
					Dim syntaxNode As Microsoft.CodeAnalysis.SyntaxNode = syntax
					If (BoundTreeVisitor.CancelledByStackGuardException._Closure$__.$I4-0 Is Nothing) Then
						func = Function(n As Microsoft.CodeAnalysis.SyntaxNode) Not TypeOf n Is ExpressionSyntax
						BoundTreeVisitor.CancelledByStackGuardException._Closure$__.$I4-0 = func
					Else
						func = BoundTreeVisitor.CancelledByStackGuardException._Closure$__.$I4-0
					End If
					Dim syntaxNode1 As Microsoft.CodeAnalysis.SyntaxNode = syntaxNode.DescendantNodes(func, False).OfType(Of ExpressionSyntax)().FirstOrDefault()
					If (syntaxNode1 Is Nothing) Then
						syntaxNode1 = syntax
					End If
					syntax = syntaxNode1
				End If
				Dim firstToken As SyntaxToken = syntax.GetFirstToken(False, False, False, False)
				Return firstToken.GetLocation()
			End Function
		End Class
	End Class
End Namespace