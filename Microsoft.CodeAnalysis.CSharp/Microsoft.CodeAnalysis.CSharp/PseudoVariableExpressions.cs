namespace Microsoft.CodeAnalysis.CSharp
{
    public abstract class PseudoVariableExpressions
    {
        internal abstract BoundExpression GetValue(BoundPseudoVariable variable, DiagnosticBag diagnostics);

        internal abstract BoundExpression GetAddress(BoundPseudoVariable variable);
    }
}
