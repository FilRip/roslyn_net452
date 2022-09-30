using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundStopStatement : BoundStatement
	{
		public BoundStopStatement(SyntaxNode syntax, bool hasErrors)
			: base(BoundKind.StopStatement, syntax, hasErrors)
		{
		}

		public BoundStopStatement(SyntaxNode syntax)
			: base(BoundKind.StopStatement, syntax)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitStopStatement(this);
		}
	}
}
