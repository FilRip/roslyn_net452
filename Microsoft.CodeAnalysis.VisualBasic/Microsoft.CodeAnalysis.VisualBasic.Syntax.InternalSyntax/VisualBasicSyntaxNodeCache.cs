using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class VisualBasicSyntaxNodeCache
	{
		internal static GreenNode TryGetNode(int kind, GreenNode child1, ISyntaxFactoryContext context, ref int hash)
		{
			return SyntaxNodeCache.TryGetNode(kind, child1, GetNodeFlags(context), out hash);
		}

		internal static GreenNode TryGetNode(int kind, GreenNode child1, GreenNode child2, ISyntaxFactoryContext context, ref int hash)
		{
			return SyntaxNodeCache.TryGetNode(kind, child1, child2, GetNodeFlags(context), out hash);
		}

		internal static GreenNode TryGetNode(int kind, GreenNode child1, GreenNode child2, GreenNode child3, ISyntaxFactoryContext context, ref int hash)
		{
			return SyntaxNodeCache.TryGetNode(kind, child1, child2, child3, GetNodeFlags(context), out hash);
		}

		private static GreenNode.NodeFlags GetNodeFlags(ISyntaxFactoryContext context)
		{
			GreenNode.NodeFlags nodeFlags = SyntaxNodeCache.GetDefaultNodeFlags();
			if (context.IsWithinAsyncMethodOrLambda)
			{
				nodeFlags |= GreenNode.NodeFlags.FactoryContextIsInAsync;
			}
			if (context.IsWithinIteratorContext)
			{
				nodeFlags |= GreenNode.NodeFlags.FactoryContextIsInQuery;
			}
			return nodeFlags;
		}
	}
}
