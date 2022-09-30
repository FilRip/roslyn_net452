using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundEndStatement : BoundStatement
	{
		public BoundEndStatement(SyntaxNode syntax, bool hasErrors)
			: base(BoundKind.EndStatement, syntax, hasErrors)
		{
		}

		public BoundEndStatement(SyntaxNode syntax)
			: base(BoundKind.EndStatement, syntax)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitEndStatement(this);
		}
	}
}
