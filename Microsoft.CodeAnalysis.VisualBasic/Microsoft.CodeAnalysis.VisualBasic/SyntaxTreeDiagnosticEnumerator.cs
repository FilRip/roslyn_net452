using System;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct SyntaxTreeDiagnosticEnumerator
	{
		private struct NodeIteration
		{
			internal readonly GreenNode node;

			internal int diagnosticIndex;

			internal int slotIndex;

			internal readonly bool inDocumentationComment;

			internal NodeIteration(GreenNode node, bool inDocumentationComment)
			{
				this = default(NodeIteration);
				this.node = node;
				slotIndex = -1;
				diagnosticIndex = -1;
				this.inDocumentationComment = inDocumentationComment;
			}
		}

		private readonly SyntaxTree _tree;

		private NodeIteration[] _stack;

		private int _count;

		private Diagnostic _current;

		private int _position;

		public Diagnostic Current => _current;

		internal SyntaxTreeDiagnosticEnumerator(SyntaxTree tree, Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.VisualBasicSyntaxNode node, int position, bool inDocumentationComment)
		{
			this = default(SyntaxTreeDiagnosticEnumerator);
			if (node != null && node.ContainsDiagnostics)
			{
				_tree = tree;
				_stack = new NodeIteration[8];
				Push(node, inDocumentationComment);
			}
			else
			{
				_tree = null;
				_stack = null;
				_count = 0;
			}
			_current = null;
			_position = position;
		}

		public bool MoveNext()
		{
			while (_count > 0)
			{
				int diagnosticIndex = _stack[_count - 1].diagnosticIndex;
				GreenNode node = _stack[_count - 1].node;
				DiagnosticInfo[] diagnostics = node.GetDiagnostics();
				bool inDocumentationComment = _stack[_count - 1].inDocumentationComment;
				if (diagnostics != null && diagnosticIndex < diagnostics.Length - 1)
				{
					diagnosticIndex++;
					DiagnosticInfo diagnosticInfo = diagnostics[diagnosticIndex];
					if (inDocumentationComment)
					{
						diagnosticInfo = ErrorFactory.ErrorInfo(ERRID.WRN_XMLDocParseError1, diagnosticInfo);
					}
					int num = _position;
					if (!node.IsToken)
					{
						num += node.GetLeadingTriviaWidth();
					}
					_current = new VBDiagnostic(diagnosticInfo, _tree.GetLocation(new TextSpan(num, node.Width)));
					_stack[_count - 1].diagnosticIndex = diagnosticIndex;
					return true;
				}
				int num2 = _stack[_count - 1].slotIndex;
				inDocumentationComment = inDocumentationComment || node.RawKind == 710;
				while (true)
				{
					if (num2 < node.SlotCount - 1)
					{
						num2++;
						GreenNode slot = node.GetSlot(num2);
						if (slot != null)
						{
							if (slot.ContainsDiagnostics)
							{
								_stack[_count - 1].slotIndex = num2;
								Push(slot, inDocumentationComment);
								break;
							}
							_position += slot.FullWidth;
						}
						continue;
					}
					if (node.SlotCount == 0)
					{
						_position += node.Width;
					}
					Pop();
					break;
				}
			}
			return false;
		}

		private void Push(GreenNode node, bool inDocumentationComment)
		{
			if (node is Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken token)
			{
				PushToken(token, inDocumentationComment);
			}
			else
			{
				PushNode(node, inDocumentationComment);
			}
		}

		private void PushToken(Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax.SyntaxToken token, bool inDocumentationComment)
		{
			GreenNode trailingTrivia = token.GetTrailingTrivia();
			if (trailingTrivia != null)
			{
				Push(trailingTrivia, inDocumentationComment);
			}
			PushNode(token, inDocumentationComment);
			GreenNode leadingTrivia = token.GetLeadingTrivia();
			if (leadingTrivia != null)
			{
				Push(leadingTrivia, inDocumentationComment);
			}
		}

		private void PushNode(GreenNode node, bool inDocumentationComment)
		{
			if (_count >= _stack.Length)
			{
				NodeIteration[] array = new NodeIteration[_stack.Length * 2 - 1 + 1];
				Array.Copy(_stack, array, _stack.Length);
				_stack = array;
			}
			_stack[_count] = new NodeIteration(node, inDocumentationComment);
			_count++;
		}

		private void Pop()
		{
			_count--;
		}
	}
}
