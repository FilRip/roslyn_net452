using System;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis
{
	[StandardModule]
	public sealed class VisualBasicExtensions
	{
		public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind)
		{
			return trivia.RawKind == (int)kind;
		}

		public static bool IsKind(this SyntaxToken token, SyntaxKind kind)
		{
			return token.RawKind == (int)kind;
		}

		public static bool IsKind(this SyntaxNode node, SyntaxKind kind)
		{
			if (node != null)
			{
				return node.RawKind == (int)kind;
			}
			return false;
		}

		public static bool IsKind(this SyntaxNodeOrToken nodeOrToken, SyntaxKind kind)
		{
			return nodeOrToken.RawKind == (int)kind;
		}

		public static int IndexOf<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
		{
			return list.IndexOf((int)kind);
		}

		public static bool Any<TNode>(this SyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
		{
			return list.IndexOf((int)kind) >= 0;
		}

		public static int IndexOf<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
		{
			return list.IndexOf((int)kind);
		}

		public static bool Any<TNode>(this SeparatedSyntaxList<TNode> list, SyntaxKind kind) where TNode : SyntaxNode
		{
			return list.IndexOf((int)kind) >= 0;
		}

		public static int IndexOf(this SyntaxTriviaList list, SyntaxKind kind)
		{
			return list.IndexOf((int)kind);
		}

		public static bool Any(this SyntaxTriviaList list, SyntaxKind kind)
		{
			return list.IndexOf((int)kind) >= 0;
		}

		public static int IndexOf(this SyntaxTokenList list, SyntaxKind kind)
		{
			return list.IndexOf((int)kind);
		}

		public static bool Any(this SyntaxTokenList list, SyntaxKind kind)
		{
			return list.IndexOf((int)kind) >= 0;
		}

		internal static SyntaxToken FirstOrDefault(this SyntaxTokenList list, SyntaxKind kind)
		{
			int num = list.IndexOf((int)kind);
			if (num < 0)
			{
				return default(SyntaxToken);
			}
			return list[num];
		}

		internal static SyntaxToken First(this SyntaxTokenList list, SyntaxKind kind)
		{
			int num = list.IndexOf((int)kind);
			if (num < 0)
			{
				throw new InvalidOperationException();
			}
			return list[num];
		}
	}
}
