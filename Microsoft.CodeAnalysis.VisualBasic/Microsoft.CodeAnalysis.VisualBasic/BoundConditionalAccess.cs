using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundConditionalAccess : BoundExpression
	{
		private readonly BoundExpression _Receiver;

		private readonly BoundRValuePlaceholder _Placeholder;

		private readonly BoundExpression _AccessExpression;

		public BoundExpression Receiver => _Receiver;

		public BoundRValuePlaceholder Placeholder => _Placeholder;

		public BoundExpression AccessExpression => _AccessExpression;

		public BoundConditionalAccess(SyntaxNode syntax, BoundExpression receiver, BoundRValuePlaceholder placeholder, BoundExpression accessExpression, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.ConditionalAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiver) || BoundNodeExtensions.NonNullAndHasErrors(placeholder) || BoundNodeExtensions.NonNullAndHasErrors(accessExpression))
		{
			_Receiver = receiver;
			_Placeholder = placeholder;
			_AccessExpression = accessExpression;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitConditionalAccess(this);
		}

		public BoundConditionalAccess Update(BoundExpression receiver, BoundRValuePlaceholder placeholder, BoundExpression accessExpression, TypeSymbol type)
		{
			if (receiver != Receiver || placeholder != Placeholder || accessExpression != AccessExpression || (object)type != base.Type)
			{
				BoundConditionalAccess boundConditionalAccess = new BoundConditionalAccess(base.Syntax, receiver, placeholder, accessExpression, type, base.HasErrors);
				boundConditionalAccess.CopyAttributes(this);
				return boundConditionalAccess;
			}
			return this;
		}
	}
}
