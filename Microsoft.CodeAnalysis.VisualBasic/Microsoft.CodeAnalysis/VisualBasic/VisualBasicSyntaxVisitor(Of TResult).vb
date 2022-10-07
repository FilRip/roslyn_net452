Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports System

Namespace Microsoft.CodeAnalysis.VisualBasic
	Public MustInherit Class VisualBasicSyntaxVisitor(Of TResult)
		Protected Sub New()
			MyBase.New()
		End Sub

		Public Overridable Function DefaultVisit(ByVal node As SyntaxNode) As TResult
			Return Nothing
		End Function

		Public Overridable Function Visit(ByVal node As SyntaxNode) As TResult
			Dim tResult As TResult
			tResult = If(node Is Nothing, Nothing, DirectCast(node, VisualBasicSyntaxNode).Accept(Of TResult)(Me))
			Return tResult
		End Function

		Public Overridable Function VisitAccessorBlock(ByVal node As AccessorBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAccessorStatement(ByVal node As AccessorStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAddRemoveHandlerStatement(ByVal node As AddRemoveHandlerStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAggregateClause(ByVal node As AggregateClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAggregationRangeVariable(ByVal node As AggregationRangeVariableSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAnonymousObjectCreationExpression(ByVal node As AnonymousObjectCreationExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArgumentList(ByVal node As ArgumentListSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayCreationExpression(ByVal node As ArrayCreationExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayRankSpecifier(ByVal node As ArrayRankSpecifierSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitArrayType(ByVal node As ArrayTypeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAsNewClause(ByVal node As AsNewClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAssignmentStatement(ByVal node As AssignmentStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAttribute(ByVal node As AttributeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAttributeList(ByVal node As AttributeListSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAttributesStatement(ByVal node As AttributesStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAttributeTarget(ByVal node As AttributeTargetSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitAwaitExpression(ByVal node As AwaitExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBadDirectiveTrivia(ByVal node As BadDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBinaryConditionalExpression(ByVal node As BinaryConditionalExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitBinaryExpression(ByVal node As BinaryExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCallStatement(ByVal node As CallStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCaseBlock(ByVal node As CaseBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCaseStatement(ByVal node As CaseStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCatchBlock(ByVal node As CatchBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCatchFilterClause(ByVal node As CatchFilterClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCatchStatement(ByVal node As CatchStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitClassBlock(ByVal node As ClassBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitClassStatement(ByVal node As ClassStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCollectionInitializer(ByVal node As CollectionInitializerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCollectionRangeVariable(ByVal node As CollectionRangeVariableSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCompilationUnit(ByVal node As CompilationUnitSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConditionalAccessExpression(ByVal node As ConditionalAccessExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConstDirectiveTrivia(ByVal node As ConstDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitConstructorBlock(ByVal node As ConstructorBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitContinueStatement(ByVal node As ContinueStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCrefOperatorReference(ByVal node As CrefOperatorReferenceSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCrefReference(ByVal node As CrefReferenceSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCrefSignature(ByVal node As CrefSignatureSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCrefSignaturePart(ByVal node As CrefSignaturePartSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitCTypeExpression(ByVal node As CTypeExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDeclareStatement(ByVal node As DeclareStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDelegateStatement(ByVal node As DelegateStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDirectCastExpression(ByVal node As DirectCastExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDisableWarningDirectiveTrivia(ByVal node As DisableWarningDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDistinctClause(ByVal node As DistinctClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDocumentationCommentTrivia(ByVal node As DocumentationCommentTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDoLoopBlock(ByVal node As DoLoopBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitDoStatement(ByVal node As DoStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitElseBlock(ByVal node As ElseBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitElseCaseClause(ByVal node As ElseCaseClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitElseDirectiveTrivia(ByVal node As ElseDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitElseIfBlock(ByVal node As ElseIfBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitElseIfStatement(ByVal node As ElseIfStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitElseStatement(ByVal node As ElseStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEmptyStatement(ByVal node As EmptyStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEnableWarningDirectiveTrivia(ByVal node As EnableWarningDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEndBlockStatement(ByVal node As EndBlockStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEndExternalSourceDirectiveTrivia(ByVal node As EndExternalSourceDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEndIfDirectiveTrivia(ByVal node As EndIfDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEndRegionDirectiveTrivia(ByVal node As EndRegionDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEnumBlock(ByVal node As EnumBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEnumMemberDeclaration(ByVal node As EnumMemberDeclarationSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEnumStatement(ByVal node As EnumStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEqualsValue(ByVal node As EqualsValueSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEraseStatement(ByVal node As EraseStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitErrorStatement(ByVal node As ErrorStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEventBlock(ByVal node As EventBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitEventStatement(ByVal node As EventStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExitStatement(ByVal node As ExitStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExpressionRangeVariable(ByVal node As ExpressionRangeVariableSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExpressionStatement(ByVal node As ExpressionStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExternalChecksumDirectiveTrivia(ByVal node As ExternalChecksumDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitExternalSourceDirectiveTrivia(ByVal node As ExternalSourceDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFieldDeclaration(ByVal node As FieldDeclarationSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFinallyBlock(ByVal node As FinallyBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFinallyStatement(ByVal node As FinallyStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForBlock(ByVal node As ForBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForEachBlock(ByVal node As ForEachBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForEachStatement(ByVal node As ForEachStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForStatement(ByVal node As ForStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitForStepClause(ByVal node As ForStepClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFromClause(ByVal node As FromClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitFunctionAggregation(ByVal node As FunctionAggregationSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGenericName(ByVal node As GenericNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGetTypeExpression(ByVal node As GetTypeExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGetXmlNamespaceExpression(ByVal node As GetXmlNamespaceExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGlobalName(ByVal node As GlobalNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGoToStatement(ByVal node As GoToStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGroupAggregation(ByVal node As GroupAggregationSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGroupByClause(ByVal node As GroupByClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitGroupJoinClause(ByVal node As GroupJoinClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitHandlesClause(ByVal node As HandlesClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitHandlesClauseItem(ByVal node As HandlesClauseItemSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitIdentifierName(ByVal node As IdentifierNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitIfDirectiveTrivia(ByVal node As IfDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitIfStatement(ByVal node As IfStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitImplementsClause(ByVal node As ImplementsClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitImplementsStatement(ByVal node As ImplementsStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitImportAliasClause(ByVal node As ImportAliasClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitImportsStatement(ByVal node As ImportsStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitIncompleteMember(ByVal node As IncompleteMemberSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInferredFieldInitializer(ByVal node As InferredFieldInitializerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInheritsStatement(ByVal node As InheritsStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterfaceBlock(ByVal node As InterfaceBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterfaceStatement(ByVal node As InterfaceStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolatedStringExpression(ByVal node As InterpolatedStringExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolatedStringText(ByVal node As InterpolatedStringTextSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolation(ByVal node As InterpolationSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolationAlignmentClause(ByVal node As InterpolationAlignmentClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInterpolationFormatClause(ByVal node As InterpolationFormatClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitInvocationExpression(ByVal node As InvocationExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitJoinCondition(ByVal node As JoinConditionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitKeywordEventContainer(ByVal node As KeywordEventContainerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLabel(ByVal node As LabelSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLabelStatement(ByVal node As LabelStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLambdaHeader(ByVal node As LambdaHeaderSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLetClause(ByVal node As LetClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLiteralExpression(ByVal node As LiteralExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLocalDeclarationStatement(ByVal node As LocalDeclarationStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitLoopStatement(ByVal node As LoopStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMeExpression(ByVal node As MeExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMemberAccessExpression(ByVal node As MemberAccessExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMethodBlock(ByVal node As MethodBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMethodStatement(ByVal node As MethodStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMidExpression(ByVal node As MidExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitModifiedIdentifier(ByVal node As ModifiedIdentifierSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitModuleBlock(ByVal node As ModuleBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitModuleStatement(ByVal node As ModuleStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMultiLineIfBlock(ByVal node As MultiLineIfBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMultiLineLambdaExpression(ByVal node As MultiLineLambdaExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMyBaseExpression(ByVal node As MyBaseExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitMyClassExpression(ByVal node As MyClassExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNameColonEquals(ByVal node As NameColonEqualsSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNamedFieldInitializer(ByVal node As NamedFieldInitializerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNamedTupleElement(ByVal node As NamedTupleElementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNameOfExpression(ByVal node As NameOfExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNamespaceBlock(ByVal node As NamespaceBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNamespaceStatement(ByVal node As NamespaceStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNextStatement(ByVal node As NextStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitNullableType(ByVal node As NullableTypeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitObjectCollectionInitializer(ByVal node As ObjectCollectionInitializerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitObjectCreationExpression(ByVal node As ObjectCreationExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitObjectMemberInitializer(ByVal node As ObjectMemberInitializerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOmittedArgument(ByVal node As OmittedArgumentSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOnErrorGoToStatement(ByVal node As OnErrorGoToStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOnErrorResumeNextStatement(ByVal node As OnErrorResumeNextStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOperatorBlock(ByVal node As OperatorBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOperatorStatement(ByVal node As OperatorStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOptionStatement(ByVal node As OptionStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOrderByClause(ByVal node As OrderByClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitOrdering(ByVal node As OrderingSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitParameter(ByVal node As ParameterSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitParameterList(ByVal node As ParameterListSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitParenthesizedExpression(ByVal node As ParenthesizedExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPartitionClause(ByVal node As PartitionClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPartitionWhileClause(ByVal node As PartitionWhileClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPredefinedCastExpression(ByVal node As PredefinedCastExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPredefinedType(ByVal node As PredefinedTypeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPrintStatement(ByVal node As PrintStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPropertyBlock(ByVal node As PropertyBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitPropertyStatement(ByVal node As PropertyStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQualifiedCrefOperatorReference(ByVal node As QualifiedCrefOperatorReferenceSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQualifiedName(ByVal node As QualifiedNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitQueryExpression(ByVal node As QueryExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRaiseEventStatement(ByVal node As RaiseEventStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRangeArgument(ByVal node As RangeArgumentSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRangeCaseClause(ByVal node As RangeCaseClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRedimClause(ByVal node As RedimClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitReDimStatement(ByVal node As ReDimStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitReferenceDirectiveTrivia(ByVal node As ReferenceDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRegionDirectiveTrivia(ByVal node As RegionDirectiveTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitRelationalCaseClause(ByVal node As RelationalCaseClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitResumeStatement(ByVal node As ResumeStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitReturnStatement(ByVal node As ReturnStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSelectBlock(ByVal node As SelectBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSelectClause(ByVal node As SelectClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSelectStatement(ByVal node As SelectStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSimpleArgument(ByVal node As SimpleArgumentSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSimpleAsClause(ByVal node As SimpleAsClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSimpleCaseClause(ByVal node As SimpleCaseClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSimpleImportsClause(ByVal node As SimpleImportsClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSimpleJoinClause(ByVal node As SimpleJoinClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSingleLineElseClause(ByVal node As SingleLineElseClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSingleLineIfStatement(ByVal node As SingleLineIfStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSingleLineLambdaExpression(ByVal node As SingleLineLambdaExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSkippedTokensTrivia(ByVal node As SkippedTokensTriviaSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSpecialConstraint(ByVal node As SpecialConstraintSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitStopOrEndStatement(ByVal node As StopOrEndStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitStructureBlock(ByVal node As StructureBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitStructureStatement(ByVal node As StructureStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSubNewStatement(ByVal node As SubNewStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSyncLockBlock(ByVal node As SyncLockBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitSyncLockStatement(ByVal node As SyncLockStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTernaryConditionalExpression(ByVal node As TernaryConditionalExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitThrowStatement(ByVal node As ThrowStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTryBlock(ByVal node As TryBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTryCastExpression(ByVal node As TryCastExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTryStatement(ByVal node As TryStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTupleExpression(ByVal node As TupleExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTupleType(ByVal node As TupleTypeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeArgumentList(ByVal node As TypeArgumentListSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeConstraint(ByVal node As TypeConstraintSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypedTupleElement(ByVal node As TypedTupleElementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeOfExpression(ByVal node As TypeOfExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeParameter(ByVal node As TypeParameterSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeParameterList(ByVal node As TypeParameterListSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeParameterMultipleConstraintClause(ByVal node As TypeParameterMultipleConstraintClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitTypeParameterSingleConstraintClause(ByVal node As TypeParameterSingleConstraintClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUnaryExpression(ByVal node As UnaryExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUsingBlock(ByVal node As UsingBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitUsingStatement(ByVal node As UsingStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitVariableDeclarator(ByVal node As VariableDeclaratorSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitVariableNameEquals(ByVal node As VariableNameEqualsSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWhereClause(ByVal node As WhereClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWhileBlock(ByVal node As WhileBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWhileOrUntilClause(ByVal node As WhileOrUntilClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWhileStatement(ByVal node As WhileStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithBlock(ByVal node As WithBlockSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithEventsEventContainer(ByVal node As WithEventsEventContainerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithEventsPropertyEventContainer(ByVal node As WithEventsPropertyEventContainerSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitWithStatement(ByVal node As WithStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlAttribute(ByVal node As XmlAttributeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlBracketedName(ByVal node As XmlBracketedNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlCDataSection(ByVal node As XmlCDataSectionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlComment(ByVal node As XmlCommentSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlCrefAttribute(ByVal node As XmlCrefAttributeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlDeclaration(ByVal node As XmlDeclarationSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlDeclarationOption(ByVal node As XmlDeclarationOptionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlDocument(ByVal node As XmlDocumentSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlElement(ByVal node As XmlElementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlElementEndTag(ByVal node As XmlElementEndTagSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlElementStartTag(ByVal node As XmlElementStartTagSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlEmbeddedExpression(ByVal node As XmlEmbeddedExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlEmptyElement(ByVal node As XmlEmptyElementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlMemberAccessExpression(ByVal node As XmlMemberAccessExpressionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlName(ByVal node As XmlNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlNameAttribute(ByVal node As XmlNameAttributeSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlNamespaceImportsClause(ByVal node As XmlNamespaceImportsClauseSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlPrefix(ByVal node As XmlPrefixSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlPrefixName(ByVal node As XmlPrefixNameSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlProcessingInstruction(ByVal node As XmlProcessingInstructionSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlString(ByVal node As XmlStringSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitXmlText(ByVal node As XmlTextSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function

		Public Overridable Function VisitYieldStatement(ByVal node As YieldStatementSyntax) As TResult
			Return Me.DefaultVisit(node)
		End Function
	End Class
End Namespace