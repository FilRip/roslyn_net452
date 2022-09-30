using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundWhileStatement : BoundLoopStatement
	{
		private readonly BoundExpression _Condition;

		private readonly BoundStatement _Body;

		public BoundExpression Condition => _Condition;

		public BoundStatement Body => _Body;

		public BoundWhileStatement(SyntaxNode syntax, BoundExpression condition, BoundStatement body, LabelSymbol continueLabel, LabelSymbol exitLabel, bool hasErrors = false)
			: base(BoundKind.WhileStatement, syntax, continueLabel, exitLabel, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(condition) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_Condition = condition;
			_Body = body;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitWhileStatement(this);
		}

		public BoundWhileStatement Update(BoundExpression condition, BoundStatement body, LabelSymbol continueLabel, LabelSymbol exitLabel)
		{
			if (condition != Condition || body != Body || (object)continueLabel != base.ContinueLabel || (object)exitLabel != base.ExitLabel)
			{
				BoundWhileStatement boundWhileStatement = new BoundWhileStatement(base.Syntax, condition, body, continueLabel, exitLabel, base.HasErrors);
				boundWhileStatement.CopyAttributes(this);
				return boundWhileStatement;
			}
			return this;
		}
	}
}
