using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax
{
	public class SyntaxNodeRemover
	{
		private class SyntaxRemover : VisualBasicSyntaxRewriter
		{
			private readonly HashSet<SyntaxNode> _nodesToRemove;

			private readonly SyntaxRemoveOptions _options;

			private readonly TextSpan _searchSpan;

			private readonly SyntaxTriviaListBuilder _residualTrivia;

			private HashSet<SyntaxNode> _directivesToKeep;

			internal SyntaxTriviaList ResidualTrivia
			{
				get
				{
					if (_residualTrivia != null)
					{
						return _residualTrivia.ToList();
					}
					return default(SyntaxTriviaList);
				}
			}

			public SyntaxRemover(SyntaxNode[] nodes, SyntaxRemoveOptions options)
				: base(nodes.Any((SyntaxNode n) => n.IsPartOfStructuredTrivia()))
			{
				_nodesToRemove = new HashSet<SyntaxNode>(nodes);
				_options = options;
				_searchSpan = ComputeTotalSpan(nodes);
				_residualTrivia = SyntaxTriviaListBuilder.Create();
			}

			private static TextSpan ComputeTotalSpan(SyntaxNode[] nodes)
			{
				TextSpan fullSpan = nodes[0].FullSpan;
				int num = fullSpan.Start;
				int num2 = fullSpan.End;
				for (int i = 1; i < nodes.Length; i++)
				{
					TextSpan fullSpan2 = nodes[i].FullSpan;
					num = Math.Min(num, fullSpan2.Start);
					num2 = Math.Max(num2, fullSpan2.End);
				}
				return new TextSpan(num, num2 - num);
			}

			private void AddResidualTrivia(SyntaxTriviaList trivia, bool requiresNewLine = false)
			{
				if (requiresNewLine)
				{
					AddEndOfLine();
				}
				_residualTrivia.Add(in trivia);
			}

			private void AddEndOfLine()
			{
				if (_residualTrivia.Count == 0 || !IsEndOfLine(_residualTrivia[_residualTrivia.Count - 1]))
				{
					_residualTrivia.Add(SyntaxFactory.CarriageReturnLineFeed);
				}
			}

			private static bool IsEndOfLine(SyntaxTrivia trivia)
			{
				if (VisualBasicExtensions.Kind(trivia) != SyntaxKind.EndOfLineTrivia && VisualBasicExtensions.Kind(trivia) != SyntaxKind.CommentTrivia)
				{
					return trivia.IsDirective;
				}
				return true;
			}

			private static bool HasEndOfLine(SyntaxTriviaList trivia)
			{
				return trivia.Any((SyntaxTrivia t) => IsEndOfLine(t));
			}

			private bool IsForRemoval(SyntaxNode node)
			{
				return _nodesToRemove.Contains(node);
			}

			private bool ShouldVisit(SyntaxNode node)
			{
				if (!node.FullSpan.IntersectsWith(_searchSpan))
				{
					if (_residualTrivia != null)
					{
						return _residualTrivia.Count > 0;
					}
					return false;
				}
				return true;
			}

			public override SyntaxNode Visit(SyntaxNode node)
			{
				SyntaxNode result = node;
				if (node != null)
				{
					if (IsForRemoval(node))
					{
						AddTrivia(node);
						result = null;
					}
					else if (ShouldVisit(node))
					{
						result = base.Visit(node);
					}
				}
				return result;
			}

			public override SyntaxToken VisitToken(SyntaxToken token)
			{
				SyntaxToken syntaxToken = token;
				if (VisitIntoStructuredTrivia)
				{
					syntaxToken = base.VisitToken(token);
				}
				if (VisualBasicExtensions.Kind(syntaxToken) != 0 && _residualTrivia != null && _residualTrivia.Count > 0)
				{
					SyntaxTriviaListBuilder residualTrivia = _residualTrivia;
					SyntaxTriviaList list = syntaxToken.LeadingTrivia;
					residualTrivia.Add(in list);
					syntaxToken = syntaxToken.WithLeadingTrivia(_residualTrivia.ToList());
					_residualTrivia.Clear();
				}
				return syntaxToken;
			}

			public override SeparatedSyntaxList<TNode> VisitList<TNode>(SeparatedSyntaxList<TNode> list)
			{
				SyntaxNodeOrTokenList withSeparators = list.GetWithSeparators();
				bool flag = false;
				SyntaxNodeOrTokenListBuilder syntaxNodeOrTokenListBuilder = null;
				int count = withSeparators.Count;
				int num = count - 1;
				for (int i = 0; i <= num; i++)
				{
					SyntaxNodeOrToken syntaxNodeOrToken = withSeparators[i];
					SyntaxNodeOrToken syntaxNodeOrToken2 = default(SyntaxNodeOrToken);
					if (syntaxNodeOrToken.IsToken)
					{
						if (flag)
						{
							flag = false;
							syntaxNodeOrToken2 = default(SyntaxNodeOrToken);
						}
						else
						{
							syntaxNodeOrToken2 = VisitListSeparator(syntaxNodeOrToken.AsToken());
						}
					}
					else
					{
						TNode val = (TNode)syntaxNodeOrToken.AsNode();
						if (IsForRemoval(val))
						{
							if (syntaxNodeOrTokenListBuilder == null)
							{
								syntaxNodeOrTokenListBuilder = new SyntaxNodeOrTokenListBuilder(count);
								syntaxNodeOrTokenListBuilder.Add(withSeparators, 0, i);
							}
							CommonSyntaxNodeRemover.GetSeparatorInfo(withSeparators, i, 730, out var nextTokenIsSeparator, out var nextSeparatorBelongsToNode);
							if (!nextSeparatorBelongsToNode && syntaxNodeOrTokenListBuilder.Count > 0 && syntaxNodeOrTokenListBuilder[syntaxNodeOrTokenListBuilder.Count - 1].IsToken)
							{
								SyntaxToken token = syntaxNodeOrTokenListBuilder[syntaxNodeOrTokenListBuilder.Count - 1].AsToken();
								AddTrivia(token, val);
								syntaxNodeOrTokenListBuilder.RemoveLast();
							}
							else if (nextTokenIsSeparator)
							{
								SyntaxToken token2 = withSeparators[i + 1].AsToken();
								AddTrivia(val, token2);
								flag = true;
							}
							else
							{
								AddTrivia(val);
							}
							syntaxNodeOrToken2 = default(SyntaxNodeOrToken);
						}
						else
						{
							syntaxNodeOrToken2 = VisitListElement(val);
						}
					}
					if (syntaxNodeOrToken != syntaxNodeOrToken2 && syntaxNodeOrTokenListBuilder == null)
					{
						syntaxNodeOrTokenListBuilder = new SyntaxNodeOrTokenListBuilder(count);
						syntaxNodeOrTokenListBuilder.Add(withSeparators, 0, i);
					}
					if (syntaxNodeOrTokenListBuilder != null && !Microsoft.CodeAnalysis.VisualBasicExtensions.IsKind(syntaxNodeOrToken2, SyntaxKind.None))
					{
						syntaxNodeOrTokenListBuilder.Add(in syntaxNodeOrToken2);
					}
				}
				if (syntaxNodeOrTokenListBuilder != null)
				{
					return VisualBasicExtensions.AsSeparatedList<TNode>(syntaxNodeOrTokenListBuilder.ToList());
				}
				return list;
			}

			private void AddTrivia(SyntaxNode node)
			{
				if ((_options & SyntaxRemoveOptions.KeepLeadingTrivia) != 0)
				{
					AddResidualTrivia(node.GetLeadingTrivia());
				}
				else if ((_options & SyntaxRemoveOptions.KeepEndOfLine) != 0 && HasEndOfLine(node.GetLeadingTrivia()))
				{
					AddEndOfLine();
				}
				if ((_options & (SyntaxRemoveOptions.KeepUnbalancedDirectives | SyntaxRemoveOptions.KeepDirectives)) != 0)
				{
					AddDirectives(node, GetRemovedSpan(node.Span, node.FullSpan));
				}
				if ((_options & SyntaxRemoveOptions.KeepTrailingTrivia) != 0)
				{
					AddResidualTrivia(node.GetTrailingTrivia());
				}
				else if ((_options & SyntaxRemoveOptions.KeepEndOfLine) != 0 && HasEndOfLine(node.GetTrailingTrivia()))
				{
					AddEndOfLine();
				}
				if ((_options & SyntaxRemoveOptions.AddElasticMarker) != 0)
				{
					AddResidualTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));
				}
			}

			private void AddTrivia(SyntaxToken token, SyntaxNode node)
			{
				if ((_options & SyntaxRemoveOptions.KeepLeadingTrivia) != 0)
				{
					AddResidualTrivia(token.LeadingTrivia);
					AddResidualTrivia(token.TrailingTrivia);
					AddResidualTrivia(node.GetLeadingTrivia());
				}
				else if ((_options & SyntaxRemoveOptions.KeepEndOfLine) != 0 && (HasEndOfLine(token.LeadingTrivia) || HasEndOfLine(token.TrailingTrivia) || HasEndOfLine(node.GetLeadingTrivia())))
				{
					AddEndOfLine();
				}
				if ((_options & (SyntaxRemoveOptions.KeepUnbalancedDirectives | SyntaxRemoveOptions.KeepDirectives)) != 0)
				{
					TextSpan fullSpan = TextSpan.FromBounds(token.FullSpan.Start, node.FullSpan.End);
					TextSpan span = TextSpan.FromBounds(token.Span.Start, node.Span.End);
					AddDirectives(node.Parent, GetRemovedSpan(span, fullSpan));
				}
				if ((_options & SyntaxRemoveOptions.KeepTrailingTrivia) != 0)
				{
					AddResidualTrivia(node.GetTrailingTrivia());
				}
				else if ((_options & SyntaxRemoveOptions.KeepEndOfLine) != 0 && HasEndOfLine(node.GetTrailingTrivia()))
				{
					AddEndOfLine();
				}
				if ((_options & SyntaxRemoveOptions.AddElasticMarker) != 0)
				{
					AddResidualTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));
				}
			}

			private void AddTrivia(SyntaxNode node, SyntaxToken token)
			{
				if ((_options & SyntaxRemoveOptions.KeepLeadingTrivia) != 0)
				{
					AddResidualTrivia(node.GetLeadingTrivia());
				}
				else if ((_options & SyntaxRemoveOptions.KeepEndOfLine) != 0 && HasEndOfLine(node.GetLeadingTrivia()))
				{
					AddEndOfLine();
				}
				if ((_options & (SyntaxRemoveOptions.KeepUnbalancedDirectives | SyntaxRemoveOptions.KeepDirectives)) != 0)
				{
					TextSpan fullSpan = TextSpan.FromBounds(node.FullSpan.Start, token.FullSpan.End);
					TextSpan span = TextSpan.FromBounds(node.Span.Start, token.Span.End);
					AddDirectives(node.Parent, GetRemovedSpan(span, fullSpan));
				}
				if ((_options & SyntaxRemoveOptions.KeepTrailingTrivia) != 0)
				{
					AddResidualTrivia(node.GetTrailingTrivia());
					AddResidualTrivia(token.LeadingTrivia);
					AddResidualTrivia(token.TrailingTrivia);
				}
				else if ((_options & SyntaxRemoveOptions.KeepEndOfLine) != 0 && (HasEndOfLine(node.GetTrailingTrivia()) || HasEndOfLine(token.LeadingTrivia) || HasEndOfLine(token.TrailingTrivia)))
				{
					AddEndOfLine();
				}
				if ((_options & SyntaxRemoveOptions.AddElasticMarker) != 0)
				{
					AddResidualTrivia(SyntaxFactory.TriviaList(SyntaxFactory.ElasticMarker));
				}
			}

			private TextSpan GetRemovedSpan(TextSpan span, TextSpan fullSpan)
			{
				TextSpan result = fullSpan;
				if ((_options & SyntaxRemoveOptions.KeepLeadingTrivia) != 0)
				{
					result = TextSpan.FromBounds(span.Start, result.End);
				}
				if ((_options & SyntaxRemoveOptions.KeepTrailingTrivia) != 0)
				{
					result = TextSpan.FromBounds(result.Start, span.End);
				}
				return result;
			}

			private void AddDirectives(SyntaxNode node, TextSpan span)
			{
				_Closure_0024__22_002D0 arg = default(_Closure_0024__22_002D0);
				_Closure_0024__22_002D0 CS_0024_003C_003E8__locals0 = new _Closure_0024__22_002D0(arg);
				CS_0024_003C_003E8__locals0._0024VB_0024Local_span = span;
				if (!node.ContainsDirectives)
				{
					return;
				}
				if (_directivesToKeep == null)
				{
					_directivesToKeep = new HashSet<SyntaxNode>();
				}
				else
				{
					_directivesToKeep.Clear();
				}
				IEnumerable<DirectiveTriviaSyntax> enumerable = from tr in node.DescendantTrivia(CS_0024_003C_003E8__locals0._0024VB_0024Local_span, (SyntaxNode n) => n.ContainsDirectives, descendIntoTrivia: true)
					where tr.IsDirective
					select (DirectiveTriviaSyntax)tr.GetStructure();
				foreach (DirectiveTriviaSyntax item in enumerable)
				{
					if ((_options & SyntaxRemoveOptions.KeepDirectives) != 0)
					{
						_directivesToKeep.Add(item);
					}
					else if (HasRelatedDirectives(item))
					{
						List<DirectiveTriviaSyntax> relatedDirectives = item.GetRelatedDirectives();
						if (!relatedDirectives.All((CS_0024_003C_003E8__locals0._0024I3 == null) ? (CS_0024_003C_003E8__locals0._0024I3 = (DirectiveTriviaSyntax rd) => rd.FullSpan.OverlapsWith(CS_0024_003C_003E8__locals0._0024VB_0024Local_span)) : CS_0024_003C_003E8__locals0._0024I3))
						{
							foreach (DirectiveTriviaSyntax item2 in relatedDirectives.Where((CS_0024_003C_003E8__locals0._0024I4 == null) ? (CS_0024_003C_003E8__locals0._0024I4 = (DirectiveTriviaSyntax rd) => rd.FullSpan.OverlapsWith(CS_0024_003C_003E8__locals0._0024VB_0024Local_span)) : CS_0024_003C_003E8__locals0._0024I4))
							{
								_directivesToKeep.Add(item2);
							}
						}
					}
					if (_directivesToKeep.Contains(item))
					{
						AddResidualTrivia(SyntaxFactory.TriviaList(item.ParentTrivia), requiresNewLine: true);
					}
				}
			}

			private static bool HasRelatedDirectives(DirectiveTriviaSyntax directive)
			{
				SyntaxKind syntaxKind = directive.Kind();
				if (syntaxKind - 737 <= (SyntaxKind)4 || syntaxKind == SyntaxKind.EndRegionDirectiveTrivia)
				{
					return true;
				}
				return false;
			}
		}

		internal static TRoot RemoveNodes<TRoot>(TRoot root, IEnumerable<SyntaxNode> nodes, SyntaxRemoveOptions options) where TRoot : SyntaxNode
		{
			if (nodes.ToArray().Length == 0)
			{
				return root;
			}
			SyntaxRemover syntaxRemover = new SyntaxRemover(nodes.ToArray(), options);
			SyntaxNode syntaxNode = syntaxRemover.Visit(root);
			SyntaxTriviaList residualTrivia = syntaxRemover.ResidualTrivia;
			if (residualTrivia.Count > 0)
			{
				syntaxNode = syntaxNode.WithTrailingTrivia(syntaxNode.GetTrailingTrivia().Concat(residualTrivia));
			}
			return (TRoot)syntaxNode;
		}
	}
}
