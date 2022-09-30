using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundOnErrorStatement : BoundStatement
	{
		private readonly OnErrorStatementKind _OnErrorKind;

		private readonly LabelSymbol _LabelOpt;

		private readonly BoundExpression _LabelExpressionOpt;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)LabelExpressionOpt);

		public OnErrorStatementKind OnErrorKind => _OnErrorKind;

		public LabelSymbol LabelOpt => _LabelOpt;

		public BoundExpression LabelExpressionOpt => _LabelExpressionOpt;

		public BoundOnErrorStatement(SyntaxNode syntax, LabelSymbol label, BoundExpression labelExpressionOpt, bool hasErrors = false)
			: this(syntax, OnErrorStatementKind.GoToLabel, label, labelExpressionOpt, hasErrors)
		{
		}

		public BoundOnErrorStatement(SyntaxNode syntax, OnErrorStatementKind onErrorKind, LabelSymbol labelOpt, BoundExpression labelExpressionOpt, bool hasErrors = false)
			: base(BoundKind.OnErrorStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(labelExpressionOpt))
		{
			_OnErrorKind = onErrorKind;
			_LabelOpt = labelOpt;
			_LabelExpressionOpt = labelExpressionOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitOnErrorStatement(this);
		}

		public BoundOnErrorStatement Update(OnErrorStatementKind onErrorKind, LabelSymbol labelOpt, BoundExpression labelExpressionOpt)
		{
			if (onErrorKind != OnErrorKind || (object)labelOpt != LabelOpt || labelExpressionOpt != LabelExpressionOpt)
			{
				BoundOnErrorStatement boundOnErrorStatement = new BoundOnErrorStatement(base.Syntax, onErrorKind, labelOpt, labelExpressionOpt, base.HasErrors);
				boundOnErrorStatement.CopyAttributes(this);
				return boundOnErrorStatement;
			}
			return this;
		}
	}
}
