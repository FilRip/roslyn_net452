namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal sealed class TupleVirtualElementFieldSymbol : TupleElementFieldSymbol
	{
		private readonly string _name;

		private readonly bool _cannotUse;

		public override string Name => _name;

		internal override int? TypeLayoutOffset => null;

		public override Symbol AssociatedSymbol => null;

		public override bool IsVirtualTupleField => true;

		public TupleVirtualElementFieldSymbol(TupleTypeSymbol container, FieldSymbol underlyingField, string name, bool cannotUse, int tupleElementOrdinal, Location location, bool isImplicitlyDeclared, TupleElementFieldSymbol correspondingDefaultFieldOpt)
			: base(container, underlyingField, tupleElementOrdinal, location, isImplicitlyDeclared, correspondingDefaultFieldOpt)
		{
			_name = name;
			_cannotUse = cannotUse;
		}

		internal override UseSiteInfo<AssemblySymbol> GetUseSiteInfo()
		{
			if (_cannotUse)
			{
				UseSiteInfo<AssemblySymbol> result = new UseSiteInfo<AssemblySymbol>(ErrorFactory.ErrorInfo(ERRID.ERR_TupleInferredNamesNotAvailable, _name, new VisualBasicRequiredLanguageVersion(LanguageVersion.VisualBasic15_3)));
				return result;
			}
			return base.GetUseSiteInfo();
		}
	}
}
