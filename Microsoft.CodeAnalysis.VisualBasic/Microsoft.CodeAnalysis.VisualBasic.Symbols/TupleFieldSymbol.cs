using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class TupleFieldSymbol : WrappedFieldSymbol
	{
		protected readonly TupleTypeSymbol _containingTuple;

		private readonly int _tupleElementIndex;

		public override bool IsTupleField => true;

		public override FieldSymbol TupleUnderlyingField => _underlyingField;

		public override int TupleElementIndex
		{
			get
			{
				if (_tupleElementIndex < 0)
				{
					return -1;
				}
				return _tupleElementIndex >> 1;
			}
		}

		public override bool IsDefaultTupleElement => (_tupleElementIndex & -2147483647) == 0;

		public override Symbol AssociatedSymbol => _containingTuple.GetTupleMemberSymbolForUnderlyingMember(_underlyingField.AssociatedSymbol);

		public override Symbol ContainingSymbol => _containingTuple;

		public override ImmutableArray<CustomModifier> CustomModifiers => _underlyingField.CustomModifiers;

		public override TypeSymbol Type => _underlyingField.Type;

		public TupleFieldSymbol(TupleTypeSymbol container, FieldSymbol underlyingField, int tupleElementIndex)
			: base(underlyingField)
		{
			_containingTuple = container;
			_tupleElementIndex = tupleElementIndex;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingField.GetAttributes();
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = base.GetUseSiteInfo();
			MergeUseSiteInfo(useSiteInfo, _underlyingField.GetUseSiteInfo());
			return useSiteInfo;
		}

		public override int GetHashCode()
		{
			return Hash.Combine(_containingTuple.GetHashCode(), _tupleElementIndex.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TupleFieldSymbol);
		}

		public bool Equals(TupleFieldSymbol other)
		{
			if ((object)other != null && _tupleElementIndex == other._tupleElementIndex)
			{
				return TypeSymbol.Equals(_containingTuple, other._containingTuple, TypeCompareKind.ConsiderEverything);
			}
			return false;
		}
	}
}
