using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.Operations
{
	internal sealed class VisualBasicOperationFactory
	{
		private struct BinaryOperatorInfo
		{
			public readonly BoundExpression LeftOperand;

			public readonly BoundExpression RightOperand;

			public readonly BinaryOperatorKind OperatorKind;

			public readonly MethodSymbol OperatorMethod;

			public readonly bool IsLifted;

			public readonly bool IsChecked;

			public readonly bool IsCompareText;

			public BinaryOperatorInfo(BoundExpression leftOperand, BoundExpression rightOperand, BinaryOperatorKind binaryOperatorKind, MethodSymbol operatorMethod, bool isLifted, bool isChecked, bool isCompareText)
			{
				this = default(BinaryOperatorInfo);
				LeftOperand = leftOperand;
				RightOperand = rightOperand;
				OperatorKind = binaryOperatorKind;
				OperatorMethod = operatorMethod;
				IsLifted = isLifted;
				IsChecked = isChecked;
				IsCompareText = isCompareText;
			}
		}

		internal class Helper
		{
			internal static UnaryOperatorKind DeriveUnaryOperatorKind(Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind operatorKind)
			{
				return (operatorKind & Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.OpMask) switch
				{
					Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Plus => UnaryOperatorKind.Plus, 
					Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Minus => UnaryOperatorKind.Minus, 
					Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Not => UnaryOperatorKind.Not, 
					Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsTrue => UnaryOperatorKind.True, 
					Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.IsFalse => UnaryOperatorKind.False, 
					_ => UnaryOperatorKind.None, 
				};
			}

			internal static BinaryOperatorKind DeriveBinaryOperatorKind(Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind operatorKind, BoundExpression leftOpt)
			{
				switch (operatorKind & Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OpMask)
				{
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Add:
					return BinaryOperatorKind.Add;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Subtract:
					return BinaryOperatorKind.Subtract;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Multiply:
					return BinaryOperatorKind.Multiply;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Divide:
					return BinaryOperatorKind.Divide;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.IntegerDivide:
					return BinaryOperatorKind.IntegerDivide;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Modulo:
					return BinaryOperatorKind.Remainder;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.And:
					return BinaryOperatorKind.And;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Or:
					return BinaryOperatorKind.Or;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Xor:
					return BinaryOperatorKind.ExclusiveOr;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.AndAlso:
					return BinaryOperatorKind.ConditionalAnd;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.OrElse:
					return BinaryOperatorKind.ConditionalOr;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LeftShift:
					return BinaryOperatorKind.LeftShift;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.RightShift:
					return BinaryOperatorKind.RightShift;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThan:
					return BinaryOperatorKind.LessThan;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.LessThanOrEqual:
					return BinaryOperatorKind.LessThanOrEqual;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Equals:
				{
					sbyte? b = (sbyte?)leftOpt?.Type?.SpecialType;
					return (b.HasValue ? new bool?(b.GetValueOrDefault() == 1) : null).GetValueOrDefault() ? BinaryOperatorKind.ObjectValueEquals : BinaryOperatorKind.Equals;
				}
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.NotEquals:
				{
					sbyte? b = (sbyte?)leftOpt?.Type?.SpecialType;
					return (b.HasValue ? new bool?(b.GetValueOrDefault() == 1) : null).GetValueOrDefault() ? BinaryOperatorKind.ObjectValueNotEquals : BinaryOperatorKind.NotEquals;
				}
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Is:
					return BinaryOperatorKind.Equals;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.IsNot:
					return BinaryOperatorKind.NotEquals;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThanOrEqual:
					return BinaryOperatorKind.GreaterThanOrEqual;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.GreaterThan:
					return BinaryOperatorKind.GreaterThan;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Power:
					return BinaryOperatorKind.Power;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Like:
					return BinaryOperatorKind.Like;
				case Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Concatenate:
					return BinaryOperatorKind.Concatenate;
				default:
					return BinaryOperatorKind.None;
				}
			}
		}

		private sealed class QueryLambdaRewriterPass1 : BoundTreeRewriterWithStackGuard
		{
			private Dictionary<RangeVariableSymbol, BoundExpression> _rangeVariableMap;

			public QueryLambdaRewriterPass1()
			{
				_rangeVariableMap = null;
			}

			protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
			{
				return false;
			}

			public override BoundNode VisitQueryLambda(BoundQueryLambda node)
			{
				LocalRewriter.PopulateRangeVariableMapForQueryLambdaRewrite(node, ref _rangeVariableMap, inExpressionLambda: true);
				BoundStatement rewrittenBody = LocalRewriter.CreateReturnStatementForQueryLambdaBody(VisitExpressionWithStackGuard(node.Expression), node, (object)node.LambdaSymbol.ReturnType == LambdaSymbol.ReturnTypePendingDelegate);
				LocalRewriter.RemoveRangeVariables(node, _rangeVariableMap);
				return LocalRewriter.RewriteQueryLambda(rewrittenBody, node);
			}

			public override BoundNode VisitRangeVariable(BoundRangeVariable node)
			{
				BoundExpression value = null;
				if (!_rangeVariableMap.TryGetValue(node.RangeVariable, out value))
				{
					return node;
				}
				switch (value.Kind)
				{
				case BoundKind.Parameter:
				{
					BoundParameter boundParameter = (BoundParameter)value;
					value = new BoundParameter(node.Syntax, boundParameter.ParameterSymbol, boundParameter.IsLValue, boundParameter.SuppressVirtualCalls, boundParameter.Type, boundParameter.HasErrors);
					break;
				}
				case BoundKind.PropertyAccess:
				{
					BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)value;
					value = new BoundPropertyAccess(node.Syntax, boundPropertyAccess.PropertySymbol, boundPropertyAccess.PropertyGroupOpt, boundPropertyAccess.AccessKind, boundPropertyAccess.IsWriteable, boundPropertyAccess.IsWriteable, boundPropertyAccess.ReceiverOpt, boundPropertyAccess.Arguments, boundPropertyAccess.DefaultArguments, boundPropertyAccess.Type, boundPropertyAccess.HasErrors);
					break;
				}
				}
				if (node.WasCompilerGenerated)
				{
					value.SetWasCompilerGenerated();
				}
				return value;
			}
		}

		private sealed class QueryLambdaRewriterPass2 : BoundTreeRewriterWithStackGuard
		{
			private readonly HashSet<BoundParameter> _uniqueNodes;

			public QueryLambdaRewriterPass2()
			{
				_uniqueNodes = new HashSet<BoundParameter>();
			}

			protected override bool ConvertInsufficientExecutionStackExceptionToCancelledByStackGuardException()
			{
				return false;
			}

			public override BoundNode VisitParameter(BoundParameter node)
			{
				ParameterSymbol parameterSymbol = node.ParameterSymbol;
				if ((object)parameterSymbol != null && parameterSymbol.ContainingSymbol.IsQueryLambdaMethod && !_uniqueNodes.Add(node))
				{
					bool wasCompilerGenerated = node.WasCompilerGenerated;
					node = new BoundParameter(node.Syntax, node.ParameterSymbol, node.IsLValue, node.SuppressVirtualCalls, node.Type, node.HasErrors);
					if (wasCompilerGenerated)
					{
						BoundNodeExtensions.MakeCompilerGenerated(node);
					}
				}
				return node;
			}
		}

		private ConcurrentDictionary<BoundValuePlaceholderBase, BoundNode> _lazyPlaceholderToParentMap;

		private readonly SemanticModel _semanticModel;

		public VisualBasicOperationFactory(SemanticModel semanticModel)
		{
			_lazyPlaceholderToParentMap = null;
			_semanticModel = semanticModel;
		}

		private BoundNode TryGetParent(BoundValuePlaceholderBase placeholder)
		{
			BoundNode value = null;
			if (_lazyPlaceholderToParentMap != null && _lazyPlaceholderToParentMap.TryGetValue(placeholder, out value))
			{
				return value;
			}
			return null;
		}

		private void RecordParent(BoundValuePlaceholderBase placeholderOpt, BoundNode parent)
		{
			if (placeholderOpt != null)
			{
				if (_lazyPlaceholderToParentMap == null)
				{
					Interlocked.CompareExchange(ref _lazyPlaceholderToParentMap, new ConcurrentDictionary<BoundValuePlaceholderBase, BoundNode>(2, 10, ReferenceEqualityComparer.Instance), null);
				}
				_lazyPlaceholderToParentMap.GetOrAdd(placeholderOpt, parent);
			}
		}

		public IOperation Create(BoundNode boundNode)
		{
			if (boundNode == null)
			{
				return null;
			}
			switch (boundNode.Kind)
			{
			case BoundKind.AssignmentOperator:
				return CreateBoundAssignmentOperatorOperation((BoundAssignmentOperator)boundNode);
			case BoundKind.MeReference:
				return CreateBoundMeReferenceOperation((BoundMeReference)boundNode);
			case BoundKind.MyBaseReference:
				return CreateBoundMyBaseReferenceOperation((BoundMyBaseReference)boundNode);
			case BoundKind.MyClassReference:
				return CreateBoundMyClassReferenceOperation((BoundMyClassReference)boundNode);
			case BoundKind.Literal:
				return CreateBoundLiteralOperation((BoundLiteral)boundNode);
			case BoundKind.AwaitOperator:
				return CreateBoundAwaitOperatorOperation((BoundAwaitOperator)boundNode);
			case BoundKind.NameOfOperator:
				return CreateBoundNameOfOperatorOperation((BoundNameOfOperator)boundNode);
			case BoundKind.Lambda:
				return CreateBoundLambdaOperation((BoundLambda)boundNode);
			case BoundKind.Call:
				return CreateBoundCallOperation((BoundCall)boundNode);
			case BoundKind.OmittedArgument:
				return CreateBoundOmittedArgumentOperation((BoundOmittedArgument)boundNode);
			case BoundKind.Parenthesized:
				return CreateBoundParenthesizedOperation((BoundParenthesized)boundNode);
			case BoundKind.ArrayAccess:
				return CreateBoundArrayAccessOperation((BoundArrayAccess)boundNode);
			case BoundKind.UnaryOperator:
				return CreateBoundUnaryOperatorOperation((BoundUnaryOperator)boundNode);
			case BoundKind.UserDefinedUnaryOperator:
				return CreateBoundUserDefinedUnaryOperatorOperation((BoundUserDefinedUnaryOperator)boundNode);
			case BoundKind.BinaryOperator:
				return CreateBoundBinaryOperatorOperation((BoundBinaryOperator)boundNode);
			case BoundKind.UserDefinedBinaryOperator:
				return CreateBoundUserDefinedBinaryOperatorOperation((BoundUserDefinedBinaryOperator)boundNode);
			case BoundKind.BinaryConditionalExpression:
				return CreateBoundBinaryConditionalExpressionOperation((BoundBinaryConditionalExpression)boundNode);
			case BoundKind.UserDefinedShortCircuitingOperator:
				return CreateBoundUserDefinedShortCircuitingOperatorOperation((BoundUserDefinedShortCircuitingOperator)boundNode);
			case BoundKind.BadExpression:
				return CreateBoundBadExpressionOperation((BoundBadExpression)boundNode);
			case BoundKind.TryCast:
				return CreateBoundTryCastOperation((BoundTryCast)boundNode);
			case BoundKind.DirectCast:
				return CreateBoundDirectCastOperation((BoundDirectCast)boundNode);
			case BoundKind.Conversion:
				return CreateBoundConversionOperation((BoundConversion)boundNode);
			case BoundKind.DelegateCreationExpression:
				return CreateBoundDelegateCreationExpressionOperation((BoundDelegateCreationExpression)boundNode);
			case BoundKind.TernaryConditionalExpression:
				return CreateBoundTernaryConditionalExpressionOperation((BoundTernaryConditionalExpression)boundNode);
			case BoundKind.TypeOf:
				return CreateBoundTypeOfOperation((BoundTypeOf)boundNode);
			case BoundKind.GetType:
				return CreateBoundGetTypeOperation((BoundGetType)boundNode);
			case BoundKind.ObjectCreationExpression:
				return CreateBoundObjectCreationExpressionOperation((BoundObjectCreationExpression)boundNode);
			case BoundKind.ObjectInitializerExpression:
				return CreateBoundObjectInitializerExpressionOperation((BoundObjectInitializerExpression)boundNode);
			case BoundKind.CollectionInitializerExpression:
				return CreateBoundCollectionInitializerExpressionOperation((BoundCollectionInitializerExpression)boundNode);
			case BoundKind.NewT:
				return CreateBoundNewTOperation((BoundNewT)boundNode);
			case BoundKind.NoPiaObjectCreationExpression:
				return CreateNoPiaObjectCreationExpressionOperation((BoundNoPiaObjectCreationExpression)boundNode);
			case BoundKind.ArrayCreation:
				return CreateBoundArrayCreationOperation((BoundArrayCreation)boundNode);
			case BoundKind.ArrayInitialization:
				return CreateBoundArrayInitializationOperation((BoundArrayInitialization)boundNode);
			case BoundKind.PropertyAccess:
				return CreateBoundPropertyAccessOperation((BoundPropertyAccess)boundNode);
			case BoundKind.EventAccess:
				return CreateBoundEventAccessOperation((BoundEventAccess)boundNode);
			case BoundKind.FieldAccess:
				return CreateBoundFieldAccessOperation((BoundFieldAccess)boundNode);
			case BoundKind.ConditionalAccess:
				return CreateBoundConditionalAccessOperation((BoundConditionalAccess)boundNode);
			case BoundKind.ConditionalAccessReceiverPlaceholder:
				return CreateBoundConditionalAccessReceiverPlaceholderOperation((BoundConditionalAccessReceiverPlaceholder)boundNode);
			case BoundKind.Parameter:
				return CreateBoundParameterOperation((BoundParameter)boundNode);
			case BoundKind.Local:
				return CreateBoundLocalOperation((BoundLocal)boundNode);
			case BoundKind.LocalDeclaration:
				return CreateBoundLocalDeclarationOperation((BoundLocalDeclaration)boundNode);
			case BoundKind.LateInvocation:
				return CreateBoundLateInvocationOperation((BoundLateInvocation)boundNode);
			case BoundKind.LateMemberAccess:
				return CreateBoundLateMemberAccessOperation((BoundLateMemberAccess)boundNode);
			case BoundKind.FieldInitializer:
				return CreateBoundFieldInitializerOperation((BoundFieldInitializer)boundNode);
			case BoundKind.PropertyInitializer:
				return CreateBoundPropertyInitializerOperation((BoundPropertyInitializer)boundNode);
			case BoundKind.ParameterEqualsValue:
				return CreateBoundParameterEqualsValueOperation((BoundParameterEqualsValue)boundNode);
			case BoundKind.RValuePlaceholder:
				return CreateBoundRValuePlaceholderOperation((BoundRValuePlaceholder)boundNode);
			case BoundKind.IfStatement:
				return CreateBoundIfStatementOperation((BoundIfStatement)boundNode);
			case BoundKind.SelectStatement:
				return CreateBoundSelectStatementOperation((BoundSelectStatement)boundNode);
			case BoundKind.CaseBlock:
				return CreateBoundCaseBlockOperation((BoundCaseBlock)boundNode);
			case BoundKind.SimpleCaseClause:
				return CreateBoundSimpleCaseClauseOperation((BoundSimpleCaseClause)boundNode);
			case BoundKind.RangeCaseClause:
				return CreateBoundRangeCaseClauseOperation((BoundRangeCaseClause)boundNode);
			case BoundKind.RelationalCaseClause:
				return CreateBoundRelationalCaseClauseOperation((BoundRelationalCaseClause)boundNode);
			case BoundKind.DoLoopStatement:
				return CreateBoundDoLoopStatementOperation((BoundDoLoopStatement)boundNode);
			case BoundKind.ForToStatement:
				return CreateBoundForToStatementOperation((BoundForToStatement)boundNode);
			case BoundKind.ForEachStatement:
				return CreateBoundForEachStatementOperation((BoundForEachStatement)boundNode);
			case BoundKind.TryStatement:
				return CreateBoundTryStatementOperation((BoundTryStatement)boundNode);
			case BoundKind.CatchBlock:
				return CreateBoundCatchBlockOperation((BoundCatchBlock)boundNode);
			case BoundKind.Block:
				return CreateBoundBlockOperation((BoundBlock)boundNode);
			case BoundKind.BadStatement:
				return CreateBoundBadStatementOperation((BoundBadStatement)boundNode);
			case BoundKind.ReturnStatement:
				return CreateBoundReturnStatementOperation((BoundReturnStatement)boundNode);
			case BoundKind.ThrowStatement:
				return CreateBoundThrowStatementOperation((BoundThrowStatement)boundNode);
			case BoundKind.WhileStatement:
				return CreateBoundWhileStatementOperation((BoundWhileStatement)boundNode);
			case BoundKind.DimStatement:
				return CreateBoundDimStatementOperation((BoundDimStatement)boundNode);
			case BoundKind.YieldStatement:
				return CreateBoundYieldStatementOperation((BoundYieldStatement)boundNode);
			case BoundKind.LabelStatement:
				return CreateBoundLabelStatementOperation((BoundLabelStatement)boundNode);
			case BoundKind.GotoStatement:
				return CreateBoundGotoStatementOperation((BoundGotoStatement)boundNode);
			case BoundKind.ContinueStatement:
				return CreateBoundContinueStatementOperation((BoundContinueStatement)boundNode);
			case BoundKind.ExitStatement:
				return CreateBoundExitStatementOperation((BoundExitStatement)boundNode);
			case BoundKind.SyncLockStatement:
				return CreateBoundSyncLockStatementOperation((BoundSyncLockStatement)boundNode);
			case BoundKind.NoOpStatement:
				return CreateBoundNoOpStatementOperation((BoundNoOpStatement)boundNode);
			case BoundKind.StopStatement:
				return CreateBoundStopStatementOperation((BoundStopStatement)boundNode);
			case BoundKind.EndStatement:
				return CreateBoundEndStatementOperation((BoundEndStatement)boundNode);
			case BoundKind.WithStatement:
				return CreateBoundWithStatementOperation((BoundWithStatement)boundNode);
			case BoundKind.UsingStatement:
				return CreateBoundUsingStatementOperation((BoundUsingStatement)boundNode);
			case BoundKind.ExpressionStatement:
				return CreateBoundExpressionStatementOperation((BoundExpressionStatement)boundNode);
			case BoundKind.RaiseEventStatement:
				return CreateBoundRaiseEventStatementOperation((BoundRaiseEventStatement)boundNode);
			case BoundKind.AddHandlerStatement:
				return CreateBoundAddHandlerStatementOperation((BoundAddHandlerStatement)boundNode);
			case BoundKind.RemoveHandlerStatement:
				return CreateBoundRemoveHandlerStatementOperation((BoundRemoveHandlerStatement)boundNode);
			case BoundKind.TupleLiteral:
				return CreateBoundTupleLiteralOperation((BoundTupleLiteral)boundNode);
			case BoundKind.ConvertedTupleLiteral:
				return CreateBoundConvertedTupleLiteralOperation((BoundConvertedTupleLiteral)boundNode);
			case BoundKind.InterpolatedStringExpression:
				return CreateBoundInterpolatedStringExpressionOperation((BoundInterpolatedStringExpression)boundNode);
			case BoundKind.Interpolation:
				return CreateBoundInterpolationOperation((BoundInterpolation)boundNode);
			case BoundKind.AnonymousTypeCreationExpression:
				return CreateBoundAnonymousTypeCreationExpressionOperation((BoundAnonymousTypeCreationExpression)boundNode);
			case BoundKind.AnonymousTypeFieldInitializer:
				return Create(((BoundAnonymousTypeFieldInitializer)boundNode).Value);
			case BoundKind.AnonymousTypePropertyAccess:
				return CreateBoundAnonymousTypePropertyAccessOperation((BoundAnonymousTypePropertyAccess)boundNode);
			case BoundKind.WithLValueExpressionPlaceholder:
				return CreateBoundWithLValueExpressionPlaceholder((BoundWithLValueExpressionPlaceholder)boundNode);
			case BoundKind.WithRValueExpressionPlaceholder:
				return CreateBoundWithRValueExpressionPlaceholder((BoundWithRValueExpressionPlaceholder)boundNode);
			case BoundKind.QueryExpression:
				return CreateBoundQueryExpressionOperation((BoundQueryExpression)boundNode);
			case BoundKind.LValueToRValueWrapper:
				return CreateBoundLValueToRValueWrapper((BoundLValueToRValueWrapper)boundNode);
			case BoundKind.QueryClause:
				return Create(((BoundQueryClause)boundNode).UnderlyingExpression);
			case BoundKind.QueryableSource:
				return Create(((BoundQueryableSource)boundNode).Source);
			case BoundKind.AggregateClause:
				return CreateBoundAggregateClauseOperation((BoundAggregateClause)boundNode);
			case BoundKind.Ordering:
				return Create(((BoundOrdering)boundNode).UnderlyingExpression);
			case BoundKind.GroupAggregation:
				return Create(((BoundGroupAggregation)boundNode).Group);
			case BoundKind.QuerySource:
				return Create(((BoundQuerySource)boundNode).Expression);
			case BoundKind.ToQueryableCollectionConversion:
				return Create(((BoundToQueryableCollectionConversion)boundNode).ConversionCall);
			case BoundKind.QueryLambda:
			{
				BoundNode boundNode2 = RewriteQueryLambda((BoundQueryLambda)boundNode);
				return Create(boundNode2);
			}
			case BoundKind.RangeVariableAssignment:
				return Create(((BoundRangeVariableAssignment)boundNode).Value);
			case BoundKind.BadVariable:
				return Create(((BoundBadVariable)boundNode).Expression);
			case BoundKind.NullableIsTrueOperator:
				return CreateBoundNullableIsTrueOperator((BoundNullableIsTrueOperator)boundNode);
			case BoundKind.RedimStatement:
				return CreateBoundReDimOperation((BoundRedimStatement)boundNode);
			case BoundKind.RedimClause:
				return CreateBoundReDimClauseOperation((BoundRedimClause)boundNode);
			case BoundKind.TypeArguments:
				return CreateBoundTypeArgumentsOperation((BoundTypeArguments)boundNode);
			case BoundKind.TypeExpression:
			case BoundKind.TypeOrValueExpression:
			case BoundKind.NamespaceExpression:
			case BoundKind.CompoundAssignmentTargetPlaceholder:
			case BoundKind.AddressOfOperator:
			case BoundKind.MethodGroup:
			case BoundKind.PropertyGroup:
			case BoundKind.EraseStatement:
			case BoundKind.Attribute:
			case BoundKind.LateAddressOfOperator:
			case BoundKind.ArrayLiteral:
			case BoundKind.ByRefArgumentWithCopyBack:
			case BoundKind.Label:
			case BoundKind.UnboundLambda:
			case BoundKind.RangeVariable:
			case BoundKind.XmlNamespace:
			case BoundKind.XmlDocument:
			case BoundKind.XmlProcessingInstruction:
			case BoundKind.XmlComment:
			case BoundKind.XmlElement:
			case BoundKind.XmlMemberAccess:
			case BoundKind.XmlEmbeddedExpression:
			case BoundKind.XmlCData:
			case BoundKind.ResumeStatement:
			case BoundKind.OnErrorStatement:
			case BoundKind.UnstructuredExceptionHandlingStatement:
			case BoundKind.MidResult:
			case BoundKind.TypeAsValueExpression:
			{
				ConstantValue constantValue = (boundNode as BoundExpression)?.ConstantValueOpt;
				bool wasCompilerGenerated = boundNode.WasCompilerGenerated;
				return new NoneOperation(GetIOperationChildren(boundNode), _semanticModel, boundNode.Syntax, null, constantValue, wasCompilerGenerated);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(boundNode.Kind);
			}
		}

		public ImmutableArray<TOperation> CreateFromArray<TBoundNode, TOperation>(ImmutableArray<TBoundNode> nodeArray) where TBoundNode : BoundNode where TOperation : class, IOperation
		{
			ArrayBuilder<TOperation> instance = ArrayBuilder<TOperation>.GetInstance(nodeArray.Length);
			ImmutableArray<TBoundNode>.Enumerator enumerator = nodeArray.GetEnumerator();
			while (enumerator.MoveNext())
			{
				TBoundNode current = enumerator.Current;
				instance.AddIfNotNull((TOperation)Create(current));
			}
			return instance.ToImmutableAndFree();
		}

		internal ImmutableArray<IOperation> GetIOperationChildren(BoundNode boundNode)
		{
			if (((IBoundNodeWithIOperationChildren)boundNode).Children.IsDefaultOrEmpty)
			{
				return ImmutableArray<IOperation>.Empty;
			}
			ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(((IBoundNodeWithIOperationChildren)boundNode).Children.Length);
			ImmutableArray<BoundNode>.Enumerator enumerator = ((IBoundNodeWithIOperationChildren)boundNode).Children.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundNode current = enumerator.Current;
				IOperation operation = Create(current);
				if (operation != null)
				{
					instance.Add(operation);
				}
			}
			return instance.ToImmutableAndFree();
		}

		private IOperation CreateBoundAssignmentOperatorOperation(BoundAssignmentOperator boundAssignmentOperator)
		{
			if (IsMidStatement(boundAssignmentOperator.Right))
			{
				ConstantValue constantValueOpt = boundAssignmentOperator.ConstantValueOpt;
				bool wasCompilerGenerated = boundAssignmentOperator.WasCompilerGenerated;
				return new NoneOperation(GetIOperationChildren(boundAssignmentOperator), _semanticModel, boundAssignmentOperator.Syntax, null, constantValueOpt, wasCompilerGenerated);
			}
			if (boundAssignmentOperator.LeftOnTheRightOpt != null)
			{
				return CreateCompoundAssignment(boundAssignmentOperator);
			}
			IOperation target = Create(boundAssignmentOperator.Left);
			IOperation value = Create(boundAssignmentOperator.Right);
			bool wasCompilerGenerated2 = boundAssignmentOperator.WasCompilerGenerated;
			return new SimpleAssignmentOperation(isRef: false, syntax: boundAssignmentOperator.Syntax, type: boundAssignmentOperator.Type, constantValue: boundAssignmentOperator.ConstantValueOpt, target: target, value: value, semanticModel: _semanticModel, isImplicit: wasCompilerGenerated2);
		}

		private IInstanceReferenceOperation CreateBoundMeReferenceOperation(BoundMeReference boundMeReference)
		{
			return new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, syntax: boundMeReference.Syntax, type: boundMeReference.Type, isImplicit: boundMeReference.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IInstanceReferenceOperation CreateBoundMyBaseReferenceOperation(BoundMyBaseReference boundMyBaseReference)
		{
			return new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, syntax: boundMyBaseReference.Syntax, type: boundMyBaseReference.Type, isImplicit: boundMyBaseReference.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IInstanceReferenceOperation CreateBoundMyClassReferenceOperation(BoundMyClassReference boundMyClassReference)
		{
			return new InstanceReferenceOperation(InstanceReferenceKind.ContainingTypeInstance, syntax: boundMyClassReference.Syntax, type: boundMyClassReference.Type, isImplicit: boundMyClassReference.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		internal ILiteralOperation CreateBoundLiteralOperation(BoundLiteral boundLiteral, bool @implicit = false)
		{
			SyntaxNode syntax = boundLiteral.Syntax;
			ITypeSymbol type = boundLiteral.Type;
			ConstantValue constantValueOpt = boundLiteral.ConstantValueOpt;
			bool isImplicit = boundLiteral.WasCompilerGenerated || @implicit;
			return new LiteralOperation(_semanticModel, syntax, type, constantValueOpt, isImplicit);
		}

		private IAwaitOperation CreateBoundAwaitOperatorOperation(BoundAwaitOperator boundAwaitOperator)
		{
			return new AwaitOperation(Create(boundAwaitOperator.Operand), syntax: boundAwaitOperator.Syntax, type: boundAwaitOperator.Type, isImplicit: boundAwaitOperator.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private INameOfOperation CreateBoundNameOfOperatorOperation(BoundNameOfOperator boundNameOfOperator)
		{
			return new NameOfOperation(Create(boundNameOfOperator.Argument), syntax: boundNameOfOperator.Syntax, type: boundNameOfOperator.Type, constantValue: boundNameOfOperator.ConstantValueOpt, isImplicit: boundNameOfOperator.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IAnonymousFunctionOperation CreateBoundLambdaOperation(BoundLambda boundLambda)
		{
			return new AnonymousFunctionOperation(boundLambda.LambdaSymbol, (IBlockOperation)Create(boundLambda.Body), syntax: boundLambda.Syntax, isImplicit: boundLambda.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IInvocationOperation CreateBoundCallOperation(BoundCall boundCall)
		{
			IMethodSymbol method = boundCall.Method;
			int num;
			if (method != null && (method.IsVirtual || method.IsAbstract || method.IsOverride))
			{
				BoundExpression receiverOpt = boundCall.ReceiverOpt;
				if (receiverOpt != null && receiverOpt.Kind != BoundKind.MyBaseReference)
				{
					BoundExpression receiverOpt2 = boundCall.ReceiverOpt;
					num = ((receiverOpt2 != null && receiverOpt2.Kind != BoundKind.MyClassReference) ? 1 : 0);
					goto IL_0059;
				}
			}
			num = 0;
			goto IL_0059;
			IL_0059:
			bool isVirtual = (byte)num != 0;
			BoundExpression node = boundCall.ReceiverOpt ?? boundCall.MethodGroupOpt?.ReceiverOpt;
			IOperation instance = CreateReceiverOperation(node, method);
			ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundCall);
			SyntaxNode syntax = boundCall.Syntax;
			ITypeSymbol type = boundCall.Type;
			bool wasCompilerGenerated = boundCall.WasCompilerGenerated;
			return new InvocationOperation(method, instance, isVirtual, arguments, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IOmittedArgumentOperation CreateBoundOmittedArgumentOperation(BoundOmittedArgument boundOmittedArgument)
		{
			SyntaxNode syntax = boundOmittedArgument.Syntax;
			ITypeSymbol type = boundOmittedArgument.Type;
			bool wasCompilerGenerated = boundOmittedArgument.WasCompilerGenerated;
			return new OmittedArgumentOperation(_semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IParenthesizedOperation CreateBoundParenthesizedOperation(BoundParenthesized boundParenthesized)
		{
			return new ParenthesizedOperation(Create(boundParenthesized.Expression), syntax: boundParenthesized.Syntax, type: boundParenthesized.Type, constantValue: boundParenthesized.ConstantValueOpt, isImplicit: boundParenthesized.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IArrayElementReferenceOperation CreateBoundArrayAccessOperation(BoundArrayAccess boundArrayAccess)
		{
			return new ArrayElementReferenceOperation(Create(boundArrayAccess.Expression), CreateFromArray<BoundExpression, IOperation>(boundArrayAccess.Indices), syntax: boundArrayAccess.Syntax, type: boundArrayAccess.Type, isImplicit: boundArrayAccess.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		internal IOperation CreateBoundUnaryOperatorChild(BoundExpression boundOperator)
		{
			switch (boundOperator.Kind)
			{
			case BoundKind.UnaryOperator:
				return Create(((BoundUnaryOperator)boundOperator).Operand);
			case BoundKind.UserDefinedUnaryOperator:
			{
				BoundUserDefinedUnaryOperator boundUserDefinedUnaryOperator = (BoundUserDefinedUnaryOperator)boundOperator;
				if (boundUserDefinedUnaryOperator.UnderlyingExpression.Kind == BoundKind.Call)
				{
					return Create(boundUserDefinedUnaryOperator.Operand);
				}
				return GetChildOfBadExpression(boundUserDefinedUnaryOperator.UnderlyingExpression, 0);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(boundOperator.Kind);
			}
		}

		private IUnaryOperation CreateBoundUnaryOperatorOperation(BoundUnaryOperator boundUnaryOperator)
		{
			IOperation operand = CreateBoundUnaryOperatorChild(boundUnaryOperator);
			return new UnaryOperation(Helper.DeriveUnaryOperatorKind(boundUnaryOperator.OperatorKind), operatorMethod: null, syntax: boundUnaryOperator.Syntax, type: boundUnaryOperator.Type, constantValue: boundUnaryOperator.ConstantValueOpt, isLifted: (boundUnaryOperator.OperatorKind & Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Lifted) != 0, isChecked: boundUnaryOperator.Checked, isImplicit: boundUnaryOperator.WasCompilerGenerated, operand: operand, semanticModel: _semanticModel);
		}

		private IUnaryOperation CreateBoundUserDefinedUnaryOperatorOperation(BoundUserDefinedUnaryOperator boundUserDefinedUnaryOperator)
		{
			IOperation operand = CreateBoundUnaryOperatorChild(boundUserDefinedUnaryOperator);
			return new UnaryOperation(Helper.DeriveUnaryOperatorKind(boundUserDefinedUnaryOperator.OperatorKind), operatorMethod: TryGetOperatorMethod(boundUserDefinedUnaryOperator), syntax: boundUserDefinedUnaryOperator.Syntax, type: boundUserDefinedUnaryOperator.Type, constantValue: boundUserDefinedUnaryOperator.ConstantValueOpt, isLifted: (boundUserDefinedUnaryOperator.OperatorKind & Microsoft.CodeAnalysis.VisualBasic.UnaryOperatorKind.Lifted) != 0, isChecked: false, isImplicit: boundUserDefinedUnaryOperator.WasCompilerGenerated, operand: operand, semanticModel: _semanticModel);
		}

		private static MethodSymbol TryGetOperatorMethod(BoundUserDefinedUnaryOperator boundUserDefinedUnaryOperator)
		{
			if (boundUserDefinedUnaryOperator.UnderlyingExpression.Kind != BoundKind.Call)
			{
				return null;
			}
			return boundUserDefinedUnaryOperator.Call.Method;
		}

		internal IOperation CreateBoundBinaryOperatorChild(BoundExpression binaryOperator, bool isLeft)
		{
			switch (binaryOperator.Kind)
			{
			case BoundKind.UserDefinedBinaryOperator:
			{
				BoundUserDefinedBinaryOperator boundUserDefinedBinaryOperator = (BoundUserDefinedBinaryOperator)binaryOperator;
				BinaryOperatorInfo userDefinedBinaryOperatorInfo2 = GetUserDefinedBinaryOperatorInfo(boundUserDefinedBinaryOperator);
				return GetUserDefinedBinaryOperatorChild(boundUserDefinedBinaryOperator, isLeft ? userDefinedBinaryOperatorInfo2.LeftOperand : userDefinedBinaryOperatorInfo2.RightOperand);
			}
			case BoundKind.UserDefinedShortCircuitingOperator:
			{
				BoundUserDefinedShortCircuitingOperator boundUserDefinedShortCircuitingOperator = (BoundUserDefinedShortCircuitingOperator)binaryOperator;
				BinaryOperatorInfo userDefinedBinaryOperatorInfo = GetUserDefinedBinaryOperatorInfo(boundUserDefinedShortCircuitingOperator.BitwiseOperator);
				if (isLeft)
				{
					return (boundUserDefinedShortCircuitingOperator.LeftOperand != null) ? Create(boundUserDefinedShortCircuitingOperator.LeftOperand) : GetUserDefinedBinaryOperatorChild(boundUserDefinedShortCircuitingOperator.BitwiseOperator, userDefinedBinaryOperatorInfo.LeftOperand);
				}
				return GetUserDefinedBinaryOperatorChild(boundUserDefinedShortCircuitingOperator.BitwiseOperator, userDefinedBinaryOperatorInfo.RightOperand);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(binaryOperator.Kind);
			}
		}

		private IOperation CreateBoundBinaryOperatorOperation(BoundBinaryOperator boundBinaryOperator)
		{
			ArrayBuilder<BoundBinaryOperator> instance = ArrayBuilder<BoundBinaryOperator>.GetInstance();
			BoundBinaryOperator result = boundBinaryOperator;
			do
			{
				instance.Push(result);
				result = result.Left as BoundBinaryOperator;
			}
			while (result != null);
			IOperation operation = null;
			while (instance.TryPop(out result))
			{
				operation = operation ?? Create(result.Left);
				IOperation rightOperand = Create(result.Right);
				BinaryOperatorInfo binaryOperatorInfo = GetBinaryOperatorInfo(result);
				SyntaxNode syntax = result.Syntax;
				ITypeSymbol type = result.Type;
				ConstantValue constantValueOpt = result.ConstantValueOpt;
				bool wasCompilerGenerated = result.WasCompilerGenerated;
				operation = new BinaryOperation(binaryOperatorInfo.OperatorKind, operation, rightOperand, binaryOperatorInfo.IsLifted, binaryOperatorInfo.IsChecked, binaryOperatorInfo.IsCompareText, binaryOperatorInfo.OperatorMethod, null, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
			}
			instance.Free();
			return operation;
		}

		private IBinaryOperation CreateBoundUserDefinedBinaryOperatorOperation(BoundUserDefinedBinaryOperator boundUserDefinedBinaryOperator)
		{
			IOperation leftOperand = CreateBoundBinaryOperatorChild(boundUserDefinedBinaryOperator, isLeft: true);
			IOperation rightOperand = CreateBoundBinaryOperatorChild(boundUserDefinedBinaryOperator, isLeft: false);
			BinaryOperatorInfo userDefinedBinaryOperatorInfo = GetUserDefinedBinaryOperatorInfo(boundUserDefinedBinaryOperator);
			SyntaxNode syntax = boundUserDefinedBinaryOperator.Syntax;
			ITypeSymbol type = boundUserDefinedBinaryOperator.Type;
			ConstantValue constantValueOpt = boundUserDefinedBinaryOperator.ConstantValueOpt;
			bool wasCompilerGenerated = boundUserDefinedBinaryOperator.WasCompilerGenerated;
			return new BinaryOperation(userDefinedBinaryOperatorInfo.OperatorKind, leftOperand, rightOperand, userDefinedBinaryOperatorInfo.IsLifted, userDefinedBinaryOperatorInfo.IsChecked, userDefinedBinaryOperatorInfo.IsCompareText, userDefinedBinaryOperatorInfo.OperatorMethod, null, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private ICoalesceOperation CreateBoundBinaryConditionalExpressionOperation(BoundBinaryConditionalExpression boundBinaryConditionalExpression)
		{
			IOperation value = Create(boundBinaryConditionalExpression.TestExpression);
			IOperation whenNull = Create(boundBinaryConditionalExpression.ElseExpression);
			SyntaxNode syntax = boundBinaryConditionalExpression.Syntax;
			ITypeSymbol type = boundBinaryConditionalExpression.Type;
			ConstantValue constantValueOpt = boundBinaryConditionalExpression.ConstantValueOpt;
			bool wasCompilerGenerated = boundBinaryConditionalExpression.WasCompilerGenerated;
			Conversion conversion = new Conversion(Conversions.Identity);
			if (!TypeSymbol.Equals(boundBinaryConditionalExpression.Type, boundBinaryConditionalExpression.TestExpression.Type, TypeCompareKind.ConsiderEverything))
			{
				BoundExpression convertedTestExpression = boundBinaryConditionalExpression.ConvertedTestExpression;
				if (convertedTestExpression != null)
				{
					conversion = ((convertedTestExpression.Kind != BoundKind.Conversion) ? default(Conversion) : CreateConversion(convertedTestExpression));
				}
			}
			return new CoalesceOperation(value, whenNull, conversion, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private IBinaryOperation CreateBoundUserDefinedShortCircuitingOperatorOperation(BoundUserDefinedShortCircuitingOperator boundUserDefinedShortCircuitingOperator)
		{
			IOperation leftOperand = CreateBoundBinaryOperatorChild(boundUserDefinedShortCircuitingOperator, isLeft: true);
			IOperation rightOperand = CreateBoundBinaryOperatorChild(boundUserDefinedShortCircuitingOperator, isLeft: false);
			BinaryOperatorInfo userDefinedBinaryOperatorInfo = GetUserDefinedBinaryOperatorInfo(boundUserDefinedShortCircuitingOperator.BitwiseOperator);
			int operatorKind = ((userDefinedBinaryOperatorInfo.OperatorKind == BinaryOperatorKind.And) ? 13 : 14);
			SyntaxNode syntax = boundUserDefinedShortCircuitingOperator.Syntax;
			ITypeSymbol type = boundUserDefinedShortCircuitingOperator.Type;
			ConstantValue constantValueOpt = boundUserDefinedShortCircuitingOperator.ConstantValueOpt;
			bool isChecked = false;
			bool isCompareText = false;
			bool wasCompilerGenerated = boundUserDefinedShortCircuitingOperator.WasCompilerGenerated;
			IMethodSymbol unaryOperatorMethod = null;
			BoundExpression leftTest = boundUserDefinedShortCircuitingOperator.LeftTest;
			if (leftTest != null)
			{
				unaryOperatorMethod = TryGetOperatorMethod((BoundUserDefinedUnaryOperator)((leftTest.Kind == BoundKind.UserDefinedUnaryOperator) ? leftTest : ((BoundNullableIsTrueOperator)leftTest).Operand));
			}
			return new BinaryOperation((BinaryOperatorKind)operatorKind, leftOperand, rightOperand, userDefinedBinaryOperatorInfo.IsLifted, isChecked, isCompareText, userDefinedBinaryOperatorInfo.OperatorMethod, unaryOperatorMethod, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private IInvalidOperation CreateBoundBadExpressionOperation(BoundBadExpression boundBadExpression)
		{
			SyntaxNode syntax = boundBadExpression.Syntax;
			ITypeSymbol type = (syntax.IsMissing ? null : boundBadExpression.Type);
			ConstantValue constantValueOpt = boundBadExpression.ConstantValueOpt;
			bool isImplicit = boundBadExpression.WasCompilerGenerated || boundBadExpression.ChildBoundNodes.Any((BoundExpression e) => e?.Syntax == boundBadExpression.Syntax);
			return new InvalidOperation(CreateFromArray<BoundExpression, IOperation>(boundBadExpression.ChildBoundNodes), _semanticModel, syntax, type, constantValueOpt, isImplicit);
		}

		private IInvalidOperation CreateBoundTypeArgumentsOperation(BoundTypeArguments boundTypeArguments)
		{
			SyntaxNode syntax = boundTypeArguments.Syntax;
			ITypeSymbol type = null;
			ConstantValue constantValueOpt = boundTypeArguments.ConstantValueOpt;
			bool wasCompilerGenerated = boundTypeArguments.WasCompilerGenerated;
			return new InvalidOperation(ImmutableArray<IOperation>.Empty, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private IOperation CreateBoundTryCastOperation(BoundTryCast boundTryCast)
		{
			return CreateBoundConversionOrCastOperation(boundTryCast, isTryCast: true);
		}

		private IOperation CreateBoundDirectCastOperation(BoundDirectCast boundDirectCast)
		{
			return CreateBoundConversionOrCastOperation(boundDirectCast, isTryCast: false);
		}

		private IOperation CreateBoundConversionOperation(BoundConversion boundConversion)
		{
			if (boundConversion.Syntax.IsMissing && boundConversion.Operand.Kind == BoundKind.BadExpression)
			{
				return Create(boundConversion.Operand);
			}
			return CreateBoundConversionOrCastOperation(boundConversion, isTryCast: false);
		}

		private IOperation CreateBoundConversionOrCastOperation(BoundConversionOrCast boundConversionOrCast, bool isTryCast)
		{
			bool isChecked = false;
			ITypeSymbol type = boundConversionOrCast.Type;
			ConstantValue constantValueOpt = boundConversionOrCast.ConstantValueOpt;
			SyntaxNode syntax = boundConversionOrCast.Syntax;
			bool isImplicit = boundConversionOrCast.WasCompilerGenerated || !boundConversionOrCast.ExplicitCastInCode;
			BoundExpression conversionOperand = GetConversionOperand(boundConversionOrCast);
			if (conversionOperand.Syntax == boundConversionOrCast.Syntax)
			{
				if (conversionOperand.Kind == BoundKind.ConvertedTupleLiteral && TypeSymbol.Equals(conversionOperand.Type, boundConversionOrCast.Type, TypeCompareKind.ConsiderEverything))
				{
					return Create(conversionOperand);
				}
				isImplicit = true;
			}
			(IOperation, Conversion, bool) conversionInfo = GetConversionInfo(boundConversionOrCast);
			Conversion item = conversionInfo.Item2;
			if (conversionInfo.Item3)
			{
				return new DelegateCreationOperation(conversionInfo.Item1, _semanticModel, syntax, type, isImplicit);
			}
			return new ConversionOperation(conversionInfo.Item1, item, isTryCast, isChecked, _semanticModel, syntax, type, constantValueOpt, isImplicit);
		}

		private IDelegateCreationOperation CreateBoundDelegateCreationExpressionOperation(BoundDelegateCreationExpression boundDelegateCreationExpression)
		{
			IMethodReferenceOperation target = CreateBoundDelegateCreationExpressionChildOperation(boundDelegateCreationExpression);
			SyntaxNode syntax = boundDelegateCreationExpression.Syntax;
			ITypeSymbol type = boundDelegateCreationExpression.Type;
			bool isImplicit = true;
			return new DelegateCreationOperation(target, _semanticModel, syntax, type, isImplicit);
		}

		internal IMethodReferenceOperation CreateBoundDelegateCreationExpressionChildOperation(BoundDelegateCreationExpression boundDelegateCreationExpression)
		{
			IMethodSymbol method = boundDelegateCreationExpression.Method;
			bool isVirtual = method != null && (method.IsAbstract || method.IsOverride || method.IsVirtual) && !boundDelegateCreationExpression.SuppressVirtualCalls;
			IOperation instance = CreateReceiverOperation(boundDelegateCreationExpression.ReceiverOpt ?? boundDelegateCreationExpression.MethodGroupOpt?.ReceiverOpt, method);
			SyntaxNode syntax = boundDelegateCreationExpression.Syntax;
			ITypeSymbol type = null;
			bool wasCompilerGenerated = boundDelegateCreationExpression.WasCompilerGenerated;
			return new MethodReferenceOperation(method, isVirtual, instance, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IConditionalOperation CreateBoundTernaryConditionalExpressionOperation(BoundTernaryConditionalExpression boundTernaryConditionalExpression)
		{
			IOperation condition = Create(boundTernaryConditionalExpression.Condition);
			IOperation whenTrue = Create(boundTernaryConditionalExpression.WhenTrue);
			IOperation whenFalse = Create(boundTernaryConditionalExpression.WhenFalse);
			SyntaxNode syntax = boundTernaryConditionalExpression.Syntax;
			ITypeSymbol type = boundTernaryConditionalExpression.Type;
			ConstantValue constantValueOpt = boundTernaryConditionalExpression.ConstantValueOpt;
			bool wasCompilerGenerated = boundTernaryConditionalExpression.WasCompilerGenerated;
			bool isRef = false;
			return new ConditionalOperation(condition, whenTrue, whenFalse, isRef, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private IIsTypeOperation CreateBoundTypeOfOperation(BoundTypeOf boundTypeOf)
		{
			return new IsTypeOperation(Create(boundTypeOf.Operand), boundTypeOf.TargetType, boundTypeOf.IsTypeOfIsNotExpression, syntax: boundTypeOf.Syntax, type: boundTypeOf.Type, isImplicit: boundTypeOf.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private ITypeOfOperation CreateBoundGetTypeOperation(BoundGetType boundGetType)
		{
			return new TypeOfOperation(boundGetType.SourceType.Type, syntax: boundGetType.Syntax, type: boundGetType.Type, isImplicit: boundGetType.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IOperation CreateBoundLateInvocationOperation(BoundLateInvocation boundLateInvocation)
		{
			return new DynamicInvocationOperation(Create(boundLateInvocation.Member), CreateFromArray<BoundExpression, IOperation>(boundLateInvocation.ArgumentsOpt), boundLateInvocation.ArgumentNamesOpt, default(ImmutableArray<RefKind>), syntax: boundLateInvocation.Syntax, type: boundLateInvocation.Type, isImplicit: boundLateInvocation.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IObjectCreationOperation CreateBoundObjectCreationExpressionOperation(BoundObjectCreationExpression boundObjectCreationExpression)
		{
			return new ObjectCreationOperation(boundObjectCreationExpression.ConstructorOpt, (IObjectOrCollectionInitializerOperation)Create(boundObjectCreationExpression.InitializerOpt), DeriveArguments(boundObjectCreationExpression), syntax: boundObjectCreationExpression.Syntax, type: boundObjectCreationExpression.Type, constantValue: boundObjectCreationExpression.ConstantValueOpt, isImplicit: boundObjectCreationExpression.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IObjectOrCollectionInitializerOperation CreateBoundObjectInitializerExpressionOperation(BoundObjectInitializerExpression boundObjectInitializerExpression)
		{
			return new ObjectOrCollectionInitializerOperation(CreateFromArray<BoundExpression, IOperation>(boundObjectInitializerExpression.Initializers), syntax: boundObjectInitializerExpression.Syntax, type: boundObjectInitializerExpression.Type, isImplicit: boundObjectInitializerExpression.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IObjectOrCollectionInitializerOperation CreateBoundCollectionInitializerExpressionOperation(BoundCollectionInitializerExpression boundCollectionInitializerExpression)
		{
			return new ObjectOrCollectionInitializerOperation(CreateFromArray<BoundExpression, IOperation>(boundCollectionInitializerExpression.Initializers), syntax: boundCollectionInitializerExpression.Syntax, type: boundCollectionInitializerExpression.Type, isImplicit: boundCollectionInitializerExpression.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private ITypeParameterObjectCreationOperation CreateBoundNewTOperation(BoundNewT boundNewT)
		{
			return new TypeParameterObjectCreationOperation((IObjectOrCollectionInitializerOperation)Create(boundNewT.InitializerOpt), syntax: boundNewT.Syntax, type: boundNewT.Type, isImplicit: boundNewT.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private INoPiaObjectCreationOperation CreateNoPiaObjectCreationExpressionOperation(BoundNoPiaObjectCreationExpression creation)
		{
			return new NoPiaObjectCreationOperation((IObjectOrCollectionInitializerOperation)Create(creation.InitializerOpt), syntax: creation.Syntax, type: creation.Type, isImplicit: creation.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IArrayCreationOperation CreateBoundArrayCreationOperation(BoundArrayCreation boundArrayCreation)
		{
			return new ArrayCreationOperation(CreateFromArray<BoundExpression, IOperation>(boundArrayCreation.Bounds), (IArrayInitializerOperation)Create(boundArrayCreation.InitializerOpt), syntax: boundArrayCreation.Syntax, type: boundArrayCreation.Type, isImplicit: boundArrayCreation.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IArrayInitializerOperation CreateBoundArrayInitializationOperation(BoundArrayInitialization boundArrayInitialization)
		{
			return new ArrayInitializerOperation(CreateFromArray<BoundExpression, IOperation>(boundArrayInitialization.Initializers), syntax: boundArrayInitialization.Syntax, isImplicit: boundArrayInitialization.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IPropertyReferenceOperation CreateBoundPropertyAccessOperation(BoundPropertyAccess boundPropertyAccess)
		{
			IPropertySymbol propertySymbol = boundPropertyAccess.PropertySymbol;
			IOperation instance = CreateReceiverOperation(boundPropertyAccess.ReceiverOpt ?? boundPropertyAccess.PropertyGroupOpt?.ReceiverOpt, propertySymbol);
			ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundPropertyAccess);
			SyntaxNode syntax = boundPropertyAccess.Syntax;
			ITypeSymbol type = boundPropertyAccess.Type;
			bool wasCompilerGenerated = boundPropertyAccess.WasCompilerGenerated;
			return new PropertyReferenceOperation(propertySymbol, arguments, instance, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IInstanceReferenceOperation CreateBoundWithLValueExpressionPlaceholder(BoundWithLValueExpressionPlaceholder boundWithLValueExpressionPlaceholder)
		{
			return new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, syntax: boundWithLValueExpressionPlaceholder.Syntax, type: boundWithLValueExpressionPlaceholder.Type, isImplicit: boundWithLValueExpressionPlaceholder.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IInstanceReferenceOperation CreateBoundWithRValueExpressionPlaceholder(BoundWithRValueExpressionPlaceholder boundWithRValueExpressionPlaceholder)
		{
			return new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, syntax: boundWithRValueExpressionPlaceholder.Syntax, type: boundWithRValueExpressionPlaceholder.Type, isImplicit: boundWithRValueExpressionPlaceholder.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IEventReferenceOperation CreateBoundEventAccessOperation(BoundEventAccess boundEventAccess)
		{
			IEventSymbol eventSymbol = boundEventAccess.EventSymbol;
			IOperation instance = CreateReceiverOperation(boundEventAccess.ReceiverOpt, eventSymbol);
			SyntaxNode syntax = boundEventAccess.Syntax;
			ITypeSymbol type = boundEventAccess.Type;
			bool wasCompilerGenerated = boundEventAccess.WasCompilerGenerated;
			return new EventReferenceOperation(eventSymbol, instance, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IFieldReferenceOperation CreateBoundFieldAccessOperation(BoundFieldAccess boundFieldAccess)
		{
			IFieldSymbol fieldSymbol = boundFieldAccess.FieldSymbol;
			bool isDeclaration = false;
			IOperation instance = CreateReceiverOperation(boundFieldAccess.ReceiverOpt, fieldSymbol);
			SyntaxNode syntax = boundFieldAccess.Syntax;
			ITypeSymbol type = boundFieldAccess.Type;
			ConstantValue constantValueOpt = boundFieldAccess.ConstantValueOpt;
			bool wasCompilerGenerated = boundFieldAccess.WasCompilerGenerated;
			return new FieldReferenceOperation(fieldSymbol, isDeclaration, instance, _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private IConditionalAccessOperation CreateBoundConditionalAccessOperation(BoundConditionalAccess boundConditionalAccess)
		{
			RecordParent(boundConditionalAccess.Placeholder, boundConditionalAccess);
			return new ConditionalAccessOperation(Create(boundConditionalAccess.Receiver), Create(boundConditionalAccess.AccessExpression), syntax: boundConditionalAccess.Syntax, type: boundConditionalAccess.Type, isImplicit: boundConditionalAccess.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IConditionalAccessInstanceOperation CreateBoundConditionalAccessReceiverPlaceholderOperation(BoundConditionalAccessReceiverPlaceholder boundConditionalAccessReceiverPlaceholder)
		{
			SyntaxNode syntax = boundConditionalAccessReceiverPlaceholder.Syntax;
			ITypeSymbol type = boundConditionalAccessReceiverPlaceholder.Type;
			bool wasCompilerGenerated = boundConditionalAccessReceiverPlaceholder.WasCompilerGenerated;
			return new ConditionalAccessInstanceOperation(_semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IParameterReferenceOperation CreateBoundParameterOperation(BoundParameter boundParameter)
		{
			return new ParameterReferenceOperation(boundParameter.ParameterSymbol, syntax: boundParameter.Syntax, type: boundParameter.Type, isImplicit: boundParameter.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IOperation CreateBoundLocalOperation(BoundLocal boundLocal)
		{
			return new LocalReferenceOperation(boundLocal.LocalSymbol, isDeclaration: false, syntax: boundLocal.Syntax, type: boundLocal.Type, constantValue: boundLocal.ConstantValueOpt, isImplicit: boundLocal.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IDynamicMemberReferenceOperation CreateBoundLateMemberAccessOperation(BoundLateMemberAccess boundLateMemberAccess)
		{
			IOperation instance = Create(boundLateMemberAccess.ReceiverOpt);
			string nameOpt = boundLateMemberAccess.NameOpt;
			ImmutableArray<ITypeSymbol> typeArguments = ImmutableArray<ITypeSymbol>.Empty;
			if (boundLateMemberAccess.TypeArgumentsOpt != null)
			{
				typeArguments = ImmutableArray<ITypeSymbol>.CastUp(boundLateMemberAccess.TypeArgumentsOpt.Arguments);
			}
			ITypeSymbol containingType = null;
			if ((object)boundLateMemberAccess.ContainerTypeOpt != null && (boundLateMemberAccess.ReceiverOpt == null || !TypeSymbol.Equals(boundLateMemberAccess.ContainerTypeOpt, boundLateMemberAccess.ReceiverOpt.Type, TypeCompareKind.ConsiderEverything)))
			{
				containingType = boundLateMemberAccess.ContainerTypeOpt;
			}
			return new DynamicMemberReferenceOperation(syntax: boundLateMemberAccess.Syntax, type: boundLateMemberAccess.Type, isImplicit: boundLateMemberAccess.WasCompilerGenerated, instance: instance, memberName: nameOpt, typeArguments: typeArguments, containingType: containingType, semanticModel: _semanticModel);
		}

		private IFieldInitializerOperation CreateBoundFieldInitializerOperation(BoundFieldInitializer boundFieldInitializer)
		{
			return new FieldInitializerOperation(boundFieldInitializer.InitializedFields.As<IFieldSymbol>(), value: Create(boundFieldInitializer.InitialValue), syntax: boundFieldInitializer.Syntax, isImplicit: boundFieldInitializer.WasCompilerGenerated, locals: ImmutableArray<ILocalSymbol>.Empty, semanticModel: _semanticModel);
		}

		private IPropertyInitializerOperation CreateBoundPropertyInitializerOperation(BoundPropertyInitializer boundPropertyInitializer)
		{
			return new PropertyInitializerOperation(boundPropertyInitializer.InitializedProperties.As<IPropertySymbol>(), value: Create(boundPropertyInitializer.InitialValue), syntax: boundPropertyInitializer.Syntax, isImplicit: boundPropertyInitializer.WasCompilerGenerated, locals: ImmutableArray<ILocalSymbol>.Empty, semanticModel: _semanticModel);
		}

		private IParameterInitializerOperation CreateBoundParameterEqualsValueOperation(BoundParameterEqualsValue boundParameterEqualsValue)
		{
			return new ParameterInitializerOperation(boundParameterEqualsValue.Parameter, value: Create(boundParameterEqualsValue.Value), syntax: boundParameterEqualsValue.Syntax, isImplicit: boundParameterEqualsValue.WasCompilerGenerated, locals: ImmutableArray<ILocalSymbol>.Empty, semanticModel: _semanticModel);
		}

		private IOperation CreateBoundLValueToRValueWrapper(BoundLValueToRValueWrapper boundNode)
		{
			return Create(boundNode.UnderlyingLValue);
		}

		private IOperation CreateBoundRValuePlaceholderOperation(BoundRValuePlaceholder boundRValuePlaceholder)
		{
			SyntaxNode syntax = boundRValuePlaceholder.Syntax;
			ITypeSymbol type = boundRValuePlaceholder.Type;
			bool wasCompilerGenerated = boundRValuePlaceholder.WasCompilerGenerated;
			BoundNode boundNode = TryGetParent(boundRValuePlaceholder);
			PlaceholderKind placeholderKind = PlaceholderKind.Unspecified;
			if (boundNode != null)
			{
				switch (boundNode.Kind)
				{
				case BoundKind.ConditionalAccess:
					syntax = (syntax as ConditionalAccessExpressionSyntax)?.Expression ?? syntax;
					return new ConditionalAccessInstanceOperation(_semanticModel, syntax, type, wasCompilerGenerated);
				case BoundKind.SelectStatement:
					placeholderKind = PlaceholderKind.SwitchOperationExpression;
					break;
				case BoundKind.ForToStatement:
				{
					BoundForToUserDefinedOperators operatorsOpt = ((BoundForToStatement)boundNode).OperatorsOpt;
					placeholderKind = ((boundRValuePlaceholder != operatorsOpt.LeftOperandPlaceholder) ? PlaceholderKind.ForToLoopBinaryOperatorRightOperand : PlaceholderKind.ForToLoopBinaryOperatorLeftOperand);
					break;
				}
				case BoundKind.AggregateClause:
					placeholderKind = PlaceholderKind.AggregationGroup;
					break;
				}
			}
			return new PlaceholderOperation(placeholderKind, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private IConditionalOperation CreateBoundIfStatementOperation(BoundIfStatement boundIfStatement)
		{
			IOperation condition = Create(boundIfStatement.Condition);
			IOperation whenTrue = Create(boundIfStatement.Consequence);
			IOperation whenFalse = Create(boundIfStatement.AlternativeOpt);
			SyntaxNode syntax = boundIfStatement.Syntax;
			ITypeSymbol type = null;
			ConstantValue constantValue = null;
			bool wasCompilerGenerated = boundIfStatement.WasCompilerGenerated;
			bool isRef = false;
			return new ConditionalOperation(condition, whenTrue, whenFalse, isRef, _semanticModel, syntax, type, constantValue, wasCompilerGenerated);
		}

		private ISwitchOperation CreateBoundSelectStatementOperation(BoundSelectStatement boundSelectStatement)
		{
			RecordParent(boundSelectStatement.ExprPlaceholderOpt, boundSelectStatement);
			IOperation value = Create(boundSelectStatement.ExpressionStatement.Expression);
			ImmutableArray<ISwitchCaseOperation> cases = CreateFromArray<BoundCaseBlock, ISwitchCaseOperation>(boundSelectStatement.CaseBlocks);
			ILabelSymbol exitLabel = boundSelectStatement.ExitLabel;
			SyntaxNode syntax = boundSelectStatement.Syntax;
			bool wasCompilerGenerated = boundSelectStatement.WasCompilerGenerated;
			return new SwitchOperation(ImmutableArray<ILocalSymbol>.Empty, value, cases, exitLabel, _semanticModel, syntax, wasCompilerGenerated);
		}

		internal ImmutableArray<ICaseClauseOperation> CreateBoundCaseBlockClauses(BoundCaseBlock boundCaseBlock)
		{
			BoundCaseStatement caseStatement = boundCaseBlock.CaseStatement;
			if (caseStatement.CaseClauses.IsEmpty && Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(caseStatement.Syntax) == SyntaxKind.CaseElseStatement)
			{
				return ImmutableArray.Create((ICaseClauseOperation)new DefaultCaseClauseOperation(null, _semanticModel, caseStatement.Syntax, boundCaseBlock.WasCompilerGenerated));
			}
			return caseStatement.CaseClauses.SelectAsArray((BoundCaseClause n) => (ICaseClauseOperation)Create(n));
		}

		internal ImmutableArray<IOperation> CreateBoundCaseBlockBody(BoundCaseBlock boundCaseBlock)
		{
			return ImmutableArray.Create(Create(boundCaseBlock.Body));
		}

		internal IOperation CreateBoundCaseBlockCondition(BoundCaseBlock boundCaseBlock)
		{
			return Create(boundCaseBlock.CaseStatement.ConditionOpt);
		}

		private ISwitchCaseOperation CreateBoundCaseBlockOperation(BoundCaseBlock boundCaseBlock)
		{
			return new SwitchCaseOperation(CreateBoundCaseBlockClauses(boundCaseBlock), ImmutableArray.Create(Create(boundCaseBlock.Body)), condition: CreateBoundCaseBlockCondition(boundCaseBlock), syntax: boundCaseBlock.Syntax, isImplicit: boundCaseBlock.WasCompilerGenerated, locals: ImmutableArray<ILocalSymbol>.Empty, semanticModel: _semanticModel);
		}

		private ISingleValueCaseClauseOperation CreateBoundSimpleCaseClauseOperation(BoundSimpleCaseClause boundSimpleCaseClause)
		{
			return new SingleValueCaseClauseOperation(Create(GetSingleValueCaseClauseValue(boundSimpleCaseClause)), syntax: boundSimpleCaseClause.Syntax, isImplicit: boundSimpleCaseClause.WasCompilerGenerated, label: null, semanticModel: _semanticModel);
		}

		private IRangeCaseClauseOperation CreateBoundRangeCaseClauseOperation(BoundRangeCaseClause boundRangeCaseClause)
		{
			return new RangeCaseClauseOperation(Create(GetCaseClauseValue(boundRangeCaseClause.LowerBoundOpt, boundRangeCaseClause.LowerBoundConditionOpt)), Create(GetCaseClauseValue(boundRangeCaseClause.UpperBoundOpt, boundRangeCaseClause.UpperBoundConditionOpt)), syntax: boundRangeCaseClause.Syntax, isImplicit: boundRangeCaseClause.WasCompilerGenerated, label: null, semanticModel: _semanticModel);
		}

		private IRelationalCaseClauseOperation CreateBoundRelationalCaseClauseOperation(BoundRelationalCaseClause boundRelationalCaseClause)
		{
			IOperation operation = Create(GetSingleValueCaseClauseValue(boundRelationalCaseClause));
			return new RelationalCaseClauseOperation(relation: (operation != null) ? Helper.DeriveBinaryOperatorKind(boundRelationalCaseClause.OperatorKind, null) : BinaryOperatorKind.None, syntax: boundRelationalCaseClause.Syntax, isImplicit: boundRelationalCaseClause.WasCompilerGenerated, value: operation, label: null, semanticModel: _semanticModel);
		}

		private IWhileLoopOperation CreateBoundDoLoopStatementOperation(BoundDoLoopStatement boundDoLoopStatement)
		{
			return new WhileLoopOperation(Create(boundDoLoopStatement.ConditionOpt), body: Create(boundDoLoopStatement.Body), ignoredCondition: (boundDoLoopStatement.TopConditionOpt != null && boundDoLoopStatement.BottomConditionOpt != null) ? Create(boundDoLoopStatement.BottomConditionOpt) : null, locals: ImmutableArray<ILocalSymbol>.Empty, continueLabel: boundDoLoopStatement.ContinueLabel, exitLabel: boundDoLoopStatement.ExitLabel, conditionIsTop: boundDoLoopStatement.ConditionIsTop, conditionIsUntil: boundDoLoopStatement.ConditionIsUntil, syntax: boundDoLoopStatement.Syntax, isImplicit: boundDoLoopStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IForToLoopOperation CreateBoundForToStatementOperation(BoundForToStatement boundForToStatement)
		{
			IOperation loopControlVariable = CreateBoundControlVariableOperation(boundForToStatement);
			IOperation initialValue = Create(boundForToStatement.InitialValue);
			IOperation limitValue = Create(boundForToStatement.LimitValue);
			IOperation stepValue = Create(boundForToStatement.StepValue);
			IOperation body = Create(boundForToStatement.Body);
			ImmutableArray<IOperation> nextVariables = (boundForToStatement.NextVariablesOpt.IsDefault ? ImmutableArray<IOperation>.Empty : CreateFromArray<BoundExpression, IOperation>(boundForToStatement.NextVariablesOpt));
			ImmutableArray<ILocalSymbol> locals = (((object)boundForToStatement.DeclaredOrInferredLocalOpt != null) ? ImmutableArray.Create((ILocalSymbol)boundForToStatement.DeclaredOrInferredLocalOpt) : ImmutableArray<ILocalSymbol>.Empty);
			ILabelSymbol continueLabel = boundForToStatement.ContinueLabel;
			ILabelSymbol exitLabel = boundForToStatement.ExitLabel;
			SyntaxNode syntax = boundForToStatement.Syntax;
			bool wasCompilerGenerated = boundForToStatement.WasCompilerGenerated;
			SynthesizedLocal item = (TypeSymbolExtensions.IsObjectType(boundForToStatement.ControlVariable.Type) ? new SynthesizedLocal((Symbol)_semanticModel.GetEnclosingSymbol(boundForToStatement.Syntax.SpanStart), boundForToStatement.ControlVariable.Type, SynthesizedLocalKind.ForInitialValue, boundForToStatement.Syntax) : null);
			ForToLoopOperationUserDefinedInfo item2 = null;
			BoundForToUserDefinedOperators operatorsOpt = boundForToStatement.OperatorsOpt;
			if (operatorsOpt != null)
			{
				RecordParent(operatorsOpt.LeftOperandPlaceholder, boundForToStatement);
				RecordParent(operatorsOpt.RightOperandPlaceholder, boundForToStatement);
				item2 = new ForToLoopOperationUserDefinedInfo((IBinaryOperation)Operation.SetParentOperation(Create(operatorsOpt.Addition), null), (IBinaryOperation)Operation.SetParentOperation(Create(operatorsOpt.Subtraction), null), Operation.SetParentOperation(Create(operatorsOpt.LessThanOrEqual), null), Operation.SetParentOperation(Create(operatorsOpt.GreaterThanOrEqual), null));
			}
			return new ForToLoopOperation(loopControlVariable, initialValue, limitValue, stepValue, boundForToStatement.Checked, nextVariables, (item, item2), body, locals, continueLabel, exitLabel, _semanticModel, syntax, wasCompilerGenerated);
		}

		internal ForEachLoopOperationInfo GetForEachLoopOperationInfo(BoundForEachStatement boundForEachStatement)
		{
			ImmutableArray<BoundExpression> getEnumeratorArguments = default(ImmutableArray<BoundExpression>);
			BitVector getEnumeratorDefaultArguments = BitVector.Null;
			ImmutableArray<BoundExpression> moveNextArguments = default(ImmutableArray<BoundExpression>);
			BitVector moveNextDefaultArguments = BitVector.Null;
			ImmutableArray<BoundExpression> currentArguments = default(ImmutableArray<BoundExpression>);
			BitVector currentDefaultArguments = BitVector.Null;
			ForEachStatementInfo forEachStatementInfo = MemberSemanticModel.GetForEachStatementInfo(boundForEachStatement, (VisualBasicCompilation)_semanticModel.Compilation, out getEnumeratorArguments, out getEnumeratorDefaultArguments, out moveNextArguments, out moveNextDefaultArguments, out currentArguments, out currentDefaultArguments);
			return new ForEachLoopOperationInfo(forEachStatementInfo.ElementType, forEachStatementInfo.GetEnumeratorMethod, forEachStatementInfo.CurrentProperty, forEachStatementInfo.MoveNextMethod, isAsynchronous: false, boundForEachStatement.EnumeratorInfo.NeedToDispose, boundForEachStatement.EnumeratorInfo.NeedToDispose && boundForEachStatement.EnumeratorInfo.IsOrInheritsFromOrImplementsIDisposable, null, forEachStatementInfo.CurrentConversion, forEachStatementInfo.ElementConversion, getEnumeratorArguments.IsDefaultOrEmpty ? default(ImmutableArray<IArgumentOperation>) : Operation.SetParentOperation(DeriveArguments(getEnumeratorArguments, ((MethodSymbol)forEachStatementInfo.GetEnumeratorMethod).Parameters, ref getEnumeratorDefaultArguments), null), moveNextArguments.IsDefaultOrEmpty ? default(ImmutableArray<IArgumentOperation>) : Operation.SetParentOperation(DeriveArguments(moveNextArguments, ((MethodSymbol)forEachStatementInfo.MoveNextMethod).Parameters, ref moveNextDefaultArguments), null), currentArguments.IsDefaultOrEmpty ? default(ImmutableArray<IArgumentOperation>) : Operation.SetParentOperation(DeriveArguments(currentArguments, ((PropertySymbol)forEachStatementInfo.CurrentProperty).Parameters, ref currentDefaultArguments), null));
		}

		private IForEachLoopOperation CreateBoundForEachStatementOperation(BoundForEachStatement boundForEachStatement)
		{
			ForEachLoopOperationInfo forEachLoopOperationInfo = GetForEachLoopOperationInfo(boundForEachStatement);
			return new ForEachLoopOperation(CreateBoundControlVariableOperation(boundForEachStatement), Create(boundForEachStatement.Collection), boundForEachStatement.NextVariablesOpt.IsDefault ? ImmutableArray<IOperation>.Empty : CreateFromArray<BoundExpression, IOperation>(boundForEachStatement.NextVariablesOpt), body: Create(boundForEachStatement.Body), locals: ((object)boundForEachStatement.DeclaredOrInferredLocalOpt != null) ? ImmutableArray.Create((ILocalSymbol)boundForEachStatement.DeclaredOrInferredLocalOpt) : ImmutableArray<ILocalSymbol>.Empty, continueLabel: boundForEachStatement.ContinueLabel, exitLabel: boundForEachStatement.ExitLabel, syntax: boundForEachStatement.Syntax, isImplicit: boundForEachStatement.WasCompilerGenerated, info: forEachLoopOperationInfo, isAsynchronous: false, semanticModel: _semanticModel);
		}

		internal IOperation CreateBoundControlVariableOperation(BoundForStatement boundForStatement)
		{
			LocalSymbol declaredOrInferredLocalOpt = boundForStatement.DeclaredOrInferredLocalOpt;
			BoundExpression controlVariable = boundForStatement.ControlVariable;
			if ((object)declaredOrInferredLocalOpt == null)
			{
				return Create(controlVariable);
			}
			return new VariableDeclaratorOperation(declaredOrInferredLocalOpt, null, ImmutableArray<IOperation>.Empty, _semanticModel, controlVariable.Syntax, boundForStatement.WasCompilerGenerated);
		}

		private ITryOperation CreateBoundTryStatementOperation(BoundTryStatement boundTryStatement)
		{
			return new TryOperation((IBlockOperation)Create(boundTryStatement.TryBlock), CreateFromArray<BoundCatchBlock, ICatchClauseOperation>(boundTryStatement.CatchBlocks), (IBlockOperation)Create(boundTryStatement.FinallyBlockOpt), boundTryStatement.ExitLabelOpt, syntax: boundTryStatement.Syntax, isImplicit: boundTryStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		internal IOperation CreateBoundCatchBlockExceptionDeclarationOrExpression(BoundCatchBlock boundCatchBlock)
		{
			if ((object)boundCatchBlock.LocalOpt != null)
			{
				BoundExpression exceptionSourceOpt = boundCatchBlock.ExceptionSourceOpt;
				if (exceptionSourceOpt != null && exceptionSourceOpt.Kind == BoundKind.Local && (object)boundCatchBlock.LocalOpt == ((BoundLocal)boundCatchBlock.ExceptionSourceOpt).LocalSymbol)
				{
					return new VariableDeclaratorOperation(boundCatchBlock.LocalOpt, null, ImmutableArray<IOperation>.Empty, _semanticModel, boundCatchBlock.ExceptionSourceOpt.Syntax, isImplicit: false);
				}
			}
			return Create(boundCatchBlock.ExceptionSourceOpt);
		}

		private ICatchClauseOperation CreateBoundCatchBlockOperation(BoundCatchBlock boundCatchBlock)
		{
			IOperation exceptionDeclarationOrExpression = CreateBoundCatchBlockExceptionDeclarationOrExpression(boundCatchBlock);
			IOperation filter = Create(boundCatchBlock.ExceptionFilterOpt);
			IBlockOperation handler = (IBlockOperation)Create(boundCatchBlock.Body);
			ITypeSymbol exceptionType = boundCatchBlock.ExceptionSourceOpt?.Type ?? ((VisualBasicCompilation)_semanticModel.Compilation).GetWellKnownType(WellKnownType.System_Exception);
			ImmutableArray<ILocalSymbol> locals = (((object)boundCatchBlock.LocalOpt != null) ? ImmutableArray.Create((ILocalSymbol)boundCatchBlock.LocalOpt) : ImmutableArray<ILocalSymbol>.Empty);
			SyntaxNode syntax = boundCatchBlock.Syntax;
			bool wasCompilerGenerated = boundCatchBlock.WasCompilerGenerated;
			return new CatchClauseOperation(exceptionDeclarationOrExpression, exceptionType, locals, filter, handler, _semanticModel, syntax, wasCompilerGenerated);
		}

		private IBlockOperation CreateBoundBlockOperation(BoundBlock boundBlock)
		{
			return new BlockOperation(CreateFromArray<BoundStatement, IOperation>(boundBlock.Statements), boundBlock.Locals.As<ILocalSymbol>(), syntax: boundBlock.Syntax, isImplicit: boundBlock.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IInvalidOperation CreateBoundBadStatementOperation(BoundBadStatement boundBadStatement)
		{
			SyntaxNode syntax = boundBadStatement.Syntax;
			bool flag = boundBadStatement.WasCompilerGenerated;
			if (!flag)
			{
				ImmutableArray<BoundNode>.Enumerator enumerator = boundBadStatement.ChildBoundNodes.GetEnumerator();
				while (enumerator.MoveNext())
				{
					if (enumerator.Current?.Syntax == boundBadStatement.Syntax)
					{
						flag = true;
						break;
					}
				}
			}
			return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(boundBadStatement.ChildBoundNodes), _semanticModel, syntax, null, null, flag);
		}

		private IReturnOperation CreateBoundReturnStatementOperation(BoundReturnStatement boundReturnStatement)
		{
			IOperation returnedValue = Create(boundReturnStatement.ExpressionOpt);
			SyntaxNode syntax = boundReturnStatement.Syntax;
			return new ReturnOperation(isImplicit: boundReturnStatement.WasCompilerGenerated || IsEndSubOrFunctionStatement(syntax), returnedValue: returnedValue, kind: OperationKind.Return, semanticModel: _semanticModel, syntax: syntax);
		}

		private static bool IsEndSubOrFunctionStatement(SyntaxNode syntax)
		{
			if ((syntax.Parent as MethodBlockBaseSyntax)?.EndBlockStatement != syntax)
			{
				return (syntax.Parent as MultiLineLambdaExpressionSyntax)?.EndSubOrFunctionStatement == syntax;
			}
			return true;
		}

		private IThrowOperation CreateBoundThrowStatementOperation(BoundThrowStatement boundThrowStatement)
		{
			return new ThrowOperation(Create(boundThrowStatement.ExpressionOpt), syntax: boundThrowStatement.Syntax, type: null, isImplicit: boundThrowStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IWhileLoopOperation CreateBoundWhileStatementOperation(BoundWhileStatement boundWhileStatement)
		{
			return new WhileLoopOperation(Create(boundWhileStatement.Condition), body: Create(boundWhileStatement.Body), ignoredCondition: null, locals: ImmutableArray<ILocalSymbol>.Empty, continueLabel: boundWhileStatement.ContinueLabel, exitLabel: boundWhileStatement.ExitLabel, conditionIsTop: true, conditionIsUntil: false, syntax: boundWhileStatement.Syntax, isImplicit: boundWhileStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IVariableDeclarationGroupOperation CreateBoundDimStatementOperation(BoundDimStatement boundDimStatement)
		{
			return new VariableDeclarationGroupOperation(GetVariableDeclarationStatementVariables(boundDimStatement.LocalDeclarations), syntax: boundDimStatement.Syntax, isImplicit: boundDimStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IVariableDeclarationGroupOperation CreateBoundLocalDeclarationOperation(BoundLocalDeclaration boundLocalDeclaration)
		{
			ImmutableArray<IVariableDeclarationOperation> variableDeclarationStatementVariables = GetVariableDeclarationStatementVariables(ImmutableArray.Create((BoundLocalDeclarationBase)boundLocalDeclaration));
			SyntaxNode syntax = boundLocalDeclaration.Syntax;
			bool isImplicit = true;
			return new VariableDeclarationGroupOperation(variableDeclarationStatementVariables, _semanticModel, syntax, isImplicit);
		}

		private IReturnOperation CreateBoundYieldStatementOperation(BoundYieldStatement boundYieldStatement)
		{
			return new ReturnOperation(Create(boundYieldStatement.Expression), syntax: boundYieldStatement.Syntax, isImplicit: boundYieldStatement.WasCompilerGenerated, kind: OperationKind.YieldReturn, semanticModel: _semanticModel);
		}

		private ILabeledOperation CreateBoundLabelStatementOperation(BoundLabelStatement boundLabelStatement)
		{
			LabelSymbol label = boundLabelStatement.Label;
			IOperation operation = null;
			SyntaxNode syntax = boundLabelStatement.Syntax;
			return new LabeledOperation(isImplicit: boundLabelStatement.WasCompilerGenerated || IsEndSubOrFunctionStatement(syntax), label: label, operation: operation, semanticModel: _semanticModel, syntax: syntax);
		}

		private IBranchOperation CreateBoundGotoStatementOperation(BoundGotoStatement boundGotoStatement)
		{
			return new BranchOperation(boundGotoStatement.Label, BranchKind.GoTo, syntax: boundGotoStatement.Syntax, isImplicit: boundGotoStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IBranchOperation CreateBoundContinueStatementOperation(BoundContinueStatement boundContinueStatement)
		{
			return new BranchOperation(boundContinueStatement.Label, BranchKind.Continue, syntax: boundContinueStatement.Syntax, isImplicit: boundContinueStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IBranchOperation CreateBoundExitStatementOperation(BoundExitStatement boundExitStatement)
		{
			return new BranchOperation(boundExitStatement.Label, BranchKind.Break, syntax: boundExitStatement.Syntax, isImplicit: boundExitStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private ILockOperation CreateBoundSyncLockStatementOperation(BoundSyncLockStatement boundSyncLockStatement)
		{
			ILocalSymbol lockTakenSymbol = ((_semanticModel.Compilation.CommonGetWellKnownTypeMember(WellKnownMember.System_Threading_Monitor__Enter2) == null) ? null : new SynthesizedLocal((Symbol)_semanticModel.GetEnclosingSymbol(boundSyncLockStatement.Syntax.SpanStart), (TypeSymbol)_semanticModel.Compilation.GetSpecialType(SpecialType.System_Boolean), SynthesizedLocalKind.LockTaken, boundSyncLockStatement.LockExpression.Syntax));
			return new LockOperation(Create(boundSyncLockStatement.LockExpression), Create(boundSyncLockStatement.Body), syntax: boundSyncLockStatement.Syntax, isImplicit: boundSyncLockStatement.WasCompilerGenerated, lockTakenSymbol: lockTakenSymbol, semanticModel: _semanticModel);
		}

		private IEmptyOperation CreateBoundNoOpStatementOperation(BoundNoOpStatement boundNoOpStatement)
		{
			SyntaxNode syntax = boundNoOpStatement.Syntax;
			bool wasCompilerGenerated = boundNoOpStatement.WasCompilerGenerated;
			return new EmptyOperation(_semanticModel, syntax, wasCompilerGenerated);
		}

		private IStopOperation CreateBoundStopStatementOperation(BoundStopStatement boundStopStatement)
		{
			SyntaxNode syntax = boundStopStatement.Syntax;
			bool wasCompilerGenerated = boundStopStatement.WasCompilerGenerated;
			return new StopOperation(_semanticModel, syntax, wasCompilerGenerated);
		}

		private IEndOperation CreateBoundEndStatementOperation(BoundEndStatement boundEndStatement)
		{
			SyntaxNode syntax = boundEndStatement.Syntax;
			bool wasCompilerGenerated = boundEndStatement.WasCompilerGenerated;
			return new EndOperation(_semanticModel, syntax, wasCompilerGenerated);
		}

		private IWithStatementOperation CreateBoundWithStatementOperation(BoundWithStatement boundWithStatement)
		{
			IOperation value = Create(boundWithStatement.OriginalExpression);
			return new WithStatementOperation(Create(boundWithStatement.Body), syntax: boundWithStatement.Syntax, isImplicit: boundWithStatement.WasCompilerGenerated, value: value, semanticModel: _semanticModel);
		}

		internal IOperation CreateBoundUsingStatementResources(BoundUsingStatement boundUsingStatement)
		{
			if (!boundUsingStatement.ResourceList.IsDefault)
			{
				return GetUsingStatementDeclaration(boundUsingStatement.ResourceList, ((UsingBlockSyntax)boundUsingStatement.Syntax).UsingStatement);
			}
			return Create(boundUsingStatement.ResourceExpressionOpt);
		}

		private IUsingOperation CreateBoundUsingStatementOperation(BoundUsingStatement boundUsingStatement)
		{
			return new UsingOperation(CreateBoundUsingStatementResources(boundUsingStatement), Create(boundUsingStatement.Body), ImmutableArray<ILocalSymbol>.CastUp(boundUsingStatement.Locals), syntax: boundUsingStatement.Syntax, isImplicit: boundUsingStatement.WasCompilerGenerated, isAsynchronous: false, disposeInfo: default(DisposeOperationInfo), semanticModel: _semanticModel);
		}

		private IExpressionStatementOperation CreateBoundExpressionStatementOperation(BoundExpressionStatement boundExpressionStatement)
		{
			return new ExpressionStatementOperation(Create(boundExpressionStatement.Expression), syntax: boundExpressionStatement.Syntax, isImplicit: boundExpressionStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		internal IEventReferenceOperation CreateBoundRaiseEventStatementEventReference(BoundRaiseEventStatement boundRaiseEventStatement)
		{
			BoundExpression receiverOpt = ((BoundCall)boundRaiseEventStatement.EventInvocation).ReceiverOpt;
			SyntaxNode syntax = receiverOpt?.Syntax ?? (boundRaiseEventStatement.Syntax as RaiseEventStatementSyntax)?.Name ?? boundRaiseEventStatement.Syntax;
			ITypeSymbol type = boundRaiseEventStatement.EventSymbol.Type;
			bool isImplicit = false;
			if (receiverOpt != null && receiverOpt.Kind == BoundKind.FieldAccess)
			{
				receiverOpt = ((BoundFieldAccess)receiverOpt).ReceiverOpt;
			}
			IOperation instance = CreateReceiverOperation(receiverOpt, boundRaiseEventStatement.EventSymbol);
			return new EventReferenceOperation(boundRaiseEventStatement.EventSymbol, instance, _semanticModel, syntax, type, isImplicit);
		}

		private IOperation CreateBoundRaiseEventStatementOperation(BoundRaiseEventStatement boundRaiseEventStatement)
		{
			SyntaxNode syntax = boundRaiseEventStatement.Syntax;
			bool wasCompilerGenerated = boundRaiseEventStatement.WasCompilerGenerated;
			EventSymbol eventSymbol = boundRaiseEventStatement.EventSymbol;
			if (!(boundRaiseEventStatement.EventInvocation is BoundCall boundCall) || (boundCall.ReceiverOpt == null && !eventSymbol.IsShared))
			{
				return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundRaiseEventStatement).InvalidNodeChildren), _semanticModel, syntax, null, null, wasCompilerGenerated);
			}
			IEventReferenceOperation eventReference = CreateBoundRaiseEventStatementEventReference(boundRaiseEventStatement);
			ImmutableArray<IArgumentOperation> arguments = DeriveArguments(boundRaiseEventStatement);
			return new RaiseEventOperation(eventReference, arguments, _semanticModel, syntax, wasCompilerGenerated);
		}

		private IExpressionStatementOperation CreateBoundAddHandlerStatementOperation(BoundAddHandlerStatement boundAddHandlerStatement)
		{
			return new ExpressionStatementOperation(GetAddRemoveHandlerStatementExpression(boundAddHandlerStatement), syntax: boundAddHandlerStatement.Syntax, isImplicit: boundAddHandlerStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IExpressionStatementOperation CreateBoundRemoveHandlerStatementOperation(BoundRemoveHandlerStatement boundRemoveHandlerStatement)
		{
			return new ExpressionStatementOperation(GetAddRemoveHandlerStatementExpression(boundRemoveHandlerStatement), syntax: boundRemoveHandlerStatement.Syntax, isImplicit: boundRemoveHandlerStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private ITupleOperation CreateBoundTupleLiteralOperation(BoundTupleLiteral boundTupleLiteral)
		{
			return CreateTupleOperation(boundTupleLiteral, boundTupleLiteral.Type);
		}

		private ITupleOperation CreateBoundConvertedTupleLiteralOperation(BoundConvertedTupleLiteral boundConvertedTupleLiteral)
		{
			return CreateTupleOperation(boundConvertedTupleLiteral, boundConvertedTupleLiteral.NaturalTypeOpt);
		}

		private ITupleOperation CreateTupleOperation(BoundTupleExpression boundTupleExpression, ITypeSymbol naturalType)
		{
			return new TupleOperation(CreateFromArray<BoundExpression, IOperation>(boundTupleExpression.Arguments), syntax: boundTupleExpression.Syntax, type: boundTupleExpression.Type, isImplicit: boundTupleExpression.WasCompilerGenerated, naturalType: naturalType, semanticModel: _semanticModel);
		}

		private IInterpolatedStringOperation CreateBoundInterpolatedStringExpressionOperation(BoundInterpolatedStringExpression boundInterpolatedString)
		{
			return new InterpolatedStringOperation(CreateBoundInterpolatedStringContentOperation(boundInterpolatedString.Contents), syntax: boundInterpolatedString.Syntax, type: boundInterpolatedString.Type, constantValue: boundInterpolatedString.ConstantValueOpt, isImplicit: boundInterpolatedString.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		internal ImmutableArray<IInterpolatedStringContentOperation> CreateBoundInterpolatedStringContentOperation(ImmutableArray<BoundNode> parts)
		{
			ArrayBuilder<IInterpolatedStringContentOperation> instance = ArrayBuilder<IInterpolatedStringContentOperation>.GetInstance(parts.Length);
			ImmutableArray<BoundNode>.Enumerator enumerator = parts.GetEnumerator();
			while (enumerator.MoveNext())
			{
				BoundNode current = enumerator.Current;
				if (current.Kind == BoundKind.Interpolation)
				{
					instance.Add((IInterpolatedStringContentOperation)Create(current));
				}
				else
				{
					instance.Add(CreateBoundInterpolatedStringTextOperation((BoundLiteral)current));
				}
			}
			return instance.ToImmutableAndFree();
		}

		private IInterpolationOperation CreateBoundInterpolationOperation(BoundInterpolation boundInterpolation)
		{
			return new InterpolationOperation(Create(boundInterpolation.Expression), Create(boundInterpolation.AlignmentOpt), Create(boundInterpolation.FormatStringOpt), syntax: boundInterpolation.Syntax, isImplicit: boundInterpolation.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IInterpolatedStringTextOperation CreateBoundInterpolatedStringTextOperation(BoundLiteral boundLiteral)
		{
			return new InterpolatedStringTextOperation(CreateBoundLiteralOperation(boundLiteral, @implicit: true), syntax: boundLiteral.Syntax, isImplicit: boundLiteral.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IAnonymousObjectCreationOperation CreateBoundAnonymousTypeCreationExpressionOperation(BoundAnonymousTypeCreationExpression boundAnonymousTypeCreationExpression)
		{
			return new AnonymousObjectCreationOperation(GetAnonymousTypeCreationInitializers(boundAnonymousTypeCreationExpression), syntax: boundAnonymousTypeCreationExpression.Syntax, type: boundAnonymousTypeCreationExpression.Type, isImplicit: boundAnonymousTypeCreationExpression.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IPropertyReferenceOperation CreateBoundAnonymousTypePropertyAccessOperation(BoundAnonymousTypePropertyAccess boundAnonymousTypePropertyAccess)
		{
			IPropertySymbol propertySymbol = (IPropertySymbol)boundAnonymousTypePropertyAccess.ExpressionSymbol;
			IOperation instance = CreateAnonymousTypePropertyAccessImplicitReceiverOperation(propertySymbol, boundAnonymousTypePropertyAccess.Syntax.FirstAncestorOrSelf<AnonymousObjectCreationExpressionSyntax>());
			ImmutableArray<IArgumentOperation> empty = ImmutableArray<IArgumentOperation>.Empty;
			SyntaxNode syntax = boundAnonymousTypePropertyAccess.Syntax;
			ITypeSymbol type = boundAnonymousTypePropertyAccess.Type;
			bool wasCompilerGenerated = boundAnonymousTypePropertyAccess.WasCompilerGenerated;
			return new PropertyReferenceOperation(propertySymbol, empty, instance, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private InstanceReferenceOperation CreateAnonymousTypePropertyAccessImplicitReceiverOperation(IPropertySymbol propertySym, SyntaxNode syntax)
		{
			return new InstanceReferenceOperation(InstanceReferenceKind.ImplicitReceiver, _semanticModel, syntax, propertySym.ContainingType, isImplicit: true);
		}

		private IOperation CreateBoundQueryExpressionOperation(BoundQueryExpression boundQueryExpression)
		{
			return new TranslatedQueryOperation(Create(boundQueryExpression.LastOperator), syntax: boundQueryExpression.Syntax, type: boundQueryExpression.Type, isImplicit: boundQueryExpression.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IOperation CreateBoundAggregateClauseOperation(BoundAggregateClause boundAggregateClause)
		{
			if (boundAggregateClause.CapturedGroupOpt == null)
			{
				return Create(boundAggregateClause.UnderlyingExpression);
			}
			RecordParent(boundAggregateClause.GroupPlaceholderOpt, boundAggregateClause);
			return new AggregateQueryOperation(Create(boundAggregateClause.CapturedGroupOpt), Create(boundAggregateClause.UnderlyingExpression), syntax: boundAggregateClause.Syntax, type: boundAggregateClause.Type, isImplicit: boundAggregateClause.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IOperation CreateBoundNullableIsTrueOperator(BoundNullableIsTrueOperator boundNullableIsTrueOperator)
		{
			SyntaxNode syntax = boundNullableIsTrueOperator.Syntax;
			ITypeSymbol type = boundNullableIsTrueOperator.Type;
			ConstantValue constantValueOpt = boundNullableIsTrueOperator.ConstantValueOpt;
			bool wasCompilerGenerated = boundNullableIsTrueOperator.WasCompilerGenerated;
			MethodSymbol methodSymbol = (MethodSymbol)((VisualBasicCompilation)_semanticModel.Compilation).GetSpecialTypeMember(SpecialMember.System_Nullable_T_GetValueOrDefault);
			if ((object)methodSymbol != null)
			{
				IOperation instance = CreateReceiverOperation(boundNullableIsTrueOperator.Operand, methodSymbol);
				return new InvocationOperation(SymbolExtensions.AsMember(methodSymbol, (NamedTypeSymbol)boundNullableIsTrueOperator.Operand.Type), instance, isVirtual: false, ImmutableArray<IArgumentOperation>.Empty, _semanticModel, syntax, boundNullableIsTrueOperator.Type, wasCompilerGenerated);
			}
			return new InvalidOperation(CreateFromArray<BoundNode, IOperation>(((IBoundInvalidNode)boundNullableIsTrueOperator).InvalidNodeChildren), _semanticModel, syntax, type, constantValueOpt, wasCompilerGenerated);
		}

		private IReDimOperation CreateBoundReDimOperation(BoundRedimStatement boundRedimStatement)
		{
			return new ReDimOperation(CreateFromArray<BoundRedimClause, IReDimClauseOperation>(boundRedimStatement.Clauses), Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(boundRedimStatement.Syntax) == SyntaxKind.ReDimPreserveStatement, syntax: boundRedimStatement.Syntax, isImplicit: boundRedimStatement.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private IReDimClauseOperation CreateBoundReDimClauseOperation(BoundRedimClause boundRedimClause)
		{
			return new ReDimClauseOperation(Create(boundRedimClause.Operand), CreateFromArray<BoundExpression, IOperation>(boundRedimClause.Indices), syntax: boundRedimClause.Syntax, isImplicit: boundRedimClause.WasCompilerGenerated, semanticModel: _semanticModel);
		}

		private static bool IsMidStatement(BoundNode node)
		{
			if (node.Kind == BoundKind.Conversion)
			{
				node = ((BoundConversion)node).Operand;
				if (node.Kind == BoundKind.UserDefinedConversion)
				{
					node = ((BoundUserDefinedConversion)node).Operand;
				}
			}
			return node.Kind == BoundKind.MidResult;
		}

		internal IOperation CreateCompoundAssignmentRightOperand(BoundAssignmentOperator boundAssignment)
		{
			BoundExpression boundExpression = null;
			switch (boundAssignment.Right.Kind)
			{
			case BoundKind.Conversion:
				boundExpression = GetConversionOperand((BoundConversion)boundAssignment.Right);
				break;
			case BoundKind.BinaryOperator:
			case BoundKind.UserDefinedBinaryOperator:
				boundExpression = boundAssignment.Right;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind);
			}
			switch (boundExpression.Kind)
			{
			case BoundKind.BinaryOperator:
				return Create(GetBinaryOperatorInfo((BoundBinaryOperator)boundExpression).RightOperand);
			case BoundKind.UserDefinedBinaryOperator:
			{
				BoundUserDefinedBinaryOperator boundUserDefinedBinaryOperator = (BoundUserDefinedBinaryOperator)boundExpression;
				return GetUserDefinedBinaryOperatorChild(boundUserDefinedBinaryOperator, GetUserDefinedBinaryOperatorInfo(boundUserDefinedBinaryOperator).RightOperand);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind);
			}
		}

		private ICompoundAssignmentOperation CreateCompoundAssignment(BoundAssignmentOperator boundAssignment)
		{
			Conversion conversion = new Conversion(Conversions.Identity);
			Conversion conversion2 = conversion;
			BoundExpression boundExpression = null;
			switch (boundAssignment.Right.Kind)
			{
			case BoundKind.Conversion:
			{
				BoundConversion obj = (BoundConversion)boundAssignment.Right;
				conversion2 = CreateConversion(obj);
				boundExpression = GetConversionOperand(obj);
				break;
			}
			case BoundKind.BinaryOperator:
			case BoundKind.UserDefinedBinaryOperator:
				boundExpression = boundAssignment.Right;
				break;
			default:
				throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind);
			}
			BinaryOperatorInfo binaryOperatorInfo = boundExpression.Kind switch
			{
				BoundKind.BinaryOperator => GetBinaryOperatorInfo((BoundBinaryOperator)boundExpression), 
				BoundKind.UserDefinedBinaryOperator => GetUserDefinedBinaryOperatorInfo((BoundUserDefinedBinaryOperator)boundExpression), 
				_ => throw ExceptionUtilities.UnexpectedValue(boundAssignment.Kind), 
			};
			BoundExpression leftOperand = binaryOperatorInfo.LeftOperand;
			if (leftOperand.Kind == BoundKind.Conversion)
			{
				BoundConversion obj2 = (BoundConversion)leftOperand;
				conversion = CreateConversion(obj2);
				leftOperand = GetConversionOperand(obj2);
			}
			IOperation target = Create(boundAssignment.Left);
			IOperation value = CreateCompoundAssignmentRightOperand(boundAssignment);
			SyntaxNode syntax = boundAssignment.Syntax;
			ITypeSymbol type = boundAssignment.Type;
			bool wasCompilerGenerated = boundAssignment.WasCompilerGenerated;
			return new CompoundAssignmentOperation(conversion, conversion2, binaryOperatorInfo.OperatorKind, binaryOperatorInfo.IsLifted, binaryOperatorInfo.IsChecked, binaryOperatorInfo.OperatorMethod, target, value, _semanticModel, syntax, type, wasCompilerGenerated);
		}

		private static BinaryOperatorInfo GetBinaryOperatorInfo(BoundBinaryOperator boundBinaryOperator)
		{
			return new BinaryOperatorInfo(boundBinaryOperator.Left, boundBinaryOperator.Right, Helper.DeriveBinaryOperatorKind(boundBinaryOperator.OperatorKind, boundBinaryOperator.Left), null, (boundBinaryOperator.OperatorKind & Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Lifted) != 0, boundBinaryOperator.Checked, (boundBinaryOperator.OperatorKind & Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.CompareText) != 0);
		}

		private static BinaryOperatorInfo GetUserDefinedBinaryOperatorInfo(BoundUserDefinedBinaryOperator boundUserDefinedBinaryOperator)
		{
			return new BinaryOperatorInfo(GetUserDefinedBinaryOperatorChildBoundNode(boundUserDefinedBinaryOperator, 0), GetUserDefinedBinaryOperatorChildBoundNode(boundUserDefinedBinaryOperator, 1), Helper.DeriveBinaryOperatorKind(boundUserDefinedBinaryOperator.OperatorKind, null), (boundUserDefinedBinaryOperator.UnderlyingExpression.Kind == BoundKind.Call) ? boundUserDefinedBinaryOperator.Call.Method : null, (boundUserDefinedBinaryOperator.OperatorKind & Microsoft.CodeAnalysis.VisualBasic.BinaryOperatorKind.Lifted) != 0, boundUserDefinedBinaryOperator.Checked, isCompareText: false);
		}

		private IOperation GetUserDefinedBinaryOperatorChild(BoundUserDefinedBinaryOperator @operator, BoundExpression child)
		{
			if (child != null)
			{
				return Create(child);
			}
			bool wasCompilerGenerated = @operator.UnderlyingExpression.WasCompilerGenerated;
			return OperationFactory.CreateInvalidOperation(_semanticModel, @operator.UnderlyingExpression.Syntax, ImmutableArray<IOperation>.Empty, wasCompilerGenerated);
		}

		private static BoundExpression GetUserDefinedBinaryOperatorChildBoundNode(BoundUserDefinedBinaryOperator @operator, int index)
		{
			if (@operator.UnderlyingExpression.Kind == BoundKind.Call)
			{
				return index switch
				{
					0 => @operator.Left, 
					1 => @operator.Right, 
					_ => throw ExceptionUtilities.UnexpectedValue(index), 
				};
			}
			return GetChildOfBadExpressionBoundNode(@operator.UnderlyingExpression, index);
		}

		internal ImmutableArray<IArgumentOperation> DeriveArguments(BoundNode boundNode)
		{
			switch (boundNode.Kind)
			{
			case BoundKind.Call:
			{
				BoundCall boundCall = (BoundCall)boundNode;
				ImmutableArray<BoundExpression> arguments3 = boundCall.Arguments;
				ImmutableArray<ParameterSymbol> parameters3 = boundCall.Method.Parameters;
				BitVector defaultArguments = boundCall.DefaultArguments;
				return DeriveArguments(arguments3, parameters3, ref defaultArguments);
			}
			case BoundKind.ObjectCreationExpression:
			{
				BoundObjectCreationExpression boundObjectCreationExpression = (BoundObjectCreationExpression)boundNode;
				if (boundObjectCreationExpression.Arguments.IsDefault)
				{
					return ImmutableArray<IArgumentOperation>.Empty;
				}
				ImmutableArray<IArgumentOperation> result2;
				if ((object)boundObjectCreationExpression.ConstructorOpt != null)
				{
					ImmutableArray<BoundExpression> arguments2 = boundObjectCreationExpression.Arguments;
					ImmutableArray<ParameterSymbol> parameters2 = boundObjectCreationExpression.ConstructorOpt.Parameters;
					BitVector defaultArguments = boundObjectCreationExpression.DefaultArguments;
					result2 = DeriveArguments(arguments2, parameters2, ref defaultArguments);
				}
				else
				{
					result2 = ImmutableArray<IArgumentOperation>.Empty;
				}
				return result2;
			}
			case BoundKind.PropertyAccess:
			{
				BoundPropertyAccess boundPropertyAccess = (BoundPropertyAccess)boundNode;
				ImmutableArray<IArgumentOperation> result;
				if (boundPropertyAccess.Arguments.Length != 0)
				{
					ImmutableArray<BoundExpression> arguments = boundPropertyAccess.Arguments;
					ImmutableArray<ParameterSymbol> parameters = boundPropertyAccess.PropertySymbol.Parameters;
					BitVector defaultArguments = boundPropertyAccess.DefaultArguments;
					result = DeriveArguments(arguments, parameters, ref defaultArguments);
				}
				else
				{
					result = ImmutableArray<IArgumentOperation>.Empty;
				}
				return result;
			}
			case BoundKind.RaiseEventStatement:
			{
				BoundRaiseEventStatement boundRaiseEventStatement = (BoundRaiseEventStatement)boundNode;
				return DeriveArguments((BoundCall)boundRaiseEventStatement.EventInvocation);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(boundNode.Kind);
			}
		}

		internal ImmutableArray<IArgumentOperation> DeriveArguments(ImmutableArray<BoundExpression> boundArguments, ImmutableArray<ParameterSymbol> parameters, ref BitVector defaultArguments)
		{
			int length = boundArguments.Length;
			ArrayBuilder<IArgumentOperation> instance = ArrayBuilder<IArgumentOperation>.GetInstance(length);
			int num = length - 1;
			for (int i = 0; i <= num; i++)
			{
				instance.Add(DeriveArgument(i, boundArguments[i], parameters, defaultArguments[i]));
			}
			return instance.ToImmutableAndFree();
		}

		private IArgumentOperation DeriveArgument(int index, BoundExpression argument, ImmutableArray<ParameterSymbol> parameters, bool isDefault)
		{
			bool isImplicit = argument.WasCompilerGenerated && Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(argument.Syntax) != SyntaxKind.OmittedArgument;
			BoundKind kind = argument.Kind;
			if (kind == BoundKind.ByRefArgumentWithCopyBack)
			{
				BoundByRefArgumentWithCopyBack boundByRefArgumentWithCopyBack = (BoundByRefArgumentWithCopyBack)argument;
				ParameterSymbol parameter = parameters[index];
				return CreateArgumentOperation(ArgumentKind.Explicit, parameter, boundByRefArgumentWithCopyBack.OriginalArgument, CreateConversion(boundByRefArgumentWithCopyBack.InConversion), CreateConversion(boundByRefArgumentWithCopyBack.OutConversion), isImplicit);
			}
			_ = parameters.Length;
			ArgumentKind kind2 = ArgumentKind.Explicit;
			if (argument.WasCompilerGenerated)
			{
				if (isDefault)
				{
					kind2 = ArgumentKind.DefaultValue;
				}
				else if (argument.Kind == BoundKind.ArrayCreation && ((BoundArrayCreation)argument).IsParamArrayArgument)
				{
					kind2 = ArgumentKind.ParamArray;
				}
			}
			return CreateArgumentOperation(kind2, parameters[index], argument, new Conversion(Conversions.Identity), new Conversion(Conversions.Identity), isImplicit);
		}

		private IArgumentOperation CreateArgumentOperation(ArgumentKind kind, IParameterSymbol parameter, BoundNode valueNode, Conversion inConversion, Conversion outConversion, bool isImplicit)
		{
			SyntaxNode syntaxNode = ((Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(valueNode.Syntax) == SyntaxKind.OmittedArgument) ? valueNode.Syntax : (valueNode.Syntax?.Parent as ArgumentSyntax));
			IOperation operation = Create(valueNode);
			if (syntaxNode == null)
			{
				syntaxNode = operation.Syntax;
				isImplicit = true;
			}
			return new ArgumentOperation(kind, parameter, operation, inConversion, outConversion, _semanticModel, syntaxNode, isImplicit);
		}

		internal IOperation CreateReceiverOperation(BoundNode node, ISymbol symbol)
		{
			if (node == null || node.Kind == BoundKind.TypeExpression)
			{
				return null;
			}
			if (symbol != null && node.WasCompilerGenerated && symbol.IsStatic && (node.Kind == BoundKind.MeReference || node.Kind == BoundKind.WithLValueExpressionPlaceholder || node.Kind == BoundKind.WithRValueExpressionPlaceholder))
			{
				return null;
			}
			return Create(node);
		}

		private static bool ParameterIsParamArray(ParameterSymbol parameter)
		{
			if (!parameter.IsParamArray || parameter.Type.Kind != SymbolKind.ArrayType)
			{
				return false;
			}
			return ((ArrayTypeSymbol)parameter.Type).IsSZArray;
		}

		private IOperation GetChildOfBadExpression(BoundNode parent, int index)
		{
			IOperation operation = Create(GetChildOfBadExpressionBoundNode(parent, index));
			if (operation != null)
			{
				return operation;
			}
			bool wasCompilerGenerated = parent.WasCompilerGenerated;
			return OperationFactory.CreateInvalidOperation(_semanticModel, parent.Syntax, ImmutableArray<IOperation>.Empty, wasCompilerGenerated);
		}

		private static BoundExpression GetChildOfBadExpressionBoundNode(BoundNode parent, int index)
		{
			if (parent is BoundBadExpression boundBadExpression && boundBadExpression.ChildBoundNodes.Length > index)
			{
				return boundBadExpression.ChildBoundNodes[index];
			}
			return null;
		}

		private ImmutableArray<IOperation> GetObjectCreationInitializers(BoundObjectCreationExpression expression)
		{
			if (expression.InitializerOpt == null)
			{
				return ImmutableArray<IOperation>.Empty;
			}
			return expression.InitializerOpt.Initializers.SelectAsArray((BoundExpression n) => Create(n));
		}

		internal ImmutableArray<IOperation> GetAnonymousTypeCreationInitializers(BoundAnonymousTypeCreationExpression expression)
		{
			ImmutableArray<AnonymousTypeManager.AnonymousTypePropertyPublicSymbol> properties = ((AnonymousTypeManager.AnonymousTypePublicSymbol)expression.Type).Properties;
			ArrayBuilder<IOperation> instance = ArrayBuilder<IOperation>.GetInstance(expression.Arguments.Length);
			int num = 0;
			int num2 = expression.Arguments.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				IOperation operation = Create(expression.Arguments[i]);
				IOperation operation2;
				bool isImplicit;
				if (num >= expression.Declarations.Length || i != expression.Declarations[num].PropertyIndex)
				{
					IPropertySymbol propertySymbol = properties[i];
					IInstanceReferenceOperation instance2 = CreateAnonymousTypePropertyAccessImplicitReceiverOperation(propertySymbol, expression.Syntax);
					operation2 = new PropertyReferenceOperation(propertySymbol, ImmutableArray<IArgumentOperation>.Empty, instance2, _semanticModel, operation.Syntax, propertySymbol.Type, isImplicit: true);
					isImplicit = true;
				}
				else
				{
					operation2 = CreateBoundAnonymousTypePropertyAccessOperation(expression.Declarations[num]);
					num++;
					isImplicit = expression.WasCompilerGenerated;
				}
				bool isRef = false;
				SyntaxNode syntax = operation.Syntax?.Parent ?? expression.Syntax;
				ITypeSymbol type = operation2.Type;
				ConstantValue constantValue = operation.GetConstantValue();
				SimpleAssignmentOperation item = new SimpleAssignmentOperation(isRef, operation2, operation, _semanticModel, syntax, type, constantValue, isImplicit);
				instance.Add(item);
			}
			return instance.ToImmutableAndFree();
		}

		private static BoundExpression GetSingleValueCaseClauseValue(BoundSingleValueCaseClause clause)
		{
			return GetCaseClauseValue(clause.ValueOpt, clause.ConditionOpt);
		}

		internal static BoundExpression GetCaseClauseValue(BoundExpression valueOpt, BoundExpression conditionOpt)
		{
			if (valueOpt != null)
			{
				return valueOpt;
			}
			return conditionOpt.Kind switch
			{
				BoundKind.BinaryOperator => ((BoundBinaryOperator)conditionOpt).Right, 
				BoundKind.UserDefinedBinaryOperator => GetUserDefinedBinaryOperatorChildBoundNode((BoundUserDefinedBinaryOperator)conditionOpt, 1), 
				_ => throw ExceptionUtilities.UnexpectedValue(conditionOpt.Kind), 
			};
		}

		internal ImmutableArray<IVariableDeclarationOperation> GetVariableDeclarationStatementVariables(ImmutableArray<BoundLocalDeclarationBase> declarations)
		{
			IEnumerable<IGrouping<SyntaxNode, BoundLocalDeclarationBase>> enumerable = from declaration in declarations
				group declaration by (declaration.Kind == BoundKind.LocalDeclaration && VisualBasicExtensions.IsKind(declaration.Syntax, SyntaxKind.ModifiedIdentifier)) ? declaration.Syntax.Parent : declaration.Syntax;
			ArrayBuilder<IVariableDeclarationOperation> instance = ArrayBuilder<IVariableDeclarationOperation>.GetInstance();
			foreach (IGrouping<SyntaxNode, BoundLocalDeclarationBase> item in enumerable)
			{
				BoundLocalDeclarationBase boundLocalDeclarationBase = item.First();
				ImmutableArray<IVariableDeclaratorOperation> immutableArray = default(ImmutableArray<IVariableDeclaratorOperation>);
				IVariableInitializerOperation initializer = null;
				if (boundLocalDeclarationBase.Kind == BoundKind.LocalDeclaration)
				{
					immutableArray = item.Cast<BoundLocalDeclaration>().SelectAsArray(GetVariableDeclarator);
					BoundLocalDeclaration boundLocalDeclaration = (BoundLocalDeclaration)item.Last();
					if (boundLocalDeclaration.DeclarationInitializerOpt != null)
					{
						VariableDeclaratorSyntax variableDeclaratorSyntax = (VariableDeclaratorSyntax)boundLocalDeclaration.Syntax.Parent;
						SyntaxNode syntaxNode = variableDeclaratorSyntax.Initializer;
						bool isImplicit = false;
						if (boundLocalDeclaration.InitializedByAsNew)
						{
							syntaxNode = variableDeclaratorSyntax.AsClause;
						}
						else if (syntaxNode == null)
						{
							syntaxNode = boundLocalDeclaration.InitializerOpt.Syntax;
							isImplicit = true;
						}
						IOperation value = Create(boundLocalDeclaration.InitializerOpt);
						initializer = new VariableInitializerOperation(ImmutableArray<ILocalSymbol>.Empty, value, _semanticModel, syntaxNode, isImplicit);
					}
				}
				else
				{
					BoundAsNewLocalDeclarations boundAsNewLocalDeclarations = (BoundAsNewLocalDeclarations)boundLocalDeclarationBase;
					immutableArray = boundAsNewLocalDeclarations.LocalDeclarations.SelectAsArray(GetVariableDeclarator);
					AsClauseSyntax asClause = ((VariableDeclaratorSyntax)boundAsNewLocalDeclarations.Syntax).AsClause;
					Create(boundAsNewLocalDeclarations.Initializer);
					IOperation value2 = Create(boundAsNewLocalDeclarations.Initializer);
					initializer = new VariableInitializerOperation(ImmutableArray<ILocalSymbol>.Empty, value2, _semanticModel, asClause, isImplicit: false);
				}
				instance.Add(new VariableDeclarationOperation(immutableArray, initializer, ImmutableArray<IOperation>.Empty, _semanticModel, item.Key, isImplicit: false));
			}
			return instance.ToImmutableAndFree();
		}

		private IVariableDeclaratorOperation GetVariableDeclarator(BoundLocalDeclaration boundLocalDeclaration)
		{
			IVariableInitializerOperation initializer = null;
			if (boundLocalDeclaration.IdentifierInitializerOpt != null)
			{
				SyntaxNode syntax = boundLocalDeclaration.Syntax;
				IOperation value = Create(boundLocalDeclaration.IdentifierInitializerOpt);
				initializer = new VariableInitializerOperation(ImmutableArray<ILocalSymbol>.Empty, value, _semanticModel, syntax, isImplicit: true);
			}
			ImmutableArray<IOperation> empty = ImmutableArray<IOperation>.Empty;
			return new VariableDeclaratorOperation(boundLocalDeclaration.LocalSymbol, initializer, empty, _semanticModel, boundLocalDeclaration.Syntax, boundLocalDeclaration.WasCompilerGenerated);
		}

		private IVariableDeclarationGroupOperation GetUsingStatementDeclaration(ImmutableArray<BoundLocalDeclarationBase> resourceList, SyntaxNode syntax)
		{
			return new VariableDeclarationGroupOperation(GetVariableDeclarationStatementVariables(resourceList), _semanticModel, syntax, isImplicit: false);
		}

		internal IOperation GetAddRemoveHandlerStatementExpression(BoundAddRemoveHandlerStatement statement)
		{
			IOperation eventReference = Create(statement.EventAccess);
			IOperation handlerValue = Create(statement.Handler);
			bool adds = statement.Kind == BoundKind.AddHandlerStatement;
			return new EventAssignmentOperation(eventReference, handlerValue, adds, _semanticModel, statement.Syntax, null, isImplicit: true);
		}

		private (IOperation Operation, Conversion Conversion, bool IsDelegateCreation) GetConversionInfo(BoundConversionOrCast boundConversion)
		{
			Conversion item = CreateConversion(boundConversion);
			BoundExpression conversionOperand = GetConversionOperand(boundConversion);
			if (item.IsIdentity && boundConversion.ExplicitCastInCode)
			{
				(IOperation, Conversion, bool) result = TryGetAdjustedConversionInfo(boundConversion, conversionOperand);
				if (result.Item1 != null)
				{
					return result;
				}
			}
			return (!IsDelegateCreation(boundConversion.Syntax, conversionOperand, boundConversion.Type)) ? (Create(conversionOperand), item, false) : (CreateDelegateCreationConversionOperand(conversionOperand), item, true);
		}

		private (IOperation Operation, Conversion Conversion, bool IsDelegateCreation) TryGetAdjustedConversionInfo(BoundConversionOrCast topLevelConversion, BoundExpression boundOperand)
		{
			(IOperation, Conversion, bool) result;
			if (boundOperand.Kind == BoundKind.Parenthesized)
			{
				(IOperation, Conversion, bool) tuple = TryGetAdjustedConversionInfo(topLevelConversion, ((BoundParenthesized)boundOperand).Expression);
				if (tuple.Item1 != null)
				{
					result = (new ParenthesizedOperation(tuple.Item1, _semanticModel, boundOperand.Syntax, tuple.Item1.Type, boundOperand.ConstantValueOpt, boundOperand.WasCompilerGenerated), tuple.Item2, tuple.Item3);
					goto IL_0132;
				}
			}
			else if (boundOperand.Kind == topLevelConversion.Kind)
			{
				BoundConversionOrCast boundConversionOrCast = (BoundConversionOrCast)boundOperand;
				BoundExpression conversionOperand = GetConversionOperand(boundConversionOrCast);
				if (boundConversionOrCast.Syntax == conversionOperand.Syntax && !TypeSymbol.Equals(boundConversionOrCast.Type, conversionOperand.Type, TypeCompareKind.ConsiderEverything) && boundConversionOrCast.ExplicitCastInCode && TypeSymbol.Equals(topLevelConversion.Type, boundConversionOrCast.Type, TypeCompareKind.ConsiderEverything))
				{
					return GetConversionInfo(boundConversionOrCast);
				}
			}
			else if (VisualBasicExtensions.IsKind(boundOperand.Syntax, SyntaxKind.AddressOfExpression) && TypeSymbol.Equals(topLevelConversion.Type, boundOperand.Type, TypeCompareKind.ConsiderEverything) && IsDelegateCreation(topLevelConversion.Syntax, boundOperand, boundOperand.Type))
			{
				result = (CreateDelegateCreationConversionOperand(boundOperand), default(Conversion), true);
				goto IL_0132;
			}
			result = default((IOperation, Conversion, bool));
			goto IL_0132;
			IL_0132:
			return result;
		}

		private static BoundExpression GetConversionOperand(BoundConversionOrCast boundConversion)
		{
			if ((boundConversion.ConversionKind & ConversionKind.UserDefined) == ConversionKind.UserDefined)
			{
				return ((BoundUserDefinedConversion)boundConversion.Operand).Operand;
			}
			return boundConversion.Operand;
		}

		private IOperation CreateDelegateCreationConversionOperand(BoundExpression operand)
		{
			if (operand.Kind == BoundKind.DelegateCreationExpression)
			{
				return CreateBoundDelegateCreationExpressionChildOperation((BoundDelegateCreationExpression)operand);
			}
			return Create(operand);
		}

		private static Conversion CreateConversion(BoundExpression expression)
		{
			Conversion result;
			if (expression.Kind != BoundKind.Conversion)
			{
				result = ((expression.Kind != BoundKind.TryCast && expression.Kind != BoundKind.DirectCast) ? new Conversion(Conversions.Identity) : new Conversion(KeyValuePairUtil.Create<ConversionKind, MethodSymbol>(((BoundConversionOrCast)expression).ConversionKind, null)));
			}
			else
			{
				BoundConversion boundConversion = (BoundConversion)expression;
				ConversionKind conversionKind = boundConversion.ConversionKind;
				MethodSymbol value = null;
				if (conversionKind.HasFlag(ConversionKind.UserDefined) && boundConversion.Operand.Kind == BoundKind.UserDefinedConversion)
				{
					value = ((BoundUserDefinedConversion)boundConversion.Operand).Call.Method;
				}
				result = new Conversion(KeyValuePairUtil.Create(conversionKind, value));
			}
			return result;
		}

		private static bool IsDelegateCreation(SyntaxNode conversionSyntax, BoundNode operand, TypeSymbol targetType)
		{
			if (!TypeSymbolExtensions.IsDelegateType(targetType))
			{
				return false;
			}
			bool num = Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(operand.Syntax) == SyntaxKind.AddressOfExpression && (Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(conversionSyntax) == SyntaxKind.CTypeExpression || Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(conversionSyntax) == SyntaxKind.DirectCastExpression || Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(conversionSyntax) == SyntaxKind.TryCastExpression || Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(conversionSyntax) == SyntaxKind.ObjectCreationExpression || (Microsoft.CodeAnalysis.VisualBasic.VisualBasicExtensions.Kind(conversionSyntax) == SyntaxKind.AddressOfExpression && conversionSyntax == operand.Syntax));
			bool flag = operand.Kind == BoundKind.Lambda || operand.Kind == BoundKind.QueryLambda || operand.Kind == BoundKind.UnboundLambda;
			return num || flag;
		}

		private static BoundNode RewriteQueryLambda(BoundQueryLambda node)
		{
			BoundLambda node2 = (BoundLambda)new QueryLambdaRewriterPass1().VisitQueryLambda(node);
			return new QueryLambdaRewriterPass2().VisitLambda(node2);
		}
	}
}
