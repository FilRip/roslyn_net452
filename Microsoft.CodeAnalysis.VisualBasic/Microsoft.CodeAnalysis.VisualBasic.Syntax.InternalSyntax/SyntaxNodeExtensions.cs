using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class SyntaxNodeExtensions
	{
		private class SkippedTriviaBuilder
		{
			private SyntaxListBuilder<GreenNode> _triviaListBuilder;

			private readonly SyntaxListBuilder<SyntaxToken> _skippedTokensBuilder;

			private readonly bool _preserveExistingDiagnostics;

			private bool _addDiagnosticsToFirstTokenOnly;

			private IEnumerable<DiagnosticInfo> _diagnosticsToAdd;

			private void AddTrivia(GreenNode trivia)
			{
				FinishInProgressTokens();
				_triviaListBuilder.AddRange(trivia);
			}

			private void FinishInProgressTokens()
			{
				if (_skippedTokensBuilder.Count <= 0)
				{
					return;
				}
				GreenNode greenNode = SyntaxFactory.SkippedTokensTrivia(_skippedTokensBuilder.ToList());
				if (_diagnosticsToAdd != null)
				{
					foreach (DiagnosticInfo item in _diagnosticsToAdd)
					{
						greenNode = greenNode.AddError(item);
					}
					_diagnosticsToAdd = null;
				}
				_triviaListBuilder.Add(greenNode);
				_skippedTokensBuilder.Clear();
			}

			public SkippedTriviaBuilder(bool preserveExistingDiagnostics, bool addDiagnosticsToFirstTokenOnly, IEnumerable<DiagnosticInfo> diagnosticsToAdd)
			{
				_triviaListBuilder = SyntaxListBuilder<GreenNode>.Create();
				_skippedTokensBuilder = SyntaxListBuilder<SyntaxToken>.Create();
				_addDiagnosticsToFirstTokenOnly = addDiagnosticsToFirstTokenOnly;
				_preserveExistingDiagnostics = preserveExistingDiagnostics;
				_diagnosticsToAdd = diagnosticsToAdd;
			}

			public void AddToken(SyntaxToken token, bool isFirst, bool isLast)
			{
				bool isMissing = token.IsMissing;
				if (token.HasLeadingTrivia && (isFirst || isMissing || TriviaListContainsStructuredTrivia(token.GetLeadingTrivia())))
				{
					FinishInProgressTokens();
					AddTrivia(token.GetLeadingTrivia());
					token = (SyntaxToken)token.WithLeadingTrivia(null);
				}
				if (!_preserveExistingDiagnostics)
				{
					token = SyntaxExtensions.WithoutDiagnostics(token);
				}
				GreenNode greenNode = null;
				if (token.HasTrailingTrivia && (isLast || isMissing || TriviaListContainsStructuredTrivia(token.GetTrailingTrivia())))
				{
					greenNode = token.GetTrailingTrivia();
					token = (SyntaxToken)token.WithTrailingTrivia(null);
				}
				if (isMissing)
				{
					if (token.ContainsDiagnostics)
					{
						if (_diagnosticsToAdd != null)
						{
							_diagnosticsToAdd = _diagnosticsToAdd.Concat<DiagnosticInfo>(token.GetDiagnostics());
						}
						else
						{
							_diagnosticsToAdd = token.GetDiagnostics();
						}
						_addDiagnosticsToFirstTokenOnly = true;
					}
				}
				else
				{
					_skippedTokensBuilder.Add(token);
				}
				if (greenNode != null)
				{
					FinishInProgressTokens();
					AddTrivia(greenNode);
				}
				if (isFirst && _addDiagnosticsToFirstTokenOnly)
				{
					FinishInProgressTokens();
				}
			}

			public Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> GetTriviaList()
			{
				FinishInProgressTokens();
				if (_diagnosticsToAdd != null && _diagnosticsToAdd.Any() && _triviaListBuilder.Count > 0)
				{
					_triviaListBuilder[_triviaListBuilder.Count - 1] = SyntaxExtensions.WithAdditionalDiagnostics(_triviaListBuilder[_triviaListBuilder.Count - 1], _diagnosticsToAdd.ToArray());
				}
				return _triviaListBuilder.ToList();
			}
		}

		private static bool IsMissingToken(SyntaxToken token)
		{
			if (token.Width == 0)
			{
				return token.Kind != SyntaxKind.EmptyToken;
			}
			return false;
		}

		private static TSyntax AddLeadingTrivia<TSyntax>(this TSyntax node, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia) where TSyntax : VisualBasicSyntaxNode
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (!trivia.Any())
			{
				return node;
			}
			TSyntax result;
			if (node is SyntaxToken token)
			{
				SyntaxToken syntaxToken;
				if (IsMissingToken(token))
				{
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> startOfTrivia = GetStartOfTrivia(trivia);
					Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> endOfTrivia = GetEndOfTrivia(trivia);
					syntaxToken = AddTrailingTrivia(SyntaxToken.AddLeadingTrivia(token, startOfTrivia), endOfTrivia);
				}
				else
				{
					syntaxToken = SyntaxToken.AddLeadingTrivia(token, trivia);
				}
				result = (TSyntax)(VisualBasicSyntaxNode)syntaxToken;
			}
			else
			{
				result = FirstTokenReplacer.Replace(node, (SyntaxToken t) => SyntaxToken.AddLeadingTrivia(t, trivia));
			}
			return result;
		}

		internal static TSyntax AddLeadingSyntax<TSyntax>(this TSyntax node, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> unexpected, ERRID errorId) where TSyntax : VisualBasicSyntaxNode
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(errorId);
			if (unexpected.Node != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia = CreateSkippedTrivia(unexpected.Node, preserveDiagnostics: true, addDiagnosticToFirstTokenOnly: true, diagnosticInfo);
				return AddLeadingTrivia(node, trivia);
			}
			return (TSyntax)node.AddError(diagnosticInfo);
		}

		internal static TSyntax AddLeadingSyntax<TSyntax>(this TSyntax node, SyntaxToken unexpected, ERRID errorId) where TSyntax : VisualBasicSyntaxNode
		{
			return AddLeadingSyntax(node, (GreenNode)unexpected, errorId);
		}

		internal static TSyntax AddLeadingSyntax<TSyntax>(this TSyntax node, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected) where TSyntax : VisualBasicSyntaxNode
		{
			return AddLeadingSyntax(node, unexpected.Node);
		}

		internal static TSyntax AddLeadingSyntax<TSyntax>(this TSyntax node, GreenNode unexpected, ERRID errorId) where TSyntax : VisualBasicSyntaxNode
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(errorId);
			if (unexpected != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia = CreateSkippedTrivia(unexpected, preserveDiagnostics: false, addDiagnosticToFirstTokenOnly: false, diagnosticInfo);
				return AddLeadingTrivia(node, trivia);
			}
			return (TSyntax)node.AddError(diagnosticInfo);
		}

		internal static TSyntax AddLeadingSyntax<TSyntax>(this TSyntax node, GreenNode unexpected) where TSyntax : VisualBasicSyntaxNode
		{
			if (unexpected != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia = CreateSkippedTrivia(unexpected, preserveDiagnostics: true, addDiagnosticToFirstTokenOnly: false, null);
				return AddLeadingTrivia(node, trivia);
			}
			return node;
		}

		internal static TSyntax AddTrailingTrivia<TSyntax>(this TSyntax node, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> trivia) where TSyntax : GreenNode
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node is SyntaxToken token)
			{
				return (TSyntax)(GreenNode)SyntaxToken.AddTrailingTrivia(token, trivia);
			}
			return LastTokenReplacer.Replace(node, (SyntaxToken t) => SyntaxToken.AddTrailingTrivia(t, trivia));
		}

		internal static TSyntax AddTrailingSyntax<TSyntax>(this TSyntax node, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected, ERRID errorId) where TSyntax : VisualBasicSyntaxNode
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(errorId);
			if (unexpected.Node != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = CreateSkippedTrivia(unexpected.Node, preserveDiagnostics: true, addDiagnosticToFirstTokenOnly: true, diagnosticInfo);
				return AddTrailingTrivia(node, syntaxList);
			}
			return (TSyntax)node.AddError(diagnosticInfo);
		}

		internal static TSyntax AddTrailingSyntax<TSyntax>(this TSyntax node, SyntaxToken unexpected, ERRID errorId) where TSyntax : VisualBasicSyntaxNode
		{
			return AddTrailingSyntax(node, (GreenNode)unexpected, errorId);
		}

		internal static TSyntax AddTrailingSyntax<TSyntax>(this TSyntax node, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<SyntaxToken> unexpected) where TSyntax : VisualBasicSyntaxNode
		{
			return AddTrailingSyntax(node, unexpected.Node);
		}

		internal static TSyntax AddTrailingSyntax<TSyntax>(this TSyntax node, SyntaxToken unexpected) where TSyntax : VisualBasicSyntaxNode
		{
			return AddTrailingSyntax(node, (GreenNode)unexpected);
		}

		internal static TSyntax AddTrailingSyntax<TSyntax>(this TSyntax node, GreenNode unexpected, ERRID errorId) where TSyntax : GreenNode
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(errorId);
			if (unexpected != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = CreateSkippedTrivia(unexpected, preserveDiagnostics: false, addDiagnosticToFirstTokenOnly: false, diagnosticInfo);
				return AddTrailingTrivia(node, syntaxList);
			}
			return (TSyntax)node.AddError(diagnosticInfo);
		}

		internal static TSyntax AddTrailingSyntax<TSyntax>(this TSyntax node, GreenNode unexpected) where TSyntax : GreenNode
		{
			if (unexpected != null)
			{
				Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> trivia = CreateSkippedTrivia(unexpected, preserveDiagnostics: true, addDiagnosticToFirstTokenOnly: false, null);
				return AddTrailingTrivia(node, trivia);
			}
			return node;
		}

		internal static TSyntax AddError<TSyntax>(this TSyntax node, ERRID errorId) where TSyntax : VisualBasicSyntaxNode
		{
			return (TSyntax)node.AddError(ErrorFactory.ErrorInfo(errorId));
		}

		internal static Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> GetStartOfTrivia(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia)
		{
			return GetStartOfTrivia(trivia, GetIndexOfEndOfTrivia(trivia));
		}

		internal static Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> GetStartOfTrivia(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia, int indexOfEnd)
		{
			if (indexOfEnd == 0)
			{
				return default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			}
			if (indexOfEnd == trivia.Count)
			{
				return trivia;
			}
			SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = SyntaxListBuilder<VisualBasicSyntaxNode>.Create();
			int num = indexOfEnd - 1;
			for (int i = 0; i <= num; i++)
			{
				syntaxListBuilder.Add(trivia[i]);
			}
			return syntaxListBuilder.ToList();
		}

		internal static Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> GetEndOfTrivia(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia)
		{
			return GetEndOfTrivia(trivia, GetIndexOfEndOfTrivia(trivia));
		}

		internal static Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> GetEndOfTrivia(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia, int indexOfEnd)
		{
			if (indexOfEnd == 0)
			{
				return trivia;
			}
			if (indexOfEnd == trivia.Count)
			{
				return default(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>);
			}
			SyntaxListBuilder<VisualBasicSyntaxNode> syntaxListBuilder = SyntaxListBuilder<VisualBasicSyntaxNode>.Create();
			int num = trivia.Count - 1;
			for (int i = indexOfEnd; i <= num; i++)
			{
				syntaxListBuilder.Add(trivia[i]);
			}
			return syntaxListBuilder.ToList();
		}

		internal static int GetLengthOfCommonEnd(Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia1, Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia2)
		{
			int count = trivia1.Count;
			int count2 = trivia2.Count;
			int indexAfterLastSkippedToken = GetIndexAfterLastSkippedToken(trivia1);
			int indexAfterLastSkippedToken2 = GetIndexAfterLastSkippedToken(trivia2);
			return Math.Min(count - indexAfterLastSkippedToken, count2 - indexAfterLastSkippedToken2);
		}

		private static int GetIndexAfterLastSkippedToken(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia)
		{
			for (int i = trivia.Count - 1; i >= 0; i += -1)
			{
				if (trivia[i]!.Kind == SyntaxKind.SkippedTokensTrivia)
				{
					return i + 1;
				}
			}
			return 0;
		}

		private static int GetIndexOfEndOfTrivia(this Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> trivia)
		{
			int count = trivia.Count;
			if (count > 0)
			{
				int num = count - 1;
				switch (trivia[num]!.Kind)
				{
				case SyntaxKind.ColonTrivia:
					return num;
				case SyntaxKind.EndOfLineTrivia:
					if (num > 0)
					{
						return trivia[num - 1]!.Kind switch
						{
							SyntaxKind.LineContinuationTrivia => count, 
							SyntaxKind.CommentTrivia => num - 1, 
							_ => num, 
						};
					}
					return num;
				case SyntaxKind.LineContinuationTrivia:
				case SyntaxKind.ConstDirectiveTrivia:
				case SyntaxKind.IfDirectiveTrivia:
				case SyntaxKind.ElseIfDirectiveTrivia:
				case SyntaxKind.ElseDirectiveTrivia:
				case SyntaxKind.EndIfDirectiveTrivia:
				case SyntaxKind.RegionDirectiveTrivia:
				case SyntaxKind.EndRegionDirectiveTrivia:
				case SyntaxKind.ExternalSourceDirectiveTrivia:
				case SyntaxKind.EndExternalSourceDirectiveTrivia:
				case SyntaxKind.ExternalChecksumDirectiveTrivia:
				case SyntaxKind.EnableWarningDirectiveTrivia:
				case SyntaxKind.DisableWarningDirectiveTrivia:
				case SyntaxKind.ReferenceDirectiveTrivia:
				case SyntaxKind.BadDirectiveTrivia:
					throw ExceptionUtilities.UnexpectedValue(trivia[num]!.Kind);
				}
			}
			return count;
		}

		private static bool TriviaListContainsStructuredTrivia(GreenNode triviaList)
		{
			if (triviaList == null)
			{
				return false;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(triviaList);
			int num = syntaxList.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				switch (syntaxList.ItemUntyped(i)!.RawKind)
				{
				case 378:
				case 709:
				case 736:
				case 737:
				case 738:
				case 739:
				case 740:
				case 741:
				case 744:
				case 745:
				case 746:
				case 747:
				case 748:
				case 749:
				case 750:
				case 753:
					return true;
				}
			}
			return false;
		}

		private static Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<GreenNode> CreateSkippedTrivia(GreenNode node, bool preserveDiagnostics, bool addDiagnosticToFirstTokenOnly, DiagnosticInfo addDiagnostic)
		{
			if (node.RawKind == 709)
			{
				if (addDiagnostic != null)
				{
					node = node.AddError(addDiagnostic);
				}
				return node;
			}
			IList<DiagnosticInfo> list = new List<DiagnosticInfo>();
			SyntaxListBuilder<SyntaxToken> tokenListBuilder = SyntaxListBuilder<SyntaxToken>.Create();
			CollectConstituentTokensAndDiagnostics(node, tokenListBuilder, list);
			if (!preserveDiagnostics)
			{
				list.Clear();
			}
			if (addDiagnostic != null)
			{
				list.Add(addDiagnostic);
			}
			SkippedTriviaBuilder skippedTriviaBuilder = new SkippedTriviaBuilder(preserveDiagnostics, addDiagnosticToFirstTokenOnly, list);
			int num = tokenListBuilder.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				SyntaxToken token = tokenListBuilder[i];
				skippedTriviaBuilder.AddToken(token, i == 0, i == tokenListBuilder.Count - 1);
			}
			return skippedTriviaBuilder.GetTriviaList();
		}

		internal static void CollectConstituentTokensAndDiagnostics(GreenNode @this, SyntaxListBuilder<SyntaxToken> tokenListBuilder, IList<DiagnosticInfo> nonTokenDiagnostics)
		{
			if (@this == null)
			{
				return;
			}
			if (@this.IsToken)
			{
				tokenListBuilder.Add((SyntaxToken)@this);
				return;
			}
			DiagnosticInfo[] diagnostics = @this.GetDiagnostics();
			if (diagnostics != null && diagnostics.Length > 0)
			{
				DiagnosticInfo[] array = diagnostics;
				foreach (DiagnosticInfo item in array)
				{
					nonTokenDiagnostics.Add(item);
				}
			}
			int num = @this.SlotCount - 1;
			for (int j = 0; j <= num; j++)
			{
				GreenNode slot = @this.GetSlot(j);
				if (slot != null)
				{
					CollectConstituentTokensAndDiagnostics(slot, tokenListBuilder, nonTokenDiagnostics);
				}
			}
		}

		internal static bool ContainsWhitespaceTrivia(this GreenNode @this)
		{
			if (@this == null)
			{
				return false;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(@this);
			int num = syntaxList.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				int rawKind = syntaxList.ItemUntyped(i)!.RawKind;
				if (rawKind == 729 || rawKind == 730)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ContainsCommentTrivia(this GreenNode @this)
		{
			if (@this == null)
			{
				return false;
			}
			Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode> syntaxList = new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(@this);
			int num = syntaxList.Count - 1;
			for (int i = 0; i <= num; i++)
			{
				if (syntaxList.ItemUntyped(i)!.RawKind == 732)
				{
					return true;
				}
			}
			return false;
		}

		internal static SyntaxToken ExtractAnonymousTypeMemberName(this ExpressionSyntax input, ref bool isNameDictionaryAccess, ref bool isRejectedXmlName)
		{
			ArrayBuilder<ConditionalAccessExpressionSyntax> conditionalAccessStack = null;
			SyntaxToken result = ExtractAnonymousTypeMemberName(ref conditionalAccessStack, input, ref isNameDictionaryAccess, ref isRejectedXmlName);
			conditionalAccessStack?.Free();
			return result;
		}

		private static SyntaxToken ExtractAnonymousTypeMemberName(this ref ArrayBuilder<ConditionalAccessExpressionSyntax> conditionalAccessStack, ExpressionSyntax input, ref bool isNameDictionaryAccess, ref bool isRejectedXmlName)
		{
			while (true)
			{
				switch (input.Kind)
				{
				case SyntaxKind.IdentifierName:
					return ((IdentifierNameSyntax)input).Identifier;
				case SyntaxKind.XmlName:
				{
					XmlNameSyntax xmlNameSyntax = (XmlNameSyntax)input;
					if (!Scanner.IsIdentifier(xmlNameSyntax.LocalName.ToString()))
					{
						isRejectedXmlName = true;
						return null;
					}
					return xmlNameSyntax.LocalName;
				}
				case SyntaxKind.XmlBracketedName:
					input = ((XmlBracketedNameSyntax)input).Name;
					continue;
				case SyntaxKind.SimpleMemberAccessExpression:
				case SyntaxKind.DictionaryAccessExpression:
				{
					MemberAccessExpressionSyntax memberAccessExpressionSyntax = (MemberAccessExpressionSyntax)input;
					ExpressionSyntax expressionSyntax = memberAccessExpressionSyntax.Expression ?? PopAndGetConditionalAccessReceiver(conditionalAccessStack);
					if (input.Kind == SyntaxKind.SimpleMemberAccessExpression && expressionSyntax != null)
					{
						SyntaxKind kind = expressionSyntax.Kind;
						if (kind - 293 <= SyntaxKind.List)
						{
							input = expressionSyntax;
							continue;
						}
					}
					ClearConditionalAccessStack(conditionalAccessStack);
					isNameDictionaryAccess = input.Kind == SyntaxKind.DictionaryAccessExpression;
					input = memberAccessExpressionSyntax.Name;
					continue;
				}
				case SyntaxKind.XmlElementAccessExpression:
				case SyntaxKind.XmlDescendantAccessExpression:
				case SyntaxKind.XmlAttributeAccessExpression:
				{
					XmlMemberAccessExpressionSyntax obj = (XmlMemberAccessExpressionSyntax)input;
					ClearConditionalAccessStack(conditionalAccessStack);
					input = obj.Name;
					continue;
				}
				case SyntaxKind.InvocationExpression:
				{
					InvocationExpressionSyntax invocationExpressionSyntax = (InvocationExpressionSyntax)input;
					ExpressionSyntax expressionSyntax2 = invocationExpressionSyntax.Expression ?? PopAndGetConditionalAccessReceiver(conditionalAccessStack);
					if (expressionSyntax2 == null)
					{
						break;
					}
					if (invocationExpressionSyntax.ArgumentList == null || invocationExpressionSyntax.ArgumentList.Arguments.Count == 0)
					{
						input = expressionSyntax2;
						continue;
					}
					if (invocationExpressionSyntax.ArgumentList.Arguments.Count == 1)
					{
						SyntaxKind kind2 = expressionSyntax2.Kind;
						if (kind2 - 293 <= SyntaxKind.List)
						{
							input = expressionSyntax2;
							continue;
						}
					}
					break;
				}
				case SyntaxKind.ConditionalAccessExpression:
				{
					ConditionalAccessExpressionSyntax conditionalAccessExpressionSyntax = (ConditionalAccessExpressionSyntax)input;
					if (conditionalAccessStack == null)
					{
						conditionalAccessStack = ArrayBuilder<ConditionalAccessExpressionSyntax>.GetInstance();
					}
					conditionalAccessStack.Push(conditionalAccessExpressionSyntax);
					input = conditionalAccessExpressionSyntax.WhenNotNull;
					continue;
				}
				}
				break;
			}
			return null;
		}

		private static void ClearConditionalAccessStack(ArrayBuilder<ConditionalAccessExpressionSyntax> conditionalAccessStack)
		{
			conditionalAccessStack?.Clear();
		}

		private static ExpressionSyntax PopAndGetConditionalAccessReceiver(ArrayBuilder<ConditionalAccessExpressionSyntax> conditionalAccessStack)
		{
			if (conditionalAccessStack == null || conditionalAccessStack.Count == 0)
			{
				return null;
			}
			return conditionalAccessStack.Pop().Expression;
		}

		internal static bool IsExecutableStatementOrItsPart(VisualBasicSyntaxNode node)
		{
			if (node is ExecutableStatementSyntax)
			{
				return true;
			}
			switch (node.Kind)
			{
			case SyntaxKind.IfStatement:
			case SyntaxKind.ElseIfStatement:
			case SyntaxKind.ElseStatement:
			case SyntaxKind.TryStatement:
			case SyntaxKind.CatchStatement:
			case SyntaxKind.FinallyStatement:
			case SyntaxKind.SelectStatement:
			case SyntaxKind.CaseStatement:
			case SyntaxKind.CaseElseStatement:
			case SyntaxKind.SyncLockStatement:
			case SyntaxKind.WhileStatement:
			case SyntaxKind.ForStatement:
			case SyntaxKind.ForEachStatement:
			case SyntaxKind.UsingStatement:
			case SyntaxKind.WithStatement:
			case SyntaxKind.SimpleDoStatement:
			case SyntaxKind.DoWhileStatement:
			case SyntaxKind.DoUntilStatement:
				return true;
			default:
				return false;
			}
		}
	}
}
