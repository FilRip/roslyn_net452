using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundResumeStatement : BoundStatement
	{
		private readonly ResumeStatementKind _ResumeKind;

		private readonly LabelSymbol _LabelOpt;

		private readonly BoundExpression _LabelExpressionOpt;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)LabelExpressionOpt);

		public ResumeStatementKind ResumeKind => _ResumeKind;

		public LabelSymbol LabelOpt => _LabelOpt;

		public BoundExpression LabelExpressionOpt => _LabelExpressionOpt;

		public BoundResumeStatement(SyntaxNode syntax, bool isNext = false)
			: this(syntax, isNext ? ResumeStatementKind.Next : ResumeStatementKind.Plain, null, null)
		{
		}

		public BoundResumeStatement(SyntaxNode syntax, LabelSymbol label, BoundExpression labelExpressionOpt, bool hasErrors = false)
			: this(syntax, ResumeStatementKind.Label, label, labelExpressionOpt, hasErrors)
		{
		}

		public BoundResumeStatement(SyntaxNode syntax, ResumeStatementKind resumeKind, LabelSymbol labelOpt, BoundExpression labelExpressionOpt, bool hasErrors = false)
			: base(BoundKind.ResumeStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(labelExpressionOpt))
		{
			_ResumeKind = resumeKind;
			_LabelOpt = labelOpt;
			_LabelExpressionOpt = labelExpressionOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitResumeStatement(this);
		}

		public BoundResumeStatement Update(ResumeStatementKind resumeKind, LabelSymbol labelOpt, BoundExpression labelExpressionOpt)
		{
			if (resumeKind != ResumeKind || (object)labelOpt != LabelOpt || labelExpressionOpt != LabelExpressionOpt)
			{
				BoundResumeStatement boundResumeStatement = new BoundResumeStatement(base.Syntax, resumeKind, labelOpt, labelExpressionOpt, base.HasErrors);
				boundResumeStatement.CopyAttributes(this);
				return boundResumeStatement;
			}
			return this;
		}
	}
}
