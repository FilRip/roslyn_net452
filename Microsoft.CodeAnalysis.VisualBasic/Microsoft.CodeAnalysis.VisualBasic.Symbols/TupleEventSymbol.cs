using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class TupleEventSymbol : WrappedEventSymbol
	{
		private readonly TupleTypeSymbol _containingType;

		public override bool IsTupleEvent => true;

		public override EventSymbol TupleUnderlyingEvent => _underlyingEvent;

		public override Symbol ContainingSymbol => _containingType;

		public override TypeSymbol Type => _underlyingEvent.Type;

		public override MethodSymbol AddMethod => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingEvent.AddMethod);

		public override MethodSymbol RemoveMethod => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingEvent.RemoveMethod);

		internal override FieldSymbol AssociatedField => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingEvent.AssociatedField);

		public override MethodSymbol RaiseMethod => _containingType.GetTupleMemberSymbolForUnderlyingMember(_underlyingEvent.RaiseMethod);

		internal override bool IsExplicitInterfaceImplementation => _underlyingEvent.IsExplicitInterfaceImplementation;

		public override ImmutableArray<EventSymbol> ExplicitInterfaceImplementations => _underlyingEvent.ExplicitInterfaceImplementations;

		public TupleEventSymbol(TupleTypeSymbol container, EventSymbol underlyingEvent)
			: base(underlyingEvent)
		{
			_containingType = container;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			UseSiteInfo<AssemblySymbol> useSiteInfo = base.GetUseSiteInfo();
			MergeUseSiteInfo(useSiteInfo, _underlyingEvent.GetUseSiteInfo());
			return useSiteInfo;
		}

		public override int GetHashCode()
		{
			return _underlyingEvent.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TupleEventSymbol);
		}

		public bool Equals(TupleEventSymbol other)
		{
			if ((object)other != this)
			{
				if ((object)other != null && TypeSymbol.Equals(_containingType, other._containingType, TypeCompareKind.ConsiderEverything))
				{
					return _underlyingEvent == other._underlyingEvent;
				}
				return false;
			}
			return true;
		}

		public override ImmutableArray<VisualBasicAttributeData> GetAttributes()
		{
			return _underlyingEvent.GetAttributes();
		}
	}
}
