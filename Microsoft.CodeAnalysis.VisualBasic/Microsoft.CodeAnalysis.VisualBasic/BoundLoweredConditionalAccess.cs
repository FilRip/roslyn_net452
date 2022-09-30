using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundLoweredConditionalAccess : BoundExpression
	{
		private readonly BoundExpression _ReceiverOrCondition;

		private readonly bool _CaptureReceiver;

		private readonly int _PlaceholderId;

		private readonly BoundExpression _WhenNotNull;

		private readonly BoundExpression _WhenNullOpt;

		public BoundExpression ReceiverOrCondition => _ReceiverOrCondition;

		public bool CaptureReceiver => _CaptureReceiver;

		public int PlaceholderId => _PlaceholderId;

		public BoundExpression WhenNotNull => _WhenNotNull;

		public BoundExpression WhenNullOpt => _WhenNullOpt;

		public BoundLoweredConditionalAccess(SyntaxNode syntax, BoundExpression receiverOrCondition, bool captureReceiver, int placeholderId, BoundExpression whenNotNull, BoundExpression whenNullOpt, TypeSymbol type, bool hasErrors = false)
			: base(BoundKind.LoweredConditionalAccess, syntax, type, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(receiverOrCondition) || BoundNodeExtensions.NonNullAndHasErrors(whenNotNull) || BoundNodeExtensions.NonNullAndHasErrors(whenNullOpt))
		{
			_ReceiverOrCondition = receiverOrCondition;
			_CaptureReceiver = captureReceiver;
			_PlaceholderId = placeholderId;
			_WhenNotNull = whenNotNull;
			_WhenNullOpt = whenNullOpt;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitLoweredConditionalAccess(this);
		}

		public BoundLoweredConditionalAccess Update(BoundExpression receiverOrCondition, bool captureReceiver, int placeholderId, BoundExpression whenNotNull, BoundExpression whenNullOpt, TypeSymbol type)
		{
			if (receiverOrCondition != ReceiverOrCondition || captureReceiver != CaptureReceiver || placeholderId != PlaceholderId || whenNotNull != WhenNotNull || whenNullOpt != WhenNullOpt || (object)type != base.Type)
			{
				BoundLoweredConditionalAccess boundLoweredConditionalAccess = new BoundLoweredConditionalAccess(base.Syntax, receiverOrCondition, captureReceiver, placeholderId, whenNotNull, whenNullOpt, type, base.HasErrors);
				boundLoweredConditionalAccess.CopyAttributes(this);
				return boundLoweredConditionalAccess;
			}
			return this;
		}
	}
}
