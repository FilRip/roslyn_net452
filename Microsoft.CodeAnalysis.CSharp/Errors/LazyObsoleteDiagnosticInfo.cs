// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class LazyObsoleteDiagnosticInfo : DiagnosticInfo
    {
        private DiagnosticInfo _lazyActualObsoleteDiagnostic;

        private readonly object _symbolOrSymbolWithAnnotations;
        private readonly Symbol _containingSymbol;
        private readonly EBinder _binderFlags;

        internal LazyObsoleteDiagnosticInfo(object symbol, Symbol containingSymbol, EBinder binderFlags)
            : base(CSharp.MessageProvider.Instance, (int)ErrorCode.Unknown)
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
                // A symbol's Obsoleteness may not have been calculated yet if the symbol is coming
                // from a different compilation's source. In that case, force completion of attributes.
                var symbol = (_symbolOrSymbolWithAnnotations as Symbol) ?? ((TypeWithAnnotations)_symbolOrSymbolWithAnnotations).Type;
                symbol.ForceCompleteObsoleteAttribute();

                var kind = ObsoleteAttributeHelpers.GetObsoleteDiagnosticKind(symbol, _containingSymbol, forceComplete: true);

                var info = (kind == ObsoleteDiagnosticKind.Diagnostic) ?
                    ObsoleteAttributeHelpers.CreateObsoleteDiagnostic(symbol, _binderFlags) :
                    null;

                // If this symbol is not obsolete or is in an obsolete context, we don't want to report any diagnostics.
                // Therefore make this a Void diagnostic.
                Interlocked.CompareExchange(ref _lazyActualObsoleteDiagnostic, info ?? CSDiagnosticInfo.VoidDiagnosticInfo, null);
            }

            return _lazyActualObsoleteDiagnostic;
        }
    }
}
