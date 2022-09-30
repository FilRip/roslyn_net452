using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class Analyzer
	{
		internal static void AnalyzeMethodBody(MethodSymbol method, BoundBlock body, DiagnosticBag diagnostics)
		{
			DiagnosticBag diagnosticBag = diagnostics;
			if (method.IsImplicitlyDeclared && (object)method.AssociatedSymbol != null && method.AssociatedSymbol.IsMyGroupCollectionProperty)
			{
				diagnosticBag = DiagnosticBag.GetInstance();
			}
			FlowAnalysisPass.Analyze(method, body, diagnosticBag);
			ForLoopVerification.VerifyForLoops(body, diagnosticBag);
			if (diagnosticBag != diagnostics)
			{
				((SynthesizedMyGroupCollectionPropertySymbol)method.AssociatedSymbol).RelocateDiagnostics(diagnosticBag, diagnostics);
				diagnosticBag.Free();
			}
		}
	}
}
