namespace Microsoft.CodeAnalysis.VisualBasic
{
	public struct ForEachStatementInfo
	{
		public IMethodSymbol GetEnumeratorMethod { get; }

		public IMethodSymbol MoveNextMethod { get; }

		public IPropertySymbol CurrentProperty { get; }

		public IMethodSymbol DisposeMethod { get; }

		public ITypeSymbol ElementType { get; }

		public Conversion ElementConversion { get; }

		public Conversion CurrentConversion { get; }

		internal ForEachStatementInfo(IMethodSymbol getEnumeratorMethod, IMethodSymbol moveNextMethod, IPropertySymbol currentProperty, IMethodSymbol disposeMethod, ITypeSymbol elementType, Conversion elementConversion, Conversion currentConversion)
		{
			this = default(ForEachStatementInfo);
			GetEnumeratorMethod = getEnumeratorMethod;
			MoveNextMethod = moveNextMethod;
			CurrentProperty = currentProperty;
			DisposeMethod = disposeMethod;
			ElementType = elementType;
			ElementConversion = elementConversion;
			CurrentConversion = currentConversion;
		}
	}
}
