using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class FlowAnalysisPass
	{
		public static void Analyze(MethodSymbol method, BoundBlock block, DiagnosticBag diagnostics)
		{
			Analyze(method.DeclaringCompilation, method, block, diagnostics);
		}

		private static void Analyze(VisualBasicCompilation compilation, MethodSymbol method, BoundBlock block, DiagnosticBag diagnostics)
		{
			ControlFlowPass.Analyze(new FlowAnalysisInfo(compilation, method, block), diagnostics, suppressConstantExpressionsSupport: true);
			DataFlowPass.Analyze(new FlowAnalysisInfo(compilation, method, block), diagnostics, suppressConstExpressionsSupport: true);
		}
	}
}
