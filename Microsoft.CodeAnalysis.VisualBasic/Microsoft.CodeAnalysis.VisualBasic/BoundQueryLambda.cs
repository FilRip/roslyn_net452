using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundQueryLambda : BoundExpression
	{
		private readonly SynthesizedLambdaSymbol _LambdaSymbol;

		private readonly ImmutableArray<RangeVariableSymbol> _RangeVariables;

		private readonly BoundExpression _Expression;

		private readonly bool _ExprIsOperandOfConditionalBranch;

		protected override ImmutableArray<BoundNode> Children => ImmutableArray.Create((BoundNode)Expression);

		public SynthesizedLambdaSymbol LambdaSymbol => _LambdaSymbol;

		public ImmutableArray<RangeVariableSymbol> RangeVariables => _RangeVariables;

		public BoundExpression Expression => _Expression;

		public bool ExprIsOperandOfConditionalBranch => _ExprIsOperandOfConditionalBranch;

		public BoundQueryLambda(SyntaxNode syntax, SynthesizedLambdaSymbol lambdaSymbol, ImmutableArray<RangeVariableSymbol> rangeVariables, BoundExpression expression, bool exprIsOperandOfConditionalBranch, bool hasErrors = false)
			: base(BoundKind.QueryLambda, syntax, null, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(expression))
		{
			_LambdaSymbol = lambdaSymbol;
			_RangeVariables = rangeVariables;
			_Expression = expression;
			_ExprIsOperandOfConditionalBranch = exprIsOperandOfConditionalBranch;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitQueryLambda(this);
		}

		public BoundQueryLambda Update(SynthesizedLambdaSymbol lambdaSymbol, ImmutableArray<RangeVariableSymbol> rangeVariables, BoundExpression expression, bool exprIsOperandOfConditionalBranch)
		{
			if ((object)lambdaSymbol != LambdaSymbol || rangeVariables != RangeVariables || expression != Expression || exprIsOperandOfConditionalBranch != ExprIsOperandOfConditionalBranch)
			{
				BoundQueryLambda boundQueryLambda = new BoundQueryLambda(base.Syntax, lambdaSymbol, rangeVariables, expression, exprIsOperandOfConditionalBranch, base.HasErrors);
				boundQueryLambda.CopyAttributes(this);
				return boundQueryLambda;
			}
			return this;
		}
	}
}
