using System.Collections.Generic;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BadSymbolDiagnostic : DiagnosticInfo, IDiagnosticInfoWithSymbols
	{
		private readonly Symbol _badSymbol;

		public Symbol BadSymbol => _badSymbol;

		public override IReadOnlyList<Location> AdditionalLocations => _badSymbol.Locations;

		internal BadSymbolDiagnostic(Symbol badSymbol, ERRID errid)
			: this(badSymbol, errid, badSymbol)
		{
		}

		internal BadSymbolDiagnostic(Symbol badSymbol, ERRID errid, params object[] additionalArgs)
			: base(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, (int)errid, additionalArgs)
		{
			_badSymbol = badSymbol;
		}

		private void GetAssociatedSymbols(ArrayBuilder<Symbol> builder)
		{
			builder.Add(_badSymbol);
		}

		void IDiagnosticInfoWithSymbols.GetAssociatedSymbols(ArrayBuilder<Symbol> builder)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAssociatedSymbols
			this.GetAssociatedSymbols(builder);
		}
	}
}
