using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.CSharp
{
    public class DiagnosticInfoWithSymbols : DiagnosticInfo
    {
        internal readonly ImmutableArray<Symbol> Symbols;

        public DiagnosticInfoWithSymbols(ErrorCode errorCode, object[] arguments, ImmutableArray<Symbol> symbols)
            : base(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, (int)errorCode, arguments)
        {
            Symbols = symbols;
        }

        public DiagnosticInfoWithSymbols(bool isWarningAsError, ErrorCode errorCode, object[] arguments, ImmutableArray<Symbol> symbols)
            : base(Microsoft.CodeAnalysis.CSharp.MessageProvider.Instance, isWarningAsError, (int)errorCode, arguments)
        {
            Symbols = symbols;
        }
    }
}
