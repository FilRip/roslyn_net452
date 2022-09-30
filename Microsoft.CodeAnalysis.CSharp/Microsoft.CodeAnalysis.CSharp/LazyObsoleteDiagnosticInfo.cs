using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LazyObsoleteDiagnosticInfo : DiagnosticInfo
    {
        private DiagnosticInfo _lazyActualObsoleteDiagnostic;

        private readonly object _symbolOrSymbolWithAnnotations;

        private readonly Symbol _containingSymbol;

        private readonly BinderFlags _binderFlags;

        internal LazyObsoleteDiagnosticInfo(object symbol, Symbol containingSymbol, BinderFlags binderFlags)
            : base(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, -1)
        {
            _symbolOrSymbolWithAnnotations = symbol;
            _containingSymbol = containingSymbol;
            _binderFlags = binderFlags;
            _lazyActualObsoleteDiagnostic = null;
        }

        public override DiagnosticInfo GetResolvedInfo()
        {
            if (_lazyActualObsoleteDiagnostic == null)
            {
                Symbol symbol = (_symbolOrSymbolWithAnnotations as Symbol) ?? ((TypeWithAnnotations)_symbolOrSymbolWithAnnotations).Type;
                symbol.ForceCompleteObsoleteAttribute();
                DiagnosticInfo diagnosticInfo = ((ObsoleteAttributeHelpers.GetObsoleteDiagnosticKind(symbol, _containingSymbol, forceComplete: true) == ObsoleteDiagnosticKind.Diagnostic) ? ObsoleteAttributeHelpers.CreateObsoleteDiagnostic(symbol, _binderFlags) : null);
                Interlocked.CompareExchange(ref _lazyActualObsoleteDiagnostic, diagnosticInfo ?? CSDiagnosticInfo.VoidDiagnosticInfo, null);
            }
            return _lazyActualObsoleteDiagnostic;
        }
    }
}
