using System;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	public class VisualBasicDiagnosticFormatter : DiagnosticFormatter
	{
		public new static VisualBasicDiagnosticFormatter Instance { get; } = new VisualBasicDiagnosticFormatter();


		protected VisualBasicDiagnosticFormatter()
		{
		}

		internal override string FormatSourceSpan(LinePositionSpan span, IFormatProvider formatter)
		{
			return "(" + (span.Start.Line + 1) + ") ";
		}
	}
}
