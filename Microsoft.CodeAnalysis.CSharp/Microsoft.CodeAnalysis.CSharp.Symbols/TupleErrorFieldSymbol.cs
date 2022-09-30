using System.Collections.Immutable;

using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class TupleErrorFieldSymbol : SynthesizedFieldSymbolBase
    {
        private readonly TypeWithAnnotations _type;

        private readonly int _tupleElementIndex;

        private readonly ImmutableArray<Location> _locations;

        private readonly DiagnosticInfo _useSiteDiagnosticInfo;

        private readonly TupleErrorFieldSymbol _correspondingDefaultField;

        private readonly bool _isImplicitlyDeclared;

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

        public override bool IsExplicitlyNamedTupleElement
        {
            get
            {
                if (_tupleElementIndex >= 0)
                {
                    return !_isImplicitlyDeclared;
                }
                return false;
            }
        }

        public override FieldSymbol TupleUnderlyingField => null;

        public override FieldSymbol OriginalDefinition => this;

        public override ImmutableArray<Location> Locations => _locations;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get
            {
                if (!_isImplicitlyDeclared)
                {
                    return Symbol.GetDeclaringSyntaxReferenceHelper<CSharpSyntaxNode>(_locations);
                }
                return ImmutableArray<SyntaxReference>.Empty;
            }
        }

        public override bool IsImplicitlyDeclared => _isImplicitlyDeclared;

        public override FieldSymbol CorrespondingTupleField => _correspondingDefaultField;

        internal override bool SuppressDynamicAttribute => true;

        public TupleErrorFieldSymbol(NamedTypeSymbol container, string name, int tupleElementIndex, Location location, TypeWithAnnotations type, DiagnosticInfo useSiteDiagnosticInfo, bool isImplicitlyDeclared, TupleErrorFieldSymbol correspondingDefaultFieldOpt)
            : base(container, name, isPublic: true, isReadOnly: false, isStatic: false)
        {
            _type = type;
            _locations = ((location == null) ? ImmutableArray<Location>.Empty : ImmutableArray.Create(location));
            _useSiteDiagnosticInfo = useSiteDiagnosticInfo;
            _tupleElementIndex = (((object)correspondingDefaultFieldOpt == null) ? (tupleElementIndex << 1) : ((tupleElementIndex << 1) + 1));
            _isImplicitlyDeclared = isImplicitlyDeclared;
            _correspondingDefaultField = correspondingDefaultFieldOpt ?? this;
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _type;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            return new UseSiteInfo<AssemblySymbol>(_useSiteDiagnosticInfo);
        }

        public sealed override int GetHashCode()
        {
            return Hash.Combine(ContainingType.GetHashCode(), _tupleElementIndex.GetHashCode());
        }

        public override bool Equals(Symbol obj, TypeCompareKind compareKind)
        {
            return Equals(obj as TupleErrorFieldSymbol, compareKind);
        }

        public bool Equals(TupleErrorFieldSymbol other, TypeCompareKind compareKind)
        {
            if ((object)other == this)
            {
                return true;
            }
            if ((object)other != null && _tupleElementIndex == other._tupleElementIndex)
            {
                return TypeSymbol.Equals(ContainingType, other.ContainingType, compareKind);
            }
            return false;
        }

        internal override FieldSymbol AsMember(NamedTypeSymbol newOwner)
        {
            if ((object)newOwner == ContainingType)
            {
                return this;
            }
            TupleErrorFieldSymbol correspondingDefaultFieldOpt = null;
            if ((object)_correspondingDefaultField != this)
            {
                correspondingDefaultFieldOpt = (TupleErrorFieldSymbol)_correspondingDefaultField.AsMember(newOwner);
            }
            return new TupleErrorFieldSymbol(newOwner, Name, TupleElementIndex, _locations.IsEmpty ? null : Locations[0], newOwner.TupleElementTypesWithAnnotations[TupleElementIndex], _useSiteDiagnosticInfo, _isImplicitlyDeclared, correspondingDefaultFieldOpt);
        }
    }
}
