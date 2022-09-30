using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct BasesBeingResolved
	{
		public readonly ConsList<TypeSymbol> InheritsBeingResolvedOpt;

		public readonly ConsList<TypeSymbol> ImplementsBeingResolvedOpt;

		public static BasesBeingResolved Empty => default(BasesBeingResolved);

		public BasesBeingResolved(ConsList<TypeSymbol> inheritsBeingResolved, ConsList<TypeSymbol> implementsBeingResolved)
		{
			this = default(BasesBeingResolved);
			InheritsBeingResolvedOpt = inheritsBeingResolved;
			ImplementsBeingResolvedOpt = implementsBeingResolved;
		}

		public BasesBeingResolved PrependInheritsBeingResolved(TypeSymbol symbol)
		{
			return new BasesBeingResolved((InheritsBeingResolvedOpt ?? ConsList<TypeSymbol>.Empty).Prepend(symbol), ImplementsBeingResolvedOpt);
		}

		public BasesBeingResolved PrependImplementsBeingResolved(TypeSymbol symbol)
		{
			return new BasesBeingResolved(InheritsBeingResolvedOpt, (ImplementsBeingResolvedOpt ?? ConsList<TypeSymbol>.Empty).Prepend(symbol));
		}
	}
}
