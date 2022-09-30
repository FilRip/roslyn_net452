namespace Microsoft.CodeAnalysis.VisualBasic.Symbols
{
	internal class TypeEarlyWellKnownAttributeData : CommonTypeEarlyWellKnownAttributeData
	{
		private bool _hasVisualBasicEmbeddedAttribute;

		private bool _hasAttributeForExtensibleInterface;

		internal bool HasVisualBasicEmbeddedAttribute
		{
			get
			{
				return _hasVisualBasicEmbeddedAttribute;
			}
			set
			{
				_hasVisualBasicEmbeddedAttribute = value;
			}
		}

		internal bool HasAttributeForExtensibleInterface
		{
			get
			{
				return _hasAttributeForExtensibleInterface;
			}
			set
			{
				_hasAttributeForExtensibleInterface = value;
			}
		}

		public TypeEarlyWellKnownAttributeData()
		{
			_hasVisualBasicEmbeddedAttribute = false;
			_hasAttributeForExtensibleInterface = false;
		}
	}
}
