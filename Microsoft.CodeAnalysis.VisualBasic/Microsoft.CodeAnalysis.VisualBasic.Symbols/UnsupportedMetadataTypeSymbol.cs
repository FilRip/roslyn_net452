using System;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class UnsupportedMetadataTypeSymbol : ErrorTypeSymbol
	{
		private readonly BadImageFormatException _mrEx;

		internal override bool MangleName => false;

		internal override DiagnosticInfo ErrorInfo => ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, string.Empty);

		public UnsupportedMetadataTypeSymbol(BadImageFormatException mrEx = null)
		{
			_mrEx = mrEx;
		}

		public UnsupportedMetadataTypeSymbol(string explanation)
		{
		}
	}
}
