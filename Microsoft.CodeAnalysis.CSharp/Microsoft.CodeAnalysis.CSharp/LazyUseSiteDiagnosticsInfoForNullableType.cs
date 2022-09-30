using Microsoft.CodeAnalysis.CSharp.Symbols;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LazyUseSiteDiagnosticsInfoForNullableType : LazyDiagnosticInfo
    {
        private readonly LanguageVersion _languageVersion;

        private readonly TypeWithAnnotations _possiblyNullableTypeSymbol;

        internal LazyUseSiteDiagnosticsInfoForNullableType(LanguageVersion languageVersion, TypeWithAnnotations possiblyNullableTypeSymbol)
        {
            _languageVersion = languageVersion;
            _possiblyNullableTypeSymbol = possiblyNullableTypeSymbol;
        }

        protected override DiagnosticInfo? ResolveInfo()
        {
            if (_possiblyNullableTypeSymbol.IsNullableType())
            {
                return _possiblyNullableTypeSymbol.Type.OriginalDefinition.GetUseSiteInfo().DiagnosticInfo;
            }
            return Binder.GetNullableUnconstrainedTypeParameterDiagnosticIfNecessary(_languageVersion, in _possiblyNullableTypeSymbol);
        }
    }
}
