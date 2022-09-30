namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal delegate void ReportMismatchInReturnType<TArg>(BindingDiagnosticBag bag, MethodSymbol overriddenMethod, MethodSymbol overridingMethod, bool topLevel, TArg arg);
}
