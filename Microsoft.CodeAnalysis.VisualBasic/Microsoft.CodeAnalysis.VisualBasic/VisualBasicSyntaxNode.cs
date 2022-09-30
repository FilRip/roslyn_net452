using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public abstract class VisualBasicSyntaxNode : SyntaxNode
	{
		internal static ReadOnlyCollection<Diagnostic> EmptyErrorCollection = new ReadOnlyCollection<Diagnostic>(Array.Empty<Diagnostic>());

		internal Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode VbGreen => (Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)base.Green;

		internal new SyntaxTree SyntaxTree
		{
			get
			{
				if (_syntaxTree == null)
				{
					ArrayBuilder<SyntaxNode> instance = ArrayBuilder<SyntaxNode>.GetInstance();
					SyntaxTree syntaxTree = null;
					SyntaxNode syntaxNode = this;
					SyntaxNode syntaxNode2 = null;
					while (syntaxNode != null)
					{
						syntaxTree = syntaxNode._syntaxTree;
						if (syntaxTree != null)
						{
							break;
						}
						syntaxNode2 = syntaxNode;
						instance.Push(syntaxNode);
						syntaxNode = syntaxNode2.Parent;
					}
					if (syntaxTree == null)
					{
						syntaxTree = VisualBasicSyntaxTree.CreateWithoutClone((VisualBasicSyntaxNode)syntaxNode2);
					}
					while (instance.Count > 0)
					{
						SyntaxTree syntaxTree2 = Interlocked.CompareExchange(ref instance.Pop()._syntaxTree, syntaxTree, null);
						if (syntaxTree2 != null)
						{
							syntaxTree = syntaxTree2;
						}
					}
					instance.Free();
				}
				return _syntaxTree;
			}
		}

		public override string Language => "Visual Basic";

		internal new VisualBasicSyntaxNode Parent => (VisualBasicSyntaxNode)base.Parent;

		public bool IsDirective => base.Green.IsDirective;

		public new int SpanStart => base.Position + base.Green.GetLeadingTriviaWidth();

		protected override SyntaxTree SyntaxTreeCore => SyntaxTree;

		internal VisualBasicSyntaxNode(GreenNode green, SyntaxNode parent, int position)
			: base(green, parent, position)
		{
		}

		internal VisualBasicSyntaxNode(GreenNode green, int position, SyntaxTree syntaxTree)
			: base(green, null, position)
		{
			_syntaxTree = syntaxTree;
		}

		public abstract TResult Accept<TResult>(VisualBasicSyntaxVisitor<TResult> visitor);

		public abstract void Accept(VisualBasicSyntaxVisitor visitor);

		public SyntaxKind Kind()
		{
			return (SyntaxKind)base.Green.RawKind;
		}

		public static SyntaxNode DeserializeFrom(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new InvalidOperationException(CodeAnalysisResources.TheStreamCannotBeReadFrom);
			}
			using ObjectReader objectReader = ObjectReader.TryGetReader(stream, leaveOpen: true, cancellationToken);
			if (objectReader == null)
			{
				throw new ArgumentException(CodeAnalysisResources.Stream_contains_invalid_data, "stream");
			}
			return ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)objectReader.ReadValue()).CreateRed(null, 0);
		}

		public new SyntaxTriviaList GetLeadingTrivia()
		{
			return GetFirstToken(includeZeroWidth: true).LeadingTrivia;
		}

		public new SyntaxTriviaList GetTrailingTrivia()
		{
			return GetLastToken(includeZeroWidth: true).TrailingTrivia;
		}

		internal ReadOnlyCollection<Diagnostic> GetSyntaxErrors(SyntaxTree tree)
		{
			return DoGetSyntaxErrors(tree, this);
		}

		internal static ReadOnlyCollection<Diagnostic> DoGetSyntaxErrors(SyntaxTree tree, SyntaxNodeOrToken nodeOrToken)
		{
			if (!nodeOrToken.ContainsDiagnostics)
			{
				return EmptyErrorCollection;
			}
			Stack<SyntaxNodeOrToken> stack = new Stack<SyntaxNodeOrToken>();
			List<Diagnostic> list = new List<Diagnostic>();
			stack.Push(nodeOrToken);
			while (stack.Count > 0)
			{
				nodeOrToken = stack.Pop();
				GreenNode underlyingNode = nodeOrToken.UnderlyingNode;
				if (underlyingNode.ContainsDiagnostics)
				{
					DiagnosticInfo[] diagnostics = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)underlyingNode).GetDiagnostics();
					if (diagnostics != null)
					{
						int num = diagnostics.Length - 1;
						for (int i = 0; i <= num; i++)
						{
							DiagnosticInfo errorInfo = diagnostics[i];
							list.Add(CreateSyntaxError(tree, nodeOrToken, errorInfo));
						}
					}
				}
				if (!nodeOrToken.IsToken)
				{
					PushNodesWithErrors(stack, nodeOrToken.ChildNodesAndTokens());
				}
				else if (nodeOrToken.IsToken)
				{
					ProcessTrivia(tree, list, stack, nodeOrToken.GetLeadingTrivia());
					ProcessTrivia(tree, list, stack, nodeOrToken.GetTrailingTrivia());
				}
			}
			return new ReadOnlyCollection<Diagnostic>(list);
		}

		private static void PushNodesWithErrors(Stack<SyntaxNodeOrToken> stack, ChildSyntaxList nodes)
		{
			ChildSyntaxList.Enumerator enumerator = nodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxNodeOrToken current = enumerator.Current;
				if (current.ContainsDiagnostics)
				{
					stack.Push(current);
				}
			}
		}

		private static void ProcessTrivia(SyntaxTree tree, List<Diagnostic> errorList, Stack<SyntaxNodeOrToken> stack, SyntaxTriviaList nodes)
		{
			SyntaxTriviaList.Enumerator enumerator = nodes.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxTrivia current = enumerator.Current;
				if (!current.UnderlyingNode!.ContainsDiagnostics)
				{
					continue;
				}
				if (current.HasStructure)
				{
					stack.Push((VisualBasicSyntaxNode)current.GetStructure());
					continue;
				}
				DiagnosticInfo[] diagnostics = ((Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode)current.UnderlyingNode).GetDiagnostics();
				if (diagnostics != null)
				{
					int num = diagnostics.Length - 1;
					for (int i = 0; i <= num; i++)
					{
						DiagnosticInfo errorInfo = diagnostics[i];
						errorList.Add(CreateSyntaxError(tree, current, errorInfo));
					}
				}
			}
		}

		private static Diagnostic CreateSyntaxError(SyntaxTree tree, SyntaxNodeOrToken nodeOrToken, DiagnosticInfo errorInfo)
		{
			return new VBDiagnostic(errorInfo, (tree == null) ? new SourceLocation(tree, nodeOrToken.Span) : tree.GetLocation(nodeOrToken.Span));
		}

		private static Diagnostic CreateSyntaxError(SyntaxTree tree, SyntaxTrivia nodeOrToken, DiagnosticInfo errorInfo)
		{
			return new VBDiagnostic(errorInfo, (tree == null) ? new SourceLocation(tree, nodeOrToken.Span) : tree.GetLocation(nodeOrToken.Span));
		}

		internal VisualBasicSyntaxNode AddError(DiagnosticInfo err)
		{
			DiagnosticInfo[] array;
			if (base.Green.GetDiagnostics() == null)
			{
				array = new DiagnosticInfo[1] { err };
			}
			else
			{
				array = base.Green.GetDiagnostics();
				int num = array.Length;
				array = (DiagnosticInfo[])Utils.CopyArray(array, new DiagnosticInfo[num + 1]);
				array[num] = err;
			}
			return (VisualBasicSyntaxNode)base.Green.SetDiagnostics(array).CreateRed(null, 0);
		}

		public new SyntaxToken GetFirstToken(bool includeZeroWidth = false, bool includeSkipped = false, bool includeDirectives = false, bool includeDocumentationComments = false)
		{
			return base.GetFirstToken(includeZeroWidth, includeSkipped, includeDirectives, includeDocumentationComments);
		}

		public new SyntaxToken GetLastToken(bool includeZeroWidth = false, bool includeSkipped = false, bool includeDirectives = false, bool includeDocumentationComments = false)
		{
			return base.GetLastToken(includeZeroWidth, includeSkipped, includeDirectives, includeDocumentationComments);
		}

		public IList<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax> GetDirectives(Func<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, bool> filter = null)
		{
			return ((SyntaxNodeOrToken)this).GetDirectives(filter);
		}

		public Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax GetFirstDirective(Func<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, bool> predicate = null)
		{
			ChildSyntaxList.Enumerator enumerator = ChildNodesAndTokens().GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxNodeOrToken current = enumerator.Current;
				if (!current.ContainsDirectives)
				{
					continue;
				}
				if (current.IsNode)
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax firstDirective = ((VisualBasicSyntaxNode)current.AsNode()).GetFirstDirective(predicate);
					if (firstDirective != null)
					{
						return firstDirective;
					}
					continue;
				}
				SyntaxTriviaList.Enumerator enumerator2 = current.AsToken().LeadingTrivia.GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SyntaxTrivia current2 = enumerator2.Current;
					if (current2.IsDirective)
					{
						Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax directiveTriviaSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)current2.GetStructure();
						if (predicate == null || predicate(directiveTriviaSyntax))
						{
							return directiveTriviaSyntax;
						}
					}
				}
			}
			return null;
		}

		public Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax GetLastDirective(Func<Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax, bool> predicate = null)
		{
			ChildSyntaxList.Reversed.Enumerator enumerator = ChildNodesAndTokens().Reverse().GetEnumerator();
			while (enumerator.MoveNext())
			{
				SyntaxNodeOrToken current = enumerator.Current;
				if (!current.ContainsDirectives)
				{
					continue;
				}
				if (current.IsNode)
				{
					Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax lastDirective = ((VisualBasicSyntaxNode)current.AsNode()).GetLastDirective(predicate);
					if (lastDirective != null)
					{
						return lastDirective;
					}
					continue;
				}
				SyntaxTriviaList.Reversed.Enumerator enumerator2 = current.AsToken().LeadingTrivia.Reverse().GetEnumerator();
				while (enumerator2.MoveNext())
				{
					SyntaxTrivia current2 = enumerator2.Current;
					if (current2.IsDirective)
					{
						Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax directiveTriviaSyntax = (Microsoft.CodeAnalysis.VisualBasic.Syntax.DirectiveTriviaSyntax)current2.GetStructure();
						if (predicate == null || predicate(directiveTriviaSyntax))
						{
							return directiveTriviaSyntax;
						}
					}
				}
			}
			return null;
		}

		protected override SyntaxNode ReplaceCore<TNode>(IEnumerable<TNode> nodes = null, Func<TNode, TNode, SyntaxNode> computeReplacementNode = null, IEnumerable<SyntaxToken> tokens = null, Func<SyntaxToken, SyntaxToken, SyntaxToken> computeReplacementToken = null, IEnumerable<SyntaxTrivia> trivia = null, Func<SyntaxTrivia, SyntaxTrivia, SyntaxTrivia> computeReplacementTrivia = null)
		{
			return SyntaxReplacer.Replace(this, nodes, computeReplacementNode, tokens, computeReplacementToken, trivia, computeReplacementTrivia).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode RemoveNodesCore(IEnumerable<SyntaxNode> nodes, SyntaxRemoveOptions options)
		{
			return SyntaxNodeRemover.RemoveNodes(this, nodes, options).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode ReplaceNodeInListCore(SyntaxNode originalNode, IEnumerable<SyntaxNode> replacementNodes)
		{
			return SyntaxReplacer.ReplaceNodeInList(this, originalNode, replacementNodes).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode InsertNodesInListCore(SyntaxNode nodeInList, IEnumerable<SyntaxNode> nodesToInsert, bool insertBefore)
		{
			return SyntaxReplacer.InsertNodeInList(this, nodeInList, nodesToInsert, insertBefore).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode ReplaceTokenInListCore(SyntaxToken originalToken, IEnumerable<SyntaxToken> newTokens)
		{
			return SyntaxReplacer.ReplaceTokenInList(this, originalToken, newTokens).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode InsertTokensInListCore(SyntaxToken originalToken, IEnumerable<SyntaxToken> newTokens, bool insertBefore)
		{
			return SyntaxReplacer.InsertTokenInList(this, originalToken, newTokens, insertBefore).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode ReplaceTriviaInListCore(SyntaxTrivia originalTrivia, IEnumerable<SyntaxTrivia> newTrivia)
		{
			return SyntaxReplacer.ReplaceTriviaInList(this, originalTrivia, newTrivia).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode InsertTriviaInListCore(SyntaxTrivia originalTrivia, IEnumerable<SyntaxTrivia> newTrivia, bool insertBefore)
		{
			return SyntaxReplacer.InsertTriviaInList(this, originalTrivia, newTrivia, insertBefore).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		protected override SyntaxNode NormalizeWhitespaceCore(string indentation, string eol, bool elasticTrivia)
		{
			return SyntaxNormalizer.Normalize(this, indentation, eol, elasticTrivia, useDefaultCasing: false).AsRootOfNewTreeWithOptionsFrom(SyntaxTree);
		}

		public new Location GetLocation()
		{
			if (SyntaxTree != null)
			{
				SyntaxTree syntaxTree = SyntaxTree;
				if (EmbeddedSymbolExtensions.IsEmbeddedSyntaxTree(syntaxTree))
				{
					return new EmbeddedTreeLocation(EmbeddedSymbolExtensions.GetEmbeddedKind(syntaxTree), base.Span);
				}
				if (VisualBasicExtensions.IsMyTemplate(syntaxTree))
				{
					return new MyTemplateLocation(syntaxTree, base.Span);
				}
			}
			return new SourceLocation(this);
		}

		internal new SyntaxReference GetReference()
		{
			return SyntaxTree.GetReference(this);
		}

		public new IEnumerable<Diagnostic> GetDiagnostics()
		{
			return SyntaxTree.GetDiagnostics(this);
		}

		protected override bool IsEquivalentToCore(SyntaxNode node, bool topLevel = false)
		{
			return SyntaxFactory.AreEquivalent(this, (VisualBasicSyntaxNode)node, topLevel);
		}

		internal override bool ShouldCreateWeakList()
		{
			return this is Microsoft.CodeAnalysis.VisualBasic.Syntax.MethodBlockBaseSyntax;
		}

		public new SyntaxToken FindToken(int position, bool findInsideTrivia = false)
		{
			return base.FindToken(position, findInsideTrivia);
		}

		public new SyntaxTrivia FindTrivia(int textPosition, bool findInsideTrivia = false)
		{
			return base.FindTrivia(textPosition, findInsideTrivia);
		}
	}
}
