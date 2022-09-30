namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class PseudoVariableExpressions
	{
		internal abstract BoundExpression GetAddress(BoundPseudoVariable variable);

		internal abstract BoundExpression GetValue(BoundPseudoVariable variable, DiagnosticBag diagnostics);
	}
}
