using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class FirstTokenReplacer : VisualBasicSyntaxRewriter
	{
		private readonly Func<SyntaxToken, SyntaxToken> _newItem;

		private bool _isFirst;

		private FirstTokenReplacer(Func<SyntaxToken, SyntaxToken> newItem)
		{
			_isFirst = true;
			_newItem = newItem;
		}

		internal static TTree Replace<TTree>(TTree root, Func<SyntaxToken, SyntaxToken> newItem) where TTree : VisualBasicSyntaxNode
		{
			return (TTree)new FirstTokenReplacer(newItem).Visit(root);
		}

		public override VisualBasicSyntaxNode Visit(VisualBasicSyntaxNode node)
		{
			if (node == null)
			{
				return null;
			}
			if (!_isFirst)
			{
				return node;
			}
			VisualBasicSyntaxNode result = base.Visit(node);
			_isFirst = false;
			return result;
		}

		public override SyntaxToken VisitSyntaxToken(SyntaxToken token)
		{
			return _newItem(token);
		}
	}
}
