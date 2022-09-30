using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class SyntaxNodeExtensions
	{
		public static TNode WithAnnotations<TNode>(this TNode node, params SyntaxAnnotation[] annotations) where TNode : VisualBasicSyntaxNode
		{
			return (TNode)node.Green.SetAnnotations(annotations).CreateRed();
		}

		public static Microsoft.CodeAnalysis.VisualBasic.Syntax.WithStatementSyntax ContainingWithStatement(this VisualBasicSyntaxNode node)
		{
			if (node == null)
			{
				return null;
			}
			for (node = node.Parent; node != null; node = node.Parent)
			{
				switch (node.Kind())
				{
				case SyntaxKind.WithBlock:
					return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.WithBlockSyntax)node).WithStatement;
				default:
					continue;
				case SyntaxKind.SubBlock:
				case SyntaxKind.FunctionBlock:
				case SyntaxKind.ConstructorBlock:
				case SyntaxKind.OperatorBlock:
				case SyntaxKind.PropertyBlock:
				case SyntaxKind.EventBlock:
					break;
				}
				break;
			}
			return null;
		}

		public static void GetAncestors<T, C>(this VisualBasicSyntaxNode node, ArrayBuilder<T> result) where T : VisualBasicSyntaxNode where C : VisualBasicSyntaxNode
		{
			VisualBasicSyntaxNode parent = node.Parent;
			while (parent != null && !(parent is C))
			{
				if (parent is T)
				{
					result.Add((T)parent);
				}
				parent = parent.Parent;
			}
			result.ReverseContents();
		}

		public static T GetAncestorOrSelf<T>(this VisualBasicSyntaxNode node) where T : VisualBasicSyntaxNode
		{
			while (node != null)
			{
				if (node is T result)
				{
					return result;
				}
				node = node.Parent;
			}
			return null;
		}

		public static bool IsLambdaExpressionSyntax(this SyntaxNode @this)
		{
			SyntaxKind syntaxKind = VisualBasicExtensions.Kind(@this);
			if (syntaxKind == SyntaxKind.SingleLineFunctionLambdaExpression || syntaxKind - 342 <= SyntaxKind.EmptyStatement)
			{
				return true;
			}
			return false;
		}

		internal static SyntaxToken ExtractAnonymousTypeMemberName(this Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax input, out Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax failedToInferFromXmlName)
		{
			failedToInferFromXmlName = null;
			SyntaxToken result;
			while (true)
			{
				switch (input.Kind())
				{
				case SyntaxKind.IdentifierName:
					return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)input).Identifier;
				case SyntaxKind.XmlName:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax xmlNameSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlNameSyntax)input;
					if (!Scanner.IsIdentifier(xmlNameSyntax.LocalName.ToString()))
					{
						failedToInferFromXmlName = xmlNameSyntax;
						result = default(SyntaxToken);
						break;
					}
					return xmlNameSyntax.LocalName;
				}
				case SyntaxKind.XmlBracketedName:
					input = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlBracketedNameSyntax)input).Name;
					continue;
				case SyntaxKind.SimpleMemberAccessExpression:
				case SyntaxKind.DictionaryAccessExpression:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax memberAccessExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax)input;
					if (input.Kind() == SyntaxKind.SimpleMemberAccessExpression)
					{
						Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax2 = memberAccessExpressionSyntax.Expression ?? GetCorrespondingConditionalAccessReceiver(memberAccessExpressionSyntax);
						if (expressionSyntax2 != null)
						{
							SyntaxKind syntaxKind2 = expressionSyntax2.Kind();
							if (syntaxKind2 - 293 <= SyntaxKind.List)
							{
								input = expressionSyntax2;
								continue;
							}
						}
					}
					input = memberAccessExpressionSyntax.Name;
					continue;
				}
				case SyntaxKind.XmlElementAccessExpression:
				case SyntaxKind.XmlDescendantAccessExpression:
				case SyntaxKind.XmlAttributeAccessExpression:
					input = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax)input).Name;
					continue;
				case SyntaxKind.InvocationExpression:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax invocationExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax)input;
					Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = invocationExpressionSyntax.Expression ?? GetCorrespondingConditionalAccessReceiver(invocationExpressionSyntax);
					if (expressionSyntax != null)
					{
						if (invocationExpressionSyntax.ArgumentList == null || invocationExpressionSyntax.ArgumentList.Arguments.Count == 0)
						{
							input = expressionSyntax;
							continue;
						}
						if (invocationExpressionSyntax.ArgumentList.Arguments.Count == 1)
						{
							SyntaxKind syntaxKind = expressionSyntax.Kind();
							if (syntaxKind - 293 <= SyntaxKind.List)
							{
								input = expressionSyntax;
								continue;
							}
						}
					}
					goto default;
				}
				case SyntaxKind.ConditionalAccessExpression:
					input = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax)input).WhenNotNull;
					continue;
				default:
					result = default(SyntaxToken);
					break;
				}
				break;
			}
			return result;
		}

		private static Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax GetCorrespondingConditionalAccessReceiver(Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax node)
		{
			return GetCorrespondingConditionalAccessExpression(node)?.Expression;
		}

		internal static Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax GetCorrespondingConditionalAccessExpression(this Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax node)
		{
			VisualBasicSyntaxNode visualBasicSyntaxNode = node;
			for (VisualBasicSyntaxNode parent = visualBasicSyntaxNode.Parent; parent != null; parent = visualBasicSyntaxNode.Parent)
			{
				switch (parent.Kind())
				{
				case SyntaxKind.SimpleMemberAccessExpression:
				case SyntaxKind.DictionaryAccessExpression:
					if (((Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax)parent).Expression != visualBasicSyntaxNode)
					{
						return null;
					}
					break;
				case SyntaxKind.XmlElementAccessExpression:
				case SyntaxKind.XmlDescendantAccessExpression:
				case SyntaxKind.XmlAttributeAccessExpression:
					if (((Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax)parent).Base != visualBasicSyntaxNode)
					{
						return null;
					}
					break;
				case SyntaxKind.InvocationExpression:
					if (((Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax)parent).Expression != visualBasicSyntaxNode)
					{
						return null;
					}
					break;
				case SyntaxKind.ConditionalAccessExpression:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax)parent;
					if (conditionalAccessExpressionSyntax.WhenNotNull == visualBasicSyntaxNode)
					{
						return conditionalAccessExpressionSyntax;
					}
					if (conditionalAccessExpressionSyntax.Expression != visualBasicSyntaxNode)
					{
						return null;
					}
					break;
				}
				default:
					return null;
				}
				visualBasicSyntaxNode = parent;
			}
			return null;
		}

		internal static Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax GetLeafAccess(this Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax conditionalAccess)
		{
			Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionSyntax expressionSyntax = conditionalAccess.WhenNotNull;
			while (true)
			{
				switch (expressionSyntax.Kind())
				{
				case SyntaxKind.SimpleMemberAccessExpression:
				case SyntaxKind.DictionaryAccessExpression:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax memberAccessExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.MemberAccessExpressionSyntax)expressionSyntax;
					if (memberAccessExpressionSyntax.Expression == null)
					{
						return memberAccessExpressionSyntax;
					}
					expressionSyntax = memberAccessExpressionSyntax.Expression;
					break;
				}
				case SyntaxKind.XmlElementAccessExpression:
				case SyntaxKind.XmlDescendantAccessExpression:
				case SyntaxKind.XmlAttributeAccessExpression:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax xmlMemberAccessExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.XmlMemberAccessExpressionSyntax)expressionSyntax;
					if (xmlMemberAccessExpressionSyntax.Base == null)
					{
						return xmlMemberAccessExpressionSyntax;
					}
					expressionSyntax = xmlMemberAccessExpressionSyntax.Base;
					break;
				}
				case SyntaxKind.InvocationExpression:
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax invocationExpressionSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.InvocationExpressionSyntax)expressionSyntax;
					if (invocationExpressionSyntax.Expression == null)
					{
						return invocationExpressionSyntax;
					}
					expressionSyntax = invocationExpressionSyntax.Expression;
					break;
				}
				case SyntaxKind.ConditionalAccessExpression:
					expressionSyntax = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.ConditionalAccessExpressionSyntax)expressionSyntax).Expression;
					if (expressionSyntax == null)
					{
						return null;
					}
					break;
				default:
					return null;
				}
			}
		}

		public static bool AllAreMissing(this IEnumerable<VisualBasicSyntaxNode> arguments, SyntaxKind kind)
		{
			return !arguments.Any((VisualBasicSyntaxNode arg) => arg.Kind() != kind || !((Microsoft.CodeAnalysis.VisualBasic.Syntax.IdentifierNameSyntax)arg).IsMissing);
		}

		public static bool AllAreMissingIdentifierName(this IEnumerable<VisualBasicSyntaxNode> arguments)
		{
			return AllAreMissing(arguments, SyntaxKind.IdentifierName);
		}

		public static SyntaxToken QueryClauseKeywordOrRangeVariableIdentifier(this SyntaxNode syntax)
		{
			switch (VisualBasicExtensions.Kind(syntax))
			{
			case SyntaxKind.CollectionRangeVariable:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.CollectionRangeVariableSyntax)syntax).Identifier.Identifier;
			case SyntaxKind.ExpressionRangeVariable:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.ExpressionRangeVariableSyntax)syntax).NameEquals.Identifier.Identifier;
			case SyntaxKind.FromClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.FromClauseSyntax)syntax).FromKeyword;
			case SyntaxKind.LetClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.LetClauseSyntax)syntax).LetKeyword;
			case SyntaxKind.AggregateClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.AggregateClauseSyntax)syntax).AggregateKeyword;
			case SyntaxKind.DistinctClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.DistinctClauseSyntax)syntax).DistinctKeyword;
			case SyntaxKind.WhereClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.WhereClauseSyntax)syntax).WhereKeyword;
			case SyntaxKind.SkipWhileClause:
			case SyntaxKind.TakeWhileClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionWhileClauseSyntax)syntax).SkipOrTakeKeyword;
			case SyntaxKind.SkipClause:
			case SyntaxKind.TakeClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.PartitionClauseSyntax)syntax).SkipOrTakeKeyword;
			case SyntaxKind.GroupByClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupByClauseSyntax)syntax).GroupKeyword;
			case SyntaxKind.GroupJoinClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.GroupJoinClauseSyntax)syntax).GroupKeyword;
			case SyntaxKind.SimpleJoinClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.SimpleJoinClauseSyntax)syntax).JoinKeyword;
			case SyntaxKind.OrderByClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.OrderByClauseSyntax)syntax).OrderKeyword;
			case SyntaxKind.SelectClause:
				return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.SelectClauseSyntax)syntax).SelectKeyword;
			default:
				throw ExceptionUtilities.UnexpectedValue(VisualBasicExtensions.Kind(syntax));
			}
		}

		internal static Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax EnclosingStructuredTrivia(this VisualBasicSyntaxNode node)
		{
			while (node != null)
			{
				if (node.IsStructuredTrivia)
				{
					return (Microsoft.CodeAnalysis.VisualBasic.Syntax.StructuredTriviaSyntax)node;
				}
				node = node.Parent;
			}
			return null;
		}
	}
}
