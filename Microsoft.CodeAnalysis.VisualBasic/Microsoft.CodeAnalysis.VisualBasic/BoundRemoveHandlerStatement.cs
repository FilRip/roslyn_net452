using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundRemoveHandlerStatement : BoundAddRemoveHandlerStatement
	{
		public BoundRemoveHandlerStatement(SyntaxNode syntax, BoundExpression eventAccess, BoundExpression handler, bool hasErrors = false)
			: base(BoundKind.RemoveHandlerStatement, syntax, eventAccess, handler, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(eventAccess) || BoundNodeExtensions.NonNullAndHasErrors(handler))
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitRemoveHandlerStatement(this);
		}

		public BoundRemoveHandlerStatement Update(BoundExpression eventAccess, BoundExpression handler)
		{
			if (eventAccess != base.EventAccess || handler != base.Handler)
			{
				BoundRemoveHandlerStatement boundRemoveHandlerStatement = new BoundRemoveHandlerStatement(base.Syntax, eventAccess, handler, base.HasErrors);
				boundRemoveHandlerStatement.CopyAttributes(this);
				return boundRemoveHandlerStatement;
			}
			return this;
		}
	}
}
