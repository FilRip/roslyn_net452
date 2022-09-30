using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LazyArrayElementCantBeRefAnyDiagnosticInfo : LazyDiagnosticInfo
    {
        private readonly TypeWithAnnotations _possiblyRestrictedTypeSymbol;

        internal LazyArrayElementCantBeRefAnyDiagnosticInfo(TypeWithAnnotations possiblyRestrictedTypeSymbol)
        {
            _possiblyRestrictedTypeSymbol = possiblyRestrictedTypeSymbol;
        }

        protected override DiagnosticInfo ResolveInfo()
        {
            if (_possiblyRestrictedTypeSymbol.IsRestrictedType())
            {
                return new CSDiagnosticInfo(ErrorCode.ERR_ArrayElementCantBeRefAny, _possiblyRestrictedTypeSymbol.Type);
            }
            return null;
        }
    }
}
