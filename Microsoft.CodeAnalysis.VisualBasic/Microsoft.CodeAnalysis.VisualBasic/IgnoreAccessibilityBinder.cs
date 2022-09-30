using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class IgnoreAccessibilityBinder : Binder
	{
		public IgnoreAccessibilityBinder(Binder containingBinder)
			: base(containingBinder)
		{
		}

		internal override LookupOptions BinderSpecificLookupOptions(LookupOptions options)
		{
			return base.ContainingBinder.BinderSpecificLookupOptions(options) | LookupOptions.IgnoreAccessibility;
		}

		public override AccessCheckResult CheckAccessibility(Symbol sym, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeSymbol accessThroughType = null, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			return AccessCheckResult.Accessible;
		}
	}
}
