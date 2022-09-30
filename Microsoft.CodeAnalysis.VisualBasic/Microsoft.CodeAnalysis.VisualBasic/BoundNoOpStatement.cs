using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundNoOpStatement : BoundStatement
	{
		private readonly NoOpStatementFlavor _Flavor;

		public NoOpStatementFlavor Flavor => _Flavor;

		public BoundNoOpStatement(SyntaxNode syntax)
			: this(syntax, NoOpStatementFlavor.Default)
		{
		}

		public BoundNoOpStatement(SyntaxNode syntax, bool hasErrors)
			: this(syntax, NoOpStatementFlavor.Default, hasErrors)
		{
		}

		public BoundNoOpStatement(SyntaxNode syntax, NoOpStatementFlavor flavor, bool hasErrors)
			: base(BoundKind.NoOpStatement, syntax, hasErrors)
		{
			_Flavor = flavor;
		}

		public BoundNoOpStatement(SyntaxNode syntax, NoOpStatementFlavor flavor)
			: base(BoundKind.NoOpStatement, syntax)
		{
			_Flavor = flavor;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitNoOpStatement(this);
		}

		public BoundNoOpStatement Update(NoOpStatementFlavor flavor)
		{
			if (flavor != Flavor)
			{
				BoundNoOpStatement boundNoOpStatement = new BoundNoOpStatement(base.Syntax, flavor, base.HasErrors);
				boundNoOpStatement.CopyAttributes(this);
				return boundNoOpStatement;
			}
			return this;
		}
	}
}
