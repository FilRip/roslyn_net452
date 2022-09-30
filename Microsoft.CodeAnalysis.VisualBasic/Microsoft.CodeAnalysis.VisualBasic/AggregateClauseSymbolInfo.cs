namespace Microsoft.CodeAnalysis.VisualBasic
{
	public struct AggregateClauseSymbolInfo
	{
		public SymbolInfo Select1 { get; }

		public SymbolInfo Select2 { get; }

		internal AggregateClauseSymbolInfo(SymbolInfo select1)
		{
			this = default(AggregateClauseSymbolInfo);
			Select1 = select1;
			Select2 = SymbolInfo.None;
		}

		internal AggregateClauseSymbolInfo(SymbolInfo select1, SymbolInfo select2)
		{
			this = default(AggregateClauseSymbolInfo);
			Select1 = select1;
			Select2 = select2;
		}
	}
}
