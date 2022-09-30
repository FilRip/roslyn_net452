using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BasesBeingResolvedBinder : Binder
	{
		public BasesBeingResolvedBinder(Binder containingBinder, BasesBeingResolved basesBeingResolved)
			: base(containingBinder, basesBeingResolved)
		{
		}

		public override AccessCheckResult CheckAccessibility(Symbol sym, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeSymbol accessThroughType = null, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			BasesBeingResolved basesBeingResolved2 = BasesBeingResolved();
			foreach (TypeSymbol item in basesBeingResolved.InheritsBeingResolvedOpt ?? ConsList<TypeSymbol>.Empty)
			{
				basesBeingResolved2 = basesBeingResolved2.PrependInheritsBeingResolved(item);
			}
			foreach (TypeSymbol item2 in basesBeingResolved.ImplementsBeingResolvedOpt ?? ConsList<TypeSymbol>.Empty)
			{
				basesBeingResolved2 = basesBeingResolved2.PrependImplementsBeingResolved(item2);
			}
			return m_containingBinder.CheckAccessibility(sym, ref useSiteInfo, accessThroughType, basesBeingResolved2);
		}
	}
}
