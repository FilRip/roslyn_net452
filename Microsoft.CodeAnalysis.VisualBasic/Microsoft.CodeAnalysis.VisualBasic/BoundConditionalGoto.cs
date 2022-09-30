using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundConditionalGoto : BoundStatement
	{
		private readonly BoundExpression _Condition;

		private readonly bool _JumpIfTrue;

		private readonly LabelSymbol _Label;

		public BoundExpression Condition => _Condition;

		public bool JumpIfTrue => _JumpIfTrue;

		public LabelSymbol Label => _Label;

		public BoundConditionalGoto(SyntaxNode syntax, BoundExpression condition, bool jumpIfTrue, LabelSymbol label, bool hasErrors = false)
			: base(BoundKind.ConditionalGoto, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(condition))
		{
			_Condition = condition;
			_JumpIfTrue = jumpIfTrue;
			_Label = label;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitConditionalGoto(this);
		}

		public BoundConditionalGoto Update(BoundExpression condition, bool jumpIfTrue, LabelSymbol label)
		{
			if (condition != Condition || jumpIfTrue != JumpIfTrue || (object)label != Label)
			{
				BoundConditionalGoto boundConditionalGoto = new BoundConditionalGoto(base.Syntax, condition, jumpIfTrue, label, base.HasErrors);
				boundConditionalGoto.CopyAttributes(this);
				return boundConditionalGoto;
			}
			return this;
		}
	}
}
