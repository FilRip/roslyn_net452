Imports System
Imports System.Collections.Immutable

Namespace Microsoft.CodeAnalysis.VisualBasic
	Friend MustInherit Class BoundTreeWalker
		Inherits BoundTreeVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overrides Function VisitAddHandlerStatement(ByVal node As BoundAddHandlerStatement) As BoundNode
			Me.Visit(node.EventAccess)
			Me.Visit(node.Handler)
			Return Nothing
		End Function

		Public Overrides Function VisitAddressOfOperator(ByVal node As BoundAddressOfOperator) As BoundNode
			Me.Visit(node.MethodGroup)
			Return Nothing
		End Function

		Public Overrides Function VisitAggregateClause(ByVal node As BoundAggregateClause) As BoundNode
			Me.Visit(node.CapturedGroupOpt)
			Me.Visit(node.GroupPlaceholderOpt)
			Me.Visit(node.UnderlyingExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitAnonymousTypeCreationExpression(ByVal node As BoundAnonymousTypeCreationExpression) As BoundNode
			Me.VisitList(Of BoundAnonymousTypePropertyAccess)(node.Declarations)
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitAnonymousTypeFieldInitializer(ByVal node As BoundAnonymousTypeFieldInitializer) As BoundNode
			Me.Visit(node.Value)
			Return Nothing
		End Function

		Public Overrides Function VisitAnonymousTypePropertyAccess(ByVal node As BoundAnonymousTypePropertyAccess) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitArrayAccess(ByVal node As BoundArrayAccess) As BoundNode
			Me.Visit(node.Expression)
			Me.VisitList(Of BoundExpression)(node.Indices)
			Return Nothing
		End Function

		Public Overrides Function VisitArrayCreation(ByVal node As BoundArrayCreation) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Bounds)
			Me.Visit(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitArrayInitialization(ByVal node As BoundArrayInitialization) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Initializers)
			Return Nothing
		End Function

		Public Overrides Function VisitArrayLength(ByVal node As BoundArrayLength) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitArrayLiteral(ByVal node As BoundArrayLiteral) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Bounds)
			Me.Visit(node.Initializer)
			Return Nothing
		End Function

		Public Overrides Function VisitAsNewLocalDeclarations(ByVal node As BoundAsNewLocalDeclarations) As BoundNode
			Me.VisitList(Of BoundLocalDeclaration)(node.LocalDeclarations)
			Me.Visit(node.Initializer)
			Return Nothing
		End Function

		Public Overrides Function VisitAssignmentOperator(ByVal node As BoundAssignmentOperator) As BoundNode
			Me.Visit(node.Left)
			Me.Visit(node.LeftOnTheRightOpt)
			Me.Visit(node.Right)
			Return Nothing
		End Function

		Public Overrides Function VisitAttribute(ByVal node As BoundAttribute) As BoundNode
			Me.VisitList(Of BoundExpression)(node.ConstructorArguments)
			Me.VisitList(Of BoundExpression)(node.NamedArguments)
			Return Nothing
		End Function

		Public Overrides Function VisitAwaitOperator(ByVal node As BoundAwaitOperator) As BoundNode
			Me.Visit(node.Operand)
			Me.Visit(node.AwaitableInstancePlaceholder)
			Me.Visit(node.GetAwaiter)
			Me.Visit(node.AwaiterInstancePlaceholder)
			Me.Visit(node.IsCompleted)
			Me.Visit(node.GetResult)
			Return Nothing
		End Function

		Public Overrides Function VisitBadExpression(ByVal node As BoundBadExpression) As BoundNode
			Me.VisitList(Of BoundExpression)(node.ChildBoundNodes)
			Return Nothing
		End Function

		Public Overrides Function VisitBadStatement(ByVal node As BoundBadStatement) As BoundNode
			Me.VisitList(Of BoundNode)(node.ChildBoundNodes)
			Return Nothing
		End Function

		Public Overrides Function VisitBadVariable(ByVal node As BoundBadVariable) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitBinaryConditionalExpression(ByVal node As BoundBinaryConditionalExpression) As BoundNode
			Me.Visit(node.TestExpression)
			Me.Visit(node.ElseExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitBinaryOperator(ByVal node As BoundBinaryOperator) As BoundNode
			Me.Visit(node.Left)
			Me.Visit(node.Right)
			Return Nothing
		End Function

		Public Overrides Function VisitBlock(ByVal node As BoundBlock) As BoundNode
			Me.VisitList(Of BoundStatement)(node.Statements)
			Return Nothing
		End Function

		Public Overrides Function VisitByRefArgumentPlaceholder(ByVal node As BoundByRefArgumentPlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitByRefArgumentWithCopyBack(ByVal node As BoundByRefArgumentWithCopyBack) As BoundNode
			Me.Visit(node.OriginalArgument)
			Me.Visit(node.InConversion)
			Me.Visit(node.InPlaceholder)
			Me.Visit(node.OutConversion)
			Me.Visit(node.OutPlaceholder)
			Return Nothing
		End Function

		Public Overrides Function VisitCall(ByVal node As BoundCall) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitCaseBlock(ByVal node As BoundCaseBlock) As BoundNode
			Me.Visit(node.CaseStatement)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitCaseStatement(ByVal node As BoundCaseStatement) As BoundNode
			Me.VisitList(Of BoundCaseClause)(node.CaseClauses)
			Me.Visit(node.ConditionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitCatchBlock(ByVal node As BoundCatchBlock) As BoundNode
			Me.Visit(node.ExceptionSourceOpt)
			Me.Visit(node.ErrorLineNumberOpt)
			Me.Visit(node.ExceptionFilterOpt)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitCollectionInitializerExpression(ByVal node As BoundCollectionInitializerExpression) As BoundNode
			Me.Visit(node.PlaceholderOpt)
			Me.VisitList(Of BoundExpression)(node.Initializers)
			Return Nothing
		End Function

		Public Overrides Function VisitComplexConditionalAccessReceiver(ByVal node As BoundComplexConditionalAccessReceiver) As BoundNode
			Me.Visit(node.ValueTypeReceiver)
			Me.Visit(node.ReferenceTypeReceiver)
			Return Nothing
		End Function

		Public Overrides Function VisitCompoundAssignmentTargetPlaceholder(ByVal node As BoundCompoundAssignmentTargetPlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitConditionalAccess(ByVal node As BoundConditionalAccess) As BoundNode
			Me.Visit(node.Receiver)
			Me.Visit(node.Placeholder)
			Me.Visit(node.AccessExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitConditionalAccessReceiverPlaceholder(ByVal node As BoundConditionalAccessReceiverPlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitConditionalGoto(ByVal node As BoundConditionalGoto) As BoundNode
			Me.Visit(node.Condition)
			Return Nothing
		End Function

		Public Overrides Function VisitContinueStatement(ByVal node As BoundContinueStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitConversion(ByVal node As BoundConversion) As BoundNode
			Me.Visit(node.Operand)
			Me.Visit(node.ExtendedInfoOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitConvertedTupleElements(ByVal node As BoundConvertedTupleElements) As BoundNode
			Me.VisitList(Of BoundRValuePlaceholder)(node.ElementPlaceholders)
			Me.VisitList(Of BoundExpression)(node.ConvertedElements)
			Return Nothing
		End Function

		Public Overrides Function VisitConvertedTupleLiteral(ByVal node As BoundConvertedTupleLiteral) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitDelegateCreationExpression(ByVal node As BoundDelegateCreationExpression) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Me.Visit(node.RelaxationLambdaOpt)
			Me.Visit(node.RelaxationReceiverPlaceholderOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitDimStatement(ByVal node As BoundDimStatement) As BoundNode
			Me.VisitList(Of BoundLocalDeclarationBase)(node.LocalDeclarations)
			Me.Visit(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitDirectCast(ByVal node As BoundDirectCast) As BoundNode
			Me.Visit(node.Operand)
			Me.Visit(node.RelaxationLambdaOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitDoLoopStatement(ByVal node As BoundDoLoopStatement) As BoundNode
			Me.Visit(node.TopConditionOpt)
			Me.Visit(node.BottomConditionOpt)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitDup(ByVal node As BoundDup) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitEndStatement(ByVal node As BoundEndStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitEraseStatement(ByVal node As BoundEraseStatement) As BoundNode
			Me.VisitList(Of BoundAssignmentOperator)(node.Clauses)
			Return Nothing
		End Function

		Public Overrides Function VisitEventAccess(ByVal node As BoundEventAccess) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitExitStatement(ByVal node As BoundExitStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitExpressionStatement(ByVal node As BoundExpressionStatement) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitFieldAccess(ByVal node As BoundFieldAccess) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitFieldInfo(ByVal node As BoundFieldInfo) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitFieldInitializer(ByVal node As BoundFieldInitializer) As BoundNode
			Me.Visit(node.MemberAccessExpressionOpt)
			Me.Visit(node.InitialValue)
			Return Nothing
		End Function

		Public Overrides Function VisitForEachStatement(ByVal node As BoundForEachStatement) As BoundNode
			Me.Visit(node.Collection)
			Me.Visit(node.ControlVariable)
			Me.Visit(node.Body)
			Me.VisitList(Of BoundExpression)(node.NextVariablesOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitForToStatement(ByVal node As BoundForToStatement) As BoundNode
			Me.Visit(node.InitialValue)
			Me.Visit(node.LimitValue)
			Me.Visit(node.StepValue)
			Me.Visit(node.OperatorsOpt)
			Me.Visit(node.ControlVariable)
			Me.Visit(node.Body)
			Me.VisitList(Of BoundExpression)(node.NextVariablesOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitForToUserDefinedOperators(ByVal node As BoundForToUserDefinedOperators) As BoundNode
			Me.Visit(node.LeftOperandPlaceholder)
			Me.Visit(node.RightOperandPlaceholder)
			Me.Visit(node.Addition)
			Me.Visit(node.Subtraction)
			Me.Visit(node.LessThanOrEqual)
			Me.Visit(node.GreaterThanOrEqual)
			Return Nothing
		End Function

		Public Overrides Function VisitGetType(ByVal node As BoundGetType) As BoundNode
			Me.Visit(node.SourceType)
			Return Nothing
		End Function

		Public Overrides Function VisitGlobalStatementInitializer(ByVal node As BoundGlobalStatementInitializer) As BoundNode
			Me.Visit(node.Statement)
			Return Nothing
		End Function

		Public Overrides Function VisitGotoStatement(ByVal node As BoundGotoStatement) As BoundNode
			Me.Visit(node.LabelExpressionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitGroupAggregation(ByVal node As BoundGroupAggregation) As BoundNode
			Me.Visit(node.Group)
			Return Nothing
		End Function

		Public Overrides Function VisitGroupTypeInferenceLambda(ByVal node As GroupTypeInferenceLambda) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitHostObjectMemberReference(ByVal node As BoundHostObjectMemberReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitIfStatement(ByVal node As BoundIfStatement) As BoundNode
			Me.Visit(node.Condition)
			Me.Visit(node.Consequence)
			Me.Visit(node.AlternativeOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitInitializer(ByVal node As BoundInitializer) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitInstrumentationPayloadRoot(ByVal node As BoundInstrumentationPayloadRoot) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitInterpolatedStringExpression(ByVal node As BoundInterpolatedStringExpression) As BoundNode
			Me.VisitList(Of BoundNode)(node.Contents)
			Return Nothing
		End Function

		Public Overrides Function VisitInterpolation(ByVal node As BoundInterpolation) As BoundNode
			Me.Visit(node.Expression)
			Me.Visit(node.AlignmentOpt)
			Me.Visit(node.FormatStringOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitLabel(ByVal node As BoundLabel) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLabelStatement(ByVal node As BoundLabelStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLambda(ByVal node As BoundLambda) As BoundNode
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitLateAddressOfOperator(ByVal node As BoundLateAddressOfOperator) As BoundNode
			Me.Visit(node.MemberAccess)
			Return Nothing
		End Function

		Public Overrides Function VisitLateBoundArgumentSupportingAssignmentWithCapture(ByVal node As BoundLateBoundArgumentSupportingAssignmentWithCapture) As BoundNode
			Me.Visit(node.OriginalArgument)
			Return Nothing
		End Function

		Public Overrides Function VisitLateInvocation(ByVal node As BoundLateInvocation) As BoundNode
			Me.Visit(node.Member)
			Me.VisitList(Of BoundExpression)(node.ArgumentsOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitLateMemberAccess(ByVal node As BoundLateMemberAccess) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Me.Visit(node.TypeArgumentsOpt)
			Return Nothing
		End Function

		Public Overridable Sub VisitList(Of T As BoundNode)(ByVal list As ImmutableArray(Of T))
			If (Not list.IsDefault) Then
				Dim enumerator As ImmutableArray(Of T).Enumerator = list.GetEnumerator()
				While enumerator.MoveNext()
					Me.Visit(DirectCast(enumerator.Current, BoundNode))
				End While
			End If
		End Sub

		Public Overrides Function VisitLiteral(ByVal node As BoundLiteral) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLocal(ByVal node As BoundLocal) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLocalDeclaration(ByVal node As BoundLocalDeclaration) As BoundNode
			Me.Visit(node.DeclarationInitializerOpt)
			Me.Visit(node.IdentifierInitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitLoweredConditionalAccess(ByVal node As BoundLoweredConditionalAccess) As BoundNode
			Me.Visit(node.ReceiverOrCondition)
			Me.Visit(node.WhenNotNull)
			Me.Visit(node.WhenNullOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitLValuePlaceholder(ByVal node As BoundLValuePlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitLValueToRValueWrapper(ByVal node As BoundLValueToRValueWrapper) As BoundNode
			Me.Visit(node.UnderlyingLValue)
			Return Nothing
		End Function

		Public Overrides Function VisitMaximumMethodDefIndex(ByVal node As BoundMaximumMethodDefIndex) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMeReference(ByVal node As BoundMeReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMethodDefIndex(ByVal node As BoundMethodDefIndex) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMethodGroup(ByVal node As BoundMethodGroup) As BoundNode
			Me.Visit(node.TypeArgumentsOpt)
			Me.Visit(node.ReceiverOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitMethodInfo(ByVal node As BoundMethodInfo) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMidResult(ByVal node As BoundMidResult) As BoundNode
			Me.Visit(node.Original)
			Me.Visit(node.Start)
			Me.Visit(node.LengthOpt)
			Me.Visit(node.Source)
			Return Nothing
		End Function

		Public Overrides Function VisitModuleVersionId(ByVal node As BoundModuleVersionId) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitModuleVersionIdString(ByVal node As BoundModuleVersionIdString) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMyBaseReference(ByVal node As BoundMyBaseReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitMyClassReference(ByVal node As BoundMyClassReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitNameOfOperator(ByVal node As BoundNameOfOperator) As BoundNode
			Me.Visit(node.Argument)
			Return Nothing
		End Function

		Public Overrides Function VisitNamespaceExpression(ByVal node As BoundNamespaceExpression) As BoundNode
			Me.Visit(node.UnevaluatedReceiverOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitNewT(ByVal node As BoundNewT) As BoundNode
			Me.Visit(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitNoOpStatement(ByVal node As BoundNoOpStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitNoPiaObjectCreationExpression(ByVal node As BoundNoPiaObjectCreationExpression) As BoundNode
			Me.Visit(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitNullableIsTrueOperator(ByVal node As BoundNullableIsTrueOperator) As BoundNode
			Me.Visit(node.Operand)
			Return Nothing
		End Function

		Public Overrides Function VisitObjectCreationExpression(ByVal node As BoundObjectCreationExpression) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Me.Visit(node.InitializerOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitObjectInitializerExpression(ByVal node As BoundObjectInitializerExpression) As BoundNode
			Me.Visit(node.PlaceholderOpt)
			Me.VisitList(Of BoundExpression)(node.Initializers)
			Return Nothing
		End Function

		Public Overrides Function VisitOmittedArgument(ByVal node As BoundOmittedArgument) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitOnErrorStatement(ByVal node As BoundOnErrorStatement) As BoundNode
			Me.Visit(node.LabelExpressionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitOrdering(ByVal node As BoundOrdering) As BoundNode
			Me.Visit(node.UnderlyingExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitParameter(ByVal node As BoundParameter) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitParameterEqualsValue(ByVal node As BoundParameterEqualsValue) As BoundNode
			Me.Visit(node.Value)
			Return Nothing
		End Function

		Public Overrides Function VisitParenthesized(ByVal node As BoundParenthesized) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitPreviousSubmissionReference(ByVal node As BoundPreviousSubmissionReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitPropertyAccess(ByVal node As BoundPropertyAccess) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitPropertyGroup(ByVal node As BoundPropertyGroup) As BoundNode
			Me.Visit(node.ReceiverOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitPropertyInitializer(ByVal node As BoundPropertyInitializer) As BoundNode
			Me.Visit(node.MemberAccessExpressionOpt)
			Me.Visit(node.InitialValue)
			Return Nothing
		End Function

		Public Overrides Function VisitPseudoVariable(ByVal node As BoundPseudoVariable) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitQueryableSource(ByVal node As BoundQueryableSource) As BoundNode
			Me.Visit(node.Source)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryClause(ByVal node As BoundQueryClause) As BoundNode
			Me.Visit(node.UnderlyingExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryExpression(ByVal node As BoundQueryExpression) As BoundNode
			Me.Visit(node.LastOperator)
			Return Nothing
		End Function

		Public Overrides Function VisitQueryLambda(ByVal node As BoundQueryLambda) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitQuerySource(ByVal node As BoundQuerySource) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitRaiseEventStatement(ByVal node As BoundRaiseEventStatement) As BoundNode
			Me.Visit(node.EventInvocation)
			Return Nothing
		End Function

		Public Overrides Function VisitRangeCaseClause(ByVal node As BoundRangeCaseClause) As BoundNode
			Me.Visit(node.LowerBoundOpt)
			Me.Visit(node.UpperBoundOpt)
			Me.Visit(node.LowerBoundConditionOpt)
			Me.Visit(node.UpperBoundConditionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariable(ByVal node As BoundRangeVariable) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitRangeVariableAssignment(ByVal node As BoundRangeVariableAssignment) As BoundNode
			Me.Visit(node.Value)
			Return Nothing
		End Function

		Public Overrides Function VisitRedimClause(ByVal node As BoundRedimClause) As BoundNode
			Me.Visit(node.Operand)
			Me.VisitList(Of BoundExpression)(node.Indices)
			Return Nothing
		End Function

		Public Overrides Function VisitRedimStatement(ByVal node As BoundRedimStatement) As BoundNode
			Me.VisitList(Of BoundRedimClause)(node.Clauses)
			Return Nothing
		End Function

		Public Overrides Function VisitReferenceAssignment(ByVal node As BoundReferenceAssignment) As BoundNode
			Me.Visit(node.ByRefLocal)
			Me.Visit(node.LValue)
			Return Nothing
		End Function

		Public Overrides Function VisitRelationalCaseClause(ByVal node As BoundRelationalCaseClause) As BoundNode
			Me.Visit(node.ValueOpt)
			Me.Visit(node.ConditionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitRelaxationLambda(ByVal node As BoundRelaxationLambda) As BoundNode
			Me.Visit(node.Lambda)
			Me.Visit(node.ReceiverPlaceholderOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitRemoveHandlerStatement(ByVal node As BoundRemoveHandlerStatement) As BoundNode
			Me.Visit(node.EventAccess)
			Me.Visit(node.Handler)
			Return Nothing
		End Function

		Public Overrides Function VisitResumeStatement(ByVal node As BoundResumeStatement) As BoundNode
			Me.Visit(node.LabelExpressionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitReturnStatement(ByVal node As BoundReturnStatement) As BoundNode
			Me.Visit(node.ExpressionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitRValuePlaceholder(ByVal node As BoundRValuePlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitSelectStatement(ByVal node As BoundSelectStatement) As BoundNode
			Me.Visit(node.ExpressionStatement)
			Me.Visit(node.ExprPlaceholderOpt)
			Me.VisitList(Of BoundCaseBlock)(node.CaseBlocks)
			Return Nothing
		End Function

		Public Overrides Function VisitSequence(ByVal node As BoundSequence) As BoundNode
			Me.VisitList(Of BoundExpression)(node.SideEffects)
			Me.Visit(node.ValueOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitSequencePoint(ByVal node As BoundSequencePoint) As BoundNode
			Me.Visit(node.StatementOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitSequencePointExpression(ByVal node As BoundSequencePointExpression) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitSequencePointWithSpan(ByVal node As BoundSequencePointWithSpan) As BoundNode
			Me.Visit(node.StatementOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitSimpleCaseClause(ByVal node As BoundSimpleCaseClause) As BoundNode
			Me.Visit(node.ValueOpt)
			Me.Visit(node.ConditionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitSourceDocumentIndex(ByVal node As BoundSourceDocumentIndex) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitSpillSequence(ByVal node As BoundSpillSequence) As BoundNode
			Me.VisitList(Of BoundStatement)(node.Statements)
			Me.Visit(node.ValueOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitStateMachineScope(ByVal node As BoundStateMachineScope) As BoundNode
			Me.Visit(node.Statement)
			Return Nothing
		End Function

		Public Overrides Function VisitStatementList(ByVal node As BoundStatementList) As BoundNode
			Me.VisitList(Of BoundStatement)(node.Statements)
			Return Nothing
		End Function

		Public Overrides Function VisitStopStatement(ByVal node As BoundStopStatement) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitSyncLockStatement(ByVal node As BoundSyncLockStatement) As BoundNode
			Me.Visit(node.LockExpression)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitTernaryConditionalExpression(ByVal node As BoundTernaryConditionalExpression) As BoundNode
			Me.Visit(node.Condition)
			Me.Visit(node.WhenTrue)
			Me.Visit(node.WhenFalse)
			Return Nothing
		End Function

		Public Overrides Function VisitThrowStatement(ByVal node As BoundThrowStatement) As BoundNode
			Me.Visit(node.ExpressionOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitToQueryableCollectionConversion(ByVal node As BoundToQueryableCollectionConversion) As BoundNode
			Me.Visit(node.ConversionCall)
			Return Nothing
		End Function

		Public Overrides Function VisitTryCast(ByVal node As BoundTryCast) As BoundNode
			Me.Visit(node.Operand)
			Me.Visit(node.RelaxationLambdaOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitTryStatement(ByVal node As BoundTryStatement) As BoundNode
			Me.Visit(node.TryBlock)
			Me.VisitList(Of BoundCatchBlock)(node.CatchBlocks)
			Me.Visit(node.FinallyBlockOpt)
			Return Nothing
		End Function

		Public Overrides Function VisitTupleLiteral(ByVal node As BoundTupleLiteral) As BoundNode
			Me.VisitList(Of BoundExpression)(node.Arguments)
			Return Nothing
		End Function

		Public Overrides Function VisitTypeArguments(ByVal node As BoundTypeArguments) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitTypeAsValueExpression(ByVal node As BoundTypeAsValueExpression) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitTypeExpression(ByVal node As BoundTypeExpression) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitTypeOf(ByVal node As BoundTypeOf) As BoundNode
			Me.Visit(node.Operand)
			Return Nothing
		End Function

		Public Overrides Function VisitTypeOrValueExpression(ByVal node As BoundTypeOrValueExpression) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitUnaryOperator(ByVal node As BoundUnaryOperator) As BoundNode
			Me.Visit(node.Operand)
			Return Nothing
		End Function

		Public Overrides Function VisitUnboundLambda(ByVal node As UnboundLambda) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingCatchFilter(ByVal node As BoundUnstructuredExceptionHandlingCatchFilter) As BoundNode
			Me.Visit(node.ActiveHandlerLocal)
			Me.Visit(node.ResumeTargetLocal)
			Return Nothing
		End Function

		Public Overrides Function VisitUnstructuredExceptionHandlingStatement(ByVal node As BoundUnstructuredExceptionHandlingStatement) As BoundNode
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitUnstructuredExceptionOnErrorSwitch(ByVal node As BoundUnstructuredExceptionOnErrorSwitch) As BoundNode
			Me.Visit(node.Value)
			Me.VisitList(Of BoundGotoStatement)(node.Jumps)
			Return Nothing
		End Function

		Public Overrides Function VisitUnstructuredExceptionResumeSwitch(ByVal node As BoundUnstructuredExceptionResumeSwitch) As BoundNode
			Me.Visit(node.ResumeTargetTemporary)
			Me.Visit(node.ResumeLabel)
			Me.Visit(node.ResumeNextLabel)
			Me.VisitList(Of BoundGotoStatement)(node.Jumps)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedBinaryOperator(ByVal node As BoundUserDefinedBinaryOperator) As BoundNode
			Me.Visit(node.UnderlyingExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedConversion(ByVal node As BoundUserDefinedConversion) As BoundNode
			Me.Visit(node.UnderlyingExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedShortCircuitingOperator(ByVal node As BoundUserDefinedShortCircuitingOperator) As BoundNode
			Me.Visit(node.LeftOperand)
			Me.Visit(node.LeftOperandPlaceholder)
			Me.Visit(node.LeftTest)
			Me.Visit(node.BitwiseOperator)
			Return Nothing
		End Function

		Public Overrides Function VisitUserDefinedUnaryOperator(ByVal node As BoundUserDefinedUnaryOperator) As BoundNode
			Me.Visit(node.UnderlyingExpression)
			Return Nothing
		End Function

		Public Overrides Function VisitUsingStatement(ByVal node As BoundUsingStatement) As BoundNode
			Me.VisitList(Of BoundLocalDeclarationBase)(node.ResourceList)
			Me.Visit(node.ResourceExpressionOpt)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitValueTypeMeReference(ByVal node As BoundValueTypeMeReference) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitWhileStatement(ByVal node As BoundWhileStatement) As BoundNode
			Me.Visit(node.Condition)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitWithLValueExpressionPlaceholder(ByVal node As BoundWithLValueExpressionPlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitWithRValueExpressionPlaceholder(ByVal node As BoundWithRValueExpressionPlaceholder) As BoundNode
			Return Nothing
		End Function

		Public Overrides Function VisitWithStatement(ByVal node As BoundWithStatement) As BoundNode
			Me.Visit(node.OriginalExpression)
			Me.Visit(node.Body)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlAttribute(ByVal node As BoundXmlAttribute) As BoundNode
			Me.Visit(node.Name)
			Me.Visit(node.Value)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlCData(ByVal node As BoundXmlCData) As BoundNode
			Me.Visit(node.Value)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlComment(ByVal node As BoundXmlComment) As BoundNode
			Me.Visit(node.Value)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlDeclaration(ByVal node As BoundXmlDeclaration) As BoundNode
			Me.Visit(node.Version)
			Me.Visit(node.Encoding)
			Me.Visit(node.Standalone)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlDocument(ByVal node As BoundXmlDocument) As BoundNode
			Me.Visit(node.Declaration)
			Me.VisitList(Of BoundExpression)(node.ChildNodes)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlElement(ByVal node As BoundXmlElement) As BoundNode
			Me.Visit(node.Argument)
			Me.VisitList(Of BoundExpression)(node.ChildNodes)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlEmbeddedExpression(ByVal node As BoundXmlEmbeddedExpression) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlMemberAccess(ByVal node As BoundXmlMemberAccess) As BoundNode
			Me.Visit(node.MemberAccess)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlName(ByVal node As BoundXmlName) As BoundNode
			Me.Visit(node.XmlNamespace)
			Me.Visit(node.LocalName)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlNamespace(ByVal node As BoundXmlNamespace) As BoundNode
			Me.Visit(node.XmlNamespace)
			Return Nothing
		End Function

		Public Overrides Function VisitXmlProcessingInstruction(ByVal node As BoundXmlProcessingInstruction) As BoundNode
			Me.Visit(node.Target)
			Me.Visit(node.Data)
			Return Nothing
		End Function

		Public Overrides Function VisitYieldStatement(ByVal node As BoundYieldStatement) As BoundNode
			Me.Visit(node.Expression)
			Return Nothing
		End Function
	End Class
End Namespace