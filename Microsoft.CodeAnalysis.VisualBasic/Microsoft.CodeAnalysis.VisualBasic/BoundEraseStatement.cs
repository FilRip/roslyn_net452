using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundEraseStatement : BoundStatement
	{
		private readonly ImmutableArray<BoundAssignmentOperator> _Clauses;

		protected override ImmutableArray<BoundNode> Children => StaticCast<BoundNode>.From(Clauses);

		public ImmutableArray<BoundAssignmentOperator> Clauses => _Clauses;

		public BoundEraseStatement(SyntaxNode syntax, ImmutableArray<BoundAssignmentOperator> clauses, bool hasErrors = false)
			: base(BoundKind.EraseStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(clauses))
		{
			_Clauses = clauses;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitEraseStatement(this);
		}

		public BoundEraseStatement Update(ImmutableArray<BoundAssignmentOperator> clauses)
		{
			if (clauses != Clauses)
			{
				BoundEraseStatement boundEraseStatement = new BoundEraseStatement(base.Syntax, clauses, base.HasErrors);
				boundEraseStatement.CopyAttributes(this);
				return boundEraseStatement;
			}
			return this;
		}
	}
}
