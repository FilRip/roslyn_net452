namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal abstract class VisualBasicSyntaxVisitor
	{
		public virtual VisualBasicSyntaxNode Visit(VisualBasicSyntaxNode node)
		{
			return node?.Accept(this);
		}

		public virtual VisualBasicSyntaxNode VisitVisualBasicSyntaxNode(VisualBasicSyntaxNode node)
		{
			return node;
		}

		public virtual VisualBasicSyntaxNode VisitStatement(StatementSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitExecutableStatement(ExecutableStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitDeclarationStatement(DeclarationStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitEmptyStatement(EmptyStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitEndBlockStatement(EndBlockStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitOptionStatement(OptionStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitImportsStatement(ImportsStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitImportsClause(ImportsClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitSimpleImportsClause(SimpleImportsClauseSyntax node)
		{
			return VisitImportsClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitImportAliasClause(ImportAliasClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlNamespaceImportsClause(XmlNamespaceImportsClauseSyntax node)
		{
			return VisitImportsClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitNamespaceBlock(NamespaceBlockSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitNamespaceStatement(NamespaceStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeBlock(TypeBlockSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitModuleBlock(ModuleBlockSyntax node)
		{
			return VisitTypeBlock(node);
		}

		public virtual VisualBasicSyntaxNode VisitStructureBlock(StructureBlockSyntax node)
		{
			return VisitTypeBlock(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterfaceBlock(InterfaceBlockSyntax node)
		{
			return VisitTypeBlock(node);
		}

		public virtual VisualBasicSyntaxNode VisitClassBlock(ClassBlockSyntax node)
		{
			return VisitTypeBlock(node);
		}

		public virtual VisualBasicSyntaxNode VisitEnumBlock(EnumBlockSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitInheritsOrImplementsStatement(InheritsOrImplementsStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitInheritsStatement(InheritsStatementSyntax node)
		{
			return VisitInheritsOrImplementsStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitImplementsStatement(ImplementsStatementSyntax node)
		{
			return VisitInheritsOrImplementsStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeStatement(TypeStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitModuleStatement(ModuleStatementSyntax node)
		{
			return VisitTypeStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitStructureStatement(StructureStatementSyntax node)
		{
			return VisitTypeStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterfaceStatement(InterfaceStatementSyntax node)
		{
			return VisitTypeStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitClassStatement(ClassStatementSyntax node)
		{
			return VisitTypeStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitEnumStatement(EnumStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeParameterList(TypeParameterListSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeParameter(TypeParameterSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeParameterConstraintClause(TypeParameterConstraintClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeParameterSingleConstraintClause(TypeParameterSingleConstraintClauseSyntax node)
		{
			return VisitTypeParameterConstraintClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeParameterMultipleConstraintClause(TypeParameterMultipleConstraintClauseSyntax node)
		{
			return VisitTypeParameterConstraintClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitConstraint(ConstraintSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitSpecialConstraint(SpecialConstraintSyntax node)
		{
			return VisitConstraint(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeConstraint(TypeConstraintSyntax node)
		{
			return VisitConstraint(node);
		}

		public virtual VisualBasicSyntaxNode VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitMethodBlockBase(MethodBlockBaseSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitMethodBlock(MethodBlockSyntax node)
		{
			return VisitMethodBlockBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitConstructorBlock(ConstructorBlockSyntax node)
		{
			return VisitMethodBlockBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitOperatorBlock(OperatorBlockSyntax node)
		{
			return VisitMethodBlockBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitAccessorBlock(AccessorBlockSyntax node)
		{
			return VisitMethodBlockBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitPropertyBlock(PropertyBlockSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitEventBlock(EventBlockSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitMethodBase(MethodBaseSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitParameterList(ParameterListSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitMethodStatement(MethodStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitSubNewStatement(SubNewStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitDeclareStatement(DeclareStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitDelegateStatement(DelegateStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitEventStatement(EventStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitOperatorStatement(OperatorStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitPropertyStatement(PropertyStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitAccessorStatement(AccessorStatementSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitImplementsClause(ImplementsClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitHandlesClause(HandlesClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitEventContainer(EventContainerSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitKeywordEventContainer(KeywordEventContainerSyntax node)
		{
			return VisitEventContainer(node);
		}

		public virtual VisualBasicSyntaxNode VisitWithEventsEventContainer(WithEventsEventContainerSyntax node)
		{
			return VisitEventContainer(node);
		}

		public virtual VisualBasicSyntaxNode VisitWithEventsPropertyEventContainer(WithEventsPropertyEventContainerSyntax node)
		{
			return VisitEventContainer(node);
		}

		public virtual VisualBasicSyntaxNode VisitHandlesClauseItem(HandlesClauseItemSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitIncompleteMember(IncompleteMemberSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitVariableDeclarator(VariableDeclaratorSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAsClause(AsClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitSimpleAsClause(SimpleAsClauseSyntax node)
		{
			return VisitAsClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitAsNewClause(AsNewClauseSyntax node)
		{
			return VisitAsClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitObjectCreationInitializer(ObjectCreationInitializerSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitObjectMemberInitializer(ObjectMemberInitializerSyntax node)
		{
			return VisitObjectCreationInitializer(node);
		}

		public virtual VisualBasicSyntaxNode VisitObjectCollectionInitializer(ObjectCollectionInitializerSyntax node)
		{
			return VisitObjectCreationInitializer(node);
		}

		public virtual VisualBasicSyntaxNode VisitFieldInitializer(FieldInitializerSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitInferredFieldInitializer(InferredFieldInitializerSyntax node)
		{
			return VisitFieldInitializer(node);
		}

		public virtual VisualBasicSyntaxNode VisitNamedFieldInitializer(NamedFieldInitializerSyntax node)
		{
			return VisitFieldInitializer(node);
		}

		public virtual VisualBasicSyntaxNode VisitEqualsValue(EqualsValueSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitParameter(ParameterSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitModifiedIdentifier(ModifiedIdentifierSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitArrayRankSpecifier(ArrayRankSpecifierSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAttributeList(AttributeListSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAttribute(AttributeSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAttributeTarget(AttributeTargetSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAttributesStatement(AttributesStatementSyntax node)
		{
			return VisitDeclarationStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitExpressionStatement(ExpressionStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitPrintStatement(PrintStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitWhileBlock(WhileBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitUsingBlock(UsingBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitSyncLockBlock(SyncLockBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitWithBlock(WithBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitLocalDeclarationStatement(LocalDeclarationStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitLabelStatement(LabelStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitGoToStatement(GoToStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitLabel(LabelSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitStopOrEndStatement(StopOrEndStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitExitStatement(ExitStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitContinueStatement(ContinueStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitReturnStatement(ReturnStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitSingleLineIfStatement(SingleLineIfStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitSingleLineElseClause(SingleLineElseClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitMultiLineIfBlock(MultiLineIfBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitIfStatement(IfStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitElseIfBlock(ElseIfBlockSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitElseIfStatement(ElseIfStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitElseBlock(ElseBlockSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitElseStatement(ElseStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitTryBlock(TryBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitTryStatement(TryStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitCatchBlock(CatchBlockSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCatchStatement(CatchStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitCatchFilterClause(CatchFilterClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitFinallyBlock(FinallyBlockSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitFinallyStatement(FinallyStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitErrorStatement(ErrorStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitOnErrorGoToStatement(OnErrorGoToStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitOnErrorResumeNextStatement(OnErrorResumeNextStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitResumeStatement(ResumeStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitSelectBlock(SelectBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitSelectStatement(SelectStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitCaseBlock(CaseBlockSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCaseStatement(CaseStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitCaseClause(CaseClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitElseCaseClause(ElseCaseClauseSyntax node)
		{
			return VisitCaseClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitSimpleCaseClause(SimpleCaseClauseSyntax node)
		{
			return VisitCaseClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitRangeCaseClause(RangeCaseClauseSyntax node)
		{
			return VisitCaseClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitRelationalCaseClause(RelationalCaseClauseSyntax node)
		{
			return VisitCaseClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitSyncLockStatement(SyncLockStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitDoLoopBlock(DoLoopBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitDoStatement(DoStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitLoopStatement(LoopStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitWhileOrUntilClause(WhileOrUntilClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitWhileStatement(WhileStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitForOrForEachBlock(ForOrForEachBlockSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitForBlock(ForBlockSyntax node)
		{
			return VisitForOrForEachBlock(node);
		}

		public virtual VisualBasicSyntaxNode VisitForEachBlock(ForEachBlockSyntax node)
		{
			return VisitForOrForEachBlock(node);
		}

		public virtual VisualBasicSyntaxNode VisitForOrForEachStatement(ForOrForEachStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitForStatement(ForStatementSyntax node)
		{
			return VisitForOrForEachStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitForStepClause(ForStepClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitForEachStatement(ForEachStatementSyntax node)
		{
			return VisitForOrForEachStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitNextStatement(NextStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitUsingStatement(UsingStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitThrowStatement(ThrowStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitAssignmentStatement(AssignmentStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitMidExpression(MidExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitCallStatement(CallStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitAddRemoveHandlerStatement(AddRemoveHandlerStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitRaiseEventStatement(RaiseEventStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitWithStatement(WithStatementSyntax node)
		{
			return VisitStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitReDimStatement(ReDimStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitRedimClause(RedimClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitEraseStatement(EraseStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitExpression(ExpressionSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitLiteralExpression(LiteralExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitParenthesizedExpression(ParenthesizedExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitTupleExpression(TupleExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitTupleType(TupleTypeSyntax node)
		{
			return VisitType(node);
		}

		public virtual VisualBasicSyntaxNode VisitTupleElement(TupleElementSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypedTupleElement(TypedTupleElementSyntax node)
		{
			return VisitTupleElement(node);
		}

		public virtual VisualBasicSyntaxNode VisitNamedTupleElement(NamedTupleElementSyntax node)
		{
			return VisitTupleElement(node);
		}

		public virtual VisualBasicSyntaxNode VisitInstanceExpression(InstanceExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitMeExpression(MeExpressionSyntax node)
		{
			return VisitInstanceExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitMyBaseExpression(MyBaseExpressionSyntax node)
		{
			return VisitInstanceExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitMyClassExpression(MyClassExpressionSyntax node)
		{
			return VisitInstanceExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitGetTypeExpression(GetTypeExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeOfExpression(TypeOfExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitGetXmlNamespaceExpression(GetXmlNamespaceExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlMemberAccessExpression(XmlMemberAccessExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitNewExpression(NewExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
		{
			return VisitNewExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpressionSyntax node)
		{
			return VisitNewExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitArrayCreationExpression(ArrayCreationExpressionSyntax node)
		{
			return VisitNewExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitCollectionInitializer(CollectionInitializerSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitCastExpression(CastExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitCTypeExpression(CTypeExpressionSyntax node)
		{
			return VisitCastExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitDirectCastExpression(DirectCastExpressionSyntax node)
		{
			return VisitCastExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitTryCastExpression(TryCastExpressionSyntax node)
		{
			return VisitCastExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitPredefinedCastExpression(PredefinedCastExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitUnaryExpression(UnaryExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitBinaryConditionalExpression(BinaryConditionalExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitTernaryConditionalExpression(TernaryConditionalExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitLambdaExpression(LambdaExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitSingleLineLambdaExpression(SingleLineLambdaExpressionSyntax node)
		{
			return VisitLambdaExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitMultiLineLambdaExpression(MultiLineLambdaExpressionSyntax node)
		{
			return VisitLambdaExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitLambdaHeader(LambdaHeaderSyntax node)
		{
			return VisitMethodBase(node);
		}

		public virtual VisualBasicSyntaxNode VisitArgumentList(ArgumentListSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitArgument(ArgumentSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitOmittedArgument(OmittedArgumentSyntax node)
		{
			return VisitArgument(node);
		}

		public virtual VisualBasicSyntaxNode VisitSimpleArgument(SimpleArgumentSyntax node)
		{
			return VisitArgument(node);
		}

		public virtual VisualBasicSyntaxNode VisitNameColonEquals(NameColonEqualsSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitRangeArgument(RangeArgumentSyntax node)
		{
			return VisitArgument(node);
		}

		public virtual VisualBasicSyntaxNode VisitQueryExpression(QueryExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitQueryClause(QueryClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCollectionRangeVariable(CollectionRangeVariableSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitExpressionRangeVariable(ExpressionRangeVariableSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAggregationRangeVariable(AggregationRangeVariableSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitVariableNameEquals(VariableNameEqualsSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitAggregation(AggregationSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitFunctionAggregation(FunctionAggregationSyntax node)
		{
			return VisitAggregation(node);
		}

		public virtual VisualBasicSyntaxNode VisitGroupAggregation(GroupAggregationSyntax node)
		{
			return VisitAggregation(node);
		}

		public virtual VisualBasicSyntaxNode VisitFromClause(FromClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitLetClause(LetClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitAggregateClause(AggregateClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitDistinctClause(DistinctClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitWhereClause(WhereClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitPartitionWhileClause(PartitionWhileClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitPartitionClause(PartitionClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitGroupByClause(GroupByClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitJoinClause(JoinClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitJoinCondition(JoinConditionSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitSimpleJoinClause(SimpleJoinClauseSyntax node)
		{
			return VisitJoinClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitGroupJoinClause(GroupJoinClauseSyntax node)
		{
			return VisitJoinClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitOrderByClause(OrderByClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitOrdering(OrderingSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitSelectClause(SelectClauseSyntax node)
		{
			return VisitQueryClause(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlNode(XmlNodeSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlDocument(XmlDocumentSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlDeclaration(XmlDeclarationSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlDeclarationOption(XmlDeclarationOptionSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlElement(XmlElementSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlText(XmlTextSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlElementStartTag(XmlElementStartTagSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlElementEndTag(XmlElementEndTagSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlEmptyElement(XmlEmptyElementSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlAttribute(XmlAttributeSyntax node)
		{
			return VisitBaseXmlAttribute(node);
		}

		public virtual VisualBasicSyntaxNode VisitBaseXmlAttribute(BaseXmlAttributeSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlString(XmlStringSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlPrefixName(XmlPrefixNameSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlName(XmlNameSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlBracketedName(XmlBracketedNameSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlPrefix(XmlPrefixSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlComment(XmlCommentSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlProcessingInstruction(XmlProcessingInstructionSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlCDataSection(XmlCDataSectionSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlEmbeddedExpression(XmlEmbeddedExpressionSyntax node)
		{
			return VisitXmlNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitType(TypeSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitArrayType(ArrayTypeSyntax node)
		{
			return VisitType(node);
		}

		public virtual VisualBasicSyntaxNode VisitNullableType(NullableTypeSyntax node)
		{
			return VisitType(node);
		}

		public virtual VisualBasicSyntaxNode VisitPredefinedType(PredefinedTypeSyntax node)
		{
			return VisitType(node);
		}

		public virtual VisualBasicSyntaxNode VisitName(NameSyntax node)
		{
			return VisitType(node);
		}

		public virtual VisualBasicSyntaxNode VisitSimpleName(SimpleNameSyntax node)
		{
			return VisitName(node);
		}

		public virtual VisualBasicSyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
		{
			return VisitSimpleName(node);
		}

		public virtual VisualBasicSyntaxNode VisitGenericName(GenericNameSyntax node)
		{
			return VisitSimpleName(node);
		}

		public virtual VisualBasicSyntaxNode VisitQualifiedName(QualifiedNameSyntax node)
		{
			return VisitName(node);
		}

		public virtual VisualBasicSyntaxNode VisitGlobalName(GlobalNameSyntax node)
		{
			return VisitName(node);
		}

		public virtual VisualBasicSyntaxNode VisitTypeArgumentList(TypeArgumentListSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCrefReference(CrefReferenceSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCrefSignature(CrefSignatureSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCrefSignaturePart(CrefSignaturePartSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitCrefOperatorReference(CrefOperatorReferenceSyntax node)
		{
			return VisitName(node);
		}

		public virtual VisualBasicSyntaxNode VisitQualifiedCrefOperatorReference(QualifiedCrefOperatorReferenceSyntax node)
		{
			return VisitName(node);
		}

		public virtual VisualBasicSyntaxNode VisitYieldStatement(YieldStatementSyntax node)
		{
			return VisitExecutableStatement(node);
		}

		public virtual VisualBasicSyntaxNode VisitAwaitExpression(AwaitExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitStructuredTrivia(StructuredTriviaSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitSkippedTokensTrivia(SkippedTokensTriviaSyntax node)
		{
			return VisitStructuredTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitDocumentationCommentTrivia(DocumentationCommentTriviaSyntax node)
		{
			return VisitStructuredTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlCrefAttribute(XmlCrefAttributeSyntax node)
		{
			return VisitBaseXmlAttribute(node);
		}

		public virtual VisualBasicSyntaxNode VisitXmlNameAttribute(XmlNameAttributeSyntax node)
		{
			return VisitBaseXmlAttribute(node);
		}

		public virtual VisualBasicSyntaxNode VisitConditionalAccessExpression(ConditionalAccessExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitNameOfExpression(NameOfExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterpolatedStringExpression(InterpolatedStringExpressionSyntax node)
		{
			return VisitExpression(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterpolatedStringContent(InterpolatedStringContentSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterpolatedStringText(InterpolatedStringTextSyntax node)
		{
			return VisitInterpolatedStringContent(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterpolation(InterpolationSyntax node)
		{
			return VisitInterpolatedStringContent(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterpolationAlignmentClause(InterpolationAlignmentClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitInterpolationFormatClause(InterpolationFormatClauseSyntax node)
		{
			return VisitVisualBasicSyntaxNode(node);
		}

		public virtual VisualBasicSyntaxNode VisitDirectiveTrivia(DirectiveTriviaSyntax node)
		{
			return VisitStructuredTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitConstDirectiveTrivia(ConstDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitIfDirectiveTrivia(IfDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitElseDirectiveTrivia(ElseDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitEndIfDirectiveTrivia(EndIfDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitExternalSourceDirectiveTrivia(ExternalSourceDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitEndExternalSourceDirectiveTrivia(EndExternalSourceDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitExternalChecksumDirectiveTrivia(ExternalChecksumDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitEnableWarningDirectiveTrivia(EnableWarningDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitDisableWarningDirectiveTrivia(DisableWarningDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitReferenceDirectiveTrivia(ReferenceDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual VisualBasicSyntaxNode VisitBadDirectiveTrivia(BadDirectiveTriviaSyntax node)
		{
			return VisitDirectiveTrivia(node);
		}

		public virtual SyntaxToken VisitSyntaxToken(SyntaxToken token)
		{
			return token;
		}

		public virtual SyntaxTrivia VisitSyntaxTrivia(SyntaxTrivia trivia)
		{
			return trivia;
		}
	}
}
