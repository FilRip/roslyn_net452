using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LambdaUtilities
	{
		public static bool IsLambda(SyntaxNode node)
		{
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
			case SyntaxKind.FunctionAggregation:
			case SyntaxKind.WhereClause:
			case SyntaxKind.SkipWhileClause:
			case SyntaxKind.TakeWhileClause:
			case SyntaxKind.AscendingOrdering:
			case SyntaxKind.DescendingOrdering:
				return true;
			case SyntaxKind.ExpressionRangeVariable:
				return IsLambdaExpressionRangeVariable(node);
			case SyntaxKind.CollectionRangeVariable:
				return IsLambdaCollectionRangeVariable(node);
			case SyntaxKind.JoinCondition:
				return IsLambdaJoinCondition(node);
			default:
				return false;
			}
		}

		public static bool IsNotLambda(SyntaxNode node)
		{
			return !IsLambda(node);
		}

		public static SyntaxNode GetLambda(SyntaxNode lambdaBody)
		{
			return lambdaBody.Parent;
		}

		public static SyntaxNode GetCorrespondingLambdaBody(SyntaxNode oldBody, SyntaxNode newLambdaOrPeer)
		{
			SyntaxNode lambda = GetLambda(oldBody);
			SyntaxNode lambdaBody = null;
			if (TryGetSimpleLambdaBody(newLambdaOrPeer, out lambdaBody))
			{
				return lambdaBody;
			}
			switch (VisualBasicExtensions.Kind(lambda))
			{
			case SyntaxKind.ExpressionRangeVariable:
				return GetExpressionRangeVariableLambdaBody((ExpressionRangeVariableSyntax)newLambdaOrPeer);
			case SyntaxKind.CollectionRangeVariable:
				return ((CollectionRangeVariableSyntax)newLambdaOrPeer).Expression;
			case SyntaxKind.JoinCondition:
			{
				JoinConditionSyntax obj = (JoinConditionSyntax)lambda;
				JoinClauseSyntax joinClause = (JoinClauseSyntax)((JoinConditionSyntax)newLambdaOrPeer).Parent;
				return (obj.Left == oldBody) ? GetJoinLeftLambdaBody(joinClause) : GetJoinRightLambdaBody(joinClause);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(lambda));
			}
		}

		public static bool IsLambdaBody(SyntaxNode node)
		{
			SyntaxNode lambdaBody = null;
			if (IsLambdaBodyStatementOrExpression(node, out lambdaBody))
			{
				return node == lambdaBody;
			}
			return false;
		}

		public static bool IsLambdaBodyStatementOrExpression(SyntaxNode node, out SyntaxNode lambdaBody = null)
		{
			SyntaxNode syntaxNode = node?.Parent;
			if (syntaxNode == null)
			{
				lambdaBody = null;
				return false;
			}
			if (TryGetSimpleLambdaBody(syntaxNode, out lambdaBody))
			{
				return true;
			}
			switch (VisualBasicExtensions.Kind(syntaxNode))
			{
			case SyntaxKind.ExpressionRangeVariable:
			{
				ExpressionRangeVariableSyntax expressionRangeVariableSyntax = (ExpressionRangeVariableSyntax)syntaxNode;
				if (node == expressionRangeVariableSyntax.Expression)
				{
					lambdaBody = GetExpressionRangeVariableLambdaBody(expressionRangeVariableSyntax);
					return true;
				}
				break;
			}
			case SyntaxKind.CollectionRangeVariable:
			{
				CollectionRangeVariableSyntax collectionRangeVariableSyntax = (CollectionRangeVariableSyntax)syntaxNode;
				if (node != collectionRangeVariableSyntax.Expression)
				{
					break;
				}
				SyntaxNode parent = syntaxNode.Parent;
				if (IsLambdaCollectionRangeVariable(syntaxNode))
				{
					lambdaBody = node;
					return true;
				}
				if (IsJoinClause(parent))
				{
					_ = parent.Parent;
					do
					{
						parent = parent.Parent;
					}
					while (IsJoinClause(parent));
					if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent, SyntaxKind.AggregateClause) && !IsQueryStartingClause(parent))
					{
						lambdaBody = GetAggregateLambdaBody((AggregateClauseSyntax)parent);
						return true;
					}
				}
				break;
			}
			case SyntaxKind.SkipClause:
			case SyntaxKind.TakeClause:
			{
				SyntaxNode parent2 = syntaxNode.Parent;
				if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(parent2, SyntaxKind.AggregateClause) && !IsQueryStartingClause(parent2))
				{
					lambdaBody = GetAggregateLambdaBody((AggregateClauseSyntax)parent2);
					return true;
				}
				break;
			}
			case SyntaxKind.JoinCondition:
			{
				JoinConditionSyntax joinConditionSyntax = (JoinConditionSyntax)syntaxNode;
				JoinClauseSyntax joinClause = (JoinClauseSyntax)syntaxNode.Parent;
				if (node == joinConditionSyntax.Left)
				{
					lambdaBody = GetJoinLeftLambdaBody(joinClause);
				}
				else
				{
					lambdaBody = GetJoinRightLambdaBody(joinClause);
				}
				return true;
			}
			}
			lambdaBody = null;
			return false;
		}

		private static bool IsJoinClause(SyntaxNode node)
		{
			if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(node, SyntaxKind.GroupJoinClause))
			{
				return Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(node, SyntaxKind.SimpleJoinClause);
			}
			return true;
		}

		internal static VisualBasicSyntaxNode GetLambdaExpressionLambdaBody(LambdaExpressionSyntax lambda)
		{
			return lambda.SubOrFunctionHeader;
		}

		internal static VisualBasicSyntaxNode GetFromOrAggregateVariableLambdaBody(CollectionRangeVariableSyntax rangeVariable)
		{
			return rangeVariable.Expression;
		}

		internal static VisualBasicSyntaxNode GetOrderingLambdaBody(OrderingSyntax ordering)
		{
			return ordering.Expression;
		}

		internal static VisualBasicSyntaxNode GetAggregationLambdaBody(FunctionAggregationSyntax aggregation)
		{
			return aggregation.Argument;
		}

		internal static VisualBasicSyntaxNode GetLetVariableLambdaBody(ExpressionRangeVariableSyntax rangeVariable)
		{
			return rangeVariable.Expression;
		}

		internal static VisualBasicSyntaxNode GetSelectLambdaBody(SelectClauseSyntax selectClause)
		{
			return selectClause.Variables.First().Expression;
		}

		internal static VisualBasicSyntaxNode GetAggregateLambdaBody(AggregateClauseSyntax aggregateClause)
		{
			return aggregateClause.Variables.First().Expression;
		}

		internal static VisualBasicSyntaxNode GetGroupByItemsLambdaBody(GroupByClauseSyntax groupByClause)
		{
			return groupByClause.Items.First().Expression;
		}

		internal static VisualBasicSyntaxNode GetGroupByKeysLambdaBody(GroupByClauseSyntax groupByClause)
		{
			return groupByClause.Keys.First().Expression;
		}

		internal static VisualBasicSyntaxNode GetJoinLeftLambdaBody(JoinClauseSyntax joinClause)
		{
			return joinClause.JoinConditions.First().Left;
		}

		internal static VisualBasicSyntaxNode GetJoinRightLambdaBody(JoinClauseSyntax joinClause)
		{
			return joinClause.JoinConditions.First().Right;
		}

		private static SyntaxNode GetExpressionRangeVariableLambdaBody(ExpressionRangeVariableSyntax rangeVariable)
		{
			VisualBasicSyntaxNode parent = rangeVariable.Parent;
			switch (parent.Kind())
			{
			case SyntaxKind.LetClause:
				return GetLetVariableLambdaBody(rangeVariable);
			case SyntaxKind.SelectClause:
				return GetSelectLambdaBody((SelectClauseSyntax)parent);
			case SyntaxKind.GroupByClause:
			{
				GroupByClauseSyntax groupByClauseSyntax = (GroupByClauseSyntax)parent;
				if (rangeVariable.SpanStart < groupByClauseSyntax.ByKeyword.SpanStart || (rangeVariable.SpanStart == groupByClauseSyntax.ByKeyword.SpanStart && rangeVariable == groupByClauseSyntax.Items.Last()))
				{
					return GetGroupByItemsLambdaBody(groupByClauseSyntax);
				}
				return GetGroupByKeysLambdaBody(groupByClauseSyntax);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(parent.Kind());
			}
		}

		public static bool TryGetLambdaBodies(SyntaxNode node, out SyntaxNode lambdaBody1, out SyntaxNode lambdaBody2)
		{
			lambdaBody1 = null;
			lambdaBody2 = null;
			if (TryGetSimpleLambdaBody(node, out lambdaBody1))
			{
				return true;
			}
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.CollectionRangeVariable:
				if (!IsLambdaCollectionRangeVariable(node))
				{
					return false;
				}
				lambdaBody1 = ((CollectionRangeVariableSyntax)node).Expression;
				break;
			case SyntaxKind.ExpressionRangeVariable:
				if (!IsLambdaExpressionRangeVariable(node))
				{
					return false;
				}
				lambdaBody1 = ((ExpressionRangeVariableSyntax)node).Expression;
				break;
			case SyntaxKind.JoinCondition:
			{
				JoinConditionSyntax joinConditionSyntax = ((JoinClauseSyntax)node.Parent).JoinConditions.First();
				if (node != joinConditionSyntax)
				{
					return false;
				}
				lambdaBody1 = joinConditionSyntax.Left;
				lambdaBody2 = joinConditionSyntax.Right;
				break;
			}
			default:
				return false;
			}
			return true;
		}

		internal static IEnumerable<SyntaxNode> GetLambdaBodyExpressionsAndStatements(SyntaxNode lambdaBody)
		{
			SyntaxNode lambda = GetLambda(lambdaBody);
			switch (VisualBasicExtensions.Kind(lambda))
			{
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
				return SpecializedCollections.SingletonEnumerable(((SingleLineLambdaExpressionSyntax)lambda).Body);
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
				return ((MultiLineLambdaExpressionSyntax)lambda).Statements;
			case SyntaxKind.FunctionAggregation:
			case SyntaxKind.WhereClause:
			case SyntaxKind.SkipWhileClause:
			case SyntaxKind.TakeWhileClause:
			case SyntaxKind.AscendingOrdering:
			case SyntaxKind.DescendingOrdering:
				return SpecializedCollections.SingletonEnumerable(lambdaBody);
			case SyntaxKind.ExpressionRangeVariable:
			{
				SyntaxNode parent2 = lambda.Parent;
				switch (VisualBasicExtensions.Kind(parent2))
				{
				case SyntaxKind.LetClause:
					return SpecializedCollections.SingletonEnumerable(lambdaBody);
				case SyntaxKind.SelectClause:
					return EnumerateExpressions(((SelectClauseSyntax)parent2).Variables);
				case SyntaxKind.GroupByClause:
				{
					GroupByClauseSyntax groupByClauseSyntax = (GroupByClauseSyntax)parent2;
					if (lambdaBody.SpanStart < groupByClauseSyntax.ByKeyword.SpanStart)
					{
						return EnumerateExpressions(groupByClauseSyntax.Items);
					}
					return EnumerateExpressions(groupByClauseSyntax.Keys);
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(parent2));
				}
			}
			case SyntaxKind.CollectionRangeVariable:
			{
				SyntaxNode parent = lambda.Parent;
				switch (VisualBasicExtensions.Kind(parent))
				{
				case SyntaxKind.FromClause:
					return SpecializedCollections.SingletonEnumerable(lambdaBody);
				case SyntaxKind.AggregateClause:
				{
					AggregateClauseSyntax aggregateClauseSyntax = (AggregateClauseSyntax)parent;
					if (lambda == aggregateClauseSyntax.Variables.First())
					{
						return GetAggregateLambdaBodyExpressions(aggregateClauseSyntax);
					}
					return SpecializedCollections.SingletonEnumerable(lambdaBody);
				}
				default:
					throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(parent));
				}
			}
			case SyntaxKind.JoinCondition:
			{
				JoinClauseSyntax clause = (JoinClauseSyntax)lambda.Parent;
				JoinConditionSyntax joinConditionSyntax = (JoinConditionSyntax)lambda;
				if (lambdaBody == joinConditionSyntax.Left)
				{
					return EnumerateJoinClauseLeftExpressions(clause);
				}
				return EnumerateJoinClauseRightExpressions(clause);
			}
			default:
				throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(lambda));
			}
		}

		private static IEnumerable<SyntaxNode> GetAggregateLambdaBodyExpressions(AggregateClauseSyntax clause)
		{
			ArrayBuilder<SyntaxNode> instance = ArrayBuilder<SyntaxNode>.GetInstance();
			instance.Add(clause.Variables.First().Expression);
			SyntaxList<QueryClauseSyntax>.Enumerator enumerator = clause.AdditionalQueryOperators.GetEnumerator();
			while (enumerator.MoveNext())
			{
				QueryClauseSyntax current = enumerator.Current;
				switch (current.Kind())
				{
				case SyntaxKind.SkipClause:
				case SyntaxKind.TakeClause:
					instance.Add(((PartitionClauseSyntax)current).Count);
					break;
				case SyntaxKind.SimpleJoinClause:
				case SyntaxKind.GroupJoinClause:
					AddFirstJoinVariableRecursive(instance, (JoinClauseSyntax)current);
					break;
				}
			}
			return instance.ToImmutableAndFree();
		}

		private static void AddFirstJoinVariableRecursive(ArrayBuilder<SyntaxNode> result, JoinClauseSyntax joinClause)
		{
			result.Add(joinClause.JoinedVariables.First().Expression);
			SyntaxList<JoinClauseSyntax>.Enumerator enumerator = joinClause.AdditionalJoins.GetEnumerator();
			while (enumerator.MoveNext())
			{
				JoinClauseSyntax current = enumerator.Current;
				AddFirstJoinVariableRecursive(result, current);
			}
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_24_EnumerateExpressions))]
		private static IEnumerable<SyntaxNode> EnumerateExpressions(SeparatedSyntaxList<ExpressionRangeVariableSyntax> variables)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_24_EnumerateExpressions(-2)
			{
				_0024P_variables = variables
			};
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_25_EnumerateJoinClauseLeftExpressions))]
		private static IEnumerable<SyntaxNode> EnumerateJoinClauseLeftExpressions(JoinClauseSyntax clause)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_25_EnumerateJoinClauseLeftExpressions(-2)
			{
				_0024P_clause = clause
			};
		}

		[IteratorStateMachine(typeof(VB_0024StateMachine_26_EnumerateJoinClauseRightExpressions))]
		private static IEnumerable<SyntaxNode> EnumerateJoinClauseRightExpressions(JoinClauseSyntax clause)
		{
			//yield-return decompiler failed: Method not found
			return new VB_0024StateMachine_26_EnumerateJoinClauseRightExpressions(-2)
			{
				_0024P_clause = clause
			};
		}

		private static bool TryGetSimpleLambdaBody(SyntaxNode node, out SyntaxNode lambdaBody)
		{
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
				lambdaBody = GetLambdaExpressionLambdaBody((LambdaExpressionSyntax)node);
				break;
			case SyntaxKind.WhereClause:
				lambdaBody = ((WhereClauseSyntax)node).Condition;
				break;
			case SyntaxKind.SkipWhileClause:
			case SyntaxKind.TakeWhileClause:
				lambdaBody = ((PartitionWhileClauseSyntax)node).Condition;
				break;
			case SyntaxKind.AscendingOrdering:
			case SyntaxKind.DescendingOrdering:
				lambdaBody = GetOrderingLambdaBody((OrderingSyntax)node);
				break;
			case SyntaxKind.FunctionAggregation:
				lambdaBody = GetAggregationLambdaBody((FunctionAggregationSyntax)node);
				if (lambdaBody == null)
				{
					return false;
				}
				break;
			default:
				lambdaBody = null;
				return false;
			}
			return true;
		}

		internal static bool IsLambdaExpressionRangeVariable(SyntaxNode expressionRangeVariable)
		{
			SyntaxNode parent = expressionRangeVariable.Parent;
			switch (VisualBasicExtensions.Kind(parent))
			{
			case SyntaxKind.LetClause:
				return true;
			case SyntaxKind.SelectClause:
			{
				SelectClauseSyntax selectClauseSyntax = (SelectClauseSyntax)parent;
				return expressionRangeVariable == selectClauseSyntax.Variables.First();
			}
			case SyntaxKind.GroupByClause:
			{
				GroupByClauseSyntax groupByClauseSyntax = (GroupByClauseSyntax)parent;
				return expressionRangeVariable == groupByClauseSyntax.Keys.First() || expressionRangeVariable == groupByClauseSyntax.Items.FirstOrDefault();
			}
			default:
				return false;
			}
		}

		internal static bool IsLambdaCollectionRangeVariable(SyntaxNode collectionRangeVariable)
		{
			SyntaxNode parent = collectionRangeVariable.Parent;
			if (IsJoinClause(parent))
			{
				return false;
			}
			if (IsQueryStartingClause(parent))
			{
				return collectionRangeVariable != GetCollectionRangeVariables(parent).First();
			}
			return true;
		}

		private static bool IsQueryStartingClause(SyntaxNode clause)
		{
			if (Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(clause.Parent, SyntaxKind.QueryExpression))
			{
				return clause == ((QueryExpressionSyntax)clause.Parent).Clauses.First();
			}
			return false;
		}

		private static SeparatedSyntaxList<CollectionRangeVariableSyntax> GetCollectionRangeVariables(SyntaxNode clause)
		{
			switch (VisualBasicExtensions.Kind(clause))
			{
			case SyntaxKind.FromClause:
				return ((FromClauseSyntax)clause).Variables;
			case SyntaxKind.AggregateClause:
				return ((AggregateClauseSyntax)clause).Variables;
			case SyntaxKind.SimpleJoinClause:
			case SyntaxKind.GroupJoinClause:
				return ((JoinClauseSyntax)clause).JoinedVariables;
			default:
				throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(clause));
			}
		}

		internal static bool IsLambdaJoinCondition(SyntaxNode joinCondition)
		{
			return joinCondition == ((JoinClauseSyntax)joinCondition.Parent).JoinConditions.First();
		}

		public static bool AreEquivalentIgnoringLambdaBodies(SyntaxNode oldNode, SyntaxNode newNode)
		{
			IEnumerable<SyntaxToken> first = oldNode.DescendantTokens(delegate(SyntaxNode node)
			{
				if (node != oldNode)
				{
					SyntaxNode lambdaBody2 = null;
					return !IsLambdaBodyStatementOrExpression(node, out lambdaBody2);
				}
				return true;
			});
			IEnumerable<SyntaxToken> second = newNode.DescendantTokens(delegate(SyntaxNode node)
			{
				if (node != newNode)
				{
					SyntaxNode lambdaBody = null;
					return !IsLambdaBodyStatementOrExpression(node, out lambdaBody);
				}
				return true;
			});
			return first.SequenceEqual(second, SyntaxFactory.AreEquivalent);
		}

		internal static bool IsNonUserCodeQueryLambda(SyntaxNode syntax)
		{
			if (!Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.GroupJoinClause) && !Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.SimpleJoinClause) && !Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.AggregateClause) && !Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.FromClause) && !Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.GroupByClause))
			{
				return Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntax, SyntaxKind.SimpleAsClause);
			}
			return true;
		}

		internal static bool IsClosureScope(SyntaxNode node)
		{
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.SubBlock:
			case SyntaxKind.FunctionBlock:
			case SyntaxKind.ConstructorBlock:
			case SyntaxKind.OperatorBlock:
			case SyntaxKind.GetAccessorBlock:
			case SyntaxKind.SetAccessorBlock:
			case SyntaxKind.AddHandlerAccessorBlock:
			case SyntaxKind.RemoveHandlerAccessorBlock:
			case SyntaxKind.RaiseEventAccessorBlock:
				return true;
			case SyntaxKind.WhileBlock:
			case SyntaxKind.UsingBlock:
			case SyntaxKind.SyncLockBlock:
			case SyntaxKind.WithBlock:
			case SyntaxKind.SingleLineIfStatement:
			case SyntaxKind.SingleLineElseClause:
			case SyntaxKind.MultiLineIfBlock:
			case SyntaxKind.ElseIfBlock:
			case SyntaxKind.ElseBlock:
			case SyntaxKind.TryBlock:
			case SyntaxKind.CatchBlock:
			case SyntaxKind.FinallyBlock:
			case SyntaxKind.CaseBlock:
			case SyntaxKind.CaseElseBlock:
			case SyntaxKind.ForBlock:
			case SyntaxKind.ForEachBlock:
			case SyntaxKind.SimpleDoLoopBlock:
			case SyntaxKind.DoWhileLoopBlock:
			case SyntaxKind.DoUntilLoopBlock:
			case SyntaxKind.DoLoopWhileBlock:
			case SyntaxKind.DoLoopUntilBlock:
				return true;
			case SyntaxKind.AggregateClause:
			case SyntaxKind.SimpleJoinClause:
			case SyntaxKind.GroupJoinClause:
				return true;
			case SyntaxKind.SingleLineFunctionLambdaExpression:
			case SyntaxKind.SingleLineSubLambdaExpression:
			case SyntaxKind.MultiLineFunctionLambdaExpression:
			case SyntaxKind.MultiLineSubLambdaExpression:
				return true;
			case SyntaxKind.ModuleBlock:
			case SyntaxKind.StructureBlock:
			case SyntaxKind.ClassBlock:
				return true;
			default:
				if (IsLambdaBody(node))
				{
					return true;
				}
				if (node.Parent?.Parent != null && node.Parent!.Parent!.Parent == null)
				{
					return true;
				}
				return false;
			}
		}
	}
}
