using System.Collections.Immutable;
using System.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic.Symbols;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRaiseEventStatement : BoundStatement, IBoundInvalidNode
	{
		private readonly EventSymbol _EventSymbol;

		private readonly BoundExpression _EventInvocation;

		private ImmutableArray<BoundNode> IBoundInvalidNode_InvalidNodeChildren => ImmutableArray.Create((BoundNode)EventInvocation);

		public EventSymbol EventSymbol => _EventSymbol;

		public BoundExpression EventInvocation => _EventInvocation;

		public BoundRaiseEventStatement(SyntaxNode syntax, EventSymbol eventSymbol, BoundExpression eventInvocation, bool hasErrors = false)
			: base(BoundKind.RaiseEventStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(eventInvocation))
		{
			_EventSymbol = eventSymbol;
			_EventInvocation = eventInvocation;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRaiseEventStatement(this);
		}

		public BoundRaiseEventStatement Update(EventSymbol eventSymbol, BoundExpression eventInvocation)
		{
			if ((object)eventSymbol != EventSymbol || eventInvocation != EventInvocation)
			{
				BoundRaiseEventStatement boundRaiseEventStatement = new BoundRaiseEventStatement(base.Syntax, eventSymbol, eventInvocation, base.HasErrors);
				boundRaiseEventStatement.CopyAttributes(this);
				return boundRaiseEventStatement;
			}
			return this;
		}
	}
}
