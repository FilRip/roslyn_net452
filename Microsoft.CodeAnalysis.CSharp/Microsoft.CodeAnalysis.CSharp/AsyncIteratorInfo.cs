using Microsoft.CodeAnalysis.CSharp.Symbols;

namespace Microsoft.CodeAnalysis.CSharp
{
    internal sealed class AsyncIteratorInfo
    {
        internal FieldSymbol PromiseOfValueOrEndField { get; }

        internal FieldSymbol CombinedTokensField { get; }

        internal FieldSymbol CurrentField { get; }

        internal FieldSymbol DisposeModeField { get; }

        internal MethodSymbol SetResultMethod { get; }

        internal MethodSymbol SetExceptionMethod { get; }

        public AsyncIteratorInfo(FieldSymbol promiseOfValueOrEndField, FieldSymbol combinedTokensField, FieldSymbol currentField, FieldSymbol disposeModeField, MethodSymbol setResultMethod, MethodSymbol setExceptionMethod)
        {
            PromiseOfValueOrEndField = promiseOfValueOrEndField;
            CombinedTokensField = combinedTokensField;
            CurrentField = currentField;
            DisposeModeField = disposeModeField;
            SetResultMethod = setResultMethod;
            SetExceptionMethod = setExceptionMethod;
        }
    }
}
