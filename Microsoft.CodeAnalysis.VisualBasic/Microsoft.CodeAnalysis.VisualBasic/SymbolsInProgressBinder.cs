namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class SymbolsInProgressBinder<T> : Binder where T : Symbol
	{
		protected readonly SymbolsInProgress<T> inProgress;

		protected SymbolsInProgressBinder(SymbolsInProgress<T> inProgress, Binder next)
			: base(next)
		{
			this.inProgress = inProgress;
		}
	}
}
