using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class SyntaxFactory
	{
		internal static readonly SyntaxTrivia CarriageReturnLineFeed = EndOfLine("\r\n");

		internal static readonly SyntaxTrivia LineFeed = EndOfLine("\n");

		internal static readonly SyntaxTrivia CarriageReturn = EndOfLine("\r");

		internal static readonly SyntaxTrivia Space = Whitespace(" ");

		internal static readonly SyntaxTrivia Tab = Whitespace("\t");

		internal static readonly SyntaxTrivia ElasticCarriageReturnLineFeed = EndOfLine("\r\n", elastic: true);

		internal static readonly SyntaxTrivia ElasticLineFeed = EndOfLine("\n", elastic: true);

		internal static readonly SyntaxTrivia ElasticCarriageReturn = EndOfLine("\r", elastic: true);

		internal static readonly SyntaxTrivia ElasticSpace = Whitespace(" ", elastic: true);

		internal static readonly SyntaxTrivia ElasticTab = Whitespace("\t", elastic: true);

		internal static readonly SyntaxTrivia ElasticZeroSpace = Whitespace(string.Empty, elastic: true);

		private static PunctuationSyntax s_notMissingEmptyToken = null;

		private static PunctuationSyntax s_missingEmptyToken = null;

		private static PunctuationSyntax s_statementTerminatorToken = null;

		private static PunctuationSyntax s_colonToken = null;

		private static readonly ExpressionSyntax s_missingExpr = IdentifierName(Identifier("", null, null));

		private static readonly EmptyStatementSyntax s_emptyStatement = EmptyStatement(NotMissingEmptyToken);

		private static readonly OmittedArgumentSyntax s_omittedArgument = OmittedArgument(NotMissingEmptyToken);

		internal static PunctuationSyntax NotMissingEmptyToken
		{
			get
			{
				if (s_notMissingEmptyToken == null)
				{
					s_notMissingEmptyToken = new PunctuationSyntax(SyntaxKind.EmptyToken, "", null, null);
				}
				return s_notMissingEmptyToken;
			}
		}

		internal static PunctuationSyntax MissingEmptyToken
		{
			get
			{
				if (s_missingEmptyToken == null)
				{
					s_missingEmptyToken = new PunctuationSyntax(SyntaxKind.EmptyToken, "", null, null);
					s_missingEmptyToken.ClearFlags(GreenNode.NodeFlags.IsNotMissing);
				}
				return s_missingEmptyToken;
			}
		}

		internal static PunctuationSyntax StatementTerminatorToken
		{
			get
			{
				if (s_statementTerminatorToken == null)
				{
					s_statementTerminatorToken = new PunctuationSyntax(SyntaxKind.StatementTerminatorToken, "", null, null);
					s_statementTerminatorToken.SetFlags(GreenNode.NodeFlags.IsNotMissing);
				}
				return s_statementTerminatorToken;
			}
		}

		internal static PunctuationSyntax ColonToken
		{
			get
			{
				if (s_colonToken == null)
				{
					s_colonToken = new PunctuationSyntax(SyntaxKind.ColonToken, "", null, null);
					s_colonToken.SetFlags(GreenNode.NodeFlags.IsNotMissing);
				}
				return s_colonToken;
			}
		}

		internal static IEnumerable<object> GetNodeTypes()
		{
			return new object[296]
			{
				typeof(VisualBasicSyntaxNode),
				typeof(StatementSyntax),
				typeof(ExecutableStatementSyntax),
				typeof(DeclarationStatementSyntax),
				typeof(EmptyStatementSyntax),
				typeof(EndBlockStatementSyntax),
				typeof(CompilationUnitSyntax),
				typeof(OptionStatementSyntax),
				typeof(ImportsStatementSyntax),
				typeof(ImportsClauseSyntax),
				typeof(SimpleImportsClauseSyntax),
				typeof(ImportAliasClauseSyntax),
				typeof(XmlNamespaceImportsClauseSyntax),
				typeof(NamespaceBlockSyntax),
				typeof(NamespaceStatementSyntax),
				typeof(TypeBlockSyntax),
				typeof(ModuleBlockSyntax),
				typeof(StructureBlockSyntax),
				typeof(InterfaceBlockSyntax),
				typeof(ClassBlockSyntax),
				typeof(EnumBlockSyntax),
				typeof(InheritsOrImplementsStatementSyntax),
				typeof(InheritsStatementSyntax),
				typeof(ImplementsStatementSyntax),
				typeof(TypeStatementSyntax),
				typeof(ModuleStatementSyntax),
				typeof(StructureStatementSyntax),
				typeof(InterfaceStatementSyntax),
				typeof(ClassStatementSyntax),
				typeof(EnumStatementSyntax),
				typeof(TypeParameterListSyntax),
				typeof(TypeParameterSyntax),
				typeof(TypeParameterConstraintClauseSyntax),
				typeof(TypeParameterSingleConstraintClauseSyntax),
				typeof(TypeParameterMultipleConstraintClauseSyntax),
				typeof(ConstraintSyntax),
				typeof(SpecialConstraintSyntax),
				typeof(TypeConstraintSyntax),
				typeof(EnumMemberDeclarationSyntax),
				typeof(MethodBlockBaseSyntax),
				typeof(MethodBlockSyntax),
				typeof(ConstructorBlockSyntax),
				typeof(OperatorBlockSyntax),
				typeof(AccessorBlockSyntax),
				typeof(PropertyBlockSyntax),
				typeof(EventBlockSyntax),
				typeof(MethodBaseSyntax),
				typeof(ParameterListSyntax),
				typeof(MethodStatementSyntax),
				typeof(SubNewStatementSyntax),
				typeof(DeclareStatementSyntax),
				typeof(DelegateStatementSyntax),
				typeof(EventStatementSyntax),
				typeof(OperatorStatementSyntax),
				typeof(PropertyStatementSyntax),
				typeof(AccessorStatementSyntax),
				typeof(ImplementsClauseSyntax),
				typeof(HandlesClauseSyntax),
				typeof(EventContainerSyntax),
				typeof(KeywordEventContainerSyntax),
				typeof(WithEventsEventContainerSyntax),
				typeof(WithEventsPropertyEventContainerSyntax),
				typeof(HandlesClauseItemSyntax),
				typeof(IncompleteMemberSyntax),
				typeof(FieldDeclarationSyntax),
				typeof(VariableDeclaratorSyntax),
				typeof(AsClauseSyntax),
				typeof(SimpleAsClauseSyntax),
				typeof(AsNewClauseSyntax),
				typeof(ObjectCreationInitializerSyntax),
				typeof(ObjectMemberInitializerSyntax),
				typeof(ObjectCollectionInitializerSyntax),
				typeof(FieldInitializerSyntax),
				typeof(InferredFieldInitializerSyntax),
				typeof(NamedFieldInitializerSyntax),
				typeof(EqualsValueSyntax),
				typeof(ParameterSyntax),
				typeof(ModifiedIdentifierSyntax),
				typeof(ArrayRankSpecifierSyntax),
				typeof(AttributeListSyntax),
				typeof(AttributeSyntax),
				typeof(AttributeTargetSyntax),
				typeof(AttributesStatementSyntax),
				typeof(ExpressionStatementSyntax),
				typeof(PrintStatementSyntax),
				typeof(WhileBlockSyntax),
				typeof(UsingBlockSyntax),
				typeof(SyncLockBlockSyntax),
				typeof(WithBlockSyntax),
				typeof(LocalDeclarationStatementSyntax),
				typeof(LabelStatementSyntax),
				typeof(GoToStatementSyntax),
				typeof(LabelSyntax),
				typeof(StopOrEndStatementSyntax),
				typeof(ExitStatementSyntax),
				typeof(ContinueStatementSyntax),
				typeof(ReturnStatementSyntax),
				typeof(SingleLineIfStatementSyntax),
				typeof(SingleLineElseClauseSyntax),
				typeof(MultiLineIfBlockSyntax),
				typeof(IfStatementSyntax),
				typeof(ElseIfBlockSyntax),
				typeof(ElseIfStatementSyntax),
				typeof(ElseBlockSyntax),
				typeof(ElseStatementSyntax),
				typeof(TryBlockSyntax),
				typeof(TryStatementSyntax),
				typeof(CatchBlockSyntax),
				typeof(CatchStatementSyntax),
				typeof(CatchFilterClauseSyntax),
				typeof(FinallyBlockSyntax),
				typeof(FinallyStatementSyntax),
				typeof(ErrorStatementSyntax),
				typeof(OnErrorGoToStatementSyntax),
				typeof(OnErrorResumeNextStatementSyntax),
				typeof(ResumeStatementSyntax),
				typeof(SelectBlockSyntax),
				typeof(SelectStatementSyntax),
				typeof(CaseBlockSyntax),
				typeof(CaseStatementSyntax),
				typeof(CaseClauseSyntax),
				typeof(ElseCaseClauseSyntax),
				typeof(SimpleCaseClauseSyntax),
				typeof(RangeCaseClauseSyntax),
				typeof(RelationalCaseClauseSyntax),
				typeof(SyncLockStatementSyntax),
				typeof(DoLoopBlockSyntax),
				typeof(DoStatementSyntax),
				typeof(LoopStatementSyntax),
				typeof(WhileOrUntilClauseSyntax),
				typeof(WhileStatementSyntax),
				typeof(ForOrForEachBlockSyntax),
				typeof(ForBlockSyntax),
				typeof(ForEachBlockSyntax),
				typeof(ForOrForEachStatementSyntax),
				typeof(ForStatementSyntax),
				typeof(ForStepClauseSyntax),
				typeof(ForEachStatementSyntax),
				typeof(NextStatementSyntax),
				typeof(UsingStatementSyntax),
				typeof(ThrowStatementSyntax),
				typeof(AssignmentStatementSyntax),
				typeof(MidExpressionSyntax),
				typeof(CallStatementSyntax),
				typeof(AddRemoveHandlerStatementSyntax),
				typeof(RaiseEventStatementSyntax),
				typeof(WithStatementSyntax),
				typeof(ReDimStatementSyntax),
				typeof(RedimClauseSyntax),
				typeof(EraseStatementSyntax),
				typeof(ExpressionSyntax),
				typeof(LiteralExpressionSyntax),
				typeof(ParenthesizedExpressionSyntax),
				typeof(TupleExpressionSyntax),
				typeof(TupleTypeSyntax),
				typeof(TupleElementSyntax),
				typeof(TypedTupleElementSyntax),
				typeof(NamedTupleElementSyntax),
				typeof(InstanceExpressionSyntax),
				typeof(MeExpressionSyntax),
				typeof(MyBaseExpressionSyntax),
				typeof(MyClassExpressionSyntax),
				typeof(GetTypeExpressionSyntax),
				typeof(TypeOfExpressionSyntax),
				typeof(GetXmlNamespaceExpressionSyntax),
				typeof(MemberAccessExpressionSyntax),
				typeof(XmlMemberAccessExpressionSyntax),
				typeof(InvocationExpressionSyntax),
				typeof(NewExpressionSyntax),
				typeof(ObjectCreationExpressionSyntax),
				typeof(AnonymousObjectCreationExpressionSyntax),
				typeof(ArrayCreationExpressionSyntax),
				typeof(CollectionInitializerSyntax),
				typeof(CastExpressionSyntax),
				typeof(CTypeExpressionSyntax),
				typeof(DirectCastExpressionSyntax),
				typeof(TryCastExpressionSyntax),
				typeof(PredefinedCastExpressionSyntax),
				typeof(BinaryExpressionSyntax),
				typeof(UnaryExpressionSyntax),
				typeof(BinaryConditionalExpressionSyntax),
				typeof(TernaryConditionalExpressionSyntax),
				typeof(LambdaExpressionSyntax),
				typeof(SingleLineLambdaExpressionSyntax),
				typeof(MultiLineLambdaExpressionSyntax),
				typeof(LambdaHeaderSyntax),
				typeof(ArgumentListSyntax),
				typeof(ArgumentSyntax),
				typeof(OmittedArgumentSyntax),
				typeof(SimpleArgumentSyntax),
				typeof(NameColonEqualsSyntax),
				typeof(RangeArgumentSyntax),
				typeof(QueryExpressionSyntax),
				typeof(QueryClauseSyntax),
				typeof(CollectionRangeVariableSyntax),
				typeof(ExpressionRangeVariableSyntax),
				typeof(AggregationRangeVariableSyntax),
				typeof(VariableNameEqualsSyntax),
				typeof(AggregationSyntax),
				typeof(FunctionAggregationSyntax),
				typeof(GroupAggregationSyntax),
				typeof(FromClauseSyntax),
				typeof(LetClauseSyntax),
				typeof(AggregateClauseSyntax),
				typeof(DistinctClauseSyntax),
				typeof(WhereClauseSyntax),
				typeof(PartitionWhileClauseSyntax),
				typeof(PartitionClauseSyntax),
				typeof(GroupByClauseSyntax),
				typeof(JoinClauseSyntax),
				typeof(JoinConditionSyntax),
				typeof(SimpleJoinClauseSyntax),
				typeof(GroupJoinClauseSyntax),
				typeof(OrderByClauseSyntax),
				typeof(OrderingSyntax),
				typeof(SelectClauseSyntax),
				typeof(XmlNodeSyntax),
				typeof(XmlDocumentSyntax),
				typeof(XmlDeclarationSyntax),
				typeof(XmlDeclarationOptionSyntax),
				typeof(XmlElementSyntax),
				typeof(XmlTextSyntax),
				typeof(XmlElementStartTagSyntax),
				typeof(XmlElementEndTagSyntax),
				typeof(XmlEmptyElementSyntax),
				typeof(XmlAttributeSyntax),
				typeof(BaseXmlAttributeSyntax),
				typeof(XmlStringSyntax),
				typeof(XmlPrefixNameSyntax),
				typeof(XmlNameSyntax),
				typeof(XmlBracketedNameSyntax),
				typeof(XmlPrefixSyntax),
				typeof(XmlCommentSyntax),
				typeof(XmlProcessingInstructionSyntax),
				typeof(XmlCDataSectionSyntax),
				typeof(XmlEmbeddedExpressionSyntax),
				typeof(TypeSyntax),
				typeof(ArrayTypeSyntax),
				typeof(NullableTypeSyntax),
				typeof(PredefinedTypeSyntax),
				typeof(NameSyntax),
				typeof(SimpleNameSyntax),
				typeof(IdentifierNameSyntax),
				typeof(GenericNameSyntax),
				typeof(QualifiedNameSyntax),
				typeof(GlobalNameSyntax),
				typeof(TypeArgumentListSyntax),
				typeof(CrefReferenceSyntax),
				typeof(CrefSignatureSyntax),
				typeof(CrefSignaturePartSyntax),
				typeof(CrefOperatorReferenceSyntax),
				typeof(QualifiedCrefOperatorReferenceSyntax),
				typeof(YieldStatementSyntax),
				typeof(AwaitExpressionSyntax),
				typeof(SyntaxToken),
				typeof(KeywordSyntax),
				typeof(PunctuationSyntax),
				typeof(BadTokenSyntax),
				typeof(XmlNameTokenSyntax),
				typeof(XmlTextTokenSyntax),
				typeof(InterpolatedStringTextTokenSyntax),
				typeof(IdentifierTokenSyntax),
				typeof(IntegerLiteralTokenSyntax),
				typeof(FloatingLiteralTokenSyntax),
				typeof(DecimalLiteralTokenSyntax),
				typeof(DateLiteralTokenSyntax),
				typeof(StringLiteralTokenSyntax),
				typeof(CharacterLiteralTokenSyntax),
				typeof(StructuredTriviaSyntax),
				typeof(SkippedTokensTriviaSyntax),
				typeof(DocumentationCommentTriviaSyntax),
				typeof(XmlCrefAttributeSyntax),
				typeof(XmlNameAttributeSyntax),
				typeof(ConditionalAccessExpressionSyntax),
				typeof(NameOfExpressionSyntax),
				typeof(InterpolatedStringExpressionSyntax),
				typeof(InterpolatedStringContentSyntax),
				typeof(InterpolatedStringTextSyntax),
				typeof(InterpolationSyntax),
				typeof(InterpolationAlignmentClauseSyntax),
				typeof(InterpolationFormatClauseSyntax),
				typeof(DirectiveTriviaSyntax),
				typeof(SyntaxTrivia),
				typeof(ConstDirectiveTriviaSyntax),
				typeof(IfDirectiveTriviaSyntax),
				typeof(ElseDirectiveTriviaSyntax),
				typeof(EndIfDirectiveTriviaSyntax),
				typeof(RegionDirectiveTriviaSyntax),
				typeof(EndRegionDirectiveTriviaSyntax),
				typeof(ExternalSourceDirectiveTriviaSyntax),
				typeof(EndExternalSourceDirectiveTriviaSyntax),
				typeof(ExternalChecksumDirectiveTriviaSyntax),
				typeof(EnableWarningDirectiveTriviaSyntax),
				typeof(DisableWarningDirectiveTriviaSyntax),
				typeof(ReferenceDirectiveTriviaSyntax),
				typeof(BadDirectiveTriviaSyntax)
			};
		}

		internal static EmptyStatementSyntax EmptyStatement(PunctuationSyntax empty)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(2, empty, out hash);
			if (greenNode != null)
			{
				return (EmptyStatementSyntax)greenNode;
			}
			EmptyStatementSyntax emptyStatementSyntax = new EmptyStatementSyntax(SyntaxKind.EmptyStatement, empty);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(emptyStatementSyntax, hash);
			}
			return emptyStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndIfStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(5, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndIfStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndUsingStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(6, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndUsingStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndWithStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(7, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndWithStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndSelectStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(8, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSelectStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndStructureStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(9, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndStructureStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndEnumStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(10, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndEnumStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndInterfaceStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(11, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndInterfaceStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndClassStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(12, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndClassStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndModuleStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(13, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndModuleStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndNamespaceStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(14, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndNamespaceStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndSubStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(15, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSubStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndFunctionStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(16, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndFunctionStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndGetStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(17, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndGetStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndSetStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(18, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSetStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndPropertyStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(19, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndPropertyStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndOperatorStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(20, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndOperatorStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndEventStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(21, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndEventStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndAddHandlerStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(22, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndAddHandlerStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndRemoveHandlerStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(23, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndRemoveHandlerStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndRaiseEventStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(24, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndRaiseEventStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndWhileStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(25, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndWhileStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndTryStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(26, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndTryStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndSyncLockStatement(KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(27, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(SyntaxKind.EndSyncLockStatement, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static EndBlockStatementSyntax EndBlockStatement(SyntaxKind kind, KeywordSyntax endKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, endKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (EndBlockStatementSyntax)greenNode;
			}
			EndBlockStatementSyntax endBlockStatementSyntax = new EndBlockStatementSyntax(kind, endKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(endBlockStatementSyntax, hash);
			}
			return endBlockStatementSyntax;
		}

		internal static CompilationUnitSyntax CompilationUnit(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> options, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> imports, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, PunctuationSyntax endOfFileToken)
		{
			return new CompilationUnitSyntax(SyntaxKind.CompilationUnit, options.Node, imports.Node, attributes.Node, members.Node, endOfFileToken);
		}

		internal static OptionStatementSyntax OptionStatement(KeywordSyntax optionKeyword, KeywordSyntax nameKeyword, KeywordSyntax valueKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(41, optionKeyword, nameKeyword, valueKeyword, out hash);
			if (greenNode != null)
			{
				return (OptionStatementSyntax)greenNode;
			}
			OptionStatementSyntax optionStatementSyntax = new OptionStatementSyntax(SyntaxKind.OptionStatement, optionKeyword, nameKeyword, valueKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(optionStatementSyntax, hash);
			}
			return optionStatementSyntax;
		}

		internal static ImportsStatementSyntax ImportsStatement(KeywordSyntax importsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> importsClauses)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(42, importsKeyword, importsClauses.Node, out hash);
			if (greenNode != null)
			{
				return (ImportsStatementSyntax)greenNode;
			}
			ImportsStatementSyntax importsStatementSyntax = new ImportsStatementSyntax(SyntaxKind.ImportsStatement, importsKeyword, importsClauses.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(importsStatementSyntax, hash);
			}
			return importsStatementSyntax;
		}

		internal static SimpleImportsClauseSyntax SimpleImportsClause(ImportAliasClauseSyntax alias, NameSyntax name)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(44, alias, name, out hash);
			if (greenNode != null)
			{
				return (SimpleImportsClauseSyntax)greenNode;
			}
			SimpleImportsClauseSyntax simpleImportsClauseSyntax = new SimpleImportsClauseSyntax(SyntaxKind.SimpleImportsClause, alias, name);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleImportsClauseSyntax, hash);
			}
			return simpleImportsClauseSyntax;
		}

		internal static ImportAliasClauseSyntax ImportAliasClause(IdentifierTokenSyntax identifier, PunctuationSyntax equalsToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(754, identifier, equalsToken, out hash);
			if (greenNode != null)
			{
				return (ImportAliasClauseSyntax)greenNode;
			}
			ImportAliasClauseSyntax importAliasClauseSyntax = new ImportAliasClauseSyntax(SyntaxKind.ImportAliasClause, identifier, equalsToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(importAliasClauseSyntax, hash);
			}
			return importAliasClauseSyntax;
		}

		internal static XmlNamespaceImportsClauseSyntax XmlNamespaceImportsClause(PunctuationSyntax lessThanToken, XmlAttributeSyntax xmlNamespace, PunctuationSyntax greaterThanToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(45, lessThanToken, xmlNamespace, greaterThanToken, out hash);
			if (greenNode != null)
			{
				return (XmlNamespaceImportsClauseSyntax)greenNode;
			}
			XmlNamespaceImportsClauseSyntax xmlNamespaceImportsClauseSyntax = new XmlNamespaceImportsClauseSyntax(SyntaxKind.XmlNamespaceImportsClause, lessThanToken, xmlNamespace, greaterThanToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlNamespaceImportsClauseSyntax, hash);
			}
			return xmlNamespaceImportsClauseSyntax;
		}

		internal static NamespaceBlockSyntax NamespaceBlock(NamespaceStatementSyntax namespaceStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endNamespaceStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(48, namespaceStatement, members.Node, endNamespaceStatement, out hash);
			if (greenNode != null)
			{
				return (NamespaceBlockSyntax)greenNode;
			}
			NamespaceBlockSyntax namespaceBlockSyntax = new NamespaceBlockSyntax(SyntaxKind.NamespaceBlock, namespaceStatement, members.Node, endNamespaceStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(namespaceBlockSyntax, hash);
			}
			return namespaceBlockSyntax;
		}

		internal static NamespaceStatementSyntax NamespaceStatement(KeywordSyntax namespaceKeyword, NameSyntax name)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(49, namespaceKeyword, name, out hash);
			if (greenNode != null)
			{
				return (NamespaceStatementSyntax)greenNode;
			}
			NamespaceStatementSyntax namespaceStatementSyntax = new NamespaceStatementSyntax(SyntaxKind.NamespaceStatement, namespaceKeyword, name);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(namespaceStatementSyntax, hash);
			}
			return namespaceStatementSyntax;
		}

		internal static ModuleBlockSyntax ModuleBlock(ModuleStatementSyntax moduleStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endModuleStatement)
		{
			return new ModuleBlockSyntax(SyntaxKind.ModuleBlock, moduleStatement, inherits.Node, implements.Node, members.Node, endModuleStatement);
		}

		internal static StructureBlockSyntax StructureBlock(StructureStatementSyntax structureStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endStructureStatement)
		{
			return new StructureBlockSyntax(SyntaxKind.StructureBlock, structureStatement, inherits.Node, implements.Node, members.Node, endStructureStatement);
		}

		internal static InterfaceBlockSyntax InterfaceBlock(InterfaceStatementSyntax interfaceStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endInterfaceStatement)
		{
			return new InterfaceBlockSyntax(SyntaxKind.InterfaceBlock, interfaceStatement, inherits.Node, implements.Node, members.Node, endInterfaceStatement);
		}

		internal static ClassBlockSyntax ClassBlock(ClassStatementSyntax classStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endClassStatement)
		{
			return new ClassBlockSyntax(SyntaxKind.ClassBlock, classStatement, inherits.Node, implements.Node, members.Node, endClassStatement);
		}

		internal static EnumBlockSyntax EnumBlock(EnumStatementSyntax enumStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> members, EndBlockStatementSyntax endEnumStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(54, enumStatement, members.Node, endEnumStatement, out hash);
			if (greenNode != null)
			{
				return (EnumBlockSyntax)greenNode;
			}
			EnumBlockSyntax enumBlockSyntax = new EnumBlockSyntax(SyntaxKind.EnumBlock, enumStatement, members.Node, endEnumStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(enumBlockSyntax, hash);
			}
			return enumBlockSyntax;
		}

		internal static InheritsStatementSyntax InheritsStatement(KeywordSyntax inheritsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> types)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(57, inheritsKeyword, types.Node, out hash);
			if (greenNode != null)
			{
				return (InheritsStatementSyntax)greenNode;
			}
			InheritsStatementSyntax inheritsStatementSyntax = new InheritsStatementSyntax(SyntaxKind.InheritsStatement, inheritsKeyword, types.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(inheritsStatementSyntax, hash);
			}
			return inheritsStatementSyntax;
		}

		internal static ImplementsStatementSyntax ImplementsStatement(KeywordSyntax implementsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> types)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(58, implementsKeyword, types.Node, out hash);
			if (greenNode != null)
			{
				return (ImplementsStatementSyntax)greenNode;
			}
			ImplementsStatementSyntax implementsStatementSyntax = new ImplementsStatementSyntax(SyntaxKind.ImplementsStatement, implementsKeyword, types.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(implementsStatementSyntax, hash);
			}
			return implementsStatementSyntax;
		}

		internal static ModuleStatementSyntax ModuleStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax moduleKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new ModuleStatementSyntax(SyntaxKind.ModuleStatement, attributeLists.Node, modifiers.Node, moduleKeyword, identifier, typeParameterList);
		}

		internal static StructureStatementSyntax StructureStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax structureKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new StructureStatementSyntax(SyntaxKind.StructureStatement, attributeLists.Node, modifiers.Node, structureKeyword, identifier, typeParameterList);
		}

		internal static InterfaceStatementSyntax InterfaceStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax interfaceKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new InterfaceStatementSyntax(SyntaxKind.InterfaceStatement, attributeLists.Node, modifiers.Node, interfaceKeyword, identifier, typeParameterList);
		}

		internal static ClassStatementSyntax ClassStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax classKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return new ClassStatementSyntax(SyntaxKind.ClassStatement, attributeLists.Node, modifiers.Node, classKeyword, identifier, typeParameterList);
		}

		internal static EnumStatementSyntax EnumStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax enumKeyword, IdentifierTokenSyntax identifier, AsClauseSyntax underlyingType)
		{
			return new EnumStatementSyntax(SyntaxKind.EnumStatement, attributeLists.Node, modifiers.Node, enumKeyword, identifier, underlyingType);
		}

		internal static TypeParameterListSyntax TypeParameterList(PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> parameters, PunctuationSyntax closeParenToken)
		{
			return new TypeParameterListSyntax(SyntaxKind.TypeParameterList, openParenToken, ofKeyword, parameters.Node, closeParenToken);
		}

		internal static TypeParameterSyntax TypeParameter(KeywordSyntax varianceKeyword, IdentifierTokenSyntax identifier, TypeParameterConstraintClauseSyntax typeParameterConstraintClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(67, varianceKeyword, identifier, typeParameterConstraintClause, out hash);
			if (greenNode != null)
			{
				return (TypeParameterSyntax)greenNode;
			}
			TypeParameterSyntax typeParameterSyntax = new TypeParameterSyntax(SyntaxKind.TypeParameter, varianceKeyword, identifier, typeParameterConstraintClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typeParameterSyntax, hash);
			}
			return typeParameterSyntax;
		}

		internal static TypeParameterSingleConstraintClauseSyntax TypeParameterSingleConstraintClause(KeywordSyntax asKeyword, ConstraintSyntax constraint)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(70, asKeyword, constraint, out hash);
			if (greenNode != null)
			{
				return (TypeParameterSingleConstraintClauseSyntax)greenNode;
			}
			TypeParameterSingleConstraintClauseSyntax typeParameterSingleConstraintClauseSyntax = new TypeParameterSingleConstraintClauseSyntax(SyntaxKind.TypeParameterSingleConstraintClause, asKeyword, constraint);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typeParameterSingleConstraintClauseSyntax, hash);
			}
			return typeParameterSingleConstraintClauseSyntax;
		}

		internal static TypeParameterMultipleConstraintClauseSyntax TypeParameterMultipleConstraintClause(KeywordSyntax asKeyword, PunctuationSyntax openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> constraints, PunctuationSyntax closeBraceToken)
		{
			return new TypeParameterMultipleConstraintClauseSyntax(SyntaxKind.TypeParameterMultipleConstraintClause, asKeyword, openBraceToken, constraints.Node, closeBraceToken);
		}

		internal static SpecialConstraintSyntax NewConstraint(KeywordSyntax constraintKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(72, constraintKeyword, out hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(SyntaxKind.NewConstraint, constraintKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal static SpecialConstraintSyntax ClassConstraint(KeywordSyntax constraintKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(73, constraintKeyword, out hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(SyntaxKind.ClassConstraint, constraintKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal static SpecialConstraintSyntax StructureConstraint(KeywordSyntax constraintKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(74, constraintKeyword, out hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(SyntaxKind.StructureConstraint, constraintKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal static SpecialConstraintSyntax SpecialConstraint(SyntaxKind kind, KeywordSyntax constraintKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, constraintKeyword, out hash);
			if (greenNode != null)
			{
				return (SpecialConstraintSyntax)greenNode;
			}
			SpecialConstraintSyntax specialConstraintSyntax = new SpecialConstraintSyntax(kind, constraintKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(specialConstraintSyntax, hash);
			}
			return specialConstraintSyntax;
		}

		internal static TypeConstraintSyntax TypeConstraint(TypeSyntax type)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(75, type, out hash);
			if (greenNode != null)
			{
				return (TypeConstraintSyntax)greenNode;
			}
			TypeConstraintSyntax typeConstraintSyntax = new TypeConstraintSyntax(SyntaxKind.TypeConstraint, type);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typeConstraintSyntax, hash);
			}
			return typeConstraintSyntax;
		}

		internal static EnumMemberDeclarationSyntax EnumMemberDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, IdentifierTokenSyntax identifier, EqualsValueSyntax initializer)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(78, attributeLists.Node, identifier, initializer, out hash);
			if (greenNode != null)
			{
				return (EnumMemberDeclarationSyntax)greenNode;
			}
			EnumMemberDeclarationSyntax enumMemberDeclarationSyntax = new EnumMemberDeclarationSyntax(SyntaxKind.EnumMemberDeclaration, attributeLists.Node, identifier, initializer);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(enumMemberDeclarationSyntax, hash);
			}
			return enumMemberDeclarationSyntax;
		}

		internal static MethodBlockSyntax SubBlock(MethodStatementSyntax subOrFunctionStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(79, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, out hash);
			if (greenNode != null)
			{
				return (MethodBlockSyntax)greenNode;
			}
			MethodBlockSyntax methodBlockSyntax = new MethodBlockSyntax(SyntaxKind.SubBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(methodBlockSyntax, hash);
			}
			return methodBlockSyntax;
		}

		internal static MethodBlockSyntax FunctionBlock(MethodStatementSyntax subOrFunctionStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(80, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, out hash);
			if (greenNode != null)
			{
				return (MethodBlockSyntax)greenNode;
			}
			MethodBlockSyntax methodBlockSyntax = new MethodBlockSyntax(SyntaxKind.FunctionBlock, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(methodBlockSyntax, hash);
			}
			return methodBlockSyntax;
		}

		internal static MethodBlockSyntax MethodBlock(SyntaxKind kind, MethodStatementSyntax subOrFunctionStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement, out hash);
			if (greenNode != null)
			{
				return (MethodBlockSyntax)greenNode;
			}
			MethodBlockSyntax methodBlockSyntax = new MethodBlockSyntax(kind, subOrFunctionStatement, statements.Node, endSubOrFunctionStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(methodBlockSyntax, hash);
			}
			return methodBlockSyntax;
		}

		internal static ConstructorBlockSyntax ConstructorBlock(SubNewStatementSyntax subNewStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(81, subNewStatement, statements.Node, endSubStatement, out hash);
			if (greenNode != null)
			{
				return (ConstructorBlockSyntax)greenNode;
			}
			ConstructorBlockSyntax constructorBlockSyntax = new ConstructorBlockSyntax(SyntaxKind.ConstructorBlock, subNewStatement, statements.Node, endSubStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(constructorBlockSyntax, hash);
			}
			return constructorBlockSyntax;
		}

		internal static OperatorBlockSyntax OperatorBlock(OperatorStatementSyntax operatorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endOperatorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(82, operatorStatement, statements.Node, endOperatorStatement, out hash);
			if (greenNode != null)
			{
				return (OperatorBlockSyntax)greenNode;
			}
			OperatorBlockSyntax operatorBlockSyntax = new OperatorBlockSyntax(SyntaxKind.OperatorBlock, operatorStatement, statements.Node, endOperatorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(operatorBlockSyntax, hash);
			}
			return operatorBlockSyntax;
		}

		internal static AccessorBlockSyntax GetAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(83, accessorStatement, statements.Node, endAccessorStatement, out hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.GetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal static AccessorBlockSyntax SetAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(84, accessorStatement, statements.Node, endAccessorStatement, out hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.SetAccessorBlock, accessorStatement, statements.Node, endAccessorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal static AccessorBlockSyntax AddHandlerAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(85, accessorStatement, statements.Node, endAccessorStatement, out hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.AddHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal static AccessorBlockSyntax RemoveHandlerAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(86, accessorStatement, statements.Node, endAccessorStatement, out hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.RemoveHandlerAccessorBlock, accessorStatement, statements.Node, endAccessorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal static AccessorBlockSyntax RaiseEventAccessorBlock(AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(87, accessorStatement, statements.Node, endAccessorStatement, out hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(SyntaxKind.RaiseEventAccessorBlock, accessorStatement, statements.Node, endAccessorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal static AccessorBlockSyntax AccessorBlock(SyntaxKind kind, AccessorStatementSyntax accessorStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endAccessorStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, accessorStatement, statements.Node, endAccessorStatement, out hash);
			if (greenNode != null)
			{
				return (AccessorBlockSyntax)greenNode;
			}
			AccessorBlockSyntax accessorBlockSyntax = new AccessorBlockSyntax(kind, accessorStatement, statements.Node, endAccessorStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(accessorBlockSyntax, hash);
			}
			return accessorBlockSyntax;
		}

		internal static PropertyBlockSyntax PropertyBlock(PropertyStatementSyntax propertyStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> accessors, EndBlockStatementSyntax endPropertyStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(88, propertyStatement, accessors.Node, endPropertyStatement, out hash);
			if (greenNode != null)
			{
				return (PropertyBlockSyntax)greenNode;
			}
			PropertyBlockSyntax propertyBlockSyntax = new PropertyBlockSyntax(SyntaxKind.PropertyBlock, propertyStatement, accessors.Node, endPropertyStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(propertyBlockSyntax, hash);
			}
			return propertyBlockSyntax;
		}

		internal static EventBlockSyntax EventBlock(EventStatementSyntax eventStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> accessors, EndBlockStatementSyntax endEventStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(89, eventStatement, accessors.Node, endEventStatement, out hash);
			if (greenNode != null)
			{
				return (EventBlockSyntax)greenNode;
			}
			EventBlockSyntax eventBlockSyntax = new EventBlockSyntax(SyntaxKind.EventBlock, eventStatement, accessors.Node, endEventStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(eventBlockSyntax, hash);
			}
			return eventBlockSyntax;
		}

		internal static ParameterListSyntax ParameterList(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> parameters, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(92, openParenToken, parameters.Node, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (ParameterListSyntax)greenNode;
			}
			ParameterListSyntax parameterListSyntax = new ParameterListSyntax(SyntaxKind.ParameterList, openParenToken, parameters.Node, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(parameterListSyntax, hash);
			}
			return parameterListSyntax;
		}

		internal static MethodStatementSyntax SubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			return new MethodStatementSyntax(SyntaxKind.SubStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause);
		}

		internal static MethodStatementSyntax FunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			return new MethodStatementSyntax(SyntaxKind.FunctionStatement, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause);
		}

		internal static MethodStatementSyntax MethodStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, HandlesClauseSyntax handlesClause, ImplementsClauseSyntax implementsClause)
		{
			return new MethodStatementSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause, handlesClause, implementsClause);
		}

		internal static SubNewStatementSyntax SubNewStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subKeyword, KeywordSyntax newKeyword, ParameterListSyntax parameterList)
		{
			return new SubNewStatementSyntax(SyntaxKind.SubNewStatement, attributeLists.Node, modifiers.Node, subKeyword, newKeyword, parameterList);
		}

		internal static DeclareStatementSyntax DeclareSubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DeclareStatementSyntax(SyntaxKind.DeclareSubStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause);
		}

		internal static DeclareStatementSyntax DeclareFunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DeclareStatementSyntax(SyntaxKind.DeclareFunctionStatement, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause);
		}

		internal static DeclareStatementSyntax DeclareStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax declareKeyword, KeywordSyntax charsetKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, KeywordSyntax libKeyword, LiteralExpressionSyntax libraryName, KeywordSyntax aliasKeyword, LiteralExpressionSyntax aliasName, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DeclareStatementSyntax(kind, attributeLists.Node, modifiers.Node, declareKeyword, charsetKeyword, subOrFunctionKeyword, identifier, libKeyword, libraryName, aliasKeyword, aliasName, parameterList, asClause);
		}

		internal static DelegateStatementSyntax DelegateSubStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DelegateStatementSyntax(SyntaxKind.DelegateSubStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause);
		}

		internal static DelegateStatementSyntax DelegateFunctionStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DelegateStatementSyntax(SyntaxKind.DelegateFunctionStatement, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause);
		}

		internal static DelegateStatementSyntax DelegateStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax delegateKeyword, KeywordSyntax subOrFunctionKeyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new DelegateStatementSyntax(kind, attributeLists.Node, modifiers.Node, delegateKeyword, subOrFunctionKeyword, identifier, typeParameterList, parameterList, asClause);
		}

		internal static EventStatementSyntax EventStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax customKeyword, KeywordSyntax eventKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause, ImplementsClauseSyntax implementsClause)
		{
			return new EventStatementSyntax(SyntaxKind.EventStatement, attributeLists.Node, modifiers.Node, customKeyword, eventKeyword, identifier, parameterList, asClause, implementsClause);
		}

		internal static OperatorStatementSyntax OperatorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax operatorKeyword, SyntaxToken operatorToken, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new OperatorStatementSyntax(SyntaxKind.OperatorStatement, attributeLists.Node, modifiers.Node, operatorKeyword, operatorToken, parameterList, asClause);
		}

		internal static PropertyStatementSyntax PropertyStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax propertyKeyword, IdentifierTokenSyntax identifier, ParameterListSyntax parameterList, AsClauseSyntax asClause, EqualsValueSyntax initializer, ImplementsClauseSyntax implementsClause)
		{
			return new PropertyStatementSyntax(SyntaxKind.PropertyStatement, attributeLists.Node, modifiers.Node, propertyKeyword, identifier, parameterList, asClause, initializer, implementsClause);
		}

		internal static AccessorStatementSyntax GetAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.GetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList);
		}

		internal static AccessorStatementSyntax SetAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.SetAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList);
		}

		internal static AccessorStatementSyntax AddHandlerAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.AddHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList);
		}

		internal static AccessorStatementSyntax RemoveHandlerAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.RemoveHandlerAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList);
		}

		internal static AccessorStatementSyntax RaiseEventAccessorStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(SyntaxKind.RaiseEventAccessorStatement, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList);
		}

		internal static AccessorStatementSyntax AccessorStatement(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax accessorKeyword, ParameterListSyntax parameterList)
		{
			return new AccessorStatementSyntax(kind, attributeLists.Node, modifiers.Node, accessorKeyword, parameterList);
		}

		internal static ImplementsClauseSyntax ImplementsClause(KeywordSyntax implementsKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> interfaceMembers)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(112, implementsKeyword, interfaceMembers.Node, out hash);
			if (greenNode != null)
			{
				return (ImplementsClauseSyntax)greenNode;
			}
			ImplementsClauseSyntax implementsClauseSyntax = new ImplementsClauseSyntax(SyntaxKind.ImplementsClause, implementsKeyword, interfaceMembers.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(implementsClauseSyntax, hash);
			}
			return implementsClauseSyntax;
		}

		internal static HandlesClauseSyntax HandlesClause(KeywordSyntax handlesKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> events)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(113, handlesKeyword, events.Node, out hash);
			if (greenNode != null)
			{
				return (HandlesClauseSyntax)greenNode;
			}
			HandlesClauseSyntax handlesClauseSyntax = new HandlesClauseSyntax(SyntaxKind.HandlesClause, handlesKeyword, events.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(handlesClauseSyntax, hash);
			}
			return handlesClauseSyntax;
		}

		internal static KeywordEventContainerSyntax KeywordEventContainer(KeywordSyntax keyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(114, keyword, out hash);
			if (greenNode != null)
			{
				return (KeywordEventContainerSyntax)greenNode;
			}
			KeywordEventContainerSyntax keywordEventContainerSyntax = new KeywordEventContainerSyntax(SyntaxKind.KeywordEventContainer, keyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(keywordEventContainerSyntax, hash);
			}
			return keywordEventContainerSyntax;
		}

		internal static WithEventsEventContainerSyntax WithEventsEventContainer(IdentifierTokenSyntax identifier)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(115, identifier, out hash);
			if (greenNode != null)
			{
				return (WithEventsEventContainerSyntax)greenNode;
			}
			WithEventsEventContainerSyntax withEventsEventContainerSyntax = new WithEventsEventContainerSyntax(SyntaxKind.WithEventsEventContainer, identifier);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withEventsEventContainerSyntax, hash);
			}
			return withEventsEventContainerSyntax;
		}

		internal static WithEventsPropertyEventContainerSyntax WithEventsPropertyEventContainer(WithEventsEventContainerSyntax withEventsContainer, PunctuationSyntax dotToken, IdentifierNameSyntax property)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(116, withEventsContainer, dotToken, property, out hash);
			if (greenNode != null)
			{
				return (WithEventsPropertyEventContainerSyntax)greenNode;
			}
			WithEventsPropertyEventContainerSyntax withEventsPropertyEventContainerSyntax = new WithEventsPropertyEventContainerSyntax(SyntaxKind.WithEventsPropertyEventContainer, withEventsContainer, dotToken, property);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withEventsPropertyEventContainerSyntax, hash);
			}
			return withEventsPropertyEventContainerSyntax;
		}

		internal static HandlesClauseItemSyntax HandlesClauseItem(EventContainerSyntax eventContainer, PunctuationSyntax dotToken, IdentifierNameSyntax eventMember)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(117, eventContainer, dotToken, eventMember, out hash);
			if (greenNode != null)
			{
				return (HandlesClauseItemSyntax)greenNode;
			}
			HandlesClauseItemSyntax handlesClauseItemSyntax = new HandlesClauseItemSyntax(SyntaxKind.HandlesClauseItem, eventContainer, dotToken, eventMember);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(handlesClauseItemSyntax, hash);
			}
			return handlesClauseItemSyntax;
		}

		internal static IncompleteMemberSyntax IncompleteMember(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, IdentifierTokenSyntax missingIdentifier)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(118, attributeLists.Node, modifiers.Node, missingIdentifier, out hash);
			if (greenNode != null)
			{
				return (IncompleteMemberSyntax)greenNode;
			}
			IncompleteMemberSyntax incompleteMemberSyntax = new IncompleteMemberSyntax(SyntaxKind.IncompleteMember, attributeLists.Node, modifiers.Node, missingIdentifier);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(incompleteMemberSyntax, hash);
			}
			return incompleteMemberSyntax;
		}

		internal static FieldDeclarationSyntax FieldDeclaration(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> declarators)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(119, attributeLists.Node, modifiers.Node, declarators.Node, out hash);
			if (greenNode != null)
			{
				return (FieldDeclarationSyntax)greenNode;
			}
			FieldDeclarationSyntax fieldDeclarationSyntax = new FieldDeclarationSyntax(SyntaxKind.FieldDeclaration, attributeLists.Node, modifiers.Node, declarators.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(fieldDeclarationSyntax, hash);
			}
			return fieldDeclarationSyntax;
		}

		internal static VariableDeclaratorSyntax VariableDeclarator(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> names, AsClauseSyntax asClause, EqualsValueSyntax initializer)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(122, names.Node, asClause, initializer, out hash);
			if (greenNode != null)
			{
				return (VariableDeclaratorSyntax)greenNode;
			}
			VariableDeclaratorSyntax variableDeclaratorSyntax = new VariableDeclaratorSyntax(SyntaxKind.VariableDeclarator, names.Node, asClause, initializer);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(variableDeclaratorSyntax, hash);
			}
			return variableDeclaratorSyntax;
		}

		internal static SimpleAsClauseSyntax SimpleAsClause(KeywordSyntax asKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, TypeSyntax type)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(123, asKeyword, attributeLists.Node, type, out hash);
			if (greenNode != null)
			{
				return (SimpleAsClauseSyntax)greenNode;
			}
			SimpleAsClauseSyntax simpleAsClauseSyntax = new SimpleAsClauseSyntax(SyntaxKind.SimpleAsClause, asKeyword, attributeLists.Node, type);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleAsClauseSyntax, hash);
			}
			return simpleAsClauseSyntax;
		}

		internal static AsNewClauseSyntax AsNewClause(KeywordSyntax asKeyword, NewExpressionSyntax newExpression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(124, asKeyword, newExpression, out hash);
			if (greenNode != null)
			{
				return (AsNewClauseSyntax)greenNode;
			}
			AsNewClauseSyntax asNewClauseSyntax = new AsNewClauseSyntax(SyntaxKind.AsNewClause, asKeyword, newExpression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(asNewClauseSyntax, hash);
			}
			return asNewClauseSyntax;
		}

		internal static ObjectMemberInitializerSyntax ObjectMemberInitializer(KeywordSyntax withKeyword, PunctuationSyntax openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> initializers, PunctuationSyntax closeBraceToken)
		{
			return new ObjectMemberInitializerSyntax(SyntaxKind.ObjectMemberInitializer, withKeyword, openBraceToken, initializers.Node, closeBraceToken);
		}

		internal static ObjectCollectionInitializerSyntax ObjectCollectionInitializer(KeywordSyntax fromKeyword, CollectionInitializerSyntax initializer)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(126, fromKeyword, initializer, out hash);
			if (greenNode != null)
			{
				return (ObjectCollectionInitializerSyntax)greenNode;
			}
			ObjectCollectionInitializerSyntax objectCollectionInitializerSyntax = new ObjectCollectionInitializerSyntax(SyntaxKind.ObjectCollectionInitializer, fromKeyword, initializer);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(objectCollectionInitializerSyntax, hash);
			}
			return objectCollectionInitializerSyntax;
		}

		internal static InferredFieldInitializerSyntax InferredFieldInitializer(KeywordSyntax keyKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(127, keyKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (InferredFieldInitializerSyntax)greenNode;
			}
			InferredFieldInitializerSyntax inferredFieldInitializerSyntax = new InferredFieldInitializerSyntax(SyntaxKind.InferredFieldInitializer, keyKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(inferredFieldInitializerSyntax, hash);
			}
			return inferredFieldInitializerSyntax;
		}

		internal static NamedFieldInitializerSyntax NamedFieldInitializer(KeywordSyntax keyKeyword, PunctuationSyntax dotToken, IdentifierNameSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax expression)
		{
			return new NamedFieldInitializerSyntax(SyntaxKind.NamedFieldInitializer, keyKeyword, dotToken, name, equalsToken, expression);
		}

		internal static EqualsValueSyntax EqualsValue(PunctuationSyntax equalsToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(129, equalsToken, value, out hash);
			if (greenNode != null)
			{
				return (EqualsValueSyntax)greenNode;
			}
			EqualsValueSyntax equalsValueSyntax = new EqualsValueSyntax(SyntaxKind.EqualsValue, equalsToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(equalsValueSyntax, hash);
			}
			return equalsValueSyntax;
		}

		internal static ParameterSyntax Parameter(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, EqualsValueSyntax @default)
		{
			return new ParameterSyntax(SyntaxKind.Parameter, attributeLists.Node, modifiers.Node, identifier, asClause, @default);
		}

		internal static ModifiedIdentifierSyntax ModifiedIdentifier(IdentifierTokenSyntax identifier, PunctuationSyntax nullable, ArgumentListSyntax arrayBounds, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> arrayRankSpecifiers)
		{
			return new ModifiedIdentifierSyntax(SyntaxKind.ModifiedIdentifier, identifier, nullable, arrayBounds, arrayRankSpecifiers.Node);
		}

		internal static ArrayRankSpecifierSyntax ArrayRankSpecifier(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> commaTokens, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(134, openParenToken, commaTokens.Node, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (ArrayRankSpecifierSyntax)greenNode;
			}
			ArrayRankSpecifierSyntax arrayRankSpecifierSyntax = new ArrayRankSpecifierSyntax(SyntaxKind.ArrayRankSpecifier, openParenToken, commaTokens.Node, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(arrayRankSpecifierSyntax, hash);
			}
			return arrayRankSpecifierSyntax;
		}

		internal static AttributeListSyntax AttributeList(PunctuationSyntax lessThanToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> attributes, PunctuationSyntax greaterThanToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(135, lessThanToken, attributes.Node, greaterThanToken, out hash);
			if (greenNode != null)
			{
				return (AttributeListSyntax)greenNode;
			}
			AttributeListSyntax attributeListSyntax = new AttributeListSyntax(SyntaxKind.AttributeList, lessThanToken, attributes.Node, greaterThanToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributeListSyntax, hash);
			}
			return attributeListSyntax;
		}

		internal static AttributeSyntax Attribute(AttributeTargetSyntax target, TypeSyntax name, ArgumentListSyntax argumentList)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(136, target, name, argumentList, out hash);
			if (greenNode != null)
			{
				return (AttributeSyntax)greenNode;
			}
			AttributeSyntax attributeSyntax = new AttributeSyntax(SyntaxKind.Attribute, target, name, argumentList);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributeSyntax, hash);
			}
			return attributeSyntax;
		}

		internal static AttributeTargetSyntax AttributeTarget(KeywordSyntax attributeModifier, PunctuationSyntax colonToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(137, attributeModifier, colonToken, out hash);
			if (greenNode != null)
			{
				return (AttributeTargetSyntax)greenNode;
			}
			AttributeTargetSyntax attributeTargetSyntax = new AttributeTargetSyntax(SyntaxKind.AttributeTarget, attributeModifier, colonToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributeTargetSyntax, hash);
			}
			return attributeTargetSyntax;
		}

		internal static AttributesStatementSyntax AttributesStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(138, attributeLists.Node, out hash);
			if (greenNode != null)
			{
				return (AttributesStatementSyntax)greenNode;
			}
			AttributesStatementSyntax attributesStatementSyntax = new AttributesStatementSyntax(SyntaxKind.AttributesStatement, attributeLists.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(attributesStatementSyntax, hash);
			}
			return attributesStatementSyntax;
		}

		internal static ExpressionStatementSyntax ExpressionStatement(ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(139, expression, out hash);
			if (greenNode != null)
			{
				return (ExpressionStatementSyntax)greenNode;
			}
			ExpressionStatementSyntax expressionStatementSyntax = new ExpressionStatementSyntax(SyntaxKind.ExpressionStatement, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(expressionStatementSyntax, hash);
			}
			return expressionStatementSyntax;
		}

		internal static PrintStatementSyntax PrintStatement(PunctuationSyntax questionToken, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(140, questionToken, expression, out hash);
			if (greenNode != null)
			{
				return (PrintStatementSyntax)greenNode;
			}
			PrintStatementSyntax printStatementSyntax = new PrintStatementSyntax(SyntaxKind.PrintStatement, questionToken, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(printStatementSyntax, hash);
			}
			return printStatementSyntax;
		}

		internal static WhileBlockSyntax WhileBlock(WhileStatementSyntax whileStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endWhileStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(141, whileStatement, statements.Node, endWhileStatement, out hash);
			if (greenNode != null)
			{
				return (WhileBlockSyntax)greenNode;
			}
			WhileBlockSyntax whileBlockSyntax = new WhileBlockSyntax(SyntaxKind.WhileBlock, whileStatement, statements.Node, endWhileStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileBlockSyntax, hash);
			}
			return whileBlockSyntax;
		}

		internal static UsingBlockSyntax UsingBlock(UsingStatementSyntax usingStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endUsingStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(144, usingStatement, statements.Node, endUsingStatement, out hash);
			if (greenNode != null)
			{
				return (UsingBlockSyntax)greenNode;
			}
			UsingBlockSyntax usingBlockSyntax = new UsingBlockSyntax(SyntaxKind.UsingBlock, usingStatement, statements.Node, endUsingStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(usingBlockSyntax, hash);
			}
			return usingBlockSyntax;
		}

		internal static SyncLockBlockSyntax SyncLockBlock(SyncLockStatementSyntax syncLockStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSyncLockStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(145, syncLockStatement, statements.Node, endSyncLockStatement, out hash);
			if (greenNode != null)
			{
				return (SyncLockBlockSyntax)greenNode;
			}
			SyncLockBlockSyntax syncLockBlockSyntax = new SyncLockBlockSyntax(SyntaxKind.SyncLockBlock, syncLockStatement, statements.Node, endSyncLockStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(syncLockBlockSyntax, hash);
			}
			return syncLockBlockSyntax;
		}

		internal static WithBlockSyntax WithBlock(WithStatementSyntax withStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endWithStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(146, withStatement, statements.Node, endWithStatement, out hash);
			if (greenNode != null)
			{
				return (WithBlockSyntax)greenNode;
			}
			WithBlockSyntax withBlockSyntax = new WithBlockSyntax(SyntaxKind.WithBlock, withStatement, statements.Node, endWithStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withBlockSyntax, hash);
			}
			return withBlockSyntax;
		}

		internal static LocalDeclarationStatementSyntax LocalDeclarationStatement(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> declarators)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(147, modifiers.Node, declarators.Node, out hash);
			if (greenNode != null)
			{
				return (LocalDeclarationStatementSyntax)greenNode;
			}
			LocalDeclarationStatementSyntax localDeclarationStatementSyntax = new LocalDeclarationStatementSyntax(SyntaxKind.LocalDeclarationStatement, modifiers.Node, declarators.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(localDeclarationStatementSyntax, hash);
			}
			return localDeclarationStatementSyntax;
		}

		internal static LabelStatementSyntax LabelStatement(SyntaxToken labelToken, PunctuationSyntax colonToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(148, labelToken, colonToken, out hash);
			if (greenNode != null)
			{
				return (LabelStatementSyntax)greenNode;
			}
			LabelStatementSyntax labelStatementSyntax = new LabelStatementSyntax(SyntaxKind.LabelStatement, labelToken, colonToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelStatementSyntax, hash);
			}
			return labelStatementSyntax;
		}

		internal static GoToStatementSyntax GoToStatement(KeywordSyntax goToKeyword, LabelSyntax label)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(149, goToKeyword, label, out hash);
			if (greenNode != null)
			{
				return (GoToStatementSyntax)greenNode;
			}
			GoToStatementSyntax goToStatementSyntax = new GoToStatementSyntax(SyntaxKind.GoToStatement, goToKeyword, label);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(goToStatementSyntax, hash);
			}
			return goToStatementSyntax;
		}

		internal static LabelSyntax IdentifierLabel(SyntaxToken labelToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(150, labelToken, out hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(SyntaxKind.IdentifierLabel, labelToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal static LabelSyntax NumericLabel(SyntaxToken labelToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(151, labelToken, out hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(SyntaxKind.NumericLabel, labelToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal static LabelSyntax NextLabel(SyntaxToken labelToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(152, labelToken, out hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(SyntaxKind.NextLabel, labelToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal static LabelSyntax Label(SyntaxKind kind, SyntaxToken labelToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, labelToken, out hash);
			if (greenNode != null)
			{
				return (LabelSyntax)greenNode;
			}
			LabelSyntax labelSyntax = new LabelSyntax(kind, labelToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(labelSyntax, hash);
			}
			return labelSyntax;
		}

		internal static StopOrEndStatementSyntax StopStatement(KeywordSyntax stopOrEndKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(153, stopOrEndKeyword, out hash);
			if (greenNode != null)
			{
				return (StopOrEndStatementSyntax)greenNode;
			}
			StopOrEndStatementSyntax stopOrEndStatementSyntax = new StopOrEndStatementSyntax(SyntaxKind.StopStatement, stopOrEndKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(stopOrEndStatementSyntax, hash);
			}
			return stopOrEndStatementSyntax;
		}

		internal static StopOrEndStatementSyntax EndStatement(KeywordSyntax stopOrEndKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(156, stopOrEndKeyword, out hash);
			if (greenNode != null)
			{
				return (StopOrEndStatementSyntax)greenNode;
			}
			StopOrEndStatementSyntax stopOrEndStatementSyntax = new StopOrEndStatementSyntax(SyntaxKind.EndStatement, stopOrEndKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(stopOrEndStatementSyntax, hash);
			}
			return stopOrEndStatementSyntax;
		}

		internal static StopOrEndStatementSyntax StopOrEndStatement(SyntaxKind kind, KeywordSyntax stopOrEndKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, stopOrEndKeyword, out hash);
			if (greenNode != null)
			{
				return (StopOrEndStatementSyntax)greenNode;
			}
			StopOrEndStatementSyntax stopOrEndStatementSyntax = new StopOrEndStatementSyntax(kind, stopOrEndKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(stopOrEndStatementSyntax, hash);
			}
			return stopOrEndStatementSyntax;
		}

		internal static ExitStatementSyntax ExitDoStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(157, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitDoStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitForStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(158, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitForStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitSubStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(159, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitSubStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitFunctionStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(160, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitFunctionStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitOperatorStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(161, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitOperatorStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitPropertyStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(162, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitPropertyStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitTryStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(163, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitTryStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitSelectStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(164, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitSelectStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitWhileStatement(KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(165, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(SyntaxKind.ExitWhileStatement, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ExitStatementSyntax ExitStatement(SyntaxKind kind, KeywordSyntax exitKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, exitKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ExitStatementSyntax)greenNode;
			}
			ExitStatementSyntax exitStatementSyntax = new ExitStatementSyntax(kind, exitKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(exitStatementSyntax, hash);
			}
			return exitStatementSyntax;
		}

		internal static ContinueStatementSyntax ContinueWhileStatement(KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(166, continueKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(SyntaxKind.ContinueWhileStatement, continueKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal static ContinueStatementSyntax ContinueDoStatement(KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(167, continueKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(SyntaxKind.ContinueDoStatement, continueKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal static ContinueStatementSyntax ContinueForStatement(KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(168, continueKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(SyntaxKind.ContinueForStatement, continueKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal static ContinueStatementSyntax ContinueStatement(SyntaxKind kind, KeywordSyntax continueKeyword, KeywordSyntax blockKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, continueKeyword, blockKeyword, out hash);
			if (greenNode != null)
			{
				return (ContinueStatementSyntax)greenNode;
			}
			ContinueStatementSyntax continueStatementSyntax = new ContinueStatementSyntax(kind, continueKeyword, blockKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(continueStatementSyntax, hash);
			}
			return continueStatementSyntax;
		}

		internal static ReturnStatementSyntax ReturnStatement(KeywordSyntax returnKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(169, returnKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (ReturnStatementSyntax)greenNode;
			}
			ReturnStatementSyntax returnStatementSyntax = new ReturnStatementSyntax(SyntaxKind.ReturnStatement, returnKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(returnStatementSyntax, hash);
			}
			return returnStatementSyntax;
		}

		internal static SingleLineIfStatementSyntax SingleLineIfStatement(KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, SingleLineElseClauseSyntax elseClause)
		{
			return new SingleLineIfStatementSyntax(SyntaxKind.SingleLineIfStatement, ifKeyword, condition, thenKeyword, statements.Node, elseClause);
		}

		internal static SingleLineElseClauseSyntax SingleLineElseClause(KeywordSyntax elseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(172, elseKeyword, statements.Node, out hash);
			if (greenNode != null)
			{
				return (SingleLineElseClauseSyntax)greenNode;
			}
			SingleLineElseClauseSyntax singleLineElseClauseSyntax = new SingleLineElseClauseSyntax(SyntaxKind.SingleLineElseClause, elseKeyword, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineElseClauseSyntax, hash);
			}
			return singleLineElseClauseSyntax;
		}

		internal static MultiLineIfBlockSyntax MultiLineIfBlock(IfStatementSyntax ifStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> elseIfBlocks, ElseBlockSyntax elseBlock, EndBlockStatementSyntax endIfStatement)
		{
			return new MultiLineIfBlockSyntax(SyntaxKind.MultiLineIfBlock, ifStatement, statements.Node, elseIfBlocks.Node, elseBlock, endIfStatement);
		}

		internal static IfStatementSyntax IfStatement(KeywordSyntax ifKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(182, ifKeyword, condition, thenKeyword, out hash);
			if (greenNode != null)
			{
				return (IfStatementSyntax)greenNode;
			}
			IfStatementSyntax ifStatementSyntax = new IfStatementSyntax(SyntaxKind.IfStatement, ifKeyword, condition, thenKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(ifStatementSyntax, hash);
			}
			return ifStatementSyntax;
		}

		internal static ElseIfBlockSyntax ElseIfBlock(ElseIfStatementSyntax elseIfStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(180, elseIfStatement, statements.Node, out hash);
			if (greenNode != null)
			{
				return (ElseIfBlockSyntax)greenNode;
			}
			ElseIfBlockSyntax elseIfBlockSyntax = new ElseIfBlockSyntax(SyntaxKind.ElseIfBlock, elseIfStatement, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseIfBlockSyntax, hash);
			}
			return elseIfBlockSyntax;
		}

		internal static ElseIfStatementSyntax ElseIfStatement(KeywordSyntax elseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(183, elseIfKeyword, condition, thenKeyword, out hash);
			if (greenNode != null)
			{
				return (ElseIfStatementSyntax)greenNode;
			}
			ElseIfStatementSyntax elseIfStatementSyntax = new ElseIfStatementSyntax(SyntaxKind.ElseIfStatement, elseIfKeyword, condition, thenKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseIfStatementSyntax, hash);
			}
			return elseIfStatementSyntax;
		}

		internal static ElseBlockSyntax ElseBlock(ElseStatementSyntax elseStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(181, elseStatement, statements.Node, out hash);
			if (greenNode != null)
			{
				return (ElseBlockSyntax)greenNode;
			}
			ElseBlockSyntax elseBlockSyntax = new ElseBlockSyntax(SyntaxKind.ElseBlock, elseStatement, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseBlockSyntax, hash);
			}
			return elseBlockSyntax;
		}

		internal static ElseStatementSyntax ElseStatement(KeywordSyntax elseKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(184, elseKeyword, out hash);
			if (greenNode != null)
			{
				return (ElseStatementSyntax)greenNode;
			}
			ElseStatementSyntax elseStatementSyntax = new ElseStatementSyntax(SyntaxKind.ElseStatement, elseKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseStatementSyntax, hash);
			}
			return elseStatementSyntax;
		}

		internal static TryBlockSyntax TryBlock(TryStatementSyntax tryStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> catchBlocks, FinallyBlockSyntax finallyBlock, EndBlockStatementSyntax endTryStatement)
		{
			return new TryBlockSyntax(SyntaxKind.TryBlock, tryStatement, statements.Node, catchBlocks.Node, finallyBlock, endTryStatement);
		}

		internal static TryStatementSyntax TryStatement(KeywordSyntax tryKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(189, tryKeyword, out hash);
			if (greenNode != null)
			{
				return (TryStatementSyntax)greenNode;
			}
			TryStatementSyntax tryStatementSyntax = new TryStatementSyntax(SyntaxKind.TryStatement, tryKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(tryStatementSyntax, hash);
			}
			return tryStatementSyntax;
		}

		internal static CatchBlockSyntax CatchBlock(CatchStatementSyntax catchStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(187, catchStatement, statements.Node, out hash);
			if (greenNode != null)
			{
				return (CatchBlockSyntax)greenNode;
			}
			CatchBlockSyntax catchBlockSyntax = new CatchBlockSyntax(SyntaxKind.CatchBlock, catchStatement, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(catchBlockSyntax, hash);
			}
			return catchBlockSyntax;
		}

		internal static CatchStatementSyntax CatchStatement(KeywordSyntax catchKeyword, IdentifierNameSyntax identifierName, SimpleAsClauseSyntax asClause, CatchFilterClauseSyntax whenClause)
		{
			return new CatchStatementSyntax(SyntaxKind.CatchStatement, catchKeyword, identifierName, asClause, whenClause);
		}

		internal static CatchFilterClauseSyntax CatchFilterClause(KeywordSyntax whenKeyword, ExpressionSyntax filter)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(191, whenKeyword, filter, out hash);
			if (greenNode != null)
			{
				return (CatchFilterClauseSyntax)greenNode;
			}
			CatchFilterClauseSyntax catchFilterClauseSyntax = new CatchFilterClauseSyntax(SyntaxKind.CatchFilterClause, whenKeyword, filter);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(catchFilterClauseSyntax, hash);
			}
			return catchFilterClauseSyntax;
		}

		internal static FinallyBlockSyntax FinallyBlock(FinallyStatementSyntax finallyStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(188, finallyStatement, statements.Node, out hash);
			if (greenNode != null)
			{
				return (FinallyBlockSyntax)greenNode;
			}
			FinallyBlockSyntax finallyBlockSyntax = new FinallyBlockSyntax(SyntaxKind.FinallyBlock, finallyStatement, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(finallyBlockSyntax, hash);
			}
			return finallyBlockSyntax;
		}

		internal static FinallyStatementSyntax FinallyStatement(KeywordSyntax finallyKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(194, finallyKeyword, out hash);
			if (greenNode != null)
			{
				return (FinallyStatementSyntax)greenNode;
			}
			FinallyStatementSyntax finallyStatementSyntax = new FinallyStatementSyntax(SyntaxKind.FinallyStatement, finallyKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(finallyStatementSyntax, hash);
			}
			return finallyStatementSyntax;
		}

		internal static ErrorStatementSyntax ErrorStatement(KeywordSyntax errorKeyword, ExpressionSyntax errorNumber)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(195, errorKeyword, errorNumber, out hash);
			if (greenNode != null)
			{
				return (ErrorStatementSyntax)greenNode;
			}
			ErrorStatementSyntax errorStatementSyntax = new ErrorStatementSyntax(SyntaxKind.ErrorStatement, errorKeyword, errorNumber);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(errorStatementSyntax, hash);
			}
			return errorStatementSyntax;
		}

		internal static OnErrorGoToStatementSyntax OnErrorGoToZeroStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToZeroStatement, onKeyword, errorKeyword, goToKeyword, minus, label);
		}

		internal static OnErrorGoToStatementSyntax OnErrorGoToMinusOneStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToMinusOneStatement, onKeyword, errorKeyword, goToKeyword, minus, label);
		}

		internal static OnErrorGoToStatementSyntax OnErrorGoToLabelStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(SyntaxKind.OnErrorGoToLabelStatement, onKeyword, errorKeyword, goToKeyword, minus, label);
		}

		internal static OnErrorGoToStatementSyntax OnErrorGoToStatement(SyntaxKind kind, KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax goToKeyword, PunctuationSyntax minus, LabelSyntax label)
		{
			return new OnErrorGoToStatementSyntax(kind, onKeyword, errorKeyword, goToKeyword, minus, label);
		}

		internal static OnErrorResumeNextStatementSyntax OnErrorResumeNextStatement(KeywordSyntax onKeyword, KeywordSyntax errorKeyword, KeywordSyntax resumeKeyword, KeywordSyntax nextKeyword)
		{
			return new OnErrorResumeNextStatementSyntax(SyntaxKind.OnErrorResumeNextStatement, onKeyword, errorKeyword, resumeKeyword, nextKeyword);
		}

		internal static ResumeStatementSyntax ResumeStatement(KeywordSyntax resumeKeyword, LabelSyntax label)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(200, resumeKeyword, label, out hash);
			if (greenNode != null)
			{
				return (ResumeStatementSyntax)greenNode;
			}
			ResumeStatementSyntax resumeStatementSyntax = new ResumeStatementSyntax(SyntaxKind.ResumeStatement, resumeKeyword, label);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(resumeStatementSyntax, hash);
			}
			return resumeStatementSyntax;
		}

		internal static ResumeStatementSyntax ResumeLabelStatement(KeywordSyntax resumeKeyword, LabelSyntax label)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(201, resumeKeyword, label, out hash);
			if (greenNode != null)
			{
				return (ResumeStatementSyntax)greenNode;
			}
			ResumeStatementSyntax resumeStatementSyntax = new ResumeStatementSyntax(SyntaxKind.ResumeLabelStatement, resumeKeyword, label);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(resumeStatementSyntax, hash);
			}
			return resumeStatementSyntax;
		}

		internal static ResumeStatementSyntax ResumeNextStatement(KeywordSyntax resumeKeyword, LabelSyntax label)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(202, resumeKeyword, label, out hash);
			if (greenNode != null)
			{
				return (ResumeStatementSyntax)greenNode;
			}
			ResumeStatementSyntax resumeStatementSyntax = new ResumeStatementSyntax(SyntaxKind.ResumeNextStatement, resumeKeyword, label);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(resumeStatementSyntax, hash);
			}
			return resumeStatementSyntax;
		}

		internal static SelectBlockSyntax SelectBlock(SelectStatementSyntax selectStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> caseBlocks, EndBlockStatementSyntax endSelectStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(203, selectStatement, caseBlocks.Node, endSelectStatement, out hash);
			if (greenNode != null)
			{
				return (SelectBlockSyntax)greenNode;
			}
			SelectBlockSyntax selectBlockSyntax = new SelectBlockSyntax(SyntaxKind.SelectBlock, selectStatement, caseBlocks.Node, endSelectStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(selectBlockSyntax, hash);
			}
			return selectBlockSyntax;
		}

		internal static SelectStatementSyntax SelectStatement(KeywordSyntax selectKeyword, KeywordSyntax caseKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(204, selectKeyword, caseKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (SelectStatementSyntax)greenNode;
			}
			SelectStatementSyntax selectStatementSyntax = new SelectStatementSyntax(SyntaxKind.SelectStatement, selectKeyword, caseKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(selectStatementSyntax, hash);
			}
			return selectStatementSyntax;
		}

		internal static CaseBlockSyntax CaseBlock(CaseStatementSyntax caseStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(207, caseStatement, statements.Node, out hash);
			if (greenNode != null)
			{
				return (CaseBlockSyntax)greenNode;
			}
			CaseBlockSyntax caseBlockSyntax = new CaseBlockSyntax(SyntaxKind.CaseBlock, caseStatement, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseBlockSyntax, hash);
			}
			return caseBlockSyntax;
		}

		internal static CaseBlockSyntax CaseElseBlock(CaseStatementSyntax caseStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(210, caseStatement, statements.Node, out hash);
			if (greenNode != null)
			{
				return (CaseBlockSyntax)greenNode;
			}
			CaseBlockSyntax caseBlockSyntax = new CaseBlockSyntax(SyntaxKind.CaseElseBlock, caseStatement, statements.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseBlockSyntax, hash);
			}
			return caseBlockSyntax;
		}

		internal static CaseStatementSyntax CaseStatement(KeywordSyntax caseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> cases)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(211, caseKeyword, cases.Node, out hash);
			if (greenNode != null)
			{
				return (CaseStatementSyntax)greenNode;
			}
			CaseStatementSyntax caseStatementSyntax = new CaseStatementSyntax(SyntaxKind.CaseStatement, caseKeyword, cases.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseStatementSyntax, hash);
			}
			return caseStatementSyntax;
		}

		internal static CaseStatementSyntax CaseElseStatement(KeywordSyntax caseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> cases)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(212, caseKeyword, cases.Node, out hash);
			if (greenNode != null)
			{
				return (CaseStatementSyntax)greenNode;
			}
			CaseStatementSyntax caseStatementSyntax = new CaseStatementSyntax(SyntaxKind.CaseElseStatement, caseKeyword, cases.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(caseStatementSyntax, hash);
			}
			return caseStatementSyntax;
		}

		internal static ElseCaseClauseSyntax ElseCaseClause(KeywordSyntax elseKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(213, elseKeyword, out hash);
			if (greenNode != null)
			{
				return (ElseCaseClauseSyntax)greenNode;
			}
			ElseCaseClauseSyntax elseCaseClauseSyntax = new ElseCaseClauseSyntax(SyntaxKind.ElseCaseClause, elseKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(elseCaseClauseSyntax, hash);
			}
			return elseCaseClauseSyntax;
		}

		internal static SimpleCaseClauseSyntax SimpleCaseClause(ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(214, value, out hash);
			if (greenNode != null)
			{
				return (SimpleCaseClauseSyntax)greenNode;
			}
			SimpleCaseClauseSyntax simpleCaseClauseSyntax = new SimpleCaseClauseSyntax(SyntaxKind.SimpleCaseClause, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleCaseClauseSyntax, hash);
			}
			return simpleCaseClauseSyntax;
		}

		internal static RangeCaseClauseSyntax RangeCaseClause(ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(215, lowerBound, toKeyword, upperBound, out hash);
			if (greenNode != null)
			{
				return (RangeCaseClauseSyntax)greenNode;
			}
			RangeCaseClauseSyntax rangeCaseClauseSyntax = new RangeCaseClauseSyntax(SyntaxKind.RangeCaseClause, lowerBound, toKeyword, upperBound);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(rangeCaseClauseSyntax, hash);
			}
			return rangeCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax CaseEqualsClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(216, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseEqualsClause, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax CaseNotEqualsClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(217, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseNotEqualsClause, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax CaseLessThanClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(218, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanClause, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax CaseLessThanOrEqualClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(219, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseLessThanOrEqualClause, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax CaseGreaterThanOrEqualClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(222, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanOrEqualClause, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax CaseGreaterThanClause(KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(223, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(SyntaxKind.CaseGreaterThanClause, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static RelationalCaseClauseSyntax RelationalCaseClause(SyntaxKind kind, KeywordSyntax isKeyword, PunctuationSyntax operatorToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, isKeyword, operatorToken, value, out hash);
			if (greenNode != null)
			{
				return (RelationalCaseClauseSyntax)greenNode;
			}
			RelationalCaseClauseSyntax relationalCaseClauseSyntax = new RelationalCaseClauseSyntax(kind, isKeyword, operatorToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(relationalCaseClauseSyntax, hash);
			}
			return relationalCaseClauseSyntax;
		}

		internal static SyncLockStatementSyntax SyncLockStatement(KeywordSyntax syncLockKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(226, syncLockKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (SyncLockStatementSyntax)greenNode;
			}
			SyncLockStatementSyntax syncLockStatementSyntax = new SyncLockStatementSyntax(SyntaxKind.SyncLockStatement, syncLockKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(syncLockStatementSyntax, hash);
			}
			return syncLockStatementSyntax;
		}

		internal static DoLoopBlockSyntax SimpleDoLoopBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(756, doStatement, statements.Node, loopStatement, out hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.SimpleDoLoopBlock, doStatement, statements.Node, loopStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal static DoLoopBlockSyntax DoWhileLoopBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(757, doStatement, statements.Node, loopStatement, out hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoWhileLoopBlock, doStatement, statements.Node, loopStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal static DoLoopBlockSyntax DoUntilLoopBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(758, doStatement, statements.Node, loopStatement, out hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoUntilLoopBlock, doStatement, statements.Node, loopStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal static DoLoopBlockSyntax DoLoopWhileBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(759, doStatement, statements.Node, loopStatement, out hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoLoopWhileBlock, doStatement, statements.Node, loopStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal static DoLoopBlockSyntax DoLoopUntilBlock(DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(760, doStatement, statements.Node, loopStatement, out hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(SyntaxKind.DoLoopUntilBlock, doStatement, statements.Node, loopStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal static DoLoopBlockSyntax DoLoopBlock(SyntaxKind kind, DoStatementSyntax doStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, LoopStatementSyntax loopStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, doStatement, statements.Node, loopStatement, out hash);
			if (greenNode != null)
			{
				return (DoLoopBlockSyntax)greenNode;
			}
			DoLoopBlockSyntax doLoopBlockSyntax = new DoLoopBlockSyntax(kind, doStatement, statements.Node, loopStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doLoopBlockSyntax, hash);
			}
			return doLoopBlockSyntax;
		}

		internal static DoStatementSyntax SimpleDoStatement(KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(770, doKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(SyntaxKind.SimpleDoStatement, doKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal static DoStatementSyntax DoWhileStatement(KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(771, doKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(SyntaxKind.DoWhileStatement, doKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal static DoStatementSyntax DoUntilStatement(KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(772, doKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(SyntaxKind.DoUntilStatement, doKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal static DoStatementSyntax DoStatement(SyntaxKind kind, KeywordSyntax doKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, doKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (DoStatementSyntax)greenNode;
			}
			DoStatementSyntax doStatementSyntax = new DoStatementSyntax(kind, doKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(doStatementSyntax, hash);
			}
			return doStatementSyntax;
		}

		internal static LoopStatementSyntax SimpleLoopStatement(KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(773, loopKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(SyntaxKind.SimpleLoopStatement, loopKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal static LoopStatementSyntax LoopWhileStatement(KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(774, loopKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(SyntaxKind.LoopWhileStatement, loopKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal static LoopStatementSyntax LoopUntilStatement(KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(775, loopKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(SyntaxKind.LoopUntilStatement, loopKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal static LoopStatementSyntax LoopStatement(SyntaxKind kind, KeywordSyntax loopKeyword, WhileOrUntilClauseSyntax whileOrUntilClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, loopKeyword, whileOrUntilClause, out hash);
			if (greenNode != null)
			{
				return (LoopStatementSyntax)greenNode;
			}
			LoopStatementSyntax loopStatementSyntax = new LoopStatementSyntax(kind, loopKeyword, whileOrUntilClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(loopStatementSyntax, hash);
			}
			return loopStatementSyntax;
		}

		internal static WhileOrUntilClauseSyntax WhileClause(KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(776, whileOrUntilKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (WhileOrUntilClauseSyntax)greenNode;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = new WhileOrUntilClauseSyntax(SyntaxKind.WhileClause, whileOrUntilKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax, hash);
			}
			return whileOrUntilClauseSyntax;
		}

		internal static WhileOrUntilClauseSyntax UntilClause(KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(777, whileOrUntilKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (WhileOrUntilClauseSyntax)greenNode;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = new WhileOrUntilClauseSyntax(SyntaxKind.UntilClause, whileOrUntilKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax, hash);
			}
			return whileOrUntilClauseSyntax;
		}

		internal static WhileOrUntilClauseSyntax WhileOrUntilClause(SyntaxKind kind, KeywordSyntax whileOrUntilKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, whileOrUntilKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (WhileOrUntilClauseSyntax)greenNode;
			}
			WhileOrUntilClauseSyntax whileOrUntilClauseSyntax = new WhileOrUntilClauseSyntax(kind, whileOrUntilKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileOrUntilClauseSyntax, hash);
			}
			return whileOrUntilClauseSyntax;
		}

		internal static WhileStatementSyntax WhileStatement(KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(234, whileKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (WhileStatementSyntax)greenNode;
			}
			WhileStatementSyntax whileStatementSyntax = new WhileStatementSyntax(SyntaxKind.WhileStatement, whileKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whileStatementSyntax, hash);
			}
			return whileStatementSyntax;
		}

		internal static ForBlockSyntax ForBlock(ForStatementSyntax forStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, NextStatementSyntax nextStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(237, forStatement, statements.Node, nextStatement, out hash);
			if (greenNode != null)
			{
				return (ForBlockSyntax)greenNode;
			}
			ForBlockSyntax forBlockSyntax = new ForBlockSyntax(SyntaxKind.ForBlock, forStatement, statements.Node, nextStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(forBlockSyntax, hash);
			}
			return forBlockSyntax;
		}

		internal static ForEachBlockSyntax ForEachBlock(ForEachStatementSyntax forEachStatement, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, NextStatementSyntax nextStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(238, forEachStatement, statements.Node, nextStatement, out hash);
			if (greenNode != null)
			{
				return (ForEachBlockSyntax)greenNode;
			}
			ForEachBlockSyntax forEachBlockSyntax = new ForEachBlockSyntax(SyntaxKind.ForEachBlock, forEachStatement, statements.Node, nextStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(forEachBlockSyntax, hash);
			}
			return forEachBlockSyntax;
		}

		internal static ForStatementSyntax ForStatement(KeywordSyntax forKeyword, VisualBasicSyntaxNode controlVariable, PunctuationSyntax equalsToken, ExpressionSyntax fromValue, KeywordSyntax toKeyword, ExpressionSyntax toValue, ForStepClauseSyntax stepClause)
		{
			return new ForStatementSyntax(SyntaxKind.ForStatement, forKeyword, controlVariable, equalsToken, fromValue, toKeyword, toValue, stepClause);
		}

		internal static ForStepClauseSyntax ForStepClause(KeywordSyntax stepKeyword, ExpressionSyntax stepValue)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(240, stepKeyword, stepValue, out hash);
			if (greenNode != null)
			{
				return (ForStepClauseSyntax)greenNode;
			}
			ForStepClauseSyntax forStepClauseSyntax = new ForStepClauseSyntax(SyntaxKind.ForStepClause, stepKeyword, stepValue);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(forStepClauseSyntax, hash);
			}
			return forStepClauseSyntax;
		}

		internal static ForEachStatementSyntax ForEachStatement(KeywordSyntax forKeyword, KeywordSyntax eachKeyword, VisualBasicSyntaxNode controlVariable, KeywordSyntax inKeyword, ExpressionSyntax expression)
		{
			return new ForEachStatementSyntax(SyntaxKind.ForEachStatement, forKeyword, eachKeyword, controlVariable, inKeyword, expression);
		}

		internal static NextStatementSyntax NextStatement(KeywordSyntax nextKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> controlVariables)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(242, nextKeyword, controlVariables.Node, out hash);
			if (greenNode != null)
			{
				return (NextStatementSyntax)greenNode;
			}
			NextStatementSyntax nextStatementSyntax = new NextStatementSyntax(SyntaxKind.NextStatement, nextKeyword, controlVariables.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(nextStatementSyntax, hash);
			}
			return nextStatementSyntax;
		}

		internal static UsingStatementSyntax UsingStatement(KeywordSyntax usingKeyword, ExpressionSyntax expression, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(243, usingKeyword, expression, variables.Node, out hash);
			if (greenNode != null)
			{
				return (UsingStatementSyntax)greenNode;
			}
			UsingStatementSyntax usingStatementSyntax = new UsingStatementSyntax(SyntaxKind.UsingStatement, usingKeyword, expression, variables.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(usingStatementSyntax, hash);
			}
			return usingStatementSyntax;
		}

		internal static ThrowStatementSyntax ThrowStatement(KeywordSyntax throwKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(246, throwKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (ThrowStatementSyntax)greenNode;
			}
			ThrowStatementSyntax throwStatementSyntax = new ThrowStatementSyntax(SyntaxKind.ThrowStatement, throwKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(throwStatementSyntax, hash);
			}
			return throwStatementSyntax;
		}

		internal static AssignmentStatementSyntax SimpleAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(247, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.SimpleAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax MidAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(248, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.MidAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax AddAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(249, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.AddAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax SubtractAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(250, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.SubtractAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax MultiplyAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(251, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.MultiplyAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax DivideAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(252, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.DivideAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax IntegerDivideAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(253, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.IntegerDivideAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax ExponentiateAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(254, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.ExponentiateAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax LeftShiftAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(255, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.LeftShiftAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax RightShiftAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(258, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.RightShiftAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax ConcatenateAssignmentStatement(ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(259, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(SyntaxKind.ConcatenateAssignmentStatement, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static AssignmentStatementSyntax AssignmentStatement(SyntaxKind kind, ExpressionSyntax left, PunctuationSyntax operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (AssignmentStatementSyntax)greenNode;
			}
			AssignmentStatementSyntax assignmentStatementSyntax = new AssignmentStatementSyntax(kind, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(assignmentStatementSyntax, hash);
			}
			return assignmentStatementSyntax;
		}

		internal static MidExpressionSyntax MidExpression(IdentifierTokenSyntax mid, ArgumentListSyntax argumentList)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(260, mid, argumentList, out hash);
			if (greenNode != null)
			{
				return (MidExpressionSyntax)greenNode;
			}
			MidExpressionSyntax midExpressionSyntax = new MidExpressionSyntax(SyntaxKind.MidExpression, mid, argumentList);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(midExpressionSyntax, hash);
			}
			return midExpressionSyntax;
		}

		internal static CallStatementSyntax CallStatement(KeywordSyntax callKeyword, ExpressionSyntax invocation)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(261, callKeyword, invocation, out hash);
			if (greenNode != null)
			{
				return (CallStatementSyntax)greenNode;
			}
			CallStatementSyntax callStatementSyntax = new CallStatementSyntax(SyntaxKind.CallStatement, callKeyword, invocation);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(callStatementSyntax, hash);
			}
			return callStatementSyntax;
		}

		internal static AddRemoveHandlerStatementSyntax AddHandlerStatement(KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
		{
			return new AddRemoveHandlerStatementSyntax(SyntaxKind.AddHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression);
		}

		internal static AddRemoveHandlerStatementSyntax RemoveHandlerStatement(KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
		{
			return new AddRemoveHandlerStatementSyntax(SyntaxKind.RemoveHandlerStatement, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression);
		}

		internal static AddRemoveHandlerStatementSyntax AddRemoveHandlerStatement(SyntaxKind kind, KeywordSyntax addHandlerOrRemoveHandlerKeyword, ExpressionSyntax eventExpression, PunctuationSyntax commaToken, ExpressionSyntax delegateExpression)
		{
			return new AddRemoveHandlerStatementSyntax(kind, addHandlerOrRemoveHandlerKeyword, eventExpression, commaToken, delegateExpression);
		}

		internal static RaiseEventStatementSyntax RaiseEventStatement(KeywordSyntax raiseEventKeyword, IdentifierNameSyntax name, ArgumentListSyntax argumentList)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(264, raiseEventKeyword, name, argumentList, out hash);
			if (greenNode != null)
			{
				return (RaiseEventStatementSyntax)greenNode;
			}
			RaiseEventStatementSyntax raiseEventStatementSyntax = new RaiseEventStatementSyntax(SyntaxKind.RaiseEventStatement, raiseEventKeyword, name, argumentList);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(raiseEventStatementSyntax, hash);
			}
			return raiseEventStatementSyntax;
		}

		internal static WithStatementSyntax WithStatement(KeywordSyntax withKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(265, withKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (WithStatementSyntax)greenNode;
			}
			WithStatementSyntax withStatementSyntax = new WithStatementSyntax(SyntaxKind.WithStatement, withKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(withStatementSyntax, hash);
			}
			return withStatementSyntax;
		}

		internal static ReDimStatementSyntax ReDimStatement(KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> clauses)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(266, reDimKeyword, preserveKeyword, clauses.Node, out hash);
			if (greenNode != null)
			{
				return (ReDimStatementSyntax)greenNode;
			}
			ReDimStatementSyntax reDimStatementSyntax = new ReDimStatementSyntax(SyntaxKind.ReDimStatement, reDimKeyword, preserveKeyword, clauses.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(reDimStatementSyntax, hash);
			}
			return reDimStatementSyntax;
		}

		internal static ReDimStatementSyntax ReDimPreserveStatement(KeywordSyntax reDimKeyword, KeywordSyntax preserveKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> clauses)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(267, reDimKeyword, preserveKeyword, clauses.Node, out hash);
			if (greenNode != null)
			{
				return (ReDimStatementSyntax)greenNode;
			}
			ReDimStatementSyntax reDimStatementSyntax = new ReDimStatementSyntax(SyntaxKind.ReDimPreserveStatement, reDimKeyword, preserveKeyword, clauses.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(reDimStatementSyntax, hash);
			}
			return reDimStatementSyntax;
		}

		internal static RedimClauseSyntax RedimClause(ExpressionSyntax expression, ArgumentListSyntax arrayBounds)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(270, expression, arrayBounds, out hash);
			if (greenNode != null)
			{
				return (RedimClauseSyntax)greenNode;
			}
			RedimClauseSyntax redimClauseSyntax = new RedimClauseSyntax(SyntaxKind.RedimClause, expression, arrayBounds);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(redimClauseSyntax, hash);
			}
			return redimClauseSyntax;
		}

		internal static EraseStatementSyntax EraseStatement(KeywordSyntax eraseKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> expressions)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(271, eraseKeyword, expressions.Node, out hash);
			if (greenNode != null)
			{
				return (EraseStatementSyntax)greenNode;
			}
			EraseStatementSyntax eraseStatementSyntax = new EraseStatementSyntax(SyntaxKind.EraseStatement, eraseKeyword, expressions.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(eraseStatementSyntax, hash);
			}
			return eraseStatementSyntax;
		}

		internal static LiteralExpressionSyntax CharacterLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(272, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.CharacterLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax TrueLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(273, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.TrueLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax FalseLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(274, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.FalseLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax NumericLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(275, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.NumericLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax DateLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(276, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.DateLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax StringLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(279, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.StringLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax NothingLiteralExpression(SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(280, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(SyntaxKind.NothingLiteralExpression, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static LiteralExpressionSyntax LiteralExpression(SyntaxKind kind, SyntaxToken token)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, token, out hash);
			if (greenNode != null)
			{
				return (LiteralExpressionSyntax)greenNode;
			}
			LiteralExpressionSyntax literalExpressionSyntax = new LiteralExpressionSyntax(kind, token);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(literalExpressionSyntax, hash);
			}
			return literalExpressionSyntax;
		}

		internal static ParenthesizedExpressionSyntax ParenthesizedExpression(PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(281, openParenToken, expression, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (ParenthesizedExpressionSyntax)greenNode;
			}
			ParenthesizedExpressionSyntax parenthesizedExpressionSyntax = new ParenthesizedExpressionSyntax(SyntaxKind.ParenthesizedExpression, openParenToken, expression, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(parenthesizedExpressionSyntax, hash);
			}
			return parenthesizedExpressionSyntax;
		}

		internal static TupleExpressionSyntax TupleExpression(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> arguments, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(788, openParenToken, arguments.Node, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (TupleExpressionSyntax)greenNode;
			}
			TupleExpressionSyntax tupleExpressionSyntax = new TupleExpressionSyntax(SyntaxKind.TupleExpression, openParenToken, arguments.Node, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(tupleExpressionSyntax, hash);
			}
			return tupleExpressionSyntax;
		}

		internal static TupleTypeSyntax TupleType(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> elements, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(789, openParenToken, elements.Node, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (TupleTypeSyntax)greenNode;
			}
			TupleTypeSyntax tupleTypeSyntax = new TupleTypeSyntax(SyntaxKind.TupleType, openParenToken, elements.Node, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(tupleTypeSyntax, hash);
			}
			return tupleTypeSyntax;
		}

		internal static TypedTupleElementSyntax TypedTupleElement(TypeSyntax type)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(790, type, out hash);
			if (greenNode != null)
			{
				return (TypedTupleElementSyntax)greenNode;
			}
			TypedTupleElementSyntax typedTupleElementSyntax = new TypedTupleElementSyntax(SyntaxKind.TypedTupleElement, type);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(typedTupleElementSyntax, hash);
			}
			return typedTupleElementSyntax;
		}

		internal static NamedTupleElementSyntax NamedTupleElement(IdentifierTokenSyntax identifier, SimpleAsClauseSyntax asClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(791, identifier, asClause, out hash);
			if (greenNode != null)
			{
				return (NamedTupleElementSyntax)greenNode;
			}
			NamedTupleElementSyntax namedTupleElementSyntax = new NamedTupleElementSyntax(SyntaxKind.NamedTupleElement, identifier, asClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(namedTupleElementSyntax, hash);
			}
			return namedTupleElementSyntax;
		}

		internal static MeExpressionSyntax MeExpression(KeywordSyntax keyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(282, keyword, out hash);
			if (greenNode != null)
			{
				return (MeExpressionSyntax)greenNode;
			}
			MeExpressionSyntax meExpressionSyntax = new MeExpressionSyntax(SyntaxKind.MeExpression, keyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(meExpressionSyntax, hash);
			}
			return meExpressionSyntax;
		}

		internal static MyBaseExpressionSyntax MyBaseExpression(KeywordSyntax keyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(283, keyword, out hash);
			if (greenNode != null)
			{
				return (MyBaseExpressionSyntax)greenNode;
			}
			MyBaseExpressionSyntax myBaseExpressionSyntax = new MyBaseExpressionSyntax(SyntaxKind.MyBaseExpression, keyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(myBaseExpressionSyntax, hash);
			}
			return myBaseExpressionSyntax;
		}

		internal static MyClassExpressionSyntax MyClassExpression(KeywordSyntax keyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(284, keyword, out hash);
			if (greenNode != null)
			{
				return (MyClassExpressionSyntax)greenNode;
			}
			MyClassExpressionSyntax myClassExpressionSyntax = new MyClassExpressionSyntax(SyntaxKind.MyClassExpression, keyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(myClassExpressionSyntax, hash);
			}
			return myClassExpressionSyntax;
		}

		internal static GetTypeExpressionSyntax GetTypeExpression(KeywordSyntax getTypeKeyword, PunctuationSyntax openParenToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new GetTypeExpressionSyntax(SyntaxKind.GetTypeExpression, getTypeKeyword, openParenToken, type, closeParenToken);
		}

		internal static TypeOfExpressionSyntax TypeOfIsExpression(KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
		{
			return new TypeOfExpressionSyntax(SyntaxKind.TypeOfIsExpression, typeOfKeyword, expression, operatorToken, type);
		}

		internal static TypeOfExpressionSyntax TypeOfIsNotExpression(KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
		{
			return new TypeOfExpressionSyntax(SyntaxKind.TypeOfIsNotExpression, typeOfKeyword, expression, operatorToken, type);
		}

		internal static TypeOfExpressionSyntax TypeOfExpression(SyntaxKind kind, KeywordSyntax typeOfKeyword, ExpressionSyntax expression, KeywordSyntax operatorToken, TypeSyntax type)
		{
			return new TypeOfExpressionSyntax(kind, typeOfKeyword, expression, operatorToken, type);
		}

		internal static GetXmlNamespaceExpressionSyntax GetXmlNamespaceExpression(KeywordSyntax getXmlNamespaceKeyword, PunctuationSyntax openParenToken, XmlPrefixNameSyntax name, PunctuationSyntax closeParenToken)
		{
			return new GetXmlNamespaceExpressionSyntax(SyntaxKind.GetXmlNamespaceExpression, getXmlNamespaceKeyword, openParenToken, name, closeParenToken);
		}

		internal static MemberAccessExpressionSyntax SimpleMemberAccessExpression(ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(291, expression, operatorToken, name, out hash);
			if (greenNode != null)
			{
				return (MemberAccessExpressionSyntax)greenNode;
			}
			MemberAccessExpressionSyntax memberAccessExpressionSyntax = new MemberAccessExpressionSyntax(SyntaxKind.SimpleMemberAccessExpression, expression, operatorToken, name);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(memberAccessExpressionSyntax, hash);
			}
			return memberAccessExpressionSyntax;
		}

		internal static MemberAccessExpressionSyntax DictionaryAccessExpression(ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(292, expression, operatorToken, name, out hash);
			if (greenNode != null)
			{
				return (MemberAccessExpressionSyntax)greenNode;
			}
			MemberAccessExpressionSyntax memberAccessExpressionSyntax = new MemberAccessExpressionSyntax(SyntaxKind.DictionaryAccessExpression, expression, operatorToken, name);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(memberAccessExpressionSyntax, hash);
			}
			return memberAccessExpressionSyntax;
		}

		internal static MemberAccessExpressionSyntax MemberAccessExpression(SyntaxKind kind, ExpressionSyntax expression, PunctuationSyntax operatorToken, SimpleNameSyntax name)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, expression, operatorToken, name, out hash);
			if (greenNode != null)
			{
				return (MemberAccessExpressionSyntax)greenNode;
			}
			MemberAccessExpressionSyntax memberAccessExpressionSyntax = new MemberAccessExpressionSyntax(kind, expression, operatorToken, name);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(memberAccessExpressionSyntax, hash);
			}
			return memberAccessExpressionSyntax;
		}

		internal static XmlMemberAccessExpressionSyntax XmlElementAccessExpression(ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(SyntaxKind.XmlElementAccessExpression, @base, token1, token2, token3, name);
		}

		internal static XmlMemberAccessExpressionSyntax XmlDescendantAccessExpression(ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(SyntaxKind.XmlDescendantAccessExpression, @base, token1, token2, token3, name);
		}

		internal static XmlMemberAccessExpressionSyntax XmlAttributeAccessExpression(ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(SyntaxKind.XmlAttributeAccessExpression, @base, token1, token2, token3, name);
		}

		internal static XmlMemberAccessExpressionSyntax XmlMemberAccessExpression(SyntaxKind kind, ExpressionSyntax @base, PunctuationSyntax token1, PunctuationSyntax token2, PunctuationSyntax token3, XmlNodeSyntax name)
		{
			return new XmlMemberAccessExpressionSyntax(kind, @base, token1, token2, token3, name);
		}

		internal static InvocationExpressionSyntax InvocationExpression(ExpressionSyntax expression, ArgumentListSyntax argumentList)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(296, expression, argumentList, out hash);
			if (greenNode != null)
			{
				return (InvocationExpressionSyntax)greenNode;
			}
			InvocationExpressionSyntax invocationExpressionSyntax = new InvocationExpressionSyntax(SyntaxKind.InvocationExpression, expression, argumentList);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(invocationExpressionSyntax, hash);
			}
			return invocationExpressionSyntax;
		}

		internal static ObjectCreationExpressionSyntax ObjectCreationExpression(KeywordSyntax newKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, TypeSyntax type, ArgumentListSyntax argumentList, ObjectCreationInitializerSyntax initializer)
		{
			return new ObjectCreationExpressionSyntax(SyntaxKind.ObjectCreationExpression, newKeyword, attributeLists.Node, type, argumentList, initializer);
		}

		internal static AnonymousObjectCreationExpressionSyntax AnonymousObjectCreationExpression(KeywordSyntax newKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, ObjectMemberInitializerSyntax initializer)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(298, newKeyword, attributeLists.Node, initializer, out hash);
			if (greenNode != null)
			{
				return (AnonymousObjectCreationExpressionSyntax)greenNode;
			}
			AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpressionSyntax = new AnonymousObjectCreationExpressionSyntax(SyntaxKind.AnonymousObjectCreationExpression, newKeyword, attributeLists.Node, initializer);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(anonymousObjectCreationExpressionSyntax, hash);
			}
			return anonymousObjectCreationExpressionSyntax;
		}

		internal static ArrayCreationExpressionSyntax ArrayCreationExpression(KeywordSyntax newKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, TypeSyntax type, ArgumentListSyntax arrayBounds, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> rankSpecifiers, CollectionInitializerSyntax initializer)
		{
			return new ArrayCreationExpressionSyntax(SyntaxKind.ArrayCreationExpression, newKeyword, attributeLists.Node, type, arrayBounds, rankSpecifiers.Node, initializer);
		}

		internal static CollectionInitializerSyntax CollectionInitializer(PunctuationSyntax openBraceToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> initializers, PunctuationSyntax closeBraceToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(302, openBraceToken, initializers.Node, closeBraceToken, out hash);
			if (greenNode != null)
			{
				return (CollectionInitializerSyntax)greenNode;
			}
			CollectionInitializerSyntax collectionInitializerSyntax = new CollectionInitializerSyntax(SyntaxKind.CollectionInitializer, openBraceToken, initializers.Node, closeBraceToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(collectionInitializerSyntax, hash);
			}
			return collectionInitializerSyntax;
		}

		internal static CTypeExpressionSyntax CTypeExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new CTypeExpressionSyntax(SyntaxKind.CTypeExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken);
		}

		internal static DirectCastExpressionSyntax DirectCastExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new DirectCastExpressionSyntax(SyntaxKind.DirectCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken);
		}

		internal static TryCastExpressionSyntax TryCastExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax commaToken, TypeSyntax type, PunctuationSyntax closeParenToken)
		{
			return new TryCastExpressionSyntax(SyntaxKind.TryCastExpression, keyword, openParenToken, expression, commaToken, type, closeParenToken);
		}

		internal static PredefinedCastExpressionSyntax PredefinedCastExpression(KeywordSyntax keyword, PunctuationSyntax openParenToken, ExpressionSyntax expression, PunctuationSyntax closeParenToken)
		{
			return new PredefinedCastExpressionSyntax(SyntaxKind.PredefinedCastExpression, keyword, openParenToken, expression, closeParenToken);
		}

		internal static BinaryExpressionSyntax AddExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(307, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.AddExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax SubtractExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(308, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.SubtractExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax MultiplyExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(309, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.MultiplyExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax DivideExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(310, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.DivideExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax IntegerDivideExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(311, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.IntegerDivideExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax ExponentiateExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(314, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ExponentiateExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax LeftShiftExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(315, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LeftShiftExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax RightShiftExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(316, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.RightShiftExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax ConcatenateExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(317, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ConcatenateExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax ModuloExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(318, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ModuloExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax EqualsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(319, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.EqualsExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax NotEqualsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(320, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.NotEqualsExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax LessThanExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(321, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LessThanExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax LessThanOrEqualExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(322, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LessThanOrEqualExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax GreaterThanOrEqualExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(323, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.GreaterThanOrEqualExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax GreaterThanExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(324, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.GreaterThanExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax IsExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(325, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.IsExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax IsNotExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(326, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.IsNotExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax LikeExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(327, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.LikeExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax OrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(328, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.OrExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax ExclusiveOrExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(329, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.ExclusiveOrExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax AndExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(330, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.AndExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax OrElseExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(331, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.OrElseExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax AndAlsoExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(332, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(SyntaxKind.AndAlsoExpression, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static BinaryExpressionSyntax BinaryExpression(SyntaxKind kind, ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, left, operatorToken, right, out hash);
			if (greenNode != null)
			{
				return (BinaryExpressionSyntax)greenNode;
			}
			BinaryExpressionSyntax binaryExpressionSyntax = new BinaryExpressionSyntax(kind, left, operatorToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(binaryExpressionSyntax, hash);
			}
			return binaryExpressionSyntax;
		}

		internal static UnaryExpressionSyntax UnaryPlusExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(333, operatorToken, operand, out hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.UnaryPlusExpression, operatorToken, operand);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal static UnaryExpressionSyntax UnaryMinusExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(334, operatorToken, operand, out hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.UnaryMinusExpression, operatorToken, operand);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal static UnaryExpressionSyntax NotExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(335, operatorToken, operand, out hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.NotExpression, operatorToken, operand);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal static UnaryExpressionSyntax AddressOfExpression(SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(336, operatorToken, operand, out hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(SyntaxKind.AddressOfExpression, operatorToken, operand);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal static UnaryExpressionSyntax UnaryExpression(SyntaxKind kind, SyntaxToken operatorToken, ExpressionSyntax operand)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, operatorToken, operand, out hash);
			if (greenNode != null)
			{
				return (UnaryExpressionSyntax)greenNode;
			}
			UnaryExpressionSyntax unaryExpressionSyntax = new UnaryExpressionSyntax(kind, operatorToken, operand);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(unaryExpressionSyntax, hash);
			}
			return unaryExpressionSyntax;
		}

		internal static BinaryConditionalExpressionSyntax BinaryConditionalExpression(KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax firstExpression, PunctuationSyntax commaToken, ExpressionSyntax secondExpression, PunctuationSyntax closeParenToken)
		{
			return new BinaryConditionalExpressionSyntax(SyntaxKind.BinaryConditionalExpression, ifKeyword, openParenToken, firstExpression, commaToken, secondExpression, closeParenToken);
		}

		internal static TernaryConditionalExpressionSyntax TernaryConditionalExpression(KeywordSyntax ifKeyword, PunctuationSyntax openParenToken, ExpressionSyntax condition, PunctuationSyntax firstCommaToken, ExpressionSyntax whenTrue, PunctuationSyntax secondCommaToken, ExpressionSyntax whenFalse, PunctuationSyntax closeParenToken)
		{
			return new TernaryConditionalExpressionSyntax(SyntaxKind.TernaryConditionalExpression, ifKeyword, openParenToken, condition, firstCommaToken, whenTrue, secondCommaToken, whenFalse, closeParenToken);
		}

		internal static SingleLineLambdaExpressionSyntax SingleLineFunctionLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(339, subOrFunctionHeader, body, out hash);
			if (greenNode != null)
			{
				return (SingleLineLambdaExpressionSyntax)greenNode;
			}
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = new SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineFunctionLambdaExpression, subOrFunctionHeader, body);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax, hash);
			}
			return singleLineLambdaExpressionSyntax;
		}

		internal static SingleLineLambdaExpressionSyntax SingleLineSubLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(342, subOrFunctionHeader, body, out hash);
			if (greenNode != null)
			{
				return (SingleLineLambdaExpressionSyntax)greenNode;
			}
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = new SingleLineLambdaExpressionSyntax(SyntaxKind.SingleLineSubLambdaExpression, subOrFunctionHeader, body);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax, hash);
			}
			return singleLineLambdaExpressionSyntax;
		}

		internal static SingleLineLambdaExpressionSyntax SingleLineLambdaExpression(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, VisualBasicSyntaxNode body)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, subOrFunctionHeader, body, out hash);
			if (greenNode != null)
			{
				return (SingleLineLambdaExpressionSyntax)greenNode;
			}
			SingleLineLambdaExpressionSyntax singleLineLambdaExpressionSyntax = new SingleLineLambdaExpressionSyntax(kind, subOrFunctionHeader, body);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(singleLineLambdaExpressionSyntax, hash);
			}
			return singleLineLambdaExpressionSyntax;
		}

		internal static MultiLineLambdaExpressionSyntax MultiLineFunctionLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(343, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, out hash);
			if (greenNode != null)
			{
				return (MultiLineLambdaExpressionSyntax)greenNode;
			}
			MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = new MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineFunctionLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax, hash);
			}
			return multiLineLambdaExpressionSyntax;
		}

		internal static MultiLineLambdaExpressionSyntax MultiLineSubLambdaExpression(LambdaHeaderSyntax subOrFunctionHeader, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(344, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, out hash);
			if (greenNode != null)
			{
				return (MultiLineLambdaExpressionSyntax)greenNode;
			}
			MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = new MultiLineLambdaExpressionSyntax(SyntaxKind.MultiLineSubLambdaExpression, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax, hash);
			}
			return multiLineLambdaExpressionSyntax;
		}

		internal static MultiLineLambdaExpressionSyntax MultiLineLambdaExpression(SyntaxKind kind, LambdaHeaderSyntax subOrFunctionHeader, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> statements, EndBlockStatementSyntax endSubOrFunctionStatement)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement, out hash);
			if (greenNode != null)
			{
				return (MultiLineLambdaExpressionSyntax)greenNode;
			}
			MultiLineLambdaExpressionSyntax multiLineLambdaExpressionSyntax = new MultiLineLambdaExpressionSyntax(kind, subOrFunctionHeader, statements.Node, endSubOrFunctionStatement);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(multiLineLambdaExpressionSyntax, hash);
			}
			return multiLineLambdaExpressionSyntax;
		}

		internal static LambdaHeaderSyntax SubLambdaHeader(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new LambdaHeaderSyntax(SyntaxKind.SubLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause);
		}

		internal static LambdaHeaderSyntax FunctionLambdaHeader(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new LambdaHeaderSyntax(SyntaxKind.FunctionLambdaHeader, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause);
		}

		internal static LambdaHeaderSyntax LambdaHeader(SyntaxKind kind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributeLists, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax subOrFunctionKeyword, ParameterListSyntax parameterList, SimpleAsClauseSyntax asClause)
		{
			return new LambdaHeaderSyntax(kind, attributeLists.Node, modifiers.Node, subOrFunctionKeyword, parameterList, asClause);
		}

		internal static ArgumentListSyntax ArgumentList(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> arguments, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(347, openParenToken, arguments.Node, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (ArgumentListSyntax)greenNode;
			}
			ArgumentListSyntax argumentListSyntax = new ArgumentListSyntax(SyntaxKind.ArgumentList, openParenToken, arguments.Node, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(argumentListSyntax, hash);
			}
			return argumentListSyntax;
		}

		internal static OmittedArgumentSyntax OmittedArgument(PunctuationSyntax empty)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(348, empty, out hash);
			if (greenNode != null)
			{
				return (OmittedArgumentSyntax)greenNode;
			}
			OmittedArgumentSyntax omittedArgumentSyntax = new OmittedArgumentSyntax(SyntaxKind.OmittedArgument, empty);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(omittedArgumentSyntax, hash);
			}
			return omittedArgumentSyntax;
		}

		internal static SimpleArgumentSyntax SimpleArgument(NameColonEqualsSyntax nameColonEquals, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(349, nameColonEquals, expression, out hash);
			if (greenNode != null)
			{
				return (SimpleArgumentSyntax)greenNode;
			}
			SimpleArgumentSyntax simpleArgumentSyntax = new SimpleArgumentSyntax(SyntaxKind.SimpleArgument, nameColonEquals, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(simpleArgumentSyntax, hash);
			}
			return simpleArgumentSyntax;
		}

		internal static NameColonEqualsSyntax NameColonEquals(IdentifierNameSyntax name, PunctuationSyntax colonEqualsToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(755, name, colonEqualsToken, out hash);
			if (greenNode != null)
			{
				return (NameColonEqualsSyntax)greenNode;
			}
			NameColonEqualsSyntax nameColonEqualsSyntax = new NameColonEqualsSyntax(SyntaxKind.NameColonEquals, name, colonEqualsToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(nameColonEqualsSyntax, hash);
			}
			return nameColonEqualsSyntax;
		}

		internal static RangeArgumentSyntax RangeArgument(ExpressionSyntax lowerBound, KeywordSyntax toKeyword, ExpressionSyntax upperBound)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(351, lowerBound, toKeyword, upperBound, out hash);
			if (greenNode != null)
			{
				return (RangeArgumentSyntax)greenNode;
			}
			RangeArgumentSyntax rangeArgumentSyntax = new RangeArgumentSyntax(SyntaxKind.RangeArgument, lowerBound, toKeyword, upperBound);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(rangeArgumentSyntax, hash);
			}
			return rangeArgumentSyntax;
		}

		internal static QueryExpressionSyntax QueryExpression(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> clauses)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(352, clauses.Node, out hash);
			if (greenNode != null)
			{
				return (QueryExpressionSyntax)greenNode;
			}
			QueryExpressionSyntax queryExpressionSyntax = new QueryExpressionSyntax(SyntaxKind.QueryExpression, clauses.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(queryExpressionSyntax, hash);
			}
			return queryExpressionSyntax;
		}

		internal static CollectionRangeVariableSyntax CollectionRangeVariable(ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, KeywordSyntax inKeyword, ExpressionSyntax expression)
		{
			return new CollectionRangeVariableSyntax(SyntaxKind.CollectionRangeVariable, identifier, asClause, inKeyword, expression);
		}

		internal static ExpressionRangeVariableSyntax ExpressionRangeVariable(VariableNameEqualsSyntax nameEquals, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(354, nameEquals, expression, out hash);
			if (greenNode != null)
			{
				return (ExpressionRangeVariableSyntax)greenNode;
			}
			ExpressionRangeVariableSyntax expressionRangeVariableSyntax = new ExpressionRangeVariableSyntax(SyntaxKind.ExpressionRangeVariable, nameEquals, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(expressionRangeVariableSyntax, hash);
			}
			return expressionRangeVariableSyntax;
		}

		internal static AggregationRangeVariableSyntax AggregationRangeVariable(VariableNameEqualsSyntax nameEquals, AggregationSyntax aggregation)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(355, nameEquals, aggregation, out hash);
			if (greenNode != null)
			{
				return (AggregationRangeVariableSyntax)greenNode;
			}
			AggregationRangeVariableSyntax aggregationRangeVariableSyntax = new AggregationRangeVariableSyntax(SyntaxKind.AggregationRangeVariable, nameEquals, aggregation);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(aggregationRangeVariableSyntax, hash);
			}
			return aggregationRangeVariableSyntax;
		}

		internal static VariableNameEqualsSyntax VariableNameEquals(ModifiedIdentifierSyntax identifier, SimpleAsClauseSyntax asClause, PunctuationSyntax equalsToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(356, identifier, asClause, equalsToken, out hash);
			if (greenNode != null)
			{
				return (VariableNameEqualsSyntax)greenNode;
			}
			VariableNameEqualsSyntax variableNameEqualsSyntax = new VariableNameEqualsSyntax(SyntaxKind.VariableNameEquals, identifier, asClause, equalsToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(variableNameEqualsSyntax, hash);
			}
			return variableNameEqualsSyntax;
		}

		internal static FunctionAggregationSyntax FunctionAggregation(IdentifierTokenSyntax functionName, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
		{
			return new FunctionAggregationSyntax(SyntaxKind.FunctionAggregation, functionName, openParenToken, argument, closeParenToken);
		}

		internal static GroupAggregationSyntax GroupAggregation(KeywordSyntax groupKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(358, groupKeyword, out hash);
			if (greenNode != null)
			{
				return (GroupAggregationSyntax)greenNode;
			}
			GroupAggregationSyntax groupAggregationSyntax = new GroupAggregationSyntax(SyntaxKind.GroupAggregation, groupKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(groupAggregationSyntax, hash);
			}
			return groupAggregationSyntax;
		}

		internal static FromClauseSyntax FromClause(KeywordSyntax fromKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(359, fromKeyword, variables.Node, out hash);
			if (greenNode != null)
			{
				return (FromClauseSyntax)greenNode;
			}
			FromClauseSyntax fromClauseSyntax = new FromClauseSyntax(SyntaxKind.FromClause, fromKeyword, variables.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(fromClauseSyntax, hash);
			}
			return fromClauseSyntax;
		}

		internal static LetClauseSyntax LetClause(KeywordSyntax letKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(360, letKeyword, variables.Node, out hash);
			if (greenNode != null)
			{
				return (LetClauseSyntax)greenNode;
			}
			LetClauseSyntax letClauseSyntax = new LetClauseSyntax(SyntaxKind.LetClause, letKeyword, variables.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(letClauseSyntax, hash);
			}
			return letClauseSyntax;
		}

		internal static AggregateClauseSyntax AggregateClause(KeywordSyntax aggregateKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> additionalQueryOperators, KeywordSyntax intoKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> aggregationVariables)
		{
			return new AggregateClauseSyntax(SyntaxKind.AggregateClause, aggregateKeyword, variables.Node, additionalQueryOperators.Node, intoKeyword, aggregationVariables.Node);
		}

		internal static DistinctClauseSyntax DistinctClause(KeywordSyntax distinctKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(362, distinctKeyword, out hash);
			if (greenNode != null)
			{
				return (DistinctClauseSyntax)greenNode;
			}
			DistinctClauseSyntax distinctClauseSyntax = new DistinctClauseSyntax(SyntaxKind.DistinctClause, distinctKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(distinctClauseSyntax, hash);
			}
			return distinctClauseSyntax;
		}

		internal static WhereClauseSyntax WhereClause(KeywordSyntax whereKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(363, whereKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (WhereClauseSyntax)greenNode;
			}
			WhereClauseSyntax whereClauseSyntax = new WhereClauseSyntax(SyntaxKind.WhereClause, whereKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(whereClauseSyntax, hash);
			}
			return whereClauseSyntax;
		}

		internal static PartitionWhileClauseSyntax SkipWhileClause(KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(364, skipOrTakeKeyword, whileKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (PartitionWhileClauseSyntax)greenNode;
			}
			PartitionWhileClauseSyntax partitionWhileClauseSyntax = new PartitionWhileClauseSyntax(SyntaxKind.SkipWhileClause, skipOrTakeKeyword, whileKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionWhileClauseSyntax, hash);
			}
			return partitionWhileClauseSyntax;
		}

		internal static PartitionWhileClauseSyntax TakeWhileClause(KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(365, skipOrTakeKeyword, whileKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (PartitionWhileClauseSyntax)greenNode;
			}
			PartitionWhileClauseSyntax partitionWhileClauseSyntax = new PartitionWhileClauseSyntax(SyntaxKind.TakeWhileClause, skipOrTakeKeyword, whileKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionWhileClauseSyntax, hash);
			}
			return partitionWhileClauseSyntax;
		}

		internal static PartitionWhileClauseSyntax PartitionWhileClause(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, KeywordSyntax whileKeyword, ExpressionSyntax condition)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, skipOrTakeKeyword, whileKeyword, condition, out hash);
			if (greenNode != null)
			{
				return (PartitionWhileClauseSyntax)greenNode;
			}
			PartitionWhileClauseSyntax partitionWhileClauseSyntax = new PartitionWhileClauseSyntax(kind, skipOrTakeKeyword, whileKeyword, condition);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionWhileClauseSyntax, hash);
			}
			return partitionWhileClauseSyntax;
		}

		internal static PartitionClauseSyntax SkipClause(KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(366, skipOrTakeKeyword, count, out hash);
			if (greenNode != null)
			{
				return (PartitionClauseSyntax)greenNode;
			}
			PartitionClauseSyntax partitionClauseSyntax = new PartitionClauseSyntax(SyntaxKind.SkipClause, skipOrTakeKeyword, count);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionClauseSyntax, hash);
			}
			return partitionClauseSyntax;
		}

		internal static PartitionClauseSyntax TakeClause(KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(367, skipOrTakeKeyword, count, out hash);
			if (greenNode != null)
			{
				return (PartitionClauseSyntax)greenNode;
			}
			PartitionClauseSyntax partitionClauseSyntax = new PartitionClauseSyntax(SyntaxKind.TakeClause, skipOrTakeKeyword, count);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionClauseSyntax, hash);
			}
			return partitionClauseSyntax;
		}

		internal static PartitionClauseSyntax PartitionClause(SyntaxKind kind, KeywordSyntax skipOrTakeKeyword, ExpressionSyntax count)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, skipOrTakeKeyword, count, out hash);
			if (greenNode != null)
			{
				return (PartitionClauseSyntax)greenNode;
			}
			PartitionClauseSyntax partitionClauseSyntax = new PartitionClauseSyntax(kind, skipOrTakeKeyword, count);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(partitionClauseSyntax, hash);
			}
			return partitionClauseSyntax;
		}

		internal static GroupByClauseSyntax GroupByClause(KeywordSyntax groupKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> items, KeywordSyntax byKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> keys, KeywordSyntax intoKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> aggregationVariables)
		{
			return new GroupByClauseSyntax(SyntaxKind.GroupByClause, groupKeyword, items.Node, byKeyword, keys.Node, intoKeyword, aggregationVariables.Node);
		}

		internal static JoinConditionSyntax JoinCondition(ExpressionSyntax left, KeywordSyntax equalsKeyword, ExpressionSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(369, left, equalsKeyword, right, out hash);
			if (greenNode != null)
			{
				return (JoinConditionSyntax)greenNode;
			}
			JoinConditionSyntax joinConditionSyntax = new JoinConditionSyntax(SyntaxKind.JoinCondition, left, equalsKeyword, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(joinConditionSyntax, hash);
			}
			return joinConditionSyntax;
		}

		internal static SimpleJoinClauseSyntax SimpleJoinClause(KeywordSyntax joinKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinedVariables, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> additionalJoins, KeywordSyntax onKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinConditions)
		{
			return new SimpleJoinClauseSyntax(SyntaxKind.SimpleJoinClause, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node);
		}

		internal static GroupJoinClauseSyntax GroupJoinClause(KeywordSyntax groupKeyword, KeywordSyntax joinKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinedVariables, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> additionalJoins, KeywordSyntax onKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> joinConditions, KeywordSyntax intoKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> aggregationVariables)
		{
			return new GroupJoinClauseSyntax(SyntaxKind.GroupJoinClause, groupKeyword, joinKeyword, joinedVariables.Node, additionalJoins.Node, onKeyword, joinConditions.Node, intoKeyword, aggregationVariables.Node);
		}

		internal static OrderByClauseSyntax OrderByClause(KeywordSyntax orderKeyword, KeywordSyntax byKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> orderings)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(372, orderKeyword, byKeyword, orderings.Node, out hash);
			if (greenNode != null)
			{
				return (OrderByClauseSyntax)greenNode;
			}
			OrderByClauseSyntax orderByClauseSyntax = new OrderByClauseSyntax(SyntaxKind.OrderByClause, orderKeyword, byKeyword, orderings.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderByClauseSyntax, hash);
			}
			return orderByClauseSyntax;
		}

		internal static OrderingSyntax AscendingOrdering(ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(375, expression, ascendingOrDescendingKeyword, out hash);
			if (greenNode != null)
			{
				return (OrderingSyntax)greenNode;
			}
			OrderingSyntax orderingSyntax = new OrderingSyntax(SyntaxKind.AscendingOrdering, expression, ascendingOrDescendingKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderingSyntax, hash);
			}
			return orderingSyntax;
		}

		internal static OrderingSyntax DescendingOrdering(ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(376, expression, ascendingOrDescendingKeyword, out hash);
			if (greenNode != null)
			{
				return (OrderingSyntax)greenNode;
			}
			OrderingSyntax orderingSyntax = new OrderingSyntax(SyntaxKind.DescendingOrdering, expression, ascendingOrDescendingKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderingSyntax, hash);
			}
			return orderingSyntax;
		}

		internal static OrderingSyntax Ordering(SyntaxKind kind, ExpressionSyntax expression, KeywordSyntax ascendingOrDescendingKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode((int)kind, expression, ascendingOrDescendingKeyword, out hash);
			if (greenNode != null)
			{
				return (OrderingSyntax)greenNode;
			}
			OrderingSyntax orderingSyntax = new OrderingSyntax(kind, expression, ascendingOrDescendingKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(orderingSyntax, hash);
			}
			return orderingSyntax;
		}

		internal static SelectClauseSyntax SelectClause(KeywordSyntax selectKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> variables)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(377, selectKeyword, variables.Node, out hash);
			if (greenNode != null)
			{
				return (SelectClauseSyntax)greenNode;
			}
			SelectClauseSyntax selectClauseSyntax = new SelectClauseSyntax(SyntaxKind.SelectClause, selectKeyword, variables.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(selectClauseSyntax, hash);
			}
			return selectClauseSyntax;
		}

		internal static XmlDocumentSyntax XmlDocument(XmlDeclarationSyntax declaration, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> precedingMisc, XmlNodeSyntax root, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> followingMisc)
		{
			return new XmlDocumentSyntax(SyntaxKind.XmlDocument, declaration, precedingMisc.Node, root, followingMisc.Node);
		}

		internal static XmlDeclarationSyntax XmlDeclaration(PunctuationSyntax lessThanQuestionToken, KeywordSyntax xmlKeyword, XmlDeclarationOptionSyntax version, XmlDeclarationOptionSyntax encoding, XmlDeclarationOptionSyntax standalone, PunctuationSyntax questionGreaterThanToken)
		{
			return new XmlDeclarationSyntax(SyntaxKind.XmlDeclaration, lessThanQuestionToken, xmlKeyword, version, encoding, standalone, questionGreaterThanToken);
		}

		internal static XmlDeclarationOptionSyntax XmlDeclarationOption(XmlNameTokenSyntax name, PunctuationSyntax equals, XmlStringSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(380, name, equals, value, out hash);
			if (greenNode != null)
			{
				return (XmlDeclarationOptionSyntax)greenNode;
			}
			XmlDeclarationOptionSyntax xmlDeclarationOptionSyntax = new XmlDeclarationOptionSyntax(SyntaxKind.XmlDeclarationOption, name, equals, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlDeclarationOptionSyntax, hash);
			}
			return xmlDeclarationOptionSyntax;
		}

		internal static XmlElementSyntax XmlElement(XmlElementStartTagSyntax startTag, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> content, XmlElementEndTagSyntax endTag)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(381, startTag, content.Node, endTag, out hash);
			if (greenNode != null)
			{
				return (XmlElementSyntax)greenNode;
			}
			XmlElementSyntax xmlElementSyntax = new XmlElementSyntax(SyntaxKind.XmlElement, startTag, content.Node, endTag);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlElementSyntax, hash);
			}
			return xmlElementSyntax;
		}

		internal static XmlTextSyntax XmlText(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(382, textTokens.Node, out hash);
			if (greenNode != null)
			{
				return (XmlTextSyntax)greenNode;
			}
			XmlTextSyntax xmlTextSyntax = new XmlTextSyntax(SyntaxKind.XmlText, textTokens.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlTextSyntax, hash);
			}
			return xmlTextSyntax;
		}

		internal static XmlElementStartTagSyntax XmlElementStartTag(PunctuationSyntax lessThanToken, XmlNodeSyntax name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, PunctuationSyntax greaterThanToken)
		{
			return new XmlElementStartTagSyntax(SyntaxKind.XmlElementStartTag, lessThanToken, name, attributes.Node, greaterThanToken);
		}

		internal static XmlElementEndTagSyntax XmlElementEndTag(PunctuationSyntax lessThanSlashToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(384, lessThanSlashToken, name, greaterThanToken, out hash);
			if (greenNode != null)
			{
				return (XmlElementEndTagSyntax)greenNode;
			}
			XmlElementEndTagSyntax xmlElementEndTagSyntax = new XmlElementEndTagSyntax(SyntaxKind.XmlElementEndTag, lessThanSlashToken, name, greaterThanToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlElementEndTagSyntax, hash);
			}
			return xmlElementEndTagSyntax;
		}

		internal static XmlEmptyElementSyntax XmlEmptyElement(PunctuationSyntax lessThanToken, XmlNodeSyntax name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, PunctuationSyntax slashGreaterThanToken)
		{
			return new XmlEmptyElementSyntax(SyntaxKind.XmlEmptyElement, lessThanToken, name, attributes.Node, slashGreaterThanToken);
		}

		internal static XmlAttributeSyntax XmlAttribute(XmlNodeSyntax name, PunctuationSyntax equalsToken, XmlNodeSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(386, name, equalsToken, value, out hash);
			if (greenNode != null)
			{
				return (XmlAttributeSyntax)greenNode;
			}
			XmlAttributeSyntax xmlAttributeSyntax = new XmlAttributeSyntax(SyntaxKind.XmlAttribute, name, equalsToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlAttributeSyntax, hash);
			}
			return xmlAttributeSyntax;
		}

		internal static XmlStringSyntax XmlString(PunctuationSyntax startQuoteToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax endQuoteToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(387, startQuoteToken, textTokens.Node, endQuoteToken, out hash);
			if (greenNode != null)
			{
				return (XmlStringSyntax)greenNode;
			}
			XmlStringSyntax xmlStringSyntax = new XmlStringSyntax(SyntaxKind.XmlString, startQuoteToken, textTokens.Node, endQuoteToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlStringSyntax, hash);
			}
			return xmlStringSyntax;
		}

		internal static XmlPrefixNameSyntax XmlPrefixName(XmlNameTokenSyntax name)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(388, name, out hash);
			if (greenNode != null)
			{
				return (XmlPrefixNameSyntax)greenNode;
			}
			XmlPrefixNameSyntax xmlPrefixNameSyntax = new XmlPrefixNameSyntax(SyntaxKind.XmlPrefixName, name);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlPrefixNameSyntax, hash);
			}
			return xmlPrefixNameSyntax;
		}

		internal static XmlNameSyntax XmlName(XmlPrefixSyntax prefix, XmlNameTokenSyntax localName)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(389, prefix, localName, out hash);
			if (greenNode != null)
			{
				return (XmlNameSyntax)greenNode;
			}
			XmlNameSyntax xmlNameSyntax = new XmlNameSyntax(SyntaxKind.XmlName, prefix, localName);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlNameSyntax, hash);
			}
			return xmlNameSyntax;
		}

		internal static XmlBracketedNameSyntax XmlBracketedName(PunctuationSyntax lessThanToken, XmlNameSyntax name, PunctuationSyntax greaterThanToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(390, lessThanToken, name, greaterThanToken, out hash);
			if (greenNode != null)
			{
				return (XmlBracketedNameSyntax)greenNode;
			}
			XmlBracketedNameSyntax xmlBracketedNameSyntax = new XmlBracketedNameSyntax(SyntaxKind.XmlBracketedName, lessThanToken, name, greaterThanToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlBracketedNameSyntax, hash);
			}
			return xmlBracketedNameSyntax;
		}

		internal static XmlPrefixSyntax XmlPrefix(XmlNameTokenSyntax name, PunctuationSyntax colonToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(391, name, colonToken, out hash);
			if (greenNode != null)
			{
				return (XmlPrefixSyntax)greenNode;
			}
			XmlPrefixSyntax xmlPrefixSyntax = new XmlPrefixSyntax(SyntaxKind.XmlPrefix, name, colonToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlPrefixSyntax, hash);
			}
			return xmlPrefixSyntax;
		}

		internal static XmlCommentSyntax XmlComment(PunctuationSyntax lessThanExclamationMinusMinusToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax minusMinusGreaterThanToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(392, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken, out hash);
			if (greenNode != null)
			{
				return (XmlCommentSyntax)greenNode;
			}
			XmlCommentSyntax xmlCommentSyntax = new XmlCommentSyntax(SyntaxKind.XmlComment, lessThanExclamationMinusMinusToken, textTokens.Node, minusMinusGreaterThanToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlCommentSyntax, hash);
			}
			return xmlCommentSyntax;
		}

		internal static XmlProcessingInstructionSyntax XmlProcessingInstruction(PunctuationSyntax lessThanQuestionToken, XmlNameTokenSyntax name, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax questionGreaterThanToken)
		{
			return new XmlProcessingInstructionSyntax(SyntaxKind.XmlProcessingInstruction, lessThanQuestionToken, name, textTokens.Node, questionGreaterThanToken);
		}

		internal static XmlCDataSectionSyntax XmlCDataSection(PunctuationSyntax beginCDataToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> textTokens, PunctuationSyntax endCDataToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(394, beginCDataToken, textTokens.Node, endCDataToken, out hash);
			if (greenNode != null)
			{
				return (XmlCDataSectionSyntax)greenNode;
			}
			XmlCDataSectionSyntax xmlCDataSectionSyntax = new XmlCDataSectionSyntax(SyntaxKind.XmlCDataSection, beginCDataToken, textTokens.Node, endCDataToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlCDataSectionSyntax, hash);
			}
			return xmlCDataSectionSyntax;
		}

		internal static XmlEmbeddedExpressionSyntax XmlEmbeddedExpression(PunctuationSyntax lessThanPercentEqualsToken, ExpressionSyntax expression, PunctuationSyntax percentGreaterThanToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(395, lessThanPercentEqualsToken, expression, percentGreaterThanToken, out hash);
			if (greenNode != null)
			{
				return (XmlEmbeddedExpressionSyntax)greenNode;
			}
			XmlEmbeddedExpressionSyntax xmlEmbeddedExpressionSyntax = new XmlEmbeddedExpressionSyntax(SyntaxKind.XmlEmbeddedExpression, lessThanPercentEqualsToken, expression, percentGreaterThanToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(xmlEmbeddedExpressionSyntax, hash);
			}
			return xmlEmbeddedExpressionSyntax;
		}

		internal static ArrayTypeSyntax ArrayType(TypeSyntax elementType, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> rankSpecifiers)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(396, elementType, rankSpecifiers.Node, out hash);
			if (greenNode != null)
			{
				return (ArrayTypeSyntax)greenNode;
			}
			ArrayTypeSyntax arrayTypeSyntax = new ArrayTypeSyntax(SyntaxKind.ArrayType, elementType, rankSpecifiers.Node);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(arrayTypeSyntax, hash);
			}
			return arrayTypeSyntax;
		}

		internal static NullableTypeSyntax NullableType(TypeSyntax elementType, PunctuationSyntax questionMarkToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(397, elementType, questionMarkToken, out hash);
			if (greenNode != null)
			{
				return (NullableTypeSyntax)greenNode;
			}
			NullableTypeSyntax nullableTypeSyntax = new NullableTypeSyntax(SyntaxKind.NullableType, elementType, questionMarkToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(nullableTypeSyntax, hash);
			}
			return nullableTypeSyntax;
		}

		internal static PredefinedTypeSyntax PredefinedType(KeywordSyntax keyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(398, keyword, out hash);
			if (greenNode != null)
			{
				return (PredefinedTypeSyntax)greenNode;
			}
			PredefinedTypeSyntax predefinedTypeSyntax = new PredefinedTypeSyntax(SyntaxKind.PredefinedType, keyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(predefinedTypeSyntax, hash);
			}
			return predefinedTypeSyntax;
		}

		internal static IdentifierNameSyntax IdentifierName(IdentifierTokenSyntax identifier)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(399, identifier, out hash);
			if (greenNode != null)
			{
				return (IdentifierNameSyntax)greenNode;
			}
			IdentifierNameSyntax identifierNameSyntax = new IdentifierNameSyntax(SyntaxKind.IdentifierName, identifier);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(identifierNameSyntax, hash);
			}
			return identifierNameSyntax;
		}

		internal static GenericNameSyntax GenericName(IdentifierTokenSyntax identifier, TypeArgumentListSyntax typeArgumentList)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(400, identifier, typeArgumentList, out hash);
			if (greenNode != null)
			{
				return (GenericNameSyntax)greenNode;
			}
			GenericNameSyntax genericNameSyntax = new GenericNameSyntax(SyntaxKind.GenericName, identifier, typeArgumentList);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(genericNameSyntax, hash);
			}
			return genericNameSyntax;
		}

		internal static QualifiedNameSyntax QualifiedName(NameSyntax left, PunctuationSyntax dotToken, SimpleNameSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(401, left, dotToken, right, out hash);
			if (greenNode != null)
			{
				return (QualifiedNameSyntax)greenNode;
			}
			QualifiedNameSyntax qualifiedNameSyntax = new QualifiedNameSyntax(SyntaxKind.QualifiedName, left, dotToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(qualifiedNameSyntax, hash);
			}
			return qualifiedNameSyntax;
		}

		internal static GlobalNameSyntax GlobalName(KeywordSyntax globalKeyword)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(402, globalKeyword, out hash);
			if (greenNode != null)
			{
				return (GlobalNameSyntax)greenNode;
			}
			GlobalNameSyntax globalNameSyntax = new GlobalNameSyntax(SyntaxKind.GlobalName, globalKeyword);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(globalNameSyntax, hash);
			}
			return globalNameSyntax;
		}

		internal static TypeArgumentListSyntax TypeArgumentList(PunctuationSyntax openParenToken, KeywordSyntax ofKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> arguments, PunctuationSyntax closeParenToken)
		{
			return new TypeArgumentListSyntax(SyntaxKind.TypeArgumentList, openParenToken, ofKeyword, arguments.Node, closeParenToken);
		}

		internal static CrefReferenceSyntax CrefReference(TypeSyntax name, CrefSignatureSyntax signature, SimpleAsClauseSyntax asClause)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(404, name, signature, asClause, out hash);
			if (greenNode != null)
			{
				return (CrefReferenceSyntax)greenNode;
			}
			CrefReferenceSyntax crefReferenceSyntax = new CrefReferenceSyntax(SyntaxKind.CrefReference, name, signature, asClause);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefReferenceSyntax, hash);
			}
			return crefReferenceSyntax;
		}

		internal static CrefSignatureSyntax CrefSignature(PunctuationSyntax openParenToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> argumentTypes, PunctuationSyntax closeParenToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(407, openParenToken, argumentTypes.Node, closeParenToken, out hash);
			if (greenNode != null)
			{
				return (CrefSignatureSyntax)greenNode;
			}
			CrefSignatureSyntax crefSignatureSyntax = new CrefSignatureSyntax(SyntaxKind.CrefSignature, openParenToken, argumentTypes.Node, closeParenToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefSignatureSyntax, hash);
			}
			return crefSignatureSyntax;
		}

		internal static CrefSignaturePartSyntax CrefSignaturePart(KeywordSyntax modifier, TypeSyntax type)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(408, modifier, type, out hash);
			if (greenNode != null)
			{
				return (CrefSignaturePartSyntax)greenNode;
			}
			CrefSignaturePartSyntax crefSignaturePartSyntax = new CrefSignaturePartSyntax(SyntaxKind.CrefSignaturePart, modifier, type);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefSignaturePartSyntax, hash);
			}
			return crefSignaturePartSyntax;
		}

		internal static CrefOperatorReferenceSyntax CrefOperatorReference(KeywordSyntax operatorKeyword, SyntaxToken operatorToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(409, operatorKeyword, operatorToken, out hash);
			if (greenNode != null)
			{
				return (CrefOperatorReferenceSyntax)greenNode;
			}
			CrefOperatorReferenceSyntax crefOperatorReferenceSyntax = new CrefOperatorReferenceSyntax(SyntaxKind.CrefOperatorReference, operatorKeyword, operatorToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(crefOperatorReferenceSyntax, hash);
			}
			return crefOperatorReferenceSyntax;
		}

		internal static QualifiedCrefOperatorReferenceSyntax QualifiedCrefOperatorReference(NameSyntax left, PunctuationSyntax dotToken, CrefOperatorReferenceSyntax right)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(410, left, dotToken, right, out hash);
			if (greenNode != null)
			{
				return (QualifiedCrefOperatorReferenceSyntax)greenNode;
			}
			QualifiedCrefOperatorReferenceSyntax qualifiedCrefOperatorReferenceSyntax = new QualifiedCrefOperatorReferenceSyntax(SyntaxKind.QualifiedCrefOperatorReference, left, dotToken, right);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(qualifiedCrefOperatorReferenceSyntax, hash);
			}
			return qualifiedCrefOperatorReferenceSyntax;
		}

		internal static YieldStatementSyntax YieldStatement(KeywordSyntax yieldKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(411, yieldKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (YieldStatementSyntax)greenNode;
			}
			YieldStatementSyntax yieldStatementSyntax = new YieldStatementSyntax(SyntaxKind.YieldStatement, yieldKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(yieldStatementSyntax, hash);
			}
			return yieldStatementSyntax;
		}

		internal static AwaitExpressionSyntax AwaitExpression(KeywordSyntax awaitKeyword, ExpressionSyntax expression)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(412, awaitKeyword, expression, out hash);
			if (greenNode != null)
			{
				return (AwaitExpressionSyntax)greenNode;
			}
			AwaitExpressionSyntax awaitExpressionSyntax = new AwaitExpressionSyntax(SyntaxKind.AwaitExpression, awaitKeyword, expression);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(awaitExpressionSyntax, hash);
			}
			return awaitExpressionSyntax;
		}

		internal static XmlNameTokenSyntax XmlNameToken(string text, SyntaxKind possibleKeywordKind, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlNameTokenSyntax(SyntaxKind.XmlNameToken, text, leadingTrivia, trailingTrivia, possibleKeywordKind);
		}

		internal static XmlTextTokenSyntax XmlTextLiteralToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(SyntaxKind.XmlTextLiteralToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static XmlTextTokenSyntax XmlEntityLiteralToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(SyntaxKind.XmlEntityLiteralToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static XmlTextTokenSyntax DocumentationCommentLineBreakToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(SyntaxKind.DocumentationCommentLineBreakToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static XmlTextTokenSyntax XmlTextToken(SyntaxKind kind, string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new XmlTextTokenSyntax(kind, text, leadingTrivia, trailingTrivia, value);
		}

		internal static InterpolatedStringTextTokenSyntax InterpolatedStringTextToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new InterpolatedStringTextTokenSyntax(SyntaxKind.InterpolatedStringTextToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static DecimalLiteralTokenSyntax DecimalLiteralToken(string text, TypeCharacter typeSuffix, decimal value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new DecimalLiteralTokenSyntax(SyntaxKind.DecimalLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value);
		}

		internal static DateLiteralTokenSyntax DateLiteralToken(string text, DateTime value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new DateLiteralTokenSyntax(SyntaxKind.DateLiteralToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static StringLiteralTokenSyntax StringLiteralToken(string text, string value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new StringLiteralTokenSyntax(SyntaxKind.StringLiteralToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static CharacterLiteralTokenSyntax CharacterLiteralToken(string text, char value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new CharacterLiteralTokenSyntax(SyntaxKind.CharacterLiteralToken, text, leadingTrivia, trailingTrivia, value);
		}

		internal static SkippedTokensTriviaSyntax SkippedTokensTrivia(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> tokens)
		{
			return new SkippedTokensTriviaSyntax(SyntaxKind.SkippedTokensTrivia, tokens.Node);
		}

		internal static DocumentationCommentTriviaSyntax DocumentationCommentTrivia(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> content)
		{
			return new DocumentationCommentTriviaSyntax(SyntaxKind.DocumentationCommentTrivia, content.Node);
		}

		internal static XmlCrefAttributeSyntax XmlCrefAttribute(XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, CrefReferenceSyntax reference, PunctuationSyntax endQuoteToken)
		{
			return new XmlCrefAttributeSyntax(SyntaxKind.XmlCrefAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken);
		}

		internal static XmlNameAttributeSyntax XmlNameAttribute(XmlNameSyntax name, PunctuationSyntax equalsToken, PunctuationSyntax startQuoteToken, IdentifierNameSyntax reference, PunctuationSyntax endQuoteToken)
		{
			return new XmlNameAttributeSyntax(SyntaxKind.XmlNameAttribute, name, equalsToken, startQuoteToken, reference, endQuoteToken);
		}

		internal static ConditionalAccessExpressionSyntax ConditionalAccessExpression(ExpressionSyntax expression, PunctuationSyntax questionMarkToken, ExpressionSyntax whenNotNull)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(713, expression, questionMarkToken, whenNotNull, out hash);
			if (greenNode != null)
			{
				return (ConditionalAccessExpressionSyntax)greenNode;
			}
			ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = new ConditionalAccessExpressionSyntax(SyntaxKind.ConditionalAccessExpression, expression, questionMarkToken, whenNotNull);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(conditionalAccessExpressionSyntax, hash);
			}
			return conditionalAccessExpressionSyntax;
		}

		internal static NameOfExpressionSyntax NameOfExpression(KeywordSyntax nameOfKeyword, PunctuationSyntax openParenToken, ExpressionSyntax argument, PunctuationSyntax closeParenToken)
		{
			return new NameOfExpressionSyntax(SyntaxKind.NameOfExpression, nameOfKeyword, openParenToken, argument, closeParenToken);
		}

		internal static InterpolatedStringExpressionSyntax InterpolatedStringExpression(PunctuationSyntax dollarSignDoubleQuoteToken, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> contents, PunctuationSyntax doubleQuoteToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(780, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken, out hash);
			if (greenNode != null)
			{
				return (InterpolatedStringExpressionSyntax)greenNode;
			}
			InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax = new InterpolatedStringExpressionSyntax(SyntaxKind.InterpolatedStringExpression, dollarSignDoubleQuoteToken, contents.Node, doubleQuoteToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolatedStringExpressionSyntax, hash);
			}
			return interpolatedStringExpressionSyntax;
		}

		internal static InterpolatedStringTextSyntax InterpolatedStringText(InterpolatedStringTextTokenSyntax textToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(781, textToken, out hash);
			if (greenNode != null)
			{
				return (InterpolatedStringTextSyntax)greenNode;
			}
			InterpolatedStringTextSyntax interpolatedStringTextSyntax = new InterpolatedStringTextSyntax(SyntaxKind.InterpolatedStringText, textToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolatedStringTextSyntax, hash);
			}
			return interpolatedStringTextSyntax;
		}

		internal static InterpolationSyntax Interpolation(PunctuationSyntax openBraceToken, ExpressionSyntax expression, InterpolationAlignmentClauseSyntax alignmentClause, InterpolationFormatClauseSyntax formatClause, PunctuationSyntax closeBraceToken)
		{
			return new InterpolationSyntax(SyntaxKind.Interpolation, openBraceToken, expression, alignmentClause, formatClause, closeBraceToken);
		}

		internal static InterpolationAlignmentClauseSyntax InterpolationAlignmentClause(PunctuationSyntax commaToken, ExpressionSyntax value)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(783, commaToken, value, out hash);
			if (greenNode != null)
			{
				return (InterpolationAlignmentClauseSyntax)greenNode;
			}
			InterpolationAlignmentClauseSyntax interpolationAlignmentClauseSyntax = new InterpolationAlignmentClauseSyntax(SyntaxKind.InterpolationAlignmentClause, commaToken, value);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolationAlignmentClauseSyntax, hash);
			}
			return interpolationAlignmentClauseSyntax;
		}

		internal static InterpolationFormatClauseSyntax InterpolationFormatClause(PunctuationSyntax colonToken, InterpolatedStringTextTokenSyntax formatStringToken)
		{
			int hash;
			GreenNode greenNode = SyntaxNodeCache.TryGetNode(784, colonToken, formatStringToken, out hash);
			if (greenNode != null)
			{
				return (InterpolationFormatClauseSyntax)greenNode;
			}
			InterpolationFormatClauseSyntax interpolationFormatClauseSyntax = new InterpolationFormatClauseSyntax(SyntaxKind.InterpolationFormatClause, colonToken, formatStringToken);
			if (hash >= 0)
			{
				SyntaxNodeCache.AddNode(interpolationFormatClauseSyntax, hash);
			}
			return interpolationFormatClauseSyntax;
		}

		internal static SyntaxTrivia WhitespaceTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text);
		}

		internal static SyntaxTrivia EndOfLineTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.EndOfLineTrivia, text);
		}

		internal static SyntaxTrivia ColonTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.ColonTrivia, text);
		}

		internal static SyntaxTrivia CommentTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.CommentTrivia, text);
		}

		internal static SyntaxTrivia ConflictMarkerTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.ConflictMarkerTrivia, text);
		}

		internal static SyntaxTrivia LineContinuationTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.LineContinuationTrivia, text);
		}

		internal static SyntaxTrivia DocumentationCommentExteriorTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.DocumentationCommentExteriorTrivia, text);
		}

		internal static SyntaxTrivia DisabledTextTrivia(string text)
		{
			return new SyntaxTrivia(SyntaxKind.DisabledTextTrivia, text);
		}

		internal static SyntaxTrivia SyntaxTrivia(SyntaxKind kind, string text)
		{
			return new SyntaxTrivia(kind, text);
		}

		internal static ConstDirectiveTriviaSyntax ConstDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax constKeyword, IdentifierTokenSyntax name, PunctuationSyntax equalsToken, ExpressionSyntax value)
		{
			return new ConstDirectiveTriviaSyntax(SyntaxKind.ConstDirectiveTrivia, hashToken, constKeyword, name, equalsToken, value);
		}

		internal static IfDirectiveTriviaSyntax IfDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			return new IfDirectiveTriviaSyntax(SyntaxKind.IfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword);
		}

		internal static IfDirectiveTriviaSyntax ElseIfDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax elseKeyword, KeywordSyntax ifOrElseIfKeyword, ExpressionSyntax condition, KeywordSyntax thenKeyword)
		{
			return new IfDirectiveTriviaSyntax(SyntaxKind.ElseIfDirectiveTrivia, hashToken, elseKeyword, ifOrElseIfKeyword, condition, thenKeyword);
		}

		internal static ElseDirectiveTriviaSyntax ElseDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax elseKeyword)
		{
			return new ElseDirectiveTriviaSyntax(SyntaxKind.ElseDirectiveTrivia, hashToken, elseKeyword);
		}

		internal static EndIfDirectiveTriviaSyntax EndIfDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax ifKeyword)
		{
			return new EndIfDirectiveTriviaSyntax(SyntaxKind.EndIfDirectiveTrivia, hashToken, endKeyword, ifKeyword);
		}

		internal static RegionDirectiveTriviaSyntax RegionDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax regionKeyword, StringLiteralTokenSyntax name)
		{
			return new RegionDirectiveTriviaSyntax(SyntaxKind.RegionDirectiveTrivia, hashToken, regionKeyword, name);
		}

		internal static EndRegionDirectiveTriviaSyntax EndRegionDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax regionKeyword)
		{
			return new EndRegionDirectiveTriviaSyntax(SyntaxKind.EndRegionDirectiveTrivia, hashToken, endKeyword, regionKeyword);
		}

		internal static ExternalSourceDirectiveTriviaSyntax ExternalSourceDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax externalSourceKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax commaToken, IntegerLiteralTokenSyntax lineStart, PunctuationSyntax closeParenToken)
		{
			return new ExternalSourceDirectiveTriviaSyntax(SyntaxKind.ExternalSourceDirectiveTrivia, hashToken, externalSourceKeyword, openParenToken, externalSource, commaToken, lineStart, closeParenToken);
		}

		internal static EndExternalSourceDirectiveTriviaSyntax EndExternalSourceDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax endKeyword, KeywordSyntax externalSourceKeyword)
		{
			return new EndExternalSourceDirectiveTriviaSyntax(SyntaxKind.EndExternalSourceDirectiveTrivia, hashToken, endKeyword, externalSourceKeyword);
		}

		internal static ExternalChecksumDirectiveTriviaSyntax ExternalChecksumDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax externalChecksumKeyword, PunctuationSyntax openParenToken, StringLiteralTokenSyntax externalSource, PunctuationSyntax firstCommaToken, StringLiteralTokenSyntax guid, PunctuationSyntax secondCommaToken, StringLiteralTokenSyntax checksum, PunctuationSyntax closeParenToken)
		{
			return new ExternalChecksumDirectiveTriviaSyntax(SyntaxKind.ExternalChecksumDirectiveTrivia, hashToken, externalChecksumKeyword, openParenToken, externalSource, firstCommaToken, guid, secondCommaToken, checksum, closeParenToken);
		}

		internal static EnableWarningDirectiveTriviaSyntax EnableWarningDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax enableKeyword, KeywordSyntax warningKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> errorCodes)
		{
			return new EnableWarningDirectiveTriviaSyntax(SyntaxKind.EnableWarningDirectiveTrivia, hashToken, enableKeyword, warningKeyword, errorCodes.Node);
		}

		internal static DisableWarningDirectiveTriviaSyntax DisableWarningDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax disableKeyword, KeywordSyntax warningKeyword, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SeparatedSyntaxList<GreenNode> errorCodes)
		{
			return new DisableWarningDirectiveTriviaSyntax(SyntaxKind.DisableWarningDirectiveTrivia, hashToken, disableKeyword, warningKeyword, errorCodes.Node);
		}

		internal static ReferenceDirectiveTriviaSyntax ReferenceDirectiveTrivia(PunctuationSyntax hashToken, KeywordSyntax referenceKeyword, StringLiteralTokenSyntax file)
		{
			return new ReferenceDirectiveTriviaSyntax(SyntaxKind.ReferenceDirectiveTrivia, hashToken, referenceKeyword, file);
		}

		internal static BadDirectiveTriviaSyntax BadDirectiveTrivia(PunctuationSyntax hashToken)
		{
			return new BadDirectiveTriviaSyntax(SyntaxKind.BadDirectiveTrivia, hashToken);
		}

		internal static SyntaxTrivia EndOfLine(string text, bool elastic = false)
		{
			SyntaxTrivia syntaxTrivia = null;
			if (EmbeddedOperators.CompareString(text, "\r", TextCompare: false) != 0)
			{
				if (EmbeddedOperators.CompareString(text, "\n", TextCompare: false) != 0)
				{
					if (EmbeddedOperators.CompareString(text, "\r\n", TextCompare: false) == 0)
					{
						syntaxTrivia = (elastic ? ElasticCarriageReturnLineFeed : CarriageReturnLineFeed);
					}
				}
				else
				{
					syntaxTrivia = (elastic ? ElasticLineFeed : LineFeed);
				}
			}
			else
			{
				syntaxTrivia = (elastic ? ElasticCarriageReturn : CarriageReturn);
			}
			if (syntaxTrivia != null)
			{
				return syntaxTrivia;
			}
			syntaxTrivia = SyntaxTrivia(SyntaxKind.EndOfLineTrivia, text);
			if (!elastic)
			{
				return syntaxTrivia;
			}
			return SyntaxExtensions.WithAnnotations(syntaxTrivia, SyntaxAnnotation.ElasticAnnotation);
		}

		internal static SyntaxTrivia Whitespace(string text, bool elastic = false)
		{
			SyntaxTrivia syntaxTrivia = SyntaxTrivia(SyntaxKind.WhitespaceTrivia, text);
			if (!elastic)
			{
				return syntaxTrivia;
			}
			return SyntaxExtensions.WithAnnotations(syntaxTrivia, SyntaxAnnotation.ElasticAnnotation);
		}

		internal static SyntaxToken Token(GreenNode leading, SyntaxKind kind, GreenNode trailing, string text = null)
		{
			return SyntaxToken.Create(kind, leading, trailing, (text == null) ? SyntaxFacts.GetText(kind) : text);
		}

		internal static IEnumerable<SyntaxTrivia> GetWellKnownTrivia()
		{
			return new SyntaxTrivia[14]
			{
				CarriageReturn,
				CarriageReturnLineFeed,
				LineFeed,
				Space,
				Tab,
				ElasticCarriageReturn,
				ElasticLineFeed,
				ElasticCarriageReturnLineFeed,
				ElasticSpace,
				ElasticTab,
				ElasticZeroSpace,
				Whitespace("  "),
				Whitespace("   "),
				Whitespace("    ")
			};
		}

		internal static IntegerLiteralTokenSyntax IntegerLiteralToken(string text, LiteralBase @base, TypeCharacter typeSuffix, ulong value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			switch (typeSuffix)
			{
			case TypeCharacter.ShortLiteral:
				return new IntegerLiteralTokenSyntax<short>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (short)value);
			case TypeCharacter.UShortLiteral:
				return new IntegerLiteralTokenSyntax<ushort>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (ushort)value);
			case TypeCharacter.Integer:
			case TypeCharacter.IntegerLiteral:
				return new IntegerLiteralTokenSyntax<int>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (int)value);
			case TypeCharacter.UIntegerLiteral:
				return new IntegerLiteralTokenSyntax<uint>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (uint)value);
			case TypeCharacter.Long:
			case TypeCharacter.LongLiteral:
				return new IntegerLiteralTokenSyntax<long>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (long)value);
			case TypeCharacter.ULongLiteral:
				return new IntegerLiteralTokenSyntax<ulong>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, value);
			case TypeCharacter.None:
			{
				bool flag = false;
				if ((@base != 0) ? (decimal.Compare(new decimal(value & 0xFFFFFFFF00000000uL), 0m) == 0) : (decimal.Compare(new decimal(value), new decimal(2147483647L)) <= 0))
				{
					return new IntegerLiteralTokenSyntax<int>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (int)value);
				}
				return new IntegerLiteralTokenSyntax<long>(SyntaxKind.IntegerLiteralToken, text, leadingTrivia, trailingTrivia, @base, typeSuffix, (long)value);
			}
			default:
				throw new ArgumentException("typeSuffix");
			}
		}

		internal static FloatingLiteralTokenSyntax FloatingLiteralToken(string text, TypeCharacter typeSuffix, double value, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			switch (typeSuffix)
			{
			case TypeCharacter.None:
			case TypeCharacter.Double:
			case TypeCharacter.DoubleLiteral:
				return new FloatingLiteralTokenSyntax<double>(SyntaxKind.FloatingLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, value);
			case TypeCharacter.Single:
			case TypeCharacter.SingleLiteral:
				return new FloatingLiteralTokenSyntax<float>(SyntaxKind.FloatingLiteralToken, text, leadingTrivia, trailingTrivia, typeSuffix, (float)value);
			default:
				throw new ArgumentException("typeSuffix");
			}
		}

		internal static IdentifierTokenSyntax Identifier(string text, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new SimpleIdentifierSyntax(SyntaxKind.IdentifierToken, null, null, text, leadingTrivia, trailingTrivia);
		}

		internal static IdentifierTokenSyntax Identifier(string text, SyntaxKind possibleKeywordKind, bool isBracketed, string identifierText, TypeCharacter typeCharacter, GreenNode leadingTrivia, GreenNode trailingTrivia)
		{
			return new ComplexIdentifierSyntax(SyntaxKind.IdentifierToken, null, null, text, leadingTrivia, trailingTrivia, possibleKeywordKind, isBracketed, identifierText, typeCharacter);
		}

		internal static IdentifierTokenSyntax Identifier(string text)
		{
			return new SimpleIdentifierSyntax(SyntaxKind.IdentifierToken, null, null, text, null, null);
		}

		internal static IdentifierTokenSyntax MissingIdentifier()
		{
			return new SimpleIdentifierSyntax(SyntaxKind.IdentifierToken, null, null, "", null, null);
		}

		internal static IdentifierTokenSyntax MissingIdentifier(SyntaxKind kind)
		{
			return new ComplexIdentifierSyntax(SyntaxKind.IdentifierToken, null, null, "", null, null, kind, isBracketed: false, "", TypeCharacter.None);
		}

		internal static KeywordSyntax MissingKeyword(SyntaxKind kind)
		{
			return new KeywordSyntax(kind, "", null, null);
		}

		internal static PunctuationSyntax MissingPunctuation(SyntaxKind kind)
		{
			return new PunctuationSyntax(kind, "", null, null);
		}

		internal static StringLiteralTokenSyntax MissingStringLiteral()
		{
			return StringLiteralToken("", "", null, null);
		}

		internal static CharacterLiteralTokenSyntax MissingCharacterLiteralToken()
		{
			return CharacterLiteralToken("", '\0', null, null);
		}

		internal static IntegerLiteralTokenSyntax MissingIntegerLiteralToken()
		{
			return IntegerLiteralToken("", LiteralBase.Decimal, TypeCharacter.None, 0uL, null, null);
		}

		internal static SyntaxToken MissingToken(SyntaxKind kind)
		{
			SyntaxToken result;
			switch (kind)
			{
			case SyntaxKind.StatementTerminatorToken:
				result = Token(null, SyntaxKind.StatementTerminatorToken, null, string.Empty);
				break;
			case SyntaxKind.EndOfFileToken:
				result = EndOfFileToken();
				break;
			case SyntaxKind.AddHandlerKeyword:
			case SyntaxKind.AddressOfKeyword:
			case SyntaxKind.AliasKeyword:
			case SyntaxKind.AndKeyword:
			case SyntaxKind.AndAlsoKeyword:
			case SyntaxKind.AsKeyword:
			case SyntaxKind.BooleanKeyword:
			case SyntaxKind.ByRefKeyword:
			case SyntaxKind.ByteKeyword:
			case SyntaxKind.ByValKeyword:
			case SyntaxKind.CallKeyword:
			case SyntaxKind.CaseKeyword:
			case SyntaxKind.CatchKeyword:
			case SyntaxKind.CBoolKeyword:
			case SyntaxKind.CByteKeyword:
			case SyntaxKind.CCharKeyword:
			case SyntaxKind.CDateKeyword:
			case SyntaxKind.CDecKeyword:
			case SyntaxKind.CDblKeyword:
			case SyntaxKind.CharKeyword:
			case SyntaxKind.CIntKeyword:
			case SyntaxKind.ClassKeyword:
			case SyntaxKind.CLngKeyword:
			case SyntaxKind.CObjKeyword:
			case SyntaxKind.ConstKeyword:
			case SyntaxKind.ContinueKeyword:
			case SyntaxKind.CSByteKeyword:
			case SyntaxKind.CShortKeyword:
			case SyntaxKind.CSngKeyword:
			case SyntaxKind.CStrKeyword:
			case SyntaxKind.CTypeKeyword:
			case SyntaxKind.CUIntKeyword:
			case SyntaxKind.CULngKeyword:
			case SyntaxKind.CUShortKeyword:
			case SyntaxKind.DateKeyword:
			case SyntaxKind.DecimalKeyword:
			case SyntaxKind.DeclareKeyword:
			case SyntaxKind.DefaultKeyword:
			case SyntaxKind.DelegateKeyword:
			case SyntaxKind.DimKeyword:
			case SyntaxKind.DirectCastKeyword:
			case SyntaxKind.DoKeyword:
			case SyntaxKind.DoubleKeyword:
			case SyntaxKind.EachKeyword:
			case SyntaxKind.ElseKeyword:
			case SyntaxKind.ElseIfKeyword:
			case SyntaxKind.EndKeyword:
			case SyntaxKind.EnumKeyword:
			case SyntaxKind.EraseKeyword:
			case SyntaxKind.ErrorKeyword:
			case SyntaxKind.EventKeyword:
			case SyntaxKind.ExitKeyword:
			case SyntaxKind.FalseKeyword:
			case SyntaxKind.FinallyKeyword:
			case SyntaxKind.ForKeyword:
			case SyntaxKind.FriendKeyword:
			case SyntaxKind.FunctionKeyword:
			case SyntaxKind.GetKeyword:
			case SyntaxKind.GetTypeKeyword:
			case SyntaxKind.GetXmlNamespaceKeyword:
			case SyntaxKind.GlobalKeyword:
			case SyntaxKind.GoToKeyword:
			case SyntaxKind.HandlesKeyword:
			case SyntaxKind.IfKeyword:
			case SyntaxKind.ImplementsKeyword:
			case SyntaxKind.ImportsKeyword:
			case SyntaxKind.InKeyword:
			case SyntaxKind.InheritsKeyword:
			case SyntaxKind.IntegerKeyword:
			case SyntaxKind.InterfaceKeyword:
			case SyntaxKind.IsKeyword:
			case SyntaxKind.IsNotKeyword:
			case SyntaxKind.LetKeyword:
			case SyntaxKind.LibKeyword:
			case SyntaxKind.LikeKeyword:
			case SyntaxKind.LongKeyword:
			case SyntaxKind.LoopKeyword:
			case SyntaxKind.MeKeyword:
			case SyntaxKind.ModKeyword:
			case SyntaxKind.ModuleKeyword:
			case SyntaxKind.MustInheritKeyword:
			case SyntaxKind.MustOverrideKeyword:
			case SyntaxKind.MyBaseKeyword:
			case SyntaxKind.MyClassKeyword:
			case SyntaxKind.NamespaceKeyword:
			case SyntaxKind.NarrowingKeyword:
			case SyntaxKind.NextKeyword:
			case SyntaxKind.NewKeyword:
			case SyntaxKind.NotKeyword:
			case SyntaxKind.NothingKeyword:
			case SyntaxKind.NotInheritableKeyword:
			case SyntaxKind.NotOverridableKeyword:
			case SyntaxKind.ObjectKeyword:
			case SyntaxKind.OfKeyword:
			case SyntaxKind.OnKeyword:
			case SyntaxKind.OperatorKeyword:
			case SyntaxKind.OptionKeyword:
			case SyntaxKind.OptionalKeyword:
			case SyntaxKind.OrKeyword:
			case SyntaxKind.OrElseKeyword:
			case SyntaxKind.OverloadsKeyword:
			case SyntaxKind.OverridableKeyword:
			case SyntaxKind.OverridesKeyword:
			case SyntaxKind.ParamArrayKeyword:
			case SyntaxKind.PartialKeyword:
			case SyntaxKind.PrivateKeyword:
			case SyntaxKind.PropertyKeyword:
			case SyntaxKind.ProtectedKeyword:
			case SyntaxKind.PublicKeyword:
			case SyntaxKind.RaiseEventKeyword:
			case SyntaxKind.ReadOnlyKeyword:
			case SyntaxKind.ReDimKeyword:
			case SyntaxKind.REMKeyword:
			case SyntaxKind.RemoveHandlerKeyword:
			case SyntaxKind.ResumeKeyword:
			case SyntaxKind.ReturnKeyword:
			case SyntaxKind.SByteKeyword:
			case SyntaxKind.SelectKeyword:
			case SyntaxKind.SetKeyword:
			case SyntaxKind.ShadowsKeyword:
			case SyntaxKind.SharedKeyword:
			case SyntaxKind.ShortKeyword:
			case SyntaxKind.SingleKeyword:
			case SyntaxKind.StaticKeyword:
			case SyntaxKind.StepKeyword:
			case SyntaxKind.StopKeyword:
			case SyntaxKind.StringKeyword:
			case SyntaxKind.StructureKeyword:
			case SyntaxKind.SubKeyword:
			case SyntaxKind.SyncLockKeyword:
			case SyntaxKind.ThenKeyword:
			case SyntaxKind.ThrowKeyword:
			case SyntaxKind.ToKeyword:
			case SyntaxKind.TrueKeyword:
			case SyntaxKind.TryKeyword:
			case SyntaxKind.TryCastKeyword:
			case SyntaxKind.TypeOfKeyword:
			case SyntaxKind.UIntegerKeyword:
			case SyntaxKind.ULongKeyword:
			case SyntaxKind.UShortKeyword:
			case SyntaxKind.UsingKeyword:
			case SyntaxKind.WhenKeyword:
			case SyntaxKind.WhileKeyword:
			case SyntaxKind.WideningKeyword:
			case SyntaxKind.WithKeyword:
			case SyntaxKind.WithEventsKeyword:
			case SyntaxKind.WriteOnlyKeyword:
			case SyntaxKind.XorKeyword:
			case SyntaxKind.EndIfKeyword:
			case SyntaxKind.GosubKeyword:
			case SyntaxKind.VariantKeyword:
			case SyntaxKind.WendKeyword:
			case SyntaxKind.OutKeyword:
			case SyntaxKind.NameOfKeyword:
				result = MissingKeyword(kind);
				break;
			case SyntaxKind.ReferenceKeyword:
			case SyntaxKind.AggregateKeyword:
			case SyntaxKind.AllKeyword:
			case SyntaxKind.AnsiKeyword:
			case SyntaxKind.AscendingKeyword:
			case SyntaxKind.AssemblyKeyword:
			case SyntaxKind.AutoKeyword:
			case SyntaxKind.BinaryKeyword:
			case SyntaxKind.ByKeyword:
			case SyntaxKind.CompareKeyword:
			case SyntaxKind.CustomKeyword:
			case SyntaxKind.DescendingKeyword:
			case SyntaxKind.DisableKeyword:
			case SyntaxKind.DistinctKeyword:
			case SyntaxKind.EnableKeyword:
			case SyntaxKind.EqualsKeyword:
			case SyntaxKind.ExplicitKeyword:
			case SyntaxKind.ExternalSourceKeyword:
			case SyntaxKind.ExternalChecksumKeyword:
			case SyntaxKind.FromKeyword:
			case SyntaxKind.GroupKeyword:
			case SyntaxKind.InferKeyword:
			case SyntaxKind.IntoKeyword:
			case SyntaxKind.IsFalseKeyword:
			case SyntaxKind.IsTrueKeyword:
			case SyntaxKind.JoinKeyword:
			case SyntaxKind.KeyKeyword:
			case SyntaxKind.MidKeyword:
			case SyntaxKind.OffKeyword:
			case SyntaxKind.OrderKeyword:
			case SyntaxKind.PreserveKeyword:
			case SyntaxKind.RegionKeyword:
			case SyntaxKind.SkipKeyword:
			case SyntaxKind.StrictKeyword:
			case SyntaxKind.TakeKeyword:
			case SyntaxKind.TextKeyword:
			case SyntaxKind.UnicodeKeyword:
			case SyntaxKind.UntilKeyword:
			case SyntaxKind.WarningKeyword:
			case SyntaxKind.WhereKeyword:
				return MissingIdentifier(kind);
			case SyntaxKind.ExclamationToken:
			case SyntaxKind.CommaToken:
			case SyntaxKind.HashToken:
			case SyntaxKind.AmpersandToken:
			case SyntaxKind.SingleQuoteToken:
			case SyntaxKind.OpenParenToken:
			case SyntaxKind.CloseParenToken:
			case SyntaxKind.OpenBraceToken:
			case SyntaxKind.CloseBraceToken:
			case SyntaxKind.SemicolonToken:
			case SyntaxKind.AsteriskToken:
			case SyntaxKind.PlusToken:
			case SyntaxKind.MinusToken:
			case SyntaxKind.DotToken:
			case SyntaxKind.SlashToken:
			case SyntaxKind.ColonToken:
			case SyntaxKind.LessThanToken:
			case SyntaxKind.LessThanEqualsToken:
			case SyntaxKind.LessThanGreaterThanToken:
			case SyntaxKind.EqualsToken:
			case SyntaxKind.GreaterThanToken:
			case SyntaxKind.GreaterThanEqualsToken:
			case SyntaxKind.BackslashToken:
			case SyntaxKind.CaretToken:
			case SyntaxKind.ColonEqualsToken:
			case SyntaxKind.AmpersandEqualsToken:
			case SyntaxKind.AsteriskEqualsToken:
			case SyntaxKind.PlusEqualsToken:
			case SyntaxKind.MinusEqualsToken:
			case SyntaxKind.SlashEqualsToken:
			case SyntaxKind.BackslashEqualsToken:
			case SyntaxKind.CaretEqualsToken:
			case SyntaxKind.LessThanLessThanToken:
			case SyntaxKind.GreaterThanGreaterThanToken:
			case SyntaxKind.LessThanLessThanEqualsToken:
			case SyntaxKind.GreaterThanGreaterThanEqualsToken:
			case SyntaxKind.QuestionToken:
			case SyntaxKind.DoubleQuoteToken:
				result = MissingPunctuation(kind);
				break;
			case SyntaxKind.FloatingLiteralToken:
				result = FloatingLiteralToken("", TypeCharacter.None, 0.0, null, null);
				break;
			case SyntaxKind.DecimalLiteralToken:
				result = DecimalLiteralToken("", TypeCharacter.None, 0m, null, null);
				break;
			case SyntaxKind.DateLiteralToken:
				result = DateLiteralToken("", DateTime.MinValue, null, null);
				break;
			case SyntaxKind.XmlNameToken:
				result = XmlNameToken("", SyntaxKind.XmlNameToken, null, null);
				break;
			case SyntaxKind.XmlTextLiteralToken:
				result = XmlTextLiteralToken("", "", null, null);
				break;
			case SyntaxKind.SlashGreaterThanToken:
			case SyntaxKind.LessThanSlashToken:
			case SyntaxKind.LessThanExclamationMinusMinusToken:
			case SyntaxKind.MinusMinusGreaterThanToken:
			case SyntaxKind.LessThanQuestionToken:
			case SyntaxKind.QuestionGreaterThanToken:
			case SyntaxKind.LessThanPercentEqualsToken:
			case SyntaxKind.PercentGreaterThanToken:
			case SyntaxKind.BeginCDataToken:
			case SyntaxKind.EndCDataToken:
				result = MissingPunctuation(kind);
				break;
			case SyntaxKind.IdentifierToken:
				result = MissingIdentifier();
				break;
			case SyntaxKind.IntegerLiteralToken:
				result = MissingIntegerLiteralToken();
				break;
			case SyntaxKind.StringLiteralToken:
				result = MissingStringLiteral();
				break;
			case SyntaxKind.CharacterLiteralToken:
				result = MissingCharacterLiteralToken();
				break;
			case SyntaxKind.InterpolatedStringTextToken:
				result = InterpolatedStringTextToken("", "", null, null);
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(kind);
			}
			return result;
		}

		internal static BadTokenSyntax BadToken(SyntaxSubKind SubKind, string text, GreenNode precedingTrivia, GreenNode followingTrivia)
		{
			return new BadTokenSyntax(SyntaxKind.BadToken, SubKind, null, null, text, precedingTrivia, followingTrivia);
		}

		internal static PunctuationSyntax EndOfFileToken(SyntaxTrivia precedingTrivia)
		{
			return new PunctuationSyntax(SyntaxKind.EndOfFileToken, "", precedingTrivia, null);
		}

		internal static PunctuationSyntax EndOfFileToken()
		{
			return new PunctuationSyntax(SyntaxKind.EndOfFileToken, "", null, null);
		}

		internal static IdentifierTokenSyntax Identifier(string text, bool isBracketed, string baseText, TypeCharacter typeCharacter, GreenNode precedingTrivia, GreenNode followingTrivia)
		{
			return new ComplexIdentifierSyntax(SyntaxKind.IdentifierToken, null, null, text, precedingTrivia, followingTrivia, SyntaxKind.IdentifierToken, isBracketed, baseText, typeCharacter);
		}

		internal static ExpressionSyntax MissingExpression()
		{
			return s_missingExpr;
		}

		internal static EmptyStatementSyntax EmptyStatement()
		{
			return s_emptyStatement;
		}

		internal static OmittedArgumentSyntax OmittedArgument()
		{
			return s_omittedArgument;
		}

		public static TypeBlockSyntax TypeBlock(SyntaxKind blockKind, TypeStatementSyntax begin, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<InheritsStatementSyntax> inherits, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<ImplementsStatementSyntax> implements, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<StatementSyntax> members, EndBlockStatementSyntax end)
		{
			return blockKind switch
			{
				SyntaxKind.ModuleBlock => ModuleBlock((ModuleStatementSyntax)begin, inherits, implements, members, end), 
				SyntaxKind.ClassBlock => ClassBlock((ClassStatementSyntax)begin, inherits, implements, members, end), 
				SyntaxKind.StructureBlock => StructureBlock((StructureStatementSyntax)begin, inherits, implements, members, end), 
				SyntaxKind.InterfaceBlock => InterfaceBlock((InterfaceStatementSyntax)begin, inherits, implements, members, end), 
				_ => throw ExceptionUtilities.UnexpectedValue(blockKind), 
			};
		}

		public static TypeStatementSyntax TypeStatement(SyntaxKind statementKind, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> attributes, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> modifiers, KeywordSyntax keyword, IdentifierTokenSyntax identifier, TypeParameterListSyntax typeParameterList)
		{
			return statementKind switch
			{
				SyntaxKind.ModuleStatement => ModuleStatement(attributes, modifiers, keyword, identifier, typeParameterList), 
				SyntaxKind.ClassStatement => ClassStatement(attributes, modifiers, keyword, identifier, typeParameterList), 
				SyntaxKind.StructureStatement => StructureStatement(attributes, modifiers, keyword, identifier, typeParameterList), 
				SyntaxKind.InterfaceStatement => InterfaceStatement(attributes, modifiers, keyword, identifier, typeParameterList), 
				_ => throw ExceptionUtilities.UnexpectedValue(statementKind), 
			};
		}
	}
}
