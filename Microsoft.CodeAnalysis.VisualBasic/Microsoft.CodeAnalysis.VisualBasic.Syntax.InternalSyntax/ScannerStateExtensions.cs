using Microsoft.VisualBasic.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Syntax.InternalSyntax
{
	[StandardModule]
	internal sealed class ScannerStateExtensions
	{
		internal static bool IsVBState(this ScannerState state)
		{
			return state <= ScannerState.VBAllowLeadingMultilineTrivia;
		}
	}
}
