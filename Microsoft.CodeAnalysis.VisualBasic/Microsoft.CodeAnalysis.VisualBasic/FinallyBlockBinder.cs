using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class FinallyBlockBinder : ExitableStatementBinder
	{
		public FinallyBlockBinder(Binder enclosing)
			: base(enclosing, SyntaxKind.None, SyntaxKind.None)
		{
		}

		public override LabelSymbol GetExitLabel(SyntaxKind exitSyntaxKind)
		{
			if (exitSyntaxKind == SyntaxKind.ExitTryStatement)
			{
				return base.ContainingBinder.ContainingBinder.GetExitLabel(exitSyntaxKind);
			}
			return base.GetExitLabel(exitSyntaxKind);
		}
	}
}
