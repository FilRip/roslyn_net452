using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal sealed class Blender : Scanner
	{
		private struct NextPreprocessorStateGetter
		{
			private readonly PreprocessorState _state;

			private readonly VisualBasicSyntaxNode _node;

			private PreprocessorState _nextState;

			public bool Valid => _node != null;

			public NextPreprocessorStateGetter(PreprocessorState state, VisualBasicSyntaxNode node)
			{
				this = default(NextPreprocessorStateGetter);
				_state = state;
				_node = node;
				_nextState = null;
			}

			public PreprocessorState State()
			{
				if (_nextState == null)
				{
					_nextState = Scanner.ApplyDirectives(_state, _node);
				}
				return _nextState;
			}
		}

		private readonly Stack<GreenNode> _nodeStack;

		private readonly TextChangeRange _change;

		private readonly TextChangeRange _affectedRange;

		private VisualBasicSyntaxNode _currentNode;

		private int _curNodeStart;

		private int _curNodeLength;

		private readonly Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode _baseTreeRoot;

		private PreprocessorState _currentPreprocessorState;

		private NextPreprocessorStateGetter _nextPreprocessorStateGetter;

		private static void PushReverseNonterminal(Stack<GreenNode> stack, GreenNode nonterminal)
		{
			int slotCount = nonterminal.SlotCount;
			int num = slotCount;
			for (int i = 1; i <= num; i++)
			{
				GreenNode slot = nonterminal.GetSlot(slotCount - i);
				PushChildReverse(stack, slot);
			}
		}

		private static void PushReverseTerminal(Stack<GreenNode> stack, SyntaxToken tk)
		{
			GreenNode trailingTrivia = tk.GetTrailingTrivia();
			if (trailingTrivia != null)
			{
				PushChildReverse(stack, trailingTrivia);
			}
			PushChildReverse(stack, (SyntaxToken)tk.WithLeadingTrivia(null).WithTrailingTrivia(null));
			trailingTrivia = tk.GetLeadingTrivia();
			if (trailingTrivia != null)
			{
				PushChildReverse(stack, trailingTrivia);
			}
		}

		private static void PushChildReverse(Stack<GreenNode> stack, GreenNode child)
		{
			if (child != null)
			{
				if (child.IsList)
				{
					PushReverseNonterminal(stack, child);
				}
				else
				{
					stack.Push(child);
				}
			}
		}

		private static TextSpan ExpandToNearestStatements(Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode root, TextSpan span)
		{
			TextSpan rootFullSpan = new TextSpan(0, root.FullWidth);
			TextSpan result = NearestStatementThatContainsPosition(root, span.Start, rootFullSpan);
			if (span.Length == 0)
			{
				return result;
			}
			return TextSpan.FromBounds(end: NearestStatementThatContainsPosition(root, span.End - 1, rootFullSpan).End, start: result.Start);
		}

		private static TextSpan NearestStatementThatContainsPosition(SyntaxNode node, int position, TextSpan rootFullSpan)
		{
			if (!node.FullSpan.Contains(position))
			{
				return new TextSpan(position, 0);
			}
			if (VisualBasicExtensions.Kind(node) == SyntaxKind.CompilationUnit || IsStatementLike(node))
			{
				while (true)
				{
					SyntaxNode syntaxNode = node.ChildThatContainsPosition(position).AsNode();
					if (syntaxNode == null || !IsStatementLike(syntaxNode))
					{
						break;
					}
					node = syntaxNode;
				}
				return node.FullSpan;
			}
			return rootFullSpan;
		}

		private static bool IsStatementLike(SyntaxNode node)
		{
			switch (VisualBasicExtensions.Kind(node))
			{
			case SyntaxKind.ElseIfBlock:
			case SyntaxKind.ElseBlock:
			case SyntaxKind.CatchBlock:
			case SyntaxKind.FinallyBlock:
				return Microsoft.CodeAnalysis.VisualBasicExtensions.Any(node.GetTrailingTrivia(), SyntaxKind.EndOfLineTrivia);
			case SyntaxKind.SingleLineIfStatement:
			case SyntaxKind.SingleLineElseClause:
				return false;
			default:
				return node is Microsoft.CodeAnalysis.VisualBasic.Syntax.StatementSyntax;
			}
		}

		private static TextSpan ExpandByLookAheadAndBehind(Microsoft.CodeAnalysis.VisualBasic.VisualBasicSyntaxNode root, TextSpan span)
		{
			int fullWidth = root.FullWidth;
			int num = Math.Min(span.Start, Math.Max(0, fullWidth - 1));
			int num2 = span.End;
			if (num > 0)
			{
				int num3 = 0;
				do
				{
					Microsoft.CodeAnalysis.SyntaxToken token = root.FindTokenInternal(num);
					if (VisualBasicExtensions.Kind(token) == SyntaxKind.None)
					{
						break;
					}
					num = token.Position;
					if (num == 0)
					{
						break;
					}
					num--;
					num3++;
				}
				while (num3 <= 4);
			}
			if (num2 < fullWidth)
			{
				num2++;
			}
			return TextSpan.FromBounds(num, num2);
		}

		internal Blender(SourceText newText, TextChangeRange[] changes, SyntaxTree baseTreeRoot, VisualBasicParseOptions options)
			: base(newText, options)
		{
			_nodeStack = new Stack<GreenNode>();
			_currentPreprocessorState = _scannerPreprocessorState;
			_nextPreprocessorStateGetter = default(NextPreprocessorStateGetter);
			_baseTreeRoot = VisualBasicExtensions.GetVisualBasicRoot(baseTreeRoot);
			_currentNode = _baseTreeRoot.VbGreen;
			_curNodeStart = 0;
			_curNodeLength = 0;
			TryCrumbleOnce();
			if (_currentNode != null)
			{
				_change = TextChangeRange.Collapse(changes);
				TextSpan span = ExpandToNearestStatements(_baseTreeRoot, ExpandByLookAheadAndBehind(_baseTreeRoot, _change.Span));
				_affectedRange = new TextChangeRange(span, span.Length - _change.Span.Length + _change.NewLength);
			}
		}

		private int MapNewPositionToOldTree(int position)
		{
			TextChangeRange change = _change;
			if (position < change.Span.Start)
			{
				return position;
			}
			change = _change;
			int start = change.Span.Start;
			change = _change;
			if (position >= start + change.NewLength)
			{
				change = _change;
				int num = position - change.NewLength;
				change = _change;
				return num + change.Span.Length;
			}
			return -1;
		}

		private bool TryPopNode()
		{
			if (_nodeStack.Count > 0)
			{
				GreenNode greenNode = _nodeStack.Pop();
				_currentNode = (VisualBasicSyntaxNode)greenNode;
				_curNodeStart += _curNodeLength;
				_curNodeLength = greenNode.FullWidth;
				if (_nextPreprocessorStateGetter.Valid)
				{
					_currentPreprocessorState = _nextPreprocessorStateGetter.State();
				}
				_nextPreprocessorStateGetter = new NextPreprocessorStateGetter(_currentPreprocessorState, (VisualBasicSyntaxNode)greenNode);
				return true;
			}
			_currentNode = null;
			return false;
		}

		internal override bool TryCrumbleOnce()
		{
			if (_currentNode == null)
			{
				return false;
			}
			if (_currentNode.SlotCount == 0)
			{
				if (!_currentNode.ContainsStructuredTrivia)
				{
					return false;
				}
				PushReverseTerminal(_nodeStack, (SyntaxToken)_currentNode);
			}
			else
			{
				if (!ShouldCrumble(_currentNode))
				{
					return false;
				}
				PushReverseNonterminal(_nodeStack, _currentNode);
			}
			_curNodeLength = 0;
			_nextPreprocessorStateGetter = default(NextPreprocessorStateGetter);
			return TryPopNode();
		}

		private static bool ShouldCrumble(VisualBasicSyntaxNode node)
		{
			if (node is StructuredTriviaSyntax)
			{
				return false;
			}
			switch (node.Kind)
			{
			case SyntaxKind.SingleLineIfStatement:
			case SyntaxKind.SingleLineElseClause:
				return false;
			case SyntaxKind.EnumBlock:
				return false;
			default:
				return true;
			}
		}

		private VisualBasicSyntaxNode GetCurrentNode(int position)
		{
			int num = MapNewPositionToOldTree(position);
			if (num == -1)
			{
				return null;
			}
			while (true)
			{
				if (_curNodeStart > num)
				{
					return null;
				}
				if (_curNodeStart + _curNodeLength <= num)
				{
					if (!TryPopNode())
					{
						return null;
					}
					continue;
				}
				if (_curNodeStart == num && CanReuseNode(_currentNode))
				{
					break;
				}
				if (!TryCrumbleOnce())
				{
					return null;
				}
			}
			return _currentNode;
		}

		internal override VisualBasicSyntaxNode GetCurrentSyntaxNode()
		{
			if (_currentNode == null)
			{
				return null;
			}
			int position = _currentToken.Position;
			TextChangeRange affectedRange = _affectedRange;
			int start = affectedRange.Span.Start;
			affectedRange = _affectedRange;
			TextSpan textSpan = new TextSpan(start, affectedRange.NewLength);
			if (textSpan.Contains(position))
			{
				return null;
			}
			return GetCurrentNode(position);
		}

		private bool CanReuseNode(VisualBasicSyntaxNode node)
		{
			if (node == null)
			{
				return false;
			}
			if (node.SlotCount == 0)
			{
				return false;
			}
			if (node.ContainsDiagnostics)
			{
				return false;
			}
			if (node.ContainsAnnotations)
			{
				return false;
			}
			if (node.Kind == SyntaxKind.IfStatement)
			{
				return false;
			}
			TextSpan textSpan = new TextSpan(_curNodeStart, _curNodeLength);
			TextChangeRange affectedRange = _affectedRange;
			if (affectedRange.Span.Length == 0)
			{
				affectedRange = _affectedRange;
				if (textSpan.Contains(affectedRange.Span.Start))
				{
					return false;
				}
			}
			else
			{
				affectedRange = _affectedRange;
				if (textSpan.OverlapsWith(affectedRange.Span))
				{
					return false;
				}
			}
			if (node.ContainsDirectives && !(node is DirectiveTriviaSyntax))
			{
				return _scannerPreprocessorState.IsEquivalentTo(_currentPreprocessorState);
			}
			if (_currentToken.State != ScannerState.VBAllowLeadingMultilineTrivia && ContainsLeadingLineBreaks(node))
			{
				return false;
			}
			if (_currentNode.IsMissing)
			{
				return false;
			}
			return true;
		}

		private bool ContainsLeadingLineBreaks(VisualBasicSyntaxNode node)
		{
			GreenNode leadingTrivia = node.GetLeadingTrivia();
			if (leadingTrivia != null)
			{
				if (leadingTrivia.RawKind == 730)
				{
					return true;
				}
				if (leadingTrivia is SyntaxList syntaxList)
				{
					int num = syntaxList.SlotCount - 1;
					for (int i = 0; i <= num; i++)
					{
						if (leadingTrivia.GetSlot(i)!.RawKind == 730)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		internal override void MoveToNextSyntaxNode()
		{
			if (_currentNode != null)
			{
				PreprocessorState preprocessorState = _nextPreprocessorStateGetter.State();
				_lineBufferOffset = _currentToken.Position + _curNodeLength;
				if (_currentNode.ContainsDirectives)
				{
					_currentToken = _currentToken.With(preprocessorState);
				}
				base.MoveToNextSyntaxNode();
				TryPopNode();
			}
		}

		internal override void MoveToNextSyntaxNodeInTrivia()
		{
			if (_currentNode != null)
			{
				_lineBufferOffset += _curNodeLength;
				base.MoveToNextSyntaxNodeInTrivia();
				TryPopNode();
			}
		}
	}
}
