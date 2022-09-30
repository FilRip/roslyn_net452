using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundGlobalStatementInitializer : BoundInitializer
	{
		private readonly BoundStatement _Statement;

		public BoundStatement Statement => _Statement;

		public BoundGlobalStatementInitializer(SyntaxNode syntax, BoundStatement statement, bool hasErrors = false)
			: base(BoundKind.GlobalStatementInitializer, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statement))
		{
			_Statement = statement;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitGlobalStatementInitializer(this);
		}

		public BoundGlobalStatementInitializer Update(BoundStatement statement)
		{
			if (statement != Statement)
			{
				BoundGlobalStatementInitializer boundGlobalStatementInitializer = new BoundGlobalStatementInitializer(base.Syntax, statement, base.HasErrors);
				boundGlobalStatementInitializer.CopyAttributes(this);
				return boundGlobalStatementInitializer;
			}
			return this;
		}
	}
}
