using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundStatementList : BoundStatement
	{
		private readonly ImmutableArray<BoundStatement> _Statements;

		public ImmutableArray<BoundStatement> Statements => _Statements;

		public BoundStatementList(SyntaxNode syntax, ImmutableArray<BoundStatement> statements, bool hasErrors = false)
			: base(BoundKind.StatementList, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statements))
		{
			_Statements = statements;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitStatementList(this);
		}

		public BoundStatementList Update(ImmutableArray<BoundStatement> statements)
		{
			if (statements != Statements)
			{
				BoundStatementList boundStatementList = new BoundStatementList(base.Syntax, statements, base.HasErrors);
				boundStatementList.CopyAttributes(this);
				return boundStatementList;
			}
			return this;
		}
	}
}
