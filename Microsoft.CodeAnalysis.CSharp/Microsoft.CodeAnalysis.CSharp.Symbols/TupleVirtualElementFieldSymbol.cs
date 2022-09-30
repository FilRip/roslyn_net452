using System.Collections.Immutable;

using Roslyn.Utilities;

#nullable enable

namespace Microsoft.CodeAnalysis.CSharp.Symbols
{
    internal sealed class TupleVirtualElementFieldSymbol : TupleElementFieldSymbol
    {
        private readonly string _name;

        private readonly bool _cannotUse;

        public override string Name => _name;

        internal override int? TypeLayoutOffset => null;

        public override FieldSymbol OriginalDefinition => this;

        public override bool IsVirtualTupleField => true;

        public TupleVirtualElementFieldSymbol(NamedTypeSymbol container, FieldSymbol underlyingField, string name, int tupleElementIndex, ImmutableArray<Location> locations, bool cannotUse, bool isImplicitlyDeclared, FieldSymbol? correspondingDefaultFieldOpt)
            : base(container, underlyingField, tupleElementIndex, locations, isImplicitlyDeclared, correspondingDefaultFieldOpt)
        {
            _name = name;
            _cannotUse = cannotUse;
        }

        public override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
        {
            if (_cannotUse)
            {
                return new UseSiteInfo<AssemblySymbol>(new CSDiagnosticInfo(ErrorCode.ERR_TupleInferredNamesNotAvailable, _name, new CSharpRequiredLanguageVersion(MessageID.IDS_FeatureInferredTupleNames.RequiredVersion())));
            }
            return base.GetUseSiteInfo();
        }

        internal override TypeWithAnnotations GetFieldType(ConsList<FieldSymbol> fieldsBeingBound)
        {
            return _underlyingField.GetFieldType(fieldsBeingBound);
        }

        public override ImmutableArray<CSharpAttributeData> GetAttributes()
        {
            return _underlyingField.GetAttributes();
        }

        internal override FieldSymbol AsMember(NamedTypeSymbol newOwner)
        {
            NamedTypeSymbol newUnderlyingOwner = GetNewUnderlyingOwner(newOwner);
            FieldSymbol correspondingDefaultFieldOpt = null;
            if ((object)_correspondingDefaultField != this)
            {
                correspondingDefaultFieldOpt = _correspondingDefaultField.OriginalDefinition.AsMember(newOwner);
            }
            return new TupleVirtualElementFieldSymbol(newOwner, _underlyingField.OriginalDefinition.AsMember(newUnderlyingOwner), _name, TupleElementIndex, Locations, _cannotUse, IsImplicitlyDeclared, correspondingDefaultFieldOpt);
        }
    }
}
