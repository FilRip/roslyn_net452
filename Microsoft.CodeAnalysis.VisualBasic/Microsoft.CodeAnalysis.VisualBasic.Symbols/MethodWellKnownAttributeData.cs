namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class MethodWellKnownAttributeData : CommonMethodWellKnownAttributeData
	{
		private bool _hasSTAThreadAttribute;

		private bool _hasMTAThreadAttribute;

		private bool _isPropertyAccessorWithDebuggerHiddenAttribute;

		internal bool HasSTAThreadAttribute
		{
			get
			{
				return _hasSTAThreadAttribute;
			}
			set
			{
				_hasSTAThreadAttribute = value;
			}
		}

		internal bool HasMTAThreadAttribute
		{
			get
			{
				return _hasMTAThreadAttribute;
			}
			set
			{
				_hasMTAThreadAttribute = value;
			}
		}

		internal bool IsPropertyAccessorWithDebuggerHiddenAttribute
		{
			get
			{
				return _isPropertyAccessorWithDebuggerHiddenAttribute;
			}
			set
			{
				_isPropertyAccessorWithDebuggerHiddenAttribute = value;
			}
		}

		public MethodWellKnownAttributeData()
			: base(preserveSigFirstWriteWins: true)
		{
			_hasSTAThreadAttribute = false;
			_hasMTAThreadAttribute = false;
			_isPropertyAccessorWithDebuggerHiddenAttribute = false;
		}
	}
}
