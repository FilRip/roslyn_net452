using System.Collections.Generic;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic.CodeGen
{
	internal class Optimizer
	{
		public static BoundStatement Optimize(Symbol container, BoundStatement src, bool debugFriendly, out HashSet<LocalSymbol> stackLocals)
		{
			return StackScheduler.OptimizeLocalsOut(container, src, debugFriendly, out stackLocals);
		}
	}
}
