using System.Diagnostics;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class BoundSyncLockStatement : BoundStatement
	{
		private readonly BoundExpression _LockExpression;

		private readonly BoundBlock _Body;

		public BoundExpression LockExpression => _LockExpression;

		public BoundBlock Body => _Body;

		public BoundSyncLockStatement(SyntaxNode syntax, BoundExpression lockExpression, BoundBlock body, bool hasErrors = false)
			: base(BoundKind.SyncLockStatement, syntax, hasErrors || BoundNodeExtensions.NonNullAndHasErrors(lockExpression) || BoundNodeExtensions.NonNullAndHasErrors(body))
		{
			_LockExpression = lockExpression;
			_Body = body;
		}

		[DebuggerStepThrough]
		public override BoundNode Accept(BoundTreeVisitor visitor)
		{
			return visitor.VisitSyncLockStatement(this);
		}

		public BoundSyncLockStatement Update(BoundExpression lockExpression, BoundBlock body)
		{
			if (lockExpression != LockExpression || body != Body)
			{
				BoundSyncLockStatement boundSyncLockStatement = new BoundSyncLockStatement(base.Syntax, lockExpression, body, base.HasErrors);
				boundSyncLockStatement.CopyAttributes(this);
				return boundSyncLockStatement;
			}
			return this;
		}
	}
}
