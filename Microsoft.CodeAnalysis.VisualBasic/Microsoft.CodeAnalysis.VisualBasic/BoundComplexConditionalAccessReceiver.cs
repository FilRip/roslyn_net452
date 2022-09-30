using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundComplexConditionalAccessReceiver : BoundExpression
	{
		private readonly BoundExpression _ValueTypeReceiver;

		private readonly BoundExpression _ReferenceTypeReceiver;

		public BoundExpression ValueTypeReceiver => _ValueTypeReceiver;

		public BoundExpression ReferenceTypeReceiver => _ReferenceTypeReceiver;

		public BoundComplexConditionalAccessReceiver(SyntaxNode syntax, BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ComplexConditionalAccessReceiver, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(valueTypeReceiver) || BoundNodeExtensions.NonNullAndHasErrors(referenceTypeReceiver))
		{
			_ValueTypeReceiver = valueTypeReceiver;
			_ReferenceTypeReceiver = referenceTypeReceiver;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitComplexConditionalAccessReceiver(this);
		}

		public BoundComplexConditionalAccessReceiver Update(BoundExpression valueTypeReceiver, BoundExpression referenceTypeReceiver, TypeSymbol type)
		{
			if (valueTypeReceiver != ValueTypeReceiver || referenceTypeReceiver != ReferenceTypeReceiver || (object)type != base.Type)
			{
				BoundComplexConditionalAccessReceiver boundComplexConditionalAccessReceiver = new BoundComplexConditionalAccessReceiver(base.Syntax, valueTypeReceiver, referenceTypeReceiver, type, base.HasErrors);
				boundComplexConditionalAccessReceiver.CopyAttributes(this);
				return boundComplexConditionalAccessReceiver;
			}
			return this;
		}
	}
}
