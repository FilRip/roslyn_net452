using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundForToStatement : BoundForStatement
	{
		private readonly BoundExpression _InitialValue;

		private readonly BoundExpression _LimitValue;

		private readonly BoundExpression _StepValue;

		private readonly bool _Checked;

		private readonly BoundForToUserDefinedOperators _OperatorsOpt;

		public BoundExpression InitialValue => _InitialValue;

		public BoundExpression LimitValue => _LimitValue;

		public BoundExpression StepValue => _StepValue;

		public bool Checked => _Checked;

		public BoundForToUserDefinedOperators OperatorsOpt => _OperatorsOpt;

		public BoundForToStatement(SyntaxNode syntax, BoundExpression initialValue, BoundExpression limitValue, BoundExpression stepValue, bool @checked, BoundForToUserDefinedOperators operatorsOpt, LocalSymbol declaredOrInferredLocalOpt, BoundExpression controlVariable, BoundStatement body, ImmutableArray<BoundExpression> nextVariablesOpt, LabelSymbol continueLabel, LabelSymbol exitLabel, bool hasErrors = false)
			: base(BoundKind.ForToStatement, syntax, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(initialValue) || BoundNodeExtensions.NonNullAndHasErrors(limitValue) || BoundNodeExtensions.NonNullAndHasErrors(stepValue) || BoundNodeExtensions.NonNullAndHasErrors(operatorsOpt) || BoundNodeExtensions.NonNullAndHasErrors(controlVariable) || BoundNodeExtensions.NonNullAndHasErrors(body) || BoundNodeExtensions.NonNullAndHasErrors(nextVariablesOpt))
		{
			_InitialValue = initialValue;
			_LimitValue = limitValue;
			_StepValue = stepValue;
			_Checked = @checked;
			_OperatorsOpt = operatorsOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitForToStatement(this);
		}

		public BoundForToStatement Update(BoundExpression initialValue, BoundExpression limitValue, BoundExpression stepValue, bool @checked, BoundForToUserDefinedOperators operatorsOpt, LocalSymbol declaredOrInferredLocalOpt, BoundExpression controlVariable, BoundStatement body, ImmutableArray<BoundExpression> nextVariablesOpt, LabelSymbol continueLabel, LabelSymbol exitLabel)
		{
			if (initialValue != InitialValue || limitValue != LimitValue || stepValue != StepValue || @checked != Checked || operatorsOpt != OperatorsOpt || (object)declaredOrInferredLocalOpt != base.DeclaredOrInferredLocalOpt || controlVariable != base.ControlVariable || body != base.Body || nextVariablesOpt != base.NextVariablesOpt || (object)continueLabel != base.ContinueLabel || (object)exitLabel != base.ExitLabel)
			{
				BoundForToStatement boundForToStatement = new BoundForToStatement(base.Syntax, initialValue, limitValue, stepValue, @checked, operatorsOpt, declaredOrInferredLocalOpt, controlVariable, body, nextVariablesOpt, continueLabel, exitLabel, base.HasErrors);
				boundForToStatement.CopyAttributes(this);
				return boundForToStatement;
			}
			return this;
		}
	}
}
