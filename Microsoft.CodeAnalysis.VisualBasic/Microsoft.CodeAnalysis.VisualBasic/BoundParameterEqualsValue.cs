using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundParameterEqualsValue : BoundNode
	{
		private readonly ParameterSymbol _Parameter;

		private readonly BoundExpression _Value;

		public ParameterSymbol Parameter => _Parameter;

		public BoundExpression Value => _Value;

		public BoundParameterEqualsValue(SyntaxNode syntax, ParameterSymbol parameter, BoundExpression value, bool hasErrors = false)
			: base(BoundKind.ParameterEqualsValue, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(value))
		{
			_Parameter = parameter;
			_Value = value;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitParameterEqualsValue(this);
		}

		public BoundParameterEqualsValue Update(ParameterSymbol parameter, BoundExpression value)
		{
			if ((object)parameter != Parameter || value != Value)
			{
				BoundParameterEqualsValue boundParameterEqualsValue = new BoundParameterEqualsValue(base.Syntax, parameter, value, base.HasErrors);
				boundParameterEqualsValue.CopyAttributes(this);
				return boundParameterEqualsValue;
			}
			return this;
		}
	}
}
