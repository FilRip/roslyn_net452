namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal delegate void ReportMismatchInParameterType<TArg>(BindingDiagnosticBag bag, MethodSymbol overriddenMethod, MethodSymbol overridingMethod, ParameterSymbol parameter, bool topLevel, TArg arg);
}
