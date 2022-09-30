namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal abstract class BoundAddRemoveHandlerStatement : BoundStatement
	{
		private readonly BoundExpression _EventAccess;

		private readonly BoundExpression _Handler;

		public BoundExpression EventAccess => _EventAccess;

		public BoundExpression Handler => _Handler;

		protected BoundAddRemoveHandlerStatement(BoundKind kind, SyntaxNode syntax, BoundExpression eventAccess, BoundExpression handler, bool hasErrors = false)
			: base(kind, syntax, hasErrors)
		{
			_EventAccess = eventAccess;
			_Handler = handler;
		}
	}
}
