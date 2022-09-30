using System.Collections.Immutable;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class TupleElementFieldSymbol : TupleFieldSymbol
	{
		private readonly ImmutableArray<Location> _locations;

		private readonly bool _isImplicitlyDeclared;

		private readonly TupleElementFieldSymbol _correspondingDefaultField;

		public override ImmutableArray<Location> Locations => _locations;

		public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
		{
			get
			{
				if (!_isImplicitlyDeclared)
				{
					return Symbol.GetDeclaringSyntaxReferenceHelper<VisualBasicSyntaxNode>(_locations);
				}
				return ImmutableArray<SyntaxReference>.Empty;
			}
		}

		public override bool IsImplicitlyDeclared => _isImplicitlyDeclared;

		internal override int? TypeLayoutOffset
		{
			get
			{
				if ((object)_underlyingField.ContainingType != _containingTuple.TupleUnderlyingType)
				{
					return null;
				}
				return base.TypeLayoutOffset;
			}
		}

		public override Symbol AssociatedSymbol
		{
			get
			{
				if ((object)_underlyingField.ContainingType != _containingTuple.TupleUnderlyingType)
				{
					return null;
				}
				return base.AssociatedSymbol;
			}
		}

		public override FieldSymbol CorrespondingTupleField => _correspondingDefaultField;

		public TupleElementFieldSymbol(TupleTypeSymbol container, FieldSymbol underlyingField, int tupleElementIndex, Location location, bool isImplicitlyDeclared, TupleElementFieldSymbol correspondingDefaultFieldOpt)
			: base(container, underlyingField, ((object)correspondingDefaultFieldOpt == null) ? (tupleElementIndex << 1) : ((tupleElementIndex << 1) + 1))
		{
			_locations = (((object)location == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(location));
			_isImplicitlyDeclared = isImplicitlyDeclared;
			_correspondingDefaultField = correspondingDefaultFieldOpt ?? this;
		}
	}
}
