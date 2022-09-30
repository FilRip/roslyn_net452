using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSelectStatement : BoundStatement
	{
		private readonly BoundExpressionStatement _ExpressionStatement;

		private readonly BoundRValuePlaceholder _ExprPlaceholderOpt;

		private readonly ImmutableArray<BoundCaseBlock> _CaseBlocks;

		private readonly bool _RecommendSwitchTable;

		private readonly LabelSymbol _ExitLabel;

		public BoundExpressionStatement ExpressionStatement => _ExpressionStatement;

		public BoundRValuePlaceholder ExprPlaceholderOpt => _ExprPlaceholderOpt;

		public ImmutableArray<BoundCaseBlock> CaseBlocks => _CaseBlocks;

		public bool RecommendSwitchTable => _RecommendSwitchTable;

		public LabelSymbol ExitLabel => _ExitLabel;

		public BoundSelectStatement(SyntaxNode syntax, BoundExpressionStatement expressionStatement, BoundRValuePlaceholder exprPlaceholderOpt, ImmutableArray<BoundCaseBlock> caseBlocks, bool recommendSwitchTable, LabelSymbol exitLabel, bool hasErrors = false)
			: base(BoundKind.SelectStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expressionStatement) || BoundNodeExtensions.NonNullAndHasErrors(exprPlaceholderOpt) || BoundNodeExtensions.NonNullAndHasErrors(caseBlocks))
		{
			_ExpressionStatement = expressionStatement;
			_ExprPlaceholderOpt = exprPlaceholderOpt;
			_CaseBlocks = caseBlocks;
			_RecommendSwitchTable = recommendSwitchTable;
			_ExitLabel = exitLabel;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSelectStatement(this);
		}

		public BoundSelectStatement Update(BoundExpressionStatement expressionStatement, BoundRValuePlaceholder exprPlaceholderOpt, ImmutableArray<BoundCaseBlock> caseBlocks, bool recommendSwitchTable, LabelSymbol exitLabel)
		{
			if (expressionStatement != ExpressionStatement || exprPlaceholderOpt != ExprPlaceholderOpt || caseBlocks != CaseBlocks || recommendSwitchTable != RecommendSwitchTable || (object)exitLabel != ExitLabel)
			{
				BoundSelectStatement boundSelectStatement = new BoundSelectStatement(base.Syntax, expressionStatement, exprPlaceholderOpt, caseBlocks, recommendSwitchTable, exitLabel, base.HasErrors);
				boundSelectStatement.CopyAttributes(this);
				return boundSelectStatement;
			}
			return this;
		}
	}
}
