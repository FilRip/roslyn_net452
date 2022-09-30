using System.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSequencePointWithSpan : BoundStatement
	{
		private readonly BoundStatement _StatementOpt;

		private readonly TextSpan _Span;

		public BoundStatement StatementOpt => _StatementOpt;

		public TextSpan Span => _Span;

		public BoundSequencePointWithSpan(SyntaxNode syntax, BoundStatement statementOpt, TextSpan span, bool hasErrors = false)
			: base(BoundKind.SequencePointWithSpan, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(statementOpt))
		{
			_StatementOpt = statementOpt;
			_Span = span;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSequencePointWithSpan(this);
		}

		public BoundSequencePointWithSpan Update(BoundStatement statementOpt, TextSpan span)
		{
			if (statementOpt != StatementOpt || span != Span)
			{
				BoundSequencePointWithSpan boundSequencePointWithSpan = new BoundSequencePointWithSpan(base.Syntax, statementOpt, span, base.HasErrors);
				boundSequencePointWithSpan.CopyAttributes(this);
				return boundSequencePointWithSpan;
			}
			return this;
		}
	}
}
