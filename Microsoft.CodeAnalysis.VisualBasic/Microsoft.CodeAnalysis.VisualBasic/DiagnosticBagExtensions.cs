using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	[StandardModule]
	internal sealed class DiagnosticBagExtensions
	{
		internal static DiagnosticInfo Add(this DiagnosticBag diagnostics, ERRID code, Location location)
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(code);
			VBDiagnostic diag = new VBDiagnostic(diagnosticInfo, location);
			diagnostics.Add(diag);
			return diagnosticInfo;
		}

		internal static DiagnosticInfo Add(this DiagnosticBag diagnostics, ERRID code, Location location, params object[] args)
		{
			DiagnosticInfo diagnosticInfo = ErrorFactory.ErrorInfo(code, args);
			VBDiagnostic diag = new VBDiagnostic(diagnosticInfo, location);
			diagnostics.Add(diag);
			return diagnosticInfo;
		}

		internal static void Add(this DiagnosticBag diagnostics, DiagnosticInfo info, Location location)
		{
			VBDiagnostic diag = new VBDiagnostic(info, location);
			diagnostics.Add(diag);
		}

		internal static bool Add(this DiagnosticBag diagnostics, VisualBasicSyntaxNode node, IReadOnlyCollection<DiagnosticInfo> useSiteDiagnostics)
		{
			if (!useSiteDiagnostics.IsNullOrEmpty())
			{
				return Add(diagnostics, node.GetLocation(), useSiteDiagnostics);
			}
			return false;
		}

		internal static bool Add(this DiagnosticBag diagnostics, BoundNode node, IReadOnlyCollection<DiagnosticInfo> useSiteDiagnostics)
		{
			if (!useSiteDiagnostics.IsNullOrEmpty())
			{
				return Add(diagnostics, node.Syntax.GetLocation(), useSiteDiagnostics);
			}
			return false;
		}

		internal static bool Add(this DiagnosticBag diagnostics, SyntaxNodeOrToken node, IReadOnlyCollection<DiagnosticInfo> useSiteDiagnostics)
		{
			if (!useSiteDiagnostics.IsNullOrEmpty())
			{
				return Add(diagnostics, node.GetLocation(), useSiteDiagnostics);
			}
			return false;
		}

		internal static bool Add(this DiagnosticBag diagnostics, Location location, IReadOnlyCollection<DiagnosticInfo> useSiteDiagnostics)
		{
			if (useSiteDiagnostics.IsNullOrEmpty())
			{
				return false;
			}
			foreach (DiagnosticInfo useSiteDiagnostic in useSiteDiagnostics)
			{
				VBDiagnostic diag = new VBDiagnostic(useSiteDiagnostic, location);
				diagnostics.Add(diag);
			}
			return true;
		}
	}
}
