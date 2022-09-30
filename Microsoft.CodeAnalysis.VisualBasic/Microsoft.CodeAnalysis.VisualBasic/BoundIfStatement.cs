using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundIfStatement : BoundStatement
	{
		private readonly BoundExpression _Condition;

		private readonly BoundStatement _Consequence;

		private readonly BoundStatement _AlternativeOpt;

		public BoundExpression Condition => _Condition;

		public BoundStatement Consequence => _Consequence;

		public BoundStatement AlternativeOpt => _AlternativeOpt;

		public BoundIfStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement consequence, BoundStatement alternativeOpt, bool hasErrors = false)
			: base(BoundKind.IfStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(condition) || BoundNodeExtensions.NonNullAndHasErrors(consequence) || BoundNodeExtensions.NonNullAndHasErrors(alternativeOpt))
		{
			_Condition = condition;
			_Consequence = consequence;
			_AlternativeOpt = alternativeOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitIfStatement(this);
		}

		public BoundIfStatement Update(BoundExpression condition, BoundStatement consequence, BoundStatement alternativeOpt)
		{
			if (condition != Condition || consequence != Consequence || alternativeOpt != AlternativeOpt)
			{
				BoundIfStatement boundIfStatement = new BoundIfStatement(base.Syntax, condition, consequence, alternativeOpt, base.HasErrors);
				boundIfStatement.CopyAttributes(this);
				return boundIfStatement;
			}
			return this;
		}
	}
}
