using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundTreeWalker : BoundTreeVisitor
	{
		public virtual void VisitList<T>(ImmutableArray<T> list) where T : BoundNode
		{
			if (!list.IsDefault)
			{
				ImmutableArray<T>.Enumerator enumerator = list.GetEnumerator();
				while (enumerator.MoveNext())
				{
					T current = enumerator.Current;
					Visit(current);
				}
			}
		}

		public override BoundNode VisitTypeArguments(BoundTypeArguments node)
		{
			return null;
		}

		public override BoundNode VisitOmittedArgument(BoundOmittedArgument node)
		{
			return null;
		}

		public override BoundNode VisitLValueToRValueWrapper(BoundLValueToRValueWrapper node)
		{
			Visit(node.UnderlyingLValue);
			return null;
		}

		public override BoundNode VisitWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitRValuePlaceholder(BoundRValuePlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitLValuePlaceholder(BoundLValuePlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitDup(BoundDup node)
		{
			return null;
		}

		public override BoundNode VisitBadExpression(BoundBadExpression node)
		{
			VisitList(node.ChildBoundNodes);
			return null;
		}

		public override BoundNode VisitBadStatement(BoundBadStatement node)
		{
			VisitList(node.ChildBoundNodes);
			return null;
		}

		public override BoundNode VisitParenthesized(BoundParenthesized node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitBadVariable(BoundBadVariable node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitArrayAccess(BoundArrayAccess node)
		{
			Visit(node.Expression);
			VisitList(node.Indices);
			return null;
		}

		public override BoundNode VisitArrayLength(BoundArrayLength node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitGetType(BoundGetType node)
		{
			Visit(node.SourceType);
			return null;
		}

		public override BoundNode VisitFieldInfo(BoundFieldInfo node)
		{
			return null;
		}

		public override BoundNode VisitMethodInfo(BoundMethodInfo node)
		{
			return null;
		}

		public override BoundNode VisitTypeExpression(BoundTypeExpression node)
		{
			return null;
		}

		public override BoundNode VisitTypeOrValueExpression(BoundTypeOrValueExpression node)
		{
			return null;
		}

		public override BoundNode VisitNamespaceExpression(BoundNamespaceExpression node)
		{
			Visit(node.UnevaluatedReceiverOpt);
			return null;
		}

		public override BoundNode VisitMethodDefIndex(BoundMethodDefIndex node)
		{
			return null;
		}

		public override BoundNode VisitMaximumMethodDefIndex(BoundMaximumMethodDefIndex node)
		{
			return null;
		}

		public override BoundNode VisitInstrumentationPayloadRoot(BoundInstrumentationPayloadRoot node)
		{
			return null;
		}

		public override BoundNode VisitModuleVersionId(BoundModuleVersionId node)
		{
			return null;
		}

		public override BoundNode VisitModuleVersionIdString(BoundModuleVersionIdString node)
		{
			return null;
		}

		public override BoundNode VisitSourceDocumentIndex(BoundSourceDocumentIndex node)
		{
			return null;
		}

		public override BoundNode VisitUnaryOperator(BoundUnaryOperator node)
		{
			Visit(node.Operand);
			return null;
		}

		public override BoundNode VisitUserDefinedUnaryOperator(BoundUserDefinedUnaryOperator node)
		{
			Visit(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitNullableIsTrueOperator(BoundNullableIsTrueOperator node)
		{
			Visit(node.Operand);
			return null;
		}

		public override BoundNode VisitBinaryOperator(BoundBinaryOperator node)
		{
			Visit(node.Left);
			Visit(node.Right);
			return null;
		}

		public override BoundNode VisitUserDefinedBinaryOperator(BoundUserDefinedBinaryOperator node)
		{
			Visit(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitUserDefinedShortCircuitingOperator(BoundUserDefinedShortCircuitingOperator node)
		{
			Visit(node.LeftOperand);
			Visit(node.LeftOperandPlaceholder);
			Visit(node.LeftTest);
			Visit(node.BitwiseOperator);
			return null;
		}

		public override BoundNode VisitCompoundAssignmentTargetPlaceholder(BoundCompoundAssignmentTargetPlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitAssignmentOperator(BoundAssignmentOperator node)
		{
			Visit(node.Left);
			Visit(node.LeftOnTheRightOpt);
			Visit(node.Right);
			return null;
		}

		public override BoundNode VisitReferenceAssignment(BoundReferenceAssignment node)
		{
			Visit(node.ByRefLocal);
			Visit(node.LValue);
			return null;
		}

		public override BoundNode VisitAddressOfOperator(BoundAddressOfOperator node)
		{
			Visit(node.MethodGroup);
			return null;
		}

		public override BoundNode VisitTernaryConditionalExpression(BoundTernaryConditionalExpression node)
		{
			Visit(node.Condition);
			Visit(node.WhenTrue);
			Visit(node.WhenFalse);
			return null;
		}

		public override BoundNode VisitBinaryConditionalExpression(BoundBinaryConditionalExpression node)
		{
			Visit(node.TestExpression);
			Visit(node.ElseExpression);
			return null;
		}

		public override BoundNode VisitConversion(BoundConversion node)
		{
			Visit(node.Operand);
			Visit(node.ExtendedInfoOpt);
			return null;
		}

		public override BoundNode VisitRelaxationLambda(BoundRelaxationLambda node)
		{
			Visit(node.Lambda);
			Visit(node.ReceiverPlaceholderOpt);
			return null;
		}

		public override BoundNode VisitConvertedTupleElements(BoundConvertedTupleElements node)
		{
			VisitList(node.ElementPlaceholders);
			VisitList(node.ConvertedElements);
			return null;
		}

		public override BoundNode VisitUserDefinedConversion(BoundUserDefinedConversion node)
		{
			Visit(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitDirectCast(BoundDirectCast node)
		{
			Visit(node.Operand);
			Visit(node.RelaxationLambdaOpt);
			return null;
		}

		public override BoundNode VisitTryCast(BoundTryCast node)
		{
			Visit(node.Operand);
			Visit(node.RelaxationLambdaOpt);
			return null;
		}

		public override BoundNode VisitTypeOf(BoundTypeOf node)
		{
			Visit(node.Operand);
			return null;
		}

		public override BoundNode VisitSequencePoint(BoundSequencePoint node)
		{
			Visit(node.StatementOpt);
			return null;
		}

		public override BoundNode VisitSequencePointExpression(BoundSequencePointExpression node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitSequencePointWithSpan(BoundSequencePointWithSpan node)
		{
			Visit(node.StatementOpt);
			return null;
		}

		public override BoundNode VisitNoOpStatement(BoundNoOpStatement node)
		{
			return null;
		}

		public override BoundNode VisitMethodGroup(BoundMethodGroup node)
		{
			Visit(node.TypeArgumentsOpt);
			Visit(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitPropertyGroup(BoundPropertyGroup node)
		{
			Visit(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitReturnStatement(BoundReturnStatement node)
		{
			Visit(node.ExpressionOpt);
			return null;
		}

		public override BoundNode VisitYieldStatement(BoundYieldStatement node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitThrowStatement(BoundThrowStatement node)
		{
			Visit(node.ExpressionOpt);
			return null;
		}

		public override BoundNode VisitRedimStatement(BoundRedimStatement node)
		{
			VisitList(node.Clauses);
			return null;
		}

		public override BoundNode VisitRedimClause(BoundRedimClause node)
		{
			Visit(node.Operand);
			VisitList(node.Indices);
			return null;
		}

		public override BoundNode VisitEraseStatement(BoundEraseStatement node)
		{
			VisitList(node.Clauses);
			return null;
		}

		public override BoundNode VisitCall(BoundCall node)
		{
			Visit(node.ReceiverOpt);
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitAttribute(BoundAttribute node)
		{
			VisitList(node.ConstructorArguments);
			VisitList(node.NamedArguments);
			return null;
		}

		public override BoundNode VisitLateMemberAccess(BoundLateMemberAccess node)
		{
			Visit(node.ReceiverOpt);
			Visit(node.TypeArgumentsOpt);
			return null;
		}

		public override BoundNode VisitLateInvocation(BoundLateInvocation node)
		{
			Visit(node.Member);
			VisitList(node.ArgumentsOpt);
			return null;
		}

		public override BoundNode VisitLateAddressOfOperator(BoundLateAddressOfOperator node)
		{
			Visit(node.MemberAccess);
			return null;
		}

		public override BoundNode VisitTupleLiteral(BoundTupleLiteral node)
		{
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitConvertedTupleLiteral(BoundConvertedTupleLiteral node)
		{
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitObjectCreationExpression(BoundObjectCreationExpression node)
		{
			VisitList(node.Arguments);
			Visit(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitNoPiaObjectCreationExpression(BoundNoPiaObjectCreationExpression node)
		{
			Visit(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitAnonymousTypeCreationExpression(BoundAnonymousTypeCreationExpression node)
		{
			VisitList(node.Declarations);
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitAnonymousTypePropertyAccess(BoundAnonymousTypePropertyAccess node)
		{
			return null;
		}

		public override BoundNode VisitAnonymousTypeFieldInitializer(BoundAnonymousTypeFieldInitializer node)
		{
			Visit(node.Value);
			return null;
		}

		public override BoundNode VisitObjectInitializerExpression(BoundObjectInitializerExpression node)
		{
			Visit(node.PlaceholderOpt);
			VisitList(node.Initializers);
			return null;
		}

		public override BoundNode VisitCollectionInitializerExpression(BoundCollectionInitializerExpression node)
		{
			Visit(node.PlaceholderOpt);
			VisitList(node.Initializers);
			return null;
		}

		public override BoundNode VisitNewT(BoundNewT node)
		{
			Visit(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitDelegateCreationExpression(BoundDelegateCreationExpression node)
		{
			Visit(node.ReceiverOpt);
			Visit(node.RelaxationLambdaOpt);
			Visit(node.RelaxationReceiverPlaceholderOpt);
			return null;
		}

		public override BoundNode VisitArrayCreation(BoundArrayCreation node)
		{
			VisitList(node.Bounds);
			Visit(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitArrayLiteral(BoundArrayLiteral node)
		{
			VisitList(node.Bounds);
			Visit(node.Initializer);
			return null;
		}

		public override BoundNode VisitArrayInitialization(BoundArrayInitialization node)
		{
			VisitList(node.Initializers);
			return null;
		}

		public override BoundNode VisitFieldAccess(BoundFieldAccess node)
		{
			Visit(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitPropertyAccess(BoundPropertyAccess node)
		{
			Visit(node.ReceiverOpt);
			VisitList(node.Arguments);
			return null;
		}

		public override BoundNode VisitEventAccess(BoundEventAccess node)
		{
			Visit(node.ReceiverOpt);
			return null;
		}

		public override BoundNode VisitBlock(BoundBlock node)
		{
			VisitList(node.Statements);
			return null;
		}

		public override BoundNode VisitStateMachineScope(BoundStateMachineScope node)
		{
			Visit(node.Statement);
			return null;
		}

		public override BoundNode VisitLocalDeclaration(BoundLocalDeclaration node)
		{
			Visit(node.DeclarationInitializerOpt);
			Visit(node.IdentifierInitializerOpt);
			return null;
		}

		public override BoundNode VisitAsNewLocalDeclarations(BoundAsNewLocalDeclarations node)
		{
			VisitList(node.LocalDeclarations);
			Visit(node.Initializer);
			return null;
		}

		public override BoundNode VisitDimStatement(BoundDimStatement node)
		{
			VisitList(node.LocalDeclarations);
			Visit(node.InitializerOpt);
			return null;
		}

		public override BoundNode VisitInitializer(BoundInitializer node)
		{
			return null;
		}

		public override BoundNode VisitFieldInitializer(BoundFieldInitializer node)
		{
			Visit(node.MemberAccessExpressionOpt);
			Visit(node.InitialValue);
			return null;
		}

		public override BoundNode VisitPropertyInitializer(BoundPropertyInitializer node)
		{
			Visit(node.MemberAccessExpressionOpt);
			Visit(node.InitialValue);
			return null;
		}

		public override BoundNode VisitParameterEqualsValue(BoundParameterEqualsValue node)
		{
			Visit(node.Value);
			return null;
		}

		public override BoundNode VisitGlobalStatementInitializer(BoundGlobalStatementInitializer node)
		{
			Visit(node.Statement);
			return null;
		}

		public override BoundNode VisitSequence(BoundSequence node)
		{
			VisitList(node.SideEffects);
			Visit(node.ValueOpt);
			return null;
		}

		public override BoundNode VisitExpressionStatement(BoundExpressionStatement node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitIfStatement(BoundIfStatement node)
		{
			Visit(node.Condition);
			Visit(node.Consequence);
			Visit(node.AlternativeOpt);
			return null;
		}

		public override BoundNode VisitSelectStatement(BoundSelectStatement node)
		{
			Visit(node.ExpressionStatement);
			Visit(node.ExprPlaceholderOpt);
			VisitList(node.CaseBlocks);
			return null;
		}

		public override BoundNode VisitCaseBlock(BoundCaseBlock node)
		{
			Visit(node.CaseStatement);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitCaseStatement(BoundCaseStatement node)
		{
			VisitList(node.CaseClauses);
			Visit(node.ConditionOpt);
			return null;
		}

		public override BoundNode VisitSimpleCaseClause(BoundSimpleCaseClause node)
		{
			Visit(node.ValueOpt);
			Visit(node.ConditionOpt);
			return null;
		}

		public override BoundNode VisitRangeCaseClause(BoundRangeCaseClause node)
		{
			Visit(node.LowerBoundOpt);
			Visit(node.UpperBoundOpt);
			Visit(node.LowerBoundConditionOpt);
			Visit(node.UpperBoundConditionOpt);
			return null;
		}

		public override BoundNode VisitRelationalCaseClause(BoundRelationalCaseClause node)
		{
			Visit(node.ValueOpt);
			Visit(node.ConditionOpt);
			return null;
		}

		public override BoundNode VisitDoLoopStatement(BoundDoLoopStatement node)
		{
			Visit(node.TopConditionOpt);
			Visit(node.BottomConditionOpt);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitWhileStatement(BoundWhileStatement node)
		{
			Visit(node.Condition);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitForToUserDefinedOperators(BoundForToUserDefinedOperators node)
		{
			Visit(node.LeftOperandPlaceholder);
			Visit(node.RightOperandPlaceholder);
			Visit(node.Addition);
			Visit(node.Subtraction);
			Visit(node.LessThanOrEqual);
			Visit(node.GreaterThanOrEqual);
			return null;
		}

		public override BoundNode VisitForToStatement(BoundForToStatement node)
		{
			Visit(node.InitialValue);
			Visit(node.LimitValue);
			Visit(node.StepValue);
			Visit(node.OperatorsOpt);
			Visit(node.ControlVariable);
			Visit(node.Body);
			VisitList(node.NextVariablesOpt);
			return null;
		}

		public override BoundNode VisitForEachStatement(BoundForEachStatement node)
		{
			Visit(node.Collection);
			Visit(node.ControlVariable);
			Visit(node.Body);
			VisitList(node.NextVariablesOpt);
			return null;
		}

		public override BoundNode VisitExitStatement(BoundExitStatement node)
		{
			return null;
		}

		public override BoundNode VisitContinueStatement(BoundContinueStatement node)
		{
			return null;
		}

		public override BoundNode VisitTryStatement(BoundTryStatement node)
		{
			Visit(node.TryBlock);
			VisitList(node.CatchBlocks);
			Visit(node.FinallyBlockOpt);
			return null;
		}

		public override BoundNode VisitCatchBlock(BoundCatchBlock node)
		{
			Visit(node.ExceptionSourceOpt);
			Visit(node.ErrorLineNumberOpt);
			Visit(node.ExceptionFilterOpt);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitLiteral(BoundLiteral node)
		{
			return null;
		}

		public override BoundNode VisitMeReference(BoundMeReference node)
		{
			return null;
		}

		public override BoundNode VisitValueTypeMeReference(BoundValueTypeMeReference node)
		{
			return null;
		}

		public override BoundNode VisitMyBaseReference(BoundMyBaseReference node)
		{
			return null;
		}

		public override BoundNode VisitMyClassReference(BoundMyClassReference node)
		{
			return null;
		}

		public override BoundNode VisitPreviousSubmissionReference(BoundPreviousSubmissionReference node)
		{
			return null;
		}

		public override BoundNode VisitHostObjectMemberReference(BoundHostObjectMemberReference node)
		{
			return null;
		}

		public override BoundNode VisitLocal(BoundLocal node)
		{
			return null;
		}

		public override BoundNode VisitPseudoVariable(BoundPseudoVariable node)
		{
			return null;
		}

		public override BoundNode VisitParameter(BoundParameter node)
		{
			return null;
		}

		public override BoundNode VisitByRefArgumentPlaceholder(BoundByRefArgumentPlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitByRefArgumentWithCopyBack(BoundByRefArgumentWithCopyBack node)
		{
			Visit(node.OriginalArgument);
			Visit(node.InConversion);
			Visit(node.InPlaceholder);
			Visit(node.OutConversion);
			Visit(node.OutPlaceholder);
			return null;
		}

		public override BoundNode VisitLateBoundArgumentSupportingAssignmentWithCapture(BoundLateBoundArgumentSupportingAssignmentWithCapture node)
		{
			Visit(node.OriginalArgument);
			return null;
		}

		public override BoundNode VisitLabelStatement(BoundLabelStatement node)
		{
			return null;
		}

		public override BoundNode VisitLabel(BoundLabel node)
		{
			return null;
		}

		public override BoundNode VisitGotoStatement(BoundGotoStatement node)
		{
			Visit(node.LabelExpressionOpt);
			return null;
		}

		public override BoundNode VisitStatementList(BoundStatementList node)
		{
			VisitList(node.Statements);
			return null;
		}

		public override BoundNode VisitConditionalGoto(BoundConditionalGoto node)
		{
			Visit(node.Condition);
			return null;
		}

		public override BoundNode VisitWithStatement(BoundWithStatement node)
		{
			Visit(node.OriginalExpression);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitUnboundLambda(UnboundLambda node)
		{
			return null;
		}

		public override BoundNode VisitLambda(BoundLambda node)
		{
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitQueryExpression(BoundQueryExpression node)
		{
			Visit(node.LastOperator);
			return null;
		}

		public override BoundNode VisitQuerySource(BoundQuerySource node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitToQueryableCollectionConversion(BoundToQueryableCollectionConversion node)
		{
			Visit(node.ConversionCall);
			return null;
		}

		public override BoundNode VisitQueryableSource(BoundQueryableSource node)
		{
			Visit(node.Source);
			return null;
		}

		public override BoundNode VisitQueryClause(BoundQueryClause node)
		{
			Visit(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitOrdering(BoundOrdering node)
		{
			Visit(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitQueryLambda(BoundQueryLambda node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitRangeVariableAssignment(BoundRangeVariableAssignment node)
		{
			Visit(node.Value);
			return null;
		}

		public override BoundNode VisitGroupTypeInferenceLambda(GroupTypeInferenceLambda node)
		{
			return null;
		}

		public override BoundNode VisitAggregateClause(BoundAggregateClause node)
		{
			Visit(node.CapturedGroupOpt);
			Visit(node.GroupPlaceholderOpt);
			Visit(node.UnderlyingExpression);
			return null;
		}

		public override BoundNode VisitGroupAggregation(BoundGroupAggregation node)
		{
			Visit(node.Group);
			return null;
		}

		public override BoundNode VisitRangeVariable(BoundRangeVariable node)
		{
			return null;
		}

		public override BoundNode VisitAddHandlerStatement(BoundAddHandlerStatement node)
		{
			Visit(node.EventAccess);
			Visit(node.Handler);
			return null;
		}

		public override BoundNode VisitRemoveHandlerStatement(BoundRemoveHandlerStatement node)
		{
			Visit(node.EventAccess);
			Visit(node.Handler);
			return null;
		}

		public override BoundNode VisitRaiseEventStatement(BoundRaiseEventStatement node)
		{
			Visit(node.EventInvocation);
			return null;
		}

		public override BoundNode VisitUsingStatement(BoundUsingStatement node)
		{
			VisitList(node.ResourceList);
			Visit(node.ResourceExpressionOpt);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitSyncLockStatement(BoundSyncLockStatement node)
		{
			Visit(node.LockExpression);
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitXmlName(BoundXmlName node)
		{
			Visit(node.XmlNamespace);
			Visit(node.LocalName);
			return null;
		}

		public override BoundNode VisitXmlNamespace(BoundXmlNamespace node)
		{
			Visit(node.XmlNamespace);
			return null;
		}

		public override BoundNode VisitXmlDocument(BoundXmlDocument node)
		{
			Visit(node.Declaration);
			VisitList(node.ChildNodes);
			return null;
		}

		public override BoundNode VisitXmlDeclaration(BoundXmlDeclaration node)
		{
			Visit(node.Version);
			Visit(node.Encoding);
			Visit(node.Standalone);
			return null;
		}

		public override BoundNode VisitXmlProcessingInstruction(BoundXmlProcessingInstruction node)
		{
			Visit(node.Target);
			Visit(node.Data);
			return null;
		}

		public override BoundNode VisitXmlComment(BoundXmlComment node)
		{
			Visit(node.Value);
			return null;
		}

		public override BoundNode VisitXmlAttribute(BoundXmlAttribute node)
		{
			Visit(node.Name);
			Visit(node.Value);
			return null;
		}

		public override BoundNode VisitXmlElement(BoundXmlElement node)
		{
			Visit(node.Argument);
			VisitList(node.ChildNodes);
			return null;
		}

		public override BoundNode VisitXmlMemberAccess(BoundXmlMemberAccess node)
		{
			Visit(node.MemberAccess);
			return null;
		}

		public override BoundNode VisitXmlEmbeddedExpression(BoundXmlEmbeddedExpression node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitXmlCData(BoundXmlCData node)
		{
			Visit(node.Value);
			return null;
		}

		public override BoundNode VisitResumeStatement(BoundResumeStatement node)
		{
			Visit(node.LabelExpressionOpt);
			return null;
		}

		public override BoundNode VisitOnErrorStatement(BoundOnErrorStatement node)
		{
			Visit(node.LabelExpressionOpt);
			return null;
		}

		public override BoundNode VisitUnstructuredExceptionHandlingStatement(BoundUnstructuredExceptionHandlingStatement node)
		{
			Visit(node.Body);
			return null;
		}

		public override BoundNode VisitUnstructuredExceptionHandlingCatchFilter(BoundUnstructuredExceptionHandlingCatchFilter node)
		{
			Visit(node.ActiveHandlerLocal);
			Visit(node.ResumeTargetLocal);
			return null;
		}

		public override BoundNode VisitUnstructuredExceptionOnErrorSwitch(BoundUnstructuredExceptionOnErrorSwitch node)
		{
			Visit(node.Value);
			VisitList(node.Jumps);
			return null;
		}

		public override BoundNode VisitUnstructuredExceptionResumeSwitch(BoundUnstructuredExceptionResumeSwitch node)
		{
			Visit(node.ResumeTargetTemporary);
			Visit(node.ResumeLabel);
			Visit(node.ResumeNextLabel);
			VisitList(node.Jumps);
			return null;
		}

		public override BoundNode VisitAwaitOperator(BoundAwaitOperator node)
		{
			Visit(node.Operand);
			Visit(node.AwaitableInstancePlaceholder);
			Visit(node.GetAwaiter);
			Visit(node.AwaiterInstancePlaceholder);
			Visit(node.IsCompleted);
			Visit(node.GetResult);
			return null;
		}

		public override BoundNode VisitSpillSequence(BoundSpillSequence node)
		{
			VisitList(node.Statements);
			Visit(node.ValueOpt);
			return null;
		}

		public override BoundNode VisitStopStatement(BoundStopStatement node)
		{
			return null;
		}

		public override BoundNode VisitEndStatement(BoundEndStatement node)
		{
			return null;
		}

		public override BoundNode VisitMidResult(BoundMidResult node)
		{
			Visit(node.Original);
			Visit(node.Start);
			Visit(node.LengthOpt);
			Visit(node.Source);
			return null;
		}

		public override BoundNode VisitConditionalAccess(BoundConditionalAccess node)
		{
			Visit(node.Receiver);
			Visit(node.Placeholder);
			Visit(node.AccessExpression);
			return null;
		}

		public override BoundNode VisitConditionalAccessReceiverPlaceholder(BoundConditionalAccessReceiverPlaceholder node)
		{
			return null;
		}

		public override BoundNode VisitLoweredConditionalAccess(BoundLoweredConditionalAccess node)
		{
			Visit(node.ReceiverOrCondition);
			Visit(node.WhenNotNull);
			Visit(node.WhenNullOpt);
			return null;
		}

		public override BoundNode VisitComplexConditionalAccessReceiver(BoundComplexConditionalAccessReceiver node)
		{
			Visit(node.ValueTypeReceiver);
			Visit(node.ReferenceTypeReceiver);
			return null;
		}

		public override BoundNode VisitNameOfOperator(BoundNameOfOperator node)
		{
			Visit(node.Argument);
			return null;
		}

		public override BoundNode VisitTypeAsValueExpression(BoundTypeAsValueExpression node)
		{
			Visit(node.Expression);
			return null;
		}

		public override BoundNode VisitInterpolatedStringExpression(BoundInterpolatedStringExpression node)
		{
			VisitList(node.Contents);
			return null;
		}

		public override BoundNode VisitInterpolation(BoundInterpolation node)
		{
			Visit(node.Expression);
			Visit(node.AlignmentOpt);
			Visit(node.FormatStringOpt);
			return null;
		}
	}
}
