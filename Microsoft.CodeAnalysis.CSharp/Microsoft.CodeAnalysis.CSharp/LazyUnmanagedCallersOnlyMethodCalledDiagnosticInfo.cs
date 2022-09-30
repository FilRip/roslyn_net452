using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LazyUnmanagedCallersOnlyMethodCalledDiagnosticInfo : DiagnosticInfo
    {
        private DiagnosticInfo? _lazyActualUnmanagedCallersOnlyDiagnostic;

        private readonly MethodSymbol _method;

        private readonly bool _isDelegateConversion;

        internal LazyUnmanagedCallersOnlyMethodCalledDiagnosticInfo(MethodSymbol method, bool isDelegateConversion)
            : base(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, -1)
        {
            _method = method;
            _lazyActualUnmanagedCallersOnlyDiagnostic = null;
            _isDelegateConversion = isDelegateConversion;
        }

        public override DiagnosticInfo GetResolvedInfo()
        {
            if (_lazyActualUnmanagedCallersOnlyDiagnostic == null)
            {
                DiagnosticInfo value = ((_method.GetUnmanagedCallersOnlyAttributeData(forceComplete: true) == null) ? CSDiagnosticInfo.VoidDiagnosticInfo : new CSDiagnosticInfo(_isDelegateConversion ? ErrorCode.ERR_UnmanagedCallersOnlyMethodsCannotBeConvertedToDelegate : ErrorCode.ERR_UnmanagedCallersOnlyMethodsCannotBeCalledDirectly, _method));
                Interlocked.CompareExchange(ref _lazyActualUnmanagedCallersOnlyDiagnostic, value, null);
            }
            return _lazyActualUnmanagedCallersOnlyDiagnostic;
        }
    }
}
