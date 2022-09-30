using System;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	internal class LastTokenReplacer : VisualBasicSyntaxRewriter
	{
		private readonly Func<SyntaxToken, SyntaxToken> _newItem;

		private int _skipCnt;

		private LastTokenReplacer(Func<SyntaxToken, SyntaxToken> newItem)
		{
			_newItem = newItem;
		}

		internal static TTree Replace<TTree>(TTree root, Func<SyntaxToken, SyntaxToken> newItem) where TTree : GreenNode
		{
			return (TTree)new LastTokenReplacer(newItem).VisitGreen(root);
		}

		public override VisualBasicSyntaxNode Visit(VisualBasicSyntaxNode node)
		{
			return (VisualBasicSyntaxNode)VisitGreen(node);
		}

		private GreenNode VisitGreen(GreenNode node)
		{
			if (node == null)
			{
				return null;
			}
			if (_skipCnt != 0)
			{
				_skipCnt--;
				return node;
			}
			if (!node.IsToken)
			{
				int num = 0;
				int num2 = node.SlotCount - 1;
				for (int i = 0; i <= num2; i++)
				{
					GreenNode slot = node.GetSlot(i);
					if (slot != null)
					{
						num = ((!slot.IsList) ? (num + 1) : (num + slot.SlotCount));
					}
				}
				if (num == 0)
				{
					return node;
				}
				int skipCnt = _skipCnt;
				_skipCnt = num - 1;
				GreenNode result = ((!node.IsList) ? base.Visit((VisualBasicSyntaxNode)node) : VisitList(new Microsoft.CodeAnalysis.Syntax.InternalSyntax.SyntaxList<VisualBasicSyntaxNode>(node)).Node);
				_skipCnt = skipCnt;
				return result;
			}
			return base.Visit((SyntaxToken)node);
		}

		public override SyntaxToken VisitSyntaxToken(SyntaxToken token)
		{
			return _newItem(token);
		}
	}
}
