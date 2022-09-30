using System.Collections.Immutable;
using System.Linq;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class PointerTypeSymbol : ErrorTypeSymbol
	{
		private readonly TypeSymbol _pointedAtType;

		private readonly ImmutableArray<CustomModifier> _customModifiers;

		internal override bool MangleName => false;

		internal override DiagnosticInfo ErrorInfo => ErrorFactory.ErrorInfo(ERRID.ERR_UnsupportedType1, string.Empty);

		public PointerTypeSymbol(TypeSymbol pointedAtType, ImmutableArray<CustomModifier> customModifiers)
		{
			_pointedAtType = pointedAtType;
			_customModifiers = customModifiers.NullToEmpty();
		}

		public override int GetHashCode()
		{
			int num = 0;
			PointerTypeSymbol pointerTypeSymbol = this;
			TypeSymbol pointedAtType;
			do
			{
				num++;
				pointedAtType = pointerTypeSymbol._pointedAtType;
				pointerTypeSymbol = pointedAtType as PointerTypeSymbol;
			}
			while ((object)pointerTypeSymbol != null);
			return Hash.Combine(pointedAtType, num);
		}

		public override bool Equals(TypeSymbol obj, TypeCompareKind comparison)
		{
			if ((object)this == obj)
			{
				return true;
			}
			return obj is PointerTypeSymbol pointerTypeSymbol && pointerTypeSymbol._pointedAtType.Equals(_pointedAtType, comparison) && ((comparison & TypeCompareKind.IgnoreCustomModifiersAndArraySizesAndLowerBounds) != 0 || pointerTypeSymbol._customModifiers.SequenceEqual(_customModifiers));
		}
	}
}
