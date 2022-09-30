using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundReturnStatement : BoundStatement
	{
		private readonly BoundExpression _ExpressionOpt;

		private readonly LocalSymbol _FunctionLocalOpt;

		private readonly LabelSymbol _ExitLabelOpt;

		public BoundExpression ExpressionOpt => _ExpressionOpt;

		public LocalSymbol FunctionLocalOpt => _FunctionLocalOpt;

		public LabelSymbol ExitLabelOpt => _ExitLabelOpt;

		internal bool IsEndOfMethodReturn()
		{
			return (object)ExitLabelOpt == null;
		}

		public BoundReturnStatement(SyntaxNode syntax, BoundExpression expressionOpt, LocalSymbol functionLocalOpt, LabelSymbol exitLabelOpt, bool hasErrors = false)
			: base(BoundKind.ReturnStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expressionOpt))
		{
			_ExpressionOpt = expressionOpt;
			_FunctionLocalOpt = functionLocalOpt;
			_ExitLabelOpt = exitLabelOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitReturnStatement(this);
		}

		public BoundReturnStatement Update(BoundExpression expressionOpt, LocalSymbol functionLocalOpt, LabelSymbol exitLabelOpt)
		{
			if (expressionOpt != ExpressionOpt || (object)functionLocalOpt != FunctionLocalOpt || (object)exitLabelOpt != ExitLabelOpt)
			{
				BoundReturnStatement boundReturnStatement = new BoundReturnStatement(base.Syntax, expressionOpt, functionLocalOpt, exitLabelOpt, base.HasErrors);
				boundReturnStatement.CopyAttributes(this);
				return boundReturnStatement;
			}
			return this;
		}
	}
}
