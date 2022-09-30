using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class TopLevelCodeBinder : SubOrFunctionBodyBinder
	{
		public override bool IsInQuery => false;

		public TopLevelCodeBinder(MethodSymbol scriptInitializer, Binder containingBinder)
			: base(scriptInitializer, scriptInitializer.Syntax, containingBinder)
		{
		}

		public override LocalSymbol GetLocalForFunctionValue()
		{
			return null;
		}
	}
}
