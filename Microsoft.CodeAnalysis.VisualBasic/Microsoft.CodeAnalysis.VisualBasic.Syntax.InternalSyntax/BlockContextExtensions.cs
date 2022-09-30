using System;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class BlockContextExtensions
	{
		internal static BlockContext EndLambda(this BlockContext context)
		{
			bool flag = false;
			do
			{
				flag = context.IsLambda;
				context = context.EndBlock(null);
			}
			while (!flag);
			return context;
		}

		internal static void RecoverFromMissingEnd(this BlockContext context, BlockContext lastContext)
		{
			while (context.Level > lastContext.Level)
			{
				context = context.EndBlock(null);
			}
		}

		internal static bool IsWithin(this BlockContext context, params SyntaxKind[] kinds)
		{
			return FindNearest(context, kinds) != null;
		}

		internal static BlockContext FindNearest(this BlockContext context, Func<BlockContext, bool> conditionIsTrue)
		{
			while (context != null)
			{
				if (conditionIsTrue(context))
				{
					return context;
				}
				context = context.PrevBlock;
			}
			return null;
		}

		internal static BlockContext FindNearest(this BlockContext context, Func<SyntaxKind, bool> conditionIsTrue)
		{
			while (context != null)
			{
				if (conditionIsTrue(context.BlockKind))
				{
					return context;
				}
				context = context.PrevBlock;
			}
			return null;
		}

		internal static BlockContext FindNearest(this BlockContext context, params SyntaxKind[] kinds)
		{
			while (context != null)
			{
				if (SyntaxKindExtensions.Contains(kinds, context.BlockKind))
				{
					return context;
				}
				context = context.PrevBlock;
			}
			return null;
		}

		internal static BlockContext FindNearestInSameMethodScope(this BlockContext context, params SyntaxKind[] kinds)
		{
			while (context != null)
			{
				if (SyntaxKindExtensions.Contains(kinds, context.BlockKind))
				{
					return context;
				}
				if (context.IsLambda)
				{
					return null;
				}
				context = context.PrevBlock;
			}
			return null;
		}

		internal static BlockContext FindNearestLambdaOrSingleLineIf(this BlockContext context, BlockContext lastContext)
		{
			while (context != lastContext)
			{
				if (context.IsLambda || context.IsLineIf)
				{
					return context;
				}
				context = context.PrevBlock;
			}
			return null;
		}
	}
}
