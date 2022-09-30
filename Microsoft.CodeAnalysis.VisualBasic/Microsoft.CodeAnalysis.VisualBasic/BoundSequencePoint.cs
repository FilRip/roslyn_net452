using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSequencePoint : BoundStatement
	{
		private readonly BoundStatement _StatementOpt;

		public BoundStatement StatementOpt => _StatementOpt;

		public BoundSequencePoint(SyntaxNode syntax, BoundStatement statementOpt, bool hasErrors = false)
			: base(BoundKind.SequencePoint, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statementOpt))
		{
			_StatementOpt = statementOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSequencePoint(this);
		}

		public BoundSequencePoint Update(BoundStatement statementOpt)
		{
			if (statementOpt != StatementOpt)
			{
				BoundSequencePoint boundSequencePoint = new BoundSequencePoint(base.Syntax, statementOpt, base.HasErrors);
				boundSequencePoint.CopyAttributes(this);
				return boundSequencePoint;
			}
			return this;
		}
	}
}
