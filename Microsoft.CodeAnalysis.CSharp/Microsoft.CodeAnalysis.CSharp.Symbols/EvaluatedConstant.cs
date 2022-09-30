namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class EvaluatedConstant
    {
        public readonly ConstantValue Value;

        public readonly ImmutableBindingDiagnostic<AssemblySymbol> Diagnostics;

        public EvaluatedConstant(ConstantValue value, ImmutableBindingDiagnostic<AssemblySymbol> diagnostics)
        {
            Value = value;
            Diagnostics = diagnostics.NullToEmpty();
        }
    }
}
