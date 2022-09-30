using System.Threading;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class LazyObsoleteDiagnosticInfo : DiagnosticInfo
	{
		private DiagnosticInfo _lazyActualObsoleteDiagnostic;

		private readonly Symbol _symbol;

		private readonly Symbol _containingSymbol;

		internal LazyObsoleteDiagnosticInfo(Symbol sym, Symbol containingSymbol)
			: base(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, -1)
		{
			_symbol = sym;
			_containingSymbol = containingSymbol;
		}

		internal override DiagnosticInfo GetResolvedInfo()
		{
			if (_lazyActualObsoleteDiagnostic == null)
			{
				_symbol.ForceCompleteObsoleteAttribute();
				DiagnosticInfo diagnosticInfo = ((ObsoleteAttributeHelpers.GetObsoleteDiagnosticKind(_containingSymbol, _symbol, forceComplete: true) == ObsoleteDiagnosticKind.Diagnostic) ? ObsoleteAttributeHelpers.CreateObsoleteDiagnostic(_symbol) : null);
				Interlocked.CompareExchange(ref _lazyActualObsoleteDiagnostic, diagnosticInfo ?? ErrorFactory.VoidDiagnosticInfo, null);
			}
			return _lazyActualObsoleteDiagnostic;
		}
	}
}
