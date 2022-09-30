using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundGotoStatement : BoundStatement
	{
		private readonly LabelSymbol _Label;

		private readonly BoundLabel _LabelExpressionOpt;

		public LabelSymbol Label => _Label;

		public BoundLabel LabelExpressionOpt => _LabelExpressionOpt;

		public BoundGotoStatement(SyntaxNode syntax, LabelSymbol label, BoundLabel labelExpressionOpt, bool hasErrors = false)
			: base(BoundKind.GotoStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(labelExpressionOpt))
		{
			_Label = label;
			_LabelExpressionOpt = labelExpressionOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitGotoStatement(this);
		}

		public BoundGotoStatement Update(LabelSymbol label, BoundLabel labelExpressionOpt)
		{
			if ((object)label != Label || labelExpressionOpt != LabelExpressionOpt)
			{
				BoundGotoStatement boundGotoStatement = new BoundGotoStatement(base.Syntax, label, labelExpressionOpt, base.HasErrors);
				boundGotoStatement.CopyAttributes(this);
				return boundGotoStatement;
			}
			return this;
		}
	}
}
