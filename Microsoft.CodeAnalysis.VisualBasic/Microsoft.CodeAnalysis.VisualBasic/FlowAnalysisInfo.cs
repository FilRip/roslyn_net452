namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal struct FlowAnalysisInfo
	{
		public readonly VisualBasicCompilation Compilation;

		public readonly Symbol Symbol;

		public readonly BoundNode Node;

		public FlowAnalysisInfo(VisualBasicCompilation _compilation, Symbol _symbol, BoundNode _node)
		{
			this = default(FlowAnalysisInfo);
			Compilation = _compilation;
			Symbol = _symbol;
			Node = _node;
		}
	}
}
