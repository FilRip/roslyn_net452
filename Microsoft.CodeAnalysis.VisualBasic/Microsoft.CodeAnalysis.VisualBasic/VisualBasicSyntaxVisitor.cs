using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public abstract class VisualBasicSyntaxVisitor<TResult>
	{
		public virtual TResult VisitEmptyStatement(EmptyStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEndBlockStatement(EndBlockStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCompilationUnit(CompilationUnitSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOptionStatement(OptionStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitImportsStatement(ImportsStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSimpleImportsClause(SimpleImportsClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitImportAliasClause(ImportAliasClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlNamespaceImportsClause(XmlNamespaceImportsClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNamespaceBlock(NamespaceBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNamespaceStatement(NamespaceStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitModuleBlock(ModuleBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitStructureBlock(StructureBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterfaceBlock(InterfaceBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitClassBlock(ClassBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEnumBlock(EnumBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInheritsStatement(InheritsStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitImplementsStatement(ImplementsStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitModuleStatement(ModuleStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitStructureStatement(StructureStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterfaceStatement(InterfaceStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitClassStatement(ClassStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEnumStatement(EnumStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeParameterList(TypeParameterListSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeParameter(TypeParameterSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeParameterSingleConstraintClause(TypeParameterSingleConstraintClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeParameterMultipleConstraintClause(TypeParameterMultipleConstraintClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSpecialConstraint(SpecialConstraintSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeConstraint(TypeConstraintSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMethodBlock(MethodBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOperatorBlock(OperatorBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAccessorBlock(AccessorBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPropertyBlock(PropertyBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEventBlock(EventBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitParameterList(ParameterListSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMethodStatement(MethodStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSubNewStatement(SubNewStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDeclareStatement(DeclareStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDelegateStatement(DelegateStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEventStatement(EventStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOperatorStatement(OperatorStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPropertyStatement(PropertyStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAccessorStatement(AccessorStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitImplementsClause(ImplementsClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitHandlesClause(HandlesClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitKeywordEventContainer(KeywordEventContainerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWithEventsEventContainer(WithEventsEventContainerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWithEventsPropertyEventContainer(WithEventsPropertyEventContainerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitHandlesClauseItem(HandlesClauseItemSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitIncompleteMember(IncompleteMemberSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitVariableDeclarator(VariableDeclaratorSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSimpleAsClause(SimpleAsClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAsNewClause(AsNewClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitObjectMemberInitializer(ObjectMemberInitializerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitObjectCollectionInitializer(ObjectCollectionInitializerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInferredFieldInitializer(InferredFieldInitializerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNamedFieldInitializer(NamedFieldInitializerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEqualsValue(EqualsValueSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitParameter(ParameterSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitModifiedIdentifier(ModifiedIdentifierSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAttributeList(AttributeListSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAttribute(AttributeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAttributeTarget(AttributeTargetSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAttributesStatement(AttributesStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPrintStatement(PrintStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWhileBlock(WhileBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitUsingBlock(UsingBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWithBlock(WithBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLabelStatement(LabelStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGoToStatement(GoToStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLabel(LabelSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitStopOrEndStatement(StopOrEndStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitExitStatement(ExitStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitContinueStatement(ContinueStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitReturnStatement(ReturnStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitIfStatement(IfStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitElseIfStatement(ElseIfStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitElseBlock(ElseBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitElseStatement(ElseStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTryBlock(TryBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTryStatement(TryStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCatchBlock(CatchBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCatchStatement(CatchStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCatchFilterClause(CatchFilterClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitFinallyBlock(FinallyBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitFinallyStatement(FinallyStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitErrorStatement(ErrorStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOnErrorGoToStatement(OnErrorGoToStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitResumeStatement(ResumeStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSelectBlock(SelectBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSelectStatement(SelectStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCaseBlock(CaseBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCaseStatement(CaseStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitElseCaseClause(ElseCaseClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSimpleCaseClause(SimpleCaseClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitRangeCaseClause(RangeCaseClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitRelationalCaseClause(RelationalCaseClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSyncLockStatement(SyncLockStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDoStatement(DoStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLoopStatement(LoopStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWhileOrUntilClause(WhileOrUntilClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWhileStatement(WhileStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitForBlock(ForBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitForEachBlock(ForEachBlockSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitForStatement(ForStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitForStepClause(ForStepClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitForEachStatement(ForEachStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNextStatement(NextStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitUsingStatement(UsingStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitThrowStatement(ThrowStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAssignmentStatement(AssignmentStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMidExpression(MidExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCallStatement(CallStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAddRemoveHandlerStatement(AddRemoveHandlerStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitRaiseEventStatement(RaiseEventStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWithStatement(WithStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitReDimStatement(ReDimStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitRedimClause(RedimClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEraseStatement(EraseStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTupleExpression(TupleExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTupleType(TupleTypeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypedTupleElement(TypedTupleElementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNamedTupleElement(NamedTupleElementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMeExpression(MeExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMyBaseExpression(MyBaseExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMyClassExpression(MyClassExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGetTypeExpression(GetTypeExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeOfExpression(TypeOfExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGetXmlNamespaceExpression(GetXmlNamespaceExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlMemberAccessExpression(XmlMemberAccessExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCollectionInitializer(CollectionInitializerSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCTypeExpression(CTypeExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDirectCastExpression(DirectCastExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTryCastExpression(TryCastExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPredefinedCastExpression(PredefinedCastExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitUnaryExpression(UnaryExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitBinaryConditionalExpression(BinaryConditionalExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTernaryConditionalExpression(TernaryConditionalExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSingleLineLambdaExpression(SingleLineLambdaExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLambdaHeader(LambdaHeaderSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitArgumentList(ArgumentListSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOmittedArgument(OmittedArgumentSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSimpleArgument(SimpleArgumentSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNameColonEquals(NameColonEqualsSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitRangeArgument(RangeArgumentSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitQueryExpression(QueryExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCollectionRangeVariable(CollectionRangeVariableSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitExpressionRangeVariable(ExpressionRangeVariableSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAggregationRangeVariable(AggregationRangeVariableSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitVariableNameEquals(VariableNameEqualsSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitFunctionAggregation(FunctionAggregationSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGroupAggregation(GroupAggregationSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitFromClause(FromClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitLetClause(LetClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAggregateClause(AggregateClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDistinctClause(DistinctClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitWhereClause(WhereClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPartitionWhileClause(PartitionWhileClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPartitionClause(PartitionClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGroupByClause(GroupByClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitJoinCondition(JoinConditionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSimpleJoinClause(SimpleJoinClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGroupJoinClause(GroupJoinClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOrderByClause(OrderByClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitOrdering(OrderingSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSelectClause(SelectClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlDocument(XmlDocumentSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlDeclaration(XmlDeclarationSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlDeclarationOption(XmlDeclarationOptionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlElement(XmlElementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlText(XmlTextSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlElementStartTag(XmlElementStartTagSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlElementEndTag(XmlElementEndTagSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlEmptyElement(XmlEmptyElementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlAttribute(XmlAttributeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlString(XmlStringSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlPrefixName(XmlPrefixNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlName(XmlNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlBracketedName(XmlBracketedNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlPrefix(XmlPrefixSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlComment(XmlCommentSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlCDataSection(XmlCDataSectionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlEmbeddedExpression(XmlEmbeddedExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitArrayType(ArrayTypeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNullableType(NullableTypeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitPredefinedType(PredefinedTypeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitIdentifierName(IdentifierNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGenericName(GenericNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitQualifiedName(QualifiedNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitGlobalName(GlobalNameSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitTypeArgumentList(TypeArgumentListSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCrefReference(CrefReferenceSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCrefSignature(CrefSignatureSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCrefSignaturePart(CrefSignaturePartSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitCrefOperatorReference(CrefOperatorReferenceSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitQualifiedCrefOperatorReference(QualifiedCrefOperatorReferenceSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitYieldStatement(YieldStatementSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitAwaitExpression(AwaitExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitXmlNameAttribute(XmlNameAttributeSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitNameOfExpression(NameOfExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterpolation(InterpolationSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitConstDirectiveTrivia(ConstDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitExternalSourceDirectiveTrivia(ExternalSourceDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEndExternalSourceDirectiveTrivia(EndExternalSourceDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitExternalChecksumDirectiveTrivia(ExternalChecksumDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitEnableWarningDirectiveTrivia(EnableWarningDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitDisableWarningDirectiveTrivia(DisableWarningDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
		{
			return DefaultVisit(node);
		}

		public virtual TResult Visit(SyntaxNode node)
		{
			if (node != null)
			{
				return ((VisualBasicSyntaxNode)node).Accept(this);
			}
			return default(TResult);
		}

		public virtual TResult DefaultVisit(SyntaxNode node)
		{
			return default(TResult);
		}
	}
	public abstract class VisualBasicSyntaxVisitor
	{
		public virtual void VisitEmptyStatement(EmptyStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEndBlockStatement(EndBlockStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCompilationUnit(CompilationUnitSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOptionStatement(OptionStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitImportsStatement(ImportsStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSimpleImportsClause(SimpleImportsClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitImportAliasClause(ImportAliasClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlNamespaceImportsClause(XmlNamespaceImportsClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNamespaceBlock(NamespaceBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNamespaceStatement(NamespaceStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitModuleBlock(ModuleBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitStructureBlock(StructureBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterfaceBlock(InterfaceBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitClassBlock(ClassBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEnumBlock(EnumBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInheritsStatement(InheritsStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitImplementsStatement(ImplementsStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitModuleStatement(ModuleStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitStructureStatement(StructureStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterfaceStatement(InterfaceStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitClassStatement(ClassStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEnumStatement(EnumStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeParameterList(TypeParameterListSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeParameter(TypeParameterSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeParameterSingleConstraintClause(TypeParameterSingleConstraintClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeParameterMultipleConstraintClause(TypeParameterMultipleConstraintClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSpecialConstraint(SpecialConstraintSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeConstraint(TypeConstraintSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMethodBlock(MethodBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOperatorBlock(OperatorBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAccessorBlock(AccessorBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPropertyBlock(PropertyBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEventBlock(EventBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitParameterList(ParameterListSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMethodStatement(MethodStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSubNewStatement(SubNewStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDeclareStatement(DeclareStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDelegateStatement(DelegateStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEventStatement(EventStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOperatorStatement(OperatorStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPropertyStatement(PropertyStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAccessorStatement(AccessorStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitImplementsClause(ImplementsClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitHandlesClause(HandlesClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitKeywordEventContainer(KeywordEventContainerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWithEventsEventContainer(WithEventsEventContainerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWithEventsPropertyEventContainer(WithEventsPropertyEventContainerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitHandlesClauseItem(HandlesClauseItemSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitIncompleteMember(IncompleteMemberSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitVariableDeclarator(VariableDeclaratorSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSimpleAsClause(SimpleAsClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAsNewClause(AsNewClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitObjectMemberInitializer(ObjectMemberInitializerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitObjectCollectionInitializer(ObjectCollectionInitializerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInferredFieldInitializer(InferredFieldInitializerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNamedFieldInitializer(NamedFieldInitializerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEqualsValue(EqualsValueSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitParameter(ParameterSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitModifiedIdentifier(ModifiedIdentifierSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAttributeList(AttributeListSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAttribute(AttributeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAttributeTarget(AttributeTargetSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAttributesStatement(AttributesStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPrintStatement(PrintStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWhileBlock(WhileBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitUsingBlock(UsingBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWithBlock(WithBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLabelStatement(LabelStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGoToStatement(GoToStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLabel(LabelSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitStopOrEndStatement(StopOrEndStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitExitStatement(ExitStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitContinueStatement(ContinueStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitReturnStatement(ReturnStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitIfStatement(IfStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitElseIfStatement(ElseIfStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitElseBlock(ElseBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitElseStatement(ElseStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTryBlock(TryBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTryStatement(TryStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCatchBlock(CatchBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCatchStatement(CatchStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCatchFilterClause(CatchFilterClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitFinallyBlock(FinallyBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitFinallyStatement(FinallyStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitErrorStatement(ErrorStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOnErrorGoToStatement(OnErrorGoToStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitResumeStatement(ResumeStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSelectBlock(SelectBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSelectStatement(SelectStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCaseBlock(CaseBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCaseStatement(CaseStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitElseCaseClause(ElseCaseClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSimpleCaseClause(SimpleCaseClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitRangeCaseClause(RangeCaseClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitRelationalCaseClause(RelationalCaseClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSyncLockStatement(SyncLockStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDoStatement(DoStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLoopStatement(LoopStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWhileOrUntilClause(WhileOrUntilClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWhileStatement(WhileStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitForBlock(ForBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitForEachBlock(ForEachBlockSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitForStatement(ForStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitForStepClause(ForStepClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitForEachStatement(ForEachStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNextStatement(NextStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitUsingStatement(UsingStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitThrowStatement(ThrowStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAssignmentStatement(AssignmentStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMidExpression(MidExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCallStatement(CallStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAddRemoveHandlerStatement(AddRemoveHandlerStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitRaiseEventStatement(RaiseEventStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWithStatement(WithStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitReDimStatement(ReDimStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitRedimClause(RedimClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEraseStatement(EraseStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTupleExpression(TupleExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTupleType(TupleTypeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypedTupleElement(TypedTupleElementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNamedTupleElement(NamedTupleElementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMeExpression(MeExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMyBaseExpression(MyBaseExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMyClassExpression(MyClassExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGetTypeExpression(GetTypeExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeOfExpression(TypeOfExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGetXmlNamespaceExpression(GetXmlNamespaceExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlMemberAccessExpression(XmlMemberAccessExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCollectionInitializer(CollectionInitializerSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCTypeExpression(CTypeExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDirectCastExpression(DirectCastExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTryCastExpression(TryCastExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPredefinedCastExpression(PredefinedCastExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitUnaryExpression(UnaryExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitBinaryConditionalExpression(BinaryConditionalExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTernaryConditionalExpression(TernaryConditionalExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSingleLineLambdaExpression(SingleLineLambdaExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLambdaHeader(LambdaHeaderSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitArgumentList(ArgumentListSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOmittedArgument(OmittedArgumentSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSimpleArgument(SimpleArgumentSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNameColonEquals(NameColonEqualsSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitRangeArgument(RangeArgumentSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitQueryExpression(QueryExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCollectionRangeVariable(CollectionRangeVariableSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitExpressionRangeVariable(ExpressionRangeVariableSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAggregationRangeVariable(AggregationRangeVariableSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitVariableNameEquals(VariableNameEqualsSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitFunctionAggregation(FunctionAggregationSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGroupAggregation(GroupAggregationSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitFromClause(FromClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitLetClause(LetClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAggregateClause(AggregateClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDistinctClause(DistinctClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitWhereClause(WhereClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPartitionWhileClause(PartitionWhileClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPartitionClause(PartitionClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGroupByClause(GroupByClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitJoinCondition(JoinConditionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSimpleJoinClause(SimpleJoinClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGroupJoinClause(GroupJoinClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOrderByClause(OrderByClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitOrdering(OrderingSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSelectClause(SelectClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlDocument(XmlDocumentSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlDeclaration(XmlDeclarationSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlDeclarationOption(XmlDeclarationOptionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlElement(XmlElementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlText(XmlTextSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlElementStartTag(XmlElementStartTagSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlElementEndTag(XmlElementEndTagSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlEmptyElement(XmlEmptyElementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlAttribute(XmlAttributeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlString(XmlStringSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlPrefixName(XmlPrefixNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlName(XmlNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlBracketedName(XmlBracketedNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlPrefix(XmlPrefixSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlComment(XmlCommentSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlCDataSection(XmlCDataSectionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlEmbeddedExpression(XmlEmbeddedExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitArrayType(ArrayTypeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNullableType(NullableTypeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitPredefinedType(PredefinedTypeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitIdentifierName(IdentifierNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGenericName(GenericNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitQualifiedName(QualifiedNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitGlobalName(GlobalNameSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitTypeArgumentList(TypeArgumentListSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCrefReference(CrefReferenceSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCrefSignature(CrefSignatureSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCrefSignaturePart(CrefSignaturePartSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitCrefOperatorReference(CrefOperatorReferenceSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitQualifiedCrefOperatorReference(QualifiedCrefOperatorReferenceSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitYieldStatement(YieldStatementSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitAwaitExpression(AwaitExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitXmlNameAttribute(XmlNameAttributeSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitNameOfExpression(NameOfExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterpolation(InterpolationSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitConstDirectiveTrivia(ConstDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitExternalSourceDirectiveTrivia(ExternalSourceDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEndExternalSourceDirectiveTrivia(EndExternalSourceDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitExternalChecksumDirectiveTrivia(ExternalChecksumDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitEnableWarningDirectiveTrivia(EnableWarningDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitDisableWarningDirectiveTrivia(DisableWarningDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
		{
			DefaultVisit(node);
		}

		public virtual void Visit(SyntaxNode node)
		{
			if (node != null)
			{
				((VisualBasicSyntaxNode)node).Accept(this);
			}
		}

		public virtual void DefaultVisit(SyntaxNode node)
		{
		}
	}
}
