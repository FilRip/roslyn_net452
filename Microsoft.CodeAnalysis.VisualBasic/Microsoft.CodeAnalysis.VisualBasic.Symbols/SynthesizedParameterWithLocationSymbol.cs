using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class SynthesizedParameterWithLocationSymbol : SynthesizedParameterSymbol
	{
		private readonly ImmutableArray<Location> _locations;

		public override ImmutableArray<Location> Locations => _locations;

		public SynthesizedParameterWithLocationSymbol(MethodSymbol container, TypeSymbol type, int ordinal, bool isByRef, string name, Location location)
			: base(container, type, ordinal, isByRef, name)
		{
			_locations = ImmutableArray.Create(location);
		}
	}
}
