using System.Runtime.InteropServices;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class SourceModuleBinder : Binder
	{
		private readonly SourceModuleSymbol _sourceModule;

		public override OptionStrict OptionStrict => _sourceModule.Options.OptionStrict;

		public override bool OptionInfer => _sourceModule.Options.OptionInfer;

		public override bool OptionExplicit => _sourceModule.Options.OptionExplicit;

		public override bool OptionCompareText => _sourceModule.Options.OptionCompareText;

		public override bool CheckOverflow => _sourceModule.Options.CheckOverflow;

		public override QuickAttributeChecker QuickAttributeChecker => _sourceModule.QuickAttributeChecker;

		public SourceModuleBinder(Binder containingBinder, SourceModuleSymbol sourceModule)
			: base(containingBinder, sourceModule, sourceModule.ContainingSourceAssembly.DeclaringCompilation)
		{
			_sourceModule = sourceModule;
		}

		public override AccessCheckResult CheckAccessibility(Symbol sym, [In][Out] ref CompoundUseSiteInfo<AssemblySymbol> useSiteInfo, TypeSymbol accessThroughType = null, BasesBeingResolved basesBeingResolved = default(BasesBeingResolved))
		{
			if (!base.IgnoresAccessibility)
			{
				return AccessCheck.CheckSymbolAccessibility(sym, _sourceModule.ContainingSourceAssembly, ref useSiteInfo, basesBeingResolved);
			}
			return AccessCheckResult.Accessible;
		}
	}
}
