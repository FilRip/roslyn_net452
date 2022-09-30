using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundAddHandlerStatement : BoundAddRemoveHandlerStatement
	{
		public BoundAddHandlerStatement(SyntaxNode syntax, BoundExpression eventAccess, BoundExpression handler, bool hasErrors = false)
			: base(BoundKind.AddHandlerStatement, syntax, eventAccess, handler, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(eventAccess) || BoundNodeExtensions.NonNullAndHasErrors(handler))
		{
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitAddHandlerStatement(this);
		}

		public BoundAddHandlerStatement Update(BoundExpression eventAccess, BoundExpression handler)
		{
			if (eventAccess != base.EventAccess || handler != base.Handler)
			{
				BoundAddHandlerStatement boundAddHandlerStatement = new BoundAddHandlerStatement(base.Syntax, eventAccess, handler, base.HasErrors);
				boundAddHandlerStatement.CopyAttributes(this);
				return boundAddHandlerStatement;
			}
			return this;
		}
	}
}
