using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace Microsoft.CodeAnalysis.VisualBasic
{
	internal sealed class ConditionalAccessBinder : Binder
	{
		private readonly ConditionalAccessExpressionSyntax _conditionalAccess;

		private readonly BoundValuePlaceholderBase _placeholder;

		public ConditionalAccessBinder(Binder containingBinder, ConditionalAccessExpressionSyntax conditionalAccess, BoundValuePlaceholderBase placeholder)
			: base(containingBinder)
		{
			_conditionalAccess = conditionalAccess;
			_placeholder = placeholder;
		}

		protected override BoundExpression TryGetConditionalAccessReceiver(ConditionalAccessExpressionSyntax node)
		{
			if (node == _conditionalAccess)
			{
				return _placeholder;
			}
			return base.TryGetConditionalAccessReceiver(node);
		}
	}
}
