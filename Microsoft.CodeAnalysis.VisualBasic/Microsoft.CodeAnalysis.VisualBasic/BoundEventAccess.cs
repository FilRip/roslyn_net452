using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundEventAccess : BoundExpression
	{
		private readonly BoundExpression _ReceiverOpt;

		private readonly EventSymbol _EventSymbol;

		public override Symbol ExpressionSymbol => EventSymbol;

		public BoundExpression ReceiverOpt => _ReceiverOpt;

		public EventSymbol EventSymbol => _EventSymbol;

		public BoundEventAccess(SyntaxNode syntax, BoundExpression receiverOpt, EventSymbol eventSymbol, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.EventAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiverOpt))
		{
			_ReceiverOpt = receiverOpt;
			_EventSymbol = eventSymbol;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitEventAccess(this);
		}

		public BoundEventAccess Update(BoundExpression receiverOpt, EventSymbol eventSymbol, TypeSymbol type)
		{
			if (receiverOpt != ReceiverOpt || (object)eventSymbol != EventSymbol || (object)type != base.Type)
			{
				BoundEventAccess boundEventAccess = new BoundEventAccess(base.Syntax, receiverOpt, eventSymbol, type, base.HasErrors);
				boundEventAccess.CopyAttributes(this);
				return boundEventAccess;
			}
			return this;
		}
	}
}
