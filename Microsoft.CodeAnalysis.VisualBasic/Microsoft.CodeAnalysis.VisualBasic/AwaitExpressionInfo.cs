namespace Microsoft.CodeAnalysis.VisualBasic
{
	public struct AwaitExpressionInfo
	{
		private readonly IMethodSymbol _getAwaiter;

		private readonly IPropertySymbol _isCompleted;

		private readonly IMethodSymbol _getResult;

		public IMethodSymbol GetAwaiterMethod => _getAwaiter;

		public IMethodSymbol GetResultMethod => _getResult;

		public IPropertySymbol IsCompletedProperty => _isCompleted;

		internal AwaitExpressionInfo(IMethodSymbol getAwaiter, IPropertySymbol isCompleted, IMethodSymbol getResult)
		{
			this = default(AwaitExpressionInfo);
			_getAwaiter = getAwaiter;
			_isCompleted = isCompleted;
			_getResult = getResult;
		}
	}
}
