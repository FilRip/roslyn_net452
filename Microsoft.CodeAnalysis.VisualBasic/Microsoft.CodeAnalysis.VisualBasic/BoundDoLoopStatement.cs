using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundDoLoopStatement : BoundLoopStatement
	{
		private readonly BoundExpression _TopConditionOpt;

		private readonly BoundExpression _BottomConditionOpt;

		private readonly bool _TopConditionIsUntil;

		private readonly bool _BottomConditionIsUntil;

		private readonly BoundStatement _Body;

		public bool ConditionIsTop => TopConditionOpt != null;

		public bool ConditionIsUntil
		{
			get
			{
				if (TopConditionOpt != null)
				{
					return TopConditionIsUntil;
				}
				return BottomConditionIsUntil;
			}
		}

		public BoundExpression ConditionOpt
		{
			get
			{
				if (TopConditionOpt != null)
				{
					return TopConditionOpt;
				}
				return BottomConditionOpt;
			}
		}

		public BoundExpression TopConditionOpt => _TopConditionOpt;

		public BoundExpression BottomConditionOpt => _BottomConditionOpt;

		public bool TopConditionIsUntil => _TopConditionIsUntil;

		public bool BottomConditionIsUntil => _BottomConditionIsUntil;

		public BoundStatement Body => _Body;

		public BoundDoLoopStatement(SyntaxNode syntax, BoundExpression topConditionOpt, BoundExpression bottomConditionOpt, bool topConditionIsUntil, bool bottomConditionIsUntil, BoundStatement body, LabelSymbol continueLabel, LabelSymbol exitLabel, bool hasErrors = false)
			: base(BoundKind.DoLoopStatement, syntax, continueLabel, exitLabel, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(topConditionOpt) || BoundNodeExtensions.NonNullAndHasErrors(bottomConditionOpt) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_TopConditionOpt = topConditionOpt;
			_BottomConditionOpt = bottomConditionOpt;
			_TopConditionIsUntil = topConditionIsUntil;
			_BottomConditionIsUntil = bottomConditionIsUntil;
			_Body = body;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitDoLoopStatement(this);
		}

		public BoundDoLoopStatement Update(BoundExpression topConditionOpt, BoundExpression bottomConditionOpt, bool topConditionIsUntil, bool bottomConditionIsUntil, BoundStatement body, LabelSymbol continueLabel, LabelSymbol exitLabel)
		{
			if (topConditionOpt != TopConditionOpt || bottomConditionOpt != BottomConditionOpt || topConditionIsUntil != TopConditionIsUntil || bottomConditionIsUntil != BottomConditionIsUntil || body != Body || (object)continueLabel != base.ContinueLabel || (object)exitLabel != base.ExitLabel)
			{
				BoundDoLoopStatement boundDoLoopStatement = new BoundDoLoopStatement(base.Syntax, topConditionOpt, bottomConditionOpt, topConditionIsUntil, bottomConditionIsUntil, body, continueLabel, exitLabel, base.HasErrors);
				boundDoLoopStatement.CopyAttributes(this);
				return boundDoLoopStatement;
			}
			return this;
		}
	}
}
