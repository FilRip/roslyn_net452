namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MethodEarlyWellKnownAttributeData : CommonMethodEarlyWellKnownAttributeData
	{
		private bool _isExtensionMethod;

		internal bool IsExtensionMethod
		{
			get
			{
				return _isExtensionMethod;
			}
			set
			{
				_isExtensionMethod = value;
			}
		}

		public MethodEarlyWellKnownAttributeData()
		{
			_isExtensionMethod = false;
		}
	}
}
