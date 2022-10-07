Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public MustInherit Class VisualBasicSyntaxVisitor
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Sub DefaultVisit(ByVal node As SyntaxNode)
		End Sub

		Public Overridable Sub Visit(ByVal node As SyntaxNode)
			If (node IsNot Nothing) Then
				DirectCast(node, VisualBasicSyntaxNode).Accept(Me)
			End If
		End Sub

		Public Overridable Sub VisitAccessorBlock(ByVal node As AccessorBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAccessorStatement(ByVal node As AccessorStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAddRemoveHandlerStatement(ByVal node As AddRemoveHandlerStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAggregateClause(ByVal node As AggregateClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAggregationRangeVariable(ByVal node As AggregationRangeVariableSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAnonymousObjectCreationExpression(ByVal node As AnonymousObjectCreationExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitArgumentList(ByVal node As ArgumentListSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitArrayCreationExpression(ByVal node As ArrayCreationExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitArrayRankSpecifier(ByVal node As ArrayRankSpecifierSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitArrayType(ByVal node As ArrayTypeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAsNewClause(ByVal node As AsNewClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAssignmentStatement(ByVal node As AssignmentStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAttribute(ByVal node As AttributeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAttributeList(ByVal node As AttributeListSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAttributesStatement(ByVal node As AttributesStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAttributeTarget(ByVal node As AttributeTargetSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitAwaitExpression(ByVal node As AwaitExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitBadDirectiveTrivia(ByVal node As BadDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitBinaryConditionalExpression(ByVal node As BinaryConditionalExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitBinaryExpression(ByVal node As BinaryExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCallStatement(ByVal node As CallStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCaseBlock(ByVal node As CaseBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCaseStatement(ByVal node As CaseStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCatchBlock(ByVal node As CatchBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCatchFilterClause(ByVal node As CatchFilterClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCatchStatement(ByVal node As CatchStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitClassBlock(ByVal node As ClassBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitClassStatement(ByVal node As ClassStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCollectionInitializer(ByVal node As CollectionInitializerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCollectionRangeVariable(ByVal node As CollectionRangeVariableSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCompilationUnit(ByVal node As CompilationUnitSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitConditionalAccessExpression(ByVal node As ConditionalAccessExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitConstDirectiveTrivia(ByVal node As ConstDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitConstructorBlock(ByVal node As ConstructorBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitContinueStatement(ByVal node As ContinueStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCrefOperatorReference(ByVal node As CrefOperatorReferenceSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCrefReference(ByVal node As CrefReferenceSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCrefSignature(ByVal node As CrefSignatureSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCrefSignaturePart(ByVal node As CrefSignaturePartSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitCTypeExpression(ByVal node As CTypeExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDeclareStatement(ByVal node As DeclareStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDelegateStatement(ByVal node As DelegateStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDirectCastExpression(ByVal node As DirectCastExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDisableWarningDirectiveTrivia(ByVal node As DisableWarningDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDistinctClause(ByVal node As DistinctClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDocumentationCommentTrivia(ByVal node As DocumentationCommentTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDoLoopBlock(ByVal node As DoLoopBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitDoStatement(ByVal node As DoStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitElseBlock(ByVal node As ElseBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitElseCaseClause(ByVal node As ElseCaseClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitElseDirectiveTrivia(ByVal node As ElseDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitElseIfBlock(ByVal node As ElseIfBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitElseIfStatement(ByVal node As ElseIfStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitElseStatement(ByVal node As ElseStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEmptyStatement(ByVal node As EmptyStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEnableWarningDirectiveTrivia(ByVal node As EnableWarningDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEndBlockStatement(ByVal node As EndBlockStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEndExternalSourceDirectiveTrivia(ByVal node As EndExternalSourceDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEndIfDirectiveTrivia(ByVal node As EndIfDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEndRegionDirectiveTrivia(ByVal node As EndRegionDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEnumBlock(ByVal node As EnumBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEnumMemberDeclaration(ByVal node As EnumMemberDeclarationSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEnumStatement(ByVal node As EnumStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEqualsValue(ByVal node As EqualsValueSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEraseStatement(ByVal node As EraseStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitErrorStatement(ByVal node As ErrorStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEventBlock(ByVal node As EventBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitEventStatement(ByVal node As EventStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitExitStatement(ByVal node As ExitStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitExpressionRangeVariable(ByVal node As ExpressionRangeVariableSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitExpressionStatement(ByVal node As ExpressionStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitExternalChecksumDirectiveTrivia(ByVal node As ExternalChecksumDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitExternalSourceDirectiveTrivia(ByVal node As ExternalSourceDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitFieldDeclaration(ByVal node As FieldDeclarationSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitFinallyBlock(ByVal node As FinallyBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitFinallyStatement(ByVal node As FinallyStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitForBlock(ByVal node As ForBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitForEachBlock(ByVal node As ForEachBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitForEachStatement(ByVal node As ForEachStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitForStatement(ByVal node As ForStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitForStepClause(ByVal node As ForStepClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitFromClause(ByVal node As FromClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitFunctionAggregation(ByVal node As FunctionAggregationSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGenericName(ByVal node As GenericNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGetTypeExpression(ByVal node As GetTypeExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGetXmlNamespaceExpression(ByVal node As GetXmlNamespaceExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGlobalName(ByVal node As GlobalNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGoToStatement(ByVal node As GoToStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGroupAggregation(ByVal node As GroupAggregationSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGroupByClause(ByVal node As GroupByClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitGroupJoinClause(ByVal node As GroupJoinClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitHandlesClause(ByVal node As HandlesClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitHandlesClauseItem(ByVal node As HandlesClauseItemSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitIdentifierName(ByVal node As IdentifierNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitIfDirectiveTrivia(ByVal node As IfDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitIfStatement(ByVal node As IfStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitImplementsClause(ByVal node As ImplementsClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitImplementsStatement(ByVal node As ImplementsStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitImportAliasClause(ByVal node As ImportAliasClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitImportsStatement(ByVal node As ImportsStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitIncompleteMember(ByVal node As IncompleteMemberSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInferredFieldInitializer(ByVal node As InferredFieldInitializerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInheritsStatement(ByVal node As InheritsStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterfaceBlock(ByVal node As InterfaceBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterfaceStatement(ByVal node As InterfaceStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterpolatedStringExpression(ByVal node As InterpolatedStringExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterpolatedStringText(ByVal node As InterpolatedStringTextSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterpolation(ByVal node As InterpolationSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterpolationAlignmentClause(ByVal node As InterpolationAlignmentClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInterpolationFormatClause(ByVal node As InterpolationFormatClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitInvocationExpression(ByVal node As InvocationExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitJoinCondition(ByVal node As JoinConditionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitKeywordEventContainer(ByVal node As KeywordEventContainerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLabel(ByVal node As LabelSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLabelStatement(ByVal node As LabelStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLambdaHeader(ByVal node As LambdaHeaderSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLetClause(ByVal node As LetClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLiteralExpression(ByVal node As LiteralExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLocalDeclarationStatement(ByVal node As LocalDeclarationStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitLoopStatement(ByVal node As LoopStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMeExpression(ByVal node As MeExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMemberAccessExpression(ByVal node As MemberAccessExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMethodBlock(ByVal node As MethodBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMethodStatement(ByVal node As MethodStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMidExpression(ByVal node As MidExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitModifiedIdentifier(ByVal node As ModifiedIdentifierSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitModuleBlock(ByVal node As ModuleBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitModuleStatement(ByVal node As ModuleStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMultiLineIfBlock(ByVal node As MultiLineIfBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMultiLineLambdaExpression(ByVal node As MultiLineLambdaExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMyBaseExpression(ByVal node As MyBaseExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitMyClassExpression(ByVal node As MyClassExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNameColonEquals(ByVal node As NameColonEqualsSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNamedFieldInitializer(ByVal node As NamedFieldInitializerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNamedTupleElement(ByVal node As NamedTupleElementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNameOfExpression(ByVal node As NameOfExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNamespaceBlock(ByVal node As NamespaceBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNamespaceStatement(ByVal node As NamespaceStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNextStatement(ByVal node As NextStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitNullableType(ByVal node As NullableTypeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitObjectCollectionInitializer(ByVal node As ObjectCollectionInitializerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitObjectCreationExpression(ByVal node As ObjectCreationExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitObjectMemberInitializer(ByVal node As ObjectMemberInitializerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOmittedArgument(ByVal node As OmittedArgumentSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOnErrorGoToStatement(ByVal node As OnErrorGoToStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOnErrorResumeNextStatement(ByVal node As OnErrorResumeNextStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOperatorBlock(ByVal node As OperatorBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOperatorStatement(ByVal node As OperatorStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOptionStatement(ByVal node As OptionStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOrderByClause(ByVal node As OrderByClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitOrdering(ByVal node As OrderingSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitParameter(ByVal node As ParameterSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitParameterList(ByVal node As ParameterListSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitParenthesizedExpression(ByVal node As ParenthesizedExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPartitionClause(ByVal node As PartitionClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPartitionWhileClause(ByVal node As PartitionWhileClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPredefinedCastExpression(ByVal node As PredefinedCastExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPredefinedType(ByVal node As PredefinedTypeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPrintStatement(ByVal node As PrintStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPropertyBlock(ByVal node As PropertyBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitPropertyStatement(ByVal node As PropertyStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitQualifiedCrefOperatorReference(ByVal node As QualifiedCrefOperatorReferenceSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitQualifiedName(ByVal node As QualifiedNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitQueryExpression(ByVal node As QueryExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitRaiseEventStatement(ByVal node As RaiseEventStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitRangeArgument(ByVal node As RangeArgumentSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitRangeCaseClause(ByVal node As RangeCaseClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitRedimClause(ByVal node As RedimClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitReDimStatement(ByVal node As ReDimStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitReferenceDirectiveTrivia(ByVal node As ReferenceDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitRegionDirectiveTrivia(ByVal node As RegionDirectiveTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitRelationalCaseClause(ByVal node As RelationalCaseClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitResumeStatement(ByVal node As ResumeStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitReturnStatement(ByVal node As ReturnStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSelectBlock(ByVal node As SelectBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSelectClause(ByVal node As SelectClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSelectStatement(ByVal node As SelectStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSimpleArgument(ByVal node As SimpleArgumentSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSimpleAsClause(ByVal node As SimpleAsClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSimpleCaseClause(ByVal node As SimpleCaseClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSimpleImportsClause(ByVal node As SimpleImportsClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSimpleJoinClause(ByVal node As SimpleJoinClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSingleLineElseClause(ByVal node As SingleLineElseClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSingleLineIfStatement(ByVal node As SingleLineIfStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSingleLineLambdaExpression(ByVal node As SingleLineLambdaExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSkippedTokensTrivia(ByVal node As SkippedTokensTriviaSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSpecialConstraint(ByVal node As SpecialConstraintSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitStopOrEndStatement(ByVal node As StopOrEndStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitStructureBlock(ByVal node As StructureBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitStructureStatement(ByVal node As StructureStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSubNewStatement(ByVal node As SubNewStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSyncLockBlock(ByVal node As SyncLockBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitSyncLockStatement(ByVal node As SyncLockStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTernaryConditionalExpression(ByVal node As TernaryConditionalExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitThrowStatement(ByVal node As ThrowStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTryBlock(ByVal node As TryBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTryCastExpression(ByVal node As TryCastExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTryStatement(ByVal node As TryStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTupleExpression(ByVal node As TupleExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTupleType(ByVal node As TupleTypeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeArgumentList(ByVal node As TypeArgumentListSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeConstraint(ByVal node As TypeConstraintSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypedTupleElement(ByVal node As TypedTupleElementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeOfExpression(ByVal node As TypeOfExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeParameter(ByVal node As TypeParameterSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeParameterList(ByVal node As TypeParameterListSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeParameterMultipleConstraintClause(ByVal node As TypeParameterMultipleConstraintClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitTypeParameterSingleConstraintClause(ByVal node As TypeParameterSingleConstraintClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitUnaryExpression(ByVal node As UnaryExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitUsingBlock(ByVal node As UsingBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitUsingStatement(ByVal node As UsingStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitVariableDeclarator(ByVal node As VariableDeclaratorSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitVariableNameEquals(ByVal node As VariableNameEqualsSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWhereClause(ByVal node As WhereClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWhileBlock(ByVal node As WhileBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWhileOrUntilClause(ByVal node As WhileOrUntilClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWhileStatement(ByVal node As WhileStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWithBlock(ByVal node As WithBlockSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWithEventsEventContainer(ByVal node As WithEventsEventContainerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWithEventsPropertyEventContainer(ByVal node As WithEventsPropertyEventContainerSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitWithStatement(ByVal node As WithStatementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlAttribute(ByVal node As XmlAttributeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlBracketedName(ByVal node As XmlBracketedNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlCDataSection(ByVal node As XmlCDataSectionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlComment(ByVal node As XmlCommentSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlCrefAttribute(ByVal node As XmlCrefAttributeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlDeclaration(ByVal node As XmlDeclarationSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlDeclarationOption(ByVal node As XmlDeclarationOptionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlDocument(ByVal node As XmlDocumentSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlElement(ByVal node As XmlElementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlElementEndTag(ByVal node As XmlElementEndTagSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlElementStartTag(ByVal node As XmlElementStartTagSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlEmbeddedExpression(ByVal node As XmlEmbeddedExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlEmptyElement(ByVal node As XmlEmptyElementSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlMemberAccessExpression(ByVal node As XmlMemberAccessExpressionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlName(ByVal node As XmlNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlNameAttribute(ByVal node As XmlNameAttributeSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlNamespaceImportsClause(ByVal node As XmlNamespaceImportsClauseSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlPrefix(ByVal node As XmlPrefixSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlPrefixName(ByVal node As XmlPrefixNameSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlProcessingInstruction(ByVal node As XmlProcessingInstructionSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlString(ByVal node As XmlStringSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitXmlText(ByVal node As XmlTextSyntax)
			Me.DefaultVisit(node)
		End Sub

		Public Overridable Sub VisitYieldStatement(ByVal node As YieldStatementSyntax)
			Me.DefaultVisit(node)
		End Sub
	End Class
End Namespace