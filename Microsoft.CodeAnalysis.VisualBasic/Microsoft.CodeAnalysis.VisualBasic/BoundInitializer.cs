using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal class BoundInitializer : BoundStatement
	{
		protected BoundInitializer(BoundKind kind, SyntaxNode syntax, bool hasErrors)
			: base(kind, syntax, hasErrors)
		{
		}

		protected BoundInitializer(BoundKind kind, SyntaxNode syntax)
			: base(kind, syntax)
		{
		}

		public BoundInitializer(SyntaxNode syntax, bool hasErrors)
			: base(BoundKind.Initializer, syntax, hasErrors)
		{
		}

		public BoundInitializer(SyntaxNode syntax)
			: base(BoundKind.Initializer, syntax)
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitInitializer(this);
		}
	}
}
