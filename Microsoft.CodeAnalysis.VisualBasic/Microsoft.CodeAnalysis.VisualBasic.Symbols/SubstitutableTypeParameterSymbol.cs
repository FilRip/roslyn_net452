using System.Runtime.CompilerServices;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal abstract class SubstitutableTypeParameterSymbol : TypeParameterSymbol
	{
		public sealed override TypeParameterSymbol OriginalDefinition => this;

		public sealed override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public sealed override bool Equals(TypeSymbol other, TypeCompareKind comparison)
		{
			if ((object)other == this)
			{
				return true;
			}
			if ((object)other == null || (comparison & TypeCompareKind.AllIgnoreOptionsForVB) == 0)
			{
				return false;
			}
			if ((object)other.OriginalDefinition != this)
			{
				return false;
			}
			return other.Equals(this, comparison);
		}
	}
}
