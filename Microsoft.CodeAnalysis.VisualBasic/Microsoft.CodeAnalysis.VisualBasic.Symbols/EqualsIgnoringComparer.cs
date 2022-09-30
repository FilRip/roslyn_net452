using System.Collections.Generic;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class EqualsIgnoringComparer : EqualityComparer<TypeSymbol>
	{
		private readonly TypeCompareKind _comparison;

		public static EqualsIgnoringComparer InstanceIgnoringTupleNames { get; } = new EqualsIgnoringComparer(TypeCompareKind.IgnoreTupleNames);


		public static EqualsIgnoringComparer InstanceCLRSignatureCompare { get; } = new EqualsIgnoringComparer(TypeCompareKind.IgnoreTupleNames);


		public EqualsIgnoringComparer(TypeCompareKind comparison)
		{
			_comparison = comparison;
		}

		public override bool Equals(TypeSymbol type1, TypeSymbol type2)
		{
			if ((object)type1 != null)
			{
				return TypeSymbolExtensions.IsSameType(type1, type2, _comparison);
			}
			return (object)type2 == null;
		}

		public override int GetHashCode(TypeSymbol obj)
		{
			return obj?.GetHashCode() ?? 0;
		}
	}
}
