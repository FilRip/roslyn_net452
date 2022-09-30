using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLValueToRValueWrapper : BoundExpression
	{
		private readonly BoundExpression _UnderlyingLValue;

		public BoundExpression UnderlyingLValue => _UnderlyingLValue;

		public BoundLValueToRValueWrapper(SyntaxNode syntax, BoundExpression underlyingLValue, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.LValueToRValueWrapper, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(underlyingLValue))
		{
			_UnderlyingLValue = underlyingLValue;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLValueToRValueWrapper(this);
		}

		public BoundLValueToRValueWrapper Update(BoundExpression underlyingLValue, TypeSymbol type)
		{
			if (underlyingLValue != UnderlyingLValue || (object)type != base.Type)
			{
				BoundLValueToRValueWrapper boundLValueToRValueWrapper = new BoundLValueToRValueWrapper(base.Syntax, underlyingLValue, type, base.HasErrors);
				boundLValueToRValueWrapper.CopyAttributes(this);
				return boundLValueToRValueWrapper;
			}
			return this;
		}
	}
}
