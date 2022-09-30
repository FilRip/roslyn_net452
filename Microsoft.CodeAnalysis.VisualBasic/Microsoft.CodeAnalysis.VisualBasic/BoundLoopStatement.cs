using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundLoopStatement : BoundStatement
	{
		private readonly LabelSymbol _ContinueLabel;

		private readonly LabelSymbol _ExitLabel;

		public LabelSymbol ContinueLabel => _ContinueLabel;

		public LabelSymbol ExitLabel => _ExitLabel;

		protected BoundLoopStatement(BoundKind kind, SyntaxNode syntax, LabelSymbol continueLabel, LabelSymbol exitLabel, bool hasErrors)
			: base(kind, syntax, hasErrors)
		{
			_ContinueLabel = continueLabel;
			_ExitLabel = exitLabel;
		}

		protected BoundLoopStatement(BoundKind kind, SyntaxNode syntax, LabelSymbol continueLabel, LabelSymbol exitLabel)
			: base(kind, syntax)
		{
			_ContinueLabel = continueLabel;
			_ExitLabel = exitLabel;
		}
	}
}
