using System.Threading;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SimpleSyntaxReference : SyntaxReference
	{
		private readonly SyntaxTree _tree;

		private readonly SyntaxNode _node;

		public override SyntaxTree SyntaxTree => _tree;

		public override TextSpan Span => _node.Span;

		internal SimpleSyntaxReference(SyntaxTree tree, SyntaxNode node)
		{
			_tree = tree;
			_node = node;
		}

		public override SyntaxNode GetSyntax(CancellationToken cancellationToken = default(CancellationToken))
		{
			return _node;
		}
	}
}
