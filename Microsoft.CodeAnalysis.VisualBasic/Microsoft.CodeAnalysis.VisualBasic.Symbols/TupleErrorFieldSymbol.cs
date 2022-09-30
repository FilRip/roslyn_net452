using System.Collections.Immutable;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class TupleErrorFieldSymbol : SynthesizedFieldSymbol
	{
		private readonly int _tupleElementIndex;

		private readonly ImmutableArray<Location> _locations;

		private readonly DiagnosticInfo _useSiteDiagnosticInfo;

		private readonly TupleErrorFieldSymbol _correspondingDefaultField;

		private readonly bool _isImplicitlyDeclared;

		public override bool IsTupleField => true;

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

		public override FieldSymbol TupleUnderlyingField => null;

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

		public override FieldSymbol CorrespondingTupleField => _correspondingDefaultField;

		public override TypeSymbol Type => _type;

		public TupleErrorFieldSymbol(NamedTypeSymbol container, string name, int tupleElementIndex, Location location, TypeSymbol type, DiagnosticInfo useSiteDiagnosticInfo, bool isImplicitlyDeclared, TupleErrorFieldSymbol correspondingDefaultFieldOpt)
			: base(container, container, type, name, Accessibility.Public)
		{
			_locations = (((object)location == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(location));
			_useSiteDiagnosticInfo = useSiteDiagnosticInfo;
			_tupleElementIndex = (((object)correspondingDefaultFieldOpt == null) ? (tupleElementIndex << 1) : ((tupleElementIndex << 1) + 1));
			_isImplicitlyDeclared = isImplicitlyDeclared;
			_correspondingDefaultField = correspondingDefaultFieldOpt ?? this;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			return new UseSiteInfo<AssemblySymbol>(_useSiteDiagnosticInfo);
		}

		public override int GetHashCode()
		{
			return Hash.Combine(ContainingType.GetHashCode(), _tupleElementIndex.GetHashCode());
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as TupleErrorFieldSymbol);
		}

		public bool Equals(TupleErrorFieldSymbol other)
		{
			if ((object)other != this)
			{
				if ((object)other != null && _tupleElementIndex == other._tupleElementIndex)
				{
					return TypeSymbol.Equals(ContainingType, other.ContainingType, TypeCompareKind.ConsiderEverything);
				}
				return false;
			}
			return true;
		}
	}
}
