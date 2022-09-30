using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.PooledObjects;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class AmbiguousSymbolDiagnostic : DiagnosticInfo, IDiagnosticInfoWithSymbols
	{
		private ImmutableArray<Symbol> _symbols;

		public ImmutableArray<Symbol> AmbiguousSymbols => _symbols;

		public override IReadOnlyList<Location> AdditionalLocations
		{
			get
			{
				ArrayBuilder<Location> instance = ArrayBuilder<Location>.GetInstance();
				ImmutableArray<Symbol>.Enumerator enumerator = _symbols.GetEnumerator();
				while (enumerator.MoveNext())
				{
					ImmutableArray<Location>.Enumerator enumerator2 = enumerator.Current.Locations.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						Location current = enumerator2.Current;
						instance.Add(current);
					}
				}
				return instance.ToImmutableAndFree();
			}
		}

		internal AmbiguousSymbolDiagnostic(ERRID errid, ImmutableArray<Symbol> symbols, params object[] args)
			: base(Microsoft.CodeAnalysis.VisualBasic.MessageProvider.Instance, (int)errid, args)
		{
			_symbols = symbols;
		}

		private void GetAssociatedSymbols(ArrayBuilder<Symbol> builder)
		{
			builder.AddRange(_symbols);
		}

		void IDiagnosticInfoWithSymbols.GetAssociatedSymbols(ArrayBuilder<Symbol> builder)
		{
			//ILSpy generated this explicit interface implementation from .override directive in GetAssociatedSymbols
			this.GetAssociatedSymbols(builder);
		}
	}
}
