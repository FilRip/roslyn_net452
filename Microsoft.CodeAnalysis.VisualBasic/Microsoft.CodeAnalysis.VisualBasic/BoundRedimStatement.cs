using System.Collections.Immutable;
using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRedimStatement : BoundStatement
	{
		private readonly ImmutableArray<BoundRedimClause> _Clauses;

		public ImmutableArray<BoundRedimClause> Clauses => _Clauses;

		public BoundRedimStatement(SyntaxNode syntax, ImmutableArray<BoundRedimClause> clauses, bool hasErrors = false)
			: base(BoundKind.RedimStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(clauses))
		{
			_Clauses = clauses;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRedimStatement(this);
		}

		public BoundRedimStatement Update(ImmutableArray<BoundRedimClause> clauses)
		{
			if (clauses != Clauses)
			{
				BoundRedimStatement boundRedimStatement = new BoundRedimStatement(base.Syntax, clauses, base.HasErrors);
				boundRedimStatement.CopyAttributes(this);
				return boundRedimStatement;
			}
			return this;
		}
	}
}
