using Microsoft.CodeAnalysis.VisualBasic.Symbols;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LocalInProgressBinder : Binder
	{
		private readonly ConsList<LocalSymbol> _symbols;

		public override ConsList<LocalSymbol> ImplicitlyTypedLocalsBeingBound => _symbols;

		public LocalInProgressBinder(Binder containingBinder, LocalSymbol symbol)
			: base(containingBinder)
		{
			_symbols = new ConsList<LocalSymbol>(symbol, containingBinder.ImplicitlyTypedLocalsBeingBound);
		}
	}
}
