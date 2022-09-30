using System.Threading;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class LazyDiagnosticInfo : DiagnosticInfo
    {
        private DiagnosticInfo? _lazyInfo;

        protected LazyDiagnosticInfo()
            : base(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, -1)
        {
        }

        public sealed override DiagnosticInfo GetResolvedInfo()
        {
            if (_lazyInfo == null)
            {
                Interlocked.CompareExchange(ref _lazyInfo, ResolveInfo() ?? CSDiagnosticInfo.VoidDiagnosticInfo, null);
            }
            return _lazyInfo;
        }

        protected abstract DiagnosticInfo? ResolveInfo();
    }
}
