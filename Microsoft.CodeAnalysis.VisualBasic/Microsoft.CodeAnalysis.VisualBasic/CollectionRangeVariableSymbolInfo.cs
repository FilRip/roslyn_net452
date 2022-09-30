namespace Microsoft.CodeAnalysis.VisualBasic
{
	public struct CollectionRangeVariableSymbolInfo
	{
		internal static readonly CollectionRangeVariableSymbolInfo None = new CollectionRangeVariableSymbolInfo(SymbolInfo.None, SymbolInfo.None, SymbolInfo.None);

		public SymbolInfo ToQueryableCollectionConversion { get; }

		public SymbolInfo AsClauseConversion { get; }

		public SymbolInfo SelectMany { get; }

		internal CollectionRangeVariableSymbolInfo(SymbolInfo toQueryableCollectionConversion, SymbolInfo asClauseConversion, SymbolInfo selectMany)
		{
			this = default(CollectionRangeVariableSymbolInfo);
			ToQueryableCollectionConversion = toQueryableCollectionConversion;
			AsClauseConversion = asClauseConversion;
			SelectMany = selectMany;
		}
	}
}
